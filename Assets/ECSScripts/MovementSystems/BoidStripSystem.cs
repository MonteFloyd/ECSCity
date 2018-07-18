using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;

namespace ECSCity
{


    public class BoidStripSystem : JobComponentSystem
    {
        public class BoidStripBarrier : BarrierSystem
        { }


        struct HumanBoidData
        {
            [ReadOnly] public EntityArray Instances;
            public ComponentDataArray<BoidTag> boidLabel;
            [ReadOnly] public ComponentDataArray<Position> humanPositions;
            [ReadOnly] public ComponentDataArray<Target> Targets;
            [ReadOnly] public SubtractiveComponent<GravityType> ignore;

        }

        [Inject] HumanBoidData m_humanBoidData;
        [Inject] public BoidStripBarrier m_boidStripBarrier;

        
        public struct BoidStripJob : IJobParallelFor
        {
            public EntityCommandBuffer.Concurrent commandBuffer;
            [ReadOnly] public EntityArray Boids;
            [ReadOnly] public ComponentDataArray<Position> boidPositions;
            [ReadOnly] public ComponentDataArray<Target> boidTargets;
            public float distanceTreshold;

            void IJobParallelFor.Execute(int index)
            {
                if (math.distance(boidPositions[index].Value,boidTargets[index].Value) < distanceTreshold)
                {
                    commandBuffer.RemoveComponent<BoidTag>(Boids[index]);
                }
            }

            //public void Execute()
            //{
            //    for (int index = 0; index < boidPositions.Length; ++index)
            //    {
            //        if (math.lengthSquared(boidPositions[index].Value - boidTargets[index].Value) < distanceTreshold)
            //        {
            //            commandBuffer.RemoveComponent<BoidTag>(Boids[index]);
            //        }
            //    }
            //}
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            
            //var removeBoidJobHandle = new BoidStripJob
            //{
            //    commandBuffer = m_boidStripBarrier.CreateCommandBuffer(),
            //    Boids = m_humanBoidData.Instances,
            //    boidPositions = m_humanBoidData.humanPositions,
            //    boidTargets = m_humanBoidData.Targets,
            //    distanceTreshold = ECSBootstrapper.boidDisableDistance

            //}.Schedule(inputDeps);

            var removeBoidJobHandle = new BoidStripJob
            {
                commandBuffer = m_boidStripBarrier.CreateCommandBuffer(),
                Boids = m_humanBoidData.Instances,
                boidPositions = m_humanBoidData.humanPositions,
                boidTargets = m_humanBoidData.Targets,
                distanceTreshold = ECSBootstrapper.boidDisableDistance

            }.Schedule(m_humanBoidData.Instances.Length,64,inputDeps);

            return removeBoidJobHandle;



            //EntityCommandBuffer buffer = m_boidStripBarrier.CreateCommandBuffer();
            
           
            //if (m_humanBoidData.Instances.Length != 0)
            //{
            //    for (int index = 0; index < m_humanBoidData.Instances.Length; ++index)
            //    {
            //        if (math.lengthSquared(m_humanBoidData.humanPositions[index].Value - m_humanBoidData.Targets[index].Value) < ECSBootstrapper.boidDisableDistance)
            //        {
            //            buffer.RemoveComponent<BoidTag>(m_humanBoidData.Instances[index]);
                            
            //                //RemoveComponent(m_humanBoidData.Instances[index], typeof(BoidTag));

            //            //<BoidTag>(m_humanBoidData.Instances[index]);
            //        }
            //    }
            //}
            //return inputDeps;


        }


    }



}