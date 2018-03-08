﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{

    public class InventoryManager : MonoBehaviour
    {
        public Weapon rightHandWeapon;
        public Weapon leftHandWeapon;
        public bool hasLeftHandWeapon = true;

        public void Init()
        {
            CloseAllDamageColliders();


        }

        public void CloseAllDamageColliders()
        {
            if (rightHandWeapon.w_hook != null)
                rightHandWeapon.w_hook.CloseDamageColliders();

            if (leftHandWeapon.w_hook != null)
                leftHandWeapon.w_hook.CloseDamageColliders();
        }
    }

    [System.Serializable]
    public class Weapon
    {
        public List<Action> action;
        public List<Action> two_handed_Actions;
        public bool leftHandMirror;
        public GameObject weaponModel;
        public WeaponHook w_hook;

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

    }
}
