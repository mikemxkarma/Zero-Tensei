using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class Savepoint : MonoBehaviour {

        public GameObject player;
        public GameObject savePoint;
        public StateManager pllr;
      // Use this for initialization

      void Start() {
          
        }

        public void OnTriggerEnter(Collider other)
        {
            pllr.lockOn = false;
            pllr.savepoint = this;
        }
    }
}
