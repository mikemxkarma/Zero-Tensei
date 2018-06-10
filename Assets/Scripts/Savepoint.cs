using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class Savepoint : MonoBehaviour {

        public GameObject player;
        public GameObject savePoint;
        public StateManager pllr;
        public Textriger SavepointInfo;

      // Use this for initialization

      void Start() {
          
        }

        public void OnTriggerEnter(Collider other)
        {
            SavepointInfo.savepoint = true;
            pllr.lockOn = false;
            pllr.savepoint = this;        
        }

    }
}
