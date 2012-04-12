using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GrandLarceny.AI;

namespace GrandLarceny
{
	[Serializable()]
	public class Guard : NPE
	{
        private float m_leftPatrolPoint;
        private float m_rightPatrolPoint;
        private Boolean m_hasPatrol;
        private Boolean m_hasFlashLight;
		private Boolean m_guardFaceRight;
		[NonSerialized]
		private LinkedList<LampSwitch> m_lampSwitchTargets;
		private LinkedList<int> m_lampSwitchTargetsId;

        private const float MOVEMENTSPEED = 150;
		private const float CHASINGSPEED = 450;
		private const float WALKINGANIMATIONSPEED = MOVEMENTSPEED / 16;
		private const float CHASINGANIMATIONSPEED = CHASINGSPEED / 25;
		private const float TURNANIMATIONSPEED = 10f;
		private const float TURNQUICKANIMATIONSPEED = 50f;

		[NonSerialized]
        private Entity m_chaseTarget = null;
		private Boolean m_running = false;
		private Boolean m_facingRight;

		private float m_sightRange = 670f;

		[NonSerialized]
		private FlashCone m_flashLight;
		private int m_flashLightId;

		[NonSerialized]
		private float m_strikeReloadTime = 0;
		[NonSerialized]
		private bool m_striking;

		#region Guard Textures
		[NonSerialized]
		private Texture2D t2d_run;
		[NonSerialized]
		private Texture2D t2d_walk;
		[NonSerialized]
		private Texture2D t2d_flashWalk;
		[NonSerialized]
		private Texture2D t2d_flashIdle;
		[NonSerialized]
		private Texture2D t2d_flashTurn;
		[NonSerialized]
		private Texture2D t2d_idle;
		[NonSerialized]
		private Texture2D t2d_pickUpFlash;
		[NonSerialized]
		private Texture2D t2d_putDownFlash;
		[NonSerialized]
		private Texture2D t2d_strike;
		[NonSerialized]
		private Texture2D t2d_turn;
		[NonSerialized]
		private Texture2D t2d_qmark;
		[NonSerialized]
		private Texture2D t2d_emark;
		#endregion

		//flashlight addicted guard always has their flashlight up
		private Boolean m_FlashLightAddicted;

		public Guard(Vector2 a_posV2, String a_sprite, float a_leftpatrolPoint, float a_rightpatrolPoint, Boolean a_hasFlashLight, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			m_leftPatrolPoint = a_leftpatrolPoint;
			m_rightPatrolPoint = a_rightpatrolPoint;
			m_hasPatrol = (m_leftPatrolPoint != m_rightPatrolPoint);
			m_hasFlashLight = a_hasFlashLight;
			m_FlashLightAddicted = a_sprite == "Images//Sprite//Guard//guard_flash_idle";
			m_aiState = AIStatepatroling.getInstance();
			if (m_hasFlashLight && m_FlashLightAddicted)
			{
				m_flashLight = new FlashCone(this, new Vector2(0,-7), "Images//LightCone//light_guard_idle",m_facingRight, 0.249f);
				Game.getInstance().getState().addObject(m_flashLight);
				m_flashLightId = m_flashLight.getId();
			}
			m_gravity = 1000;
			m_guardFaceRight = m_facingRight;
		}
        public Guard(Vector2 a_posV2, String a_sprite, float a_patrolPoint, Boolean a_hasFlashLight, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
            m_hasPatrol = false;
            m_hasFlashLight = a_hasFlashLight;
            m_leftPatrolPoint = a_patrolPoint;
            m_rightPatrolPoint = a_patrolPoint;
			m_aiState = AIStatepatroling.getInstance();
			m_FlashLightAddicted = a_sprite == "Images//Sprite//Guard//guard_flash_idle";
			if (m_hasFlashLight && m_FlashLightAddicted)
			{
				m_flashLight = new FlashCone(this, new Vector2(0, -7), "Images//LightCone//light_guard_idle", m_facingRight, 0.249f);
				Game.getInstance().getState().addObject(m_flashLight);
				m_flashLightId = m_flashLight.getId();
			}
			m_gravity = 1000;
			m_guardFaceRight = m_facingRight;
		}

