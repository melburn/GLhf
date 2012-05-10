using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny.AI
{
	[Serializable()]
	public abstract class AIState
	{
		abstract public AIState execute(NPE a_agent, GameTime a_gameTime);
	}
}
