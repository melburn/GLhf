using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public abstract class CollisionShape
	{
		protected Rectangle OutBox;

		public abstract bool Collides(CollisionShape a_cs2);

		public Rectangle getOutBox()
		{
			return OutBox;
		}
	}
}
