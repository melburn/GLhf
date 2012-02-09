using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	class Environment : GameObject
	{
		public Environment(Vector2 a_posV2, String a_sprite)
			: base(a_posV2, a_sprite)
		{
			
		}
	}
}
