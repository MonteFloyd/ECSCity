using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

namespace ECSCity
{
    public class WandererNavigationSystem : JobComponentSystem
    {

        public class AddBoidBarrier : BarrierSystem
        { }

        struct WandererData
        {
            [ReadOnly] public EntityArray Instances;
            //[ReadOnly] public SharedComponentDataArray<WorkerSchedule> workerSchedules;
            //[ReadOnly] public ComponentDataArray<Position> humanPositions;
            [ReadOnly] public ComponentDataArray<HomePosition> homePositions;
            [ReadOnly] public SubtractiveComponent<BoidTag> ignore;
            [ReadOnly] public ComponentDataArray<Wanderer> tag;
            public ComponentDataArray<Target> Targets;
        }

        [Inject] WandererData m_WandererData;
        [Inject] AddBoidBarrier addBoidBuffer;
        private int savedTime;
        private int maxX;
        private int minX;
        private int maxZ;
        private int minZ;



        public struct setTargetJob : IJobParallelFor
        {
            public ComponentDataArray<Target> target;
            //[ReadOnly] public SharedComponentDataArray<WorkerSchedule> workerSchedules;
            [ReadOnly] public ComponentDataArray<HomePosition> homePosition;
            //[ReadOnly] public ComponentDataArray<WorkPosition> workPosition;

            [DeallocateOnJobCompletion]
            [ReadOnly] public NativeArray<float3> randomPositions;


            [ReadOnly] public EntityArray Instance;
            public EntityCommandBuffer.Concurrent commandBuffer;

            public int currTime;

            public void Execute(int index)
            {
                float3 newPos = math.select(homePosition[index].Value, randomPositions[index], (currTime > 8 && currTime < 19));

                if (target[index].Value.x != newPos.x || target[index].Value.z != newPos.z)
                {
                    //commandBuffer.AddComponent(Instance[index], default(BoidTag));
                    target[index] = new Target { Value = newPos };
                }
                //commandBuffer.AddComponent(Instances[index], default(BoidTag));
                //targets[index] = new Target { Value = workerSchedules[index].Value[currentTime].Value };

                //workerSchedules[index].Value[currentTime].Value


            }
        }


        protected override void OnStartRunning()
        {
            savedTime = 0;

            maxX = ECSBootstrapper.maxX;
            minX = ECSBootstrapper.minX;

            maxZ = ECSBootstrapper.maxZ;
            minZ = ECSBootstrapper.minZ;


        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            int currentTime = Mathf.FloorToInt(DayNightSystem.worldTime);

            if (currentTime != savedTime)
            {
                var newPositions = new NativeArray<float3>(m_WandererData.Instances.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                for(int i = 0; i< newPositions.Length; ++i)
                {
                    newPositions[i] = new float3(Random.Range(minX,maxX ), ECSBootstrapper.humanPositionY , Random.Range(minZ, maxZ));
                }
                var SetTargetJob = new setTargetJob
                {
                    Instance = m_WandererData.Instances,
                    target = m_WandererData.Targets,
                    homePosition = m_WandererData.homePositions,
                    randomPositions = newPositions,
                    currTime = currentTime,
                    commandBuffer = addBoidBuffer.CreateCommandBuffer()

                }.Schedule(m_WandererData.Targets.Length, 64, inputDeps);

                savedTime = currentTime;
                newPositions.Dispose();
                return SetTargetJob;
            }
            else
            {
                return inputDeps;
            }
        }
    }






}


