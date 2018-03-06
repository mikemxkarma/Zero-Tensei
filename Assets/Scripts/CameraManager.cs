using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///=================================================================
///   Namespace:      GameControll
///   Class:          CameraManager
///   Description:    Handles the Input of Keyboard and Controller.
///   Date: 20-02-2018
///   Notes:
///   Revision History:   
///=================================================================
///
namespace GameControll
{

    public class CameraManager : MonoBehaviour
    {
 
        public float followSpeed = 500;
        public float mouseSpeed = 2;
        public float controllerSpeed = 20;       
        public Transform target;
        public EnemyTarget lockonTarget;
        public Transform lockonTransform;
        [HideInInspector]
        public Transform pivot;
        [HideInInspector]
        public Transform camTransform;
        StateManager states;

        float turnSmoothing = 0.1f;
        public float minAngle = -35;
        public float maxAngle = 35;
        public float lookAngle;
        public float tiltAngle;

        bool usedRightAxis;

        public bool lockOnMode;
        float smoothX;
        float smoothY;
        float smoothXvelocity;
        float smoothYvelocity;

        public void Init(StateManager st)
        {
            states = st;
            target = st.transform;
            camTransform = Camera.main.transform;
            pivot = camTransform.parent;
        }

        public void Tick(float d)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            float c_h = Input.GetAxis("RightAxis X");
            float c_v = Input.GetAxis("RightAxis Y");

            float targetSpeed = mouseSpeed;

            if (lockonTarget != null)
            {
                if (lockonTransform == null)
                {
                    lockonTransform = lockonTarget.GetTarget();
                    states.lockOnTransform = lockonTransform;
                }

                if (Mathf.Abs(c_h) > 0.6f)
                {
                    if (!usedRightAxis)
                    {
                        lockonTransform = lockonTarget.GetTarget((c_h > 0));
                        states.lockOnTransform = lockonTransform;
                        usedRightAxis = true;
                    }

                }
            }

            if (usedRightAxis)
            {
                if (Mathf.Abs(c_h) < 0.6f)
                {
                    usedRightAxis = false;

                }
            }

            if (c_h != 0 || c_v != 0)//mouse input over keyboard overlap
            {
                h = c_h;
                v = c_v;
                targetSpeed = controllerSpeed;
            }
            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
        }

        void FollowTarget(float d)
        {
            float speed = d * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        void HandleRotations(float d, float v, float h, float targetSpeed)
        {
            if (turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXvelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYvelocity, turnSmoothing);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);


            if (lockOnMode && lockonTarget != null)
            {
                Vector3 targetDir = lockonTransform.position - transform.position;
                targetDir.Normalize();
                // targetDir.y = 0;
            
                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, d * 9);
                lookAngle = transform.eulerAngles.y;
                return;
            }
 
            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);//rotate camara Yaxis

        }


        public static CameraManager singleton;
         void Awake()
        {
            singleton = this;
        }

    }
}