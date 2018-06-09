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

        private void OnTriggerEnter(Collider other)
        {
            if (pllr.characterStats.hp == 0)
            {
                player.transform.position = savePoint.transform.position;
                player.transform.rotation = savePoint.transform.rotation;
            }
            
        }

    }
}
