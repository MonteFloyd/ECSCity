using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


namespace ECSCity
{
    public class DayNightSystem : ComponentSystem
    {
        //struct timeData
        //{
        //    public ComponentDataArray<worldTime> worldTime;
        //}

        //[Inject] timeData time;
        //public Transform mainLight;

        public static float worldTime;




        protected override void OnUpdate()
        {
            float dt = Time.deltaTime;
            worldTime += dt * ECSBootstrapper.timeScale;
            if(worldTime >= 23.99f)
            {
                worldTime = 00.00f;
            }
            
            //mainLight.rotation = Quaternion.Euler(220f+15f*worldTime , 0, 0);
               
                
                

        }


        public void Setup()
        {
            //mainLight = GameObject.Find("Light").GetComponent<Transform>();
            worldTime = 12.00f;
        }


    }
}
