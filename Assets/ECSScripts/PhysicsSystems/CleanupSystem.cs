using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

namespace ECSCity
{
    public class CleanupSystem : ComponentSystem
    {
        //public class cleanupBarrier : BarrierSystem { }

        struct cleanupData
        {
            public EntityArray entities;
            [ReadOnly] public ComponentDataArray<Velocity> velocities;
            [ReadOnly] public ComponentDataArray<GravityType> tag;

        }


        [Inject] cleanupData m_cleanupData;
        //[Inject] cleanupBarrier commandBuffer;


        //struct cleanupJob : IJobParallelFor
        //{
        //    [ReadOnly] public EntityCommandBuffer commandBuffer;
        //    [ReadOnly] public ComponentDataArray<Velocity> velocity;
        //    [ReadOnly] public EntityArray entity;

        //    public void Execute(int index)
        //    {
        //        if(velocity[index].Value.y == 0.0f)
        //        {
        //            commandBuffer.RemoveComponent<Velocity>(entity[index]);
        //            commandBuffer.RemoveComponent<GravityType>(entity[index]);
        //        }

        //    }
        //}


        protected override void OnUpdate()
        {

            //return new cleanupJob
            //{
            //    commandBuffer = commandBuffer.CreateCommandBuffer(),
            //    velocity = m_cleanupData.velocities,
            //    entity = m_cleanupData.entities
            //}.Schedule(m_cleanupData.entities.Length, 1, inputDeps);

            for (int i = 0; i < m_cleanupData.entities.Length; ++i)
            {
                //Debug.Log(m_cleanupData.velocities[i].Value.y);
                if (m_cleanupData.velocities[i].Value.y == 0)
                {
                    //PostUpdateCommands.RemoveComponent<Velocity>(m_cleanupData.entities[i]);
                    //PostUpdateCommands.RemoveComponent<GravityType>(m_cleanupData.entities[i]);
                    PostUpdateCommands.DestroyEntity(m_cleanupData.entities[i]);
                }

            }


        }




    }



}
