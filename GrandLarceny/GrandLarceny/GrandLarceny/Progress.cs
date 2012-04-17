using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
{
	[Serializable()]
	public class Progress
	{
		private String m_saveName;
		private Dictionary<String,Boolean> m_equipments;
		private Dictionary<String, int> m_consumables;

		public Progress(String a_saveName)
		{
			m_saveName = a_saveName;
			m_equipments = new Dictionary<string, bool>();
		}

		public Boolean hasEquipment(string a_equipment)
		{
			return m_equipments.ContainsKey(a_equipment) && m_equipments[a_equipment];
		}

		public void setEquipment(string a_equipment, bool a_has)
		{
			m_equipments[a_equipment] = a_has;
		}

		public void increaseConsumable(string a_consumable)
		{
			if (m_consumables.ContainsKey(a_consumable))
			{
				m_consumables.Add(a_consumable, 1);
			}
			else
			{
				m_consumables.Add(a_consumable, m_consumables[a_consumable] + 1);
			}
		}

		public bool decreaseConsumable(string a_consumable)
		{
			if (m_consumables.ContainsKey(a_consumable) && m_consumables[a_consumable] > 0)
			{
				m_consumables.Add(a_consumable, m_consumables[a_consumable] - 1);
				return true;
			}
			else
			{
				return false;
			}
		}
		public bool hasConsumable(string a_consumable)
		{
			return m_consumables.ContainsKey(a_consumable) && m_consumables[a_consumable] > 0;
		}
	}
}
