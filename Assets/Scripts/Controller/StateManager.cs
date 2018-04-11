using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///=================================================================
///   Namespace:      GameControll
///   Class:          StateManager
///   Description:    Handles the Input of Keyboard and Controller.
///   Date: 20-02-2018
///   Notes:
///   Revision History:   
///=================================================================

namespace GameControll
{

    public class StateManager : MonoBehaviour
    {
        #region Fields    
        [Header("Initialize")]
        public GameObject activeModel;
        [Header("Inputs")]
        public Vector3 moveDirection;
        public float moveAmount;
        public float horizontal;
        public float vertical;
        public bool rt, rb, lt, lb;

        public bool rollInput;
        public bool itemInput;


        [Header("Stats")]
        public float moveSpeed = 2;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;
        public float rollSpeed = 1;
        public float parryOffset = 1.4f;
        public float backStabOffset = 1.4f;

        [Header("Other")]
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;
        public AnimationCurve roll_curve;
        //public EnemyStates parryTarget;


        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool usingItem;
        public bool canBeParried;
        public bool parryIsOn;
        public bool isTwoHanded;
        public bool isBlocking;
        public bool isLeftHand;


        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigidBody;
        [HideInInspector]
        public AnimatorHook a_hook;
        [HideInInspector]
        public ActionManager actionManager;
        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayers;
        [HideInInspector]
        public InventoryManager inventoryManager;

        float _actionDelay;
        #endregion

        #region Methods
        public void Init()
        {
            SetupAnimator();
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.angularDrag = 999;
            rigidBody.drag = 4;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            inventoryManager = GetComponent<InventoryManager>();
            inventoryManager.Init(this);

            actionManager = GetComponent<ActionManager>();
            actionManager.Init(this);

            a_hook = activeModel.GetComponent<AnimatorHook>();
            if (a_hook == null)
                a_hook = activeModel.AddComponent<AnimatorHook>();

            a_hook.Init(this, null);



            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);
            anim.SetBool("onGround", true);
        }

