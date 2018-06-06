using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class DroneBehaviour : MonoBehaviour
    {
        public Transform player;
        public StateManager playerState;
        public Camera camera;

        Vector3 position;
        Vector3 nearPosition;
        Vector3 smoothedPosition;
        Vector3 accelerativePosition;
        Vector3 relativePos;
        Quaternion rotation;

        bool firstEntry = true;

        float moveSpeed = 10f;
        float idleSpeed = 6f;
        float acceleration = 5f;
        float turnSpeed = 5f;

        Vector3 yPlaneView = new Vector3(0, 1.5f, 0);
        Vector3 looAtPlayerY = new Vector3(0, 1.2f, 0);

        float cooldownTimer = 0f;
        float randomCooldown = 2f;


        private void Start()
        {

            yPlaneView = new Vector3(0, 2f, 0);
            position = player.position + player.transform.right + player.transform.forward + yPlaneView;
        }

        void FixedUpdate()
        {
            relativePos = player.position + looAtPlayerY - transform.position;

            if ((player.transform.position - transform.position).magnitude < 3f)
            {
                rotation = Quaternion.LookRotation(relativePos);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, turnSpeed * Time.deltaTime);
            }
            else
            {
                rotation = camera.transform.rotation;
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, turnSpeed * Time.deltaTime);
            }

            position = player.position + yPlaneView;

            if ((position - transform.position).magnitude > 4f)
            {
                if (playerState.run)
                {
                    acceleration = 6f;
                }
                else
                {
                    acceleration = 3f;
                }

                if(playerState.moveAmount == 0)
                {
                    firstEntry = true;
                }
                else
                {
                    firstEntry = false;
                }

                smoothedPosition = Vector3.Lerp(transform.position, position, moveSpeed * Time.deltaTime);
                accelerativePosition = Vector3.Lerp(transform.position, smoothedPosition, acceleration * Time.deltaTime);
                transform.position = accelerativePosition;
            }


            if ((position - transform.position).magnitude <= 4f)
            {
                acceleration = 3f;

                if (firstEntry)
                {
                    
                    Vector3 randomPosition = position + Random.insideUnitSphere * 4f;
                    if (randomPosition.y < position.y - 1)
                    {
                        randomPosition += yPlaneView;
                    }
                    nearPosition = randomPosition;

                    firstEntry = false;
                }
                else if (cooldownTimer > randomCooldown)
                {
                    Vector3 randomPosition = position + Random.insideUnitSphere * 4f;
                    if (randomPosition.y < position.y - 1)
                    {
                        randomPosition += yPlaneView;
                    }
                    nearPosition = randomPosition;

                    cooldownTimer = 0;
                }

                smoothedPosition = Vector3.Lerp(transform.position, nearPosition, idleSpeed * Time.deltaTime);
                accelerativePosition = Vector3.Lerp(transform.position, smoothedPosition, acceleration * Time.deltaTime);
                transform.position = accelerativePosition;

                cooldownTimer += Time.deltaTime;
            }
        }
    }
}