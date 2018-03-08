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

            if (states.inventoryManager.hasLeftHandWeapon)
            {
                UpdateActionsWithLeftHand();
                return;
            }

            Weapon w = states.inventoryManager.rightHandWeapon;
            for (int i = 0; i < w.action.Count; i++)
            {
                Action a = GetAction(w.action[i].input);
                a.targetAnimation = w.action[i].targetAnimation;
            }
        }

        public void UpdateActionsWithLeftHand()
        {
            Weapon r_weapon = states.inventoryManager.rightHandWeapon;
            Weapon l_weapon = states.inventoryManager.leftHandWeapon;

            Action rb = GetAction(ActionInput.rb);
            Action rt = GetAction(ActionInput.rt);
            rb.targetAnimation = r_weapon.GetAction(r_weapon.action, ActionInput.rb).targetAnimation;
            rt.targetAnimation = r_weapon.GetAction(r_weapon.action, ActionInput.rt).targetAnimation;

            Action lb = GetAction(ActionInput.lb);
            Action lt = GetAction(ActionInput.lt);
            lb.targetAnimation = l_weapon.GetAction(l_weapon.action, ActionInput.rb).targetAnimation;
            lt.targetAnimation = l_weapon.GetAction(l_weapon.action, ActionInput.rt).targetAnimation;

            if (l_weapon.leftHandMirror)
            {
                lb.mirror = true;
                lt.mirror = true;
            }
        }

        public void UpdateActionsTwoHanded()
        {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.rightHandWeapon;
            for (int i = 0; i < w.two_handed_Actions.Count; i++)
            {
                Action a = GetAction(w.two_handed_Actions[i].input);
                a.targetAnimation = w.two_handed_Actions[i].targetAnimation;
            }
        }

        void EmptyAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = GetAction((ActionInput)i);
                a.targetAnimation = null;
                a.mirror = false;
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
        rb,lb,rt,lt
    }

    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public string targetAnimation;
        public bool mirror = false;
    }

    [System.Serializable]
    public class ItemAction
    {
        public string targetAnimation;
        public string item_id;
    }
}
