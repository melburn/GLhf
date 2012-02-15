using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class CollisionRectangle : CollisionShape
	{
		public float m_x;
		public float m_y;
		public float m_width;
		public float m_height;
		
		public CollisionRectangle(float a_x, float a_y, float a_width, float a_height)
		{
			m_x = a_x;
			m_y = a_y;
			m_width = a_width;
			m_height = a_height;
		}
		public void setPosition(Vector2 a_pos)
		{
			m_x = a_pos.X;
			m_y = a_pos.Y;
		}
		public void setSize(Vector2 a_size)
		{
			m_width = a_size.X;
			m_height = a_size.Y;
		}

		public override bool Collides(CollisionShape a_cs)
		{
			if (a_cs is CollisionRectangle)
			{
				CollisionRectangle a_cr = (CollisionRectangle)a_cs;
				return (m_x <= a_cr.m_x + a_cr.m_width &&
					m_x + m_width >= a_cr.m_x &&
					m_y <= a_cr.m_y + a_cr.m_height &&
					m_y + m_height >= a_cr.m_y);
			}
			return true;
		}
		public bool contains(Vector2 a_point)
		{
			return a_point.X <= m_x + m_width &&
				a_point.X >= m_x &&
				a_point.Y <= m_y + m_height &&
				a_point.Y >= m_y;
		}
	}
}
