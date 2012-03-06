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
				a_spotlight.setLit(m_switchedOn);
			}
		}
		public void disconnectSpotLight(SpotLight a_spotlight)
		{
			m_connectedSpotLights.Remove(a_spotlight);
		}
		public void toogleSwitch()
		{
			m_switchedOn = !m_switchedOn;
			LinkedList<SpotLight> t_deadSpotLights = new LinkedList<SpotLight>();
			foreach (SpotLight t_spotlight in m_connectedSpotLights)
			{
				if (t_spotlight.isDead())
				{
					t_deadSpotLights.AddLast(t_spotlight);
				}
				else
				{
					t_spotlight.setLit(m_switchedOn);
				}
			}
			m_connectedSpotLights.Except(t_deadSpotLights);
			if (m_switchedOn)
			{
				//m_img.setSprite(on);
			}
			else
			{
				//m_img.setSprite(off);
				foreach (GameObject go in Game.getInstance().getState().getCurrentList())
				{
					if (go is Guard && CollisionManager.possibleLineOfSight(go.getPosition().getGlobalCartesianCoordinates(), m_position.getGlobalCartesianCoordinates()))
					{
						((Guard)go).addLampSwitchTarget(this);
					}
				}
			}
		}

		public bool isOn()
		{
			return m_switchedOn;
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
	}
}
