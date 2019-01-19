using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameControll
{
    public class WeaponScriptableObject : ScriptableObject
    {
        public List<Weapon> wepons_all = new List<Weapon>();
        public List<WeaponStats> wepon_Stats = new List<WeaponStats>();
    }
}

