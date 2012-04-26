using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class CollisionLine : CollisionShape
	{
		Position m_endPosition;
		public CollisionLine(Vector2 a_start, Vector2 a_end)
		{
			m_position = new CartesianCoordinate(a_start);
			m_endPosition = new CartesianCoordinate(a_end);
			m_OutBox.X = (int)Math.Floor(Math.Min(m_position.getGlobalX(), m_endPosition.getGlobalX()));
			m_OutBox.Y = (int)Math.Floor(Math.Min(m_position.getGlobalY(), m_endPosition.getGlobalY()));
			m_OutBox.Width = (int)Math.Floor(Math.Abs(Math.Max(m_position.getGlobalX(), m_endPosition.getGlobalX()) - Math.Min(m_position.getGlobalX(), m_endPosition.getGlobalX())));
			m_OutBox.Height = (int)Math.Floor(Math.Abs(Math.Max(m_position.getGlobalY(), m_endPosition.getGlobalY()) - Math.Min(m_position.getGlobalY(), m_endPosition.getGlobalY())));
		}
		public override bool collides(CollisionShape a_cs)
		{
			if (a_cs is CollisionRectangle)
			{
				return a_cs.collidesWithLineSegment(m_position.getGlobalCartesian(), m_endPosition.getGlobalCartesian());
			}
			else if (a_cs is CollisionLine)
			{
				return collidesWithLineSegment(((CollisionLine)a_cs).getPosition().getGlobalCartesian(), ((CollisionLine)a_cs).getEndPosition().getGlobalCartesian());
			}
			return false;
		}
		public override bool collidesWithLineSegment(Vector2 a_point1, Vector2 a_point2)
		{
			return false;
		}
		public Position getEndPosition()
		{
			return m_endPosition;
		}
		public override void setPosition(Position a_position)
		{
			setStartPosition(a_position.getGlobalCartesian());
		}

		public void setEndPosition(Vector2 a_pos)
		{
			m_endPosition.setGlobalCartesian(a_pos);
			m_OutBox.X = (int)Math.Floor(Math.Min(m_position.getGlobalX(), m_endPosition.getGlobalX()));
			m_OutBox.Y = (int)Math.Floor(Math.Min(m_position.getGlobalY(), m_endPosition.getGlobalY()));
			m_OutBox.Width = (int)Math.Floor(Math.Abs(Math.Max(m_position.getGlobalX(), m_endPosition.getGlobalX()) - Math.Min(m_position.getGlobalX(), m_endPosition.getGlobalX())));
			m_OutBox.Height = (int)Math.Floor(Math.Abs(Math.Max(m_position.getGlobalY(), m_endPosition.getGlobalY()) - Math.Min(m_position.getGlobalY(), m_endPosition.getGlobalY())));
		}
		public void setStartPosition(Vector2 a_pos)
		{
			m_position.setGlobalCartesian(a_pos);
			m_OutBox.X = (int)Math.Floor(Math.Min(m_position.getGlobalX(), m_endPosition.getGlobalX()));
			m_OutBox.Y = (int)Math.Floor(Math.Min(m_position.getGlobalY(), m_endPosition.getGlobalY()));
			m_OutBox.Width = (int)Math.Floor(Math.Abs(Math.Max(m_position.getGlobalX(), m_endPosition.getGlobalX()) - Math.Min(m_position.getGlobalX(), m_endPosition.getGlobalX())));
			m_OutBox.Height = (int)Math.Floor(Math.Abs(Math.Max(m_position.getGlobalY(), m_endPosition.getGlobalY()) - Math.Min(m_position.getGlobalY(), m_endPosition.getGlobalY())));
		}
	}
}
