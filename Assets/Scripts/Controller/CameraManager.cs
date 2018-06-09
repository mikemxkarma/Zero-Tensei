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
        public float defZ;
        float curZ;
        public float zSpeed = 19;
        float smoothX;
        float smoothY;
        float smoothXvelocity;
        float smoothYvelocity;

        public bool lockOnMode;
        bool usedRightAxis;

        bool changeTargetLeft;
        bool changeTargetRight;

        public void Init(StateManager st)
        {
            states = st;
            target = st.transform;
            camTransform = Camera.main.transform;
            pivot = camTransform.parent;
            curZ = defZ;
        }

        public void Tick(float d)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            float c_h = Input.GetAxis("RightAxis X");
            float c_v = Input.GetAxis("RightAxis Y");

            float targetSpeed = mouseSpeed;

            changeTargetRight = Input.GetKeyUp(KeyCode.V);
            changeTargetRight = Input.GetKeyUp(KeyCode.B);



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
                if(changeTargetLeft || changeTargetRight)
                {
                    lockonTransform = lockonTarget.GetTarget(changeTargetLeft);
                    states.lockOnTransform = lockonTransform;
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
                v = -c_v;
                targetSpeed = controllerSpeed;
            }
            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
            HandlePivotPosition();
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
                Vector3 targetDir = lockonTransform.position - transform.position - new Vector3(0, lockonTransform.position.y);
                targetDir.Normalize();
                targetDir.y = 0;

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
        void HandlePivotPosition()
        {
            float targetZ = defZ;
            CameraCollision(defZ, ref targetZ);

            curZ = Mathf.Lerp(curZ, targetZ, states.delta * zSpeed);
            Vector3 tp = Vector3.zero;
            tp.z = curZ;
            camTransform.localPosition = tp;
        }

        void CameraCollision(float targetZ, ref float actualZ)
        {
            float step = Mathf.Abs(targetZ);
            int stepCount = 2;
            float stepIncrement = step / stepCount;

            RaycastHit hit;
            Vector3 origin = pivot.position;
            Vector3 direction = -pivot.forward;

            //Debug.DrawRay(origin, direction * step, Color.blue);
            if (Physics.Raycast(origin, direction, out hit, step, states.ignoreLayers))
            {
                float distance = Vector3.Distance(hit.point, origin);
                actualZ = -(distance / 2);

                   //Debug.Log(hit.transform.root.name);
            }
            else
            {
                for (int s = 0; s < stepCount + 1; s++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 dir = Vector3.zero;
                        Vector3 secondOrigin = origin + (direction * s) * stepIncrement;

                        switch (i)
                        {
                            case 0:
                                dir = camTransform.right;
                                break;
                            case 1:
                                dir = -camTransform.right;
                                break;
                            case 2:
                                dir = camTransform.up;
                                break;
                            case 3:
                                dir = -camTransform.up;
                                break;
                        }

                         //Debug.DrawRay(secondOrigin, dir * 0.2f, Color.red);
                        if (Physics.Raycast(secondOrigin, dir, out hit, 0.2f, states.ignoreLayers))
                        {
                             // Debug.Log(hit.transform.root.name);
                            float distance = Vector3.Distance(secondOrigin, origin);
                            actualZ = -(distance / 2);
                            if (actualZ < 0.2f)
                                actualZ = 0;

                            return;
                        }
                    }
                }
            }
        }


        public static CameraManager singleton;
        void Awake()
        {
            singleton = this;
        }

    }
}