using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

namespace ECSCity
{

    public class InfectionUpdateBarrier : BarrierSystem { }


    public class InfectionKillingSystem : JobComponentSystem
    {

        struct InfectedData
        {
            [ReadOnly] public EntityArray infectedEntities;

            public ComponentDataArray<InfectedTag> infectionData;


        };


        struct updateInfectionJob : IJobParallelFor
        {
            [ReadOnly] public EntityArray infected;
            public ComponentDataArray<InfectedTag> infectedData;
            public EntityCommandBuffer.Concurrent commandBuffer;
            [ReadOnly] public float dt;
            [ReadOnly] public int timeScale;

            public void Execute(int index)
            {
               
                infectedData[index] = new InfectedTag { Cooldown = infectedData[index].Cooldown - dt * timeScale };

                if (infectedData[index].Cooldown <= 0)
                {
                    commandBuffer.DestroyEntity(infected[index]);
                }

            }
        }

        [Inject] InfectedData m_Infected;

        [Inject] InfectionUpdateBarrier updateBarrier;




        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            return new updateInfectionJob
            {
                infected = m_Infected.infectedEntities,
                infectedData = m_Infected.infectionData,
                commandBuffer = updateBarrier.CreateCommandBuffer(),
                dt = Time.deltaTime,
                timeScale = ECSBootstrapper.timeScale

            }.Schedule(m_Infected.infectedEntities.Length,64,inputDeps);

        }



    }


}