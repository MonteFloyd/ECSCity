using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using Unity.Mathematics.Experimental;


namespace ECSCity
{
    public class HumanMovementSystem : JobComponentSystem
    {
        struct HumanNonBoidData
        {
            [WriteOnly] public ComponentDataArray<Heading> Heading;
            [ReadOnly] public ComponentDataArray<Target> Target;
            [ReadOnly] public ComponentDataArray<Position> Position;
            [ReadOnly] public SubtractiveComponent<BoidTag> subtractiveComponent;
        }


        [Inject] HumanNonBoidData m_Data;


        [BurstCompile]
        struct SetHeadings : IJobParallelFor
        {
            [WriteOnly] public ComponentDataArray<Heading> Headings;
            [ReadOnly] public ComponentDataArray<Target> Target;
            [ReadOnly] public ComponentDataArray<Position> Position;


            public void Execute(int index)
            {
                Headings[index] = new Heading { Value = math_experimental.normalizeSafe(Target[index].Value - Position[index].Value) };

            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new SetHeadings
            {
                Headings = m_Data.Heading,
                Target = m_Data.Target,
                Position = m_Data.Position
            }.Schedule(m_Data.Heading.Length, 64, inputDeps);

        }
    }
}


        //public class HumanMovementSystem : JobComponentSystem
        //{
        //    struct HumanData
        //    {
        //        [ReadOnly]  public int Length;
        //        [ReadOnly]  public ComponentDataArray<Human> ignore;
        //        [WriteOnly]  public ComponentDataArray<Heading> Headings;
        //    }


        //    [Inject] HumanData m_Humans;

        //    [BurstCompile]
        //    struct SetHeadings : IJobParallelFor
        //    {
        //        [WriteOnly] public ComponentDataArray<Heading> Headings;
        //        [ReadOnly]  public NativeArray<Heading> NewHeadings;


        //        public void Execute(int index)
        //        {
        //            Headings[index] = NewHeadings[index];
        //        }
        //    }

        //    //[BurstCompile]
        //    //struct HumanNavigate : IJobProcessComponentData<Human, Heading>
        //    //{
        //    //    NativeArray<Heading> NewHeading;
        //    //    //public void Execute(int index)
        //    //    //{

        //    //    //}
        //    //    void IJobProcessComponentData<Human, Heading>.Execute(ref Human human, ref Heading heading)
        //    //    {
        //    //        heading = NewHeading;
        //    //    }
        //    //}

        //    [BurstCompile] 
        //    struct Steer : IJobProcessComponentData<Heading, Position,Speed>
        //    {
        //        public float dt;

        //        public void Execute(ref Heading heading, ref Position pos,ref Speed speed)
        //        {
        //            pos.Value += math_experimental.normalizeSafe(heading.Value) *dt;

        //        }

        //    }


        //    protected override JobHandle OnUpdate(JobHandle inputDeps)
        //    {
        //        float dt = Time.deltaTime;
        //        //timer -= dt;

        //        //return inputDeps;
        //        NativeArray<Heading> RandomHeadings = new NativeArray<Heading>(m_Humans.Length, Allocator.Temp);


        //        for (int i = 0; i < m_Humans.Length; i++)
        //        {
        //            //new float3(22f, -0.20f, 4f)
        //            //Value = new float3(Random.Range(-100, 100), 0, Random.Range(-100, 100))
        //            RandomHeadings[i] = new Heading { Value = new float3(22f, 0f, 4f) };
        //        }


        //        var HeadingJob = new SetHeadings
        //        {
        //            Headings = m_Humans.Headings,
        //            NewHeadings = RandomHeadings
        //        }.Schedule(m_Humans.Length, 64, inputDeps);

        //        HeadingJob.Complete();

        //        RandomHeadings.Dispose();

        //        //return inputDeps;
        //        var SteerJob = new Steer
        //        {
        //            dt = Time.deltaTime
        //        }.Schedule(this, HeadingJob);



        //        return SteerJob;
        //    }




        //}



    



