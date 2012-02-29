using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	class GuardDog : NPE
	{
		private float m_leftPatrolPoint;
		private float m_rightPatrolPoint;
		private Boolean m_hasPatrol;
		private float MOVEMENTSPEED = 80;
		private float CHARGEINGSPEED = 400;
		private Boolean m_chargeing = false;
		private Boolean m_faceingRight;
		private float m_sightRange = 576f;
		private float m_senceRange = 72 * 2;
		private float m_chargeEndPoint;
		private Entity m_chaseTarget = null;

		public GuardDog(Vector2 a_posV2, String a_sprite, float a_leftPatrolPoint, float a_rightPatrolPoint, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			if (a_rightPatrolPoint < a_leftPatrolPoint)
			{
				throw new ArgumentException();
			}
			m_leftPatrolPoint = a_leftPatrolPoint;
			m_rightPatrolPoint = a_rightPatrolPoint;
			m_hasPatrol = (m_leftPatrolPoint != m_rightPatrolPoint);
			m_aiState = AIStatepatroling.getInstance();
		}

		internal bool canSencePlayer()
		{
			return Game.getInstance().getState().getPlayer() != null &&
				((Game.getInstance().getState().getPlayer().getPosition().getGlobalCartesianCoordinates() - m_position.getGlobalCartesianCoordinates()).Length() < m_senceRange ||
				(Game.getInstance().getState().getPlayer().isInLight() &&
				isFaceingTowards(Game.getInstance().getState().getPlayer().getPosition().getGlobalX()) &&
				Math.Abs(Game.getInstance().getState().getPlayer().getPosition().getGlobalX() - m_position.getGlobalX()) < m_sightRange &&
				Game.getInstance().getState().getPlayer().getPosition().getGlobalY() <= m_position.getGlobalY() + 100 &&
				Game.getInstance().getState().getPlayer().getPosition().getGlobalY() >= m_position.getGlobalY() - 200));
		}

		public bool isFaceingTowards(float a_x)
		{
			return (a_x <= m_position.getGlobalX() && !m_faceingRight)
				|| (a_x >= m_position.getGlobalX() && m_faceingRight);
		}

		internal bool haspatrol()
		{
			return m_hasPatrol;
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

		internal float getLeftpatrolPoint()
		{
			return m_leftPatrolPoint;
		}

		internal void goRight()
		{
			if (m_chargeing)
			{
				m_speed.X = CHARGEINGSPEED;
			}
			else
			{
				m_speed.X = MOVEMENTSPEED;
			}
			m_faceingRight = true;
		}

		internal float getRightpatrolPoint()
		{
			return m_rightPatrolPoint;
		}

		internal void goLeft()
		{
			if (m_chargeing)
			{
				m_speed.X = -CHARGEINGSPEED;
			}
			else
			{
				m_speed.X = -MOVEMENTSPEED;
			}
			m_faceingRight = false;
		}

		internal void stop()
		{
			m_speed.X = 0;
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

		public float getChargeingPoint()
		{
			return m_chargeEndPoint;
		}

		internal void setChargeing(bool a_chargeing)
		{
			if (m_chargeing)
			{
				if (!a_chargeing)
				{
					m_chargeing = false;
					if (m_speed.X > 0)
					{
						m_speed.X = CHARGEINGSPEED;
					}
					else if (m_speed.X< 0)
					{
						m_speed.X = -CHARGEINGSPEED;
					}
				}
			}
			else
			{
				if (a_chargeing)
				{
					m_chargeing = true;
					if (m_speed.X > 0)
					{
						m_speed.X = MOVEMENTSPEED;
					}
					else if (m_speed.X < 0)
					{
						m_speed.X = -MOVEMENTSPEED;
					}
				}
			}
		}

		internal void setChargePoint(float a_x)
		{
			m_chargeEndPoint = a_x;
		}

		public bool isBarkingPrefered()
		{
			//retunerar om hunden föredrar att skälla i denna situation.
			return (Game.getInstance().getState().getPlayer().getCurrentState() == Player.State.Hiding);
		}

		internal void chasePlayer()
		{
			m_chaseTarget = Game.getInstance().getState().getPlayer();
		}

		internal void forgetChaseTarget()
		{
			m_chaseTarget = null;
		}

		public bool isChargeing()
		{
			return m_chargeing;
		}

		public bool getFaceingDirection()
		{
			return m_faceingRight;
		}
	}
}
