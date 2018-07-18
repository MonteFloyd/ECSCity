using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ECSCity {

    public class inputData : MonoBehaviour {

        private int numberofPeople;
        private InputField input;

        void Awake()
        {
            input = GetComponent<InputField>();

        }
        public void setPeople()
        {
            this.numberofPeople = int.Parse(input.text);

        }

        public void Confirm()
        {
            if(numberofPeople > 0)
            ECSBootstrapper.spawnPeople(numberofPeople);
        }

     
    }

}