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
	public class Player : MovingObject
	{
		private Vector2 m_cameraPoint = new Vector2(0, 0);

		public const float CAMERASPEED = 0.1f;

		public const int CLIMBINGSPEED = 200;
		public const int PLAYERSPEED = 600;
		public const int JUMPSTRENGTH = 600;
		public const int CAMERAMAXDISTANCE = 100;
		public const int ACCELERATION = 2000;
		public const int DEACCELERATION = 800;
		public const int AIRDEACCELERATION = 300;
		public const int SLIDESPEED = 25;
		public const int ROLLSPEED = 1000;

		private float m_originalLayer;
		private float m_rollTimer;

		public const string STANDHIDINGIMAGE = "Images//Sprite//hero_wallhide";
		public const string DUCKHIDINGIMAGE = "Images//Sprite//hero_hide";
		private string m_currentHidingImage;
		[NonSerialized]
		private CollisionShape m_standHitBox;
		[NonSerialized]
		private CollisionShape m_rollHitBox;
		[NonSerialized]
		private CollisionShape m_SlideBox;

		private State m_currentState = State.Stop;
		private State m_lastState = State.Stop;

		[NonSerialized]
		private bool m_isInLight;
		[NonSerialized]
		private Direction m_ladderDirection;

		private bool m_facingRight = false;
		private bool m_collidedWithWall = false;

		public enum Direction
		{
			None,
			Left,
			Right
		}

		public enum State
		{
			Stop,
			Walking,
			Jumping,
			Slide,
			Climbing,
			Rolling,
			Hiding,
			Hanging
		}

		public Player(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			m_originalLayer = a_layer;
			m_currentState = State.Jumping;
			m_currentHidingImage = STANDHIDINGIMAGE;
		}

		public override void loadContent()
		{
			base.loadContent();
			m_standHitBox = new CollisionRectangle(0, 0, 70, 134, m_position);
			m_rollHitBox = new CollisionRectangle(0, 0, 70, 67, m_position);
			m_SlideBox = new CollisionRectangle(0, m_standHitBox.getOutBox().Height / 2, m_standHitBox.getOutBox().Width, 1, m_position);
			m_collisionShape = m_standHitBox;
		}

		public override void update(GameTime a_gameTime)
		{
			changeAnimation();
			updateState();
			m_lastState = m_currentState;
			m_gravity = 1000f;
			float t_deltaTime = ((float)a_gameTime.ElapsedGameTime.Milliseconds) / 1000f;
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
				case State.Hiding:
					{
						updateHiding();
						break;
					}
			}
			flipSprite();
			base.update(a_gameTime);
			if ((Game.getInstance().m_camera.getPosition().getLocalCartesianCoordinates() - m_cameraPoint).Length() > 3)
			{
				Game.getInstance().m_camera.getPosition().smoothStep(m_cameraPoint, CAMERASPEED);
			}
		}

		public State getCurrentState()
		{
			return m_currentState;
		}
		public State getLastState()
		{
			return m_lastState;
		}
		public void setState(State a_state)
		{
			m_currentState = a_state;
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
			if (GameState.m_previousKeyInput.IsKeyUp(Keys.Down) && GameState.m_currentKeyInput.IsKeyDown(Keys.Down))
			{
				m_currentState = State.Rolling;
				m_rollTimer = 0.3f;
				return;
			}
			if (GameState.m_currentKeyInput.IsKeyDown(Keys.Left) || GameState.m_currentKeyInput.IsKeyDown(Keys.Right))
			{
				m_currentState = State.Walking;

				if (GameState.m_currentKeyInput.IsKeyDown(Keys.Left))
				{
					m_facingRight = false;
				}
				else
				{
					m_facingRight = true;
				}
			}
			if (GameState.m_previousKeyInput.IsKeyUp(Keys.Space) && GameState.m_currentKeyInput.IsKeyDown(Keys.Space))
			{
				m_speed.Y -= JUMPSTRENGTH;
				m_currentState = State.Jumping;

			}


		}

		private void updateWalking(float a_deltaTime)
		{
			if (GameState.m_previousKeyInput.IsKeyUp(Keys.Down) && GameState.m_currentKeyInput.IsKeyDown(Keys.Down))
			{

				m_currentState = State.Rolling;
				m_rollTimer = 0.3f;
				return;
			}

			if (GameState.m_currentKeyInput.IsKeyDown(Keys.Right))
			{
				if (m_speed.X > PLAYERSPEED)
				{
					m_speed.X = m_speed.X - (DEACCELERATION * a_deltaTime);
				}
				else
				{
					if (GameState.m_currentKeyInput.IsKeyDown(Keys.LeftShift))
					{
						m_speed.X = Math.Min(m_speed.X + (ACCELERATION * a_deltaTime), 200);
					} else {
						m_speed.X = Math.Min(m_speed.X + (ACCELERATION * a_deltaTime), PLAYERSPEED);
					}
				}
			}
			if (GameState.m_currentKeyInput.IsKeyDown(Keys.Left))
			{
				if (m_speed.X < -PLAYERSPEED)
				{
					m_speed.X = m_speed.X + (DEACCELERATION * a_deltaTime);
				}
				else
				{
					if (GameState.m_currentKeyInput.IsKeyDown(Keys.LeftShift))
					{
						m_speed.X = Math.Max(m_speed.X - (ACCELERATION * a_deltaTime), -200);
					} else {
						m_speed.X = Math.Max(m_speed.X - (ACCELERATION * a_deltaTime), -PLAYERSPEED);
					}
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
			if (GameState.m_previousKeyInput.IsKeyUp(Keys.Space) && GameState.m_currentKeyInput.IsKeyDown(Keys.Space))
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
			if (GameState.m_currentKeyInput.IsKeyUp(Keys.Left) && GameState.m_currentKeyInput.IsKeyUp(Keys.Right))
			{
				if (m_speed.X > 0)
					m_speed.X = Math.Max(m_speed.X - (AIRDEACCELERATION * a_deltaTime), 0);
				else if (m_speed.X < 0)
					m_speed.X = Math.Min(m_speed.X + (AIRDEACCELERATION * a_deltaTime), 0);
			}
			else if (GameState.m_currentKeyInput.IsKeyDown(Keys.Left))
			{
				if (m_speed.X < -PLAYERSPEED)
				{
					m_speed.X += AIRDEACCELERATION * a_deltaTime;
				}
				else
				{
					m_speed.X = Math.Max(-PLAYERSPEED, m_speed.X - AIRDEACCELERATION * a_deltaTime);
				}
			}
			else if (GameState.m_currentKeyInput.IsKeyDown(Keys.Right))
			{
				if (m_speed.X > PLAYERSPEED)
				{
					m_speed.X -= AIRDEACCELERATION * a_deltaTime;
				}
				else
				{
					m_speed.X = Math.Min(PLAYERSPEED, m_speed.X + AIRDEACCELERATION * a_deltaTime);
				}
			}
			if (m_speed.X > 0)
				m_facingRight = true;
			else if (m_speed.X < 0)
				m_facingRight = false;
			m_cameraPoint.X = Math.Max(Math.Min(m_cameraPoint.X + (m_speed.X * 1.5f * a_deltaTime), CAMERAMAXDISTANCE), -CAMERAMAXDISTANCE);
		}

		private void updateSliding(float a_deltaTime)
		{
			if (m_lastPosition.Y != m_position.getGlobalY())
			{
				if (((!m_facingRight && GameState.m_currentKeyInput.IsKeyDown(Keys.Right)) || (m_facingRight && GameState.m_currentKeyInput.IsKeyDown(Keys.Left)))
					&& m_collidedWithWall)
				{
					if (GameState.m_previousKeyInput.IsKeyUp(Keys.Space) && GameState.m_currentKeyInput.IsKeyDown(Keys.Space))
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
			m_gravity = 0;
			if (GameState.m_currentKeyInput.IsKeyDown(Keys.Up))
			{
				m_speed.Y = -CLIMBINGSPEED;
			}
			else if (GameState.m_currentKeyInput.IsKeyDown(Keys.Down))
			{
				m_speed.Y = CLIMBINGSPEED;
			}
			else
			{
				m_speed.Y = 0;
			}
			if (GameState.m_currentKeyInput.IsKeyDown(Keys.Space) && GameState.m_previousKeyInput.IsKeyUp(Keys.Space))
			{
				if (GameState.m_currentKeyInput.IsKeyDown(Keys.Right) || GameState.m_currentKeyInput.IsKeyDown(Keys.Left))
				{
					m_speed.Y = -JUMPSTRENGTH;
					if (GameState.m_currentKeyInput.IsKeyDown(Keys.Left))
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
					if (m_facingRight)
					{
						m_position.plusXWith(-1);
					}
					else
					{
						m_position.plusXWith(1);
					}
				}
				m_currentState = State.Jumping;
			}
			if(m_ladderDirection == Direction.None)
				m_currentState = State.Jumping;
		}

		private void updateRolling(float a_deltaTime)
		{
			m_rollTimer -= a_deltaTime;


			if ((GameState.m_previousKeyInput.IsKeyUp(Keys.Space) && GameState.m_currentKeyInput.IsKeyDown(Keys.Space)) || m_rollTimer <= 0)
			{
				if (m_rollTimer <= 0)
				{
					m_currentState = State.Walking;
				}
				else
				{
					m_speed.Y -= JUMPSTRENGTH;
					m_currentState = State.Jumping;
				}
			}
			else
			{
				if (m_facingRight)
				{
					m_speed.X = ROLLSPEED;
				}
				else
				{
					m_speed.X = -ROLLSPEED;
				}
			}
			/*if (m_position.getGlobalY() != getLastPosition().Y)
			{
				m_currentState = State.Jumping;
			}*/
		}

		private void updateHanging()
		{
			m_gravity = 0;
			if (GameState.m_previousKeyInput.IsKeyUp(Keys.Space) && GameState.m_currentKeyInput.IsKeyDown(Keys.Space))
			{
				m_speed.Y -= JUMPSTRENGTH;
				m_currentState = State.Jumping;
				return;
			}
			if ((GameState.m_currentKeyInput.IsKeyDown(Keys.Left) || GameState.m_currentKeyInput.IsKeyDown(Keys.Down)) && m_facingRight)
			{
				m_position.plusYWith(1);
				m_currentState = State.Jumping;
			}
			if ((GameState.m_currentKeyInput.IsKeyDown(Keys.Right) || GameState.m_currentKeyInput.IsKeyDown(Keys.Down)) && !m_facingRight)
			{
				m_position.plusYWith(1);
				m_currentState = State.Jumping;
			}
		}

		private void updateHiding()
		{
			if((GameState.m_currentKeyInput.IsKeyDown(Keys.Up) && GameState.m_previousKeyInput.IsKeyUp(Keys.Up))
				|| (GameState.m_currentKeyInput.IsKeyDown(Keys.Down) && GameState.m_previousKeyInput.IsKeyUp(Keys.Down))
				|| (GameState.m_currentKeyInput.IsKeyDown(Keys.Space) && GameState.m_previousKeyInput.IsKeyUp(Keys.Space)))
			{
				m_currentState  = State.Stop;
				m_layer = m_originalLayer;
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
			else if (m_currentState == State.Hanging)
			{
				m_img.setSprite("Images//Sprite//hero_hang");
			}
			else if (m_currentState == State.Climbing)
			{
				m_img.setSprite("Images//Sprite//hero_climb");
			}
			else if (m_currentState == State.Hiding)
			{
				m_img.setSprite(m_currentHidingImage);
			}
		}
		private void updateState()
		{
			if (m_currentState != m_lastState)
			{
				if (m_lastState == State.Rolling || m_lastState == State.Hiding || m_lastState == State.Hanging)
				{
					m_collisionShape = m_standHitBox;
					if (m_lastState == State.Rolling || m_lastState == State.Hiding)
					{
						m_position.setY(m_position.getLocalY() - (m_standHitBox.getOutBox().Height - m_rollHitBox.getOutBox().Height));
						Game.getInstance().m_camera.getPosition().plusYWith(m_rollHitBox.getOutBox().Height);
					}
					m_imgOffsetX = 0;
					m_imgOffsetY = 0;
				}
				else if (m_currentState == State.Rolling || m_currentState == State.Hiding || m_currentState == State.Hanging)
				{
					if (m_currentState == State.Rolling || m_currentState == State.Hiding)
					{
						if (m_facingRight && m_currentState == State.Rolling)
						{
							m_imgOffsetX = -m_rollHitBox.getOutBox().Width;
						}
						m_imgOffsetY -= m_img.getSize().Y - m_rollHitBox.getOutBox().Height;
						m_position.setY(m_position.getLocalY() + (m_standHitBox.getOutBox().Height - m_rollHitBox.getOutBox().Height));
						Game.getInstance().m_camera.getPosition().plusYWith(-m_rollHitBox.getOutBox().Height);
					}
					else if (m_currentState == State.Hanging)
					{
						if (m_facingRight)
						{
							m_imgOffsetX = 4;
						}
						else
						{
							m_imgOffsetX = -4;
						}
						m_imgOffsetY = -2;
					}
					m_collisionShape = m_rollHitBox;
				}
			}
		}

		public override void draw(GameTime a_gameTime)
		{
			base.draw(a_gameTime);
		}

		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			m_collidedWithWall = false;
			m_ladderDirection = 0;
			m_isInLight = false;
			if (a_collisionList.Count == 0)
			{
				m_currentState = State.Jumping;
			}
			else
			{
				base.collisionCheck(a_collisionList);
			}
			if(m_ladderDirection != Direction.None && m_collidedWithWall)
			{
				m_speed.X = 0;
				if (m_currentState == State.Walking)
				{
					m_position.plusYWith(-1);
				}
				m_currentState = State.Climbing;
				if (m_speed.Y < -Player.CLIMBINGSPEED || m_speed.Y > Player.CLIMBINGSPEED)
					m_speed.Y = 0;
				if (m_ladderDirection == Direction.Left)
				{
					m_facingRight = false;
				}
				else
				{
					m_facingRight = true;
				}
			}
		}
		public bool isInLight()
		{
			return m_isInLight;
		}
		public bool isFacingRight()
		{
			return m_facingRight;
		}
		public void setFacingRight(bool a_facingRight)
		{
			m_facingRight = a_facingRight;
		}

		public void hang(Entity a_collider)
		{
			if (!a_collider.getHitBox().getOutBox().Contains((int)m_lastPosition.X + getHitBox().getOutBox().Width + 4, (int)m_lastPosition.Y)
				&& a_collider.getHitBox().getOutBox().Contains((int)m_position.getGlobalX() + getHitBox().getOutBox().Width + 4, (int)m_position.getGlobalY())
				&& m_lastPosition.Y < a_collider.getHitBox().getOutBox().Y
				&& m_speed.Y >= 0
				&& m_currentState == State.Jumping)
			{
				m_position.setY(a_collider.getPosition().getGlobalY());
				m_speed.Y = 0;
				m_currentState = State.Hanging;
				m_facingRight = true;
			}
			else if (!a_collider.getHitBox().getOutBox().Contains((int)m_lastPosition.X - 4, (int)m_lastPosition.Y)
				&& a_collider.getHitBox().getOutBox().Contains((int)m_position.getGlobalX() - 4, (int)m_position.getGlobalY())
				&& m_lastPosition.Y < a_collider.getHitBox().getOutBox().Y
				&& m_speed.Y >= 0
				&& m_currentState == State.Jumping)
			{
				m_position.setY(a_collider.getPosition().getGlobalY());
				m_speed.Y = 0;
				m_currentState = State.Hanging;
				m_facingRight = false;
			}
		}
	
		public void setIsInLight(bool a_isInLight)
		{
 			m_isInLight = a_isInLight;
		}

		public void setCollidedWithWall(bool a_collided)
		{
			m_collidedWithWall = a_collided;
		}

		public void setIsOnLadderWithDirection(Direction a_ladderDirection)
		{
			m_ladderDirection = a_ladderDirection;
		}

		public void setHidingImage(String a_imgPath)
		{
			m_currentHidingImage = a_imgPath;
		}

		public string getHidingImage()
		{
			return m_currentHidingImage;
		}

		public CollisionShape getSlideBox()
		{
			return m_SlideBox;
		}
	}
}
