using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameControll;
using System.IO;

namespace GameControll.Utilities
{
    [ExecuteInEditMode]
    public class ItemToXML : MonoBehaviour
    {
        public bool make;
        public List<RuntimeWeapon> candidatos = new List<RuntimeWeapon>();
        public string xml_version;
        public string targetName;

        private void Update()
        {
            if (!make)
                return;
            make = false;

            string xml = xml_version; //<?xml version = "1.0" enconding = "UTF-8"?>
            xml += "\n";
            xml += "<root>";

            foreach (RuntimeWeapon i in candidatos)
            {
                Weapon w = i.instance;

                xml += "<weapon>" + "\n";
               // xml += "<weaponId>" + w.itemId + "</weaponId>" + "\n";
                xml += "<oh_idle>" + w.oh_idle + "</oh_idle>" + "\n";
                xml += "<th_idle>" + w.th_idle + "</th_idle>" + "\n";

                xml += ActionListToString(w.actions, "actions");
                xml += ActionListToString(w.two_handedActions, "two_handed");

                xml += "<parryMultiplier>" + w.parryMultiplier + "</parryMultiplier>" + "\n";
                xml += "<backstabMultiplier>" + w.backstabMultiplier + "</backstabMultiplier>" + "\n";
                xml += "<LeftHandMirror>" + w.LeftHandMirror + "</LeftHandMirror>" + "\n";

             /*   xml += "<mp_x>" + w.model_pos.x + "</mp_x>";
                xml += "<mp_y>" + w.model_pos.y + "</mp_y>";
                xml += "<mp_z>" + w.model_pos.z + "</mp_z>" +"\n";

                xml += "<me_x>" + w.model_eulers.x + "</me_x>";
                xml += "<me_y>" + w.model_eulers.y + "</me_y>";
                xml += "<me_z>" + w.model_eulers.z + "</me_z>" + "\n";

                xml += "<ms_x>" + w.model_scale.x + "</ms_x>";
                xml += "<ms_y>" + w.model_scale.y + "</ms_y>";
                xml += "<ms_z>" + w.model_scale.z + "</ms_z>" + "\n";
                */
                xml += "</weapon>" + "\n";
            }


            xml += "</root>";


            string path = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            if (string.IsNullOrEmpty(targetName))
            {
                targetName = "items_database.xml";
            }
            //      else
            //     {
            //         targetName += ".xml";
            //     }

            path += targetName;

            File.WriteAllText(path, xml);


        }

        string ActionListToString(List<Action> l, string nodeName)
        {
            string xml = null;

            foreach (Action a in l)
            {
                xml += "<" + nodeName + ">" + "\n";
                xml += "<ActionInput>" + a.input.ToString() + "</ActionInput>" + "\n";
                xml += "<ActionType>" + a.type.ToString() + "</ActionType>" + "\n";
                xml += "<targetAnimation>" + a.targetAnimation.ToString() + "</targetAnimation>" + "\n";
                xml += "<mirror>" + a.mirror.ToString() + "</mirror>" + "\n";
                xml += "<canBeParried>" + a.canBeParried + "</canBeParried>" + "\n";
                xml += "<changeSpeed>" + a.changeSpeed + "</changeSpeed>" + "\n";
                xml += "<animSpeed>" + a.animSpeed + "</animSpeed>" + "\n";
                xml += "<canParry>" + a.canParry + "</canParry>" + "\n";
                xml += "<canBackstab>" + a.canBackstab + "</canBackstab>" + "\n";
                xml += "<overrideDamageAnim>" + a.overrideDamageAnim + "</overrideDamageAnim>" + "\n";
                xml += "<damageAnim>" + a.damageAnim + "</damageAnim>" + "\n";

                WeaponStats s = a.weaponStats;

                xml += "<physical>" + s.physical + "</physical>" + "\n";
                xml += "<strike>" + s.physical + "</strike>" + "\n";
                xml += "<slash>" + s.physical + "</slash>" + "\n";
                xml += "<thrust>" + s.physical + "</thrust>" + "\n";
                xml += "<magic>" + s.physical + "</magic>" + "\n";
                xml += "<fire>" + s.physical + "</fire>" + "\n";
                xml += "<lighting>" + s.physical + "</lighting>" + "\n";
                xml += "<dark>" + s.physical + "</dark>" + "\n";

                xml += "</" + nodeName + ">" + "\n";

            }
            return xml;
        }
    }
}