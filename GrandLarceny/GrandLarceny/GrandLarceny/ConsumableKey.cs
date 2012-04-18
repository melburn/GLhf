using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public class ConsumableKey : NonMovingObject
	{
		public ConsumableKey(Vector2 a_position, String a_sprite, int a_layer)
			:base(a_position, a_sprite, a_layer)
		{

		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player && !m_dead)
			{
				m_dead = true;
				Game.getInstance().getProgress().increaseConsumable("key");
			}
		}
	}
}
