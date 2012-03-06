using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	[Serializable()]
	class Platform : NonMovingObject
	{
		public Platform(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			
		}

		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				if (CollisionManager.Collides(this.getHitBox(), a_collider.getHitBox()))
				{
					//Colliding with ze floor
					if ((int)t_player.getLastPosition().Y + t_player.getHitBox().getOutBox().Height <= (int)getLastPosition().Y)
					{
						t_player.setNextPositionY(getPosition().getGlobalY() - t_player.getCollisionShape().getOutBox().Height);
						t_player.setSpeedY(0);
						if (t_player.getCurrentState() == Player.State.Jumping || t_player.getCurrentState() == Player.State.Climbing)
						{
							if (t_player.getSpeed().X == 0)
							{
								t_player.setState(Player.State.Stop);
							}
							else
							{
								t_player.setState(Player.State.Walking);
							}
						}
						else if (t_player.getCurrentState() == Player.State.Damaged)
						{
							if (t_player.getHorizontalSpeed() > 0)
							{
								t_player.setSpeedX(Math.Max(t_player.getHorizontalSpeed() - 1,0));
							}
							else
							{
								t_player.setSpeedX(Math.Min(t_player.getSpeed().X,0));
							}
						}
						return;
					}

					//Colliding with ze zeeling
					else if ((int)t_player.getLastPosition().Y + ((CollisionRectangle)t_player.getHitBox()).m_yOffset >= (int)getLastPosition().Y + getHitBox().getOutBox().Height)
					{
						t_player.setNextPositionY(getPosition().getGlobalY() + getHitBox().getOutBox().Height);
						t_player.setSpeedY(0);
						return;
					}
					//Colliding with ze left wall
					if ((int)t_player.getLastPosition().X + 1 >= (int)getLastPosition().X + getHitBox().getOutBox().Width)
					{
						t_player.setNextPositionX(getPosition().getGlobalX() + getHitBox().getOutBox().Width);
						t_player.setSpeedX(0);
						t_player.hang(this);
						
					}
					//Colliding with ze right wall
					if ((int)t_player.getLastPosition().X + t_player.getHitBox().getOutBox().Width - 1 <= (int)getLastPosition().X)
					{
						t_player.setNextPositionX(getPosition().getGlobalX() - t_player.getHitBox().getOutBox().Width);
						t_player.setSpeedX(0);
						t_player.hang(this);
					}
					t_player.hang(this);
				}
				else
				{
					if (t_player.getCurrentState() == Player.State.Climbing && t_player.getPosition().getGlobalY() <= m_position.getGlobalY() && GameState.m_currentKeyInput.IsKeyDown(Keys.Up))
					{
						t_player.setNextPositionY(m_position.getGlobalY());
						t_player.setState(Player.State.Hanging);
						t_player.setSpeedY(0);
					}
					else
					t_player.hang(this);
				}
			}
		}

		public override bool isTransparent()
		{
			return false;
		}
	}
}
