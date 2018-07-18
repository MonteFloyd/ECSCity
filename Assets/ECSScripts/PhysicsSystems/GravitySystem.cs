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
    public class GravitySystem : JobComponentSystem
    {

        [BurstCompile]
        struct gravityPullJob : IJobProcessComponentData<Velocity, GravityType>
        {
            public float dt;
            public float timeScale;


            public void Execute([WriteOnly]ref Velocity velocity, [ReadOnly]ref GravityType tag)
            {
                //var newY = math.max(0, velocity.Value.y - (9f * dt));
                velocity.Value += new float3(0f, -9f * dt* timeScale, 0f);


            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            var gravityPullJobHandle = new gravityPullJob { dt = Time.deltaTime, timeScale = ECSBootstrapper.timeScale }.Schedule(this, inputDeps);

            return gravityPullJobHandle;

        }

    }



}