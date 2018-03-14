using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class DroneBehaviour : MonoBehaviour
    {
        Vector3 startingPosition;
        public float movementSpeed;
        public float turnSpeed;
        float speed;

        public Rigidbody player;
        Rigidbody droneRigid;
        public CameraManager camera;
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
        Vector3 lastVelocity;



        private void Start()
        {
            gotTooFarAway = false;
            yPlaneView = new Vector3(0, player.position.y + 2.5f, 0);
            startingPosition = player.position - player.transform.right + new Vector3(0, 2f, 0);
            droneRigid = GetComponent<Rigidbody>();
            droneRigid.position = startingPosition;
            lastVelocity = Vector3.zero;
        }

        void Update()
        {

            #region yo
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
            /*
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
                Quaternion lookAt = Quaternion.LookRotation(camera.target.position - transform.position);
                Quaternion rotation = Quaternion.Lerp(transform.rotation, lookAt, Time.smoothDeltaTime * turnSpeed);
                transform.rotation = rotation;
            }
            */
            #endregion
            
            int avoidCount;

            avoidCount = 0;

            

            
            RaycastHit[] hit = Rays(-player.transform.right);
            for (int i = 0; i < hit.Length - 1; i++)
            {
                if (hit[i].transform.root.gameObject != this.gameObject && hit[i].transform.root.gameObject != player)
                {
                    avoidCount++;
                }
            }

            hit = Rays(player.transform.up);
            for (int i = 0; i < hit.Length - 1; i++)
            {
                if (hit[i].transform.root.gameObject != this.gameObject && hit[i].transform.root.gameObject != player)
                {
                    avoidCount++;
                }
            }

            hit = Rays(player.transform.forward);
            for (int i = 0; i < hit.Length - 1; i++)
            {
                if (hit[i].transform.root.gameObject != this.gameObject && hit[i].transform.root.gameObject != player)
                {
                    
                    avoidCount++;
                }
            }

            if(avoidCount != 0)
            {
                startingPosition = player.position - player.transform.forward + transform.right + new Vector3(0, 1.5f, 0);
            }
            else
            {
                startingPosition = player.position - player.transform.right - player.transform.forward + new Vector3(0, 1.5f, 0);
            }
            
           
            
            
            //Vector3 attractionDir = startingPosition - droneRigid.position;


            //droneRigid.velocity = attractionDir.normalized  + new Vector3(player.velocity.x, 0, player.velocity.z);
            //droneRigid.AddForce(attractionDir.normalized * 10, ForceMode.Force);

            //droneRigid.AddForceAtPosition(attractionDir.normalized, startingPosition, ForceMode.Force);

            Vector3 velocityDrone = new Vector3(player.velocity.x, 0, player.velocity.z) + ((startingPosition - droneRigid.position)/ Time.deltaTime);
            droneRigid.velocity = velocityDrone;
            

            

            /*
            Vector3 acceleration = attractionDir / droneRigid.mass;
            lastVelocity = droneRigid.velocity;

            if(acceleration == Vector3.zero)
            {
                droneRigid.velocity = Vector3.zero;
                droneRigid.angularVelocity = Vector3.zero;
            }
            //droneRigid.position = Vector3.Lerp(droneRigid.position, startingPosition, Time.deltaTime);
            */
            this.transform.rotation = camera.transform.rotation;
            Debug.Log(player.velocity.x);
            Debug.Log(player.velocity.z);

        }


        RaycastHit[] Rays(Vector3 direction)
        {
            Ray ray = new Ray(transform.position, direction);
            Debug.DrawRay(transform.position, direction * 10 , Color.red);

            float distanceToLookAhead = 5;

            RaycastHit[] hits = Physics.SphereCastAll(ray,1, distanceToLookAhead);

            return hits;
        }
    }
}

