using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class Ladder : NonMovingPlatform
	{
		public Ladder(Vector2 a_posV2, String a_sprite) : base(a_posV2, a_sprite)
		{
			
		}
	}
}
