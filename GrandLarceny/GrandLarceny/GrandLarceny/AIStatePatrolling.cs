﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
{
	[Serializable()]
	public class AIStatePatrolling : AIState
	{
		private static AIStatePatrolling instance;
		public static AIStatePatrolling getInstance()
		{
			if(instance == null)
			{
				instance = new AIStatePatrolling();
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
					((Guard)a_agent).chasePlayer();
					return AIStateChasing.getInstance();
				}
				else
				{
					if (t_guard.hasPatroll())
					{
						if (t_guard.getPosition().getGlobalCartesianCoordinates().X < t_guard.getLeftPatrollPoint() && t_guard.getHorizontalSpeed() <= 0)
						{
							t_guard.goRight();
						}
						else if (t_guard.getPosition().getGlobalCartesianCoordinates().X > t_guard.getRightPatrollPoint() && t_guard.getHorizontalSpeed() >= 0)
						{
							t_guard.goLeft();
						}
					}
					else
					{
						float t_guardPoint = t_guard.getLeftPatrollPoint();
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
			else
			{
				throw new ArgumentException("Only guards can patroll");
			}
		}
	}
}