        void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    Debug.Log("No model found");
                }
                else
                {
                    activeModel = anim.gameObject;
                }
            }
            if (anim == null)
                anim = activeModel.GetComponent<Animator>();

            anim.applyRootMotion = false;
        }

        public void FixedTick(float d)
        {
            delta = d;

            isBlocking = false;
            usingItem = anim.GetBool("interacting");
            DetectAction();
            DetectItemAction();
            inventoryManager.rightHandWeapon.weaponModel.SetActive(!usingItem);

            anim.SetBool("blocking", isBlocking);
            anim.SetBool("isLeft", isLeftHand);

            if (inAction)
            {
                anim.applyRootMotion = true;
                _actionDelay += delta;
                if (_actionDelay > 0.3f)
                {
                    inAction = false;
                    _actionDelay = 0;
                }
                else
                {

                    return;
                }
            }
            canMove = anim.GetBool("canMove");

            if (!canMove)
            {
                return;
            }
            // a_hook.rootMotionMultiplier = 1;
            a_hook.CloseRoll();
            HandleRolls();


            anim.applyRootMotion = false;
            rigidBody.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;

            float targetSpeed = moveSpeed;

            if (usingItem)
            {
                run = false;
                moveAmount = Mathf.Clamp(moveAmount, 0, 0.5f);
            }

            if (run)
                targetSpeed = runSpeed;

            if (onGround)
                rigidBody.velocity = moveDirection * (targetSpeed * moveAmount);

            if (run)
                lockOn = false;


            Vector3 targetDirection = (lockOn == false) ? moveDirection
                : (lockOnTransform != null) ?
                lockOnTransform.transform.position - transform.position
                :
                moveDirection;

            targetDirection.y = 0;
            if (targetDirection == Vector3.zero)
                targetDirection = transform.forward;
            Quaternion tRotation = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tRotation, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;

            anim.SetBool("lockOn", lockOn);

            if (lockOn == false)
                HandleMovementAnimations();
            else
                HandleLockOnAnimations(moveDirection);
        }

        public void DetectItemAction()
        {
            if (canMove == false || usingItem || isBlocking)
                return;

            if (itemInput == false)
                return;

            ItemAction slot = actionManager.consumableItem;
            string targetAnimation = slot.targetAnimation;

            if (string.IsNullOrEmpty(targetAnimation))
                return;

            // inventoryManager.currentWeapon.weaponModel.SetActive(false);
            usingItem = true;
            anim.Play(targetAnimation);

        }

        public void DetectAction()
        {
            if (canMove == false || usingItem)
                return;

            if (rb == false && rt == false && lt == false && lb == false)
                return;

            Action slot = actionManager.GetActionSlot(this);
            if (slot == null)
                return;

            switch (slot.type)
            {
                case ActionType.attack:
                    AttackAction(slot);
                    break;
                case ActionType.block:
                    BlockAction(slot);
                    break;
                case ActionType.spells:
                    break;
                case ActionType.parry:
                    ParryAction(slot);
                    break;
                default:
                    break;
            }

        }

        void AttackAction(Action slot)
        {
            if (CheckForBackStab(slot))
                return;
            if (CheckForParry(slot))
                return;


            string targetAnimation = null;
            targetAnimation = slot.targetAnimation;

            if (string.IsNullOrEmpty(targetAnimation))
                return;

            canMove = false;
            inAction = true;

            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }

            canBeParried = slot.canBeParried;
            anim.SetFloat("animSpeed", targetSpeed);
            anim.SetBool("mirror", slot.mirror);
            anim.CrossFade(targetAnimation, 0.2f);
        }

        bool CheckForParry(Action slot)
        {
            EnemyStates parryTarget = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDirection = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, rayDirection, out hit, 3, ignoreLayers))
            {
                parryTarget = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if (parryTarget == null)
            {
                return false;
            }
            if (parryTarget.parriedBy == null)
                return false;



            /*  float dis = Vector3.Distance(parryTarget.transform.position, transform.position);

             if (dis > 3)
                 return false;
            */
            Vector3 dir = parryTarget.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle < 60)
            {
                Vector3 targetPosition = -dir * parryOffset;
                targetPosition += parryTarget.transform.position;
                transform.position = targetPosition;

                if (dir == Vector3.zero)
                    dir = -parryTarget.transform.forward;

                Quaternion eRotation = Quaternion.LookRotation(-dir);
                Quaternion ourRot = Quaternion.LookRotation(dir);

                parryTarget.transform.rotation = eRotation;
                transform.rotation = ourRot;
                parryTarget.IsGettingParried();
                canMove = false;
                inAction = true;
                anim.SetBool("mirror", slot.mirror);
                anim.CrossFade("parry_attack", 0.2f);

                return true;
            }

            return false;
        }

        bool CheckForBackStab(Action slot)
        {
            if (slot.canBackstab == false)
                return false;

            EnemyStates backstab = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDirection = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, rayDirection, out hit, 1, ignoreLayers))
            {
                backstab = hit.transform.GetComponentInParent<EnemyStates>();
            }
            if (backstab == null)
                return false;

            Vector3 direction = transform.position - backstab.transform.position;
            direction.Normalize();
            direction.y = 0;
            float angle = Vector3.Angle(backstab.transform.forward, direction);

            Debug.Log("F");
            if (angle > 150)
            {
                Vector3 targetPosition = direction * backStabOffset;
                targetPosition += backstab.transform.position;
                transform.position = targetPosition;


                if(direction == Vector3.zero)
                {
                    direction = backstab.transform.forward;
                }
                Quaternion ourRotation = Quaternion.LookRotation(direction);

                transform.rotation = transform.rotation;
                backstab.IsGettingBackstabbed();
                canMove = false;
                inAction = true;

                anim.SetBool("mirror", slot.mirror);
                anim.CrossFade("parry_attack", 0.2f);

                lockOnTarget = null;
                return true;
            }
            return false;
        }

        void BlockAction(Action slot)
        {
            isBlocking = true;
            isLeftHand = slot.mirror;//if its mirror , block with left hand
        }

        void ParryAction(Action slot)
        {
            string targetAnimation = null;
            targetAnimation = slot.targetAnimation;

            if (string.IsNullOrEmpty(targetAnimation))
                return;

            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }

            anim.SetFloat("animSpeed", targetSpeed);
            canBeParried = slot.canBeParried;
            canBeParried = slot.canBeParried;
            canMove = false;
            inAction = true;
            anim.SetBool("mirror", slot.mirror);
            anim.CrossFade(targetAnimation, 0.2f);
        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            anim.SetBool("onGround", onGround);
        }

        void HandleRolls()
        {
            if (!rollInput || usingItem)
                return;

            float v = vertical;
            float h = horizontal;
            v = (moveAmount > 0.3f) ? 1 : 0;
            h = 0;


            /*   if (lockOn == false)
               {
                   v = (moveAmount > 0.3f) ? 1:0 ;
                   h = 0;
               }
               else
               {
                   //eliminate smal amounts of input
                   if (Mathf.Abs(v) < 0.3f)
                       v = 0;
                   if (Mathf.Abs(h) < 0.3f)
                       h = 0;

               }
               */
            if (v != 0)
            {
                if (moveDirection == Vector3.zero)
                    moveDirection = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(moveDirection);
                transform.rotation = targetRot;
                a_hook.InitForRoll();
                a_hook.rm_multi = rollSpeed;
            }
            else
            {
                a_hook.rm_multi = 1.3f;
            }


            anim.SetFloat("vertical", v);
            anim.SetFloat("horizontal", h);

            canMove = false;
            inAction = true;
            anim.CrossFade("Rolls", 0.2f);
        }

        void HandleMovementAnimations()
        {
            anim.SetBool("run", run);
            anim.SetFloat("vertical", moveAmount, 0.4f, delta);
        }

        void HandleLockOnAnimations(Vector3 moveDirection)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDirection);
            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat("vertical", v, 0.2f, delta);
            anim.SetFloat("horizontal", h, 0.2f, delta);
        }

        public bool OnGround()
        {
            bool r = false;
            Vector3 origin = transform.position + (Vector3.up * toGround);
            Vector3 direction = -Vector3.up;
            float distance = toGround + 0.3f;
            RaycastHit hit;
            //Debug.DrawRay(origin, direction * distance);
            if (Physics.Raycast(origin, direction, out hit, distance, ignoreLayers))
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;
            }
            return r;
        }

        public void HandlerTwoHanded()
        {
            anim.SetBool("twoHanded", isTwoHanded);

            if (isTwoHanded)
                actionManager.UpdateActionsTwoHanded();
            else
                actionManager.UpdateActionsOneHanded();
        }

        public void IsGettingParried()
        {

        }

        #endregion
    }
}
