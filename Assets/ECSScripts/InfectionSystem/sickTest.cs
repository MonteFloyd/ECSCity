//using Unity.Entities;
//using Unity.Mathematics;
//using UnityEngine;
//using Unity.Rendering;
//using Unity.Transforms;
//using Unity.Transforms2D;
//using Unity.Collections;
//using Unity.Jobs;


//namespace ECSCity
//{
//    public class sickTest : JobComponentSystem
//    {


//        public struct Infected
//        {
//            [ReadOnly]public ComponentDataArray<InfectedTag> infected;

//        }

//        public struct Humans
//        {
//            public EntityArray Instances;
//            public ComponentDataArray<Human> Useless;
//            public SubtractiveComponent<InfectedTag> subtractive;

//        }

//        public class changeRendererBarrier : BarrierSystem { }


//        [Inject] public Humans m_Entities;
//        [Inject] public Infected m_infectedEntities;
//        [Inject] changeRendererBarrier m_RendererBarrier;
//        public int infectionRate;


//        struct sickHumanJob : IJob
//        {


//            [ReadOnly] public EntityArray Humans;
//            //[ReadOnly] public MeshInstanceRenderer sickHuman;
//            [ReadOnly] public int maxIndex;
//            [ReadOnly] public int coolDown;
//            public EntityCommandBuffer commandBuffer;

//            public void Execute()
//            {
//                for (int i = 0; i < maxIndex; ++i)
//                {
//                    commandBuffer.SetSharedComponent<MeshInstanceRenderer>(Humans[i], ECSBootstrapper.humanSickRenderer);
//                    commandBuffer.AddComponent<InfectedTag>(Humans[i], new InfectedTag { Cooldown = 0.001f } );
//                }

                



//            }
//        }


//        protected override void OnStartRunning()
//        {
//            infectionRate = ECSBootstrapper.infectionRate;
//        }

//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {

//            if (true)
//            {
//                int numberOfInfections = (m_infectedEntities.infected.Length * infectionRate);

//                return new sickHumanJob
//                {
//                    Humans = m_Entities.Instances,
//                    //sickHuman = ECSBootstrapper.humanSickRenderer,
//                    maxIndex = numberOfInfections,
//                    commandBuffer = m_RendererBarrier.CreateCommandBuffer()
//                }.Schedule(inputDeps);
//            } 

//            return inputDeps;
//        }


//    }

//}


