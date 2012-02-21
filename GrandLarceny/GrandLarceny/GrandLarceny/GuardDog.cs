using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class GuardDog : NPE
	{
		private float m_leftpatrolPoint;
		private float m_rightpatrolPoint;
		private Boolean m_haspatrol;
		private float MOVEMENTSPEED = 100;
		private float CHASINGSPEED = 350;
		private Entity m_chaseTarget = null;
		private Boolean m_running = false;
		private Boolean m_faceingRight;
		private float m_sightRange = 576f;
		private float m_senceRange = 72 * 2;

		public GuardDog(Vector2 a_posV2, String a_sprite, float a_leftpatrolPoint, float a_rightpatrolPoint, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			m_leftpatrolPoint = a_leftpatrolPoint;
			m_rightpatrolPoint = a_rightpatrolPoint;
			m_haspatrol = true;
			m_aiState = AIStatepatroling.getInstance();
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

		internal bool haspatrol()
		{
			throw new NotImplementedException();
		}

		internal float getLeftpatrolPoint()
		{
			throw new NotImplementedException();
		}

		internal void goRight()
		{
			throw new NotImplementedException();
		}

		internal float getRightpatrolPoint()
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
