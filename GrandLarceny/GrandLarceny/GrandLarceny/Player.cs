﻿using System;
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
		private const int PLAYERSPEED = 600;
		private const int JUMPSTRENGTH = 600;
		private const int CAMERAMAXDISTANCE = 100;
		private const int ACCELERATION = 2000;
		private const int DEACCELERATION = 800;
		private const int AIRDEACCELERATION = 300;
		private const int SLIDESPEED = 25;
		private const int ROLLSPEED = 1000;

		private const int ROLLSTANDDIFF = 65;

		private float m_rollTimer;

		[NonSerialized]
		private CollisionShape standHitBox;
		[NonSerialized]
		private CollisionShape rollHitBox;

		[NonSerialized]
		private KeyboardState m_currentKeyInput;
		[NonSerialized]
		private KeyboardState m_previousKeyInput;
		private State m_currentState = State.Stop;

		[NonSerialized]
		private bool m_isInLight;

		private bool m_facingRight = false;

		enum State
		{
			Stop,
			Walking,
			Jumping,
			Slide,
			Climbing, 
			Rolling,
			Hanging
		}

		public Player(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			m_currentState = State.Jumping;
		}

		public override void loadContent()
		{
			base.loadContent();
			standHitBox = new CollisionRectangle(0, 0, 72 - 1, 138 - 1, m_position);
			rollHitBox = new CollisionRectangle(0, 0, 72, 72, m_position);
			m_collisionShape = standHitBox;
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
					updateRolling(t_deltaTime);
					break;
				}
				case State.Hanging:
				{
					updateHanging();
					break;
				}
			}
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
				m_rollTimer = 0.3f;
				m_collisionShape = rollHitBox;
				m_position.setY(m_position.getLocalY() + ROLLSTANDDIFF);
				Game.getInstance().m_camera.getPosition().plusYWith(-ROLLSTANDDIFF);
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
				m_rollTimer = 0.3f;
				m_collisionShape = rollHitBox;
				m_position.setY(m_position.getLocalY() + ROLLSTANDDIFF);
				Game.getInstance().m_camera.getPosition().plusYWith(-ROLLSTANDDIFF);
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
			if (m_position.getGlobalY() != getLastPosition().Y)
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
			if (m_lastPosition.Y != m_position.getGlobalY())
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

		private void updateRolling(float a_deltaTime)
		{
			m_rollTimer -= a_deltaTime;
			if (m_previousKeyInput.IsKeyUp(Keys.Space) && m_currentKeyInput.IsKeyDown(Keys.Space))
			{
				m_speed.Y -= JUMPSTRENGTH;
				m_currentState = State.Jumping;
				m_collisionShape = standHitBox;
				m_position.setY(m_position.getLocalY() - ROLLSTANDDIFF);
				Game.getInstance().m_camera.getPosition().plusYWith(+ROLLSTANDDIFF);
				return;
			}
			if (m_rollTimer <= 0)
			{
				m_currentState = State.Walking;
				m_collisionShape = standHitBox;
				m_position.setY(m_position.getLocalY() - ROLLSTANDDIFF);
				Game.getInstance().m_camera.getPosition().plusYWith(+ROLLSTANDDIFF);
			}
			else
			{
				if (m_facingRight)
					m_speed.X = ROLLSPEED;
				else
					m_speed.X = -ROLLSPEED;
			}
		}

		private void updateHanging()
		{
			m_gravity = 0;
			if (m_previousKeyInput.IsKeyUp(Keys.Space) && m_currentKeyInput.IsKeyDown(Keys.Space))
			{
				m_speed.Y -= JUMPSTRENGTH;
				m_currentState = State.Jumping;
				return;
			}
			if ((m_currentKeyInput.IsKeyDown(Keys.Left)  || m_currentKeyInput.IsKeyDown(Keys.Down)) && m_facingRight)
			{
				m_position.plusYWith(-1);
				m_currentState = State.Jumping;
			}
			if ((m_currentKeyInput.IsKeyDown(Keys.Right) || m_currentKeyInput.IsKeyDown(Keys.Down)) && !m_facingRight)
			{
				m_position.plusYWith(-1);
				m_currentState = State.Jumping;
			}
		}

		private void changeAnimation()
		{
			
				if (m_currentState == State.Stop)
				{
					m_img.setSprite("Images//Sprite//hero_stand");
				}
				else if (m_currentState == State.Walking)
				{
					m_img.setSprite("Images//Sprite//hero_stand");
				}

				if (m_currentState == State.Jumping)
				{
					if (m_speed.Y < 0)
						m_img.setSprite("Images//Sprite//hero_jump");
					else
						m_img.setSprite("Images//Sprite//hero_fall");
				}
				
				else if (m_currentState == State.Rolling)
				{
					m_img.setSprite("Images//Sprite//hero_roll");

				}
				else if (m_currentState == State.Slide)
				{
					m_img.setSprite("Images//Sprite//hero_slide");
				}
			
		}

		public override void draw(GameTime a_gameTime)
		{
			base.draw(a_gameTime);
		}

		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			if (a_collisionList.Count == 0 && m_currentState != State.Rolling)
				m_currentState = State.Jumping;
			bool t_onLadder = false;
			bool t_notSupposedToSlide = true;
			bool t_onFloor = false;
			m_isInLight = false;
			foreach (Entity t_collider in a_collisionList)
			{
				if (t_collider is Wall)
					t_notSupposedToSlide = false;
				if (CollisionManager.Collides(this.getHitBox(), t_collider.getHitBox()))
				{
					if (t_collider is Platform || t_collider is Wall)
					{
						//Colliding with ze Wall wall
						if (t_collider is Wall)
						{
							//Colliding with ze left Wall wall
							if ((int)m_lastPosition.X + 1 >= (int)t_collider.getLastPosition().X + t_collider.getHitBox().getOutBox().Width)
							{
								m_position.setX(t_collider.getPosition().getGlobalX() + t_collider.getHitBox().getOutBox().Width);
								if (m_currentState == State.Jumping)
								{
									m_currentState = State.Slide;
									m_facingRight = true;
								}
								m_speed.X = 0;
								continue;
							}
							//Colliding with ze right Wall wall
							if ((int)m_lastPosition.X + getHitBox().getOutBox().Width - 1 <= (int)t_collider.getLastPosition().X)
							{
								m_position.setX(t_collider.getPosition().getGlobalX() - (getHitBox().getOutBox().Width));
								if (m_currentState == State.Jumping)
								{
									m_currentState = State.Slide;
									m_facingRight = false;
								}
								m_speed.X = 0;
								continue;
							}
						}
						//Colliding with ze floor
						if ((int)m_lastPosition.Y + getHitBox().getOutBox().Height <= (int)t_collider.getLastPosition().Y)
						{
							m_position.setY(t_collider.getPosition().getGlobalY() - m_collisionShape.getOutBox().Height);
							m_speed.Y = 0;
							if (m_currentState == State.Jumping || m_currentState == State.Climbing)
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
						if ((int)m_lastPosition.Y >= (int)t_collider.getLastPosition().Y + t_collider.getHitBox().getOutBox().Height)
						{
							m_position.setY(t_collider.getPosition().getGlobalY() + t_collider.getHitBox().getOutBox().Height);
							m_speed.Y = 0;
							continue;
						}
						//Colliding with ze left wall
						if ((int)m_lastPosition.X + 1 >= (int)t_collider.getLastPosition().X + t_collider.getHitBox().getOutBox().Width)
						{
							m_position.setX(t_collider.getPosition().getGlobalX() + t_collider.getHitBox().getOutBox().Width);
							m_speed.X = 0;
							climb(t_collider);
						}
						//Colliding with ze right wall
						if ((int)m_lastPosition.X + getHitBox().getOutBox().Width - 1 <= (int)t_collider.getLastPosition().X)
						{
							m_position.setX(t_collider.getPosition().getGlobalX() - (getHitBox().getOutBox().Width));
							m_speed.X = 0;
							climb(t_collider);
						}
					}
					else if (t_collider is Ladder)
					{

						//Colliding with ze ladd0rz
						Rectangle t_rect = new Rectangle(t_collider.getHitBox().getOutBox().X - 2,
							(int)t_collider.getPosition().getGlobalY(), 4, t_collider.getHitBox().getOutBox().Height);
						if (t_rect.Contains((int)m_lastPosition.X, (int)m_lastPosition.Y))
						{
							if (m_currentKeyInput.IsKeyDown(Keys.Up) || (m_currentKeyInput.IsKeyDown(Keys.Down) && !t_onFloor))
							{
								m_speed.X = 0;
								m_currentState = State.Climbing;
								if (m_speed.Y < -CLIMBINGSPEED || m_speed.Y > CLIMBINGSPEED)
									m_speed.Y = 0;
								m_position.setX(t_collider.getPosition().getGlobalX());
							}
						}
						t_onLadder = true;
					}
					else if (t_collider is LightCone)
						m_isInLight = true;
				}
				else if (t_collider is Platform)
					climb(t_collider);
			}
			if (t_notSupposedToSlide && m_currentState == State.Slide)
				m_currentState = State.Jumping;
			if (!t_onLadder && m_currentState == State.Climbing)
				m_currentState = State.Jumping;
		}
		public bool isInLight()
		{
			return m_isInLight;
		}
		private void climb(Entity a_collider)
		{
			if (!a_collider.getHitBox().getOutBox().Contains((int)m_lastPosition.X + getHitBox().getOutBox().Width + 4, (int)m_lastPosition.Y)
				&& a_collider.getHitBox().getOutBox().Contains((int)m_position.getGlobalX() + getHitBox().getOutBox().Width + 4, (int)m_position.getGlobalY())
				&& m_speed.Y >= 0)
			{
				m_position.setY(a_collider.getPosition().getGlobalY());
				m_speed.Y = 0;
				m_currentState = State.Hanging;
				m_facingRight = true;
			}
			else if (!a_collider.getHitBox().getOutBox().Contains((int)m_lastPosition.X - 4, (int)m_lastPosition.Y)
				&& a_collider.getHitBox().getOutBox().Contains((int)m_lastPosition.X - 4, (int)m_lastPosition.Y + 6)
				&& m_speed.Y >= 0)
			{
				m_position.setY(a_collider.getPosition().getGlobalY());
				m_speed.Y = 0;
				m_currentState = State.Hanging;
				m_facingRight = false;
			}
		}
	}
}
