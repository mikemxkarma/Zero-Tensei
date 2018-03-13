using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class DroneBehaviour : MonoBehaviour
    {
        public float movementSpeed;
        public float turnSpeed;
        float speed;

        public Transform player;
        public StateManager playerState;

        bool gotTooFarAway;

        Vector3 nextPosition;
        Vector3 yPlaneView;

        Vector3 storeTarget;
        Vector3 newTargetPos;
        bool savePos;
        bool overrideTarget;
        Transform obstacle;
        public List<Vector3> EscapeDirections = new List<Vector3>();

        private void Start()
        {
            gotTooFarAway = false;
            yPlaneView = new Vector3(0, player.position.y + 2.5f, 0);
        }

        void Update()
        {
            if (playerState.run)
            {
                speed = playerState.runSpeed ;
            }
            else
            {
                speed = playerState.moveSpeed ;
            }
                
          /*  if (gotTooFarAway || playerState.run)
            {
                speed = playerState.runSpeed;

                if (Mathf.Abs(transform.position.x - player.position.x) > 5 ||
                    Mathf.Abs(transform.position.z - player.position.z) > 5)
                {
                    gotTooFarAway = false;
                }
            }
            else if ((Mathf.Abs(transform.position.x - player.position.x) > 15 ||
                    Mathf.Abs(transform.position.z - player.position.z) > 15))
            {
                gotTooFarAway = true;
            }

            else
            {
                speed = playerState.moveSpeed;
            }

            */
            
            if (Mathf.Abs(transform.position.x - player.position.x) > 0.5f || Mathf.Abs(transform.position.z - player.position.z) > 0.5f)
            {
                Quaternion lookAt = Quaternion.LookRotation(player.position + yPlaneView - transform.position);
                Quaternion rotation = Quaternion.Slerp(transform.rotation, lookAt, Time.smoothDeltaTime * turnSpeed);
                transform.rotation = rotation;

                nextPosition = transform.position;

                nextPosition.x += transform.forward.x * speed * Time.smoothDeltaTime;
                nextPosition.y = Mathf.Lerp(transform.position.y, player.position.y + 2.5f, Time.smoothDeltaTime * speed);
                nextPosition.z += transform.forward.z * speed * Time.smoothDeltaTime;

                transform.position = nextPosition;
            }
            else
            {
                Quaternion lookAt = Quaternion.LookRotation(player.position + yPlaneView - transform.position);
                Quaternion rotation = Quaternion.Lerp(transform.rotation, lookAt, Time.smoothDeltaTime * turnSpeed);
                transform.rotation = rotation;
            }
            
        }
        
    }
}

