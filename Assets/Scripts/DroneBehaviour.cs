using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class DroneBehaviour : MonoBehaviour
    {
        public CameraManager camera;
        Vector3 startingPosition;
        public float movementSpeed;
        public float turnSpeed;

        public Rigidbody player;
        Rigidbody droneRigid;
        public StateManager playerState;

        public bool colLeft;
        public bool colRight;
        public bool colFront;
        public bool colBack;
        public bool colUp;
        public bool colDown;

        Vector3 yPlaneView;
        
        public List<Vector3> EscapeDirections = new List<Vector3>();
        Vector3 lastVelocity;



        private void Start()
        {
            
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
            
            //if player nao estiver no estado de andar nao mudar
            //raycast no vetor velocidade
            //redirecionar velocidade para uma tangente a parede, retirando como feito antes as componentes nessa direção, e aplicando
            //a magnitude da velocidade igual. quoficiente entre a magnitude normal e a after collision

            startingPosition = player.position - player.transform.right - player.transform.forward + new Vector3(0, 1.5f, 0);

            
            Vector3 velocityDrone = (startingPosition - droneRigid.position) / Time.deltaTime * 0.2f;
            droneRigid.velocity = velocityDrone;

            
            //Soluçao: starting position muda com as colisões, e não a velocidade do drone

            
            colFront = Physics.Raycast(transform.position, droneRigid.velocity, 2f);

            

            if (colFront)
            {
                //rotate velocity vector to accompany collision
                droneRigid.velocity -= droneRigid.velocity;
            }
            /*
            colFront = Physics.Raycast(transform.position, player.transform.forward, 1f);
            colBack = Physics.Raycast(transform.position, -player.transform.forward, 1f);
            colLeft = Physics.Raycast(transform.position, -player.transform.right, 1f);
            colRight = Physics.Raycast(transform.position, player.transform.right, 1f);
            colUp = Physics.Raycast(transform.position, player.transform.up, 1f);
            colDown = Physics.Raycast(transform.position, -player.transform.up, 1f);

            if (colLeft)
            {
                droneRigid.velocity += Vector3.Scale(player.transform.right, droneRigid.velocity);
            }
            else if (colRight)
            {
                droneRigid.velocity -= Vector3.Scale(player.transform.right, droneRigid.velocity);
            }



            if (colFront)
            {
                droneRigid.velocity -= Vector3.Scale(player.transform.forward, droneRigid.velocity);
            }
            else if (colBack)
            {
                droneRigid.velocity += Vector3.Scale(player.transform.forward, droneRigid.velocity);
            }

            if (colUp)
            {
                droneRigid.velocity -= Vector3.Scale(player.transform.up, droneRigid.velocity);
            }
            else if (colDown)
            {
                droneRigid.velocity += Vector3.Scale(player.transform.up, droneRigid.velocity);
            }
            */

            this.transform.rotation = camera.transform.rotation;
            

        }


       
    }
}