		public override void linkObject()
		{
			base.linkObject();
			m_lampSwitchTargetsId = new LinkedList<int>();
			foreach (LampSwitch t_ls in m_lampSwitchTargets)
			{
				m_lampSwitchTargetsId.AddLast(t_ls.getId());
			}
			if (m_flashLight != null)
			{
				m_flashLightId = m_flashLight.getId();
			}
		}

		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionRectangle(20, 10, m_img.getSize().X - 40, m_img.getSize().Y - 10, m_position);
			m_lampSwitchTargets = new LinkedList<LampSwitch>();
			if (m_lampSwitchTargetsId == null)
			{
				m_lampSwitchTargetsId = new LinkedList<int>();
			}
			foreach (int t_lsti in m_lampSwitchTargetsId)
			{
				m_lampSwitchTargets.AddLast((LampSwitch)Game.getInstance().getState().getObjectById(t_lsti));
			}
			if (m_flashLightId > 0)
			{
				m_flashLight = (FlashCone)Game.getInstance().getState().getObjectById(m_flashLightId);
				m_flashLight.getPosition().setParentPosition(m_position);
			}
			m_facingRight = m_spriteEffects == SpriteEffects.None;

			#region Texture Loading
			t2d_run				= Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Guard//guard_run");
			t2d_walk			= Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Guard//guard_walk");
			t2d_flashWalk		= Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Guard//guard_flash_walk");
			t2d_flashIdle		= Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Guard//guard_flash_idle");
			t2d_flashTurn		= Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Guard//guard_flash_turn");
			t2d_idle			= Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Guard//guard_idle");
			t2d_pickUpFlash		= Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Guard//guard_pick_up_flash");
			t2d_putDownFlash	= Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Guard//guard_put_down_flash");
			t2d_strike			= Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Guard//guard_strike");
			t2d_turn			= Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Guard//guard_turn");
			t2d_qmark			= Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Guard//qmark");
			t2d_emark			= Game.getInstance().Content.Load<Texture2D>("Images//Sprite//Guard//Exclmarks");
			#endregion
		}

		public void setLeftGuardPoint(float a_x)
		{
			m_leftPatrolPoint = a_x;
			m_hasPatrol = (m_leftPatrolPoint != m_rightPatrolPoint);
		}

