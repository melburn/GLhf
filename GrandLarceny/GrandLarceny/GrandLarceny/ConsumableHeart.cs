using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class ConsumableHeart : Consumable
	{
		public ConsumableHeart(Vector2 a_position, String a_sprite, float a_layer)
			: base(a_position, a_sprite, a_layer)
		{

		}
		protected override void collect()
		{
			Player t_player = Game.getInstance().getState().getPlayer();
			if (t_player == null)
			{
				ErrorLogger.getInstance().writeString("Player collected a heart, but there is somehow no player, skipped");
			}
			else
			{
				t_player.heal(1);
			}
		}
	}
}
