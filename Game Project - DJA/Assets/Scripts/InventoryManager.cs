using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{

    public class InventoryManager : MonoBehaviour
    {
        public Weapon currentWeapon;


        public void Init()
        {
        }
    }

    [System.Serializable]
    public class Weapon
    {
        public List<Action> action;
        public List<Action> two_handed_Actions;
    }
}
