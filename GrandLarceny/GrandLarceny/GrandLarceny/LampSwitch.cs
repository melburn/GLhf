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
			m_connectedSpotLights.AddLast(a_spotlight);
			a_spotlight.setLit(m_switchedOn);
		}
		public void disconnectSpotLight(SpotLight a_spotlight)
		{
			m_connectedSpotLights.Remove(a_spotlight);
		}
		public void toogleSwitch()
		{
			m_switchedOn = !m_switchedOn;
			foreach (SpotLight t_spotlight in m_connectedSpotLights)
			{
				t_spotlight.setLit(m_switchedOn);
			}
			if (m_switchedOn)
			{
				//m_img.setSprite(on);
				foreach (GameObject go in Game.getInstance().getState().getObjectList())
				{
					if (go is Guard)
					{
						((Guard)go).removeLampSwitchTarget(this);
					}
				}
			}
			else
			{
				//m_img.setSprite(off);
				foreach (GameObject go in Game.getInstance().getState().getObjectList())
				{
					if (go is Guard && CollisionManager.possibleLineOfSight(go.getPosition().getGlobalCartesianCoordinates(), m_position.getGlobalCartesianCoordinates()))
					{
						((Guard)go).addLampSwitchTarget(this);
					}
				}
			}
		}
		public void setSwitch(bool a_on)
		{
			m_switchedOn = a_on;
			foreach (SpotLight t_spotlight in m_connectedSpotLights)
			{
				t_spotlight.setLit(m_switchedOn);
			}
			if (m_switchedOn)
			{
				//m_img.setSprite(on);
			}
			else
			{
				//m_img.setSprite(off);
			}
		}

		public bool isOn()
		{
			return m_switchedOn;
		}

		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			foreach (Entity t_collision in a_collisionList)
			{
				if (t_collision is Player && GameState.m_currentKeyInput.IsKeyDown(Keys.Up) && GameState.m_previousKeyInput.IsKeyUp(Keys.Up))
				{
					toogleSwitch();
				}
			}
		}
	}
}
