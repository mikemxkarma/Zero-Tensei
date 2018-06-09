using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GameControll
{
    public class EnemyStates : MonoBehaviour
    {
        [Header("Stats")]
        public int health;
        public float airTimer;
        public CharacterStats characterStats;

        [Header("Values")]
        public float delta;
        public float horizontal;
        public float vertical;

        AIAttacks curAttack;
        public void SetCurAttack(AIAttacks a)
        {
            curAttack = a;
        }

        public AIAttacks GetCurAttack()
        {
            return curAttack;
        }

        public GameObject[] defaultDamageColliders;


        [Header("States")]
        public bool canBeParried = true;
        public bool parryIsOn = true;
        //    public bool doParry = false;
        public bool isInvicible;
        public bool dontDoAnything;
        public bool canMove;
        public bool isDead;
        public bool hasDestination;
        public Vector3 targetDestination;
        public Vector3 dirToTarget;
        public bool rotateToTarget;

        Vector3 moveDirection;
        Vector3 targetDirection;

        bool run;
        bool lockOn;
        bool first;
        bool fight = false;

        public Transform player;
        public EnemyTarget lockOnTransform;
        public StateManager parriedBy;

        public Animator anim;
        EnemyTarget enTarget;
        public EnemyTarget playerTarget;
        AnimatorHook a_hook;
        public NavMeshAgent navAgent;
        public LayerMask ignoreLayers;
        public Rigidbody rigid;

        List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        List<Collider> ragdollColliders = new List<Collider>();

        public delegate void SpellEffect_Loop();
        public SpellEffect_Loop spellEffect_loop;

        float timer;
        float firstTimer;

        float cooldown = 1.5f;
        float combatRotationCooldown = 0.5f;

        public void Init()
        {
            health = 100;
            anim = GetComponentInChildren<Animator>();
            enTarget = GetComponent<EnemyTarget>();
            enTarget.Init(this);

            rigid = GetComponent<Rigidbody>();
            navAgent = this.GetComponent<NavMeshAgent>();
            rigid.isKinematic = true;



            a_hook = anim.GetComponent<AnimatorHook>();
            if (a_hook == null)
                a_hook = anim.gameObject.AddComponent<AnimatorHook>();
            a_hook.Init(null, this);

            InitRagdoll();
            parryIsOn = false;
            ignoreLayers = ~(1 << 9);

            firstTimer = 2f;
        }

        void InitRagdoll()
        {
            Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rigs.Length; i++)
            {
                if (rigs[i] == rigid)
                    continue;

                ragdollRigids.Add(rigs[i]);
                rigs[i].isKinematic = true;

                Collider col = rigs[i].gameObject.GetComponent<Collider>();
                col.isTrigger = true;
                ragdollColliders.Add(col);
            }
        }

        public void EnableRagdoll()
        {

            for (int i = 0; i < ragdollRigids.Count; i++)
            {
                ragdollRigids[i].isKinematic = false;
                ragdollColliders[i].isTrigger = false;
            }

            Collider controllerCollider = rigid.gameObject.GetComponent<Collider>();
            controllerCollider.enabled = false;
            rigid.isKinematic = true;

            StartCoroutine("CloseAnimator");

        }

        IEnumerator CloseAnimator()
        {
            yield return new WaitForEndOfFrame();
            anim.enabled = false;
            this.enabled = false;
        }

        public void Tick(float d)
        {
            delta = d;
            canMove = anim.GetBool(StaticStrings.onEmpty);

            if (spellEffect_loop != null)
                spellEffect_loop();
            if (dontDoAnything)
            {
                dontDoAnything = !canMove;
                return;
            }
            if (rotateToTarget)
            {
                LookTowardsTarget();
            }
            if (health <= 0)
            {
                if (!isDead)
                {
                    isDead = true;
                    EnableRagdoll();
                }
            }

            if (isInvicible)
            {
                isInvicible = !canMove;
            }

            if (parriedBy != null && parryIsOn == false)
            {
                // parriedBy.parryTarget = null;
                parriedBy = null;
            }

            if (canMove)
            {
                parryIsOn = false;
                anim.applyRootMotion = false;

                HandleMovementAnimations();
            }
            else
            {
                if (anim.applyRootMotion == false)
                    anim.applyRootMotion = true;
            }

        }

        void LookTowardsTarget()
        {
            Vector3 dir = dirToTarget;
            dir.y = 0;
            if (dir == Vector3.zero)
                dir = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, delta * 5);
        }

        void DoAction()
        {
            anim.Play("oh_attack_1");
            anim.applyRootMotion = true;
            anim.SetBool(StaticStrings.canMove, false);
        }

        public void DoDamage(Action action, Weapon curWeapon)
        {
            if (isInvicible)
                return;

            //int damage = StatsCalculations.CalculateBaseDamage(curWeapon.weaponStats, characterStats);
            int damage = 5;

            //characterStats.poise += damage;
            health -= damage;

            if (canMove)
            {
                if (action.overrideDamageAnim)
                    anim.Play(action.damageAnim);
                else
                {
                    int ran = Random.Range(0, 100);
                    string tempAnim = (ran < 50) ? StaticStrings.damage1 : StaticStrings.damage2;
                    anim.Play(tempAnim);
                }
            }
            Debug.Log("Damage is " + damage);

            isInvicible = true;
            // anim.Play("damage_2");
            anim.applyRootMotion = true;
            anim.SetBool(StaticStrings.canMove, false);
        }

        public void SetDestination(Vector3 d)
        {
            if (!hasDestination)
            {
                hasDestination = true;
                navAgent.isStopped = false;
                navAgent.SetDestination(d);
                targetDestination = d;
            }
        }

        public void DoDamage2()
        {
            if (isInvicible)
                return;
            anim.Play(StaticStrings.damage3);

        }

        public void CheckForParry(Transform target, StateManager states)
        {
            if (canBeParried == false || parryIsOn == false || isInvicible)
                return;

            Vector3 dir = transform.position - target.position;
            dir.Normalize();
            float dot = Vector3.Dot(target.forward, dir);
            if (dot < 0)
                return;

            isInvicible = true;
            anim.Play(StaticStrings.attack_interrupt);
            anim.applyRootMotion = true;
            anim.SetBool(StaticStrings.canMove, false);
            //states.parryTarget = this;
            parriedBy = states;
            return;
        }

        public void IsGettingParried(Action action, Weapon curWeapon)
        {
            int damage = StatsCalculations.CalculateBaseDamage(curWeapon.weaponStats, characterStats, action.parryMultiplier);
            Debug.Log(damage);

            health -= damage;
            dontDoAnything = true;
            anim.SetBool(StaticStrings.canMove, false);
            anim.Play(StaticStrings.parry_recieved);
        }

        public void IsGettingBackstabbed(Action action, Weapon curWeapon)
        {
            int damage = StatsCalculations.CalculateBaseDamage(curWeapon.weaponStats, characterStats, action.backstabMultiplier);
            Debug.Log(damage);

            health -= damage;
            dontDoAnything = true;
            anim.SetBool(StaticStrings.canMove, false);
            anim.Play(StaticStrings.backstabbed);
        }

        public ParticleSystem fireParticle;
        float _t;

        public void OnFire()
        {
            if (_t < 5)
            {
                _t += Time.deltaTime;
                fireParticle.Emit(1);
            }
            else
            {
                _t = 0;
                spellEffect_loop = null;
            }
        }

        public void OpenDamageColliders()
        {
            if (curAttack == null)
                return;

            if (curAttack.isDefaultDamageCollider || curAttack.damageCollider.Length == 0)
            {
                ObjectListStatus(defaultDamageColliders, true);
            }
            else
            {
                ObjectListStatus(curAttack.damageCollider, true);
            }
        }

        public void CloseDamageColliders()
        {
            if (curAttack == null)
                return;

            if (curAttack.isDefaultDamageCollider || curAttack.damageCollider.Length == 0)
            {
                ObjectListStatus(defaultDamageColliders, false);
            }
            else
            {
                ObjectListStatus(curAttack.damageCollider, false);
            }
        }

        void ObjectListStatus(GameObject[] l, bool status)
        {
            for (int i = 0; i < l.Length; i++)
            {
                l[i].SetActive(status);
            }
        }

        void HandleMovementAnimations()
        {
            //anim.SetBool(StaticStrings.run, run);
            //anim.SetFloat(StaticStrings.vertical, moveAmount, 0.4f, delta);
            float velocitySquared = navAgent.desiredVelocity.sqrMagnitude;
            float v = Mathf.Clamp(velocitySquared, 0, .5f);

            anim.SetFloat(StaticStrings.vertical, v, 0.2f, delta);
        }
    }
}
