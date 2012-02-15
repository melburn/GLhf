using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
{
	public abstract class CollisionShape
	{
		public abstract bool Collides(CollisionShape a_cs2);
	}
}
