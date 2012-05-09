using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class CoveringShadow : NonMovingObject
	{
		public CoveringShadow(Vector2 a_position, string a_sprite, float a_layer) : base(a_position, a_sprite, a_layer)
		{
			
		}
	}
}
