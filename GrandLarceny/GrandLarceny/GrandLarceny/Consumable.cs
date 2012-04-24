using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public abstract class Consumable : NonMovingObject
	{
		public Consumable(Vector2 a_position, String a_sprite, float a_layer)
			:base(a_position, a_sprite, a_layer)
		{
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player && !m_dead)
			{
				collect();
				m_dead = true;
			}
		}

		abstract protected void collect();
	}
}
