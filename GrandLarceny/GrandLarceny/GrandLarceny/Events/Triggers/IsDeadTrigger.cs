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

		[NonSerialized]
		private GameObject m_object;
		private int m_objectLink;

		public IsDeadTrigger(GameObject a_object, bool a_condition)
		{
			if (m_object == null)
			{
				throw new ArgumentNullException();
			}
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

		public override void linkObject()
		{
			if (m_object != null)
			{
				m_objectLink = m_object.getId();
			}
		}

		public override void loadContent()
		{
			if (m_objectLink > 0)
			{
				m_object = (LampSwitch)Game.getInstance().getState().getObjectById(m_objectLink);
				if (m_object == null)
				{
					throw new ArgumentNullException("SwitchTrigger could not find switch " + m_objectLink);
				}
			}
		}
	}
}
