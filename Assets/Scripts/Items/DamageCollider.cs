using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class DamageCollider : MonoBehaviour
    {
        StateManager states;
        EnemyStates eStates;

        public void InitPlayer(StateManager st)
        {
            states = st;
            gameObject.layer = 9;
            gameObject.SetActive(false);
        }
        public void InitEnemy(EnemyStates st)
        {
            eStates = st;
            gameObject.layer = 9;
            gameObject.SetActive(false);
        }
        void OnTriggerEnter(Collider other)
        {
            if (states)
            {
                EnemyStates es = other.transform.GetComponentInParent<EnemyStates>();

                if (es == null)
                    return;

                es.DoDamage(states.currentAction, states.inventoryManager.GetCurrentWeapon(states.currentAction.mirror));
            }
            if (eStates)
            {
                StateManager st = other.transform.GetComponentInParent<StateManager>();

                if (st != null)
                {
                    Debug.Log("v");
                }

                st.DoDamage(eStates.GetCurAttack());
                //s.DoDamage(states.currentAction, states.inventoryManager.GetCurrentWeapon(states.currentAction.mirror));
            }

        }
    }
}