		public void setRightGuardPoint(float a_x)
		{
			m_rightPatrolPoint = a_x;
			m_hasPatrol = (m_leftPatrolPoint != m_rightPatrolPoint);
		}
		public void setGuardPoint(float a_x)
		{
			m_hasPatrol = false;
			m_leftPatrolPoint = a_x;
			m_rightPatrolPoint = a_x;
		}
		internal void goRight()
		{
			if (m_aiActive)
			{
				if (m_facingRight)
				{
					if (m_speed.X == 0)
					{
						if (m_flashLight == null)
						{
							if (m_running)
							{
								m_speed.X = CHASINGSPEED;
								m_img.setAnimationSpeed(CHASINGANIMATIONSPEED);
								m_img.setSprite("Images//Sprite//Guard//guard_run");
							}
							else
							{
								m_speed.X = MOVEMENTSPEED;
								m_img.setAnimationSpeed(WALKINGANIMATIONSPEED);
								m_img.setSprite("Images//Sprite//Guard//guard_walk");
							}
						}
						else
						{
							if (m_running)
							{
								m_flashLight.kill();
								m_flashLightId = 0;
								m_flashLight = null;
								m_speed.X = CHASINGSPEED;
								m_img.setAnimationSpeed(CHASINGANIMATIONSPEED);
								m_img.setSprite("Images//Sprite//Guard//guard_run");
							}
							else
							{
								m_speed.X = MOVEMENTSPEED;
								m_img.setAnimationSpeed(WALKINGANIMATIONSPEED);
								m_img.setSprite("Images//Sprite//Guard//guard_flash_walk");
							}
							m_flashLight.getPosition().setLocalX(0);
							m_flashLight.setSprite("Images//LightCone//light_guard_walk");
							m_flashLight.setFacingRight(true);
						}
					}
				}
				else
				{
					m_speed.X = 0;
					m_aiActive = false;
					if (m_flashLight == null)
					{
						m_img.setSprite("Images//Sprite//Guard//guard_turn");
					}
					else
					{
						m_img.setSprite("Images//Sprite//Guard//guard_flash_turn");
						m_flashLight.setSprite("Images//LightCone//light_guard_turn");
						m_flashLight.getImg().setAnimationSpeed(TURNANIMATIONSPEED);
						m_flashLight.getPosition().setLocalX(-178);
						if (m_running)
						{
							m_flashLight.getImg().setAnimationSpeed(TURNQUICKANIMATIONSPEED);
						}
						else
						{
							m_flashLight.getImg().setAnimationSpeed(TURNANIMATIONSPEED);
						}
					}
					if (m_running)
					{
						m_img.setAnimationSpeed(TURNQUICKANIMATIONSPEED);
					}
					else
					{
						m_img.setAnimationSpeed(TURNANIMATIONSPEED);
					}
					m_img.setLooping(false);
				}
			}
			
		}
		internal void goLeft()
		{
			if (m_aiActive)
			{
				if (m_facingRight)
				{
					m_speed.X = 0;
					m_aiActive = false;
					if (m_flashLight == null)
					{
						m_img.setSprite("Images//Sprite//Guard//guard_turn");
					}
					else
					{
						m_img.setSprite("Images//Sprite//Guard//guard_flash_turn");
						m_flashLight.setSprite("Images//LightCone//light_guard_turn");
						m_flashLight.getPosition().setLocalX(-175);
						if (m_running)
						{
							m_flashLight.getImg().setAnimationSpeed(TURNQUICKANIMATIONSPEED);
						}
						else
						{
							m_flashLight.getImg().setAnimationSpeed(TURNANIMATIONSPEED);
						}
					}
					if (m_running)
					{
						m_img.setAnimationSpeed(TURNQUICKANIMATIONSPEED);
					}
					else
					{
						m_img.setAnimationSpeed(TURNANIMATIONSPEED);
					}
					m_img.setLooping(false);
				}
				else if (m_speed.X == 0)
				{
					if (m_flashLight == null)
					{
						if (m_running)
						{
							m_speed.X = -CHASINGSPEED;
							m_img.setAnimationSpeed(CHASINGANIMATIONSPEED);
							m_img.setSprite("Images//Sprite//Guard//guard_run");
						}
						else
						{
							m_speed.X = -MOVEMENTSPEED;
							m_img.setAnimationSpeed(WALKINGANIMATIONSPEED);
							m_img.setSprite("Images//Sprite//Guard//guard_walk");
						}
					}
					else
					{
						if (m_running)
						{
							m_flashLight.kill();
							m_flashLightId = 0;
							m_flashLight = null;
							m_speed.X = -CHASINGSPEED;
							m_img.setAnimationSpeed(CHASINGANIMATIONSPEED);
							m_img.setSprite("Images//Sprite//Guard//guard_run");
						}
						else
						{
							m_speed.X = -MOVEMENTSPEED;
							m_img.setAnimationSpeed(WALKINGANIMATIONSPEED);
							m_img.setSprite("Images//Sprite//Guard//guard_flash_walk");
						}
						m_flashLight.getPosition().setLocalX(m_img.getSize().X - m_flashLight.getImg().getSize().X);
						m_flashLight.setSprite("Images//LightCone//light_guard_walk");
						m_flashLight.setFacingRight(false);
					}
				}
			}
		}
		internal void stop()
		{
			m_speed.X = 0;
			if (m_flashLight == null)
			{
				m_img.setSprite("Images//Sprite//Guard//guard_idle");
			}
			else
			{
				m_img.setSprite("Images//Sprite//Guard//guard_flash_idle");
				m_flashLight.setSprite("Images//LightCone//light_guard_idle");
			}
		}
		internal void toggleFlashLight()
		{
			if (m_aiActive)
			{
				m_speed.X = 0;
				m_aiActive = false;
				if (m_flashLight == null)
				{
					m_img.setSprite("Images//Sprite//Guard//guard_pick_up_flash");
					if (m_facingRight)
					{
						Game.getInstance().getState().addObject(new Particle(new Vector2(m_position.getGlobalX() + 72, m_position.getGlobalY() - 10), "Images//Sprite//Guard//qmark", 10f, m_layer));
					}
					else
					{
						Game.getInstance().getState().addObject(new Particle(new Vector2(m_position.getGlobalX() - 22, m_position.getGlobalY() - 10), "Images//Sprite//Guard//qmark", 10f, m_layer));
					}
				}
				else
				{
					m_img.setSprite("Images//Sprite//Guard//guard_put_down_flash");
					m_flashLight.kill();
					m_flashLightId = 0;
					m_flashLight = null;
				}
				m_img.setLooping(false);
			}
		}

