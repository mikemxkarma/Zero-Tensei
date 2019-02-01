using System.Collections;
using System.Collections.Generic;
using GameControll;
using UnityEngine;

public class ItemScriptableObject : ScriptableObject
{

    public List<Item> spell_items = new List<Item>();
    public List<Item> weapon_items = new List<Item>();
    public List<Item> consumable_items = new List<Item>();
    
}
