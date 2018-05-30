using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class DroneBehaviour : MonoBehaviour
    {
        public Camera camera;

        Vector3 position;
        Vector3 smoothedPosition;
        float smoothSpeed = 6f;

        public Transform player;
        public StateManager playerState;
        
        Vector3 yPlaneView;
        


        private void Start()
        {
            
            yPlaneView = new Vector3(0, 2f, 0);
            position = player.position + player.transform.right + player.transform.forward + yPlaneView;
        }

        void FixedUpdate()
        {

            
            position = player.position + player.transform.right * 2 + player.transform.forward * 3 + yPlaneView;


            smoothedPosition = Vector3.Lerp(transform.position, position, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        
            

            this.transform.rotation = camera.transform.rotation;
            

        }


       
    }
}

