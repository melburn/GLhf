﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class SpotLight : NonMovingObject
	{
		private bool m_lit = true;
		private int m_lightLink;
		private bool m_turnedOffForEver = false;
		[NonSerialized]
		LightCone m_light;

		public SpotLight(Vector2 a_position, string a_sprite, float a_layer, float a_rotation, bool a_lit) :
			base(a_position, a_sprite, a_layer)
		{
			m_rotate = a_rotation;
			m_lit = a_lit;
			if (a_lit)
			{
				m_light = new LightCone(this, "Images//LightCone//Ljus", a_layer + 0.001f, 300f, 300f);
				(Game.getInstance().getState()).addObject(m_light);
				m_lightLink = m_light.getId();
			}
		}

		public override void linkObject()
		{
			base.linkObject();
			if(m_light != null)
				m_lightLink = m_light.getId();
		}

		public override void loadContent() {
			base.loadContent();
			if (m_lightLink > 0)
			{
				m_light = (LightCone)Game.getInstance().getState().getObjectById(m_lightLink);
			}
			if(m_light != null)
			{
				m_light.getPosition().setParentPosition(m_position);
			}
			m_rotationPoint.Y = m_img.getSize().Y / 2;
			m_rotationPoint.X = 0;
			m_imgOffsetY = -m_rotationPoint.Y * m_YScale;
			m_collisionShape = new CollisionRectangle((float)Math.Min(Math.Min(Math.Min((m_img.getSize().Y / 2) * Math.Cos(0.5 * Math.PI + m_rotate), (m_img.getSize().Y / 2) * Math.Cos(1.5 * Math.PI + m_rotate)),
				(m_img.getSize().Y / 2) * Math.Cos(0.5 * Math.PI + m_rotate) + (m_img.getSize().X * Math.Cos(m_rotate))),
				(m_img.getSize().Y / 2) * Math.Cos(1.5 * Math.PI + m_rotate) + (m_img.getSize().X * Math.Cos(m_rotate))),
				(float)Math.Min(Math.Min(Math.Min((m_img.getSize().Y / 2) * Math.Sin(0.5 * Math.PI + m_rotate),
				(m_img.getSize().Y / 2) * Math.Sin(1.5 * Math.PI + m_rotate)),
				(m_img.getSize().Y / 2) * Math.Sin(0.5 * Math.PI + m_rotate) + (m_img.getSize().X * Math.Sin(m_rotate))),
				(m_img.getSize().Y / 2) * Math.Sin(1.5 * Math.PI + m_rotate) + (m_img.getSize().X * Math.Sin(m_rotate))),
				(float)(Math.Max(Math.Max(Math.Max((m_img.getSize().Y / 2) * Math.Cos(0.5 * Math.PI + m_rotate), (m_img.getSize().Y / 2) * Math.Cos(1.5 * Math.PI + m_rotate)),
				(m_img.getSize().Y / 2) * Math.Cos(0.5 * Math.PI + m_rotate) + (m_img.getSize().X * Math.Cos(m_rotate))),
				(m_img.getSize().Y / 2) * Math.Cos(1.5 * Math.PI + m_rotate) + (m_img.getSize().X * Math.Cos(m_rotate))) -
				Math.Min(Math.Min(Math.Min((m_img.getSize().Y / 2) * Math.Cos(0.5 * Math.PI + m_rotate), (m_img.getSize().Y / 2) * Math.Cos(1.5 * Math.PI + m_rotate)),
				(m_img.getSize().Y / 2) * Math.Cos(0.5 * Math.PI + m_rotate) + (m_img.getSize().X * Math.Cos(m_rotate))),
				(m_img.getSize().Y / 2) * Math.Cos(1.5 * Math.PI + m_rotate) + (m_img.getSize().X * Math.Cos(m_rotate)))),
				(float)(Math.Max(Math.Max(Math.Max((m_img.getSize().Y / 2) * Math.Sin(0.5 * Math.PI + m_rotate),
				(m_img.getSize().Y / 2) * Math.Sin(1.5 * Math.PI + m_rotate)),
				(m_img.getSize().Y / 2) * Math.Sin(0.5 * Math.PI + m_rotate) + (m_img.getSize().X * Math.Sin(m_rotate))),
				(m_img.getSize().Y / 2) * Math.Sin(1.5 * Math.PI + m_rotate) + (m_img.getSize().X * Math.Sin(m_rotate))) -
				(Math.Min(Math.Min(Math.Min((m_img.getSize().Y / 2) * Math.Sin(0.5 * Math.PI + m_rotate),
				(m_img.getSize().Y / 2) * Math.Sin(1.5 * Math.PI + m_rotate)),
				(m_img.getSize().Y / 2) * Math.Sin(0.5 * Math.PI + m_rotate) + (m_img.getSize().X * Math.Sin(m_rotate))),
				(m_img.getSize().Y / 2) * Math.Sin(1.5 * Math.PI + m_rotate) + (m_img.getSize().X * Math.Sin(m_rotate))))),
				m_position);
		}

		public LightCone getLightCone()
		{
			return m_light;
		}

		public bool isLit() {
			return m_lit;
		}

		public void toggleLight() {
			if (!m_turnedOffForEver)
			{
				m_lit = !m_lit;
				if (m_lit)
				{
					if (m_light == null)
					{
						m_light = new LightCone(this, "Images//LightCone//Ljus", m_layer + 0.001f, 300f, 300f);
						Game.getInstance().getState().addObject(m_light);
						m_lightLink = m_light.getId();
					}
				}
				else
				{
					Game.getInstance().getState().removeObject(m_light);
					//just in case
					m_light.kill();
					m_light = null;
					m_lightLink = 0;
				}
			}
		}

		public LightCone getLight()
		{
			return m_light;
		}

		public override void kill()
		{
			base.kill();
			if (m_lit)
			{
				m_light.kill();
				Game.getInstance().getState().removeObject(m_light);
				m_light = null;
				m_lit = false;
			}
		}
		public override void addRotation(float a_rotation)
		{
			base.addRotation(a_rotation);
			if (m_light != null)
			{
				m_light.setRotation(m_rotate);
			}
		}

		public void setLight(bool t_on)
		{
			if (m_lit != t_on)
			{
				toggleLight();
			}
		}

		public void turnOffForEver()
		{
			if (m_lit)
			{
				toggleLight();
			}
			m_turnedOffForEver = true;
		}
	}
}
