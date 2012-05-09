using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public class LockedDoor : NonMovingObject
	{
		public LockedDoor(Vector2 a_position, String a_sprite, float a_layer)
			:base (a_position, a_sprite, a_layer)
		{
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player && !m_dead)
			{
				if (Game.getInstance().getProgress().decreaseConsumable("key"))
				{
					m_dead = true;
				}
				else
				{
					Player t_player = (Player)a_collid;
					Rectangle t_playerOutBox = t_player.getHitBox().getOutBox();

					//Colliding with ze left Wall wall
					if ((int)t_player.getLastPosition().X + 1 >= (int)getLastPosition().X + getHitBox().getOutBox().Width)
					{
						t_player.setNextPositionX(getPosition().getGlobalX() + getHitBox().getOutBox().Width);
						t_player.setSpeedX(0);

					}
					//Colliding with ze right Wall wall
					else if ((int)t_player.getLastPosition().X + t_playerOutBox.Width - 1 <= (int)getLastPosition().X)
					{
						t_player.setNextPositionX(getPosition().getGlobalX() - t_playerOutBox.Width);
						t_player.setSpeedX(0);
					}
				}
			}
		}
	}
}
