using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class AnimatorHook : MonoBehaviour
    {

        Animator anim;
        StateManager states;
        EnemyStates eStates;
        Rigidbody rigid;

        public float rm_multi;
        bool rolling;
        float roll_t;
        float delta;
        AnimationCurve roll_curve;

        public Transform ikTarget;
        public Transform bodyTarget;
        public Transform headTarget;

        public Transform ikTargetShield;
        public Transform ikTargetBodyShield;

        HandleIK ik_handler;
        public bool useIk;
        public AvatarIKGoal currentHand;

        public bool killDelta;

        public void Init(StateManager st, EnemyStates eSt)
        {
            states = st;
            eStates = eSt;

            if (st != null)
            {
                anim = st.anim;
                rigid = st.rigidBody;
                roll_curve = states.roll_curve;
                delta = st.delta;
            }

            if (eSt != null)
            {
                anim = eSt.anim;
                rigid = eSt.rigid;
                delta = eSt.delta;
            }

            ik_handler = gameObject.GetComponent<HandleIK>();
            if (ik_handler != null)
                ik_handler.Init(anim);
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

            rm_multi = 1;
            roll_t = 0;
            rolling = false;
        }

        public void OnAnimatorMove()
        {
            if (ik_handler != null)
            {
                ik_handler.OnAnimatorMoveTick(currentHand == AvatarIKGoal.LeftHand);
            }

            if (states == null && eStates == null)
                return;

            if (rigid == null)
                return;

            if (states != null)
            {
                if (states.canMove)
                    return;

                delta = states.delta;
            }

            if (eStates != null)
            {
                if (eStates.canMove)
                    return;

                delta = eStates.delta;
            }

            rigid.drag = 0;

            if (rm_multi == 0)
                rm_multi = 1;

            if (rolling == false)
            {
                Vector3 delta2 = anim.deltaPosition;
                if (killDelta)
                {
                    killDelta = false;
                    delta2 = Vector3.zero;
                }

                Vector3 v = (delta2 * rm_multi) / delta;
                //v += Physics.gravity;

                rigid.velocity = v;
            }
            else
            {
                roll_t += delta / 0.6f;

                if (roll_t > 1)
                {
                    roll_t = 1;
                }

                if (states == null)
                    return;

                float zValue = roll_curve.Evaluate(roll_t);
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rm_multi);

                rigid.velocity = v2;
            }
        }

        void OnAnimatorIK()
        {
            if (ik_handler == null)
                return;

            if (!useIk)
            {
                if (ik_handler.weight > 0)
                {
                    ik_handler.IKTick(currentHand, 0);
                }
                else
                {
                    ik_handler.weight = 0;
                }
            }
            else
            {
                ik_handler.IKTick(currentHand, 1);
            }
        }

        void LateUpdate()
        {
            if (ik_handler != null)
                ik_handler.LateTick();
        }

        public void OpenAttack()
        {
            if (states)
            {
                states.canAttack = true;
            }

            Debug.Log("can attack");
        }

        public void OpenCanMove()
        {
            if (states)
            {
                states.canMove = true;
            }

            Debug.Log("can move");
        }

        public void OpenDamageColliders()
        {
            if (states)
            {
                states.damageIsOn = true;
                states.inventoryManager.OpenAllDamageColliders();
            }

            OpenParryFlag();
        }

        public void CloseDamageColliders()
        {
            if (states)
            {
                states.damageIsOn = false;
                states.inventoryManager.CloseAllDamageColliders();
            }

            CloseParryFlag();
        }

        public void OpenParryCollider()
        {
            if (states == null)
                return;

            states.inventoryManager.OpenParryCollider();
        }

        public void CloseParryCollider()
        {
            if (states == null)
                return;

            states.inventoryManager.CloseParryCollider();
        }

        public void OpenParryFlag()
        {
            if (states)
            {
                states.parryIsOn = true;
            }

            if (eStates)
            {
                eStates.parryIsOn = true;
            }
        }

        public void CloseParryFlag()
        {
            if (states)
            {
                states.parryIsOn = false;
            }

            if (eStates)
            {
                eStates.parryIsOn = false;
            }
        }
        public void OpenRotationControl()
        {
            if (states)
            {
                states.canRotate = true;
            }
        }

        public void CloseRotationControl()
        {
            if (states)
            {
                states.canRotate = false;
            }
        }

        public void CloseParticle()
        {
            if (states)
            {
                if (states.inventoryManager.currentSpell.currentParticle != null)
                    states.inventoryManager.currentSpell.currentParticle.SetActive(false);
            }
        }

        public void InitiateThrowForProjectile()
        {
            if (states)
            {
                states.ThrowProjectile();
            }
        }

        public void InitIKForShield(bool isLeft)
        {
            ik_handler.UpdateIKTargets((isLeft) ? IKSnapshotType.shield_l : IKSnapshotType.shield_r, isLeft);
        }

        public void InitIKForBreathSpell(bool isLeft)
        {
            ik_handler.UpdateIKTargets(IKSnapshotType.breath, isLeft);
        }

    }
}
