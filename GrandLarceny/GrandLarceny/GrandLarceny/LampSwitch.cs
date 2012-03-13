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
		[NonSerialized]
		private LinkedList<SpotLight> m_connectedSpotLights;

		private LinkedList<int> m_connectedSpotLightsId;
		private bool m_switchedOn;

		public LampSwitch(Vector2 a_position, String a_sprite, float a_layer, bool a_switchedOn)
			: base(a_position, a_sprite, a_layer)
		{
			m_switchedOn = a_switchedOn;
		}

		public override void loadContent()
		{
			base.loadContent();
			m_connectedSpotLights = new LinkedList<SpotLight>();
			if (m_connectedSpotLightsId == null)
			{
				m_connectedSpotLightsId = new LinkedList<int>();
			}
			foreach(int t_ids in m_connectedSpotLightsId)
			{
				m_connectedSpotLights.AddLast((SpotLight)Game.getInstance().getState().getObjectById(t_ids));
			}

		}
		public void connectSpotLight(SpotLight a_spotlight)
		{
			if (!a_spotlight.isDead() && !m_connectedSpotLights.Contains(a_spotlight))
			{
				m_connectedSpotLights.AddLast(a_spotlight);
				m_connectedSpotLightsId.AddLast(a_spotlight.getId());
				//a_spotlight.toggleLight();
			}
		}
		public void disconnectSpotLight(SpotLight a_spotlight)
		{
			m_connectedSpotLights.Remove(a_spotlight);
			m_connectedSpotLightsId.Remove(a_spotlight.getId());
		}
		public void toggleSwitch()
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
			if (a_collid is Player && Game.getInstance().keyClicked(Keys.Up))
			{
				toggleSwitch();
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
