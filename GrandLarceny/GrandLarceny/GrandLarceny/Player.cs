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
		public const int PLAYERSPEED = 400;
		public const int JUMPSTRENGTH = 400;
		public const int CAMERAMAXDISTANCE = 100;
		public const int ACCELERATION = 2800;
		public const int DEACCELERATION = 1600;
		public const int AIRDEACCELERATION = 700;
		public const int AIRVERTICALACCELERATION = 12;
		public const int SLIDESPEED = 25;
		public const int ROLLSPEED = 1000;

		private float m_originalLayer;
		private float m_rollTimer;
		[NonSerialized]
		public GuiObject[] m_healthHearts;

		public const string STANDHIDINGIMAGE = "Images//Sprite//Hero//hero_wallhide";
		public const string DUCKHIDINGIMAGE = "Images//Sprite//Hero//hero_hide";
		private string m_currentHidingImage;
		[NonSerialized]
		private CollisionRectangle m_standHitBox;
		[NonSerialized]
		private CollisionRectangle m_rollHitBox;
		[NonSerialized]
		private CollisionRectangle m_SlideBox;

		private State m_currentState = State.Stop;
		private State m_lastState = State.Stop;

		[NonSerialized]
		private bool m_isInLight;
		[NonSerialized]
		private Direction m_ladderDirection;
		[NonSerialized]
		private float m_invunerableTimer;
		[NonSerialized]
		private float m_damagedTimer;

		private bool m_facingRight = false;
		private bool m_collidedWithWall = false;
		private int m_health;

		private Keys m_actionKey = Keys.A;

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
			Hanging,
			Damaged
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
			m_health = 3;
			m_healthHearts = new GuiObject[3];
			m_healthHearts[0] = new GuiObject(new Vector2(100, 50), "DevelopmentHotkeys//btn_hero_hotkey_normal");
			Game.getInstance().getState().addGuiObject(m_healthHearts[0]);
			m_healthHearts[1] = new GuiObject(new Vector2(200, 50), "DevelopmentHotkeys//btn_hero_hotkey_normal");
			Game.getInstance().getState().addGuiObject(m_healthHearts[1]);
			m_healthHearts[2] = new GuiObject(new Vector2(300, 50), "DevelopmentHotkeys//btn_hero_hotkey_normal");
			Game.getInstance().getState().addGuiObject(m_healthHearts[2]);
			m_standHitBox = new CollisionRectangle(0, 0, 70, 127, m_position);
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
			m_invunerableTimer = Math.Max(m_invunerableTimer - t_deltaTime, 0);
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
				case State.Damaged:
					{
						updateDamaged(t_deltaTime);
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

		private void updateDamaged(float a_deltaTime)
		{
			m_damagedTimer -= a_deltaTime;
			if (m_damagedTimer <= 0)
			{
				m_damagedTimer = 0;
				m_currentState = State.Jumping;
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
			if (!GameState.wasKeyPressed(Keys.Z) && GameState.isKeyPressed(Keys.Z))
			{
				m_currentState = State.Rolling;
				m_rollTimer = 0.3f;
				return;
			}
			if (GameState.isKeyPressed(Keys.Left) || GameState.isKeyPressed(Keys.Right))
			{
				m_currentState = State.Walking;

				if (GameState.isKeyPressed(Keys.Left))
				{
					m_facingRight = false;
				}
				else
				{
					m_facingRight = true;
				}
			}
			if (!GameState.wasKeyPressed(Keys.X) && GameState.isKeyPressed(Keys.X))
			{
				m_speed.Y -= JUMPSTRENGTH;
				m_currentState = State.Jumping;

			}


		}

		private void updateWalking(float a_deltaTime)
		{
			if (!GameState.wasKeyPressed(Keys.Z) && GameState.isKeyPressed(Keys.Z))
			{

				m_currentState = State.Rolling;
				m_rollTimer = 0.3f;
				return;
			}

			if (GameState.isKeyPressed(Keys.Right))
			{
				if (m_speed.X > PLAYERSPEED)
				{
					m_speed.X = m_speed.X - (DEACCELERATION * a_deltaTime);
				}
				else
				{
					if (GameState.isKeyPressed(Keys.LeftShift))
					{
						m_speed.X = Math.Min(m_speed.X + (ACCELERATION * a_deltaTime), 200);
					}
					else
					{
						m_speed.X = Math.Min(m_speed.X + (ACCELERATION * a_deltaTime), PLAYERSPEED);
					}
				}
			}
			if (GameState.isKeyPressed(Keys.Left))
			{
				if (m_speed.X < -PLAYERSPEED)
				{
					m_speed.X = m_speed.X + (DEACCELERATION * a_deltaTime);
				}
				else
				{
					if (GameState.isKeyPressed(Keys.LeftShift))
					{
						m_speed.X = Math.Max(m_speed.X - (ACCELERATION * a_deltaTime), -200);
					}
					else
					{
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
			if (!GameState.wasKeyPressed(Keys.X) && GameState.isKeyPressed(Keys.X))
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
			else if (GameState.isKeyPressed(Keys.Left))
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
			else if (GameState.isKeyPressed(Keys.Right))
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
			{
				m_facingRight = true;
			}
			else if (m_speed.X < 0)
			{
				m_facingRight = false;
			}

			if (m_speed.Y < -300 && GameState.isKeyPressed(Keys.X))
				m_speed.Y -= AIRVERTICALACCELERATION;

			m_cameraPoint.X = Math.Max(Math.Min(m_cameraPoint.X + (m_speed.X * 1.5f * a_deltaTime), CAMERAMAXDISTANCE), -CAMERAMAXDISTANCE);
		}

		private void updateSliding(float a_deltaTime)
		{
			if (m_lastPosition.Y != m_position.getGlobalY())
			{
				if (((!m_facingRight && GameState.isKeyPressed(Keys.Right)) || (m_facingRight && GameState.isKeyPressed(Keys.Left)))
					&& m_collidedWithWall)
				{
					if (!GameState.wasKeyPressed(Keys.X) && GameState.isKeyPressed(Keys.X))
					{
						m_speed.Y = -JUMPSTRENGTH;
						if (m_facingRight == true)
							m_speed.X += JUMPSTRENGTH * 1.5f;
						else
							m_speed.X -= JUMPSTRENGTH * 1.5f;
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
			if (GameState.isKeyPressed(Keys.Up))
			{
				m_speed.Y = -CLIMBINGSPEED;
			}
			else if (GameState.isKeyPressed(Keys.Down))
			{
				m_speed.Y = CLIMBINGSPEED;
			}
			else
			{
				m_speed.Y = 0;
			}
			if (GameState.isKeyPressed(Keys.X) && !GameState.wasKeyPressed(Keys.X))
			{
				if (!GameState.isKeyPressed(Keys.Down))
				{
					m_speed.Y = -JUMPSTRENGTH;
					if (m_facingRight)
					{
						m_facingRight = false;
						m_speed.X -= PLAYERSPEED;
					}
					else
					{
						m_facingRight = true;
						m_speed.X += PLAYERSPEED;
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
			if (m_ladderDirection == Direction.None)
				m_currentState = State.Jumping;
		}

		private void updateRolling(float a_deltaTime)
		{
			m_rollTimer -= a_deltaTime;


			if ((!GameState.wasKeyPressed(Keys.X) && GameState.isKeyPressed(Keys.X)) || m_rollTimer <= 0)
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
		}

		private void updateHanging()
		{
			m_gravity = 0;
			if (!GameState.wasKeyPressed(Keys.X) && GameState.isKeyPressed(Keys.X))
			{
				if (GameState.isKeyPressed(Keys.Down))
				{
					m_position.plusYWith(1);
					m_currentState = State.Jumping;
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
				else
				{
					m_speed.Y = -JUMPSTRENGTH;
					if (m_facingRight)
					{
						m_speed.X = -PLAYERSPEED;
					}
					else
					{
						m_speed.X = PLAYERSPEED;
					}
					m_currentState = State.Jumping;
				}
			}
			else if (GameState.isKeyPressed(Keys.Down) && m_ladderDirection != Direction.None)
			{
				m_currentState = State.Climbing;
			}
			else if (GameState.isKeyPressed(Keys.Up) && !GameState.wasKeyPressed(Keys.Up))
			{
				m_currentState = State.Stop;
				
				if (m_facingRight)
					m_position.setX(m_position.getGlobalX() + m_standHitBox.m_width);
				else
					m_position.setX(m_position.getGlobalX() - m_standHitBox.m_width);

				m_position.setY(m_position.getGlobalY() - m_standHitBox.m_height);
			}
		}

		private void updateHiding()
		{
			if ((GameState.isKeyPressed(Keys.Up) && !GameState.wasKeyPressed(Keys.Up))
				|| (GameState.isKeyPressed(Keys.Down) && !GameState.wasKeyPressed(Keys.Down))
				|| (GameState.isKeyPressed(Keys.X) && !GameState.wasKeyPressed(Keys.X)))
			{
				m_currentState = State.Stop;
				m_layer = m_originalLayer;
			}
		}

		private void changeAnimation()
		{

			if (m_currentState == State.Stop)
			{
				m_img.setSprite("Images//Sprite//Hero//hero_stand");
			}
			else if (m_currentState == State.Walking)
			{
				m_img.setSprite("Images//Sprite//Hero//hero_walk");
			}

			if (m_currentState == State.Jumping)
			{
				if (m_speed.Y < 0)
					m_img.setSprite("Images//Sprite//Hero//hero_jump");
				else
					m_img.setSprite("Images//Sprite//Hero//hero_fall");
			}

			else if (m_currentState == State.Rolling)
			{
				m_img.setSprite("Images//Sprite//Hero//hero_roll");
			}
			else if (m_currentState == State.Slide)
			{
				m_img.setSprite("Images//Sprite//Hero//hero_slide");
			}
			else if (m_currentState == State.Hanging)
			{
				m_img.setSprite("Images//Sprite//Hero//hero_hang");
			}
			else if (m_currentState == State.Climbing)
			{
				m_img.setSprite("Images//Sprite//Hero//hero_climb");
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
				if ((m_lastState == State.Rolling || (m_lastState == State.Hiding && m_currentHidingImage == DUCKHIDINGIMAGE) || m_lastState == State.Hanging)
					&& m_currentState != State.Rolling && m_currentState != State.Hanging)
				{
					if (m_currentState == State.Hiding && m_currentHidingImage == DUCKHIDINGIMAGE)
					{
						m_imgOffsetX = 0;
					}
					else
					{
						m_imgOffsetX = 0;
						m_imgOffsetY = 0;
						m_collisionShape = m_standHitBox;
						if (m_lastState == State.Rolling || m_lastState == State.Hiding)
						{
							m_position.setY(m_position.getLocalY() - (m_standHitBox.getOutBox().Height - m_rollHitBox.getOutBox().Height));
							Game.getInstance().m_camera.getPosition().plusYWith(m_rollHitBox.getOutBox().Height);
						}
					}

				}
				else if ((m_currentState == State.Rolling || (m_currentState == State.Hiding && m_currentHidingImage == DUCKHIDINGIMAGE) || m_currentState == State.Hanging)
					&& m_lastState != State.Rolling && m_lastState != State.Hiding && m_lastState != State.Hanging)
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
			if (m_ladderDirection != Direction.None && m_collidedWithWall)
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
			Rectangle t_colliderBox = a_collider.getHitBox().getOutBox();
			Rectangle t_playerBox = getHitBox().getOutBox();
			if (!t_colliderBox.Contains((int)m_lastPosition.X + t_playerBox.Width + 4, (int)m_lastPosition.Y)
				&& t_colliderBox.Contains((int)m_position.getGlobalX() + t_playerBox.Width + 4, (int)m_position.getGlobalY())
				&& m_lastPosition.Y < t_colliderBox.Y
				&& m_speed.Y >= 0
				&& m_currentState == State.Jumping)
			{
				m_position.setY(a_collider.getPosition().getGlobalY());
				m_nextPosition.Y = m_position.getGlobalY();
				m_speed.Y = 0;
				m_currentState = State.Hanging;
				m_facingRight = true;
			}
			else if (!t_colliderBox.Contains((int)m_lastPosition.X - 4, (int)m_lastPosition.Y)
				&& t_colliderBox.Contains((int)m_position.getGlobalX() - 4, (int)m_position.getGlobalY())
				&& m_lastPosition.Y < t_colliderBox.Y
				&& m_speed.Y >= 0
				&& m_currentState == State.Jumping)
			{
				m_position.setY(a_collider.getPosition().getGlobalY());
				m_nextPosition.Y = m_position.getGlobalY();
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

		public override void draw(GameTime a_gameTime)
		{
			base.draw(a_gameTime);

		}

		public void dealDamageTo(Vector2 a_knockBackForce)
		{
			if (m_invunerableTimer == 0)
			{
				//deals 1 damage
				m_health = Math.Max(m_health - 1, 0);
				updateHealthGUI();
				m_currentState = State.Damaged;
				m_speed += a_knockBackForce;
				m_invunerableTimer = 2f;
				m_damagedTimer = 1f;
				m_img.setSprite("Images//Sprite//Hero//hero_jump");
			}
		}

		private void updateHealthGUI()
		{
			for (int i = 0; i < 3; ++i)
			{
				if (i + 1 <= m_health )
				{
					m_healthHearts[i].setSprite("DevelopmentHotkeys//btn_hero_hotkey_normal");
				}
				else
				{
					m_healthHearts[i].setSprite("DevelopmentHotkeys//btn_hero_hotkey_pressed");
				}
			}
		}
	}
}
