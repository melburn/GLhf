using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class Wall : NonMovingObject
	{
		public Wall(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{

		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player)
			{
				Player t_player = (Player)a_collid;
				if (CollisionManager.Collides(this.getHitBox(), a_collid.getHitBox()))
				{
					
					//Colliding with ze left Wall wall
					if ((int)t_player.getLastPosition().X + 1 >= (int)getLastPosition().X + getHitBox().getOutBox().Width)
					{
						t_player.getPosition().setX(getPosition().getGlobalX() + getHitBox().getOutBox().Width);
						if (t_player.getCurrentState() == Player.State.Jumping)
						{
							t_player.setState(Player.State.Slide);
							t_player.setFacingRight(true);
						}
						t_player.setSpeedX(0);

					}
					//Colliding with ze right Wall wall
					else if ((int)t_player.getLastPosition().X + getHitBox().getOutBox().Width - 1 <= (int)getLastPosition().X)
					{
						t_player.getPosition().setX(getPosition().getGlobalX() - t_player.getHitBox().getOutBox().Width);
						if (t_player.getCurrentState() == Player.State.Jumping)
						{
							t_player.setState(Player.State.Slide);
							t_player.setFacingRight(false);
						}
						t_player.setSpeedX(0);

					}
					//Colliding with ze zeeling
					else if ((int)t_player.getLastPosition().Y >= (int)getLastPosition().Y + getHitBox().getOutBox().Height)
					{
						t_player.getPosition().setY(getPosition().getGlobalY() + getHitBox().getOutBox().Height);
						t_player.setSpeedY(0);
					}
				}
				t_player.setCollidedWithWall(true);
			}
		}
	}
}
