//using System.Collections;
//using System.Collections.Generic;


//using UnityEngine;
//using Unity.Mathematics;
//using Unity.Mathematics.Experimental;
//using Unity.Transforms;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Collections;
//using Unity.Burst;


//namespace ECSCity
//{
//    public class HumanSpawnBarrier : BarrierSystem
//    { }

//    public class EntityDestroyBarrier : BarrierSystem
//    { }

//    public class HumanSpawningSystem : JobComponentSystem
//    {

//        struct Settings
//        {
//            [ReadOnly] public EntityArray Entity;
//            [ReadOnly] public ComponentDataArray<HumanSpawnerSettings> settings;
//            //[ReadOnly] public HumanSpawnerSettings settings;
//            //public NativeArray<Position> NewSpawnPositions;
//        }

//        [Inject] private Settings m_Settings;
//        [Inject] public HumanSpawnBarrier m_HumanSpawnBarrier;
//        [Inject] public EntityDestroyBarrier m_EntityDestroyBarrier;


//        //[BurstCompile]
//        //struct GenerateSpawnPointsJob : IJobParallelFor
//        //{
//        //    float CenterPoint;
//        //    int Amount;
//        //    public void Execute(int index)
//        //    {

//        //        //Random.value()
//        //    }

//        //}


//        //[BurstCompile]
//        struct SpawnHumans : IJobParallelFor
//        {
//            [ReadOnly]public Position CenterPosition;
//            [ReadOnly]public float Radius;
//            [NativeDisableParallelForRestriction] public EntityCommandBuffer commandBuffer;
            

            

//            public void Execute(int index)
//            {

//                commandBuffer.CreateEntity(ECSBootstrapper.testArchetype);
//                commandBuffer.SetComponent(default(Human));
//                //commandBuffer.SetComponent(new Position { Value = new float3(Random.value * Radius, Random.value * Radius, Random.value * Radius) });
//                //commandBuffer.SetComponent(new Heading { Value = new float3(1.0f, 0.0f, 1.0f) });
//                commandBuffer.AddSharedComponent(ECSBootstrapper.humanSickRenderer);


//            }

           

//        }



//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            //if (m_Settings.Entity.Length == 0)
//            //    return inputDeps;

            
//            ////var generateHandle = new GenerateSpawnPointJob()
//            ////{ };
//            //Debug.Log(m_Settings.Entity.Length);


//            ////entityTransaction.CreateEntity();

//            ////ECSBootstrapper.entityManager.DestroyEntity(m_Settings);

//            ////generateHandle.Schedule(Amount, 64, inputDeps);
//            //////Random.Range()
//            ////NativeArray<Position> m_Positions = new NativeArray<Position>(m_Settings.Length, Allocator.Temp);
//            //EntityCommandBuffer synch = m_EntityDestroyBarrier.CreateCommandBuffer();

//            ////var m_Instances = new NativeArray<Entity>(m_Settings.Entity.Length, Allocator.TempJob);
//            ////ECSBootstrapper.entityManager.CreateEntity(ECSBootstrapper.testArchetype, m_Instances);

//            //var job = new SpawnHumans
//            //{
//            //    CenterPosition = m_Settings.settings[0].SpawnCenterPosition,
//            //    Radius = m_Settings.settings[0].Radius,
//            //    commandBuffer = m_HumanSpawnBarrier.CreateCommandBuffer(),
//            //}.Schedule(m_Settings.settings[0].Amount, 64);


//            //job.Complete();
//            //synch.DestroyEntity(m_Settings.Entity[0]);
//            ////m_Instances.Dispose();




//            return inputDeps;










//        }
        


           
//        }


//    }
