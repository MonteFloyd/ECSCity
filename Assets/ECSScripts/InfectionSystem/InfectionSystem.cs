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



    public class InfectionSystem : JobComponentSystem
    {

        public class InfectionBarrier : BarrierSystem { };

        struct InfectedData
        {
            [ReadOnly]
            public ComponentDataArray<InfectedTag> useless;



        };

        struct HumanData
        {
            [ReadOnly]
            public EntityArray entities;

            [ReadOnly]
            public ComponentDataArray<Human> tag;

            [ReadOnly]
            public SubtractiveComponent<InfectedTag> useless;


        };


        struct Infection
        {
            [ReadOnly]
            public ComponentDataArray<InfectionSetting> setting;
        };

        [Inject] InfectedData m_Infected;

        [Inject] HumanData m_Dummies;

        [Inject] Infection m_InfectionSetting;

        [Inject] InfectionBarrier infectBarrier;

        private bool dailyCheck = false;


        struct InfectDummiesJob : IJobParallelFor
        {
            [ReadOnly]
            public EntityArray Infectee;


            public EntityCommandBuffer.Concurrent commandBuffer;

            public void Execute(int index)
            {
                commandBuffer.AddComponent(Infectee[index], new InfectedTag { Cooldown = ECSBootstrapper.infectionKillingDay });
                commandBuffer.SetSharedComponent(Infectee[index], ECSBootstrapper.humanSickRenderer);
            }
        };


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (DayNightSystem.worldTime < 6) dailyCheck = true;

            if (m_InfectionSetting.setting.Length != 0 && dailyCheck && DayNightSystem.worldTime > 6)
            {
                
                int dummyLength = m_Dummies.entities.Length;
                int infectedLength = math.max(1,m_Infected.useless.Length);
                int numberOfInfections = math.min(infectedLength * m_InfectionSetting.setting[0].infectionRate, dummyLength);
                //Debug.Log(m_Infected.useless.Length * m_InfectionSetting.setting[0].infectionRate / dummyLength);


                var infectHandle = new InfectDummiesJob
                {
                    Infectee = m_Dummies.entities,
                    commandBuffer = infectBarrier.CreateCommandBuffer()
                }.Schedule(numberOfInfections, 64, inputDeps);
                dailyCheck = false;

                return infectHandle;

            }
            else
            {
                return inputDeps;
            }


        }


    }





}
