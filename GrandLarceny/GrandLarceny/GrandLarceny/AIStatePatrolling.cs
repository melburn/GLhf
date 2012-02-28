using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
{
	[Serializable()]
	public class AIStatepatroling : AIState
	{
		private static AIStatepatroling instance;
		public static AIStatepatroling getInstance()
		{
			if(instance == null)
			{
				instance = new AIStatepatroling();
			}
			return instance;
		}
		public override AIState execute(NPE a_agent)
		{
			if(a_agent==null)
			{
				throw new ArgumentException("The Agent cannot be null");
			}
			if(a_agent is Guard)
			{
				Guard t_guard = (Guard)a_agent;
				if (t_guard.canSeePlayer())
				{
					t_guard.chasePlayer();
					return AIStateChasing.getInstance();
				}
				else
				{
					if (!t_guard.hasNoLampSwitchTargets())
					{
						return AIStateGoingToTheSwitch.getInstance();
					}
					else
					{
						if (t_guard.haspatrol())
						{
							if (t_guard.getPosition().getGlobalCartesianCoordinates().X < t_guard.getLeftpatrolPoint())
							{
								t_guard.goRight();
							}
							else if (t_guard.getPosition().getGlobalCartesianCoordinates().X > t_guard.getRightpatrolPoint())
							{
								t_guard.goLeft();
							}
							else if (t_guard.getHorizontalSpeed() == 0)
							{
								t_guard.goLeft();
							}
						}
						else
						{
							float t_guardPoint = t_guard.getLeftpatrolPoint();
							if (t_guard.getPosition().getGlobalCartesianCoordinates().X + 10 < t_guardPoint)
							{
								if (t_guard.getHorizontalSpeed() >= 0)
								{
									t_guard.goRight();
								}
							}
							else if (t_guard.getPosition().getGlobalCartesianCoordinates().X - 10 > t_guardPoint)
							{
								if (t_guard.getHorizontalSpeed() <= 0)
								{
									t_guard.goLeft();
								}
							}
							else
							{
								if (t_guard.getHorizontalSpeed() != 0)
								{
									t_guard.stop();
								}
							}
						}
						return this;
					}
				}
			}
			else if (a_agent is GuardDog)
			{
				GuardDog t_guardDog = (GuardDog)a_agent;
				if (t_guardDog.canSencePlayer())
				{
					if (t_guardDog.isBarkingPrefered())
					{
						//om hunden föredrar att skälla så ser vi honom att jaga spelaren tills han är tillräkligt nära för att skälla.
						t_guardDog.chasePlayer();
						return AIStateChasing.getInstance();
					}
					else
					{
						t_guardDog.setChargePoint(Math.Sign(Game.getInstance().getState().getPlayer().getPosition().getGlobalX() - t_guardDog.getPosition().getGlobalX()) * AIStateChargeing.CHARGEDISTANCE
							+ Game.getInstance().getState().getPlayer().getPosition().getGlobalX());
						return AIStateChargeing.getInstance();
					}
				}
				else
				{
					if (t_guardDog.haspatrol())
					{
						if (t_guardDog.getPosition().getGlobalCartesianCoordinates().X < t_guardDog.getLeftpatrolPoint() && t_guardDog.getHorizontalSpeed() <= 0)
						{
							t_guardDog.goRight();
						}
						else if (t_guardDog.getPosition().getGlobalCartesianCoordinates().X > t_guardDog.getRightpatrolPoint() && t_guardDog.getHorizontalSpeed() >= 0)
						{
							t_guardDog.goLeft();
						}
					}
					else
					{
						float t_guardPoint = t_guardDog.getLeftpatrolPoint();
						if (t_guardDog.getPosition().getGlobalCartesianCoordinates().X + 10 < t_guardPoint)
						{
							if (t_guardDog.getHorizontalSpeed() >= 0)
							{
								t_guardDog.goRight();
							}
						}
						else if (t_guardDog.getPosition().getGlobalCartesianCoordinates().X - 10 > t_guardPoint)
						{
							if (t_guardDog.getHorizontalSpeed() <= 0)
							{
								t_guardDog.goLeft();
							}
						}
						else
						{
							if (t_guardDog.getHorizontalSpeed() != 0)
							{
								t_guardDog.stop();
							}
						}
					}
					return this;
				}
			}
			else
			{
				throw new ArgumentException("Only guards and guarddogs can patrol");
			}
		}
	}
}
