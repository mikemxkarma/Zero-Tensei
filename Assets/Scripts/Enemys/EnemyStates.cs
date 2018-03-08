using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class EnemyStates : MonoBehaviour
    {
        public float health;
        public bool isInvicible;
        public bool canMove;
        public bool isDead;

       public Animator anim;
        EnemyTarget enemyTarget;
        AnimatorHook a_hook;
        public Rigidbody rigid;
        public float delta;

        List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        List<Collider> ragdollColliders = new List<Collider>();

        void Start()
        {
            health = 50;
            anim = GetComponentInChildren<Animator>();
            enemyTarget = GetComponent<EnemyTarget>();
            enemyTarget.Init(this);

            rigid = GetComponent<Rigidbody>();


            a_hook = anim.GetComponent<AnimatorHook>();
            if (a_hook == null)
                a_hook = anim.gameObject.AddComponent<AnimatorHook>();

            a_hook.Init(null, this);

            InitRagdoll();
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

                Collider collider = rigs[i].gameObject.GetComponent<Collider>();
                collider.isTrigger = true;
                ragdollColliders.Add(collider);
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

         void Update()
        {
            delta = Time.deltaTime;
            canMove = anim.GetBool("canMove");

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

            if (canMove)
            {
                anim.applyRootMotion = false;
            }
        }

        public void DoDamage(float v)
        {
            if (isInvicible)
                return;

            health -= v;
            isInvicible = true;
            anim.Play("damage_1");
            anim.applyRootMotion = true;
            anim.SetBool("canMove", false);
        }
    }
}