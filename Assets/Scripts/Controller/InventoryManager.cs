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

        public int r_index;
        public int l_index;
        public int s_index;
        List<RuntimeWeapon> r_rh_weapons = new List<RuntimeWeapon>();
        List<RuntimeWeapon> r_lh_weapons = new List<RuntimeWeapon>();
        List<RuntimeSpellItems> r_spells = new List<RuntimeSpellItems>();


        public RuntimeSpellItems currentSpell;
        public RuntimeWeapon rightHandWeapon;
        public bool hasLeftHandWeapon = true;
        public RuntimeWeapon leftHandWeapon;


        public GameObject parryCollider;
        public GameObject breathCollider;
        public GameObject blockCollider;

        StateManager states;

        public void Init(StateManager st)
        {
            states = st;

            LoadInventory();

            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(st);
            CloseParryCollider();
            CloseBlockCollider();
            CloseBreathCollider();
        }

        public void LoadInventory()
        {
            for (int i = 0; i < rh_weapons.Count; i++)
            {
                WeaponToRuntimeWeapon(
                    ResourcesManager.singleton.GetWeapon(rh_weapons[i])
                    );
            }
            for (int i = 0; i < lh_weapons.Count; i++)
            {
                WeaponToRuntimeWeapon(
                    ResourcesManager.singleton.GetWeapon(lh_weapons[i])
                    , true);
            }

            if (r_rh_weapons.Count > 0)
            {
                if (r_index > r_rh_weapons.Count - 1)
                    r_index = 0;
                rightHandWeapon = r_rh_weapons[r_index];
            }
            if (r_lh_weapons.Count > 0)
            {
                if (r_index > r_lh_weapons.Count - 1)
                    r_index = 0;
                leftHandWeapon = r_lh_weapons[l_index];
            }


            if (rightHandWeapon != null)
                EquipWeapon(rightHandWeapon, false);

            if (leftHandWeapon != null)
            {
                EquipWeapon(leftHandWeapon, true);
                hasLeftHandWeapon = true;
            }


            for (int i = 0; i < spell_items.Count; i++)
            {
                SpellToRuntimeSpell(
                    ResourcesManager.singleton.GetSpell(spell_items[i])
                    );
            }

            if (r_spells.Count > 0)
            {
                if (s_index > r_spells.Count - 1)
                    s_index = 0;

                EquipSpell(r_spells[s_index]);
            }

            InitAllDamageColliders(states);
            CloseAllDamageColliders();
        }

        public void EquipWeapon(RuntimeWeapon w, bool isLeft = false)
        {
            if (isLeft)
            {
                if (leftHandWeapon != null)
                {
                   leftHandWeapon.weapon_Model.SetActive(false);
                  //  leftHandWeapon.weapon_Model.transform.localPosition = leftHandWeapon.instance.l_model_unequiped_pos;
                   // leftHandWeapon.weapon_Model.transform.localEulerAngles = leftHandWeapon.instance.l_model_unequiped_eulers;
                }
                leftHandWeapon = w;
            }
            else
            {
                if (rightHandWeapon != null)
                {
                    rightHandWeapon.weapon_Model.SetActive(false);
                  //  rightHandWeapon.weapon_Model.transform.localPosition = rightHandWeapon.instance.r_model_unequiped_pos;
                   // rightHandWeapon.weapon_Model.transform.localEulerAngles = rightHandWeapon.instance.r_model_unequiped_eulers;
                }
                rightHandWeapon = w;
            }

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

        public void EquipSpell(RuntimeSpellItems spell)
        {

            currentSpell = spell;
            UI.QuickSlot uiSlot = UI.QuickSlot.singleton;

            uiSlot.UpdateSlot(UI.QSlotType.spell, spell.instance.icon);
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

        public void InitAllDamageColliders(StateManager states)
        {
            if (rightHandWeapon.weapon_Hook != null)
                rightHandWeapon.weapon_Hook.InitDamageColliders(states);

            if (leftHandWeapon.weapon_Hook != null)
                leftHandWeapon.weapon_Hook.InitDamageColliders(states);
        }

        public void OpenParryCollider()
        {
            parryCollider.SetActive(true);
        }

        public void CloseParryCollider()
        {
            parryCollider.SetActive(false);
        }

        public RuntimeSpellItems SpellToRuntimeSpell(Spell s, bool isLeft = false)
        {
            GameObject go = new GameObject();
            RuntimeSpellItems inst = go.AddComponent<RuntimeSpellItems>();
            inst.instance = new Spell();
            StaticFunctions.DeepCopySpell(s, inst.instance);
            go.name = s.itemName;

            r_spells.Add(inst);
            return inst;
        }

        public void CreateSpellParticle(RuntimeSpellItems inst, bool isLeft = false, bool parentUnderRoot = false)
        {
            if (inst.currentParticle == null)
            {
                inst.currentParticle = Instantiate(inst.instance.particle_prefab) as GameObject;
                inst.p_hook = inst.currentParticle.GetComponentInChildren<ParticleHook>();
                inst.p_hook.Init();
            }

            if (!parentUnderRoot)
            {
                Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
                inst.currentParticle.transform.parent = p;
                inst.currentParticle.transform.localPosition = Vector3.zero;
                inst.currentParticle.transform.localRotation = Quaternion.identity;
            }
            else
            {
                inst.currentParticle.transform.parent = transform;
                inst.currentParticle.transform.localPosition = new Vector3(0f, 1f , .9f);
                inst.currentParticle.transform.localRotation = Quaternion.identity;
            }
            //inst.currentParticle.SetActive(false);
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

            inst.weapon_Model.transform.localPosition =
                (isLeft) ? inst.instance.l_model_equiped_pos : inst.instance.r_model_equiped_pos;
            inst.weapon_Model.transform.localEulerAngles =
                (isLeft) ? inst.instance.l_model_equiped_eulers : inst.instance.r_model_equiped_eulers;

            inst.weapon_Model.transform.localScale = inst.instance.model_scale;

            inst.weapon_Hook = inst.weapon_Model.GetComponentInChildren<WeaponHook>();
            inst.weapon_Hook.InitDamageColliders(states);

            if (isLeft)
            {
                r_lh_weapons.Add(inst);
            }
            else
            {
                r_rh_weapons.Add(inst);
            }

            inst.weapon_Model.SetActive(false);
            return inst;
        }

        /// <summary>
        /// Changes to the next weapon in the equiped weapons
        /// </summary>
        /// <param name="isLeft"> true :: left hand</param>
        public void ChangeToNextWeapon(bool isLeft)
        {
            if (isLeft)
            {
                if (l_index < r_lh_weapons.Count - 1)
                    l_index++;
                else
                    l_index = 0;
                EquipWeapon(r_lh_weapons[l_index], true);

            }
            else
            {
                if (r_index < r_rh_weapons.Count - 1)
                    r_index++;
                else
                    r_index = 0;
                EquipWeapon(r_rh_weapons[r_index]);
            }
            states.actionManager.UpdateActionsOneHanded();
        }

        public void ChangeToNextSpell()
        {
            if (s_index < r_spells.Count - 1)
                s_index++;
            else
                s_index = 0;

            EquipSpell(r_spells[s_index]);
        }

        #region Delegate Calls
        public void OpenBreathCollider()
        {
            breathCollider.SetActive(true);
        }

        public void CloseBreathCollider()
        {
            breathCollider.SetActive(false);
        }

        public void OpenBlockCollider()
        {
            blockCollider.SetActive(true);
        }

        public void CloseBlockCollider()
        {
            blockCollider.SetActive(false);
        }

        public void EmitSpellParticle()
        {
            currentSpell.p_hook.Emit(1);
        }
        
        #endregion
    }
    [System.Serializable]
    public class Item
    {
        public string itemName;
        public string itemDescription;
        public Sprite icon;

        public Action GetAction(List<Action> l, ActionInput inp)
        {
            if (l == null)
                return null;

            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].input == inp)
                {
                    return l[i];
                }
            }

            return null;
        }
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
        public WeaponStats weaponStats;

        public Vector3 r_model_equiped_pos;
        public Vector3 l_model_equiped_pos;
        public Vector3 r_model_equiped_eulers;
        public Vector3 l_model_equiped_eulers;
        public Vector3 model_scale;
    }

    [System.Serializable]
    public class Spell : Item
    {
        public SpellType spellType;
        public SpellClass spellClass;
        public List<SpellAction> spell_Actions = new List<SpellAction>();
        public GameObject projectile;
        public GameObject particle_prefab;
        public string spell_effect; // id

        public SpellAction GetSpellAction(List<SpellAction> l, ActionInput inp)
        {
            if (l == null)
                return null;

            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].input == inp)
                {
                    return l[i];
                }
            }

            return null;
        }
    }

}