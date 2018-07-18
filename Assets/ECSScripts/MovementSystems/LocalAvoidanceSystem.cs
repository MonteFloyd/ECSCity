using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Mathematics.Experimental;
using Samples.Common;
using UnityEngine;

namespace ECSCity {

    //[UpdateBefore(typeof(BoidStripSystem))]
    //[UpdateAfter(typeof(HumanNavigationSystem))]
    [UpdateBefore(typeof(TransformInputBarrier))]
    public class LocalAvoidanceSystem : JobComponentSystem
    {

        struct ObstacleData
        {
            [ReadOnly] ComponentDataArray<BoidObstacle> obstacleTag;
            [ReadOnly] public ComponentDataArray<Position> obstaclePositions;

        };

        struct BoidData
        {
            [ReadOnly] public ComponentDataArray<BoidTag> boidTag;
            [ReadOnly] public ComponentDataArray<Position> boidPositions;
            [ReadOnly] public ComponentDataArray<Target> boidTarget;
                       public ComponentDataArray<Heading> boidHeadings;
            [ReadOnly] public SubtractiveComponent<GravityType> ignore;

        };

        [Inject] ObstacleData m_Obstacles;
        [Inject] BoidData m_Boids;


        [BurstCompile]
        public struct CopyTargetPositions : IJobParallelFor
        {
            [ReadOnly] public ComponentDataArray<Target> Source;
            public NativeArray<Position> Results;
            public void Execute(int index)
            {
                Results[index] = new Position { Value = Source[index].Value };
            }
        }


        [BurstCompile]
        struct HashPositions : IJobParallelFor
        {
            [ReadOnly] public ComponentDataArray<Position> positions;
            public NativeMultiHashMap<int, int>.Concurrent hashMap;
            public float cellRadius;

            public void Execute(int index)
            {
                var hash = GridHash.Hash(new float2(positions[index].Value.x, positions[index].Value.z), cellRadius);
                hashMap.Add(hash, index);
            }
        }

        [BurstCompile]
        struct MergeCells : IJobNativeMultiHashMapMergedSharedKeyIndices
        {
            public NativeArray<int> cellIndices;
            public NativeArray<Position> cellSeparation;
            public NativeArray<int> cellObstaclePositionIndex;
            public NativeArray<float> cellObstacleDistance;
            public NativeArray<int> cellCount;
            [ReadOnly] public NativeArray<Position> obstaclePositions;

            void NearestPosition(NativeArray<Position> targets, float3 position, out int nearestPositionIndex, out float nearestDistance)
            {
                nearestPositionIndex = 0;
                nearestDistance = math.lengthSquared(position - targets[0].Value);
                for (int i = 1; i < targets.Length; i++)
                {
                    var targetPosition = targets[i].Value;
                    var distance = math.lengthSquared(position - targetPosition);
                    var nearest = distance < nearestDistance;

                    nearestDistance = math.select(nearestDistance, distance, nearest);
                    nearestPositionIndex = math.select(nearestPositionIndex, i, nearest);
                }
                nearestDistance = math.sqrt(nearestDistance);
            }

            public void ExecuteFirst(int index)
            {
                var position = cellSeparation[index].Value / cellCount[index];

                int obstaclePositionIndex;
                float obstacleDistance;
                NearestPosition(obstaclePositions, position, out obstaclePositionIndex, out obstacleDistance);
                cellObstaclePositionIndex[index] = obstaclePositionIndex;
                cellObstacleDistance[index] = obstacleDistance;

                cellIndices[index] = index;
            }

            public void ExecuteNext(int cellIndex, int index)
            {
                cellCount[cellIndex] += 1;
                cellSeparation[cellIndex] = new Position { Value = cellSeparation[cellIndex].Value + cellSeparation[index].Value };
                cellIndices[index] = cellIndex;
            }
        }




        [BurstCompile]
        struct Steer : IJobParallelFor
        {
            [ReadOnly] public NativeArray<int> cellIndices;
            [ReadOnly] public Boid settings;
            [ReadOnly] public NativeArray<Position> targetPositions;
            [ReadOnly] public NativeArray<Position> obstaclePositions;
            [ReadOnly] public NativeArray<Position> cellSeparation;
            [ReadOnly] public NativeArray<int> cellObstaclePositionIndex;
            [ReadOnly] public NativeArray<float> cellObstacleDistance;
            [ReadOnly] public NativeArray<int> cellCount;
            [ReadOnly] public ComponentDataArray<Position> positions;
            public float dt;
            public ComponentDataArray<Heading> headings;

            public void Execute(int index)
            {
                var forward = headings[index].Value;
                var position = positions[index].Value;
                var cellIndex = cellIndices[index];
                var neighborCount = cellCount[cellIndex];
                var separation = cellSeparation[cellIndex].Value;
                var nearestObstacleDistance = cellObstacleDistance[cellIndex];
                var nearestObstaclePositionIndex = cellObstaclePositionIndex[cellIndex];
                var nearestObstaclePosition = obstaclePositions[nearestObstaclePositionIndex].Value;
                var nearestTargetPosition = targetPositions[index].Value;

                var obstacleSteering = position - nearestObstaclePosition;
                var avoidObstacleHeading = (nearestObstaclePosition + math_experimental.normalizeSafe(obstacleSteering)
                                                        * settings.obstacleAversionDistance) - position;
                var targetHeading = settings.targetWeight
                                                        * math_experimental.normalizeSafe(nearestTargetPosition - position);
                var nearestObstacleDistanceFromRadius = nearestObstacleDistance - settings.obstacleAversionDistance;
                var separationResult = settings.separationWeight
                                                        * math_experimental.normalizeSafe((position * neighborCount) - separation);
                var normalHeading = math_experimental.normalizeSafe(separationResult + targetHeading);
                var targetForward = math.select(normalHeading, avoidObstacleHeading, nearestObstacleDistanceFromRadius < 0);
                var nextHeading = math_experimental.normalizeSafe(forward + dt * (targetForward - forward));

                nextHeading.y = 0;
                headings[index] = new Heading { Value = nextHeading };
            }
        }



        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            var boidCount = m_Boids.boidTag.Length;

