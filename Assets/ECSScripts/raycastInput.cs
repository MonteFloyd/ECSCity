using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ECSCity { 
    public class raycastInput : MonoBehaviour
    {

        public bool bombActived;

        public int infectionRate;
        public int killingDay;

        public Slider rateSlider;

        public Slider killingSlider;


        public void setKillingDay()
        {
            killingDay = (int)killingSlider.value;
        }

        public void setInfectionRate()
        {
            infectionRate = (int)rateSlider.value;
        }

        void Awake()
        {
            infectionRate = 1;
            killingDay = 2;
            bombActived = false;
        }

        public void activateBomb()
        {
            bombActived = true;
        }

        public void deactivateBomb()
        {
            bombActived = false;
        }

        public void startInfection()
        {
            ECSBootstrapper.startInfection(infectionRate, killingDay);
        }

        

        // Update is called once per frame
        void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                if (bombActived)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                    {
                        ECSBootstrapper.bombPosition(hit.point);
                        //Debug.Log(hit.point);
                    }
                }

            }


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (ECSBootstrapper.Cleanup())
                {
                    Application.Quit();
                }
            }
        }

    }
}
