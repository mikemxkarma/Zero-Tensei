using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameControll
{
    public class BreathCollider : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            EnemyStates es = other.GetComponentInParent<EnemyStates>();
            if(es != null)
            {
                es.DoDamage2();
                SpellEffectsManager.singleton.UseSpellEffect("onFire", null, es);
            }
        }
    }
}

