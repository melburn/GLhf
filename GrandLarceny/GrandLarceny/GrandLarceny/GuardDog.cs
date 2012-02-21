using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class GuardDog : NPE
	{
		private float m_leftPatrollPoint;
		private float m_rightPatrollPoint;
		private Boolean m_hasPatroll;
		private float MOVEMENTSPEED = 100;
		private float CHASINGSPEED = 350;
		private Entity m_chaseTarget = null;
		private Boolean m_running = false;
		private Boolean m_faceingRight;
		private float m_sightRange = 576f;
		private float m_senceRange = 72 * 2;

		public GuardDog(Vector2 a_posV2, String a_sprite, float a_leftPatrollPoint, float a_rightPatrollPoint, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			m_leftPatrollPoint = a_leftPatrollPoint;
			m_rightPatrollPoint = a_rightPatrollPoint;
			m_hasPatroll = true;
			m_aiState = AIStatePatrolling.getInstance();
		}

		internal bool canSencePlayer()
		{
			return (Game.getInstance().getState().getPlayer().getPosition().getGlobalCartesianCoordinates() - m_position.getGlobalCartesianCoordinates()).Length() < m_senceRange ||
				(Game.getInstance().getState().getPlayer().isInLight() &&
				isFaceingTowards(Game.getInstance().getState().getPlayer().getPosition().getGlobalX()) &&
				Math.Abs(Game.getInstance().getState().getPlayer().getPosition().getGlobalX() - m_position.getGlobalX()) < m_sightRange &&
				Game.getInstance().getState().getPlayer().getPosition().getGlobalY() <= m_position.getGlobalY() + 100 &&
				Game.getInstance().getState().getPlayer().getPosition().getGlobalY() >= m_position.getGlobalY() - 200);
		}

		private bool isFaceingTowards(float a_x)
		{
			return (a_x <= m_position.getGlobalX() && !m_faceingRight)
				|| (a_x >= m_position.getGlobalX() && m_faceingRight);
		}

		internal void chasePlayer()
		{
			throw new NotImplementedException();
		}

		internal bool hasPatroll()
		{
			throw new NotImplementedException();
		}

		internal float getLeftPatrollPoint()
		{
			throw new NotImplementedException();
		}

		internal void goRight()
		{
			throw new NotImplementedException();
		}

		internal float getRightPatrollPoint()
		{
			throw new NotImplementedException();
		}

		internal void goLeft()
		{
			throw new NotImplementedException();
		}

		internal void stop()
		{
			throw new NotImplementedException();
		}
	}
}
