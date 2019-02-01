using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class ResourcesManager : MonoBehaviour
    {      
        Dictionary<string, int> spell_ids = new Dictionary<string, int>();
        Dictionary<string, int> item_ids = new Dictionary<string, int>();
        Dictionary<string,int>weaponStats_ids = new Dictionary<string, int>();
        Dictionary<string, int> consumables_ids = new Dictionary<string, int>(); 
        
        public static ResourcesManager singleton;

        private void Awake()
        {
            singleton = this;
            LoadWeaponIds();
            LoadSpellIds();
            LoadConsumables();
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
            for (int i = 0; i < obj.wepon_Stats.Count; i++)
            {
                if (weaponStats_ids.ContainsKey(obj.wepon_Stats[i].weaponId))
                {
                    Debug.Log(obj.wepon_Stats[i].weaponId + " is a duplicate");
                }
                else
                {
                    weaponStats_ids.Add(obj.wepon_Stats[i].weaponId, i);
                }
            }
        }
        void LoadConsumables()
        {
            ConsumableScriptableObject obj = Resources.Load("GameControll.ConsumableScriptableObject") as ConsumableScriptableObject;
            if (obj == null)
            {
                Debug.Log("GameControll.ConsumableScriptableObject couldnt be loaded!");
                return;
            }

            for (int i = 0; i < obj.consumables.Count; i++)
            {
                if (consumables_ids.ContainsKey(obj.consumables[i].itemName))
                {
                    Debug.Log(obj.consumables[i].itemName + " item is a duplicate");
                }
                else
                {
                    consumables_ids.Add(obj.consumables[i].itemName, i);
                }
            }
        }
        /*
        public List<Item> GetAllItemsFromList(List<string> l, ItemType t)
        {
            List<Item> r = new List<Item>();
            for (int i = 0; i < l.Count; i++)
            {
                Item it = GetItem(l[i], t);
                r.Add(it);
            }

            return r;
        }*/
        int GetIndexFromString(Dictionary<string,int> d, string id)
        {
            int index = -1;
            d.TryGetValue(id, out index);
            return index;
        }
        /*
        public Item GetItem(string id, ItemType type)
        {
            ItemScriptableObject obj = Resources.Load("GameControll.ItemScriptableObject") as ItemScriptableObject;

            if(obj == null)
            {
                Debug.Log("GameControll.ItemScriptableObject is null!");
            }

            Dictionary<string, int> d = null;
            List<Item> l = null;
 
            switch (type)
            {
                case ItemType.weapon:
                    d = i_weapons;
                    l = obj.weapon_items;
                    break;
                case ItemType.spell:
                    d = i_spells;
                    l = obj.spell_items;
                    break;
                case ItemType.consum:
                    d = i_cons;
                    l = obj.consumable_items;
                    break;
                case ItemType.equipment:
                default:
                    break;
            }

            if (d == null)
                return null;
            if (l == null)
                return null;

            int index = GetIndexFromString(d,id);
            if (index == -1)
                return null;

            return l[index];
        }

        */
        // Weapons
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
        int GetWeaponStatsIdFromString(string id)
        {
            int index = -1;
            if (weaponStats_ids.TryGetValue(id, out index))
            {
                return index;
            }

            return -1;
        }
        
        public WeaponStats GetWeaponStats(string id)
        {
            WeaponScriptableObject obj = Resources.Load("GameControll.WeaponScriptableObject") as WeaponScriptableObject;

            if (obj == null)
            {
                Debug.Log("GameControll.WeaponScriptableObject cant be loaded!");
                return null;
            }

            int index = GetWeaponStatsIdFromString(id);

            if (index == -1)
                return null;

            return obj.wepon_Stats[index];
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
        //Consumables
        int GetConsumableIdFromString(string id)
        {
            int index = -1;
            if (consumables_ids.TryGetValue(id, out index))
            {
                return index;
            }

            return index;
        }
        public Consumable GetConsumable(string id)
        {
            ConsumableScriptableObject obj = Resources.Load("GameControll.ConsumableScriptableObject") as ConsumableScriptableObject;

            if (obj == null)
            {
                Debug.Log("GameControll.ConsumableScriptableObject cant be loaded!");
                return null;
            }

            int index = GetConsumableIdFromString(id);
            if (index == -1)
                return null;

            return obj.consumables[index];
        }
    }
    public enum ItemType
    {
        weapon, spell, consum, equipment
    }
}


