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
    public class GroundCollisionSystem : JobComponentSystem
    {
        //ComponentGroup m_Group;
        //protected override void OnCreateManager(int capacity)
        //{
        //    m_Group = GetComponentGroup(typeof(Position), typeof(Velocity), typeof(GravityType));
        //}

        //struct CollisionData
        //{
        //    [ReadOnly] public ComponentDataArray<Position> positions;
        //               public ComponentDataArray<Velocity> velocities;
        //    [ReadOnly] public ComponentDataArray<GravityType> tag;

        //}

        //[Inject] CollisionData m_Collisions;


        [BurstCompile]
        struct collisionJob : IJobProcessComponentData<Position, Velocity, GravityType>
        {
            public void Execute([ReadOnly]ref Position position, ref Velocity velocity, [ReadOnly]ref GravityType data2)
            {
                if (position.Value.y <= 0.75f && velocity.Value.y < 0)
                {
                    velocity.Value.y = math.select(0.0f, velocity.Value.y / -3, velocity.Value.y < -2);
                    //velocity.Value.y = velocity.Value.y/-2;
                    // new Velocity { Value = float3(velocity.Value.x,velocity.Value.y)}
                }
            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new collisionJob { }.Schedule(this, inputDeps);


        }
    }



}
