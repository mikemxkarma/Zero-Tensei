using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class SpellEffectsManager : MonoBehaviour
    {
        Dictionary<string, int> s_effects = new Dictionary<string, int>();

        public void UseSpellEffect(string id,StateManager caster, EnemyStates enemy = null)
        {
            int index = GetEffect(id);

            if(index == -1)
            {
                Debug.Log("Spell effect doesnt exist ");
                return;
            }
            switch (index)
            {
                case 0:
                    FireBreath(caster);
                    break;
                case 1:
                    DarkShield(caster);
                    break;
                case 2:
                    Healing(caster);
                    break;
                case 3:
                    FireBall(caster);
                    break;
                case 4:
                    OnFire(caster, enemy);
                    break;


            }


        }

        int GetEffect(string id)
        {
            int index = -1;
            if(s_effects.TryGetValue(id,out index))
            {
                return index;
            }
            return 0;
        }

        void FireBreath(StateManager caster)
        {
            caster.spellCast_start = caster.inventoryManager.OpenBreathCollider;
            caster.spellCast_loop = caster.inventoryManager.EmitSpellParticle;
            caster.spellCast_stop = caster.inventoryManager.CloseBreathCollider;
        }

        void DarkShield(StateManager caster)
        {
            caster.spellCast_start = caster.inventoryManager.OpenBlockCollider;
            caster.spellCast_loop = caster.inventoryManager.EmitSpellParticle;
            caster.spellCast_stop = caster.inventoryManager.OpenBlockCollider;
        }

        void Healing(StateManager caster)
        {
            caster.spellCast_loop = caster.AddHealth;
        }

        void FireBall(StateManager caster)
        {
            caster.spellCast_loop = caster.inventoryManager.EmitSpellParticle;
        }
        void OnFire(StateManager caster,EnemyStates enemy)
        {
            if(caster != null)
            {

            }
            if(enemy != null)
            {
                enemy.spellEffect_loop = enemy.OnFire;
            }
        }
        public static SpellEffectsManager singleton;
        void Awake()
        {
            singleton = this;

            s_effects.Add("firebreath", 0);
            s_effects.Add("darkshield", 1);
            s_effects.Add("healing", 2);
            s_effects.Add("fireball", 3);
            s_effects.Add("onFire", 4);

        }
    }
}

