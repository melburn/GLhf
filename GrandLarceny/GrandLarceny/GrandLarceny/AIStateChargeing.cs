using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
{
	[Serializable()]
	public class AIStateChargeing : AIState
	{
		static public float CHARGEDISTANCE = 200;
		private static AIStateChargeing s_instance;
		private AIStateChargeing() {}

		public static AIStateChargeing getInstance()
		{
			if (s_instance == null)
			{
				s_instance = new AIStateChargeing();
			}
			return s_instance;
		}

		public override AIState execute(NPE a_agent)
		{
			if (a_agent is GuardDog)
			{
				GuardDog t_guardDog = (GuardDog)a_agent;
				if (t_guardDog.isFaceingTowards(t_guardDog.getChargeingPoint()))
				{
					t_guardDog.setChargeing(true);
					t_guardDog.goRight();
					return this;
				}
				else
				{
					t_guardDog.setChargeing(false);
					return AIStatepatroling.getInstance();
				}
			}
			else
			{
				throw new ArgumentException("Only dogs can charge. noob.");
			}
		}
	}
}
