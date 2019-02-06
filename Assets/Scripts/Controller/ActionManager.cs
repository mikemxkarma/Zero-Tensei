using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameControll
{

    public class ActionManager : MonoBehaviour
    {

        public List<Action> actionSlot = new List<Action>();
        public int actionIndex;
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

            if (states.inventoryManager.rightHandWeapon != null)
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rb, ActionInput.rb, actionSlot);
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rt, ActionInput.rt, actionSlot);
            }

            if (states.inventoryManager.leftHandWeapon == null)
                return;
            
            if (states.inventoryManager.hasLeftHandWeapon)
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rb, ActionInput.lb, actionSlot, true);
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rt, ActionInput.lt, actionSlot, true);
            }
            else
            {
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lb, ActionInput.lb, actionSlot);
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lt, ActionInput.lt, actionSlot);
            }
        }

        public void UpdateActionsTwoHanded()
        {
            EmptyAllSlots();
            
            if (states.inventoryManager.rightHandWeapon == null)
                return;
            Weapon w = states.inventoryManager.rightHandWeapon.instance;
            
            for (int i = 0; i < w.two_handedActions.Count; i++)
            {
                Action a = StaticFunctions.GetAction(w.two_handedActions[i].GetFirstInput(), actionSlot);
                a.firstStep.targetAnim = w.two_handedActions[i].firstStep.targetAnim;
                StaticFunctions.DeepCopyStepsList(w.two_handedActions[i], a);
                a.type = w.two_handedActions[i].type;
            }
        }

        void EmptyAllSlots()
        {
            
            for (int i = 0; i < 4; i++)
            {
                
                            
            
                Action a = StaticFunctions.GetAction((ActionInput)i, actionSlot);
                if (a == null)
                {
                    Debug.Log(((ActionInput)i).ToString());
                    return;
                }

                //a.firstStep = null;
                a.comboSteps = null;
                a.mirror = false;
                a.type = ActionType.attack;
            }

            StaticFunctions.DeepCopyAction(states.inventoryManager.unarmedRuntime.instance, ActionInput.rb,ActionInput.rb, actionSlot);
            StaticFunctions.DeepCopyAction(states.inventoryManager.unarmedRuntime.instance, ActionInput.rt, ActionInput.rt, actionSlot);
            StaticFunctions.DeepCopyAction(states.inventoryManager.unarmedRuntime.instance, ActionInput.rb, ActionInput.lb, actionSlot, true);
            StaticFunctions.DeepCopyAction(states.inventoryManager.unarmedRuntime.instance, ActionInput.rt,ActionInput.lt, actionSlot, true);
        }

        public Action GetActionSlot(StateManager st)
        {
            ActionInput a_input = GetActionInput(st);
            return StaticFunctions.GetAction(a_input, actionSlot);
        }

        public Action GetActionFromInput(ActionInput a_input)
        {
            return StaticFunctions.GetAction(a_input, actionSlot);
        }
        public bool IsLeftHandSlot(Action slot)
        {
            return (slot.GetFirstInput() == ActionInput.lb || slot.GetFirstInput() == ActionInput.lt);
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
        public ActionType type;
        public SpellClass spellClass;
        public ActionAnim firstStep;
        public List<ActionAnim> comboSteps;
        public bool mirror = false;
        public bool canBeParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public bool canParry = false;
        public bool canBackstab = false;
        public float staminaCost = 5;
        public int manaCost = 0;

        
        public bool overrideKick;
        public string kickAnim;
        public ActionInput GetFirstInput()
        {
            if (firstStep == null)
                firstStep = new ActionAnim();

            return firstStep.input;
        }

        public ActionAnim GetActionStep(ref int index)
        {
            if (index == 0)
            {
                if (comboSteps.Count == 0)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }
                return firstStep;
            }

            ActionAnim retVal = comboSteps[index-1];
            index++;
            if (index > comboSteps.Count - 1)
                index = 0;
            
            return retVal;
        }

        [HideInInspector]
        public float parryMultiplier;
        [HideInInspector]
        public float backstabMultiplier;

        public bool overrideDamageAnim;
        public string damageAnim;
    }
    [System.Serializable]
    public class ActionSteps
    {
        public List<ActionAnim> branches = new List<ActionAnim>();

        public ActionAnim GetBranch(ActionInput inp)
        {
            for (int i = 0; i < branches.Count; i++)
            {
                if (branches[i].input == inp)
                    return branches[i];
            }

            return branches[0];
        }
    }
    [System.Serializable]
    public class ActionAnim
    {
        public ActionInput input;
        public string targetAnim;
    }


    [System.Serializable]
    public class SpellAction
    {
        public ActionInput input;
        public string targetAnimation;
        public string throwAnimation;
        public float castTime;
        public float manaCost;
        public float staminaCost;
    }
    [System.Serializable]
    public class ItemAction
    {
        public string targetAnimation;
        public string item_id;
    }
}
