using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public abstract class CollisionShape
	{
		protected Rectangle m_OutBox;
		protected Position m_position;

		public abstract bool Collides(CollisionShape a_cs2);
		public virtual bool contains(Vector2 a_cs2)
		{
			return false;
		}
		public abstract bool collidesWithLineSegment(Vector2 a_point1, Vector2 a_point2);
		
		public virtual Rectangle getOutBox()
		{
			return m_OutBox;
		}
		public virtual Position getPosition()
		{
			return m_position;
		}
	}
}
