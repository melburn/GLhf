using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public abstract class Consumable : NonMovingObject
	{
		protected String m_consumable;
		public Consumable(Vector2 a_position, String a_sprite, int a_layer, String a_consumable)
			:base(a_position, a_sprite, a_layer)
		{
			m_consumable = a_consumable;
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player && !m_dead && m_consumable != null)
			{
				m_dead = true;
				Game.getInstance().getProgress().increaseConsumable(m_consumable);
			}
		}
	}
}
