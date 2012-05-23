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
		private bool m_connectedToAll;

		[NonSerialized]
		private Sound m_switchSound;

		public LampSwitch(Vector2 a_position, String a_sprite, float a_layer)
			: base(a_position, a_sprite, a_layer)
		{
			m_switchedOn = a_sprite == "Images//Prop//Button//1x1_alight_switch_on";
			m_connectedToAll = false;
		}

		public override void linkObject()
		{
			base.linkObject();
			if (m_connectedSpotLights != null)
			{
				m_connectedSpotLightsId = new LinkedList<int>();
				foreach (SpotLight t_spot in m_connectedSpotLights)
					m_connectedSpotLightsId.AddLast(t_spot.getId());
			}
		}

		public override void loadContent()
		{
			base.loadContent();
			m_connectedSpotLights = new LinkedList<SpotLight>();
			if (m_connectedSpotLightsId == null)
			{
				m_connectedSpotLightsId = new LinkedList<int>();
			}
			for (int i = 0; i < m_connectedSpotLightsId.Count(); i++)
			{
				try //TODO debug
				{
					m_connectedSpotLights.AddLast((SpotLight)Game.getInstance().getState().getObjectById(m_connectedSpotLightsId.ElementAt(i)));				
				}
				catch (InvalidCastException)
				{
					m_connectedSpotLightsId.Remove(m_connectedSpotLightsId.ElementAt(i));
				}
			}
			m_collisionShape = new CollisionRectangle(28, 25, 45 - 28, 30, m_position);
			m_switchSound = new Sound("Game//lightbutton");
		}
		public void connectSpotLight(SpotLight a_spotlight)
		{
			if (!a_spotlight.isDead() && !m_connectedSpotLights.Contains(a_spotlight))
			{
				m_connectedSpotLights.AddLast(a_spotlight);
				m_connectedSpotLightsId.AddLast(a_spotlight.getId());
			}
		}
		public void disconnectSpotLight(SpotLight a_spotlight)
		{
			m_connectedSpotLights.Remove(a_spotlight);
			m_connectedSpotLightsId.Remove(a_spotlight.getId());
		}
		public void toggleSwitch()
		{
			if (m_switchedOn || (!m_connectedToAll))
			{
				m_switchedOn = !m_switchedOn;
				m_switchSound.play();
				if (m_switchedOn)
				{
					m_img.setSprite("Images//Prop//Button//1x1_alight_switch_on");
				}
				else
				{
					m_img.setSprite("Images//Prop//Button//1x1_light_switch_off");
				}
				foreach (SpotLight t_spotLight in getConnectedSpotLights())
				{
					if (m_connectedToAll)
					{
						t_spotLight.turnOffForEver();
					}
					else
					{
						t_spotLight.toggleLight();
					}
					if (!t_spotLight.isLit())
					{
						foreach (GameObject t_guard in Game.getInstance().getState().getCurrentList())
						{
							if (t_guard is Guard && ((Guard)t_guard).canSeePoint(getCenterPoint()))
							{
								((Guard)t_guard).addLampSwitchTarget(this);
								((Guard)t_guard).getHuhSound().play();
							}
						}
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
			if (a_collid is Player && a_collid.getHitBox().collides(m_collisionShape))
			{
				if (KeyboardHandler.keyClicked(GameState.getActionKey()))
				{
					toggleSwitch();
				}
				else
				{
					((Player)(a_collid)).setInteractionVisibility(true);
				}
			}
		}

		public LinkedList<SpotLight> getConnectedSpotLights()
		{
			if (m_connectedToAll)
			{
				LinkedList<GameObject>[] t_allGO = Game.getInstance().getState().getObjectList();
				LinkedList<SpotLight> t_ret = new LinkedList<SpotLight>();
				foreach (LinkedList<GameObject> t_llgo in t_allGO)
				{
					if (t_llgo != null)
					{
						foreach (GameObject t_GO in t_llgo)
						{
							if (t_GO is SpotLight)
							{
								t_ret.AddLast((SpotLight)t_GO);
							}
						}
					}
				}
				return t_ret;
			}
			else
			{
				return m_connectedSpotLights;
			}
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

		public void toggleConnectToAll()
		{
			m_connectedToAll = !m_connectedToAll;
			if (m_connectedToAll)
			{
				foreach (SpotLight t_st in getConnectedSpotLights())
				{
					t_st.setLight(m_switchedOn);
				}
			}
		}
	}
}
