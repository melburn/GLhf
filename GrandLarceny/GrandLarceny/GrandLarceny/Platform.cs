using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	[Serializable()]
	public class Platform : NonMovingObject
	{
		public Platform(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
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
						if (t_player.getCurrentState() == Player.State.Swinging)
						{
							t_player.setNextPosition(t_player.getLastPosition());
							t_player.setSwingSpeed(0);
						}
						t_player.setNextPositionY(getPosition().getGlobalY() - t_player.getHitBox().getOutBox().Height);
						t_player.setSpeedY(0);
						if (t_player.getCurrentState() == Player.State.Jumping || t_player.getCurrentState() == Player.State.Climbing
							|| t_player.getCurrentState() == Player.State.Hanging || t_player.getCurrentState() == Player.State.Slide
							|| t_player.getCurrentState() == Player.State.Swinging)
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
						return;
					}
					//Colliding with ze left wall
					if ((int)t_player.getLastPosition().X + 1 >= (int)getLastPosition().X + getHitBox().getOutBox().Width)
					{
						if (t_player.getCurrentState() == Player.State.Swinging)
						{
							t_player.setNextPosition(t_player.getLastPosition());
							t_player.setSwingSpeed(0);
						}
						else
						{
							t_player.setNextPositionX(getPosition().getGlobalX() + getHitBox().getOutBox().Width);
							t_player.setSpeedX(0);
							t_player.hang(this);
						}
						
					}
					//Colliding with ze right wall
					else if ((int)t_player.getLastPosition().X + t_player.getHitBox().getOutBox().Width - 1 <= (int)getLastPosition().X)
					{
						if (t_player.getCurrentState() == Player.State.Swinging)
						{
							t_player.setNextPosition(t_player.getLastPosition());
							t_player.setSwingSpeed(0);
						}
						else
						{
							t_player.setNextPositionX(getPosition().getGlobalX() - t_player.getHitBox().getOutBox().Width);
							t_player.setSpeedX(0);
							t_player.hang(this);
						}

					}
					//Colliding with ze zeeling
					else if ((int)t_player.getLastPosition().Y + ((CollisionRectangle)t_player.getHitBox()).m_yOffset >= (int)getLastPosition().Y + getHitBox().getOutBox().Height)
					{
						if (t_player.getCurrentState() == Player.State.Stop)
						{
							if (t_player.getLastPosition().X < m_position.getGlobalX())
							{
								t_player.setNextPositionX(getPosition().getGlobalX() - t_player.getHitBox().getOutBox().Width);
								t_player.setSpeedX(0);
							}
							else
							{
								t_player.setNextPositionX(getPosition().getGlobalX() + getHitBox().getOutBox().Width);
								t_player.setSpeedX(0);
							}
						}
						else if (t_player.getCurrentState() == Player.State.Swinging)
						{
							t_player.setNextPosition(t_player.getLastPosition());
							t_player.setSwingSpeed(0);
						}
						else
						{
							t_player.setNextPositionY(getPosition().getGlobalY() + getHitBox().getOutBox().Height);
							t_player.setSpeedY(0);
						}
						return;
					}
					t_player.hang(this);
				}
				else
				{
					if (t_player.getCurrentState() == Player.State.Climbing && t_player.getPosition().getGlobalY() <= m_position.getGlobalY() && KeyboardHandler.isKeyPressed(GameState.getUpKey()))
					{
						t_player.setNextPositionY(m_position.getGlobalY());
						t_player.setState(Player.State.Hanging);
						t_player.setSpeedY(0);
					}
					else if (t_player.getCurrentState() != Player.State.Hanging)
					{
						t_player.hang(this);
					}
					else if (t_player.getPosition().getGlobalY() + 10 > m_position.getGlobalY() && t_player.getCurrentState() == Player.State.Hanging && (KeyboardHandler.isKeyPressed(GameState.getUpKey()) || (KeyboardHandler.keyClicked(GameState.getLeftKey()) && !t_player.isFacingRight()) || (KeyboardHandler.keyClicked(GameState.getRightKey()) && t_player.isFacingRight())))
					{
						t_player.hangClimbAction();
					}
				}
			}
			else if (a_collider is Hookshot && m_collisionShape.collides(a_collider.getHitBox()))
			{
				((Hookshot)a_collider).changeMode(this);
			}
		}

		public override bool isTransparent()
		{
			return false;
		}
	}
}