		public override void kill()
		{
			base.kill();
			if (m_flashLight != null)
			{
				Game.getInstance().getState().removeObject(m_flashLight);
				m_flashLight.kill();
				m_flashLightId = 0;
				m_flashLight = null;
			}
		}
		internal float getLeftPatrolPoint()
		{
			return m_leftPatrolPoint;
		}


		internal bool hasPatrol()
		{
			return m_hasPatrol;
		}

		internal float getRightPatrolPoint()
		{
			return m_rightPatrolPoint;
		}

		internal Entity getChaseTarget()
		{
			return m_chaseTarget;
		}
		public void setChaseTarget(Entity a_target)
		{
			m_chaseTarget = a_target;
		}
		internal bool isRunning()
		{
			return m_running;
		}

		internal void setRunning(bool a_running)
		{
			m_running = a_running;
			if (m_speed.X != 0)
			{
				if (m_running)
				{
					m_img.setSprite("Images//Sprite//Guard//guard_walk");
					m_img.setAnimationSpeed(CHASINGANIMATIONSPEED);
					if (m_speed.X < 0)
					{
						m_speed.X = -CHASINGSPEED;
					}
					else
					{
						m_speed.X = CHASINGSPEED;
					}
				}
				else
				{
					if (m_speed.X < 0)
					{
						m_speed.X = -MOVEMENTSPEED;
					}
					else
					{
						m_speed.X = MOVEMENTSPEED;
					}
				}
			}
			if (m_running)
			{
				if (m_flashLight != null)
				{
					m_flashLight.kill();
					m_flashLightId = 0;
					m_flashLight = null;
				}
			}
		}

		public bool canSeePlayer()
		{
			Player t_player = Game.getInstance().getState().getPlayer();

			return t_player != null &&
				((t_player.getCurrentState() != Player.State.Hiding && t_player.isInLight())
				|| (t_player.isInLight() && t_player.getCurrentState() == Player.State.Hiding && t_player.isFacingRight() == isFacingRight())) &&
				isFacingTowards(t_player.getPosition().getGlobalX()) &&
				Math.Abs(t_player.getPosition().getGlobalX() - m_position.getGlobalX()) < m_sightRange &&
				t_player.getPosition().getGlobalY() <= m_position.getGlobalY() + 100 &&
				t_player.getPosition().getGlobalY() >= m_position.getGlobalY() - 200 &&
				canSeePoint(t_player.getPosition().getGlobalCartesianCoordinates() + (t_player.getImg().getSize() / new Vector2(2, 4)));
		}

		public bool isFacingRight()
		{
			return m_facingRight ^
				(m_img.getSubImageIndex() > 5.5 &&
				(m_img.getImagePath().Equals("Images//Sprite//Guard//guard_flash_turn") || m_img.getImagePath().Equals("Images//Sprite//Guard//guard_turn")));
		}

