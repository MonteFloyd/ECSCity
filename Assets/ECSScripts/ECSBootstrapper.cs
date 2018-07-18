using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Transforms2D;
using Unity.Collections;
//using Samples.Boids;
using Samples.Common;
using System.Collections.Generic;


namespace ECSCity
{

    public sealed class ECSBootstrapper
    {
        public static EntityArchetype humanArchetype;
        public static EntityArchetype humanSpawnArchetype;
        public static EntityArchetype homeArchetype;
        public static EntityArchetype workplaceArchetype;
        public static EntityArchetype bombArchetype;


        public static ECSCitySettings Settings;
        public static Boid boidSettings;
        public static MoveForward moveForwardComponent;
        //public static WorkerSchedule workerSchedule;
        //public static MoveSpeed speedComponent;


        public static MeshInstanceRenderer humanRenderer;
        public static MeshInstanceRenderer humanSickRenderer;
        public static MeshInstanceRenderer workplaceRenderer;
        public static MeshInstanceRenderer homeRenderer;


        public static EntityManager entityManager;

        public static int maxX;
        public static int minX;
        public static int maxZ;
        public static int minZ;

        public static float humanPositionY;

        public static float boidDisableDistance;

        public static int infectionRate;
        public static int infectionKillingDay;

        public static int timeScale;


        private static List<float3> homeList;

        private static List<float3> workList;

        private static int numOfHomes;
        private static int numOfWorkplaces;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            entityManager = World.Active.GetOrCreateManager<EntityManager>();

            

            humanArchetype = entityManager.CreateArchetype(typeof(Human),
                                                           typeof(MoveSpeed),
                                                           typeof(Position),
                                                           typeof(Heading),
                                                           typeof(HumanJustInstanced),
                                                           typeof(TransformMatrix),
                                                           typeof(BoidTag),
                                                           typeof(MoveForward),
                                                           typeof(Target),
                                                           typeof(HomePosition)
                                                           );

            bombArchetype = entityManager.CreateArchetype(typeof(BombTag));


            workplaceArchetype = entityManager.CreateArchetype(
                                                               typeof(Position),
                                                               typeof(TransformMatrix)
                                                               );

            homeArchetype = entityManager.CreateArchetype(//typeof(OpenHome),
                                                          //typeof(Speed),
                                                          typeof(Position),
                                                          typeof(TransformMatrix)
                                                          );


            homeList = new List<float3>();

            workList = new List<float3>();

            //humanSpawnArchetype = entityManager.CreateArchetype(typeof(HumanSpawnAmount),
            //                                                    typeof(HumanSpawnPoint),
            //                                                    typeof(HumanSpawnSpread));

            //humanSpawnArchetype = entityManager.CreateArchetype(typeof(HumanSpawnerSettings));


            //Debug.Log("Initialize");

            minZ = -15;
            maxZ = 15;
            minX = -15;
            maxX = 15;

            humanPositionY = 0.2f;

            infectionRate = 0;

