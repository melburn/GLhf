using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class Entity : GameObject
	{
		public Entity(Vector2 a_posV2, String a_sprite)
			: base(a_posV2, a_sprite)
		{

		}

        public virtual override void update(GameTime a_gameTime)
        {

        }

        public virtual override void draw(GameTime a_gameTime)
        {

        }
	}
}
