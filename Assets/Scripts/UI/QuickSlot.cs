using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameControll.UI
{
    public class QuickSlot : MonoBehaviour
    {
        public List<QSlots> slots = new List<QSlots>();
        public void Init()
        {

        }
        public void UpdateSlot(QSlotType stype,Sprite i)
        {
            //  QSlots 
            QSlots q = Getslot(stype);
            q.icon.sprite = i;
        }
        public QSlots Getslot(QSlotType t)
        {
            for(int i = 0; i< slots.Count; i++)
            {
                if(slots[i].type == t)
                    return slots[i];
            }
            return null;
        }
        public static QuickSlot singleton;
        private void Awake()
        {
            singleton = this;
        }

    }
    public enum QSlotType
    {
        rh,lh,item,spell
    }
    [System.Serializable]
    public class QSlots
    {
        public Image icon;
        public QSlotType type;
    }
}

