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
			return (getAngle(m_AOffset, m_BOffset, a_point) <= getAngle(m_AOffset, m_BOffset, m_COffset) &&
				getAngle(m_AOffset, m_COffset, a_point) <= getAngle(m_AOffset, m_COffset, m_BOffset) &&
				getAngle(m_BOffset, m_AOffset, a_point) <= getAngle(m_BOffset, m_AOffset, m_COffset)) &&
				getAngle(m_BOffset, m_COffset, a_point) <= getAngle(m_BOffset, m_COffset, m_AOffset) &&
				getAngle(m_COffset, m_AOffset, a_point) <= getAngle(m_COffset, m_AOffset, m_BOffset) &&
				getAngle(m_COffset, m_BOffset, a_point) <= getAngle(m_COffset, m_BOffset, m_AOffset);
		}
		public static double getAngle(Vector2 a_A, Vector2 a_B, Vector2 a_C)
		{
			return Math.Acos(Math.Pow((a_C - a_A).Length(),2)+Math.Pow((a_B - a_A).Length(),2)-Math.Pow((a_B - a_C).Length(),2) /
				(2 * (a_C - a_A).Length() * (a_C - a_A).Length()));
		}
	}
}
