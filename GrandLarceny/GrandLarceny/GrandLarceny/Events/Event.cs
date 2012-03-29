using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny.Events
{
	[Serializable()]
	class Event
	{
		private LinkedList<EventTrigger> m_triggers;
		private LinkedList<EventEffect> m_effects;

		//if the event should only occur once
		private bool m_oneShot;


		public Event(LinkedList<EventTrigger> a_triggers, LinkedList<EventEffect> a_effects, bool a_oneShot)
		{
			m_triggers = a_triggers;
			m_effects = a_effects;
			m_oneShot = a_oneShot;
		}
		//Returns true if it wants to be removed and never checked again
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

		public void add(EventEffect a_eveEffect)
		{
			m_effects.AddLast(a_eveEffect);
		}

		public void add(EventTrigger a_eveTrigger)
		{
			m_triggers.AddLast(a_eveTrigger);
		}

		public LinkedList<EventEffect> getEffects()
		{
			return m_effects;
		}

		public LinkedList<EventTrigger> getTriggers()
		{
			return m_triggers;
		}

		public bool remove(EventEffect a_eveEffect)
		{
			return m_effects.Remove(a_eveEffect);
		}

		public bool remove(EventTrigger a_eveTrigger)
		{
			return m_triggers.Remove(a_eveTrigger);
		}


		public void linkObject()
		{
			foreach (EventEffect t_ee in m_effects)
			{
				t_ee.linkObject();
			}
			foreach (EventTrigger t_et in m_triggers)
			{
				t_et.linkObject();
			}
		}
		public void loadContent()
		{
			foreach (EventEffect t_ee in m_effects)
			{
				t_ee.loadContent();
			}
			foreach (EventTrigger t_et in m_triggers)
			{
				t_et.loadContent();
			}
		}
	}
}
