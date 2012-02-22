using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class LampSwitch : NonMovingObject
	{
		private LinkedList<SpotLight> m_connectedSpotLights;
		private bool m_switchedOn;

		public LampSwitch(Vector2 a_position, String a_sprite, float a_layer, bool a_switchedOn)
			: base(a_position, a_sprite, a_layer)
		{
			m_connectedSpotLights = new LinkedList<SpotLight>();
			m_switchedOn = a_switchedOn;
		}
		public void connectSpotLight(SpotLight a_spotlight)
		{
			m_connectedSpotLights.AddLast(a_spotlight);
			a_spotlight.setLit(m_switchedOn);
		}
		public void toogleSwitch()
		{
			m_switchedOn = !m_switchedOn;
			foreach (SpotLight t_spotlight in m_connectedSpotLights)
			{
				t_spotlight.setLit(m_switchedOn);
			}
		}
		public void setSwitch(bool a_on)
		{
			m_switchedOn = a_on;
			foreach (SpotLight t_spotlight in m_connectedSpotLights)
			{
				t_spotlight.setLit(m_switchedOn);
			}
		}
	}
}