            timeScale = 1;

        }
        
        

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeWithScene()
        {
            var settingsGO = GameObject.Find("Settings");
            Settings = settingsGO.GetComponent<ECSCitySettings>();
            if (!Settings)
            {
                Debug.Log("Settings not found!");
                return;
            }
            Object.Destroy(settingsGO);


            humanRenderer = getRendererFromPrototype("HumanRenderPrototype");
            humanSickRenderer = getRendererFromPrototype("HumanSickRenderPrototype");
            workplaceRenderer = getRendererFromPrototype("WorkplaceRenderPrototype");
            homeRenderer = getRendererFromPrototype("HomeRenderPrototype");
            //var boidGO = GameObject.Find("BoidSetting");
            //boidComponent = boidGO.GetComponent<BoidComponent>();

            //speedComponent = boidGO.GetComponent<MoveSpeedComponent>();
            //moveForwardComponent = boidGO.GetComponent<MoveForwardComponent>();



            boidSettings.cellRadius = 0.3f;
            boidSettings.separationWeight = 1.2f;
            boidSettings.alignmentWeight = 1;
            boidSettings.targetWeight = 2f;
            boidSettings.obstacleAversionDistance = 4f;


            boidDisableDistance = 10f;

            var pinguStatue = entityManager.CreateEntity(typeof(Position),typeof(BoidObstacle));
            entityManager.SetComponentData(pinguStatue, new Position { Value = new float3(-2.5f, 0.2f, 8.67f) });



            World.Active.GetOrCreateManager<PlayerHUDSystem>().Setup();
            World.Active.GetOrCreateManager<DayNightSystem>().Setup();
            //Debug.Log("InitializeWithScene");

            //Entity spawner = entityManager.CreateEntity(humanSpawnArchetype);

            //entityManager.SetComponentData(spawner, new HumanSpawnerSettings(100, new Position(new float3(0f, 0f, 0f)), 20f));

            numOfWorkplaces = 4;//Mathf.CeilToInt(Settings.NumberOfHumans / Settings.WorkplaceCapacity)+1;
            //Debug.Log(math.ceil(Settings.NumberOfHumans / Settings.WorkplaceCapacity));
            //Debug.Log(numOfWorkplaces);
            numOfHomes = 5;//Mathf.CeilToInt(Settings.NumberOfHumans / Settings.HomeCapacity)+1;
            //Debug.Log(numOfHomes);

            var m_Workplaces = new NativeArray<Entity>( numOfWorkplaces ,Allocator.Temp);
            var m_Homes = new NativeArray<Entity>( numOfHomes , Allocator.Temp);


            var m_Instances = new NativeArray<Entity>(Settings.NumberOfHumans * (1-(Settings.unemploymentRate/100)), Allocator.Temp);
            float step = 5f;


            for (int i = 0; i < numOfWorkplaces; i++)
            {
                Entity workplace2 = entityManager.CreateEntity(workplaceArchetype);
                entityManager.SetComponentData(workplace2, new Position { Value = new float3(20f, -0.20f, i * step) });
                //entityManager.SetComponentData(workplace1, new NumberOfWorkers { Value = 0 });
                entityManager.AddSharedComponentData(workplace2, workplaceRenderer);
                m_Workplaces[i] = workplace2;
                workList.Add(new float3(20f, humanPositionY, i * step));
                

            }

            for (int i = 0; i < numOfHomes; i++)
            {
                Entity home1 = entityManager.CreateEntity(homeArchetype);
                entityManager.SetComponentData(home1, new Position { Value = new float3(-20f, -0.20f, i * step) });
                entityManager.AddSharedComponentData(home1, homeRenderer);
                m_Homes[i] = home1;
                homeList.Add(new float3(-20f, humanPositionY, i * step));

            }



            //int numOfSchedules = numOfWorkplaces * numOfHomes ;

            //workerSchedule = new WorkerSchedule(24);

            //for(int i = 0; i < 24; ++i)
            //{
            //    workerSchedule.Value[i] = new Position { Value = new float3(-20f, 0.2f, 0) };
            //}

            //for (int i = 9; i < 18; ++i)
            //{
            //    workerSchedule.Value[i] = new Position { Value = new float3(20f, 0.2f, 0) };
            //}

            //var m_Schedules = new List<WorkerSchedule>(numOfSchedules);

            //Position currWorkplacePos;/* = new Position { Value = new float3(20f, -0.20f, 0) };*/
            //Position currHomePos;/* = new Position { Value = new float3(-20f, -0.20f, 0) };*/
            //for (int k = 0; k < numOfHomes; ++k)
            //{
            //    for (int j = 0; j < numOfWorkplaces; ++j)
            //    {
            //        WorkerSchedule newSchedule = new WorkerSchedule { Value = new NativeArray<Position>(24, Allocator.Persistent) };

            //        for (int i = 0; i < 24; ++i)
            //        {
            //            newSchedule.Value[i] = new Position { Value = new float3(-20f, -0.20f, k * step) }; ;
            //        }
            //        //ECSBootstrapper.entityManager.RemoveComponent<OpenHome>(home1);
            //        //ECSBootstrapper.entityManager.AddComponent(home1,typeof(ClosedHome));


            //        for (int i = 9; i < 16; ++i)
            //        {
            //            newSchedule.Value[i] = new Position { Value = new float3(20f, -0.20f, j * step) }; ;
            //        }
            //        m_Schedules.Add(newSchedule);


            //        //for(int l = 0; l < batchSize; ++l)
            //        //{

            //        //    entityManager.AddSharedComponentData(m_Instances[l+(k*batchSize)], newSchedule);
            //        //    //Debug.Log(l + (k * batchSize));
            //        //}

            //        //m_Schedules[indexCount] = newSchedule;
            //        //indexCount++;
            //    }

            //    //ECSBootstrapper.entityManager.RemoveComponent<OpenWorkplace>(workplace1);
            //    //ECSBootstrapper.entityManager.AddComponent(workplace1, typeof(ClosedWorkplace));


            //}




            //int batchSize = math.min(Settings.HomeCapacity, Settings.WorkplaceCapacity);
            //int currIndex=0;
            //for(int i = 0; i < m_Instances.Length; ++i)
            //{

            //    entityManager.AddSharedComponentData(m_Instances[i], m_Schedules[currIndex]);
            //    //Debug.Log(currIndex);
            //    if(i == batchSize)
            //    {
            //        ++currIndex;
            //        batchSize = batchSize * 2;
            //    }
            //}

            //Target newTarget = new Target { Value = new float3(20f, 0f, 0) };
            entityManager.CreateEntity(humanArchetype, m_Instances);
            foreach ( Entity en in m_Instances)
            {
                
                //entityManager.AddComponent(en, typeof(WorkPosition));

                int randomHomeIndex = Random.Range(0, numOfHomes);
                int randomWorkIndex = Random.Range(0, numOfWorkplaces);
                entityManager.SetComponentData<HomePosition>(en, new HomePosition { Value = homeList[randomHomeIndex] });

                entityManager.AddComponentData<WorkPosition>(en, new WorkPosition { Value = workList[randomWorkIndex] });
                entityManager.SetComponentData<Target>(en, new Target { Value = workList[randomWorkIndex] });
                //entityManager.SetComponentData<WorkPosition>(en, new WorkPosition { Value = workList[randomIndex] });
                //entityManager.SetComponentData(en, newTarget);
                //entityManager.AddComponentData(en, new MoveSpeed { speed = Settings.HumanMoveSpeed });
                //entityManager.SetComponentData<MoveSpeed>(en, new MoveSpeed { speed = 0 });
                //entityManager.AddSharedComponentData(en, workerSchedule);
                //entityManager.AddComponent(en, ComponentType.FixedArray(typeof(Position), 24));

                //entityManager.SetSharedComponentData(en, workerSchedule);
                //entityManager.SetSharedComponentData(en, boidComponent);
                //entityManager.RemoveComponent(en, typeof(Boid));
                //entityManager.SetSharedComponentData(en, speedComponent);
                //entityManager.AddSharedComponentData(en, moveForwardComponent);

            }

            
            


            

            //var m_Wanderers = new NativeArray<Entity>(Settings.NumberOfHumans * (Settings.unemploymentRate / 100), Allocator.Temp);
            //entityManager.CreateEntity(humanArchetype, m_Wanderers);


            //foreach (Entity en in m_Instances)
            //{


            //    int randomIndex = Random.Range(0, numOfHomes - 1);
            //    entityManager.AddComponent(en, typeof(Wanderer));
            //    entityManager.SetComponentData<HomePosition>(en, new HomePosition { Value = homeList[randomIndex] });
            //    //entityManager.SetComponentData(en, newTarget);
            //    //entityManager.AddComponentData(en, new MoveSpeed { speed = Settings.HumanMoveSpeed });
            //    //entityManager.SetComponentData<MoveSpeed>(en, new MoveSpeed { speed = 0 });
            //    //entityManager.AddSharedComponentData(en, workerSchedule);
            //    //entityManager.AddComponent(en, ComponentType.FixedArray(typeof(Position), 24));

            //    //entityManager.SetSharedComponentData(en, workerSchedule);
            //    //entityManager.SetSharedComponentData(en, boidComponent);
            //    //entityManager.RemoveComponent(en, typeof(Boid));
            //    //entityManager.SetSharedComponentData(en, speedComponent);
            //    //entityManager.AddSharedComponentData(en, moveForwardComponent);

            //}







            //boidComponent.cellRadius = 6;
            //boidComponent.separationWeight = 1;
            //boidComponent.alignmentWeight = 1;
            //boidComponent.targetWeight = 2;
            //boidComponent.obstacleAversionDistance = 20;

            //foreach (Entity home in m_Homes)
            //{
            //    entityManager.RemoveComponent(home, typeof(OpenHome));
            //}

            //foreach(Entity workplace in m_Workplaces)
            //{
            //    entityManager.RemoveComponent(workplace, typeof(OpenWorkplace));
            //}


            //entityManager.SetComponentData(m_Instances[0], default(Human));
            //entityManager.SetComponentData(m_Instances[0], new Speed { Value = Settings.HumanMoveSpeed });
            //entityManager.SetComponentData(m_Instances[0],new Position { Value = new float3(0f, 0.5f, 0f) });
            //entityManager.SetComponentData(m_Instances[0],new Heading { Value = new float3(1.0f, 0.0f, 1.0f) });
            //entityManager.SetComponentData(m_Instances[0], default(HumanJustInstanced));
            //entityManager.AddSharedComponentData()
            ////entityManager.SetComponent(m_Instances[0], typeof(HumanJustInstanced));
            //entityManager.AddSharedComponentData(m_Instances[0], humanRenderer);

            m_Instances.Dispose();
            m_Homes.Dispose();
            m_Workplaces.Dispose();
            //m_Schedules.Clear();

            //startSimulation();
        }



