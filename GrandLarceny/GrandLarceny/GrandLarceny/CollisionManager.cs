using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class CollisionManager
	{
		private CollisionManager()
		{
		} //lol statisk

		public static bool Collides(CollisionShape a_cs1, CollisionShape a_cs2)
		{
			return a_cs1.Collides(a_cs2);
		}

		public static bool Collides(CollisionShape a_cs1, Vector2 a_cs2)
		{
			return a_cs1.contains(a_cs2);
		}
	}
}