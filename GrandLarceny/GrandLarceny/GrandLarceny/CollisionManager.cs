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

		public static Stack<Entity> getCollisionsWithLineSegment(Vector2 a_point1, Vector2 a_point2)
		{
			Stack<Entity> t_ret = new Stack<Entity>();
			foreach (GameObject t_go in Game.getInstance().getState().getObjectList())
			{
				if (t_go is Entity && ((Entity)t_go).getHitBox().collidesWithLineSegment(a_point1, a_point2))
				{
					t_ret.Push((Entity)t_go);
				}
			}
			return t_ret;
		}

		public static bool possibleLineOfSight(Vector2 a_point1, Vector2 a_point2)
		{
			foreach(Entity t_entity in getCollisionsWithLineSegment(a_point1, a_point2))
			{
				if(! t_entity.isTransparent())
				{
					return false;
				}
			}
			return true;
		}
	}
}