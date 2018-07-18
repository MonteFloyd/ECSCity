using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Mathematics.Experimental;
using Unity.Transforms;

namespace ECSCity
{
    public class BlowUpSystem : JobComponentSystem
    {
        public class addVelocityBarrier : BarrierSystem { }

        struct dummyData
        {
            [ReadOnly] public EntityArray dummies;

            [ReadOnly] public ComponentDataArray<Human> tag;
            [ReadOnly] public ComponentDataArray<Position> positions;
            [ReadOnly] public SubtractiveComponent<Velocity> ignore;

        }


        //struct dummiesInMotion
        //{
        //    public ComponentDataArray<Velocity> Velocities;

        //}

        struct BombData
        {
            [ReadOnly] public EntityArray bomb;
            [ReadOnly] public ComponentDataArray<BombTag> bombPosition;

        }

        [Inject] dummyData m_Dummies;
        [Inject] BombData m_BombData;
        //[Inject] dummiesInMotion m_inmotionDummies;
        [Inject] addVelocityBarrier commandBuffer;



        struct AddVelocityJob : IJobParallelFor
        {
            
            [ReadOnly] public EntityArray dummies;
            [ReadOnly] public ComponentDataArray<Position> position;
            [ReadOnly] public float3 bombPosition;
            public EntityCommandBuffer.Concurrent velocityBarrier;

            public void Execute(int index)
            {
                half distance = math.distance(bombPosition, position[index].Value);
                if(distance < 4)
                {
                    float3 force =  math_experimental.normalizeSafe( position[index].Value - bombPosition);
                    force.y = 5f;
                    force = force * (4 - distance);
                    velocityBarrier.AddComponent(dummies[index], new Velocity { Value = force });
                    velocityBarrier.AddComponent(dummies[index], default(GravityType));
                    velocityBarrier.RemoveComponent<MoveForward>(dummies[index]);

                }
            }
        }




        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (m_BombData.bombPosition.Length != 0)
            {


                var veloHandle = new AddVelocityJob
                {
                    dummies = m_Dummies.dummies,
                    position = m_Dummies.positions,
                    bombPosition = m_BombData.bombPosition[0].Value,
                    velocityBarrier = commandBuffer.CreateCommandBuffer()
                }.Schedule(m_Dummies.dummies.Length, 128, inputDeps);

                veloHandle.Complete();

                ECSBootstrapper.entityManager.DestroyEntity(m_BombData.bomb[0]);

                return inputDeps;
                //if (m_Dummies.dummies.Length != 0)
                //{

                //    for (int i = 0; i < m_Dummies.dummies.Length; ++i)
                //    {
                //        half distance = math.distance(m_BombData.bombPosition[0].Value, m_Dummies.positions[i].Value);

                //        if (distance < 5)
                //        {
                //            float3 force = m_Dummies.positions[i].Value - m_BombData.bombPosition[0].Value;
                //            force.y = 5f;
                //            force = force * (5 - distance);
                //            PostUpdateCommands.AddComponent(m_Dummies.dummies[i], new Velocity { Value = force });
                //            PostUpdateCommands.AddComponent(m_Dummies.dummies[i], default(GravityType));
                //            PostUpdateCommands.RemoveComponent<MoveForward>(m_Dummies.dummies[i]);
                //            //PostUpdateCommands.RemoveComponent<BoidTag>(m_Dummies.dummies[i]);

                //        }
                //    }


                //    //PostUpdateCommands.DestroyEntity(m_BombData.bomb[0]);
                //}


                //if (m_inmotionDummies.Velocities.Length != 0)
                //{

                //    for (int i = 0; i < m_Dummies.dummies.Length; ++i)
                //    {
                //        half distance = math.distance(m_BombData.bombPosition[0].Value, m_Dummies.positions[i].Value);

                //        if (distance < 5)
                //        {
                //            float3 force = m_Dummies.positions[i].Value - m_BombData.bombPosition[0].Value;
                //            force.y = 5f;
                //            force = force * (5 - distance);
                //            PostUpdateCommands.SetComponent(m_Dummies.dummies[i], new Velocity { Value = force });

                //        }
                //    }


                //    //PostUpdateCommands.DestroyEntity(m_BombData.bomb[0]);
                //}


            }
            else
            {
                return inputDeps;
            }


        }




    }

}