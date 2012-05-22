using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
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
		private int m_slideTimer;

		[NonSerialized]
		public GuiObject[] m_healthHearts;

		public const string STANDHIDINGIMAGE = "hero_wallhide";
		public const string DUCKHIDINGIMAGE = "hero_hide";
		public const string VENTIDLEIMAGE = "hero_ventilation_idle";
		private string m_currentHidingImage;
		private string m_currentVentilationImage;
		private string m_currentSwingingImage;
		[NonSerialized]
		private CollisionRectangle m_standHitBox;
		[NonSerialized]
		private CollisionRectangle m_rollHitBox;
		[NonSerialized]
		private CollisionRectangle m_slideBox;
		[NonSerialized]
		private CollisionRectangle m_hangHitBox;
		[NonSerialized]
		private CollisionRectangle m_swingHitBox;

		private State m_currentState = State.Stop;
		private State m_lastState = State.Stop;
		private State m_stunnedState = State.Stop;

		[NonSerialized]
		private bool m_isInLight;
		[NonSerialized]
		private GameObject m_interactionArrow;
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
		private Direction m_lastVentilationDirection;
		private List<Direction> m_leftRightList;
		private List<Direction> m_upDownList;

		private Entity m_currentVentilation = null;

		private bool m_facingRight				= false;
		private bool m_collidedWithWall			= false;
		private bool m_collidedWithVent			= false;
		private bool m_stunned					= false;
		private bool m_stunnedDeacceleration	= true;
		private bool m_stunnedGravity			= true;
		private bool m_stunnedFlipSprite		= false;
		private bool m_chase					= false;
		private bool m_deactivateChase			= false;
		private bool m_runMode					= false;

		private Rope m_rope = null;

		[NonSerialized]
		private Sound m_hitSound1;
		[NonSerialized]
		private Sound m_hitSound2;
		[NonSerialized]
		private Sound m_hitSound3;
		[NonSerialized]
		private Sound m_jumpSound;
		[NonSerialized]
		private Sound m_landSound;
		[NonSerialized]
		private Sound m_hangSound;
		[NonSerialized]
		private Sound m_stepSound;
		[NonSerialized]
		private Sound m_runSound;

		public enum Direction
		{
			None, Left, Right,
			Up, Down
		}

		public enum State
		{
			Stop, Walking, Jumping,
			Slide, Climbing, Rolling,
			Hiding, Hanging, Ventilation,
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
			if (!(Game.getInstance().getState() is DevelopmentState))
			{
				addHearts();
			}
			
			string[] t_heroSprites = Directory.GetFiles("Content//Images//Sprite//Hero//");
			foreach (string t_file in t_heroSprites)
			{
				string[] t_splitFile = Regex.Split(t_file, "//");
				string[] t_extless = t_splitFile[t_splitFile.Length - 1].Split('.');
				if (t_extless[1].Equals("xnb"))
				{
					Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Hero//" + t_extless[0]);
				}
			}

			m_img = new ImageManager("Images//Sprite//Hero//hero_stand");

			m_interactionArrow = new GameObject(new CartesianCoordinate(new Vector2(15, -70), m_position), "Images//GUI//GameGUI//interaction", m_layer - 0.1f, m_listLayer);
			setInteractionVisibility(false);
			m_interactionArrow.getImg().setAnimationSpeed(20f);
			m_standHitBox	= new CollisionRectangle(0, 0, 70, 127, m_position);
			m_rollHitBox	= new CollisionRectangle(0, 0, 70, 72, m_position);
			m_slideBox		= new CollisionRectangle(0, m_standHitBox.getOutBox().Height / 2, m_standHitBox.getOutBox().Width, 1, m_position);
			m_hangHitBox	= new CollisionRectangle(0, 0, 70, 80, m_position);
			m_swingHitBox	= new CollisionRectangle(-36, 0, 70, 127, m_position);
			m_collisionShape = m_standHitBox;
			m_ventilationDirection	= new List<Direction>();
			m_upDownList			= new List<Direction>();
			m_lastVentilationDirection = Direction.None;
			m_upDownList.Add(Direction.Up);
			m_upDownList.Add(Direction.Down);
			m_leftRightList			= new List<Direction>();
			m_leftRightList.Add(Direction.Left);
			m_leftRightList.Add(Direction.Right);
			m_playerCurrentSpeed = PLAYERSPEED;
			m_swingSpeed = 0;
			m_slideTimer = 0;
			m_currentVentilationImage = VENTIDLEIMAGE;
			m_currentSwingingImage = "hero_swing_still";
			m_position.plusYWith(-1);
			m_facingRight = true;

			m_hitSound1 = new Sound("Game//FirstHit");
			m_hitSound2 = new Sound("Game//SecHit");
			m_hitSound3 = new Sound("Game//LethalHit");
			m_jumpSound = new Sound("Game//hopp");
			m_landSound = new Sound("Game//landa2");
			m_hangSound = new Sound("Game/ledgegrab");
			m_stepSound = new Sound("Game//walk");
			m_runSound = new Sound("Game//walk4");

			m_img.m_animationEvent += new ImageManager.animationDelegate(changedSubImage);
		}
		#endregion

		#region update

		private void changedSubImage(float a_from, float a_to)
		{
			if (m_currentState == State.Walking)
			{
				if (m_runMode)
				{
					if ((a_from < 1 && a_to >= 1) ||
						(a_from < 5 && a_to >= 5))
					{
						m_runSound.play();
					}
				}
				else
				{
					if ((a_from < 2 && a_to >= 2) ||
						(a_from < 6 && a_to >= 6))
					{
						m_stepSound.play();
					}
				}
			}
		}

		public override void update(GameTime a_gameTime)
		{
			m_interactionArrow.update(a_gameTime);
			m_lastPosition = m_position.getGlobalCartesian();
			
			float t_deltaTime = ((float)a_gameTime.ElapsedGameTime.Milliseconds) / 1000f;
			m_invulnerableTimer = Math.Max(m_invulnerableTimer - t_deltaTime, 0);
			
			if (!m_stunned)
			{
				changeAnimation();
			}
			
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
				if (!m_chase && !KeyboardHandler.isKeyPressed(GameState.getSprintKey()) && m_runMode)
				{
					toggleRunMode();
				}

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
						updateSliding(a_gameTime);
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
			if ((Game.getInstance().m_camera.getPosition().getLocalCartesian() - m_cameraPoint).Length() > 3)
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
							m_position.plusYWith(m_rollHitBox.m_height / 1.295f - 1);
							Game.getInstance().m_camera.getPosition().plusYWith(-m_rollHitBox.m_height / 1.295f);
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
						m_collisionShape = m_slideBox;
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

			if (KeyboardHandler.keyClicked(GameState.getRollKey()) && m_rollActionCD <= 0)
			{
				m_currentState = State.Rolling;
				return;
			}

			if ((	KeyboardHandler.isKeyPressed(GameState.getLeftKey()) && !KeyboardHandler.isKeyPressed(GameState.getRightKey())) 
				|| (KeyboardHandler.isKeyPressed(GameState.getRightKey()) && !KeyboardHandler.isKeyPressed(GameState.getLeftKey())))
			{
				m_currentState = State.Walking;

				if (KeyboardHandler.isKeyPressed(GameState.getLeftKey()))
				{
					m_facingRight = false;
				}
				else
				{
					m_facingRight = true;
				}
			}

			if (KeyboardHandler.keyClicked(GameState.getJumpKey()))
			{
				m_speed.Y -= JUMPSTRENGTH;
				m_currentState = State.Jumping;
			}
		}

		private void updateWalking(float a_deltaTime)
		{
			if (KeyboardHandler.keyClicked(GameState.getRollKey()) && m_rollActionCD <= 0)
			{
				m_currentState = State.Rolling;
				return;
			}

			if (!m_chase && KeyboardHandler.isKeyPressed(GameState.getSprintKey()) && !m_runMode)
			{
				toggleRunMode();
			}

			if (KeyboardHandler.isKeyPressed(GameState.getRightKey()) && !KeyboardHandler.isKeyPressed(GameState.getLeftKey()))
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
			else if (KeyboardHandler.isKeyPressed(GameState.getLeftKey()) && !KeyboardHandler.isKeyPressed(GameState.getRightKey()))
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
			if (KeyboardHandler.keyClicked(GameState.getJumpKey()))
			{
				m_speed.Y -= JUMPSTRENGTH;
				m_currentState = State.Jumping;
				m_jumpSound.play();
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
			if (KeyboardHandler.keyClicked(GameState.getRollKey()) && Game.getInstance().getProgress().hasEquipment("hookshot"))
			{
				Hookshot t_hs = new Hookshot(m_position.getGlobalCartesian(), null, 0.100f);
				if (m_facingRight)
				{
					t_hs.setDirection(new Vector2(100, -100));
				}
				else
				{
					t_hs.setDirection(new Vector2(-100, -100));
				}
				Game.getInstance().getState().addObject(t_hs);

			}
			if (!KeyboardHandler.isKeyPressed(GameState.getLeftKey()) && !KeyboardHandler.isKeyPressed(GameState.getRightKey()))
			{
				if (m_speed.X > 0)
				{
					m_speed.X = Math.Max(m_speed.X - (AIRDEACCELERATION * a_deltaTime), 0);
				}
				else if (m_speed.X < 0)
				{
					m_speed.X = Math.Min(m_speed.X + (AIRDEACCELERATION * a_deltaTime), 0);
				}
			}
			else if (KeyboardHandler.isKeyPressed(GameState.getLeftKey()))
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
			else if (KeyboardHandler.isKeyPressed(GameState.getRightKey()))
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

			if (m_speed.Y < -300 && KeyboardHandler.isKeyPressed(GameState.getJumpKey()))
			{
				m_speed.Y -= AIRVERTICALACCELERATION;
			}

			m_cameraPoint.X = Math.Max(Math.Min(m_cameraPoint.X + (m_speed.X * 1.5f * a_deltaTime), CAMERAMAXDISTANCE), -CAMERAMAXDISTANCE);
		}

		private void updateSliding(GameTime a_gameTime)
		{
			if (m_slideTimer < a_gameTime.TotalGameTime.TotalMilliseconds && m_slideTimer != 0)
			{
				m_currentState = State.Jumping;
				m_slideTimer = 0;
				return;
			}
			m_imgOffsetX = 0;
			if (((!m_facingRight && KeyboardHandler.isKeyPressed(GameState.getRightKey())) || (m_facingRight && KeyboardHandler.isKeyPressed(GameState.getLeftKey())))
				&& m_collidedWithWall)
			{
				m_slideTimer = 0;
			}
			else
			{
				if (m_slideTimer == 0)
					m_slideTimer = (int)a_gameTime.TotalGameTime.TotalMilliseconds + 150;
			}
			if (KeyboardHandler.keyClicked(GameState.getJumpKey()) && m_speed.Y != 0)
			{
				m_speed.Y = -JUMPSTRENGTH;
				if (m_facingRight == true)
					m_speed.X += JUMPSTRENGTH;
				else
					m_speed.X -= JUMPSTRENGTH;
				m_currentState = State.Jumping;
				m_slideTimer = 0;
			}
			else if (m_speed.Y > SLIDESPEED)
				m_speed.Y = SLIDESPEED;
		}

		private void updateClimbing()
		{
			m_gravity = 0;
			if (KeyboardHandler.isKeyPressed(GameState.getUpKey()))
			{
				m_speed.Y = -CLIMBINGSPEED;
			}
			else if (KeyboardHandler.isKeyPressed(GameState.getDownKey()))
			{
				m_speed.Y = CLIMBINGSPEED;
			}
			else
			{
				m_speed.Y = 0;
			}
			if (KeyboardHandler.keyClicked(GameState.getJumpKey()))
			{
				if (!KeyboardHandler.isKeyPressed(GameState.getDownKey()))
				{
					m_speed.Y = -(JUMPSTRENGTH - 70);
					if (m_facingRight)
					{
						m_facingRight = false;
						m_speed.X -= PLAYERSPEEDCHASEMODE * 1.5f;
					}
					else
					{
						m_facingRight = true;
						m_speed.X += PLAYERSPEEDCHASEMODE * 1.5f;
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
				if (m_speed.Y <= 0 && m_currentState == State.Climbing)
				{
					m_speed.Y = 0;
				}
				else
				{
					m_currentState = State.Jumping;
					m_nextPosition.Y = m_position.getGlobalY() + 1;
				}
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
			if (KeyboardHandler.keyClicked(GameState.getJumpKey()))
			{
				if (KeyboardHandler.isKeyPressed(GameState.getDownKey()))
				{
					m_position.plusYWith(m_collisionShape.getOutBox().Height);
					Game.getInstance().m_camera.getPosition().plusYWith(-m_collisionShape.getOutBox().Height);
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
					m_speed.Y = -(JUMPSTRENGTH - 70);
					if (!m_facingRight)
					{
						m_speed.X += PLAYERSPEEDCHASEMODE * 1.5f;
					}
					else
					{
						m_speed.X -= PLAYERSPEEDCHASEMODE * 1.5f;
					}
					m_currentState = State.Jumping;
				}
			}
			else if (KeyboardHandler.isKeyPressed(GameState.getDownKey()) && m_ladderDirection != Direction.None)
			{
				m_currentState = State.Climbing;
				m_position.plusYWith(m_standHitBox.m_height - m_hangHitBox.m_height);
				Game.getInstance().m_camera.getPosition().plusYWith(-(m_standHitBox.m_height - m_hangHitBox.m_height));
			}
		}

		private void updateHiding(float a_deltaTime)
		{
			if (KeyboardHandler.keyClicked(GameState.getUpKey())
				|| KeyboardHandler.keyClicked(GameState.getDownKey())
				|| KeyboardHandler.keyClicked(GameState.getJumpKey())
				|| KeyboardHandler.keyClicked(GameState.getActionKey()))
			{
				m_currentState = State.Stop;
			}

			if (KeyboardHandler.isKeyPressed(GameState.getLeftKey()) || KeyboardHandler.isKeyPressed(GameState.getRightKey()))
			{
				float t_cameraXPos = 0;
				if (KeyboardHandler.isKeyPressed(GameState.getRightKey()))
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
			m_img.setAnimationSpeed(15);
			m_speed = Vector2.Zero;
			m_gravity = 0;
			if (Game.getInstance().m_camera.getLayer() == 0)
			{

				if (!m_collidedWithVent)
				{
					Game.getInstance().m_camera.setLayer(1);
					m_layer = 0.6995f;
				}
				else
				{ 
					m_layer = 0.901f;
				}
				m_cameraPoint.X = 0;
			}
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

		private List<Direction> moveDirectionInVentilation(Direction a_direction)
		{
			bool t_idle = true;
			List<Direction> t_list = null;
			switch (a_direction)
			{
				case Direction.Up:
				{
					t_idle = false;
					m_currentVentilationImage = "hero_ventilation_vertical";
					if (((CollisionRectangle)m_collisionShape).m_xOffset != 0 && !(m_lastVentilationDirection == Direction.Right && KeyboardHandler.isKeyPressed(GameState.getRightKey())))
					{
						setSprite(m_currentVentilationImage);
						m_position.plusXWith(((CollisionRectangle)m_collisionShape).m_xOffset);
						Game.getInstance().m_camera.getPosition().plusXWith(-((CollisionRectangle)m_collisionShape).m_xOffset);
						((CollisionRectangle)m_collisionShape).setOffsetX(0);
					}
					if (KeyboardHandler.isKeyPressed(GameState.getUpKey()))
					{
						((CollisionRectangle)m_collisionShape).setOffsetY(0);
						((CollisionRectangle)m_collisionShape).setOffsetX(0);
						m_speed.Y = -PLAYERSPEED;
						t_list = m_upDownList;
						m_facingRight = false;
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
			//					m_imgOffsetX = -((m_img.getSize().X / 2) - (m_currentVentilation.getImg().getSize().X / 2));
								m_currentVentilation = null;
							}
						}
							if (m_lastVentilationDirection == Direction.Left || m_lastVentilationDirection == Direction.Right || m_lastVentilationDirection == Direction.None)
							{
								m_position.plusYWith(-36);
								Game.getInstance().m_camera.getPosition().plusYWith(36);
							}
							m_lastVentilationDirection = Direction.Up;
					}
					else if (!KeyboardHandler.isKeyPressed(GameState.getDownKey()))
					{
						m_img.setAnimationSpeed(0);
						m_lastVentilationDirection = Direction.Up;
					}
					break;
				}
				case Direction.Left:
				{
					if (((CollisionRectangle)m_collisionShape).m_yOffset != 0 && !(m_lastVentilationDirection == Direction.Down && KeyboardHandler.isKeyPressed(GameState.getDownKey())))
					{
						setSprite(m_currentVentilationImage);
						m_position.plusYWith(((CollisionRectangle)m_collisionShape).m_yOffset);
						Game.getInstance().m_camera.getPosition().plusYWith(-((CollisionRectangle)m_collisionShape).m_yOffset);
						((CollisionRectangle)m_collisionShape).setOffsetY(0);
					}
					if (KeyboardHandler.isKeyPressed(GameState.getLeftKey()) && !KeyboardHandler.isKeyPressed(GameState.getRightKey()))
					{
						((CollisionRectangle)m_collisionShape).setOffsetX(0);
						((CollisionRectangle)m_collisionShape).setOffsetY(0);
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
			//					m_imgOffsetY = -((m_img.getSize().Y / 2) - (m_currentVentilation.getImg().getSize().Y / 2));
								m_currentVentilation = null;
							}
						}
						m_currentVentilationImage = "hero_ventilation_horizontal";
						t_idle = false;
						m_facingRight = false;
						if (m_lastVentilationDirection == Direction.Down || m_lastVentilationDirection == Direction.Up)
						{
							m_position.plusXWith(-36);
							Game.getInstance().m_camera.getPosition().plusXWith(36);
						}
						m_lastVentilationDirection = Direction.Left;
					}
					break;
				}
				case Direction.Right:
				{
					if (((CollisionRectangle)m_collisionShape).m_yOffset != 0 && !(m_lastVentilationDirection == Direction.Down && KeyboardHandler.isKeyPressed(GameState.getDownKey())))
					{
						setSprite(m_currentVentilationImage);
						m_position.plusYWith(((CollisionRectangle)m_collisionShape).m_yOffset);
						Game.getInstance().m_camera.getPosition().plusYWith(-((CollisionRectangle)m_collisionShape).m_yOffset);
						((CollisionRectangle)m_collisionShape).setOffsetY(0);
					}
					if (KeyboardHandler.isKeyPressed(GameState.getRightKey()) && !KeyboardHandler.isKeyPressed(GameState.getLeftKey()))
					{
						((CollisionRectangle)m_collisionShape).setOffsetX(36);
						((CollisionRectangle)m_collisionShape).setOffsetY(0);
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
				//				m_imgOffsetY = -((m_img.getSize().Y / 2) - (m_currentVentilation.getImg().getSize().Y / 2));
								m_currentVentilation = null;
							}
						}
						m_currentVentilationImage = "hero_ventilation_horizontal";
						t_idle = false;
						m_facingRight = true;
					}
					break;
				}
				case Direction.Down:
				{
					t_idle = false;
					m_currentVentilationImage = "hero_ventilation_vertical";
					if (((CollisionRectangle)m_collisionShape).m_xOffset != 0 && !(m_lastVentilationDirection == Direction.Right && KeyboardHandler.isKeyPressed(GameState.getRightKey())))
					{
						setSprite(m_currentVentilationImage);
						m_position.plusXWith(((CollisionRectangle)m_collisionShape).m_xOffset);
						Game.getInstance().m_camera.getPosition().plusXWith(-((CollisionRectangle)m_collisionShape).m_xOffset);
						((CollisionRectangle)m_collisionShape).setOffsetX(0);
					}
					if (KeyboardHandler.isKeyPressed(GameState.getDownKey()))
					{
						((CollisionRectangle)m_collisionShape).setOffsetY(36);
						((CollisionRectangle)m_collisionShape).setOffsetX(0);
						m_speed.Y = PLAYERSPEED;
						t_list = m_upDownList;
						m_facingRight = false;
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
				//				m_imgOffsetX = -((m_img.getSize().X / 2) - (m_currentVentilation.getImg().getSize().X / 2));
								m_currentVentilation = null;
							}
						}
					}
					else if (!KeyboardHandler.isKeyPressed(GameState.getUpKey()))
					{
						m_img.setAnimationSpeed(0);
						m_lastVentilationDirection = Direction.Down;
					}
					break;
				}
			}
			if (t_idle)
			{
				m_currentVentilationImage = "hero_ventilation_idle";
				m_lastVentilationDirection = Direction.None;
				if (((CollisionRectangle)m_collisionShape).m_yOffset != 0)
				{
					setSprite(m_currentVentilationImage);
					m_position.plusYWith(((CollisionRectangle)m_collisionShape).m_yOffset);
					Game.getInstance().m_camera.getPosition().plusYWith(-((CollisionRectangle)m_collisionShape).m_yOffset);
					((CollisionRectangle)m_collisionShape).setOffsetY(0);
				}
			}
			return t_list;
		}

		private void updateSwinging(float a_deltaTime)
		{
			m_gravity = 0;
			if (m_position.getLength() > 0)
			{
				if (KeyboardHandler.isKeyPressed(GameState.getRightKey()))
				{
					if ((m_swingSpeed > -MAXSWINGSPEED && !((m_rotate > Math.PI && m_rotate != 0) && m_swingSpeed <= 0 && m_swingSpeed >= -0.01f)) || (m_swingSpeed == 0 && m_rotate == Math.PI))
					{
						m_swingSpeed -= (500 * a_deltaTime) / m_position.getLength();
					}
				}
				else if (KeyboardHandler.isKeyPressed(GameState.getLeftKey()))
				{
					if ((m_swingSpeed < MAXSWINGSPEED && !(m_rotate < Math.PI && m_swingSpeed >= 0 && m_swingSpeed <= 0.01f)) || (m_swingSpeed == 0 && m_rotate == Math.PI))
					{
						m_swingSpeed += (500 * a_deltaTime) / m_position.getLength();
					}
				}
				else if (KeyboardHandler.isKeyPressed(GameState.getUpKey()))
				{
					m_position.setLength(Math.Max(m_position.getLength() - (200f * a_deltaTime), 50));
				}
				else if (KeyboardHandler.isKeyPressed(GameState.getDownKey()))
				{
					m_position.setLength(Math.Min(m_position.getLength() + (200f * a_deltaTime), m_rope.getLength()));
				}
			}
			

			m_swingSpeed += (float)((Math.Cos(m_rope.getRotation()) * 3000 * a_deltaTime) / m_position.getLength());
			m_swingSpeed *= 0.99f;
			m_rope.addRotation(m_swingSpeed * a_deltaTime);
			m_rotate = (m_rope.getRotation() - ((float)(Math.PI / 2.0))) % ((float)(Math.PI * 2.0));
			m_position.setSlope(m_rope.getRotation());
			if (m_swingSpeed > 1f)
			{
				if (!m_facingRight)
				{
					m_currentSwingingImage = "hero_swing_back";
				}
				else
				{
					m_currentSwingingImage = "hero_swing_forth";
				}
			}
			else if (m_swingSpeed < -1f)
			{
				if (m_facingRight)
				{
					m_currentSwingingImage = "hero_swing_back";
				}
				else
				{
					m_currentSwingingImage = "hero_swing_forth";
				}
			}
			else
			{
				m_currentSwingingImage = "hero_swing_still";
			}

			if (KeyboardHandler.keyClicked(GameState.getJumpKey()))
			{
				float t_force = m_swingSpeed * (m_position.getLength() + (m_img.getSize().Y / 2));
				Vector2 t_inputForce = new Vector2();
				if (KeyboardHandler.isKeyPressed(GameState.getUpKey()))
				{
					t_inputForce.Y -= 200;
				}
				if (KeyboardHandler.isKeyPressed(GameState.getDownKey()))
				{
					t_inputForce.Y += 200;
				}
				if (KeyboardHandler.isKeyPressed(GameState.getRightKey()))
				{
					t_inputForce.X += 200;
				}
				if (KeyboardHandler.isKeyPressed(GameState.getLeftKey()))
				{
					t_inputForce.X -= 200;
				}
				m_speed = new Vector2(t_force * (float)Math.Cos(m_rotate + Math.PI), t_force * (float)Math.Sin(m_rotate + Math.PI)) + t_inputForce;
				m_currentState = State.Jumping;
			}
		}
		#endregion

		#region change animation and state
		private void setSprite(string a_sprite)
		{
			
			if (m_isInLight)
			{
				m_img.setSprite("Images//Sprite//Hero//" + a_sprite + "_light");
			}
			else
			{
				m_img.setSprite("Images//Sprite//Hero//" + a_sprite);
			}
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
					{
						setSprite("hero_run");
					}
					else
					{
						setSprite("hero_walk");
					}
					break;
				}
				case State.Jumping:
				{
					if (m_speed.Y < 0)
					{
						setSprite("hero_jump");
					}
					else
					{
						setSprite("hero_fall");
					}
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
					if (m_speed.Y < 0)
					{
						setSprite("ladderclimb");
					}
					else if (m_speed.Y > 0)
					{
						setSprite("ladderclimbdown");
					}
					else
					{
						setSprite("hero_climb");
					}
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
					if (m_currentVentilation != null)
					{
		//				m_imgOffsetX = -((m_img.getSize().X / 2) - (m_currentVentilation.getImg().getSize().X / 2));
		//				m_imgOffsetY = -((m_img.getSize().Y / 2) - (m_currentVentilation.getImg().getSize().Y / 2));
					}
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
				if (!m_chase && m_runMode && (m_currentState != State.Walking && m_currentState != State.Jumping))
				{
					toggleRunMode();
				}

				if ((m_lastState == State.Rolling || m_lastState == State.Hiding || m_lastState == State.Hanging)
					&& m_currentState != State.Rolling && m_currentState != State.Hanging)
				{
					if (m_currentState == State.Hiding && m_currentHidingImage == DUCKHIDINGIMAGE)
					{
						m_imgOffsetY = -(m_img.getSize().Y - m_rollHitBox.getOutBox().Height);
						m_imgOffsetX = 0;
						setLayer(0.725f);

					} 
					else if(m_lastState == State.Hiding && m_currentHidingImage == STANDHIDINGIMAGE)
					{
						if (m_facingRight)
						{
							m_position.plusXWith(40);
							Game.getInstance().m_camera.getPosition().plusXWith(-40);
						}
						else
						{
							m_position.plusXWith(-40);
							Game.getInstance().m_camera.getPosition().plusXWith(40);
						}
					}
					else
					{
						m_imgOffsetX = 0;
						m_imgOffsetY = 0;
						m_collisionShape = m_standHitBox;
						if (m_lastState == State.Rolling || m_lastState == State.Hiding)
						{
							m_position.setLocalY(m_position.getLocalY() - (m_standHitBox.getOutBox().Height - m_rollHitBox.getOutBox().Height) - 1);
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
				else if (m_currentState == State.Climbing)
				{
					m_img.setAnimationSpeed(15);
				}
				if (m_lastState != State.Swinging && m_currentState != State.Swinging)
				{
					if (m_rope != null && !m_rope.getHitBox().collides(m_collisionShape))
						m_rope = null;
				}
				if (m_currentState == State.Swinging)
				{
					m_rotationPoint.Y = 0;
					m_rotationPoint.X = m_img.getSize().X / 2;
					m_imgOffsetX = -m_img.getSize().X / 2;
					m_collisionShape = m_swingHitBox;
				}
				else if (m_lastState == State.Swinging)
				{
					m_rotationPoint.X = m_img.getSize().X / 2;
					m_rotationPoint.Y = m_img.getSize().Y / 2;
					changePositionToCartesian();
					m_position.setParentPositionWithoutMoving(null);
					m_rotate = 0;
					m_rope.resetPosition();
					m_swingSpeed = 0;
					if (m_currentState != State.Ventilation)
					{
						m_imgOffsetX = 0;
						m_position.setGlobalX(m_position.getGlobalX() - 36);
						Game.getInstance().m_camera.getPosition().setGlobalX(Game.getInstance().m_camera.getPosition().getGlobalX() + 36);
						m_collisionShape = m_standHitBox;
					}
				}
				if (m_lastState == State.Ventilation)
				{
					m_imgOffsetX = 0;
					m_imgOffsetY = 0;
					m_layer = 0.250f;
					((CollisionRectangle)m_collisionShape).m_xOffset = 0;
					((CollisionRectangle)m_collisionShape).m_yOffset = 0;
				}
				else if (m_currentState == State.Ventilation)
				{
					m_img.setAnimationSpeed(15);
					m_layer = 0.6995f;
					if (m_currentVentilation != null)
					{
						m_imgOffsetX = -((m_img.getSize().X / 2) - (m_currentVentilation.getImg().getSize().X / 2));
						m_imgOffsetY = -((m_img.getSize().Y / 2) - (m_currentVentilation.getImg().getSize().Y / 2));
					}
					else
					{
						m_imgOffsetX = -((m_img.getSize().X / 2) - 36);
						m_imgOffsetY = -((m_img.getSize().Y / 2) - 36);
					}
				}
			}
		}
		#endregion

		#region collision/hang check
		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			m_collidedWithVent = false;
			m_collidedWithWall = false;
			m_ladderDirection = 0;
			if (!m_chase)
			{
				setIsInLight(false);
			}
			setInteractionVisibility(false);
			if (a_collisionList.Count != 0)
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
					m_imgOffsetX = 7;
				}
				else
				{
					m_facingRight = true;
					m_imgOffsetX = -7;
				}
			}
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
					&& (m_currentState == State.Jumping || m_currentState == State.Slide))
				{
					m_position.setLocalY(a_collider.getPosition().getGlobalY());
					m_nextPosition.Y = m_position.getGlobalY();
					m_speed.Y = 0;
					m_speed.X = 0;
					m_currentState = State.Hanging;
					m_facingRight = true;
					m_hangSound.play();
				}
				else if (!t_colliderBox.Contains((int)m_lastPosition.X - 4, (int)m_lastPosition.Y)
					&& t_colliderBox.Contains((int)m_position.getGlobalX() - 4, (int)m_position.getGlobalY())
					&& m_lastPosition.Y < t_colliderBox.Y
					&& m_speed.Y >= 0
					&& (m_currentState == State.Jumping || m_currentState == State.Slide))
				{
					m_position.setLocalY(a_collider.getPosition().getGlobalY());
					m_nextPosition.Y = m_position.getGlobalY();
					m_speed.Y = 0;
					m_speed.X = 0;
					m_currentState = State.Hanging;
					m_facingRight = false;
					m_hangSound.play();
				}
			}
		}
		#endregion

		#region get/set and other methods
		private void addHearts()
		{
			m_health = 3;
			m_healthHearts = new GuiObject[3];
			m_healthHearts[0] = new GuiObject(new Vector2(100, 50), "GameGUI//health");
			Game.getInstance().getState().addGuiObject(m_healthHearts[0]);
			m_healthHearts[1] = new GuiObject(new Vector2(180, 50), "GameGUI//health");
			Game.getInstance().getState().addGuiObject(m_healthHearts[1]);
			m_healthHearts[2] = new GuiObject(new Vector2(260, 50), "GameGUI//health");
			Game.getInstance().getState().addGuiObject(m_healthHearts[2]);
		}
		public void setInteractionVisibility(bool a_show)
		{
			m_interactionArrow.setVisible(a_show);
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

		public void setIsInLight(bool a_isInLight)
		{
			
			if (m_isInLight && !a_isInLight && m_img.getImagePath().EndsWith("_light"))
			{
				String t_oldName = m_img.getImagePath();
				m_img.setSpriteSilently(t_oldName.Remove(t_oldName.Length - 6));
			}
			else if (!m_isInLight && a_isInLight)
			{
				m_img.setSpriteSilently(m_img.getImagePath() + "_light");
			}
				
			m_isInLight = a_isInLight;
		}

		public void setCollidedWithWall(bool a_collided)
		{
			m_collidedWithWall = a_collided;
		}
		public void setCollidedWithVent(bool a_collided)
		{
			m_collidedWithVent = a_collided;
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
			return m_slideBox;
		}
		public void setVentilationDirection(List<Direction> a_d)
		{
			m_ventilationDirection = a_d;
		}

		public override void draw(GameTime a_gameTime)
		{
			base.draw(a_gameTime);
			m_interactionArrow.draw(a_gameTime);
		}

		public void dealDamageTo(Vector2 a_knockBackForce)
		{
			if (m_invulnerableTimer == 0)
			{
				setSprite("hero_damage");
				m_currentState = State.Jumping;

				switch (m_health)
					{
						case 3:
							{
								m_hitSound1.play();
								break;
							}
						case 2:
							{
								m_hitSound2.play();
								break;
							}
						case 1:
							{
								m_hitSound3.play();
								break;
							}
					}

				m_health = Math.Max(m_health - 1, 0);
				updateHealthGUI();
				m_stunned = true;
				m_stunnedDeacceleration = true;
				m_speed = a_knockBackForce;

				if (m_health <= 0)
				{
					m_stunnedTimer = 5f;
					m_invulnerableTimer = 5f;

					Game.getInstance().setState(new DeathScene(Game.getInstance().getState().getObjectList(), ((GameState)Game.getInstance().getState()).getBackground()));
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

		public void heal(int a_amount)
		{
			if (a_amount < 0)
			{
				ErrorLogger.getInstance().writeString("Player can not be healed a negative amount, skipped");
			}
			else
			{
				m_health = (int)Math.Min(m_health + a_amount, 3);
				updateHealthGUI();
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

				if (m_chase)
				{
					deactivateChaseMode();
				}
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
			if (a_state == State.Swinging)
			{
				//Den här raden kan se konstig ut. Men det ska fungera
				//Prata med Anton för mer info
				m_swingSpeed = (float)(-(m_speed.X * Math.Sin(m_position.getSlope()) - m_speed.Y * Math.Cos(m_position.getSlope())) / m_position.getLength());

				m_speed = Vector2.Zero;
			}
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
			if(!m_chase)
			{
				float t_eneX = a_enemy.getPosition().getGlobalX();
				float t_eneY = a_enemy.getPosition().getGlobalY();
				float t_diffX = (m_position.getGlobalX() + t_eneX) / 2;
				float t_diffY = (m_position.getGlobalY() + t_eneY) / 2;
				float t_enemyAttentionMarkX = 0;
				if (a_enemy is Guard)
				{
					if (((Guard)a_enemy).isFacingRight())
					{
						t_enemyAttentionMarkX = t_eneX + a_enemy.getHitBox().getOutBox().Width;
					}
					else
					{
						t_enemyAttentionMarkX = t_eneX - 10;
					}
				}
				float t_myAttentionMarkX = 0;
				if (m_facingRight)
				{
					t_myAttentionMarkX = m_position.getGlobalX() + m_collisionShape.getOutBox().Width;
				}
				else
				{
					t_myAttentionMarkX = m_position.getGlobalX() - 10;
				}
				string[] t_commands = { "addCinematic"
										, "addParticle:" + t_myAttentionMarkX + ":" + (m_position.getGlobalY() - 20) + ":" + "Images//Sprite//Guard//Exclmarks" + ":" + 10f + ":" + a_enemy.getLayer()
										, "addParticle:" + t_enemyAttentionMarkX + ":" + (t_eneY - 20) + ":" + "Images//Sprite//Guard//Exclmarks" + ":" + 10f + ":" + a_enemy.getLayer()
										, "setCamera:" + t_diffX+":" + t_diffY + ":" + 1000 };
				Cutscene t_cutScene = new Cutscene(Game.getInstance().getState(), t_commands);
				Game.getInstance().setState(t_cutScene);
				m_chase = true;
				Music.getInstance().play("ChaseSongIntro","ChaseSongLoop");
				m_runMode = false;
				m_playerCurrentSpeed = PLAYERSPEEDCHASEMODE;
				setIsInLight(true);
			}
		}

		public void deactivateChaseMode()
		{
			if(m_chase)
			{
				m_deactivateChase = true;
				Music.getInstance().play("StageSong");
			}
		}

		private void activateNormalMode()
		{
			m_chase = false;
			m_runMode = false;
			m_playerCurrentSpeed = PLAYERSPEED;
			setIsInLight(false);
			((GameState)Game.getInstance().getState()).clearAggro();
		}
		private void toggleRunMode()
		{
			m_runMode = !m_runMode;
			if (m_runMode)
				m_playerCurrentSpeed = PLAYERSPEEDCHASEMODE;
			else
				m_playerCurrentSpeed = PLAYERSPEED;
		}
		public bool isRunMode()
		{
			return m_runMode;
		}

		public void setSwingSpeed(float a_speed)
		{
			m_swingSpeed = a_speed;
		}
		public override void changePositionType()
		{
			base.changePositionType();
			Game.getInstance().m_camera.getPosition().setParentPositionWithoutMoving(m_position);
			m_interactionArrow.getPosition().setParentPosition(m_position);
			m_standHitBox.setPosition(m_position);
			m_rollHitBox.setPosition(m_position);
			m_slideBox.setPosition(m_position);
			m_hangHitBox.setPosition(m_position);
			m_swingHitBox.setPosition(m_position);
		}
		#endregion
	}
}
