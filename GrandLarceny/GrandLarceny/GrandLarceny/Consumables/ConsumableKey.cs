using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class ConsumableKey : Consumable
	{
		public ConsumableKey(Vector2 a_position, String a_sprite, float a_layer)
			: base(a_position, a_sprite, a_layer)
		{

		}

		public override void loadContent()
		{
			base.loadContent();
			if (m_img.getImagePath() == null)
			{
				m_img = new ImageManager("Images//Tile//1x1_tile_ph");
			}
		}

		protected override Boolean collect()
		{
			Game.getInstance().getProgress().increaseConsumable("key");
			return true;
		}
	}
}
