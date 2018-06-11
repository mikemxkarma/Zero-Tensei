using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class AIHandler : MonoBehaviour
    {
        public AIAttacks[] ai_attacks;
        public EnemyStates states;

        public StateManager en_states;
        public Transform target;

        public float sight;
        public float fov_angle;

        public int closeCount = 10;
        int _close;

        public int frameCount = 30;
        int _frame;

        public int attackCount = 30;
        int _attack;

        float dis;
        float angle;
        float delta;
        Vector3 dirToTarget;

        public Transform[] points;
        private int destPoint = 0;

        float distanceFromTarget()
        {
            if (target == null)
                return 100;

            return Vector3.Distance(target.position, transform.position);
        }

        float angleToTarget()
        {
            float a = 180;
            if (target)
            {
                Vector3 d = dirToTarget;
                a = Vector3.Angle(d, transform.forward);
            }

            return a;
        }

        void Start()
        {
            if (states == null)
                states = GetComponent<EnemyStates>();

            states.Init();
            states.navAgent.autoBraking = false;
            // Start patrol
            GotoNextPoint();
            InitDamageColliders();
        }
        void GotoNextPoint()
        {
            // Returns if no points have been set up
            if (points.Length == 0)
                return;

            // Set the agent to go to the currently selected destination.
            states.navAgent.destination = points[destPoint].position;

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.
            destPoint = (destPoint + 1) % points.Length;
        }
        void InitDamageColliders()
        {
            for (int i = 0; i < ai_attacks.Length; i++)
            {
                for (int c = 0; c < ai_attacks[i].damageCollider.Length; c++)
                {
                    DamageCollider d = ai_attacks[i].damageCollider[c].GetComponent<DamageCollider>();
                    d.InitEnemy(states);
                }
            }

            for (int i = 0; i < states.defaultDamageColliders.Length; i++)
            {
                DamageCollider d = states.defaultDamageColliders[i].GetComponent<DamageCollider>();
               d.InitEnemy(states);
            }
        }

        public AIstate aiState;
        public enum AIstate
        {
            far, close, inSight, attacking
        }

        void Update()
        {
            delta = Time.deltaTime;
            dis = distanceFromTarget();
            angle = angleToTarget();
            if (target)
                dirToTarget = target.position - transform.position;
            states.dirToTarget = dirToTarget;

            switch (aiState)
            {
                case AIstate.far:
                    HandleFarSight();
                    break;
                case AIstate.close:
                    HandleCloseSight();
                    break;
                case AIstate.inSight:
                    InSight();
                    break;
                case AIstate.attacking:
                    if (states.canMove)
                    {
                        aiState = AIstate.inSight;
                        states.rotateToTarget = true;
                        //states.agent.enabled = true;
                    }
                    break;
                default:
                    break;
            }

            states.Tick(delta);
        }

        void InSight()
        {
            HandleCooldowns();

            float d2 = Vector3.Distance(states.targetDestination, target.position);
            if (d2 > 2 || dis > sight * .5)
                GoToTarget();

            if (dis < 2)
                states.navAgent.isStopped = true;

            if (_attack > 0)
            {
                _attack--;
                return;
            }

            _attack = attackCount;

            AIAttacks a = WillAttack();
            states.SetCurAttack(a);
            if (a != null)
            {
                aiState = AIstate.attacking;
                states.anim.Play(a.targetAnim);
                states.anim.SetBool(StaticStrings.onEmpty, false);
                states.canMove = false;
                a._cooldown = a.cooldown;
                states.navAgent.isStopped = true;
                states.rotateToTarget = false;
                return;
            }


        }

        void HandleCloseSight()
        {
            _close++;
            if (_close > closeCount)
            {
                _close = 0;

                if (dis > sight || angle > fov_angle)
                {
                    aiState = AIstate.far;
                    return;
                }
            }

            RaycastToTarget();
        }

        void HandleFarSight()
        {
            if (target == null)
                return;

            _frame++;
            if (_frame > frameCount)
            {
                _frame = 0;

                if (dis < sight)
                {
                    if (angle < fov_angle)
                    {
                        aiState = AIstate.close;
                    }
                }
                else
                {
                    if (!states.navAgent.pathPending && states.navAgent.remainingDistance < 0.5f)
                        GotoNextPoint();
                }
            }
        }

        void GoToTarget()
        {
            states.hasDestination = false;
            states.SetDestination(target.position);
        }

        void HandleCooldowns()
        {
            for (int i = 0; i < ai_attacks.Length; i++)
            {
                AIAttacks a = ai_attacks[i];
                if (a._cooldown > 0)
                {
                    a._cooldown -= delta;
                    if (a._cooldown < 0)
                        a._cooldown = 0;
                }
            }
        }

        public AIAttacks WillAttack()
        {
            int weight = 0;
            List<AIAttacks> l = new List<AIAttacks>();
            for (int i = 0; i < ai_attacks.Length; i++)
            {
                AIAttacks a = ai_attacks[i];
                if (a._cooldown > 0)
                {
                    continue;
                }

                if (dis > a.minDistance)
                    continue;
                if (angle < a.minAngle)
                    continue;
                if (angle > a.maxAngle)
                    continue;
                if (a.weight == 0)
                    continue;

                weight += a.weight;
                l.Add(a);
            }

            if (l.Count == 0)
                return null;

            int ran = Random.Range(0, weight + 1);
            int c_weight = 0;
            for (int i = 0; i < l.Count; i++)
            {
                c_weight += l[i].weight;
                if (c_weight > ran)
                {
                    return l[i];
                }
            }

            return null;
        }

        void RaycastToTarget()
        {
            RaycastHit hit;
            Vector3 origin = transform.position;
            origin.y += 0.5f;
            Vector3 dir = dirToTarget;
            dir.y += 0.5f;
            if (Physics.Raycast(origin, dir, out hit, sight, states.ignoreLayers))
            {
                StateManager st = hit.transform.GetComponentInParent<StateManager>();
                if (st != null)
                {
                    states.rotateToTarget = true;
                    aiState = AIstate.inSight;
                    states.SetDestination(target.position);
                }
            }
        }





    }

    [System.Serializable]
    public class AIAttacks
    {
        public int weight;
        public float minDistance;
        public float minAngle;
        public float maxAngle;

        public float cooldown = 2;
        public float _cooldown;
        public string targetAnim;
        public bool hasReactAnim;
        public string reactAnim;

        public bool isDefaultDamageCollider;
        public GameObject[] damageCollider;

    }
}

