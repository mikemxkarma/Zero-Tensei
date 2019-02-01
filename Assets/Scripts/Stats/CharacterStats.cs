using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    [System.Serializable]
    public class CharacterStats
    {
        [Header("Current")]
        public float _health;
        public float _mana;
        public float _stamina;
        [Header("Base Power")]
        public int hp = 20;
        public int fp = 100;
        public int stamina = 30;
        public float equipLoad = 20;
        public float poise = 20;
        public int itemDiscover = 111;

        [Header("Attack Power")]
        public int R_weapon_1 = 51;
        public int R_weapon_2 = 51;
        public int R_weapon_3 = 51;
        public int L_weapon_1 = 51;
        public int L_weapon_2 = 51;
        public int L_weapon_3 = 51;

        [Header("Defence")]
        public int physical = 87;
        public int vs_strike = 87;
        public int vs_slash = 87;
        public int vs_thrust = 87;
        public int magic = 30;
        public int fire = 30;
        public int lighting = 30;
        public int dark = 30;

        [Header("Resistances")]
        public int bleed = 100;
        public int poison = 100;
        public int frost = 100;
        public int curse = 100;

        public int attunemntSlots = 0;

        public void InitCurrent()
        {
            if (statEffects != null)
            {
                statEffects();
            }
            _health = hp;
            _mana = fp;
            _stamina = stamina;


        }

        public delegate void StatEffects();
        public StatEffects statEffects;

        public void AddHealth()
        {
            hp += 5;
        }
        public void RemoveHealth()
        {
            hp = -5;
        }
    }
    [System.Serializable]
    public class Atributes
    {
        public int level = 1;
        public int fame = 0;
        public int vigor = 11;
        public int attunemnt = 11;
        public int vitality = 11;
        public int strength = 11;
        public int dexterity = 11;
        public int intellegence = 11;
        public int faith = 11;
        public int luck = 11;
    }
    [System.Serializable]
    public class WeaponStats
    {
        public string weaponId;
        public int physical;
        public int strike;
        public int slash;
        public int thrust;
        public int magic = 0;
        public int fire = 0;
        public int lighting = 0;
        public int dark = 0;
    }

}

