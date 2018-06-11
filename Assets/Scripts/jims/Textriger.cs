using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class Textriger : MonoBehaviour {

        public GameObject UInfoDisplay;
        private void Start()
        {
            UInfoDisplay.SetActive(false);
        }

        void OnTriggerEnter(Collider playr)
        {

            UInfoDisplay.SetActive(true);

        }
        void OnTriggerExit(Collider playr)
        {
                UInfoDisplay.SetActive(false);
        }
    }


