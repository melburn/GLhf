using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny.Events.Triggers
{
	[Serializable()]
	public class ChaseTrigger : EventTrigger
	{
		private bool m_condition;

		public ChaseTrigger(bool a_condition)
		{
			m_condition = a_condition;
		}

		public override bool isTrue()
		{
			return Game.getInstance().getState().getPlayer().isChase() == m_condition;
		}

		public override string ToString()
		{
			return "chase: " + m_condition;
		}
	}
}
