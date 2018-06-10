using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class StaticFunctions
    {
        public static void DeepCopyWeapon(Weapon from, Weapon to)
        {
            //  to.itemId = from.itemId;
            to.icon = from.icon;
            to.oh_idle = from.oh_idle;
            to.th_idle = from.th_idle;
            to.actions = new List<Action>();
            for (int i = 0; i < from.actions.Count; i++)
            {
                Action action = new Action();
                DeepCopyActionToAction(action, from.actions[i]);
                to.actions.Add(action);
            }
            to.two_handedActions = new List<Action>();
            for (int i = 0; i < from.two_handedActions.Count; i++)
            {
                Action action = new Action();
                DeepCopyActionToAction(action, from.two_handedActions[i]);
                to.two_handedActions.Add(action);
            }
            to.parryMultiplier = from.parryMultiplier;
            to.backstabMultiplier = from.backstabMultiplier;
            to.LeftHandMirror = from.LeftHandMirror;
            to.modelPrefab = from.modelPrefab;
            to.r_model_pos = from.r_model_pos;
            to.l_model_pos = from.l_model_pos;
            to.r_model_eulers = from.r_model_eulers;
            to.l_model_eulers = from.l_model_eulers;
            to.model_scale = from.model_scale;

            to.weaponStats = new WeaponStats();
            DeepCopyWeaponStats(from.weaponStats, to.weaponStats);
        }
        public static void DeepCopyActionToAction(Action action, Action w_action)
        {
            action.input = w_action.input;
            action.targetAnimation = w_action.targetAnimation;
            action.type = w_action.type;
            action.spellClass = w_action.spellClass;
            action.canParry = w_action.canParry;
            action.canBeParried = w_action.canBeParried;
            action.changeSpeed = w_action.changeSpeed;
            action.animSpeed = w_action.animSpeed;
            action.canBackstab = w_action.canBackstab;
            action.overrideDamageAnim = w_action.overrideDamageAnim;
            action.damageAnim = w_action.damageAnim;

            DeepCopyStepsList(w_action, action);
        }
        public static void DeepCopyStepsList(Action from, Action to)
        {
            to.steps = new List<ActionSteps>();

            for (int i = 0; i < from.steps.Count; i++)
            {
                ActionSteps step = new ActionSteps();
                DeepCopyStep(from.steps[i], step);
                to.steps.Add(step);
            }
        }

        public static void DeepCopyStep(ActionSteps from, ActionSteps to)
        {
            to.branches = new List<ActionAnim>();

            for (int i = 0; i < from.branches.Count; i++)
            {
                ActionAnim a = new ActionAnim();
                a.input = from.branches[i].input;
                a.targetAnim = from.branches[i].targetAnim;
                to.branches.Add(a);
            }
        }


        public static void DeepCopyAction(Weapon w, ActionInput input, ActionInput assign, List<Action> actionSlots, bool isLeftHand = false)
        {
            Action action = GetAction(assign, actionSlots);
            Action w_action = w.GetAction(w.actions, input);
            if (w_action == null)
            {
                Debug.Log("no weapon action found");
                return;
            }

            DeepCopyStepsList(w_action, action);
            action.targetAnimation = w_action.targetAnimation;
            action.type = w_action.type;
            action.spellClass = w_action.spellClass;
            action.canBeParried = w_action.canBeParried;
            action.changeSpeed = w_action.changeSpeed;
            action.animSpeed = w_action.animSpeed;
            action.canBackstab = w_action.canBackstab;
            action.overrideDamageAnim = w_action.overrideDamageAnim;
            action.damageAnim = w_action.damageAnim;
            action.parryMultiplier = w.parryMultiplier;
            action.backstabMultiplier = w.backstabMultiplier;

            if (isLeftHand)
                action.mirror = true;
        }
        public static void DeepCopyWeaponStats(WeaponStats from, WeaponStats to)
        {
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
                if (actionSlots[i].input == inp)
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
    }
}