        private static MeshInstanceRenderer getRendererFromPrototype(string prototypeName)
        {
            var prototype = GameObject.Find(prototypeName);

            var renderer = prototype.GetComponent<MeshInstanceRendererComponent>().Value;

            Object.Destroy(prototype);

            return renderer;

        }


        public static void bombPosition(Vector3 bombPos)
        {
            //var newBomb = entityManager.CreateEntity(bombArchetype);

            var newBomb = entityManager.CreateEntity(typeof(BombTag));

            
            entityManager.SetComponentData(newBomb, new BombTag { Value = new float3(bombPos.x, bombPos.y, bombPos.z) });
        }




        public static bool Cleanup()
        {
            var lastArray = entityManager.GetAllEntities();
            entityManager.CompleteAllJobs();

            foreach (Entity en in lastArray)
            {
                entityManager.DestroyEntity(en);
            }
            lastArray.Dispose();

            return true;

        }

        public static void startInfection(int infectionRate,int killingDay)
        {
            var newInfection = entityManager.CreateEntity(typeof(InfectionSetting));
            entityManager.SetComponentData(newInfection, new InfectionSetting { infectionRate = infectionRate });
            infectionKillingDay = 24 * killingDay;

        }

        public static void spawnPeople(int numberOfPeople)
        {
            var m_Instances = new NativeArray<Entity>(numberOfPeople * (1 - (Settings.unemploymentRate / 100)), Allocator.Temp);
            entityManager.CreateEntity(humanArchetype, m_Instances);
            foreach (Entity en in m_Instances)
            {

                //entityManager.AddComponent(en, typeof(WorkPosition));

                int randomHomeIndex = Random.Range(0, numOfHomes);
                int randomWorkIndex = Random.Range(0, numOfWorkplaces);
                entityManager.SetComponentData<HomePosition>(en, new HomePosition { Value = homeList[randomHomeIndex] });

                entityManager.AddComponentData<WorkPosition>(en, new WorkPosition { Value = workList[randomWorkIndex] });
                entityManager.SetComponentData<Target>(en, new Target { Value = workList[randomWorkIndex] });


            }

            m_Instances.Dispose();

        }








    }

}
