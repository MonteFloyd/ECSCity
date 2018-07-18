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
    public class VelocitySystem : JobComponentSystem
    {

        [BurstCompile]
        struct ApplyVelocityJob : IJobProcessComponentData<Velocity, Position>
        {
            public float dt;
            public float timeScale;

            public void Execute([ReadOnly] ref Velocity velocity, [WriteOnly]ref Position position)
            {
                position.Value += (velocity.Value * dt * timeScale);

            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            return new ApplyVelocityJob { dt = Time.deltaTime, timeScale = ECSBootstrapper.timeScale }.Schedule(this, inputDeps);

        }


    }




}