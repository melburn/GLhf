using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class CollisionTriangle : CollisionShape
	{
		public Vector2 m_AOffset;
		public Vector2 m_BOffset;
		public Vector2 m_COffset;

		public CollisionTriangle(Vector2[] a_points, Position a_position)
		{
			if (a_points == null)
			{
				throw new ArgumentNullException("points in triangle cannot be null");
			}
			if (a_position == null)
			{
				throw new ArgumentNullException("position for triangle cannot be null");
			}
			if (a_points.Length != 3)
			{
				throw new ArgumentException("triangle needs exactly 3 points, got " + a_points.Length);
			}
			m_AOffset = a_points[0];
			m_BOffset = a_points[1];
			m_COffset = a_points[2];
			m_position = a_position;
		}
		public override bool Collides(CollisionShape a_cs)
		{
			if (a_cs is CollisionRectangle)
			{
				CollisionRectangle a_cr = (CollisionRectangle)a_cs;
				foreach(Vector2 v in a_cr.getFivePoints())
				{
					if (contains(v))
					{
						return true;
					}
				}
				foreach(Vector2 v in getFourPoints())
				{
					if (a_cr.contains(v))
					{
						return true;
					}
				}
			}
			return false;
		}

		public Vector2[] getFourPoints()
		{
			Vector2[] t_ret = new Vector2[4];
			t_ret[3] = m_AOffset + m_position.getGlobalCartesianCoordinates();
			t_ret[1] = m_BOffset + m_position.getGlobalCartesianCoordinates();
			t_ret[2] = m_COffset + m_position.getGlobalCartesianCoordinates();
			t_ret[0] = new Vector2((m_AOffset.X + m_BOffset.X + m_COffset.X) / 3 + m_position.getGlobalX(), (m_AOffset.Y + m_BOffset.Y + m_COffset.Y) / 3 + m_position.getGlobalY());
			return t_ret;
		}
		public bool contains(Vector2 a_point)
		{
			Vector2 t_pointA = m_AOffset + m_position.getGlobalCartesianCoordinates();
			Vector2 t_pointB = m_BOffset + m_position.getGlobalCartesianCoordinates();
			Vector2 t_pointC = m_COffset + m_position.getGlobalCartesianCoordinates();
			return (getAngle(t_pointA, t_pointB, a_point) <= getAngle(t_pointA, t_pointB, t_pointC) &&
				getAngle(t_pointA, t_pointC, a_point) <= getAngle(t_pointA, t_pointC, t_pointB) &&
				getAngle(t_pointB, t_pointA, a_point) <= getAngle(t_pointB, t_pointA, t_pointC)) &&
				getAngle(t_pointB, t_pointC, a_point) <= getAngle(t_pointB, t_pointC, t_pointA) &&
				getAngle(t_pointC, t_pointA, a_point) <= getAngle(t_pointC, t_pointA, t_pointB) &&
				getAngle(t_pointC, t_pointB, a_point) <= getAngle(t_pointC, t_pointB, t_pointA);
		}
		public static double getAngle(Vector2 a_A, Vector2 a_B, Vector2 a_C)
		{
			return Math.Acos(Math.Pow((a_C - a_A).Length(),2)+Math.Pow((a_B - a_A).Length(),2)-Math.Pow((a_B - a_C).Length(),2) /
				(2 * (a_C - a_A).Length() * (a_C - a_A).Length()));
		}
	}
}
