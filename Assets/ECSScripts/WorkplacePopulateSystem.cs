//using Unity.Transforms;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Collections;
//using Unity.Burst;
//using UnityEngine;
//using Unity.Mathematics;
//using Unity.Rendering;
//using Samples.Boids;

//namespace ECSCity
//{


//    public class WorkplacePopulateSystem : JobComponentSystem
//    {

//        public class HumanPlacementBarrier : BarrierSystem { }

//        public struct WorkplaceData
//        {
//            public EntityArray Instances;
//            public ComponentDataArray<OpenWorkplace> Useless;
//            public ComponentDataArray<Position> Positions;
//        }

//        public struct HomeData
//        {
//            public EntityArray Instances;
//            public ComponentDataArray<OpenHome> Useless;
//            public ComponentDataArray<Position> Positions;
//        }

//        public struct HumanData
//        {
//            public EntityArray Instances;
//            public ComponentDataArray<Human> Useless;
//            public SubtractiveComponent<WorkerSchedule> ignore;
//            public SubtractiveComponent<WandererSchedule> ignore2;

//        }

//        [Inject] public WorkplaceData m_Workplaces;
//        [Inject] public HumanData m_Humans;
//        [Inject] public HomeData m_Homes;

//        //ComponentGroup humanGroup;
//        //ComponentGroup workplaceGroup;
//        //ComponentGroup houseGroup;



//        //protected override void OnCreateManager(int capacity)
//        //{
//        //    humanGroup = GetComponentGroup(typeof(Entity),typeof(Human));

//        //    workplaceGroup = GetComponentGroup(typeof(Entity), typeof(WorkplaceJustInstanced),typeof(Position) );

//        //    houseGroup = GetComponentGroup(typeof(Entity),typeof(HouseJustInstanced), typeof(Position),)

//        //}

//        struct FillJob : IJob
//        {
//            public EntityArray Workplaces;
//            public EntityArray Homes;
//            public EntityArray Humans;

//            public NativeArray<Position> workplacePositions;
//            public NativeArray<Position> homePositions;

//            public void Execute()
//            {


//            }
//        }



//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            if(m_Workplaces.Instances.Length != 0 && m_Homes.Instances.Length != 0)
//            {

//                WorkerSchedule newSchedule = new WorkerSchedule { Value = new NativeArray<Position>(24,Allocator.Persistent) };

//                for (int i = 0; i < 24; ++i)
//                {
//                    newSchedule.Value[i] = m_Homes.Positions[0];
//                }
//                ECSBootstrapper.entityManager.RemoveComponent<OpenHome>(m_Homes.Instances[0]);

//                for (int i = 9; i < 16; ++i)
//                {
//                    newSchedule.Value[i] = m_Workplaces.Positions[0];
//                }

//                ECSBootstrapper.entityManager.RemoveComponent<OpenWorkplace>(m_Workplaces.Instances[0]);


//                for (int i = 0; i < m_Humans.Instances.Length; ++i)
//                {
//                    ECSBootstrapper.entityManager.AddSharedComponentData(m_Humans.Instances[i],newSchedule);
//                }

//            }


//            return inputDeps;

//        }

//    }


//}