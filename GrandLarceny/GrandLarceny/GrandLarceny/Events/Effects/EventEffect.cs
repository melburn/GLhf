using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny.Events
{
	[Serializable()]
	public abstract class EventEffect
	{
		public abstract void execute();
		public abstract override string ToString();
	}
}
