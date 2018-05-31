﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameControll
{

    public class ActionManager : MonoBehaviour
    {
        public List<Action> actionSlot = new List<Action>();
        public ItemAction consumableItem;

        StateManager states;

        public void Init(StateManager st)
        {
            states = st;
            UpdateActionsOneHanded();
        }

        public void UpdateActionsOneHanded()
        {
            EmptyAllSlots();

            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rb, ActionInput.rb, actionSlot);
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rt, ActionInput.rt, actionSlot);

            if (states.inventoryManager.hasLeftHandWeapon)
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rb, ActionInput.lb, actionSlot, true);
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rt, ActionInput.lt, actionSlot, true);
            }
            else
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.lb, ActionInput.lb, actionSlot);
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.lt, ActionInput.lt, actionSlot);
            }
        }

        public void UpdateActionsTwoHanded()
        {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.rightHandWeapon.instance;
            for (int i = 0; i < w.two_handedActions.Count; i++)
            {
                Action a = StaticFunctions.GetAction(w.two_handedActions[i].input, actionSlot);
                a.targetAnimation = w.two_handedActions[i].targetAnimation;
                a.type = w.two_handedActions[i].type;
            }
        }

        void EmptyAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = StaticFunctions.GetAction((ActionInput)i, actionSlot);
                a.targetAnimation = null;
                a.mirror = false;
                a.type = ActionType.attack;
            }
        }

        ActionManager()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionSlot.Add(a);
            }
        }

        public Action GetActionSlot(StateManager st)
        {
            ActionInput a_input = GetActionInput(st);
            return StaticFunctions.GetAction(a_input, actionSlot);
        }



        public ActionInput GetActionInput(StateManager st)
        {

            if (st.rb)
                return ActionInput.rb;
            if (st.rt)
                return ActionInput.rt;
            if (st.lb)
                return ActionInput.lb;
            if (st.lt)
                return ActionInput.lt;

            return ActionInput.rb;
        }
    }

    public enum ActionInput
    {
        rb, lb, rt, lt
    }

    public enum ActionType
    {
        attack, block, spells, parry
    }
    public enum SpellClass
    {
        pryromancy, miracles, sorcery
    }
    public enum SpellType
    {
        projectile, buff, looping
    }

    [System.Serializable]
    public class Action
    {

        public ActionInput input;
        public ActionType type;
        public SpellClass spellClass;
        public string targetAnimation;
        public bool mirror = false;
        public bool canBeParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public bool canParry = false;
        public bool canBackstab = false;
        public float staminaCost = 5;
        public int manaCost = 0;

        [HideInInspector]
        public float parryMultiplier;
        [HideInInspector]
        public float backstabMultiplier;

        public bool overrideDamageAnim;
        public string damageAnim;
    }

    [System.Serializable]
    public class SpellAction
    {
        public ActionInput input;
        public string targetAnimation;
        public string throwAnimation;
        public float castTime;
    }
    [System.Serializable]
    public class ItemAction
    {
        public string targetAnimation;
        public string item_id;
    }
}
