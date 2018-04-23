using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{

    public class InventoryManager : MonoBehaviour
    {
        public List<string> rh_weapons;
        public List<string> lh_weapons;
        public List<string> spell_items;

        public RuntimeSpellItems currentSpell;
        public RuntimeWeapon rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public RuntimeWeapon leftHandWeapon;


        public GameObject parryCollider;

        StateManager states;

        public void Init(StateManager st)
        {
            states = st;
            if (rh_weapons.Count > 0)
            {
                rightHandWeapon = WeaponToRuntimeWeapon(
                    ResourcesManager.singleton.GetWeapon(rh_weapons[0])
                    );
            }

            if (lh_weapons.Count > 0)
            {
                leftHandWeapon = WeaponToRuntimeWeapon(
                              ResourcesManager.singleton.GetWeapon(lh_weapons[0]),
                              true
                              );
                hasLeftHandWeapon = true;
            }

            if (rightHandWeapon != null)
                EquipWeapon(rightHandWeapon, false);
            if (leftHandWeapon != null)
                EquipWeapon(leftHandWeapon, true);

            if(spell_items.Count > 0)
            {
                currentSpell = SpellToRuntimeSpell(ResourcesManager.singleton.GetSpell(spell_items[0]));
            }
            if(currentSpell != null)
            {
                EquipSpell(currentSpell);
            }

            InitAllDamageColliders();
            CloseAllDamageColliders();

            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(st);
            CloseParryCollider();
        }

        public void EquipWeapon(RuntimeWeapon w, bool isLeft = false)
        {
            string targetIdle = w.instance.oh_idle;
            targetIdle += (isLeft) ? "_l" : "_r";
            states.anim.SetBool(StaticStrings.mirror, isLeft);
            states.anim.Play(StaticStrings.changeWeapon);
            states.anim.Play(targetIdle);

            UI.QuickSlot uiSlot = UI.QuickSlot.singleton;
            uiSlot.UpdateSlot(
                (isLeft) ?
                UI.QSlotType.lh : UI.QSlotType.rh, w.instance.icon);

            w.weapon_Model.SetActive(true);
        }

        public void EquipSpell(RuntimeSpellItems s, bool isLeft = false)
        {
            UI.QuickSlot uiSlot = UI.QuickSlot.singleton;

            uiSlot.UpdateSlot(UI.QSlotType.spell, s.instance.icon);
        }
        public Weapon GetCurrentWeapon(bool isLeft)
        {
            if (isLeft)
                return leftHandWeapon.instance;
            else
                return rightHandWeapon.instance;
        }

        public void OpenAllDamageColliders()
        {
            if (rightHandWeapon.weapon_Hook != null)
                rightHandWeapon.weapon_Hook.OpenDamageColliders();

            if (leftHandWeapon.weapon_Hook != null)
                leftHandWeapon.weapon_Hook.OpenDamageColliders();
        }

        public void CloseAllDamageColliders()
        {
            if (rightHandWeapon.weapon_Hook != null)
                rightHandWeapon.weapon_Hook.CloseDamageColliders();

            if (leftHandWeapon.weapon_Hook != null)
                leftHandWeapon.weapon_Hook.CloseDamageColliders();
        }

        public void InitAllDamageColliders()
        {
            if (rightHandWeapon.weapon_Hook != null)
                rightHandWeapon.weapon_Hook.InitDamageColliders(states);

            if (leftHandWeapon.weapon_Hook != null)
                leftHandWeapon.weapon_Hook.InitDamageColliders(states);
        }

        public void CloseParryCollider()
        {
            parryCollider.SetActive(false);
        }

        public void OpenParryCollider()
        {
            parryCollider.SetActive(true);
        }

        public RuntimeSpellItems SpellToRuntimeSpell(Spell s, bool isLeft = false)
        {
            GameObject go = new GameObject();
            RuntimeSpellItems inst = go.AddComponent<RuntimeSpellItems>();
            inst.instance = new Spell();
            StaticFunctions.DeepCopySpell(s, inst.instance);
            go.name = s.itemName;

            return inst;
        }
        public RuntimeWeapon WeaponToRuntimeWeapon(Weapon w, bool isLeft = false)
        {
            GameObject go = new GameObject();
            RuntimeWeapon inst = go.AddComponent<RuntimeWeapon>();
            go.name = w.itemName;

            inst.instance = new Weapon();
            StaticFunctions.DeepCopyWeapon(w, inst.instance);

            inst.weapon_Model = Instantiate(inst.instance.modelPrefab);
            Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            inst.weapon_Model.transform.parent = p;
            inst.weapon_Model.transform.localPosition = (isLeft) ? inst.instance.l_model_pos : inst.instance.r_model_pos;
            inst.weapon_Model.transform.localEulerAngles = (isLeft) ? inst.instance.l_model_eulers : inst.instance.r_model_eulers;
            inst.weapon_Model.transform.localScale = Vector3.one;

            inst.weapon_Hook = inst.weapon_Model.GetComponentInChildren<WeaponHook>();
            inst.weapon_Hook.InitDamageColliders(states);

            return inst;
        }
    }
    [System.Serializable]
    public class Item
    {
        private int itemId;
        public string itemName;
        public string itemDescription;
        public Sprite icon;
    }

    [System.Serializable]
    public class Weapon : Item
    {
        public string oh_idle;
        public string th_idle;

        public List<Action> actions;
        public List<Action> two_handedActions;

        public float parryMultiplier;
        public float backstabMultiplier;
        public bool LeftHandMirror;

        public GameObject modelPrefab;

        public Action GetAction(List<Action> l, ActionInput inp)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].input == inp)
                {
                    return l[i];
                }
            }

            return null;
        }

        public Vector3 r_model_pos;
        public Vector3 l_model_pos;
        public Vector3 r_model_eulers;
        public Vector3 l_model_eulers;
        public Vector3 model_scale;
    }

    [System.Serializable]
    public class Spell : Item
    {
        public SpellType spellType;
        public GameObject projectile;
        public GameObject particle_prefab;

    }

}