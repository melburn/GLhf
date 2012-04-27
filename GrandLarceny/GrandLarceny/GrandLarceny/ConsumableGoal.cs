using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class ConsumableGoal : Consumable
	{
		public ConsumableGoal(Vector2 a_position, String a_sprite, float a_layer)
			:base(a_position, a_sprite, a_layer)
		{}

		protected override bool collect()
		{
			if (Game.keyClicked(GameState.getActionKey()))
			{
				return true;
			}
			else
			{
				Game.getInstance().getState().getPlayer().setInteractionVisibillity(true);
				return false;
			}
		}
	}
}
