using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class Textriger : MonoBehaviour {



        public GameObject UInfoDisplay;
        public bool savepoint;


        private void Start()
        {
            savepoint = false;
            UInfoDisplay.SetActive(false);
        }

        void OnTriggerEnter(Collider playr)
        {

            UInfoDisplay.SetActive(true);

        }
        void OnTriggerExit(Collider playr)
        {
            if (savepoint == true)
                Destroy(UInfoDisplay);

            else
                UInfoDisplay.SetActive(false);
        }
    }
}

