using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class DamageCollider : MonoBehaviour
    {
        StateManager states;

        public void Init(StateManager st)
        {
            states = st;
        }
        void OnTriggerEnter(Collider other)
        {
            EnemyStates eStates = other.transform.GetComponentInParent<EnemyStates>();

            if (eStates == null)
                return;

            eStates.DoDamage(states.currentAction, states.inventoryManager.GetCurrentWeapon(states.currentAction.mirror));
        }
    }
}
