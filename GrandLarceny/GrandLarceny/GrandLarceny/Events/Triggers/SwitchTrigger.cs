using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny.Events.Triggers
{
	[Serializable()]
	class SwitchTrigger : EventTrigger
	{
		[NonSerialized()]
		private LampSwitch m_switch;
		private int m_switchLink;

		private TriggerType m_triggerType;
		private Boolean m_lastOn;

		public enum TriggerType
		{
			clicked,
			on,
			unclicked,
			off,
		}

		public SwitchTrigger(LampSwitch a_switch, TriggerType a_triggerType)
		{
			if (a_switch == null || a_triggerType == null)
			{
				throw new ArgumentNullException();
			}
			m_switch = a_switch;
			m_triggerType = a_triggerType;
			m_lastOn = a_switch.isOn();
		}

		public override bool isTrue()
		{
			bool t_ret;
			switch (m_triggerType)
			{
				case TriggerType.clicked:
					{
						t_ret = (!m_lastOn) && m_switch.isOn();
						break;
					}
				case TriggerType.off:
					{
						t_ret = !m_switch.isOn();
						break;
					}
				case TriggerType.on:
					{
						t_ret = m_switch.isOn();
						break;
					}
				case TriggerType.unclicked:
					{
						t_ret = m_lastOn && (!m_switch.isOn());
						break;
					}
				default: throw new NotImplementedException();
			}
			m_lastOn = m_switch.isOn();
			return t_ret;
		}

		public override string ToString()
		{
			return "switch " + m_switch.getId() + " is " + m_triggerType;
		}

		public override void linkObject()
		{
			if (m_switch != null)
			{
				m_switchLink = m_switch.getId();
			}
		}

		public override void loadContent()
		{
			if (m_switchLink > 0)
			{
				m_switch = (LampSwitch)Game.getInstance().getState().getObjectById(m_switchLink);
			}
		}
	}
}
