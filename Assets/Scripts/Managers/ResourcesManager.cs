﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class ResourcesManager : MonoBehaviour
    {
        Dictionary<string, int> spell_ids = new Dictionary<string, int>();
        Dictionary<string, int> item_ids = new Dictionary<string, int>();
        public static ResourcesManager singleton;

        private void Awake()
        {
            singleton = this;
            LoadWeaponIds();
            LoadSpellIds();
        }
        void LoadSpellIds()
        {
            SpellItemScriptableObject obj = Resources.Load("GameControll.SpellItemScriptableObject") as SpellItemScriptableObject;
            if (obj == null)
            {
                Debug.Log("SpellItemScriptableObject coundlt load");
                return;
            }
            for (int i = 0; i < obj.spellItems_List.Count; i++)
            {
                if (spell_ids.ContainsKey(obj.spellItems_List[i].itemName))
                {
                    Debug.Log(obj.spellItems_List[i].itemName + " is duplicate");
                }
                else
                {
                    spell_ids.Add(obj.spellItems_List[i].itemName, i);
                }
            }
        }
        void LoadWeaponIds()
        {
            WeaponScriptableObject obj = Resources.Load("GameControll.WeaponScriptableObject") as WeaponScriptableObject;
            if (obj == null)
            {
                Debug.Log("WeaponScriptableObject coundlt load");
                return;
            }


            for (int i = 0; i < obj.wepons_all.Count; i++)
            {
                if (item_ids.ContainsKey(obj.wepons_all[i].itemName))
                {
                    Debug.Log("Item is duplicate");
                }
                else
                {
                    item_ids.Add(obj.wepons_all[i].itemName, i);
                }
            }
        }
        int GetWeaponIdFromString(string id)
        {
            int index = -1;
            if (item_ids.TryGetValue(id, out index))
                return index;
            return -1;
        }
        public Weapon GetWeapon(string id)
        {
            WeaponScriptableObject obj = Resources.Load("GameControll.WeaponScriptableObject") as WeaponScriptableObject;
            if (obj == null)
            {
                Debug.Log("WeaponScriptableObject cant load!");
                return null;
            }
            int index = GetWeaponIdFromString(id);

            if (index == -1)
                return null;

            return obj.wepons_all[index];
        }
        int GetSpellIdFromString(string id)
        {
            int index = -1;
            if (spell_ids.TryGetValue(id, out index))
            {
                return index;
            }

            return index;
        }
        public Spell GetSpell(string id)
        {
            SpellItemScriptableObject obj = Resources.Load("GameControll.SpellItemScriptableObject") as SpellItemScriptableObject;
            if (obj == null)
            {
                Debug.Log("SpellItemScriptableObject cant load!");
                return null;
            }

            int index = GetSpellIdFromString(id);
            if (index == -1)
                return null;

            return obj.spellItems_List[index];
        }
    }
}


