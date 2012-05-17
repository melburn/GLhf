using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	public class ConsumableGoal : Consumable
	{

		public ConsumableGoal(Vector2 a_position, String a_sprite, float a_layer)
			:base(a_position, a_sprite, a_layer)
		{

		}

		protected override bool collect()
		{
			int t_num = 4 - ((GameState)Game.getInstance().getState()).numberOfGoals();
			if (t_num > 0 && t_num < 4)
			{
				String t_textureName = "Images//GUI//GameGUI//stolen"+t_num+"of3";
				Particle t_stolenGoods = new Particle(new CartesianCoordinate(Vector2.Zero, Game.getInstance().m_camera.getPosition()), t_textureName, 33, 0.0015f);
				t_stolenGoods.getPosition().setLocalCartesian(-t_stolenGoods.getImg().getSize() / 2);
				t_stolenGoods.setTimer(((float)Game.getInstance().getTotalGameTime().TotalMilliseconds) + 3000f);
				Game.getInstance().getState().addObject(t_stolenGoods);
			}
			return true;
		}
	}
}
