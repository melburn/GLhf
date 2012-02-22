using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	public class LightCone : MovingObject
	{
		private float m_length;
		private float m_width;
		private GameObject m_parent;
		public LightCone(GameObject a_parent, string a_sprite, float a_layer, float a_length, float a_width) :
			base(new PolarCoordinate(a_parent.getImg().getSize().Y/2,(float)(1.5f*Math.PI+a_parent.getRotation()),a_parent.getPosition()), a_sprite, a_layer)
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
			m_XScale = a_length / 500;
			m_YScale = a_width / 500;
			m_rotate = a_parent.getRotation();
		}
		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			if (m_parent == null || m_parent.isDead())
			{
				m_dead = true;
			}
		}
		public void setRotation(float a_rotation)
		{
			if (m_rotate != a_rotation)
			{
				m_rotate = a_rotation;
				m_position.setSlope((float)(1.5f * Math.PI + m_rotate));
				m_collisionShape = new CollisionTriangle(getTrianglePointsOffset(), m_position);
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
			t_ret[0] = new Vector2((float)((m_width / 2) * Math.Cos(0.5 * Math.PI + m_rotate)), (float)((m_width / 2) * Math.Sin(0.5 * Math.PI + m_rotate)));
			t_ret[1] = new Vector2((float)(m_length*Math.Cos(m_rotate)),(float)(m_length*Math.Sin(m_rotate)));
			t_ret[2] = t_ret[0] + new Vector2((float)((m_length*Math.Cos(m_rotate))+((m_width/2)*Math.Cos(0.5*Math.PI+m_rotate))),(float)((m_length*Math.Sin(m_rotate))+((m_width/2)*Math.Sin(0.5*Math.PI+m_rotate))));
			return t_ret;
		}


		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player)
			{

				Player t_player = (Player)a_collid;
				if (t_player.getCurrentState() != Player.State.Hiding)
				{
					t_player.setIsInLight(true);
				}
			}

		}
	}
}
