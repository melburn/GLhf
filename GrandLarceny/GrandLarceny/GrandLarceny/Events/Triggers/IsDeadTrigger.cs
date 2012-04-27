using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny.Events.Triggers
{
	[Serializable()]
	public class IsDeadTrigger : EventTrigger
	{
		private bool m_condition;
		private GameObject m_object;

		public IsDeadTrigger(GameObject a_object, bool a_condition)
		{
			m_condition = a_condition;
			m_object = a_object;
		}

		public override bool isTrue()
		{
			return m_object.isDead() == m_condition;
		}

		public override string ToString()
		{
			if (m_condition)
			{
				return m_object + " is alive";
			}
			else
			{
				return m_object + " is dead";
			}
		}
	}
}
