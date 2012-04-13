﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny.AI
{
	[Serializable()]
	public class AIStateGoingToTheSwitch : AIState
	{
		private AIStateGoingToTheSwitch()
		{
		}
		private static AIStateGoingToTheSwitch instance;
		public static AIStateGoingToTheSwitch getInstance()
		{
			if (instance == null)
			{
				instance = new AIStateGoingToTheSwitch();
			}
			return instance;
		}

		public override AIState execute(NPE a_agent)
		{
			if(a_agent==null)
            {
                throw new ArgumentNullException("The Agent cannot be null");
            }
			if (a_agent is Guard)
			{
				Guard t_guard = (Guard)a_agent;
				if (t_guard.hasNoLampSwitchTargets())
				{
					return AIStatepatroling.getInstance();
				}
				else
				{
					if (!t_guard.isCarryingFlash() && t_guard.hasFlash())
					{
						t_guard.toggleFlashLight();
					}
					else
					{
						LampSwitch t_lampSwitch = t_guard.getFirstLampSwitchTarget();
						while (Math.Abs(t_lampSwitch.getPosition().getGlobalX() - a_agent.getPosition().getGlobalX()) < 10)
						{
							if (!t_lampSwitch.isOn())
							{
								t_lampSwitch.toggleSwitch();
							}
							foreach (GameObject t_g in Game.getInstance().getState().getCurrentList())
							{
								if (t_g is Guard)
								{
									((Guard)t_guard).removeLampSwitchTarget(t_lampSwitch);
								}
							}
							if (t_guard.hasNoLampSwitchTargets())
							{
								return AIStatepatroling.getInstance();
							}
							t_lampSwitch = t_guard.getFirstLampSwitchTarget();
						}
						if (t_guard.isRunning())
						{
							t_guard.setRunning(false);
						}
						else if (t_guard.getPosition().getGlobalX() < t_lampSwitch.getPosition().getGlobalX())
						{
							if (t_guard.getHorizontalSpeed() <= 0)
							{
								t_guard.goRight();
							}
						}
						else if (t_guard.getPosition().getGlobalX() > t_lampSwitch.getPosition().getGlobalX())
						{
							if (t_guard.getHorizontalSpeed() >= 0)
							{
								t_guard.goLeft();
							}
						}
					}
					return this;
				}
			}
			else
			{
				throw new ArgumentException("Only guards can go to the switch");
			}
		}
	}
}
