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
		#region variables/enums/initiation
		private Vector2 m_cameraPoint = new Vector2(0, 0);

		public const float CAMERASPEED = 0.1f;
		public const float MAXSWINGSPEED = 7f;

		public const int CLIMBINGSPEED = 200;
		public const int PLAYERSPEED = 200;
		public const int PLAYERSPEEDCHASEMODE = 420;
		public const int JUMPSTRENGTH = 520;
		public const int CAMERAMAXDISTANCE = 100;
		public const int ACCELERATION = 2800;
		public const int DEACCELERATION = 1600;
		public const int AIRDEACCELERATION = 700;
		public const int AIRVERTICALACCELERATION = 15;
		public const int SLIDESPEED = 25;
		public const int ROLLSPEED = 1200;

		private int m_playerCurrentSpeed;
		private int m_health;

		[NonSerialized]
		public GuiObject[] m_healthHearts;

		public const string STANDHIDINGIMAGE = "hero_wallhide";
		public const string DUCKHIDINGIMAGE = "hero_hide";
		public const string VENTIDLEIMAGE = "hero_ventilation_idle";
		private string m_currentHidingImage;
		private string m_currentVentilationImage = "hero_ventilation_idle";
		private string m_currentSwingingImage = "hero_swing_still";
		[NonSerialized]
		private CollisionRectangle m_standHitBox;
		[NonSerialized]
		private CollisionRectangle m_rollHitBox;
		[NonSerialized]
		private CollisionRectangle m_SlideBox;
		[NonSerialized]
		private CollisionRectangle m_hangHitBox;

		private State m_currentState = State.Stop;
		private State m_lastState = State.Stop;
		private State m_stunnedState = State.Stop;

		[NonSerialized]
		private bool m_isInLight;
		[NonSerialized]
		private Direction m_ladderDirection;
		[NonSerialized]
		private float m_invulnerableTimer;
		[NonSerialized]
		private float m_stunnedTimer;
		[NonSerialized]
		private float m_windowActionCD = 0;
		[NonSerialized]
		private float m_rollActionCD = 0;

		private float m_originalLayer;
		private float m_swingSpeed;
		

		private List<Direction> m_ventilationDirection;
		private List<Direction> m_leftRightList;
		private List<Direction> m_upDownList;

		private Entity m_currentVentilation = null;

		private bool m_facingRight = false;
		private bool m_collidedWithWall = false;
		private bool m_stunned = false;
		private bool m_stunnedDeacceleration = true;
		private bool m_stunnedGravity = true;
		private bool m_stunnedFlipSprite = false;
		private bool m_chase = false;
		private bool m_deactivateChase = false;
		private bool m_runMode = false;

		private Rope m_rope = null;

		public enum Direction
		{
			None,		Left,		Right,
			Up,			Down
		}

		public enum State
		{
			Stop,		Walking,	Jumping,
			Slide,		Climbing,	Rolling,
			Hiding,		Hanging,	Ventilation,
			Swinging
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
			m_healthHearts[0] = new GuiObject(new Vector2(100, 50), "GameGUI//health");
			Game.getInstance().getState().addGuiObject(m_healthHearts[0]);
			m_healthHearts[1] = new GuiObject(new Vector2(200, 50), "GameGUI//health");
			Game.getInstance().getState().addGuiObject(m_healthHearts[1]);
			m_healthHearts[2] = new GuiObject(new Vector2(300, 50), "GameGUI//health");
			Game.getInstance().getState().addGuiObject(m_healthHearts[2]);
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_stand");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_walk");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_jump");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_fall");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_slide");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_hang");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_climb");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_roll");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_climb_ledge");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_window_heave");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_ventilation_idle");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_ventilation_vertical");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_ventilation_horizontal");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_swing_back");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_swing_still");
			Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//hero_swing_forth");
			m_standHitBox = new CollisionRectangle(0, 0, 70, 127, m_position);
			m_rollHitBox = new CollisionRectangle(0, 0, 70, 72, m_position); // 67
			m_SlideBox = new CollisionRectangle(0, m_standHitBox.getOutBox().Height / 2, m_standHitBox.getOutBox().Width, 1, m_position);
			m_hangHitBox = new CollisionRectangle(0, 0, 70, 80, m_position);
			m_collisionShape = m_standHitBox;
			m_ventilationDirection = new List<Direction>();
			m_upDownList = new List<Direction>();
			m_upDownList.Add(Direction.Up);
			m_upDownList.Add(Direction.Down);
			m_leftRightList = new List<Direction>();
			m_leftRightList.Add(Direction.Left);
			m_leftRightList.Add(Direction.Right);
			m_playerCurrentSpeed = PLAYERSPEED;
			m_swingSpeed = 0;
			m_currentVentilationImage = VENTIDLEIMAGE;
			m_position.plusYWith(-1);

		}
		#endregion

		#region update
		public override void update(GameTime a_gameTime)
		{
			
			m_lastPosition = m_position.getGlobalCartesianCoordinates();

			if (!m_stunned) {
				changeAnimation();
			}

			float t_deltaTime = ((float)a_gameTime.ElapsedGameTime.Milliseconds) / 1000f;
			m_invulnerableTimer = Math.Max(m_invulnerableTimer - t_deltaTime, 0);

			if (m_deactivateChase)
			{
				activateNormalMode();
				m_deactivateChase = false;
			}
			

			updateState();
			m_lastState = m_currentState;

			if (m_currentState != State.Hanging)
			{
				m_gravity = 1600f;
			}

			updateCD(t_deltaTime);

			if (!m_stunned)
			{
				changeAnimation();
				
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
						updateHiding(t_deltaTime);
						break;
					}
					case State.Ventilation:
					{
						updateVentilation(t_deltaTime);
						break;
					}
					case State.Swinging:
					{
						updateSwinging(t_deltaTime);
						break;
					}
				}
			}
			else
			{
				updateStunned(t_deltaTime);
			}

			updateFlip();
			base.update(a_gameTime);
			if ((Game.getInstance().m_camera.getPosition().getLocalCartesianCoordinates() - m_cameraPoint).Length() > 3)
			{
				Game.getInstance().m_camera.getPosition().smoothStep(m_cameraPoint, CAMERASPEED);
			}
		}

		private void updateCD(float a_deltaTime)
		{
			if (m_windowActionCD > 0)
			{
				m_windowActionCD -= a_deltaTime;
			}
			if (m_rollActionCD > 0)
			{
				m_rollActionCD -= a_deltaTime;
			}
		}

		private void updateStunned(float a_deltaTime)
		{
			m_stunnedTimer -= a_deltaTime;
			if (!m_stunnedGravity)
			{
				m_gravity = 0;
			}
			if (m_stunnedDeacceleration)
			{
				if (m_speed.X > 0)
				{
					m_speed.X = Math.Max(m_speed.X - (DEACCELERATION * a_deltaTime), 0);
				}
				else if (m_speed.X < 0)
				{
					m_speed.X = Math.Min(m_speed.X + (DEACCELERATION * a_deltaTime), 0);
				}
			}
			if (m_stunnedTimer <= 0)
			{
				m_stunnedTimer = 0;
				m_stunned = false;
				
				m_stunnedGravity = true;
				m_speed.X = 0;

				if (m_stunnedFlipSprite)
				{
					m_facingRight = !m_facingRight;
					m_stunnedFlipSprite = false;
				}

				if (m_collisionShape == null)
				{
					if (m_stunnedState == State.Hanging)
					{
						changeAnimation();
									
						m_collisionShape = m_hangHitBox;
						//m_position.plusYWith(m_standHitBox.m_height / 1.1f);
						m_lastPosition.Y = m_position.getGlobalY() - 20;
						if (m_currentState != State.Hanging)
						{
							m_lastState = State.Hanging;
							m_imgOffsetY += m_rollHitBox.m_height / 4f;
							m_position.plusYWith(m_rollHitBox.m_height / 1.3f - 1);
							Game.getInstance().m_camera.getPosition().plusYWith(-m_rollHitBox.m_height / 1.3f);
						}
						else
						{
							m_imgOffsetY += m_standHitBox.m_height / 1.8f;
						}
						if (!m_facingRight)
						{
							m_imgOffsetX += m_standHitBox.m_width * 1.9f;
						}
						
					}
					else if (m_stunnedState == State.Rolling || (m_stunnedState == State.Hiding && m_currentHidingImage == DUCKHIDINGIMAGE))
					{
						m_collisionShape = m_rollHitBox;
					}
					else if (m_stunnedState == State.Slide)
					{
						m_collisionShape = m_SlideBox;
					}
					else
					{
						m_collisionShape = m_standHitBox;
					}
				}
				if (m_currentState == State.Stop)
				{
					changeAnimation();
					m_imgOffsetX = 0;
					m_imgOffsetY = 0;
				}
				m_currentState = m_stunnedState;		
			}
		}

		private void updateStop(float a_deltaTime)
		{
			if (m_speed.Y != 0)
			{
				m_currentState = State.Jumping;
				return;
			}
			if (Game.keyClicked(GameState.getRollKey()) && m_rollActionCD <= 0)
			{
				m_currentState = State.Rolling;
				return;
			}
			if ((Game.isKeyPressed(GameState.getLeftKey()) && !Game.isKeyPressed(GameState.getRightKey())) || (Game.isKeyPressed(GameState.getRightKey()) && !Game.isKeyPressed(GameState.getLeftKey())))
			{
				m_currentState = State.Walking;

				if (Game.isKeyPressed(GameState.getLeftKey()))
				{
					m_facingRight = false;
				}
				else
				{
					m_facingRight = true;
				}
			}
			if (Game.keyClicked(GameState.getJumpKey()))
			{
				m_speed.Y -= JUMPSTRENGTH;
				m_currentState = State.Jumping;
			}
		}

		private void updateWalking(float a_deltaTime)
		{
			if (Game.keyClicked(GameState.getRollKey()) && m_rollActionCD <= 0)
			{
				m_currentState = State.Rolling;
				return;
			}

			if (Game.isKeyPressed(GameState.getRightKey()) && !Game.isKeyPressed(GameState.getLeftKey()))
			{
				if (m_speed.X > m_playerCurrentSpeed)
				{
					m_speed.X = m_speed.X - (DEACCELERATION * a_deltaTime);
				}
				else
				{
					m_speed.X = Math.Min(m_speed.X + (ACCELERATION * a_deltaTime), m_playerCurrentSpeed);
				}
			}
			else if (Game.isKeyPressed(GameState.getLeftKey()) && !Game.isKeyPressed(GameState.getRightKey()))
			{
				if (m_speed.X < -m_playerCurrentSpeed)
				{
					m_speed.X = m_speed.X + (DEACCELERATION * a_deltaTime);
				}
				else
				{
					m_speed.X = Math.Max(m_speed.X - (ACCELERATION * a_deltaTime), -m_playerCurrentSpeed);
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
			if (Game.keyClicked(GameState.getJumpKey()))
			{
				m_speed.Y -= JUMPSTRENGTH;
				m_currentState = State.Jumping;
			}

			m_cameraPoint.X = Math.Max(Math.Min(m_cameraPoint.X + (m_speed.X * 1.5f * a_deltaTime), CAMERAMAXDISTANCE), -CAMERAMAXDISTANCE);
			if (m_chase || m_runMode)
			{
				m_img.setAnimationSpeed(Math.Abs(m_speed.X / 22f));
			}
			else
			{
				m_img.setAnimationSpeed(Math.Abs(m_speed.X / 10f));
			}
			if (m_speed.Y != 0)
			{
				m_currentState = State.Jumping;
			}
		}

		private void updateJumping(float a_deltaTime)
		{
			if (!Game.isKeyPressed(GameState.getLeftKey()) && !Game.isKeyPressed(GameState.getRightKey()))
			{
				if (m_speed.X > 0)
					m_speed.X = Math.Max(m_speed.X - (AIRDEACCELERATION * a_deltaTime), 0);
				else if (m_speed.X < 0)
					m_speed.X = Math.Min(m_speed.X + (AIRDEACCELERATION * a_deltaTime), 0);
			}
			else if (Game.isKeyPressed(GameState.getLeftKey()))
			{
				if (m_speed.X < -m_playerCurrentSpeed)
				{
					m_speed.X += AIRDEACCELERATION * a_deltaTime;
				}
				else
				{
					m_speed.X = Math.Max(-m_playerCurrentSpeed, m_speed.X - AIRDEACCELERATION * a_deltaTime);
				}
			}
			else if (Game.isKeyPressed(GameState.getRightKey()))
			{
				if (m_speed.X > m_playerCurrentSpeed)
				{
					m_speed.X -= AIRDEACCELERATION * a_deltaTime;
				}
				else
				{
					m_speed.X = Math.Min(m_playerCurrentSpeed, m_speed.X + AIRDEACCELERATION * a_deltaTime);
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

			if (m_speed.Y < -300 && Game.isKeyPressed(GameState.getJumpKey()))
				m_speed.Y -= AIRVERTICALACCELERATION;

			m_cameraPoint.X = Math.Max(Math.Min(m_cameraPoint.X + (m_speed.X * 1.5f * a_deltaTime), CAMERAMAXDISTANCE), -CAMERAMAXDISTANCE);
		}

		private void updateSliding(float a_deltaTime)
		{
			//if (m_lastPosition.Y != m_position.getGlobalY())
			//{
				if (((!m_facingRight && Game.isKeyPressed(GameState.getRightKey())) || (m_facingRight && Game.isKeyPressed(GameState.getLeftKey())))
					&& m_collidedWithWall)
				{
					if (Game.keyClicked(GameState.getJumpKey()))
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
			//}
			//m_currentState = State.Walking;
		}

		private void updateClimbing()
		{
			m_gravity = 0;
			if (Game.isKeyPressed(GameState.getUpKey()) && m_ladderDirection != Direction.None)
			{
				m_speed.Y = -CLIMBINGSPEED;
			}
			else if (Game.isKeyPressed(GameState.getDownKey()))
			{
				m_speed.Y = CLIMBINGSPEED;
			}
			else
			{
				m_speed.Y = 0;
			}
			if (Game.keyClicked(GameState.getJumpKey()))
			{
				if (!Game.isKeyPressed(GameState.getDownKey()))
				{
					m_speed.Y = -(JUMPSTRENGTH-70);
					if (m_facingRight)
					{
						m_facingRight = false;
						m_speed.X -= PLAYERSPEED * 2f;
					}
					else
					{
						m_facingRight = true;
						m_speed.X += PLAYERSPEED * 2f;
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
			{
				m_currentState = State.Jumping;
				m_nextPosition.Y = m_position.getGlobalY() + 1;
			}
		}

		private void updateRolling(float a_deltaTime)
		{
			if (m_facingRight)
			{
				m_speed.X = ROLLSPEED;
			}
			else
			{
				m_speed.X = -ROLLSPEED;
			}
			if (m_img.isStopped())
			{
				m_stunned = true;
				m_speed.X = m_speed.X / 5;
				m_stunnedTimer = 0.35f;
				m_stunnedDeacceleration = true;
				m_stunnedState = State.Stop;
				m_rollActionCD = 1f;
			}
			if (m_speed.Y != 0)
			{
				m_speed.X /= 2;
				m_currentState = State.Jumping;
			}
		}

		private void updateHanging()
		{
			m_gravity = 0;
			if (Game.keyClicked(GameState.getJumpKey()))
			{
				if (Game.isKeyPressed(GameState.getDownKey()))
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
					if (!m_facingRight)
						m_speed.X += JUMPSTRENGTH;
					else
						m_speed.X -= JUMPSTRENGTH;
					m_currentState = State.Jumping;
				}
			}
			else if (Game.isKeyPressed(GameState.getDownKey()) && m_ladderDirection != Direction.None)
			{
				m_currentState = State.Climbing; 
				m_position.plusYWith(m_standHitBox.m_height - m_hangHitBox.m_height);
				Game.getInstance().m_camera.getPosition().plusYWith(-(m_standHitBox.m_height - m_hangHitBox.m_height));
			}
			/*else if (Game.keyClicked(GameState.getUpKey()))
			{
				hangClimbAction();
			}*/
			
		}

		private void updateHiding(float a_deltaTime)
		{
			if (   Game.keyClicked(GameState.getUpKey())
				|| Game.keyClicked(GameState.getDownKey())
				|| Game.keyClicked(GameState.getJumpKey())
				|| Game.keyClicked(GameState.getActionKey())) 
			{
				m_currentState = State.Stop;
			}

			if (Game.isKeyPressed(GameState.getLeftKey()) || Game.isKeyPressed(GameState.getRightKey()))
			{
				float t_cameraXPos = 0;
				if (Game.isKeyPressed(GameState.getRightKey()))
				{
					t_cameraXPos = 500;
				}
				else
				{
					t_cameraXPos = -500;
				}
				m_cameraPoint.X = Math.Max(Math.Min(m_cameraPoint.X + (t_cameraXPos * 1.5f * a_deltaTime), CAMERAMAXDISTANCE * 6 + m_standHitBox.m_width), -CAMERAMAXDISTANCE * 6 + m_standHitBox.m_width / 2);
			}
			else
			{
				m_cameraPoint.X = 0;
			}
		}

		private void updateVentilation(float a_deltaTime)
		{
			m_speed = Vector2.Zero;
			m_gravity = 0;
			if (Game.getInstance().m_camera.getLayer() == 0)
			{
				if (Game.keyClicked(GameState.getUpKey()) || Game.keyClicked(GameState.getDownKey()))
				{
					Game.getInstance().m_camera.setLayer(1);
					m_cameraPoint.X = 0;
				}
				else if(Game.isKeyPressed(GameState.getLeftKey()))
				{
					m_cameraPoint.X = Math.Max(m_cameraPoint.X - (500 * a_deltaTime), -1000);
				}
				else if (Game.isKeyPressed(GameState.getRightKey()))
				{
					m_cameraPoint.X = Math.Min(m_cameraPoint.X + (500 * a_deltaTime), 1000);
				}
				else
				{
					m_cameraPoint.X = 0;
				}
			}
			else
			{
				List<Direction> t_list = null;
				foreach (Direction t_direction in m_ventilationDirection)
				{
					t_list = moveDirectionInVentilation(t_direction);
					if (t_list != null)
						break;
				}
				if (t_list != null)
				{
					m_ventilationDirection = t_list;
				}
			}
		}

		private List<Direction> moveDirectionInVentilation(Direction a_direction)
		{
			List<Direction> t_list = null;
			switch (a_direction)
			{
				case Direction.Up:
				{
					if (Game.isKeyPressed(GameState.getUpKey()))
					{
						m_speed.Y = -PLAYERSPEED;
						t_list = m_upDownList;
						if(m_currentVentilation != null)
						{
							if (m_position.getGlobalX() - m_currentVentilation.getPosition().getGlobalX() > 3)
							{
								m_position.plusXWith(-3);
							}
							else if (m_position.getGlobalX() - m_currentVentilation.getPosition().getGlobalX() < -3)
							{
								m_position.plusXWith(3);
							}
							else
							{
								m_position.setGlobalX(m_currentVentilation.getPosition().getGlobalX());
								m_currentVentilation = null;
							}
						}
						m_currentVentilationImage = "hero_ventilation_vertical";
					}
					break;
				}
				case Direction.Left:
				{
					m_currentVentilationImage = "hero_ventilation_idle";
					if (Game.isKeyPressed(GameState.getLeftKey()) && !Game.isKeyPressed(GameState.getRightKey()))
					{
						m_speed.X = -PLAYERSPEED;
						t_list = m_leftRightList;
						if (m_currentVentilation != null)
						{
							if (m_position.getGlobalY() - m_currentVentilation.getPosition().getGlobalY() > 3)
							{
								m_position.plusYWith(-3);
							}
							else if (m_position.getGlobalY() - m_currentVentilation.getPosition().getGlobalY() < -3)
							{
								m_position.plusYWith(3);
							}
							else
							{
								m_position.setGlobalY(m_currentVentilation.getPosition().getGlobalY());
								m_currentVentilation = null;
							}
						}
						m_currentVentilationImage = "hero_ventilation_horizontal";
						m_facingRight = false;
					}
					break;
				}
				case Direction.Right:
				{
					m_currentVentilationImage = "hero_ventilation_idle";					
					if (Game.isKeyPressed(GameState.getRightKey()) && !Game.isKeyPressed(GameState.getLeftKey()))
					{
						m_speed.X = PLAYERSPEED;
						t_list = m_leftRightList;
						if (m_currentVentilation != null)
						{
							if (m_position.getGlobalY() - m_currentVentilation.getPosition().getGlobalY() > 3)
							{
								m_position.plusYWith(-3);
							}
							else if (m_position.getGlobalY() - m_currentVentilation.getPosition().getGlobalY() < -3)
							{
								m_position.plusYWith(3);
							}
							else
							{
								m_position.setGlobalY(m_currentVentilation.getPosition().getGlobalY());
								m_currentVentilation = null;
							}
						}
						m_currentVentilationImage = "hero_ventilation_horizontal";
						m_facingRight = true;
					}
					break;
				}
				case Direction.Down:
				{
					if (Game.isKeyPressed(GameState.getDownKey()))
					{
						m_speed.Y = PLAYERSPEED;
						t_list = m_upDownList;
						if (m_currentVentilation != null)
						{
							if (m_position.getGlobalX() - m_currentVentilation.getPosition().getGlobalX() > 3)
							{
								m_position.plusXWith(-3);
							}
							else if (m_position.getGlobalX() - m_currentVentilation.getPosition().getGlobalX() < -3)
							{
								m_position.plusXWith(3);
							}
							else
							{
								m_position.setGlobalX(m_currentVentilation.getPosition().getGlobalX());
								m_currentVentilation = null;
							}
						}
						m_currentVentilationImage = "hero_ventilation_vertical";
					}
					break;
				}
			}
			return t_list;
		}

		private void updateSwinging(float a_deltaTime)
		{
			m_gravity = 0;
			if (m_position.getLength() > 0)
			{
				if (Game.isKeyPressed(GameState.getRightKey()))
				{
					if (m_swingSpeed > -MAXSWINGSPEED)
					{
						m_swingSpeed -= (300 * a_deltaTime) / m_position.getLength();
					}
				}
				else if (Game.isKeyPressed(GameState.getLeftKey()))
				{
					if (m_swingSpeed < MAXSWINGSPEED)
					{
						m_swingSpeed += (300 * a_deltaTime) / m_position.getLength();
					}
				}
			}
			/*if (m_rope.getRotation() < Math.PI * 1.5f && m_rope.getRotation() > Math.PI / 2)
			{
				m_swingSpeed -= 0.005f;
			}
			else if (m_rope.getRotation() < Math.PI / 2 && m_rope.getRotation() > 0 || m_rope.getRotation() > Math.PI * 1.5f)
			{
				m_swingSpeed += 0.005f;
			}
			else
			{
				if (m_swingSpeed < 0.002f && m_swingSpeed > -0.002f)
				{
					m_swingSpeed = 0;
				}
			}*/
			m_swingSpeed += (float) ( (Math.Cos(m_rope.getRotation()) * 2500 * a_deltaTime) / m_position.getLength() );
			m_swingSpeed *= 0.99f;
			m_rope.addRotation(m_swingSpeed * a_deltaTime);
			m_rotate = (m_rope.getRotation() - ((float)(Math.PI / 2.0))) % ((float)(Math.PI * 2.0));
			m_position.setSlope(m_rope.getRotation());
			if (m_swingSpeed > 1f)
				if (!m_facingRight)
					m_currentSwingingImage = "hero_swing_back";
				else
					m_currentSwingingImage = "hero_swing_forth";
			else if (m_swingSpeed < -1f)
				if (m_facingRight)
					m_currentSwingingImage = "hero_swing_back";
				else
					m_currentSwingingImage = "hero_swing_forth";
			else
				m_currentSwingingImage = "hero_swing_still";

			if (Game.keyClicked(GameState.getJumpKey()))
			{
				m_speed = new Vector2(m_swingSpeed * (m_position.getLength() + m_collisionShape.getOutBox().Height / 2) * (float)Math.Cos(m_rotate + Math.PI), m_swingSpeed * (m_position.getLength() + m_collisionShape.getOutBox().Height / 2) * (float)Math.Sin(m_rotate + Math.PI));
				changePositionType();
				m_position.setParentPositionWithoutMoving(null);
				m_position.setGlobalX(m_position.getGlobalX() - 36);
				m_rotate = 0;
				m_currentState = State.Jumping;
				m_rope.resetPosition();
			}
		}
		#endregion

		#region change animation and state
		private void setSprite(string a_sprite)
		{
			m_img.setSprite("Images//Sprite//Hero//" + a_sprite);
		}

		private void changeAnimation()
		{
			switch (m_currentState)
			{
				case State.Stop:
				{
					setSprite("hero_stand");
					break;
				}
				case State.Walking:
				{
					if (m_chase || m_runMode)
						setSprite("hero_run");
					else
						setSprite("hero_walk");
					break;
				}
				case State.Jumping:
				{
					if (m_speed.Y < 0)
						setSprite("hero_jump");
					else
						setSprite("hero_fall");
					break;
				}
				case State.Rolling:
				{
					setSprite("hero_roll");
					break;
				}
				case State.Slide:
				{
					setSprite("hero_slide");
					break;
				}
				case State.Hanging:
				{
					setSprite("hero_hang");
					break;
				}
				case State.Climbing:
				{
					setSprite("hero_climb");
					break;
				}
				case State.Hiding:
				{
					setSprite(m_currentHidingImage);
					break;
				}
				case State.Ventilation:
				{
					setSprite(m_currentVentilationImage);
					break;
				}
				case State.Swinging:
				{
					setSprite(m_currentSwingingImage);
					break;
				}
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
						m_imgOffsetY = -(m_img.getSize().Y - m_rollHitBox.getOutBox().Height);
						m_imgOffsetX = 0;
						setLayer(0.725f);
					}
					else
					{
						m_imgOffsetX = 0;
						m_imgOffsetY = 0;
						m_collisionShape = m_standHitBox;
						if (m_lastState == State.Rolling || m_lastState == State.Hiding)
						{
							m_position.setLocalY(m_position.getLocalY() - (m_standHitBox.getOutBox().Height - m_rollHitBox.getOutBox().Height) -1);
							Game.getInstance().m_camera.getPosition().plusYWith(m_standHitBox.getOutBox().Height - m_rollHitBox.getOutBox().Height);
							if (m_lastState == State.Hiding)
							{
								setLayer(m_originalLayer);
							}
						}
						else if (m_lastState == State.Hanging)
						{
							m_position.setLocalY(m_position.getLocalY() - (m_standHitBox.getOutBox().Height - m_hangHitBox.getOutBox().Height) - 1);
							Game.getInstance().m_camera.getPosition().plusYWith(m_standHitBox.getOutBox().Height - m_hangHitBox.getOutBox().Height);
						}
					}

				}
				else if ((m_currentState == State.Rolling || (m_currentState == State.Hiding && m_currentHidingImage == DUCKHIDINGIMAGE) || m_currentState == State.Hanging)
					&& m_lastState != State.Rolling && m_lastState != State.Hiding && m_lastState != State.Hanging)
				{
					if (m_currentState == State.Rolling || m_currentState == State.Hiding)
					{
						if (m_currentState == State.Rolling)
						{
							m_img.setAnimationSpeed(15);
							m_img.setLooping(false);
							if (m_facingRight)
								m_imgOffsetX = -m_rollHitBox.getOutBox().Width;
						}
						else if (m_currentState == State.Hiding)
						{
							setLayer(0.725f);
						}
						m_imgOffsetY = -(m_img.getSize().Y - m_rollHitBox.getOutBox().Height);
						m_position.setLocalY(m_position.getLocalY() + (m_standHitBox.getOutBox().Height - m_rollHitBox.getOutBox().Height));
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
					if (m_currentState == State.Hanging)
					{
						m_collisionShape = m_hangHitBox;
					}
					else
					{
						m_collisionShape = m_rollHitBox;
					}
				}
				if (m_lastState == State.Jumping && m_currentState != State.Swinging)
				{
					m_rope = null;
				}
				if (m_currentState == State.Swinging)
				{
					m_rotationPoint.Y = 0;
					m_rotationPoint.X = m_img.getSize().X / 2;
					m_imgOffsetX = -m_img.getSize().X / 2;
				}
				else if (m_lastState == State.Swinging)
				{
					m_imgOffsetX = 0;
					m_rotationPoint.X = m_img.getSize().X / 2;
					m_rotationPoint.Y = m_img.getSize().Y / 2;
				}
			}
		}
		#endregion

		#region collision/hang check
		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			m_collidedWithWall = false;
			m_ladderDirection = 0;
			if (!m_chase && !m_runMode)
			{
				setIsInLight(false);
			}
			if (a_collisionList.Count == 0 && m_collisionShape != null)
			{
		//		m_currentState = State.Jumping;
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
			if (!m_stunned)
			{
				Rectangle t_colliderBox = a_collider.getHitBox().getOutBox();
				Rectangle t_playerBox = getHitBox().getOutBox();
				if (!t_colliderBox.Contains((int)m_lastPosition.X + t_playerBox.Width + 4, (int)m_lastPosition.Y)
					&& t_colliderBox.Contains((int)m_position.getGlobalX() + t_playerBox.Width + 4, (int)m_position.getGlobalY())
					&& m_lastPosition.Y < t_colliderBox.Y
					&& m_speed.Y >= 0
					&& m_currentState == State.Jumping)
				{
					m_position.setLocalY(a_collider.getPosition().getGlobalY());
					m_nextPosition.Y = m_position.getGlobalY();
					m_speed.Y = 0;
					m_speed.X = 0;
					m_currentState = State.Hanging;
					m_facingRight = true;
				}
				else if (!t_colliderBox.Contains((int)m_lastPosition.X - 4, (int)m_lastPosition.Y)
					&& t_colliderBox.Contains((int)m_position.getGlobalX() - 4, (int)m_position.getGlobalY())
					&& m_lastPosition.Y < t_colliderBox.Y
					&& m_speed.Y >= 0
					&& m_currentState == State.Jumping)
				{
					m_position.setLocalY(a_collider.getPosition().getGlobalY());
					m_nextPosition.Y = m_position.getGlobalY();
					m_speed.Y = 0;
					m_speed.X = 0;
					m_currentState = State.Hanging;
					m_facingRight = false;
				}
			}
		}
		#endregion

		#region get/set and other methods

		public void setIsInLight(bool a_isInLight)
		{
			m_isInLight = a_isInLight;
			if (m_isInLight)
			{
				m_color = Color.White;
			}
			else
			{
				m_color = new Color(new Vector3(0.7f, 0.7f, 1f));
			}
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
		public void setVentilationDirection(List<Direction> a_d)
		{
			m_ventilationDirection = a_d;
		}

		public override void draw(GameTime a_gameTime)
		{
			base.draw(a_gameTime);

		}

		public void dealDamageTo(Vector2 a_knockBackForce)
		{
			if (m_invulnerableTimer == 0)
			{
				setSprite("hero_damage");
				//deals 1 damage
				m_currentState = State.Jumping;
				m_health = Math.Max(m_health - 1, 0);
				updateHealthGUI();
				m_stunned = true;
				m_stunnedDeacceleration = true;
				m_speed = a_knockBackForce;

				if (m_health <= 0)
				{
					m_stunnedTimer = 5f;
					m_invulnerableTimer = 5f;

					Game.getInstance().setState(new DeathScene(Game.getInstance().getState().getObjectList()));
				}
				else
				{
					m_stunnedTimer = 0.8f;
					m_invulnerableTimer = 2f;
				}
			}
		}

		private void updateHealthGUI()
		{
			for (int i = 0; i < 3; ++i)
			{
				if (i + 1 <= m_health)
				{
					m_healthHearts[i].setSprite("GameGUI//health");
				}
				else
				{
					m_healthHearts[i].setSprite("GameGUI//no_health");
				}
			}
		}

		public void windowAction()
		{
			if (m_windowActionCD <= 0)
			{
				setSprite("hero_window_heave");
				m_windowActionCD = 0.6f;
				m_collisionShape = null;
				m_img.setLooping(false);
				m_stunned = true;
				m_stunnedTimer = 0.4f;
				m_stunnedDeacceleration = false;
				m_stunnedGravity = false;
				m_stunnedState = State.Hanging;
				m_stunnedFlipSprite = true;
				m_speed.X = 0;
				m_speed.Y = 0;
				
				if (m_currentState == State.Hanging)
				{
					m_imgOffsetY -= m_standHitBox.m_height / 1.8f;
				}
				else
				{
					m_imgOffsetY -= m_rollHitBox.m_height / 4f;
				}
				setNextPositionY(m_position.getGlobalY());


				if (m_facingRight)
				{
					m_imgOffsetX = -4;
					m_imgOffsetX -= m_standHitBox.m_width * 1.9f;
					m_position.plusXWith(m_standHitBox.m_width * 1.9f);
					Game.getInstance().m_camera.getPosition().plusXWith(-m_standHitBox.m_width * 1.9f);
				}
				else
				{
					m_imgOffsetX = 4;
					m_position.plusXWith(-m_standHitBox.m_width * 1.9f);
					Game.getInstance().m_camera.getPosition().plusXWith(m_standHitBox.m_width * 1.9f);
				}
				setNextPositionX(m_position.getGlobalX());

				m_img.setAnimationSpeed(10);


			
				deactivateChaseMode();	
				

			}
		}

		public void hangClimbAction()
		{
			m_collisionShape = null;
			setSprite("hero_climb_ledge");
			m_currentState = State.Stop;
			m_lastState = State.Stop;
			m_stunnedState = State.Stop;
			m_img.setLooping(false);
			m_stunnedGravity = false;
			m_stunned = true;
			m_stunnedTimer = 0.5f;
			m_stunnedDeacceleration = false;
			m_position.plusYWith(-m_standHitBox.m_height);
			m_imgOffsetY = 0;
			setNextPositionY(m_position.getGlobalY());
			Game.getInstance().m_camera.getPosition().plusYWith(m_standHitBox.m_height);
			if (m_facingRight)
			{
				m_position.plusXWith(m_standHitBox.m_width);
				m_imgOffsetX = -m_standHitBox.m_width;
				Game.getInstance().m_camera.getPosition().plusXWith(-m_standHitBox.m_width);
			}
			else
			{
				m_position.plusXWith(-m_standHitBox.m_width);
				Game.getInstance().m_camera.getPosition().plusXWith(m_standHitBox.m_width);
			}
			setNextPositionX(m_position.getGlobalX());
			m_img.setAnimationSpeed(10);
		}

		public bool isStunned()
		{
			return m_stunned;
		}
		public bool isChase()
		{
			return m_chase;
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
		private void updateFlip()
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
		public void setVentilationObject(Entity vent)
		{
			m_currentVentilation = vent;
		}

		internal void setRope(Rope a_rope)
		{
			m_rope = a_rope;
		}
		internal GameObject getRope()
		{
			return m_rope;
		}

		public void activateChaseMode(NPE a_enemy)
		{ 
			float t_eneX = a_enemy.getPosition().getGlobalX();
			float t_eneY = a_enemy.getPosition().getGlobalY();
			float t_diffX = (m_position.getGlobalX() + t_eneX) / 2;
			float t_diffY = (m_position.getGlobalY() + t_eneY) / 2;
			float t_enemyAtentionMarkX = 0;
			if(a_enemy is Guard )
			{
				if (((Guard)a_enemy).isFacingRight())
				{
					t_enemyAtentionMarkX = t_eneX + a_enemy.getHitBox().getOutBox().Width;
				} 
				else 
				{
					t_enemyAtentionMarkX = t_eneX - 10;
				}
			}
			float t_myAtentionMarkX = 0;
			if (m_facingRight)
			{
				t_myAtentionMarkX = m_position.getGlobalX() + m_collisionShape.getOutBox().Width;
			}
			else
			{
				t_myAtentionMarkX = m_position.getGlobalX() - 10;
			}
			string[] t_commands = {"addCinematic"
									,"addParticle:"+t_myAtentionMarkX+":"+(m_position.getGlobalY()-20)+":"+"Images//Sprite//Guard//Exclmarks"+":"+10f+":"+a_enemy.getLayer()
									  ,"addParticle:"+t_enemyAtentionMarkX+":"+(t_eneY-20)+":"+"Images//Sprite//Guard//Exclmarks"+":"+10f+":"+a_enemy.getLayer()
									  ,  "setCamera:"+ t_diffX+":"+ t_diffY+":"+1000};
			Cutscene t_cutScene = new Cutscene(Game.getInstance().getState(), t_commands);
			Game.getInstance().setState(t_cutScene);
			m_chase = true;
			m_playerCurrentSpeed = PLAYERSPEEDCHASEMODE;
			setIsInLight(true);
			
		}
		public void deactivateChaseMode()
		{
			m_deactivateChase = true;
		}

		private void activateNormalMode()
		{
			m_chase = false;
			m_runMode = false;
			m_playerCurrentSpeed = PLAYERSPEED;
			setIsInLight(false);
			((GameState)Game.getInstance().getState()).clearAggro();
		}
		private void activateRunMode()
		{
			m_runMode = true;
			m_playerCurrentSpeed = PLAYERSPEEDCHASEMODE;
			setIsInLight(true);
		}

		public override void changePositionType()
		{
			base.changePositionType();
			Game.getInstance().m_camera.getPosition().setParentPosition(m_position);
			m_standHitBox.setPosition(m_position);
			m_rollHitBox.setPosition(m_position);
			m_SlideBox.setPosition(m_position);
			m_hangHitBox.setPosition(m_position);
		}
		#endregion
	}
}
