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
			OutBox = new Rectangle((int)m_x, (int)m_y, (int)m_width, (int)m_height);
		}
		public void setPosition(Vector2 a_pos)
		{
			m_x = a_pos.X;
			m_y = a_pos.Y;
			OutBox.X = (int)m_x;
			OutBox.Y = (int)m_y;
		}
		public void setSize(Vector2 a_size)
		{
			m_width = a_size.X;
			m_height = a_size.Y;
			OutBox.Width = (int)m_width;
			OutBox.Height = (int)m_height;
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
	}
}
