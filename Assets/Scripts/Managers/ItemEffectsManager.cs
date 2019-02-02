using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
	public class ItemEffectsManager : MonoBehaviour
	{

		Dictionary<string, int> effects = new Dictionary<string, int>();

		void InitEffectsId()
		{
			effects.Add("health_potion", 0);
			effects.Add("mana_potion", 1);
			effects.Add("shiny", 2);
		}

		public void CastEffect(string effectId, StateManager states)
		{
			int i = GetIntFromId(effectId);
			if (i < 0)
			{
				Debug.Log(effectId + " effect doesnt exist");
				return;
			}

			switch (i)
			{
				case 0: //health
					AddHealth(states);
					break;
				case 1: //mana
					AddMana(states);
					break;
				case 2: //adds xp
					AddFame(states);
					break;
			}

		}

		#region Effects Actual

		void AddHealth(StateManager states)
		{
			states.characterStats._health += states.characterStats._healthRecoverValue;
		}

		void AddMana(StateManager states)
		{
			states.characterStats._mana += states.characterStats._manaRecoverValue;
		}

		void AddFame(StateManager states)
		{
			states.characterStats._fame += 100;
		}

		#endregion


		int GetIntFromId(string id)
		{
			int index = -1;
			if (effects.TryGetValue(id, out index))
			{
				return index;
			}

			return index;
		}



		public static ItemEffectsManager singleton;

		void Awake()
		{
			singleton = this;
			InitEffectsId();
		}
	}
}

