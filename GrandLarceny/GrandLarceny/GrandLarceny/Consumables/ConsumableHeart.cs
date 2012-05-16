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

		public override void loadContent()
		{
			base.loadContent();
			if (m_img.getImagePath() == null)
			{
				m_img = new ImageManager("Images//Props//Consumables//shinyheart");
			}
		}

		protected override Boolean collect()
		{
			Player t_player = Game.getInstance().getState().getPlayer();
			t_player.heal(1);
			return true;
		}
	}
}
