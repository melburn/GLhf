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
        private float m_leftPatrollPoint;
        private float m_rightPatrollPoint;
        private Boolean m_hasPatroll;
        private Boolean m_hasFlashLight;
        public Boolean m_inALightArea = false;
        private Boolean m_isCarryingFlashLight;
        private float MOVEMENTSPEED = 100;
		private float CHASINGSPEED = 350;
        private Entity m_chaseTarget = null;
		private Boolean m_running = false;
		private Boolean m_faceingRight;
		private float m_sightRange = 576f;

		//flashlight addicted guard always has their flashlight up
		private Boolean m_FlashLightAddicted;

		public Guard(Vector2 a_posV2, String a_sprite, float a_leftPatrollPoint, float a_rightPatrollPoint, Boolean a_hasFlashLight, Boolean a_flashLightAddicted, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			m_leftPatrollPoint = a_leftPatrollPoint;
			m_rightPatrollPoint = a_rightPatrollPoint;
			m_hasPatroll = true;
			m_hasFlashLight = a_hasFlashLight;
			m_FlashLightAddicted = a_flashLightAddicted;
			m_aiState = AIStatePatrolling.getInstance();
		}
        public Guard(Vector2 a_posV2, String a_sprite, float a_patrollPoint, Boolean a_hasFlashLight, Boolean a_flashLightAddicted, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
            m_hasPatroll = false;
            m_hasFlashLight = a_hasFlashLight;
            m_FlashLightAddicted = a_flashLightAddicted;
            m_leftPatrollPoint = a_patrollPoint;
            m_rightPatrollPoint = a_patrollPoint;
			m_aiState = AIStatePatrolling.getInstance();
		}
		internal void goRight()
		{
			if (m_running)
			{
				m_speed.X = CHASINGSPEED;
			}
			else
			{
				m_speed.X = MOVEMENTSPEED;
			}
			m_faceingRight = true;
			if(m_isCarryingFlashLight)
			{
				//m_img.setSprite("goRightWithFlashLight");
			}
			else
			{
				//m_img.setSprite("goRight");
			}
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
			if(m_isCarryingFlashLight)
			{
				//m_img.setSprite("goWithflahs");
			}
			else
			{
				//m_img.setSprite("goLeft");
			}
		}
		internal void stop()
		{
			m_speed.X = 0;
			if(m_isCarryingFlashLight)
			{
				//m_img.setSprite("idleing");
			}
			else
			{
				m_img.setSprite("Images//Sprite//guard_idle");
			}
		}
		internal void toogleFlashLight()
		{
			if(m_isCarryingFlashLight)
			{
				m_isCarryingFlashLight = false;
				//m_img.setSprite("putthataway");
			}
			else
			{
				m_isCarryingFlashLight = true;
				//m_img.setSprite("getthatup");
			}
			m_speed.X = 0;
		}

		internal float getLeftPatrollPoint()
		{
			return m_leftPatrollPoint;
		}


		internal bool hasPatroll()
		{
			return m_hasPatroll;
		}

		internal float getRightPatrollPoint()
		{
			return m_rightPatrollPoint;
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
					if (m_isCarryingFlashLight)
					{
						//m_img.setSprite(running with the flash);
					}
					else
					{
						//m_img.setSprite(running like a boss);
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
					if (m_isCarryingFlashLight)
					{
						//m_img.setSprite(walking with the flash);
					}
					else
					{
						//m_img.setSprite(walking like a boss);
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
				Math.Abs(Game.getInstance().getState().getPlayer().getPosition().getGlobalX()-m_position.getGlobalX()) < m_sightRange;
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
			}
			else
			{
				m_spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}

		internal void chasePlayer()
		{
			m_chaseTarget = Game.getInstance().getState().getPlayer();
		}
	}
}
