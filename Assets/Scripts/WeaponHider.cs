using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class WeaponHider : MonoBehaviour {

    public GameObject weaponleg_L;
    public bool hideweaponleg_L;
    public GameObject weaponleg_R;
    public bool hideweaponleg_R;
    public GameObject weaponBack_L;
    public bool hideweaponBack_L;
    public GameObject weaopnBack_R;
    public bool hideweaopnBack_R;

    public GameObject weaponHip_L;
    public bool hideweaponHip_L;
    public GameObject weaponHip_R;
    public bool hidewweaponHip_R;

    public GameObject weaponBackSmall_L;
    public bool hideweaponBackSmall_L;
    public GameObject weaponBackSmall_R;
    public bool hideweaponBackSmall_R;

    public InventoryManager weaponsEquiped;

    // Use this for initialization
    void Start () {
            hideweaponleg_L = true;
            hideweaponleg_R = true;
            hideweaponBack_L = true;
            hideweaopnBack_R = true;
            hideweaponHip_L = true;
            hidewweaponHip_R = true;
            hideweaponBackSmall_L = true;
            hideweaponBackSmall_R = true;
            weaponleg_L.SetActive(false);
            weaponleg_R.SetActive(false);
            weaponBack_L.SetActive(false);
            weaopnBack_R.SetActive(false);
            weaponHip_L.SetActive(false);
            weaponHip_R.SetActive(false);
            weaponBackSmall_L.SetActive(false);
            weaponBackSmall_R.SetActive(false);
        }
	
	// Update is called once per frame
	void Update () {

            if (hideweaponleg_L == false)
                weaponleg_L.SetActive(true);
            else
                weaponleg_L.SetActive(false);

            if (hideweaponleg_R == false)
                weaponleg_R.SetActive(true);
            else
                weaponleg_R.SetActive(false);

            if (hideweaponBack_L == false)
                weaponBack_L.SetActive(true);
            else
                weaponBack_L.SetActive(false);

            if (hideweaopnBack_R == false)
                weaopnBack_R.SetActive(true);
            else
                weaopnBack_R.SetActive(false);

            if (hideweaponHip_L == false)
                weaponHip_L.SetActive(true);
            else
                weaponHip_L.SetActive(false);

            if (hidewweaponHip_R == false)
                weaponHip_R.SetActive(true);
            else
                weaponHip_R.SetActive(false);

            if (hideweaponBackSmall_L == false)
                weaponBackSmall_L.SetActive(true);
            else
               weaponBackSmall_L.SetActive(false);

            if (hideweaponBackSmall_R == false)
                weaponBackSmall_R.SetActive(true);
            else
                weaponBackSmall_R.SetActive(false);
        }
}
}