using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class NonMovingPlatform : NPE
	{
		public NonMovingPlatform(Vector2 a_posV2, String a_sprite) : base(a_posV2, a_sprite)
		{
			
		}
	}
}
