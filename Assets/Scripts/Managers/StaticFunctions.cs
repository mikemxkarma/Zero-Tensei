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
                action.weaponStats = new WeaponStats();
                DeepCopyActionToAction(action, from.actions[i]);
                to.actions.Add(action);
            }
            to.two_handedActions = new List<Action>();
            for (int i = 0; i < from.two_handedActions.Count; i++)
            {
                Action action = new Action();
                action.weaponStats = new WeaponStats();
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

            DeepCopyWeaponStats(w_action.weaponStats, action.weaponStats);
        }
        public static void DeepCopyAction(Weapon w, ActionInput input, ActionInput assign, List<Action> actionSlots, bool isLeftHand = false)
        {
            Action a = GetAction(assign, actionSlots);
            Action w_action = w.GetAction(w.actions, input);
            if (w_action == null)
            {
                Debug.Log("no weapon action found");
                return;
            }


            a.targetAnimation = w_action.targetAnimation;
            a.type = w_action.type;
            a.spellClass = w_action.spellClass;
            a.canBeParried = w_action.canBeParried;
            a.changeSpeed = w_action.changeSpeed;
            a.animSpeed = w_action.animSpeed;
            a.canBackstab = w_action.canBackstab;
            a.overrideDamageAnim = w_action.overrideDamageAnim;
            a.damageAnim = w_action.damageAnim;
            a.parryMultiplier = w.parryMultiplier;
            a.backstabMultiplier = w.backstabMultiplier;

            if (isLeftHand)
                a.mirror = true;
            DeepCopyWeaponStats(w_action.weaponStats, a.weaponStats);
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
            to.projectile = from.projectile;
            to.particle_prefab = from.particle_prefab;
        }
    }
}
