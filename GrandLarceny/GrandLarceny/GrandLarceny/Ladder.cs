using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	[Serializable()]
	class Ladder : NonMovingObject
	{
		public Ladder(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player)
			{
				if (CollisionManager.Collides(this.getHitBox(), a_collid.getHitBox()))
				{
					Player t_player = (Player)a_collid;
					//Colliding with ze ladd0rz
					Rectangle t_rect = new Rectangle(getHitBox().getOutBox().X - 2,
						(int)getPosition().getGlobalY(), 4, getHitBox().getOutBox().Height);
					if (t_rect.Contains((int)t_player.getLastPosition().X, (int)t_player.getLastPosition().Y))
					{
						if (Keyboard.GetState().IsKeyDown(Keys.Up))
						{
							t_player.setSpeedX(0);
							t_player.setState(Player.State.Climbing);
							if (t_player.getSpeed().Y < -Player.CLIMBINGSPEED || t_player.getSpeed().Y > Player.CLIMBINGSPEED)
								t_player.setSpeedY(0);
							t_player.getPosition().setX(getPosition().getGlobalX());
						}

					}
				}
			}

		}
	}
}