		public bool isFacingTowards(float a_x)
		{
			if(m_img.getImagePath().Equals("Images//Sprite//Guard//guard_flash_turn") || m_img.getImagePath().Equals("Images//Sprite//Guard//guard_turn"))
			{
				if (m_img.getSubImageIndex() < 5)
				{
					return (a_x >= m_position.getGlobalX() == m_facingRight);
				}
				else if (m_img.getSubImageIndex() >= 6)
				{
					return (a_x <= m_position.getGlobalX() == m_facingRight);
				}
				else
				{
					return false;
				}
			}
			else
			{
				return (a_x >= m_position.getGlobalX() == m_facingRight);
			}
		}
		public override void update(GameTime a_gameTime)
		{
			if (!m_aiActive)
			{
				if (m_img.isStopped())
				{
					m_aiActive = true;
					if (m_striking)
					{
						m_striking = false;
						m_img.setSprite("Images//Sprite//Guard//guard_idle");
					}
					else if (m_img.getImage() == t2d_turn)
					{
						m_img.setSprite("Images//Sprite//Guard//guard_idle");
						m_facingRight = !m_facingRight;
					}
					else if (m_img.getImage() == t2d_flashIdle)
					{
						m_img.setSprite("Images//Sprite//Guard//guard_flash_idle");
						m_facingRight = !m_facingRight;
						m_flashLight.setSprite("Images//LightCone//light_guard_idle");
						m_flashLight.setFacingRight(m_facingRight);
						if (m_facingRight)
						{
							m_flashLight.getPosition().setLocalX(0);
						}
						else
						{
							m_flashLight.getPosition().setLocalX(m_img.getSize().X - m_flashLight.getImg().getSize().X);
						}

					}
					else if (m_img.getImage() == t2d_pickUpFlash)
					{
						m_flashLight = new FlashCone(this, new Vector2(0, -7), "Images//LightCone//light_guard_idle", m_facingRight, 0.249f);
						m_flashLightId = m_flashLight.getId();
						if (!m_facingRight)
						{
							m_flashLight.getPosition().setLocalX(m_img.getSize().X - m_flashLight.getImg().getSize().X);
						}
						Game.getInstance().getState().addObject(m_flashLight);
						m_img.setSprite("Images//Sprite//Guard//guard_flash_idle");
					}
					else if (m_img.getImage() == t2d_putDownFlash)
					{
						m_img.setSprite("Images//Sprite//Guard//guard_idle");
					}
				}
			}

			base.update(a_gameTime);

			m_strikeReloadTime = Math.Max(m_strikeReloadTime - (a_gameTime.ElapsedGameTime.Milliseconds / 1000f), 0);

			if ((m_aiState != AIStateChasing.getInstance()) && canSeePlayer())
			{
				chasePlayer();
				m_aiState = AIStateChasing.getInstance();
			}

			if (m_facingRight)
			{
				m_spriteEffects = SpriteEffects.None;
			}
			else
			{
				m_spriteEffects = SpriteEffects.FlipHorizontally;
			}
			if (m_flashLight != null)
			{
				m_flashLight.setSubImage(m_img.getSubImageIndex());
			}
		}

