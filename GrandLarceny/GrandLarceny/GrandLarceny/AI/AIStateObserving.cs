using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny.AI
{

	class AIStateObserving : AIState
	{
		private const int s_minimumRange = 200;
		private float m_endTime;
		private bool m_prevFacingRight;

		public AIStateObserving(float a_endTime, bool a_prevFacingRight)
		{
			m_endTime = a_endTime;
			m_prevFacingRight = a_prevFacingRight;
		}

		public override AIState execute(NPE a_agent, GameTime a_gameTime)
		{
			if (a_agent is Guard)
			{
				Guard t_guard = (Guard)a_agent;
				if (t_guard.getChaseTarget() is Player)
				{
					Player t_chaseTarget = (Player)t_guard.getChaseTarget();
					if (t_chaseTarget.getListLayer() == t_guard.getListLayer() && (t_chaseTarget.getCenterPoint() - t_guard.getCenterPoint()).Length() <= s_minimumRange)
					{
						t_guard.chasePlayer();
						return AIStateChasing.getInstance();
					}
					else if (timeOver(a_gameTime))
					{
						if (t_guard.canHearPlayer() ||
							(t_guard.getCenterPoint() - t_chaseTarget.getCenterPoint()).Length() < s_minimumRange)
						{
							t_guard.chasePlayer();
							return AIStateChasing.getInstance();
						}
						else
						{
							t_guard.setChaseTarget(null);
							if (m_prevFacingRight)
							{
								if (!t_guard.isFacingRight())
								{
									t_guard.goRight();
								}
							}
							else if (t_guard.isFacingRight())
							{
								t_guard.goLeft();
							}
							return AIStateGoingToTheSwitch.getInstance();
						}
					}
					else if (t_guard.getHorizontalSpeed() != 0)
					{
						t_guard.stop();
					}
					else
					{
						t_guard.faceTowards(t_chaseTarget.getCenterPoint().X);
					}
				}
				else if (timeOver(a_gameTime))
				{
					if (m_prevFacingRight)
					{
						if (!t_guard.isFacingRight())
						{
							t_guard.goRight();
						}
					}
					else if (t_guard.isFacingRight())
					{
						t_guard.goLeft();
					}
					return AIStateGoingToTheSwitch.getInstance();
				}
				return this;
			}
			else
			{
				throw new ArgumentException("Only guards can observe");
			}
		}

		public bool timeOver(GameTime a_gameTime)
		{
			return m_endTime <= a_gameTime.TotalGameTime.TotalMilliseconds;
		}
	}
}
