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
			m_OutBox = new Rectangle((int)(m_xOffset + m_position.getGlobalX()), (int)(m_yOffset + m_position.getGlobalY()), (int)m_width, (int)m_height);
		}

		public void setPosition(Vector2 a_pos)
		{
			m_xOffset = a_pos.X;
			m_yOffset = a_pos.Y;
			m_OutBox.X = (int)(m_xOffset + m_position.getGlobalX());
			m_OutBox.Y = (int)(m_yOffset + m_position.getGlobalY());
		}
		public void setSize(Vector2 a_size)
		{
			m_width = a_size.X;
			m_height = a_size.Y;
			m_OutBox.Width = (int)m_width;
			m_OutBox.Height = (int)m_height;
		}

		public override Rectangle getOutBox()
		{
			m_OutBox.X = (int)(Math.Floor(m_xOffset + m_position.getGlobalX()));
			m_OutBox.Y = (int)(Math.Floor(m_yOffset + m_position.getGlobalY()));
			return m_OutBox;
		}

		public override bool Collides(CollisionShape a_cs)
		{
			if (a_cs is CollisionRectangle)
			{
				CollisionRectangle t_cr = (CollisionRectangle)a_cs;

				return (m_xOffset + m_position.getGlobalX() < t_cr.m_xOffset + t_cr.m_position.getGlobalX() + t_cr.m_width &&
					m_xOffset + m_position.getGlobalX() + m_width > t_cr.m_xOffset + t_cr.m_position.getGlobalX() &&
					m_yOffset + m_position.getGlobalY() < t_cr.m_yOffset + t_cr.m_position.getGlobalY() + t_cr.m_height &&
					m_yOffset + m_position.getGlobalY() + m_height > t_cr.m_yOffset + t_cr.m_position.getGlobalY());
			}
			else if (a_cs is CollisionTriangle)
			{
				return a_cs.Collides(this);
			}
			return false;
		}

		public Vector2[] getFivePoints()
		{
			Vector2[] t_ret = new Vector2[5];
			t_ret[4] = new Vector2(m_xOffset + m_position.getGlobalX(), m_yOffset + m_position.getGlobalY());
			t_ret[1] = new Vector2(m_xOffset + m_width + m_position.getGlobalX(), m_yOffset + m_position.getGlobalY());
			t_ret[2] = new Vector2(m_xOffset + m_position.getGlobalX(), m_yOffset + m_height + m_position.getGlobalY());
			t_ret[3] = new Vector2(m_xOffset + m_width + m_position.getGlobalX(), m_yOffset + m_height + m_position.getGlobalY());
			t_ret[0] = new Vector2(m_xOffset + (m_width / 2) + m_position.getGlobalX(), m_yOffset + (m_height / 2) + m_position.getGlobalY());
			return t_ret;
		}

		public override bool contains(Vector2 v)
		{
			return v.X <= m_xOffset + m_width + m_position.getGlobalX() &&
				v.X >= m_xOffset + m_position.getGlobalX() &&
				v.Y <= m_yOffset + m_height + m_position.getGlobalY() &&
				v.Y >= m_yOffset + m_position.getGlobalY();
		}

		public override bool collidesWithLineSegment(Vector2 a_point1, Vector2 a_point2)
		{
			// Find min and max X for the segment

			double minX = Math.Min(a_point1.X,a_point2.X);
			double maxX = Math.Max(a_point1.X,a_point2.X);

			if(a_point1.X > a_point2.X)
			{
			  minX = a_point2.X;
			  maxX = a_point1.X;
			}

			// Find the intersection of the segment's and rectangle's x-projections

			maxX = Math.Min(maxX, m_position.getGlobalX() + m_xOffset + m_width);

			minX = Math.Max(minX, m_position.getGlobalX() + m_xOffset);


			if(minX > maxX) // If their projections do not intersect return false
			{
			  return false;
			}

			// Find corresponding min and max Y for min and max X we found before
			
			double minY = a_point1.Y;
			double maxY = a_point2.Y;

			double dx = a_point2.X - a_point1.X;

			if(Math.Abs(dx) > 0.0000001)
			{
			  double a = (a_point2.Y - a_point1.Y) / dx;
			  double b = a_point1.Y - a * a_point1.X;
			  minY = a * minX + b;
			  maxY = a * maxX + b;
			}

			if(minY > maxY)
			{
			  double tmp = maxY;
			  maxY = minY;
			  minY = tmp;
			}

			// Find the intersection of the segment's and rectangle's y-projections

			if(maxY > m_position.getGlobalY() + m_yOffset + m_height)
			{
			  maxY = m_position.getGlobalY() + m_yOffset + m_height;
			}

			if (minY < m_position.getGlobalY() + m_yOffset)
			{
				minY = m_position.getGlobalY() + m_yOffset;
			}

			if(minY > maxY) // If Y-projections do not intersect return false
			{
			  return false;
			}

			return true;
		  }
		public override Position getPosition()
		{
			return new CartesianCoordinate(new Vector2(m_position.getGlobalX() + m_xOffset, m_position.getGlobalY() + m_yOffset));
		}

	}
}
