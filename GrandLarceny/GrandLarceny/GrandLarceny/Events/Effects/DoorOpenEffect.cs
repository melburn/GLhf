using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny.Events.Effects
{
	[Serializable()]
	class DoorOpenEffect : EventEffect
	{
		[NonSerialized]
		private SecurityDoor m_doorToOpen;
		private int m_doorLink;

		private Boolean m_closeAfterwards;
		private float m_openSpeed;
		private float m_closeSpeed;


		public override void execute()
		{
			m_doorToOpen.setCloseWhenOpen(true);
			m_doorToOpen.setOpeningSpeed(m_openSpeed);
			m_doorToOpen.setClosingSpeed(m_closeSpeed);
			m_doorToOpen.open();
		}

		public override string ToString()
		{
			return "Open door " + m_doorToOpen.getId();
		}

		public override void linkObject()
		{
			if (m_doorToOpen != null)
			{
				m_doorLink = m_doorToOpen.getId();
			}
		}

		public override void loadContent()
		{
			if (m_doorLink > 0)
			{
				m_doorToOpen = (SecurityDoor)Game.getInstance().getState().getObjectById(m_doorLink);
			}
		}
	}
}
