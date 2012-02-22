using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	public class Guard : NPE
	{
        private float m_leftPatrolPoint;
        private float m_rightPatrolPoint;
        private Boolean m_hasPatrol;
        private Boolean m_hasFlashLight;
        public Boolean m_inALightArea = false;

        private const float MOVEMENTSPEED = 150;
		private const float CHASINGSPEED = 350;
		private const float WALKINGANIMATIONSPEED = MOVEMENTSPEED / 16;
		private const float CHASINGANIMATIONSPEED = CHASINGSPEED / 16;

        private Entity m_chaseTarget = null;
		private Boolean m_running = false;
		private Boolean m_faceingRight;
		private float m_sightRange = 576f;
		private LightCone m_flashLight;
		private static Vector2[] s_lightConePositionsWhileWalkingRight;
		private static Vector2[] s_lightConePositionsWhileWalkingLeft;
		private static float[] s_lightConeRotationsWhileWalkingRight;
		private static float[] s_lightConeRotationsWhileWalkingLeft;

		//flashlight addicted guard always has their flashlight up
		private Boolean m_FlashLightAddicted;

		public Guard(Vector2 a_posV2, String a_sprite, float a_leftpatrolPoint, float a_rightpatrolPoint, Boolean a_hasFlashLight, Boolean a_flashLightAddicted, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			m_leftPatrolPoint = a_leftpatrolPoint;
			m_rightPatrolPoint = a_rightpatrolPoint;
			m_hasPatrol = (m_leftPatrolPoint != m_rightPatrolPoint);
			m_hasFlashLight = a_hasFlashLight;
			m_FlashLightAddicted = a_flashLightAddicted;
			m_aiState = AIStatepatroling.getInstance();
			if (m_hasFlashLight && m_FlashLightAddicted)
			{
				m_flashLight = new LightCone(this, "Images//LightCone//Ljus",  m_layer + 1, 300,70);
			}
		}
        public Guard(Vector2 a_posV2, String a_sprite, float a_patrolPoint, Boolean a_hasFlashLight, Boolean a_flashLightAddicted, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
            m_hasPatrol = false;
            m_hasFlashLight = a_hasFlashLight;
            m_FlashLightAddicted = a_flashLightAddicted;
            m_leftPatrolPoint = a_patrolPoint;
            m_rightPatrolPoint = a_patrolPoint;
			m_aiState = AIStatepatroling.getInstance();
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
			if (m_running)
			{
				m_speed.X = CHASINGSPEED;
				if (m_flashLight == null)
				{
					//m_img.setSprite("Images//Sprite//guard_run");
				}
			}
			else
			{
				m_speed.X = MOVEMENTSPEED;
				m_img.setAnimationSpeed(WALKINGANIMATIONSPEED);
				if(m_flashLight==null)
				{
					m_img.setSprite("Images//Sprite//guard_walk");
				}
			}
			m_faceingRight = true;
		}
		internal void goLeft()
		{
			if (m_running)
			{
				m_speed.X = -CHASINGSPEED;
			}
			else
			{
				m_speed.X = -MOVEMENTSPEED;
			}
			m_faceingRight = false;
			if (m_flashLight == null)
			{
				m_img.setSprite("Images//Sprite//guard_walk");
				m_img.setAnimationSpeed(WALKINGANIMATIONSPEED);
			}
			else
			{
				//m_img.setSprite("goWithflahs");
			}
		}
		internal void stop()
		{
			m_speed.X = 0;
			if (m_flashLight == null)
			{
				m_img.setSprite("Images//Sprite//guard_idle");
			}
			else
			{
				//setsprite
			}
		}
		internal void toogleFlashLight()
		{
			if (m_flashLight == null)
			{
				//m_img.setSprite("getthatup");
			}
			else
			{
				//m_img.setSprite("putthataway");
			}
			m_speed.X = 0;
		}

		internal float getLeftpatrolPoint()
		{
			return m_leftPatrolPoint;
		}


		internal bool haspatrol()
		{
			return m_hasPatrol;
		}

		internal float getRightpatrolPoint()
		{
			return m_rightPatrolPoint;
		}

		internal Entity getChaseTarget()
		{
			return m_chaseTarget;
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
					if (m_flashLight == null)
					{
						//m_img.setSprite(running like a boss);
					}
					else
					{
						//m_img.setSprite(running with the flash);
					}
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
					if (m_flashLight == null)
					{
						m_img.setSprite("Images//Sprite//guard_walk");
						m_img.setAnimationSpeed(WALKINGANIMATIONSPEED);
					}
					else
					{
						//m_img.setSprite(walking with the flash);
					}
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
		}

		public bool canSeePlayer()
		{
			return Game.getInstance().getState().getPlayer().isInLight() &&
				isFaceingTowards(Game.getInstance().getState().getPlayer().getPosition().getGlobalX()) &&
				Math.Abs(Game.getInstance().getState().getPlayer().getPosition().getGlobalX() - m_position.getGlobalX()) < m_sightRange &&
				Game.getInstance().getState().getPlayer().getPosition().getGlobalY() <= m_position.getGlobalY() + 100 &&
				Game.getInstance().getState().getPlayer().getPosition().getGlobalY() >= m_position.getGlobalY() - 200;
		}

		public bool isFaceingTowards(float a_x)
		{
			return (a_x <= m_position.getGlobalX() && ! m_faceingRight)
				|| (a_x >= m_position.getGlobalX() && m_faceingRight);
		}
		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			if (m_faceingRight)
			{
				m_spriteEffects = SpriteEffects.None;
				if (m_flashLight != null && m_img.getImagePath().Equals("guardwalking"))
				{
					m_flashLight.getPosition().setCartesianCoordinates(s_lightConePositionsWhileWalkingRight[(int)m_img.getSubImageIndex()]);
					m_flashLight.setRotation(s_lightConeRotationsWhileWalkingRight[(int)m_img.getSubImageIndex()]);
				}
			}
			else
			{
				m_spriteEffects = SpriteEffects.FlipHorizontally;
				if (m_flashLight != null && m_img.getImagePath().Equals("guardwalking"))
				{
					m_flashLight.getPosition().setCartesianCoordinates(s_lightConePositionsWhileWalkingLeft[(int)m_img.getSubImageIndex()]);
					m_flashLight.setRotation(s_lightConeRotationsWhileWalkingLeft[(int)m_img.getSubImageIndex()]);
				}
			}
		}

		internal void chasePlayer()
		{
			m_chaseTarget = Game.getInstance().getState().getPlayer();
		}
		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			foreach (Entity t_collision in a_collisionList)
			{
				if (t_collision is Wall)
				{
					if (m_speed.X < 0)
					{
						m_position.setX(t_collision.getHitBox().getOutBox().X + t_collision.getHitBox().getOutBox().Width);
					}
					else if (m_speed.X > 0)
					{
						m_position.setX(t_collision.getHitBox().getOutBox().X - m_collisionShape.getOutBox().Width);
					}
					m_speed.X = 0;
				}
			}
		}
	}
}