            var cellIndices = new NativeArray<int>(boidCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var hashMap = new NativeMultiHashMap<int, int>(boidCount, Allocator.TempJob);
            var copyTargetPositions = new NativeArray<Position>(m_Boids.boidTarget.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var copyObstaclePositions = new NativeArray<Position>(m_Obstacles.obstaclePositions.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var cellSeparation = new NativeArray<Position>(boidCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var cellObstacleDistance = new NativeArray<float>(boidCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var cellObstaclePositionIndex = new NativeArray<int>(boidCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var cellCount = new NativeArray<int>(boidCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            
            

            var hashPositionsJob = new HashPositions
            {
                positions = m_Boids.boidPositions,
                hashMap = hashMap,
                cellRadius = ECSBootstrapper.boidSettings.cellRadius
            };
            var hashPositionsJobHandle = hashPositionsJob.Schedule(boidCount, 64, inputDeps);

            var initialCellSeparationJob = new CopyComponentData<Position>
            {
                Source = m_Boids.boidPositions,
                Results = cellSeparation
            };
            var initialCellSeparationJobHandle = initialCellSeparationJob.Schedule(boidCount, 64, inputDeps);

            var initialCellCountJob = new MemsetNativeArray<int>
            {
                Source = cellCount,
                Value = 1
            };
            var initialCellCountJobHandle = initialCellCountJob.Schedule(boidCount, 64, inputDeps);

            var initialCellBarrierJobHandle = JobHandle.CombineDependencies( initialCellSeparationJobHandle, initialCellCountJobHandle);

            var copyTargetPositionJob = new CopyTargetPositions
            {
                Source = m_Boids.boidTarget,
                Results = copyTargetPositions
            };

            var copyTargetPositionsJobHandle = copyTargetPositionJob.Schedule(m_Boids.boidTarget.Length, 2, inputDeps);

            var copyObstaclePositionsJob = new CopyComponentData<Position>
            {
                Source = m_Obstacles.obstaclePositions,
                Results = copyObstaclePositions
            };
            var copyObstaclePositionsJobHandle = copyObstaclePositionsJob.Schedule(m_Obstacles.obstaclePositions.Length, 2, inputDeps);

            var copyTargetObstacleBarrierJobHandle = JobHandle.CombineDependencies(copyTargetPositionsJobHandle, copyObstaclePositionsJobHandle);

            var mergeCellsBarrierJobHandle = JobHandle.CombineDependencies(hashPositionsJobHandle, initialCellBarrierJobHandle, copyTargetObstacleBarrierJobHandle);

            var mergeCellsJob = new MergeCells
            {
                cellIndices = cellIndices,
                cellSeparation = cellSeparation,
                cellObstacleDistance = cellObstacleDistance,
                cellObstaclePositionIndex = cellObstaclePositionIndex,
                cellCount = cellCount,
                obstaclePositions = copyObstaclePositions
            };
            var mergeCellsJobHandle = mergeCellsJob.Schedule(hashMap, 64, mergeCellsBarrierJobHandle);

            var steerJob = new Steer
            {
                cellIndices = cellIndices,  //D
                settings = ECSBootstrapper.boidSettings,
                cellSeparation = cellSeparation,
                cellObstacleDistance = cellObstacleDistance,
                cellObstaclePositionIndex = cellObstaclePositionIndex,
                cellCount = cellCount,
                targetPositions = copyTargetPositions,
                obstaclePositions = copyObstaclePositions,
                dt = Time.deltaTime,
                positions = m_Boids.boidPositions,
                headings = m_Boids.boidHeadings,
            };
            var steerJobHandle = steerJob.Schedule(boidCount, 64, mergeCellsJobHandle);

            steerJobHandle.Complete();


            cellIndices.Dispose();
            hashMap.Dispose();
            copyTargetPositions.Dispose();
            copyObstaclePositions.Dispose();
            cellSeparation.Dispose();
            cellObstacleDistance.Dispose();
            cellObstaclePositionIndex.Dispose();
            cellCount.Dispose();


            return inputDeps;


        }

        //protected override 

        //protected override void OnStopRunning()
        //{
        //    disposalList.cellIndices.Dispose();
        //    disposalList.hashMap.Dispose();
        //    disposalList.copyTargetPositions.Dispose();
        //    disposalList.copyObstaclePositions.Dispose();
        //    disposalList.cellSeparation.Dispose();
        //    disposalList.cellObstacleDistance.Dispose();
        //    disposalList.cellObstaclePositionIndex.Dispose();
        //    disposalList.cellCount.Dispose();

        //}


    }







    }