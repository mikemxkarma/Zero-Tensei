using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameControll;
using System.Xml;
using System;

namespace GameControll.Utilities
{
    [ExecuteInEditMode]
    public class XMLtoResources : MonoBehaviour
    {
        public bool load;

        public ResourcesManager resourcesManager;
        public string weaponFileName = "items_database.xml";

        void Update()
        {
            if (!load)
                return;
            load = false;

            LoadWeaponData(resourcesManager);
        }
        public void LoadWeaponData(ResourcesManager rm)
        {
            string filePath = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            filePath += weaponFileName;

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode weapon in doc.DocumentElement.SelectNodes("//weapon"))
            {
                Weapon _weapon = new Weapon();
                _weapon.actions = new List<Action>();
                _weapon.two_handedActions = new List<Action>();


            //    XmlNode weaponId = weapon.SelectSingleNode("weaponId");
             //   _weapon.weaponId = weaponId.InnerText;

                XmlNode oh_idle = weapon.SelectSingleNode("oh_idle");
                _weapon.oh_idle = oh_idle.InnerText;
                XmlNode th_idle = weapon.SelectSingleNode("th_idle");
                _weapon.th_idle = th_idle.InnerText;

                XmlNode parryMultiplier = weapon.SelectSingleNode("parryMultiplier");
                float.TryParse(parryMultiplier.InnerText, out _weapon.parryMultiplier);
                XmlNode backstabMultiplier = weapon.SelectSingleNode("backstabMultiplier");
                float.TryParse(backstabMultiplier.InnerText, out _weapon.backstabMultiplier);


                XmlToActions(doc, "actions", ref _weapon);
                XmlToActions(doc, "two_handed", ref _weapon);

                XmlNode LeftHandMirror = weapon.SelectSingleNode("LeftHandMirror");
                _weapon.LeftHandMirror = (LeftHandMirror.InnerText == "True");

              //  _weapon.model_pos = XmlToVector(weapon, "mp");
               // _weapon.model_eulers = XmlToVector(weapon, "me");
              //  _weapon.model_scale = XmlToVector(weapon, "ms");



                //Add to Manager
            //    resourcesManager.weaponList.Add(_weapon);
            }

        }
        Vector3 XmlToVector(XmlNode weapon, string prefix)
        {
            XmlNode _x = weapon.SelectSingleNode(prefix + "_x");
            float x = 0;
            float.TryParse(_x.InnerText, out x);

            XmlNode _y = weapon.SelectSingleNode(prefix + "_y");
            float y = 0;
            float.TryParse(_y.InnerText, out y);
            XmlNode _z = weapon.SelectSingleNode(prefix + "_z");
            float z = 0;
            float.TryParse(_z.InnerText, out z);

            return new Vector3(x, y, z);
        }



        void XmlToActions(XmlDocument doc, string nodeName, ref Weapon _w)
        {
            foreach (XmlNode a in doc.DocumentElement.SelectNodes("//" + nodeName))
            {
                Action _action = new Action();

                XmlNode actionInput = a.SelectSingleNode("ActionInput");
                _action.input = (ActionInput)Enum.Parse(typeof(ActionInput), actionInput.InnerText);

                XmlNode actionType = a.SelectSingleNode("ActionType");
                _action.type = (ActionType)Enum.Parse(typeof(ActionType), actionType.InnerText);

                XmlNode targetAnimation = a.SelectSingleNode("targetAnimation");
                _action.targetAnimation = targetAnimation.InnerText;

                XmlNode mirror = a.SelectSingleNode("mirror");
                _action.mirror = (mirror.InnerText == "True");
                XmlNode canBeParried = a.SelectSingleNode("canBeParried");
                _action.canBeParried = (canBeParried.InnerText == "True");
                XmlNode changeSpeed = a.SelectSingleNode("changeSpeed");
                _action.changeSpeed = (changeSpeed.InnerText == "True");

                XmlNode animSpeed = a.SelectSingleNode("animSpeed");
                float.TryParse(animSpeed.InnerText, out _action.animSpeed);

                XmlNode canParry = a.SelectSingleNode("canParry");
                _action.canParry = (canParry.InnerText == "True");
                XmlNode canBackstab = a.SelectSingleNode("canBackstab");
                _action.canBackstab = (canBackstab.InnerText == "True");
                XmlNode overrideDamageAnim = a.SelectSingleNode("overrideDamageAnim");
                _action.overrideDamageAnim = (overrideDamageAnim.InnerText == "True");

                XmlNode damageAnim = a.SelectSingleNode("damageAnim");
                _action.damageAnim = damageAnim.InnerText;

                _action.weaponStats = new WeaponStats();

                XmlNode physical = a.SelectSingleNode("physical");
                int.TryParse(physical.InnerText, out _action.weaponStats.physical);
                XmlNode strike = a.SelectSingleNode("strike");
                int.TryParse(strike.InnerText, out _action.weaponStats.strike);
                XmlNode slash = a.SelectSingleNode("slash");
                int.TryParse(slash.InnerText, out _action.weaponStats.slash);
                XmlNode thrust = a.SelectSingleNode("thrust");
                int.TryParse(thrust.InnerText, out _action.weaponStats.thrust);
                XmlNode magic = a.SelectSingleNode("magic");
                int.TryParse(magic.InnerText, out _action.weaponStats.magic);
                XmlNode fire = a.SelectSingleNode("fire");
                int.TryParse(fire.InnerText, out _action.weaponStats.fire);
                XmlNode lighting = a.SelectSingleNode("lighting");
                int.TryParse(lighting.InnerText, out _action.weaponStats.lighting);
                XmlNode dark = a.SelectSingleNode("dark");
                int.TryParse(dark.InnerText, out _action.weaponStats.dark);

                if (nodeName == "actions")
                {
                    _w.actions.Add(_action);
                }
                else
                {
                    _w.two_handedActions.Add(_action);
                }
            }
        }
    }
}


