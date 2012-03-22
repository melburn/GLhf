using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny.Events
{
	[Serializable()]
	abstract class EventTrigger
	{
		public abstract bool isTrue();
		public abstract override string ToString();
	}
}
