using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
{
	[Serializable()]
	class AIStateBark : AIState
	{
		//hur långt ifrån hunden vakterna hör hundens skall.
		private float BARKRADIUS = 1000;

		//hur långt ifrån spelaren hunden måste vara innan han kan börja skälla.
		static public float BARKDISTANCE = 300;

		static private AIStateBark s_instance;
		private AIStateBark() { }
		static public AIStateBark getInstance()
		{
			if (s_instance == null)
			{
				s_instance = new AIStateBark();
			}
			return s_instance;
		}

		public override AIState execute(NPE a_agent)
		{
			if (a_agent is GuardDog)
			{
				GuardDog t_guardDog = (GuardDog)a_agent;
				throw new NotImplementedException();
			}
			else
			{
				throw new ArgumentException("Only dogs can bark");
			}
		}
	}
}
