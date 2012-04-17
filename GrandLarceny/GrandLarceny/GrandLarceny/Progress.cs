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
		private LinkedList<String> m_levelCleared;
		private Dictionary<String,Boolean> m_equipments;
		private int[] m_consumables;

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

		public Boolean hasClearedLevel(string a_level)
		{
			return m_levelCleared.Contains(a_level);
		}

		public void setLevelCleared(string a_level)
		{
			m_levelCleared.AddLast(a_level);
		}
	}
}
