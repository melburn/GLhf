using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
				Rectangle t_playerOutBox = t_player.getHitBox().getOutBox();
				if (CollisionManager.Collides(this.getHitBox(), a_collid.getHitBox()))
				{
					
					//Colliding with ze left Wall wall
					if ((int)t_player.getLastPosition().X + 1 >= (int)getLastPosition().X + getHitBox().getOutBox().Width)
					{
						t_player.setNextPositionX(getPosition().getGlobalX() + getHitBox().getOutBox().Width);
						if (t_player.getCurrentState() == Player.State.Jumping
							&& CollisionManager.Collides(getHitBox(), t_player.getSlideBox())
							&& (GameState.isKeyPressed(Keys.Left) || GameState.isKeyPressed(Keys.Right)))
						{
							t_player.setState(Player.State.Slide);
							t_player.setFacingRight(true);
						}
						t_player.setSpeedX(0);

					}
					//Colliding with ze right Wall wall
					else if ((int)t_player.getLastPosition().X + t_playerOutBox.Width - 1 <= (int)getLastPosition().X)
					{
						t_player.setNextPositionX(getPosition().getGlobalX() - t_playerOutBox.Width);
						if (t_player.getCurrentState() == Player.State.Jumping
							&& CollisionManager.Collides(getHitBox(), t_player.getSlideBox())
							&& (GameState.isKeyPressed(Keys.Left) || GameState.isKeyPressed(Keys.Right)))
						{
							t_player.setState(Player.State.Slide);
							t_player.setFacingRight(false);
						}
						t_player.setSpeedX(0);

					}
					//Colliding with ze zeeling
					else if ((int)t_player.getLastPosition().Y >= (int)getLastPosition().Y + getHitBox().getOutBox().Height)
					{
						t_player.setNextPositionY(getPosition().getGlobalY() + getHitBox().getOutBox().Height);
						t_player.setSpeedY(0);
					}
				}
				if(GameState.checkBigBoxCollision(getHitBox().getOutBox(), t_player.getSlideBox().getOutBox()))
					t_player.setCollidedWithWall(true);
			}
		}

		public override bool isTransparent()
		{
			return false;
		}
	}
}
