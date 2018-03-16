using System.Collections;
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
            UpdateActionsWithLeftHand();
            //    if (states.inventoryManager.hasLeftHandWeapon)
            //    {
            //        UpdateActionsWithLeftHand();
            //        return;
            //    }

            //    Weapon w = states.inventoryManager.rightHandWeapon;
            //    for (int i = 0; i < w.actions.Count; i++)
            //    {
            //        Action a = GetAction(w.actions[i].input);
            //        a.targetAnimation = w.actions[i].targetAnimation;
            //    }
            //
        }

        public void DeepCopyAction(Weapon w, ActionInput input, ActionInput assign, bool isLeftHand = false)
        {
            Action a = GetAction(assign);
            Action w_action = w.GetAction(w.actions, input);
            if (w_action == null)
                return;

            a.targetAnimation = w_action.targetAnimation;
            a.type = w_action.type;
            a.canBeParried = w_action.canBeParried;
            a.changeSpeed = w_action.changeSpeed;
            a.animSpeed = w_action.animSpeed;
            a.canBackstab = w_action.canBackstab;

            if (isLeftHand)
                a.mirror = true;
        }
        public void UpdateActionsWithLeftHand()
        {

            DeepCopyAction(states.inventoryManager.rightHandWeapon, ActionInput.rb, ActionInput.rb);
            DeepCopyAction(states.inventoryManager.rightHandWeapon, ActionInput.rt, ActionInput.rt);

            if (states.inventoryManager.hasLeftHandWeapon)
            {
                DeepCopyAction(states.inventoryManager.leftHandWeapon, ActionInput.rb, ActionInput.lb, true);
                DeepCopyAction(states.inventoryManager.leftHandWeapon, ActionInput.rt, ActionInput.lt, true);
            }
            else
            {
                DeepCopyAction(states.inventoryManager.leftHandWeapon, ActionInput.lb, ActionInput.lb);
                DeepCopyAction(states.inventoryManager.leftHandWeapon, ActionInput.lt, ActionInput.lt);
            }
        }

        public void UpdateActionsTwoHanded()
        {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.rightHandWeapon;
            for (int i = 0; i < w.two_handedActions.Count; i++)
            {
                Action a = GetAction(w.two_handedActions[i].input);
                a.targetAnimation = w.two_handedActions[i].targetAnimation;
                a.type = w.two_handedActions[i].type;
            }
        }

        void EmptyAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = GetAction((ActionInput)i);
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
            return GetAction(a_input);
        }

        Action GetAction(ActionInput inp)
        {
            for (int i = 0; i < actionSlot.Count; i++)
            {
                if (actionSlot[i].input == inp)
                    return actionSlot[i];
            }
            return null;
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

    [System.Serializable]
    public class Action
    {

        public ActionInput input;
        public ActionType type;
        public string targetAnimation;
        public bool mirror = false;
        public bool canBeParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public bool canParry = false;
        public bool canBackstab = false;

    }

    [System.Serializable]
    public class ItemAction
    {
        public string targetAnimation;
        public string item_id;
    }
}
