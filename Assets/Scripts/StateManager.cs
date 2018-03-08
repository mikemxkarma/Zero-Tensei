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
        public bool isTwoHanded;
        public bool rollInput;
        public bool itemInput;
        public bool usingItem;

        [Header("Stats")]
        public float moveSpeed = 2;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;
        public float rollSpeed = 1;
        [Header("Other")]
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;
        public AnimationCurve roll_curve;

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockOn;
        public bool inAction;
        public bool canMove;



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
            if(a_hook ==null)
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

            usingItem = anim.GetBool("interacting");

            DetectItemAction(); 
            DetectAction();
            inventoryManager.rightHandWeapon.weaponModel.SetActive(!usingItem);

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
            rigidBody.drag = (moveAmount > 0|| onGround==false) ? 0 : 4;          

            float targetSpeed = moveSpeed;

            if (usingItem)
            {
                run = false;
                moveAmount = Mathf.Clamp(moveAmount, 0, 0.5f);
            }

            if (run)
                targetSpeed = runSpeed;

            if(onGround)
            rigidBody.velocity = moveDirection*(targetSpeed*moveAmount);

            if (run)
                lockOn = false;

          
                Vector3 targetDirection = (lockOn == false)? moveDirection
                    : (lockOnTransform != null)?
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
            if (canMove == false || usingItem)
                return;

            if (itemInput == false)
                return;

            ItemAction slot = actionManager.consumableItem;
            string targetAnimation = slot.targetAnimation;

            if (string.IsNullOrEmpty(targetAnimation))
                return;

           // inventoryManager.currentWeapon.weaponModel.SetActive(false);
            usingItem = true;
            // anim.CrossFade(targetAnimation, 0.2f);
            anim.Play(targetAnimation);

        }

        public void DetectAction()
        {
            if (canMove == false || usingItem)
                return;

            if (rb == false && rt == false && lt == false && lb == false)
                return;

            string targetAnimation = null;


            Action slot = actionManager.GetActionSlot(this);
            if (slot == null)
                return;

            targetAnimation = slot.targetAnimation;

            if (string.IsNullOrEmpty(targetAnimation))
                return;

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
                a_hook.rootMotionMultiplier = rollSpeed;
            }
            else
            {
                a_hook.rootMotionMultiplier = -5f;
            }
           

            anim.SetFloat("vertical", v);
            anim.SetFloat("horizontal", h);
            canMove = false;
            inAction = true;
            anim.CrossFade("Rolls", 0.2f);
            a_hook.InitForRoll();
        }

        void HandleMovementAnimations()
        {
            anim.SetBool("run", run);
            anim.SetFloat("vertical", moveAmount,0.4f,delta);
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
            anim.SetBool("twoHanded",isTwoHanded);

            if (isTwoHanded)
                actionManager.UpdateActionsTwoHanded();
            else
                actionManager.UpdateActionsOneHanded();
        }
        #endregion
    }   
}
