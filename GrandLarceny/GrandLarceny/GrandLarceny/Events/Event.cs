using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny.Events
{
	[Serializable()]
	class Event
	{
		private EventTrigger[] m_triggers;
		private EventEffect[] m_effects;

		//if the event should only occur once
		private bool m_oneShot;


		public Event(EventTrigger[] a_triggers, EventEffect[] a_effects, bool a_oneShot)
		{
			if (a_triggers.Length == 0 || a_effects.Length == 0)
			{
				throw new ArgumentException();
			}
			m_triggers = a_triggers;
			m_effects = a_effects;
			m_oneShot = a_oneShot;
		}
		//Returns true if it wants to be removed, and never checked again
		public bool Execute()
		{
			foreach (EventTrigger et in m_triggers)
			{
				if (!et.isTrue())
				{
					return false;
				}
			}
			foreach (EventEffect ee in m_effects)
			{
				ee.execute();
			}
			return m_oneShot;
		}
	}
}
