using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
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
				throw new NotImplementedException();
				//return this;
			}
			else
			{
				throw new ArgumentException("Only guards can go to the switch");
			}
		}
	}
}
