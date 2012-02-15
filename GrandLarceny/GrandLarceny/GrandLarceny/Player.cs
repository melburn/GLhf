using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	[Serializable()]
	public class Player : Entity
	{
		private Vector2 m_cameraPoint = new Vector2(0,0);

		private const float CAMERASPEED = 0.1f;

		private const int CLIMBINGSPEED = 200;
		private const int PLAYERSPEED = 200;
		private const int JUMPSTRENGTH = 600;
		private const int CAMERAMAXDISTANCE = 100;
		private const int ACCELERATION = 2000;
		private const int DEACCELERATION = 800;
		private const int AIRDEACCELERATION = 300;
		private const int SLIDESPEED = 25;

		private float m_rollTimer;

		[NonSerialized]
		private KeyboardState m_currentKeyInput;
		[NonSerialized]
		private KeyboardState m_previousKeyInput;
		private State m_currentState = State.Stop;

		private bool m_facingRight = false;

		enum State
		{
			Stop,
			Walking,
			Jumping,
			Slide,
			Climbing, 
			Rolling
		}

		public Player(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			m_currentState = State.Jumping;
		}

		public override void update(GameTime a_gameTime)
		{
			m_gravity = 1000f;
			m_previousKeyInput = m_currentKeyInput;
			m_currentKeyInput = Keyboard.GetState();
			float t_deltaTime = ((float) a_gameTime.ElapsedGameTime.Milliseconds) / 1000f;
			switch (m_currentState)
			{
				case State.Stop:
				{
					updateStop(t_deltaTime);
					break;
				}
				case State.Walking:
				{
					updateWalking(t_deltaTime);
					break;
				}
				case State.Jumping:
				{
					updateJumping(t_deltaTime);
					break;
				}
				case State.Slide:
				{
					updateSliding(t_deltaTime);
					break;
				}
				case State.Climbing:
				{
					updateClimbing();
					break;
				}
				case State.Rolling:
				{
					updateRolling();
					break;
				}
			}
			m_previousKeyInput = m_currentKeyInput;
			changeAnimation();
			flipSprite();
			base.update(a_gameTime);
			Game.getInstance().m_camera.getPosition().smoothStep(m_cameraPoint, CAMERASPEED);
		}

		private void flipSprite()
		{
			if (m_facingRight)
			{
				m_spriteEffects = SpriteEffects.None;
			}
			else
			{
				m_spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}

		private void updateStop(float a_deltaTime)
		{
			if (m_previousKeyInput.IsKeyUp(Keys.Down) && m_currentKeyInput.IsKeyDown(Keys.Down))
			{
				m_currentState = State.Rolling;
				m_rollTimer = a_deltaTime + 15;
				return;
			}
			if (m_currentKeyInput.IsKeyDown(Keys.Left) || m_currentKeyInput.IsKeyDown(Keys.Right))
			{
				m_currentState = State.Walking;
				
				if (m_currentKeyInput.IsKeyDown(Keys.Left))
				{
					m_facingRight = false;
				}
				else
				{
					m_facingRight = true;
				}
			}
			if (m_previousKeyInput.IsKeyUp(Keys.Space) && m_currentKeyInput.IsKeyDown(Keys.Space))
			{
				m_speed.Y -= JUMPSTRENGTH;
				m_currentState = State.Jumping;
				
			}

			
		}

		private void updateWalking(float a_deltaTime)
		{
			if (m_previousKeyInput.IsKeyUp(Keys.Down) && m_currentKeyInput.IsKeyDown(Keys.Down))
			{

				m_currentState = State.Rolling;
				m_rollTimer = a_deltaTime + 15;
				return;
			}

			if (m_currentKeyInput.IsKeyDown(Keys.Right))
			{
				if (m_speed.X > PLAYERSPEED)
				{
					m_speed.X = m_speed.X - (DEACCELERATION * a_deltaTime);
				}
				else
				{
					m_speed.X = Math.Min(m_speed.X + (ACCELERATION * a_deltaTime), PLAYERSPEED);
				}
			}
			if (m_currentKeyInput.IsKeyDown(Keys.Left))
			{
				if (m_speed.X < -PLAYERSPEED)
				{
					m_speed.X = m_speed.X + (DEACCELERATION * a_deltaTime);
				}
				else
				{
					m_speed.X = Math.Max(m_speed.X - (ACCELERATION * a_deltaTime), -PLAYERSPEED);
				}
			}
			if (m_speed.X > 0)
			{
				m_speed.X = Math.Max(m_speed.X - (DEACCELERATION * a_deltaTime), 0);
				m_facingRight = true;
			}
			else if (m_speed.X < 0)
			{
				m_speed.X = Math.Min(m_speed.X + (DEACCELERATION * a_deltaTime), 0);
				m_facingRight = false;
			}
			if (m_speed.X == 0)
			{
				m_currentState = State.Stop;
				
			}
			if (m_previousKeyInput.IsKeyUp(Keys.Space) && m_currentKeyInput.IsKeyDown(Keys.Space))
			{
				m_speed.Y -= JUMPSTRENGTH;
				m_currentState = State.Jumping;
				
			}

			m_cameraPoint.X = Math.Max(Math.Min(m_cameraPoint.X + (m_speed.X * 1.5f * a_deltaTime), CAMERAMAXDISTANCE), -CAMERAMAXDISTANCE);
		
			m_img.setAnimationSpeed(Math.Abs(m_speed.X / 10f));
			if (m_position.getY() != getLastPosition().Y)
			{
				m_currentState = State.Jumping;
			}		
		}

		private void updateJumping(float a_deltaTime)
		{
			if (m_currentKeyInput.IsKeyUp(Keys.Left) && m_currentKeyInput.IsKeyUp(Keys.Right))
			{
				if (m_speed.X > 0)
					m_speed.X = Math.Max(m_speed.X - (AIRDEACCELERATION * a_deltaTime), 0);
				else if (m_speed.X < 0)
					m_speed.X = Math.Min(m_speed.X + (AIRDEACCELERATION * a_deltaTime), 0);
			}
			else if (m_currentKeyInput.IsKeyDown(Keys.Left))
			{
				if (m_speed.X < -PLAYERSPEED)
				{
					m_speed.X += AIRDEACCELERATION * a_deltaTime;
				}
				else
				{
					m_speed.X = Math.Max(-PLAYERSPEED, m_speed.X - AIRDEACCELERATION * a_deltaTime);
				}
				m_facingRight = false;
			}
			else if (m_currentKeyInput.IsKeyDown(Keys.Right))
			{
				if (m_speed.X > PLAYERSPEED)
				{
					m_speed.X -= AIRDEACCELERATION * a_deltaTime;
				}
				else
				{
					m_speed.X = Math.Min(PLAYERSPEED, m_speed.X + AIRDEACCELERATION * a_deltaTime);
				}
				m_facingRight = true;
			}
			m_cameraPoint.X = Math.Max(Math.Min(m_cameraPoint.X + (m_speed.X * 1.5f * a_deltaTime), CAMERAMAXDISTANCE), -CAMERAMAXDISTANCE);
		}

		private void updateSliding(float a_deltaTime)
		{
			if (m_lastPosition.Y != m_position.getY())
			{
				if ((!m_facingRight && m_currentKeyInput.IsKeyDown(Keys.Right)) || (m_facingRight && m_currentKeyInput.IsKeyDown(Keys.Left)))
				{
					if (m_previousKeyInput.IsKeyUp(Keys.Space) && m_currentKeyInput.IsKeyDown(Keys.Space))
					{
						m_speed.Y = -JUMPSTRENGTH;
						if (m_facingRight == true)
							m_speed.X += JUMPSTRENGTH;
						else
							m_speed.X -= JUMPSTRENGTH;
						m_currentState = State.Jumping;
						return;
					}
					if (m_speed.Y > SLIDESPEED)
						m_speed.Y = SLIDESPEED;
					return;
				}
				m_currentState = State.Jumping;
				return;
			}
			m_currentState = State.Walking;
		}

		private void updateClimbing()
		{
			if (m_currentKeyInput.IsKeyDown(Keys.Up))
			{
				m_speed.Y = -CLIMBINGSPEED;
			}
			else if (m_currentKeyInput.IsKeyDown(Keys.Down))
			{
				m_speed.Y = CLIMBINGSPEED;
			}
			else
			{
				m_gravity = 0;
				m_speed.Y = 0;
			}
			if (m_currentKeyInput.IsKeyDown(Keys.Space) && m_previousKeyInput.IsKeyUp(Keys.Space))
			{
				if (m_currentKeyInput.IsKeyDown(Keys.Right) || m_currentKeyInput.IsKeyDown(Keys.Left))
				{
					m_speed.Y = -JUMPSTRENGTH;
					if (m_currentKeyInput.IsKeyDown(Keys.Left))
					{
						m_facingRight = false;
						m_speed.X -= JUMPSTRENGTH;
					}
					else
					{
						m_facingRight = true;
						m_speed.X += JUMPSTRENGTH;
					}
					
				}
				else
				{
					m_speed.Y = 0;
				}
				m_currentState = State.Jumping;
			}
		
		}

		private void updateRolling()
		{
			if (m_previousKeyInput.IsKeyUp(Keys.Space) && m_currentKeyInput.IsKeyDown(Keys.Space))
			{
				m_speed.Y -= JUMPSTRENGTH;
				m_currentState = State.Jumping;
				return;
			}
			if (--m_rollTimer <= 0)
			{
				m_currentState = State.Walking;
			}
			else
			{
				if (m_facingRight)
					m_speed.X = 500;
				else
					m_speed.X = -500;
			}
		}

		private void changeAnimation()
		{
			
				if (m_currentState == State.Stop)
				{
					m_img.setSprite("Images//hero_stand");
				}
				else if (m_currentState == State.Walking)
				{
					m_img.setSprite("Images//hero_stand");
				}

				if (m_currentState == State.Jumping)
				{
					if (m_speed.Y < 0)
						m_img.setSprite("Images//hero_jump");
					else
						m_img.setSprite("Images//hero_fall");
				}
				
				else if (m_currentState == State.Rolling)
				{
					m_img.setSprite("Images//hero_roll");
				}
				else if (m_currentState == State.Slide)
				{
					m_img.setSprite("Images//hero_slide");
				}
			
		}

		public override void draw(GameTime a_gameTime)
		{
			base.draw(a_gameTime);
		}

		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			if (a_collisionList.Count == 0)
				m_currentState = State.Jumping;
			bool t_onLadder = false;
			bool t_onFloor = false;
			foreach (Entity t_collider in a_collisionList)
			{
				if (CollisionManager.Collides(this.getHitBox(), t_collider.getHitBox()))
				{
					if (t_collider is Platform)
					{
						//Colliding with ze floor
						if ((int)m_lastPosition.Y - 2 <= (int)t_collider.getLastPosition().Y)
						{
							m_position.setY(t_collider.getPosition().getY() - m_collisionShape.getOutBox().Height);
							m_speed.Y = 0;
							if (m_currentState == State.Jumping || m_currentState ==  State.Climbing)
							{
								if (m_speed.X == 0)
								{
									m_currentState = State.Stop;
									
								}
								else
								{
									m_currentState = State.Walking;
									
								}
							}
							t_onFloor = true;
							continue;
						}

						//Colliding with ze zeeling
						if ((int)m_lastPosition.Y + 2 >= (int)t_collider.getLastPosition().Y)
						{
							m_position.setY(t_collider.getPosition().getY() + t_collider.getHitBox().getOutBox().Height);
							m_speed.Y = 0;
							continue;
						}
						//Colliding with ze left wall
						if ((int)m_lastPosition.X + 2 >= (int)t_collider.getLastPosition().X)
						{
							m_position.setX(t_collider.getPosition().getX() + t_collider.getHitBox().getOutBox().Width);
							if(m_currentState == State.Jumping)
							{
								m_currentState = State.Slide;
								m_facingRight = true;
							}
							m_speed.X = 0;
						}
						//Colliding with ze right wall
						if ((int)m_lastPosition.X - 2 <= (int)t_collider.getLastPosition().X)
						{
							m_position.setX(t_collider.getPosition().getX() - (getHitBox().getOutBox().X));
							if (m_currentState == State.Jumping)
							{
								m_currentState = State.Slide;
								m_facingRight = false;
							}
							m_speed.X = 0;
						}
					}
					else if (t_collider is Ladder)
					{
						
						//Colliding with ze ladd0rz
						Rectangle t_rect = new Rectangle(t_collider.getHitBox().getOutBox().X - 2,
							(int)t_collider.getPosition().getY(), 4, t_collider.getHitBox().getOutBox().Height);
						if (t_rect.Contains((int)m_lastPosition.X, (int)m_lastPosition.Y))
						{
							if (m_currentKeyInput.IsKeyDown(Keys.Up) || (m_currentKeyInput.IsKeyDown(Keys.Down) && !t_onFloor))
							{
								m_speed.X = 0;
								m_currentState = State.Climbing;
								if (m_speed.Y < -CLIMBINGSPEED || m_speed.Y > CLIMBINGSPEED)
									m_speed.Y = 0;
								m_position.setX(t_collider.getPosition().getX());
							}
						}
						t_onLadder = true;
					}
				}
			}
			if (!t_onLadder && m_currentState == State.Climbing)
			{
				m_currentState = State.Jumping;
			}

		}
	}
}
