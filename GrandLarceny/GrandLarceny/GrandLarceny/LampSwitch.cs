using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	[Serializable()]
	public class LampSwitch : NonMovingObject
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
			if (!a_spotlight.isDead())
			{
				m_connectedSpotLights.AddLast(a_spotlight);
				a_spotlight.toggleLight();
			}
		}
		public void disconnectSpotLight(SpotLight a_spotlight)
		{
			m_connectedSpotLights.Remove(a_spotlight);
		}
		public void toogleSwitch()
		{
			foreach (SpotLight t_spotLight in m_connectedSpotLights)
			{
				t_spotLight.toggleLight();
				if (!t_spotLight.isLit())
				{
					foreach (GameObject t_guard in Game.getInstance().getState().getCurrentList())
					{
						if (t_guard is Guard && CollisionManager.possibleLineOfSight(t_guard.getPosition().getGlobalCartesianCoordinates(), m_position.getGlobalCartesianCoordinates()))
						{
							((Guard)t_guard).addLampSwitchTarget(this);
						}
					}
				}
			}
		}

		public bool isOn()
		{
			foreach (SpotLight t_spotLight in m_connectedSpotLights)
			{
				if (!t_spotLight.isLit())
					return false;
			}
			return true;
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player && GameState.isKeyPressed(Keys.Up) && !GameState.wasKeyPressed(Keys.Up))
			{
				toogleSwitch();
			}
		}

		public LinkedList<SpotLight> getConnectedSpotLights()
		{
			return m_connectedSpotLights;
		}

		public bool isConnectedTo(SpotLight a_spotLight)
		{
			foreach (SpotLight t_spotLight in m_connectedSpotLights)
			{
				if (a_spotLight == t_spotLight)
				{
					return true;
				}
			}
			return false;
		}
	}
}
