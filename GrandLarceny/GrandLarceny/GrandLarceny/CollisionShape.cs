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
		
		public Rectangle getOutBox()
		{
			return m_OutBox;
		}
	}
}
