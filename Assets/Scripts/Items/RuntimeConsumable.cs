using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
	public class RuntimeConsumable : MonoBehaviour
	{
		public int itemCount = 2;
		public bool unlimitedCount;
		public Consumable inst;
		public GameObject itemModel;
	}
}
