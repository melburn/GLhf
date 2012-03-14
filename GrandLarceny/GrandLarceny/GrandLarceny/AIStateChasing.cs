using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class AIStateChasing : AIState
	{
		private AIStateChasing()
		{
		}
		private static AIStateChasing instance;
		private const float MINIMUMDISTANCE = 30;
		private const float MINIMUMCAMERAANGLE = 0.1f;
		public static AIStateChasing getInstance()
		{
			if (instance == null)
			{
				instance = new AIStateChasing();
			}
			return instance;
		}
		public override AIState execute(NPE a_agent)
		{
			if (a_agent == null)
			{
				throw new ArgumentNullException("The Agent cannot be null");
			}
			if(a_agent is Guard)
			{
				Guard t_guard = (Guard)a_agent;
				Entity t_target = t_guard.getChaseTarget();
				if (t_target == null)
				{
					return AIStatepatroling.getInstance();
				}
				if (! t_guard.isRunning())
				{
					t_guard.setRunning(true);
				}
				if (Math.Abs(t_guard.getPosition().getGlobalX() - t_target.getPosition().getGlobalX()) < MINIMUMDISTANCE)
				{
					if(t_guard.getHorizontalSpeed() != 0)
					{
						t_guard.stop();
					}
					if (t_guard.canStrike())
					{
						//return AIStateStriking.getInstance();
						//sur på strikingstate

						t_guard.strike();
					}
				}
				else
				{
					if (t_guard.getPosition().getGlobalX() < t_target.getPosition().getGlobalX())
					{
						if (t_guard.getHorizontalSpeed() <= 0)
						{
							t_guard.goRight();
						}
					}
					else
					{
						if (t_guard.getHorizontalSpeed() >= 0)
						{
							t_guard.goLeft();
						}
					}
				}
				return this;
			}
			else if (a_agent is GuardDog)
			{
				GuardDog t_guardDog = (GuardDog)a_agent;
				if (t_guardDog.canSencePlayer() && ! t_guardDog.isBarkingPrefered())
				{
					t_guardDog.forgetChaseTarget();
					t_guardDog.setChargePoint(Math.Sign(Game.getInstance().getState().getPlayer().getPosition().getGlobalX() - t_guardDog.getPosition().getGlobalX()) * AIStateChargeing.CHARGEDISTANCE
							+ Game.getInstance().getState().getPlayer().getPosition().getGlobalX());
					return AIStateChargeing.getInstance();
				}
				if ((Game.getInstance().getState().getPlayer().getPosition().getGlobalCartesianCoordinates() - t_guardDog.getPosition().getGlobalCartesianCoordinates()).Length() <= AIStateBark.BARKDISTANCE)
				{
					return AIStateBark.getInstance();
				}
				else
				{
					return this;
				}
			}
			else if (a_agent is GuardCamera)
			{
				GuardCamera t_gc = (GuardCamera)a_agent;
				if (t_gc.canSeePlayer())
				{
					float t_currentRotation = t_gc.getRotation();

					float t_pointDirection = t_gc.getPosition().getAngleTo(t_gc.getTarget().getPosition());
					//float t_pointDirection = (float)Math.Atan2(t_gc.getPosition().getGlobalY() - t_gc.getTarget().getPosition().getGlobalY(), t_gc.getPosition().getGlobalY() - t_gc.getTarget().getPosition().getGlobalX());
					//float t_pointDirection = (float)Math.Atan2(t_gc.getTarget().getPosition().getGlobalY() - t_gc.getPosition().getGlobalY(), t_gc.getTarget().getPosition().getGlobalX() - t_gc.getPosition().getGlobalY());
					//if ((t_pointDirection < t_currentRotation && t_pointDirection > t_currentRotation - Math.PI) || t_pointDirection > (t_currentRotation + Math.PI) % (2 * Math.PI))
					if ((t_currentRotation + Math.PI > t_pointDirection && t_pointDirection < t_currentRotation) || t_pointDirection > t_currentRotation + Math.PI)
					{
						t_gc.rotateLeft();
					}
					else
					{
						t_gc.rotateRight();
					}
					return this;
				}
				else
				{
					return AIStatepatroling.getInstance();
				}
			}
			else
			{
				throw new ArgumentException("Only guards and dogs can chase");
			}
        }
    }
}
