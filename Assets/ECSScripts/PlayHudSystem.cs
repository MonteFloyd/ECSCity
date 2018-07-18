using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using Unity.Mathematics;

namespace ECSCity
{

    [UpdateAfter(typeof(DayNightSystem))]
    public class PlayerHUDSystem : ComponentSystem
    {

        public Text timeText;

        public void Setup()
        {
            timeText = GameObject.Find("Canvas").GetComponentInChildren<Text>();

        }

        protected override void OnUpdate()
        {
            timeText.text = "Time : " + math.floor(DayNightSystem.worldTime).ToString();
        }

    }
}
