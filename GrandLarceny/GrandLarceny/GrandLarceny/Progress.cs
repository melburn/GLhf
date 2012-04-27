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
		private String m_userName;
		private LinkedList<String> m_levelCleared;
		private Dictionary<String,Boolean> m_equipments;
		private Dictionary<String, int> m_consumables;

		public Progress(String a_saveName)
		{
			m_equipments = new Dictionary<string, bool>();
			m_levelCleared = new LinkedList<string>();
			m_consumables = new Dictionary<string, int>();
			m_saveName = a_saveName;
		}

		public string getName()
		{
			return m_saveName;
		}

		public void setProgress(string a_saveName, LinkedList<String> a_levelsCleared, Dictionary<String, Boolean> a_equipments, Dictionary<String, int> a_consumables)
		{
			m_saveName = a_saveName;
			m_levelCleared = a_levelsCleared;
			m_equipments = a_equipments;
			m_consumables = a_consumables;
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
		public Boolean hasClearedLevel(string a_level)
		{
			return m_levelCleared.Contains(a_level);
		}

		public void setLevelCleared(string a_level)
		{
			m_levelCleared.AddLast(a_level);
		}

		public void setUserName(string a_userName)
		{
			m_userName = a_userName;
		}

		public string getUserName()
		{
			return m_userName;
		}
	}
}
