using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class CollisionRectangle : CollisionShape
	{
		public float m_xOffset;
		public float m_yOffset;
		public float m_width;
		public float m_height;

		public CollisionRectangle(float a_xOffset, float a_yOffset, float a_width, float a_height, Position a_position)
		{
			m_xOffset = a_xOffset;
			m_yOffset = a_yOffset;
			m_width = a_width;
			m_height = a_height;
			m_position = a_position;	
			m_OutBox = new Rectangle((int)(m_xOffset + m_position.getX()), (int)(m_yOffset + m_position.getY()), (int)m_width, (int)m_height);
		}

		public void setPosition(Vector2 a_pos)
		{
			m_xOffset = a_pos.X;
			m_yOffset = a_pos.Y;
			m_OutBox.X = (int)(m_xOffset + m_position.getX());
			m_OutBox.Y = (int)(m_yOffset + m_position.getY());
		}
		public void setSize(Vector2 a_size)
		{
			m_width = a_size.X;
			m_height = a_size.Y;
			m_OutBox.Width = (int)m_width;
			m_OutBox.Height = (int)m_height;
		}

		public override bool Collides(CollisionShape a_cs)
		{
			if (a_cs is CollisionRectangle)
			{
				//CollisionRectangle a_cr = (CollisionRectangle)a_cs;
				return true;
					
					/*(m_xOffset <= a_cr.m_xOffset + a_cr.m_width &&
					m_xOffset + m_width >= a_cr.m_xOffset &&
					m_yOffset <= a_cr.m_yOffset + a_cr.m_height &&
					m_yOffset + m_height >= a_cr.m_yOffset);*/
			}
			return false;
		}
	}
}
