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
    //[UpdateBefore(typeof(BoidStripSystem))]
    public class WorkerNavigationSystem : JobComponentSystem
    {

        public class AddBoidBarrier : BarrierSystem
        { }

        public struct HumanData
        {
            [ReadOnly] public EntityArray Instances;
            //[ReadOnly] public SharedComponentDataArray<WorkerSchedule> workerSchedules;
            //[ReadOnly] public ComponentDataArray<Position> humanPositions;
            [ReadOnly] public ComponentDataArray<HomePosition> homePositions;
            [ReadOnly] public ComponentDataArray<WorkPosition> workPositions;
            [ReadOnly] public SubtractiveComponent<BoidTag> ignore;
            [ReadOnly] public SubtractiveComponent<GravityType> ignore2;
            public ComponentDataArray<Target> Targets;
        }

        [Inject] HumanData m_HumanData;
        [Inject] AddBoidBarrier addBoidBuffer;


        
        public struct setTargetJob : IJobParallelFor
        {
            public ComponentDataArray<Target> target;
            //[ReadOnly] public SharedComponentDataArray<WorkerSchedule> workerSchedules;
            [ReadOnly] public ComponentDataArray<HomePosition> homePosition;
            [ReadOnly] public ComponentDataArray<WorkPosition> workPosition;
            [ReadOnly] public EntityArray Instance;
            public EntityCommandBuffer.Concurrent commandBuffer;

            public int currentTime;

            public void Execute(int index)
            {
                float3 newPos =  math.select(homePosition[index].Value, workPosition[index].Value, (currentTime > 8 && currentTime < 19));
            
                if ( target[index].Value.x != newPos.x || target[index].Value.z != newPos.z)
                {
                    commandBuffer.AddComponent(Instance[index], default(BoidTag));
                    target[index] = new Target { Value = newPos };
                }
                //commandBuffer.AddComponent(Instances[index], default(BoidTag));
                //targets[index] = new Target { Value = workerSchedules[index].Value[currentTime].Value };

                //workerSchedules[index].Value[currentTime].Value


            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            var SetTargetJob = new setTargetJob
            {
                Instance = m_HumanData.Instances,
                target = m_HumanData.Targets,
                homePosition = m_HumanData.homePositions,
                workPosition = m_HumanData.workPositions,
                //workerSchedules = m_HumanData.workerSchedules,
                currentTime = Mathf.FloorToInt(DayNightSystem.worldTime),
                commandBuffer = addBoidBuffer.CreateCommandBuffer()

            }.Schedule(m_HumanData.Targets.Length, 64, inputDeps);


            return SetTargetJob;
        }
    }
}