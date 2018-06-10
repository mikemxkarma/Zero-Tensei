using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    [ExecuteInEditMode]
    public class WeaponPlacer : MonoBehaviour
    {
        public string weaponId;

        public GameObject weaponModel;

        public bool leftHand;
        public bool saveWeapon;

        void Update()
        {
            if (!saveWeapon)
                return;

            saveWeapon = false;

            if(weaponModel == null)
                return;
            if (string.IsNullOrEmpty(weaponId))
                return;

            WeaponScriptableObject obj = Resources.Load("GameControll.WeaponScriptableObject") as WeaponScriptableObject;

            if (obj == null)
                return;
            for(int i= 0; i< obj.wepons_all.Count; i++)
            {
                if (obj.wepons_all[i].itemName == weaponId)
                {
                    Weapon w = obj.wepons_all[i];

                    if (leftHand)
                    {
                        w.l_model_equiped_eulers = weaponModel.transform.localEulerAngles;
                        w.l_model_equiped_pos = weaponModel.transform.localPosition;
                    }
                    else
                    {
                        w.r_model_equiped_eulers = weaponModel.transform.localEulerAngles;
                        w.r_model_equiped_pos = weaponModel.transform.localPosition;
                    }

                    w.model_scale = weaponModel.transform.localScale;
                    return;
                }
            }
            Debug.Log(weaponId + " wasnt found in inventory");
        }
    }
}

