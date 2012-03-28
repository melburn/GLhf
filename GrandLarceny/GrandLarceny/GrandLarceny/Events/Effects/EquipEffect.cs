using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny.Events.Effects
{
	[Serializable()]
	public class EquipEffect : EventEffect
	{
		private bool m_equip;
		private string m_name;

		public EquipEffect(string a_name, bool a_equip)
		{
			m_name = a_name;
			m_equip = a_equip;
		}
		public override void execute()
		{
			if (Game.getInstance().getProgress() == null)
			{
				throw new Exception("Cannot find progress");
			}
			else
			{
				Game.getInstance().getProgress().setEquipment(m_name, m_equip);
			}
		}

		public override string ToString()
		{
			if (m_equip)
			{
				return "Equip " + m_name;
			}
			else
			{
				return "Unequip " + m_name;
			}
		}
	}
}
