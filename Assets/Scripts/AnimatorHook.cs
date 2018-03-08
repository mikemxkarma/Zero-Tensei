using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class AnimatorHook : MonoBehaviour
    {
         Animator anim;
        StateManager states;
        EnemyStates enemyStates;
        Rigidbody rigidBody;

        bool isRolling;

        public float rootMotionMultiplier;
         bool rolling;
        float roll_t;
        float delta;
        AnimationCurve roll_curve;

        public void Init(StateManager st, EnemyStates eST)
        {
            states  = st;
            enemyStates = eST;
            if (st != null)
            {
                anim = st.anim;
                rigidBody = st.rigidBody;
                roll_curve = states.roll_curve;
                delta = st.delta;
            }
            if (eST != null)
            {
                anim = eST.anim;
                rigidBody = eST.rigid;
                delta = eST.delta;
            }
        }

        public void InitForRoll()
        {
            rolling = true;
            roll_t = 0;
        }
        public void CloseRoll()
        {
            if (rolling == false)
                return;

            rootMotionMultiplier = 1;
            roll_t = 0;
            rolling = false;
        }

        void OnAnimatorMove()
        {

         if(states ==null && enemyStates ==null)
                return;

            if (rigidBody == null)
                return;
         
            if (states != null)
            {
                if (states.canMove)
                    return;

                delta = states.delta;
            }

            if (enemyStates != null)
            {
                if (enemyStates.canMove)
                    return;

                delta = enemyStates.delta;
            }

            rigidBody.drag = 0;

            if (rootMotionMultiplier == 0)
                rootMotionMultiplier = 1;

            if (rolling == false)
            {
                Vector3 delta2 = anim.deltaPosition;
                delta2.y = 0;
                Vector3 v = (delta2 * rootMotionMultiplier) / delta;
               rigidBody.velocity = v;
            }
            else
            {
                roll_t += delta/0.60f ; // sample the animation curve

                if (roll_t > 1)
                {
                    roll_t = 1;              
                }

                if (states == null)
                    return;

                float zValue = roll_curve.Evaluate(roll_t);
                Vector3 v1 = Vector3.forward * zValue;               
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rootMotionMultiplier);// / states.delta;
                rigidBody.velocity = v2;
            }
        }

        public void OpenDamageColliders()
        {
            if (states == null)
                return;
            states.inventoryManager.CloseAllDamageColliders();
        }

        public void CloseDamageColliders()
        {
            if (states == null)
                return;
            states.inventoryManager.CloseAllDamageColliders();
        }
    }
}