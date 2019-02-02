using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class StaticFunctions
    {
        public static void DeepCopyWeapon(Weapon from, Weapon to)
        {
            
            to.icon = from.icon;
            to.oh_idle = from.oh_idle;
            to.th_idle = from.th_idle;
            
            to.actions = new List<Action>();
            for (int i = 0; i < from.actions.Count; i++)
            {
                Action action = new Action();
                
                DeepCopyActionToAction(from.actions[i],action);
                
                to.actions.Add(action);
            }
            to.two_handedActions = new List<Action>();
            for (int i = 0; i < from.two_handedActions.Count; i++)
            {
                Action action = new Action();
                DeepCopyActionToAction(from.two_handedActions[i],action );
                to.two_handedActions.Add(action);
            }
            to.parryMultiplier = from.parryMultiplier;
            to.backstabMultiplier = from.backstabMultiplier;
            to.LeftHandMirror = from.LeftHandMirror;
            to.modelPrefab = from.modelPrefab;
            to.r_model_equiped_pos = from.r_model_equiped_pos;
            to.l_model_equiped_pos = from.l_model_equiped_pos;
            to.r_model_equiped_eulers = from.r_model_equiped_eulers;
            to.l_model_equiped_eulers = from.l_model_equiped_eulers;
            to.model_scale = from.model_scale;
        }
        public static void DeepCopyActionToAction(Action from, Action to)
        {
            
            to.firstStep = new ActionAnim();
            to.firstStep.input = from.firstStep.input;
            to.firstStep.targetAnim = from.firstStep.targetAnim;

            to.comboSteps = new List<ActionAnim>();
            
            to.type = from.type;
            to.spellClass = from.spellClass;
            to.canParry = from.canParry;
            to.canBeParried = from.canBeParried;
            to.changeSpeed = from.changeSpeed;
            to.animSpeed = from.animSpeed;
            to.canBackstab = from.canBackstab;
            to.overrideDamageAnim = from.overrideDamageAnim;
            to.damageAnim = from.damageAnim;

            DeepCopyStepsList(from, to);
        }
        public static void DeepCopyStepsList(Action from, Action to)
        {
            
            for (int i = 0; i < from.comboSteps.Count; i++)
            {
                ActionAnim a = new ActionAnim();
                a.input = from.comboSteps[i].input;
                a.targetAnim = from.comboSteps[i].targetAnim;
                to.comboSteps.Add(a);
            }
        }


        public static void DeepCopyAction(Weapon w, ActionInput input, ActionInput assign, List<Action> actionList, bool isLeftHand = false)
        {
            Action from = w.GetAction(w.actions, input);
            Action to = GetAction(assign, actionList);
            
            if (from == null)
            {
                Debug.Log("no weapon action found");
                return;
            }

            to.firstStep.targetAnim = from.firstStep.targetAnim;
            to.comboSteps = new List<ActionAnim>();
            DeepCopyStepsList(from, to);
            
            to.type = from.type;
            to.spellClass = from.spellClass;
            to.canBeParried = from.canBeParried;
            to.changeSpeed = from.changeSpeed;
            to.animSpeed = from.animSpeed;
            to.canBackstab = from.canBackstab;
            to.overrideDamageAnim = from.overrideDamageAnim;
            to.damageAnim = from.damageAnim;
            to.parryMultiplier = from.parryMultiplier;
            to.backstabMultiplier = from.backstabMultiplier;

            if (isLeftHand)
                to.mirror = true;
        }
        public static void DeepCopyWeaponStats(WeaponStats from, WeaponStats to)
        {

            if (from == null)
            {
                Debug.Log(to.weaponId + " weapon stats weren't found, assinging everything as zero");
                return;
            }

            to.weaponId = from.weaponId;
            to.physical = from.physical;
            to.slash = from.slash;
            to.strike = from.strike;
            to.thrust = from.thrust;
            to.magic = from.magic;
            to.lighting = from.lighting;
            to.fire = from.fire;
            to.dark = from.dark;
        }
        public static Action GetAction(ActionInput inp, List<Action> actionSlots)
        {
            for (int i = 0; i < actionSlots.Count; i++)
            {
                if (actionSlots[i].GetFirstInput() == inp)
                    return actionSlots[i];
            }
            return null;
        }
        public static void DeepCopySpell(Spell from, Spell to)
        {
            to.itemName = from.itemName;
            to.itemDescription = from.itemDescription;
            to.icon = from.icon;
            to.spellType = from.spellType;
            to.spellClass = from.spellClass;
            to.projectile = from.projectile;
            to.spell_effect = from.spell_effect;
            to.particle_prefab = from.particle_prefab;

            to.spell_Actions = new List<SpellAction>();
            for (int i = 0; i < from.spell_Actions.Count; i++)
            {
                SpellAction action = new SpellAction();
                DeepCopySpellAction(action, from.spell_Actions[i]);
                to.spell_Actions.Add(action);
            }
        }
        public static void DeepCopySpellAction(SpellAction to, SpellAction from)
        {
            to.input = from.input;
            to.targetAnimation = from.targetAnimation;
            to.throwAnimation = from.throwAnimation;
            to.castTime = from.castTime;
            to.manaCost = from.manaCost;
            to.staminaCost = from.staminaCost;
        }
        public static void DeepCopyConsumable(ref Consumable to, Consumable from)
        {
            to.consumableEffect = from.consumableEffect;
            to.targetAnim = from.targetAnim;
            to.icon = from.icon;
            to.itemDescription = from.itemDescription;
            to.itemName = from.itemName;
            to.itemPrefab = from.itemPrefab;
            to.r_model_pos = from.r_model_pos;
            to.r_model_eulers = from.r_model_eulers;
            to.model_scale = from.model_scale;
            
        }
    }
}