		internal void chasePlayer()
		{
			Player t_player = Game.getInstance().getState().getPlayer();
			m_chaseTarget = t_player;
			if (!t_player.isChase())
			{
				t_player.activateChaseMode(this);
			}
		}
		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			Platform t_supportingPlatform = null;
			foreach (Entity t_collision in a_collisionList)
			{
				if (t_collision is Wall || t_collision is Window)
				{
					if (m_speed.X < 0 && m_position.getGlobalX() > t_collision.getPosition().getGlobalX())
					{
						m_nextPosition.X = (t_collision.getHitBox().getOutBox().X + t_collision.getHitBox().getOutBox().Width - ((CollisionRectangle)m_collisionShape).m_xOffset);
						stop();
					}
					else if (m_speed.X > 0 && m_position.getGlobalX() < t_collision.getPosition().getGlobalX())
					{
						m_nextPosition.X = (t_collision.getHitBox().getOutBox().X - t_collision.getHitBox().getOutBox().Width - ((CollisionRectangle)m_collisionShape).m_xOffset);
						stop();
					}
				}
				else if (t_collision is Platform)
				{
					if (t_collision.getPosition().getGlobalY() < m_position.getGlobalY() + m_img.getSize().Y - 50)
					{
						if (m_speed.X < 0)
						{
							m_nextPosition.X = (t_collision.getHitBox().getOutBox().X + t_collision.getHitBox().getOutBox().Width - ((CollisionRectangle)m_collisionShape).m_xOffset);
							stop();
						}
						else if (m_speed.X > 0)
						{
							m_nextPosition.X = (t_collision.getHitBox().getOutBox().X - t_collision.getHitBox().getOutBox().Width - ((CollisionRectangle)m_collisionShape).m_xOffset);
							stop();
						}
					}
					else
					{
						if (m_gravity > 0)
						{
							m_gravity = 0;
							m_speed.Y = 0;
							m_nextPosition.Y = (t_collision.getPosition().getGlobalY() - m_img.getSize().Y);
						}
						if (t_supportingPlatform == null ||
							(m_facingRight && t_collision.getPosition().getGlobalX() > t_supportingPlatform.getPosition().getGlobalX()) ||
							(!m_facingRight && t_collision.getPosition().getGlobalX() < t_supportingPlatform.getPosition().getGlobalX()))
						{
							t_supportingPlatform = (Platform)t_collision;
						}
					}
				}
				else if (t_collision is Player)
				{
					Player t_player = (Player)t_collision;
					if (m_striking)
					{
						t_player.dealDamageTo(new Vector2(Math.Sign(t_player.getPosition().getGlobalX() - m_position.getGlobalX()) * 800, -200));
					}
					else if (t_player.getCurrentState() != Player.State.Rolling && t_player.getCurrentState() != Player.State.Hiding)
					{
						chasePlayer();
						m_aiState = AIStateChasing.getInstance();
					}
				}
				else if (t_collision is Guard)
				{
					Guard t_otherGuard = (Guard)t_collision;
					if(m_aiState is AIStateChasing && t_otherGuard.getAIState() is AIStatepatroling)
					{
						t_otherGuard.setChaseTarget(m_chaseTarget);
						t_otherGuard.setAIState(AIStateChasing.getInstance());
					}
				}
			}
			if (m_gravity == 0)
			{
				if (t_supportingPlatform == null)
				{
					m_gravity = 500;
				}
				else
				{
					if (m_speed.X > 0)
					{
						if (t_supportingPlatform.getPosition().getGlobalX() + t_supportingPlatform.getImg().getSize().X < m_collisionShape.getOutBox().X + m_collisionShape.getOutBox().Width)
						{
							m_nextPosition.X = (t_supportingPlatform.getPosition().getGlobalX() + t_supportingPlatform.getImg().getSize().X - m_collisionShape.getOutBox().Width);
							stop();
						}
					}
					else if (m_speed.X < 0)
					{
						if (t_supportingPlatform.getPosition().getGlobalX() > m_collisionShape.getOutBox().X)
						{
							m_nextPosition.X = (t_supportingPlatform.getPosition().getGlobalX()) -((CollisionRectangle)m_collisionShape).m_xOffset;
							stop();
						}
					}
				}
			}
		}

		public void addLampSwitchTarget(LampSwitch a_lampSwitch)
		{
			m_lampSwitchTargets.AddLast(a_lampSwitch);
			m_lampSwitchTargetsId.AddLast(a_lampSwitch.getId());
		}

		public void removeLampSwitchTarget(LampSwitch a_lampSwitch)
		{
			m_lampSwitchTargets.Remove(a_lampSwitch);
			m_lampSwitchTargetsId.Remove(a_lampSwitch.getId());
		}

		public bool hasNoLampSwitchTargets()
		{
			return m_lampSwitchTargets.Count == 0;
		}

		internal LampSwitch getFirstLampSwitchTarget()
		{
			return m_lampSwitchTargets.First();
		}

		internal void removeFirstLampSwitchTarget()
		{
			m_lampSwitchTargets.RemoveFirst();
		}

		public bool canStrike()
		{
			return (!m_striking) && (m_strikeReloadTime == 0) && (m_aiActive);
		}

		internal bool isStriking()
		{
			return m_striking;
		}

		internal void strike()
		{
			if (canStrike())
			{
				m_striking = true;
				m_strikeReloadTime = 1f;
				m_aiActive = false;
				m_img.setSprite("Images//Sprite//Guard//guard_strike");
				m_img.setLooping(false);
			}
		}

		internal bool isCarryingFlash()
		{
			return m_flashLight != null;
		}

		internal bool hasFlash()
		{
			return m_hasFlashLight;
		}

		public bool isFlashLightAddicted()
		{
			return m_FlashLightAddicted;
		}

		public void toggleFlashLightAddicted()
		{
			m_FlashLightAddicted = !m_FlashLightAddicted;
		}
		public override void flip()
		{
			base.flip();
			m_facingRight = m_spriteEffects == SpriteEffects.None;
			m_guardFaceRight = m_facingRight;
		}
		public bool guardFaceRight()
		{
			return m_guardFaceRight;
		}

		public bool canSeePoint(Vector2 a_point)
		{
			return CollisionManager.possibleLineOfSight(m_position.getGlobalCartesianCoordinates() + new Vector2(0, 10), a_point);
		}
	}
}
