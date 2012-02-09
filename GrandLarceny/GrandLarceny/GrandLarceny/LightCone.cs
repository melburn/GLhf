using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public class LightCone : GameObject
	{
		private float m_length;
		private float m_width;
		private GameObject m_parent;
		public LightCone(GameObject a_parent, string a_sprite, float a_layer, float a_length, float a_width) :
			base(new PolarCoordinate(a_length/2,a_parent.getRotation()), a_sprite, a_layer)
		{
			m_position.setParentPosition(a_parent.getPosition());
			m_parent = a_parent;
		}
		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			if (m_parent.isDead())
			{
				m_dead = true;
			}
			else
			{
				m_rotate = m_parent.getRotation();
				m_position.setSlope(m_rotate);
			}
		}
	}
}
