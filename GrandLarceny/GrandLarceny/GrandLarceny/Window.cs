using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	public class Window : NonMovingObject
	{
		private float m_playerOn;
		private bool m_open;

		public Window(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			m_open = true;
		}

		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			if (m_playerOn > 0)
			{
				m_playerOn -= ((float)a_gameTime.ElapsedGameTime.Milliseconds) / 1000f;
				if (m_playerOn <= 0)
				{
					Game.getInstance().getState().getPlayer().deactivateChaseMode();
				}
			}
		}

		public override bool isTransparent()
		{
			return false;
		}

		public void toggleOpen()
		{
			m_open = !m_open;
		}

		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				if (CollisionManager.Collides(this.getHitBox(), a_collider.getHitBox()))
				{
					//Colliding with ze floor
					/*
					if ((int)t_player.getLastPosition().Y + t_player.getHitBox().getOutBox().Height <= (int)getLastPosition().Y && t_player.getCurrentState() != Player.State.Hanging)
					{
						if (t_player.getCurrentState() == Player.State.Swinging)
						{
							t_player.setNextPosition(t_player.getLastPosition());
							t_player.setSwingSpeed(0);
						}
						m_playerOn = 0.2f;
						t_player.setNextPositionY(getPosition().getGlobalY() - t_player.getHitBox().getOutBox().Height);
						t_player.setSpeedY(0);
						if (t_player.getCurrentState() == Player.State.Jumping || t_player.getCurrentState() == Player.State.Climbing
							|| t_player.getCurrentState() == Player.State.Slide || t_player.getCurrentState() == Player.State.Swinging)
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
					*/
					//Colliding with ze zeeling
					if ((int)t_player.getLastPosition().Y + ((CollisionRectangle)t_player.getHitBox()).m_yOffset >= (int)getLastPosition().Y + getHitBox().getOutBox().Height)
					{
						if (t_player.getCurrentState() == Player.State.Swinging)
						{
							t_player.setNextPosition(t_player.getLastPosition());
							t_player.setSwingSpeed(0);
						}
						else
						{
							t_player.setNextPositionY(getPosition().getGlobalY() + getHitBox().getOutBox().Height);
							t_player.setSpeedY(0);
							return;
						}
					}
					//Colliding with ze left wall
					else if (t_player.getPosition().getGlobalX() + (t_player.getHitBox().getOutBox().Width/2) > m_position.getGlobalX() + (m_collisionShape.getOutBox().Width/2))
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
					else if (t_player.getPosition().getGlobalX() + (t_player.getHitBox().getOutBox().Width / 2) < m_position.getGlobalX() + (m_collisionShape.getOutBox().Width / 2))
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
				}
				else
				{	
					if (t_player.getCurrentState() == Player.State.Climbing && t_player.getPosition().getGlobalY() <= m_position.getGlobalY() && Game.keyClicked(GameState.getUpKey()))
					{
						t_player.setNextPositionY(m_position.getGlobalY());	
						t_player.setState(Player.State.Hanging);
						t_player.setSpeedY(0);
					}
					else if(t_player.getCurrentState() != Player.State.Hanging)
					{
						t_player.hang(this);
					}
				}
				if(m_open && !t_player.isStunned() && (t_player.getCurrentState() == Player.State.Hanging
					|| t_player.getCurrentState() == Player.State.Stop || t_player.getCurrentState() == Player.State.Walking))
				{
					if (t_player.getCurrentState() == Player.State.Hanging && t_player.getLastState() == Player.State.Hanging)
					{
						if (Game.isKeyPressed(GameState.getActionKey()))
						{
							t_player.windowAction();
						}
						else
						{
							t_player.setInteractionVisibility(true);
						}
							
					}
					else if (t_player.getPosition().getGlobalY() < m_position.getGlobalY() && t_player.getCurrentState() == Player.State.Walking
						&& ((t_player.getPosition().getGlobalX() < m_position.getGlobalX() && t_player.isFacingRight()) || (t_player.getPosition().getGlobalX() > m_position.getGlobalX() && !t_player.isFacingRight())))
					{
						t_player.windowAction();
					}
				}
			}
		}

		public override string ToString() {
			if (m_open)
				return base.ToString() + ": Open";
			else
				return base.ToString() + ": Closed";
		}
	}
}