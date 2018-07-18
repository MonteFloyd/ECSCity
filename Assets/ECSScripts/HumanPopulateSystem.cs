using Unity.Transforms;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine;
using Unity.Mathematics;
using Unity.Rendering;

namespace ECSCity
{


    public class HumanPopulateSystem : JobComponentSystem
    {

        public class RemoveLabelBarrier : BarrierSystem
        { }


        struct Entities
        {
            public EntityArray Instances;
            public ComponentDataArray<HumanJustInstanced> Useless;
            public ComponentDataArray<MoveSpeed> moveSpeed;
            public ComponentDataArray<Position> Positions;
            

        }

        [Inject] Entities m_Humans;

        [Inject] RemoveLabelBarrier m_RemoveLabelBarrier;




        [BurstCompile]
        struct Populate : IJobParallelFor
        {
            [WriteOnly] public ComponentDataArray<Position> currPositions;
            [ReadOnly] public NativeArray<Position> newPositions;
            //public Position newPos;
            //public void Execute(ref Position pos)
            //{
            //    for (int i = 0; i < Positions.Length; ++i)
            //    {
            //        Positions[i] = new Position { Value = new float3(Random.value * 50f, 0.5f, Random.value * 50f) };
            //    }

            //}

            public void Execute(int i)
            {

                currPositions[i] = newPositions[i];
            }
        }


        [BurstCompile]
        struct PopulateSpeed : IJobParallelFor
        {
            [WriteOnly] public ComponentDataArray<MoveSpeed> speed;
            [ReadOnly] public NativeArray<float> newSpeed;
            //public NativeArray<float3> newPositions;
            //public Position newPos;
            //public void Execute(ref Position pos)
            //{
            //    for (int i = 0; i < Positions.Length; ++i)
            //    {
            //        Positions[i] = new Position { Value = new float3(Random.value * 50f, 0.5f, Random.value * 50f) };
            //    }

            //}

            //public void Execute(ref HumanJustInstanced ignore, ref MoveSpeed speed)
            //{

            //    speed.speed = newSpeed;
            //}

            public void Execute(int index)
            {
                speed[index] = new MoveSpeed { speed = newSpeed[index] };
            }
        }


        //public void Execute(int index)
        //{
        //    Position newPos = new Position(new float3(Random.value * 50f, 0.5f, Random.value * 50f));
        //    Positions[index] = newPos;
        //}


        //[BurstCompile]
        struct RemoveLabel : IJobParallelFor
        {
            public EntityCommandBuffer.Concurrent CommandBuffer;
            [ReadOnly]public EntityArray Instances;


            void IJobParallelFor.Execute(int i)
            {
                CommandBuffer.AddSharedComponent(Instances[i], ECSBootstrapper.humanRenderer);
                CommandBuffer.RemoveComponent<HumanJustInstanced>(Instances[i]);
            }
        }




        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (m_Humans.Instances.Length == 0)
            {
                return inputDeps;
            }

            //Debug.Log("1");

            NativeArray<Position> m_newPositions = new NativeArray<Position>(m_Humans.Instances.Length, Allocator.Temp , NativeArrayOptions.UninitializedMemory);
            NativeArray<float> m_randomSpeeds = new NativeArray<float>(m_Humans.Instances.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (int i =0; i <m_newPositions.Length; ++i)
            {
                m_newPositions[i] = new Position { Value = new float3(Random.value * 15f, ECSBootstrapper.humanPositionY, Random.value * 15f) } ;
            }

            for (int i = 0; i < m_randomSpeeds.Length; ++i)
            {
                m_randomSpeeds[i] = Random.Range(5f,7f);
            }

            var jobHandle = new Populate
            {
                newPositions = m_newPositions,
                currPositions = m_Humans.Positions
            }.Schedule(m_Humans.Instances.Length, 64, inputDeps);

            var jobHandle3 = new PopulateSpeed {
                speed = m_Humans.moveSpeed,
                newSpeed= m_randomSpeeds
            }.Schedule(m_Humans.Instances.Length,64,inputDeps);

            jobHandle.Complete();
            jobHandle3.Complete();
            m_newPositions.Dispose();
            m_randomSpeeds.Dispose();
            //Debug.Log("2");

            //var job2Handle = new RemoveLabel
            //{
            //    CommandBuffer = m_RemoveLabelBarrier.CreateCommandBuffer(),
            //    Instances = m_Humans.Instances

            //}.Schedule(jobHandle);

            var job2Handle = new RemoveLabel
            {
                CommandBuffer = m_RemoveLabelBarrier.CreateCommandBuffer(),
                Instances = m_Humans.Instances
            }.Schedule(m_Humans.Instances.Length, 64, inputDeps);

            //var job2Handle = new RemoveLabel
            //{
            //    CommandBuffer = m_RemoveLabelBarrier.CreateCommandBuffer(),
            //    Instances = m_Humans.Instances

            //}.Schedule(m_Humans.Instances.Length,32,jobHandle);
            //Debug.Log("3");

            job2Handle.Complete();
            //m_Humans.Useless.Dispose();

            return inputDeps;
        }


    }

}