﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny.Events
{
	[Serializable()]
	public abstract class EventTrigger
	{
		public abstract bool isTrue();
		public virtual void linkObject() { }
		public virtual void loadContent() { }
	}
}
