using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
{
	class AIStateStriking : AIState
	{
		private AIStateStriking()
		{
		}
		private static AIStateStriking instance;
		private const float MINIMUMDISTANCE = 40;
		public static AIStateStriking getInstance()
		{
			if (instance == null)
			{
				instance = new AIStateStriking();
			}
			return instance;
		}

		public override AIState execute(NPE a_agent)
		{
			if (a_agent is Guard)
			{
				Guard t_guard = (Guard)a_agent;
				if (!t_guard.isStriking())
				{
					if (t_guard.canStrike())
					{
						t_guard.strike();
					}
					else
					{
						return AIStateChargeing.getInstance();
					}
				}
				return this;
			}
			else
			{
				throw new ArgumentException("bara guards kan strejka");
			}
		}
	}
}
