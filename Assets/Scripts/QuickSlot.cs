using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll.UI
{
    public class QuickSlot : MonoBehaviour
    {
        public List<QSlots> slots = new List<QSlots>();

        public void Init()
        {
            ClearIcons();
        }

        public void ClearIcons()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].icon.gameObject.SetActive(false);
            }
        }

        public void UpdateSlot(QSlotType stype, Sprite i)
        {
            QSlots q = GetSlot(stype);
            q.icon.sprite = i;
            q.icon.gameObject.SetActive(true);
        }

        public QSlots GetSlot(QSlotType t)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].type == t)
                    return slots[i];
            }

            return null;
        }

        public static QuickSlot singleton;
        void Awake()
        {
            singleton = this;
        }
    }

    public enum QSlotType
    {
        rh, lh, item, spell
    }

    [System.Serializable]
    public class QSlots
    {
        public Image icon;
        public QSlotType type;
    }
}
