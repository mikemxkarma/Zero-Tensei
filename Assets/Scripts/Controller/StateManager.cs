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
        public float horizontal;
        public float vertical;
        public Vector3 moveDirection;
        public float moveAmount;
        public bool rt, rb, lt, lb;
        public bool rollInput;
        public bool itemInput;


        [Header("Stats")]
        //public Attributes attributes;
        public CharacterStats characterStats;
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
        public bool isSpellcasting;
        public bool enableIK;
        public bool canMove;
        public bool canAttack;
        public bool onEmpty;
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
        public InventoryManager inventoryManager;
        //[HideInInspector]
        //public BoneHelper boneHelper;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayers;
        [HideInInspector]
        // for damage check
        public Action currentAction;

        [HideInInspector]
        public float airTimer;
        public ActionInput storePrevActionInput;
        public ActionInput storeActionInput;

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

            //boneHelper = gameObject.AddComponent<BoneHelper>();

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

            anim.SetBool(StaticStrings.onGround, true);

            characterStats.InitCurrent();

            UIManager.singleton.AffectAll(characterStats.hp, characterStats.fp, characterStats.stamina);
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
            usingItem = anim.GetBool(StaticStrings.interacting);
            anim.SetBool(StaticStrings.spellcasting, isSpellcasting);
            inventoryManager.rightHandWeapon.weapon_Model.SetActive(!usingItem);

            if (isBlocking == false && isSpellcasting == false)
            {
                enableIK = false;
            }

            a_hook.useIk = enableIK;

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

            onEmpty = anim.GetBool(StaticStrings.onEmpty);

            if (onEmpty)
            {
                canAttack = true;
                canMove = true;
            }

            if (!onEmpty && !canMove && !canAttack)
                return;

            if (canMove && !onEmpty)
            {
                if (moveAmount > 0.3f)
                {
                    anim.CrossFade("Empty Override", 0.1f);
                    onEmpty = true;
                }
            }

            if (canAttack)
                DetectAction();
            if (!canMove)
            {
                DetectItemAction();
            }


            anim.applyRootMotion = false;
            rigidBody.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;

            float targetSpeed = moveSpeed;

            if (usingItem || isSpellcasting)
            {
                run = false;
                moveAmount = Mathf.Clamp(moveAmount, 0, 0.5f);
            }

            if (run)
                targetSpeed = runSpeed;

            if (onGround && canMove)
                rigidBody.velocity = moveDirection * (targetSpeed * moveAmount);

            if (run)
                lockOn = false;


            HandleRotation();
            anim.SetBool(StaticStrings.lockOn, lockOn);

            if (lockOn == false)
                HandleMovementAnimations();
            else
                HandleLockOnAnimations(moveDirection);

           // anim.SetBool(StaticStrings.blocking, isBlocking);
            anim.SetBool(StaticStrings.isLeft, isLeftHand);

            HandleBlocking();

            if (isSpellcasting)
            {
                HandleSpellcasting();
                return;
            }


            a_hook.CloseRoll();
            HandleRolls();
        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            anim.SetBool(StaticStrings.onGround, onGround);

            if (!onGround)
                airTimer += delta;
            else
                airTimer = 0;
        }

        public bool IsInput()
        {
            if (rt || rb || lt || lb || rollInput)
                return true;

            return false;
        }

        void HandleRotation()
        {
            Vector3 targetDirection = (lockOn == false) ? moveDirection :
                (lockOnTransform != null) ?
                lockOnTransform.transform.position - transform.position : moveDirection;

            targetDirection.y = 0;
            if (targetDirection == Vector3.zero)
                targetDirection = transform.forward;
            Quaternion tRotation = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tRotation, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;
        }

        public void DetectItemAction()
        {
            if (onEmpty == false || usingItem || isBlocking)
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
            if (canAttack == false && (onEmpty == false || usingItem || isSpellcasting))
                return;

            if (rb == false && rt == false && lt == false && lb == false)
                return;

            ActionInput targetInput = actionManager.GetActionInput(this);
            storeActionInput = targetInput;
            if (onEmpty == false)
            {
                a_hook.killDelta = true;
                targetInput = storePrevActionInput;
            }

            storePrevActionInput = targetInput;
            Action slot = actionManager.GetActionFromInput(targetInput);

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
                    SpellAction(slot);
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

            if (characterStats._stamina < slot.staminaCost)
                return;
            if (CheckForParry(slot))
                return;
            if (CheckForBackStab(slot))
                return;

            string targetAnimation = null;
            targetAnimation =
                slot.GetActionStep(ref actionManager.actionIndex)
                .GetBranch(storeActionInput).targetAnim;

            Debug.Log(storeActionInput);

            if (string.IsNullOrEmpty(targetAnimation))
                return;

            currentAction = slot;

            canAttack = false;
            onEmpty = false;
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
            anim.SetFloat(StaticStrings.animSpeed, targetSpeed);
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            //anim.CrossFade(targetAnimation, 0.2f);
            anim.Play(targetAnimation);
            characterStats._stamina -= slot.staminaCost;
        }

        bool IsLeftHandSlot(Action slot)
        {
            return (slot.input == ActionInput.lb || slot.input == ActionInput.lt);
        }

        void SpellAction(Action slot)
        {
            if (slot.spellClass != inventoryManager.currentSpell.instance.spellClass
                || characterStats._stamina < slot.staminaCost || characterStats._mana < slot.manaCost)
            {
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.CrossFade("cant_spell", 0.2f);
                canAttack = false;
                onEmpty = false;
                canMove = false;
                inAction = true;
                return;
            }

            ActionInput inp = actionManager.GetActionInput(this);
            if (inp == ActionInput.lb)
                inp = ActionInput.rb;
            if (inp == ActionInput.lt)
                inp = ActionInput.rt;

            Spell s_inst = inventoryManager.currentSpell.instance;
            SpellAction s_slot = s_inst.GetSpellAction(s_inst.spell_Actions, inp);
            if (s_slot == null)
            {
                Debug.Log("Cant find spell slot");
                return;
            }

            SpellEffectsManager.singleton.UseSpellEffect(s_inst.spell_effect, this);

            isSpellcasting = true;
            spellcastTime = 0;
            max_spellcastTime = s_slot.castTime;
            spellTargetAnim = s_slot.throwAnimation;
            spellIsMirrored = slot.mirror;
            curSpellType = s_inst.spellType;

            string targetAnim = s_slot.targetAnimation;
            if (spellIsMirrored)
                targetAnim += StaticStrings._l;
            else
                targetAnim += StaticStrings._r;

            projectileCandidate = inventoryManager.currentSpell.instance.projectile;
            inventoryManager.CreateSpellParticle(inventoryManager.currentSpell, spellIsMirrored,
                (s_inst.spellType == SpellType.looping));
            anim.SetBool(StaticStrings.spellcasting, true);
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(targetAnim, 0.2f);

            //characterStats._stamina -= slot.staminaCost;
            characterStats._mana -= slot.manaCost;

            a_hook.InitIKForBreathSpell(spellIsMirrored);

            if (spellCast_start != null)
                spellCast_start();
        }

        float spellcastTime;
        float max_spellcastTime;
        string spellTargetAnim;
        bool spellIsMirrored;
        SpellType curSpellType;
        GameObject projectileCandidate;

        public delegate void SpellCast_Start();
        public delegate void SpellCast_Loop();
        public delegate void SpellCast_Stop();
        public SpellCast_Start spellCast_start;
        public SpellCast_Loop spellCast_loop;
        public SpellCast_Stop spellCast_stop;

        void HandleSpellcasting()
        {
            if (curSpellType == SpellType.looping)
            {
                enableIK = true;
                a_hook.currentHand = (spellIsMirrored) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;



                if (rb == false && lb == false || characterStats._mana < 2)
                {
                    isSpellcasting = false;

                    enableIK = false;

                    inventoryManager.breathCollider.SetActive(false);
                    inventoryManager.blockCollider.SetActive(false);

                    if (spellCast_stop != null)
                        spellCast_stop();

                    return;
                }
                if (spellCast_loop != null)
                    spellCast_loop();

                characterStats._mana -= 0.5f;

                return;
            }

            spellcastTime += delta;

            if (inventoryManager.currentSpell.currentParticle != null)
                inventoryManager.currentSpell.currentParticle.SetActive(true);

            if (spellcastTime > max_spellcastTime)
            {
                canAttack = false;
                onEmpty = false;
                canMove = false;
                inAction = true;
                isSpellcasting = false;

                string targetAnim = spellTargetAnim;
                anim.SetBool(StaticStrings.mirror, spellIsMirrored);
                anim.CrossFade(targetAnim, 0.2f);
            }
        }

        bool blockAnim;
        string block_idle_anim;

        void HandleBlocking()
        {
            if (isBlocking == false)
            {
                if (blockAnim)
                {
                    anim.CrossFade(block_idle_anim, 0.1f);
                    blockAnim = false;
                }
            }
            else
            {

            }
        }

        public void ThrowProjectile()
        {
            if (projectileCandidate == null)
                return;

            GameObject go = Instantiate(projectileCandidate) as GameObject;
            Transform p = anim.GetBoneTransform((spellIsMirrored) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            go.transform.position = p.position;

            if (lockOnTransform && lockOn)
                go.transform.LookAt(lockOnTransform.position);
            else
                go.transform.rotation = transform.rotation;

            Projectile proj = go.GetComponent<Projectile>();
            proj.Init();

            //characterStats._stamina -= cur_stamCost;
            //characterStats._mana -= cur_focusCost;
        }

        bool CheckForParry(Action slot)
        {
            //  if (slot.canParry == false)
            //    return false;

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


                parryTarget.IsGettingParried(slot, inventoryManager.GetCurrentWeapon(slot.mirror));
                canAttack = false;
                onEmpty = false;
                canMove = false;
                inAction = true;
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.CrossFade(StaticStrings.parry_attack, 0.2f);
                lockOnTarget = null;
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

            if (angle > 150)
            {
                Vector3 targetPosition = direction * backStabOffset;
                targetPosition += backstab.transform.position;
                transform.position = targetPosition;

                transform.rotation = transform.rotation;
                backstab.IsGettingBackstabbed(slot, inventoryManager.GetCurrentWeapon(slot.mirror));
                onEmpty = false;
                canMove = false;
                inAction = true;
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.CrossFade(StaticStrings.parry_attack, 0.2f);
                lockOnTarget = null;
                return true;
            }
            return false;
        }

        void BlockAction(Action slot)
        {
            isBlocking = true;
            onEmpty = false;
            enableIK = true;
            a_hook.currentHand = (slot.mirror) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;
            a_hook.InitIKForShield((slot.mirror));

            if (blockAnim == false)
            {
                block_idle_anim =
                    (isTwoHanded == false) ?
                    inventoryManager.GetCurrentWeapon(isLeftHand).oh_idle
                    : inventoryManager.GetCurrentWeapon(isLeftHand).th_idle;

                block_idle_anim += (isLeftHand) ? "_l" : "_r";

                string targetAnim = slot.targetAnimation;
                targetAnim += (isLeftHand) ? "_l" : "_r";
                anim.CrossFade(targetAnim, 0.1f);
                blockAnim = true;
            }
        }

        void ParryAction(Action slot)
        {
            string targetAnimation = null;

            targetAnimation =
                slot.GetActionStep(ref actionManager.actionIndex)
                .GetBranch(storeActionInput).targetAnim;

            if (string.IsNullOrEmpty(targetAnimation))
                return;

            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }

            anim.SetFloat(StaticStrings.animSpeed, targetSpeed);
            canBeParried = slot.canBeParried;
            onEmpty = false;
            canMove = false;
            inAction = true;
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(targetAnimation, 0.2f);
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


            anim.SetFloat(StaticStrings.vertical, v);
            anim.SetFloat(StaticStrings.horizontal, h);

            onEmpty = false;
            canMove = false;
            inAction = true;
            anim.CrossFade(StaticStrings.Rolls, 0.2f);
            isBlocking = false;
        }

        void HandleMovementAnimations()
        {
            anim.SetBool(StaticStrings.run, run);
            anim.SetFloat(StaticStrings.vertical, moveAmount, 0.4f, delta);
        }

        void HandleLockOnAnimations(Vector3 moveDirection)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDirection);
            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat(StaticStrings.vertical, v, 0.2f, delta);
            anim.SetFloat(StaticStrings.horizontal, h, 0.2f, delta);
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
            bool isRight = true;
            Weapon w = inventoryManager.rightHandWeapon.instance;
            if (w == null)
            {
                w = inventoryManager.leftHandWeapon.instance;
                isRight = false;
            }

            if (w == null)
                return;

            if (isTwoHanded)
            {
                anim.CrossFade(w.th_idle, 0.2f);
                actionManager.UpdateActionsTwoHanded();

                if (isRight)
                {
                    if (inventoryManager.leftHandWeapon != null)
                        inventoryManager.leftHandWeapon.weapon_Model.SetActive(false);
                }
                else
                {
                    if (inventoryManager.rightHandWeapon != null)
                        inventoryManager.rightHandWeapon.weapon_Model.SetActive(false);
                }
            }

            else
            {
                string targetAnim = w.oh_idle;
                targetAnim += (isRight) ? StaticStrings._r : StaticStrings._l;
                anim.Play(StaticStrings.equipWeapon_oh);
                //anim.Play(StaticStrings.emptyBoth);
                actionManager.UpdateActionsOneHanded();

                if (isRight)
                {
                    if (inventoryManager.leftHandWeapon != null)
                        inventoryManager.leftHandWeapon.weapon_Model.SetActive(true);
                }
                else
                {
                    if (inventoryManager.rightHandWeapon != null)
                        inventoryManager.rightHandWeapon.weapon_Model.SetActive(true);
                }
            }
        }

        public void IsGettingParried()
        {

        }

        public void AddHealth()
        {
            characterStats.hp++;
        }

        public void MonitorStats()
        {
            if (run && moveAmount > 0)
            {
                Debug.Log("running");
                characterStats._stamina -= delta * 5;
            }
            else
            {
                characterStats._stamina += delta;
            }

            if (characterStats._stamina > characterStats.fp)
                characterStats._stamina = characterStats.fp;

            characterStats._health = Mathf.Clamp(characterStats._health, 0, characterStats.hp);
            characterStats._mana = Mathf.Clamp(characterStats._mana, 0, characterStats.fp);
        }

        #endregion
    }
}
