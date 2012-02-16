using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	public class LightCone : Entity
	{
		private float m_length;
		private float m_width;
		private GameObject m_parent;
		public LightCone(GameObject a_parent, string a_sprite, float a_layer, float a_length, float a_width) :
			base(new PolarCoordinate(a_width/2,(float)(Math.PI/2+a_parent.getRotation()),a_parent.getPosition()), a_sprite, a_layer)
		{
			if (a_length <= 0)
			{
				throw new ArgumentException("length has to be positive. was "+a_length);
			}
			if (a_width <= 0)
			{
				throw new ArgumentException("width has to be positive. was "+a_width);
			}
			m_parent = a_parent;
			m_length = a_length;
			m_width = a_width;
			m_XScale = a_width / 500;
			m_YScale = a_length / 500;
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
				m_position.setSlope((float)(Math.PI / 2 + m_rotate));
			}
		}
		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionTriangle(getTrianglePointsOffset(), m_position);
		}

		private Vector2[] getTrianglePointsOffset()
		{
			Vector2[] t_ret = new Vector2[3];
			t_ret[0] = new Vector2((float)((m_width / 2) * Math.Cos(m_rotate + 1.5 * Math.PI)), (float)((m_width / 2) * Math.Sin(m_rotate + 1.5 * Math.PI)));
			t_ret[1] = new Vector2((float)(m_length*Math.Cos(m_rotate)),(float)(m_length*Math.Sin(m_rotate)));
			t_ret[2] = new Vector2((float)((m_length*Math.Cos(m_rotate))+((m_width/2)*Math.Cos(1.5*Math.PI+m_rotate))),(float)((m_length*Math.Sin(m_rotate))+((m_width/2)*Math.Sin(1.5*Math.PI+m_rotate))));
			return t_ret;
		}
		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			//I don't care 'bout your collisions, I'm freakin' nonsolid!
		}
	}
}
