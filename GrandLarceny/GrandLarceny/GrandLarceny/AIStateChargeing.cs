using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
{
	[Serializable()]
	public class AIStateChargeing : AIState
	{
		static public float CHARGEDISTANCE = 400;
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
				if (t_guardDog.isFacingTowards(t_guardDog.getChargeingPoint()))
				{
					if (! t_guardDog.isChargeing())
					{
						t_guardDog.setChargeing(true);
					}
					if (t_guardDog.ifFaceingRight())
					{
						if (t_guardDog.getHorizontalSpeed() <= 0)
						{
							t_guardDog.goRight();
						}
					}
					else
					{
						if (t_guardDog.getHorizontalSpeed() >= 0)
						{
							t_guardDog.goLeft();
						}
					}
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
