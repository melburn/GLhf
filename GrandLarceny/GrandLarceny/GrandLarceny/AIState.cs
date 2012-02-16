using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
{
	[Serializable()]
	public abstract class AIState
	{
		abstract public AIState execute(NPE npe);
	}
}
