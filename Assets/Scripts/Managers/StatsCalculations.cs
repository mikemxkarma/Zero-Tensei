﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameControll
{
    public static class StatsCalculations
    {
        public static int CalculateBaseDamage(WeaponStats w, CharacterStats st, float multiplier = 1)
        {
            float physical = w.physical * multiplier - st.physical;
            float strike = w.strike * multiplier - st.vs_strike;
            float slash = w.slash * multiplier - st.vs_slash;
            float thrust = w.thrust * multiplier - st.vs_thrust;

            float sum = physical + strike + slash + thrust;

            float magic = w.magic * multiplier - st.magic;
            float fire = w.fire * multiplier - st.fire;
            float lighting = w.lighting * multiplier - st.lighting;
            float dark = w.dark * multiplier - st.dark;

            sum += magic + fire + lighting + dark;

            if (sum <= 0)
                sum = 1;

            return Mathf.RoundToInt(sum);
        }

    }
}
