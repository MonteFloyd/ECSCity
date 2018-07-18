using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

namespace ECSCity
{

    //public struct worldTime : IComponentData
    //{
    //    public half Value;
    //}


    //BOID STUFF

    public struct Wanderer : IComponentData
    {

    }

    public struct Target : IComponentData
    {
        public float3 Value;

    }

    public struct Worker : IComponentData
    {

    }

    public struct BoidObstacle : IComponentData { }


    public struct BoidTag :IComponentData
    { }

    
    public struct Boid : ISharedComponentData
    {
        public float cellRadius;
        public float separationWeight;
        public float alignmentWeight;
        public float targetWeight;
        public float obstacleAversionDistance;
    }

    //public unsafe struct WorkerSchedule : ISharedComponentData
    //{

    //    public NativeArray<Position> Value;


    //    public WorkerSchedule(int size) { Value = new NativeArray<Position>(size, Allocator.Persistent); }
    //}


    //public struct WandererSchedule : ISharedComponentData
    //{
    //    public NativeArray<Position> Value;

    //}


    //HUMAN STUFF

    public struct OpenHome : IComponentData { }

    public struct ClosedHome : IComponentData { }

    public struct ClosedWorkplace : IComponentData { }

    public struct OpenWorkplace : IComponentData { }

    public struct HomePosition : IComponentData
    {
        public float3 Value;
    }

    public struct WorkPosition : IComponentData
    {
        public float3 Value;
    }

    public struct Speed : IComponentData
    {
        public float Value;
    }



    public struct Human : IComponentData { }



    //SPAWN STUFF

    public struct WorkplaceJustInstanced : IComponentData
    {  }

    public struct HumanJustInstanced : IComponentData
    { }

    public struct HomeJustInstanced : IComponentData { }
    



    //INFECTION STUFF

    public struct InfectedTag : IComponentData
    {
        public float Cooldown;
    };

    public struct InfectionSetting : IComponentData
    {
        public int infectionRate;
    }



    //BOMB PHYSICS STUFF

    public struct Velocity : IComponentData { public float3 Value; };

    public struct BombTag : IComponentData { public float3 Value; };

    public struct GravityType : IComponentData { };




}