using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny.AI
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
		public override AIState execute(NPE a_agent, GameTime a_gameTime)
		{
			if(a_agent==null)
			{
				throw new ArgumentException("The Agent cannot be null");
			}
			if(a_agent is Guard)
			{
				Guard t_guard = (Guard)a_agent;
				if (t_guard.canHearPlayer())
				{
					t_guard.setChaseTarget(Game.getInstance().getState().getPlayer());
					if (t_guard.isFacingRight())
					{
						Game.getInstance().getState().addObject(new Particle(
							t_guard.getPosition().getGlobalCartesian() + new Vector2(60,-20),
							"Images//Sprite//Guard//qmark",
							20f,
							t_guard.getLayer()));
					}
					else
					{
						Game.getInstance().getState().addObject(new Particle(
							t_guard.getPosition().getGlobalCartesian() + new Vector2(-20, -10),
							"Images//Sprite//Guard//qmark",
							20f,
							t_guard.getLayer()));
					}
					return new AIStateObserving(((float)a_gameTime.TotalGameTime.TotalMilliseconds) + 2000f, t_guard.isFacingRight());
				}
				else if (!t_guard.hasNoLampSwitchTargets())
				{
					return AIStateGoingToTheSwitch.getInstance();
				}
				else
				{
					if (t_guard.isCarryingFlash() != t_guard.isFlashLightAddicted() && t_guard.hasFlash())
					{
						t_guard.toggleFlashLight();
					}
					else if (t_guard.isRunning())
					{
						t_guard.setRunning(false);
					}
					else if (t_guard.hasPatrol())
					{
						if (t_guard.getPosition().getGlobalCartesian().X < t_guard.getLeftPatrolPoint())
						{
							t_guard.goRight();
						}
						else if (t_guard.getPosition().getGlobalCartesian().X > t_guard.getRightPatrolPoint())
						{
							t_guard.goLeft();
						}
						else if (t_guard.getHorizontalSpeed() == 0)
						{
							if (t_guard.isFacingRight())
							{
								t_guard.goRight();
							}
							else
							{
								t_guard.goLeft();
							}
						}
					}
					else
					{
						float t_guardPoint = t_guard.getLeftPatrolPoint();
						if (t_guard.getPosition().getGlobalCartesian().X - 10 > t_guardPoint)
						{
							if (t_guard.getHorizontalSpeed() >= 0)
							{
								t_guard.goLeft();
							}
						}
						else if (t_guard.getPosition().getGlobalCartesian().X + 10 < t_guardPoint)
						{
							if (t_guard.getHorizontalSpeed() <= 0)
							{
								t_guard.goRight();
							}
						}
						else
						{
							if (t_guard.getHorizontalSpeed() == 0)
							{
								if (t_guard.guardFaceRight())
								{
									if (!t_guard.isFacingRight())
									{
										t_guard.goRight();
									}
								}
								else if(t_guard.isFacingRight())
								{
									t_guard.goLeft();
								}
							}
							else
							{
								t_guard.stop();
							}
						}
					}
					return this;
				}
			}
			else if (a_agent is GuardDog)
			{
				GuardDog t_guardDog = (GuardDog)a_agent;
				if (t_guardDog.canSensePlayer())
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
						t_guardDog.setFacing(t_guardDog.getChargeingPoint() > t_guardDog.getPosition().getGlobalX());
						return AIStateChargeing.getInstance();
					}
				}
				else
				{
					if (t_guardDog.haspatrol())
					{
						if (t_guardDog.isChargeing())
						{
							t_guardDog.setChargeing(false);
						}
						if (t_guardDog.getPosition().getGlobalCartesian().X < t_guardDog.getLeftPatrolPoint())
						{
							if (t_guardDog.getHorizontalSpeed() <= 0)
							{
								t_guardDog.goRight();
							}
						}
						else if (t_guardDog.getPosition().getGlobalCartesian().X > t_guardDog.getRightPatrolPoint())
						{
							if (t_guardDog.getHorizontalSpeed() >= 0)
							{
								t_guardDog.goLeft();
							}
						}
						else if (t_guardDog.getHorizontalSpeed() == 0)
						{
							t_guardDog.goLeft();
						}
					}
					else
					{
						float t_guardPoint = t_guardDog.getLeftPatrolPoint();
						if (t_guardDog.getPosition().getGlobalCartesian().X - 10 > t_guardPoint)
						{
							if (t_guardDog.getHorizontalSpeed() >= 0)
							{
								t_guardDog.goRight();
							}
						}
						else if (t_guardDog.getPosition().getGlobalCartesian().X + 10 < t_guardPoint)
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
			else if(a_agent is GuardCamera)
			{
				GuardCamera t_gc = (GuardCamera)a_agent;
				if (t_gc.canSeePlayer())
				{
					t_gc.chasePlayer();
					return AIStateChasing.getInstance();
				}
				else
				{
					if (t_gc.getRotation() < t_gc.getLeftRotationPoint() && t_gc.getRotation() > t_gc.getRightRotationPoint())
					{
						if (t_gc.getRotationSpeed() == 0 && t_gc.isTurnReady())
						{
							t_gc.rotateCounter();
						}
					}
					else
					{
						float t_rotationGoal = ((t_gc.getLeftRotationPoint() + t_gc.getRightRotationPoint()) / 2f) % ((float)(Math.PI * 2));
						bool t_goClock = (t_gc.getRotation() > t_rotationGoal + Math.PI) || ((t_gc.getRotation() > t_rotationGoal - Math.PI) && (t_gc.getRotation() < t_rotationGoal));
						if (t_gc.getRotationSpeed() == 0 && t_gc.isTurnReady())
						{
							if (t_goClock)
							{
								t_gc.rotateClockW();
							}
							else
							{
								t_gc.rotateCounter();
							}
						}
						else if ((t_goClock && t_gc.getRotationSpeed() < 0) || ((!t_goClock) && t_gc.getRotationSpeed() > 0))
						{
							t_gc.stop();
						}
					}
					return this;
				}
			}
			else
			{
				throw new ArgumentException("Only guards, guardcameras and guarddogs can patrol");
			}
		}
	}
}
