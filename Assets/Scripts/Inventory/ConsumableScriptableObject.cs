using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{    
    public class ConsumableScriptableObject : ScriptableObject
    {
        public List<Consumable> consumables = new List<Consumable>();
    }
}
