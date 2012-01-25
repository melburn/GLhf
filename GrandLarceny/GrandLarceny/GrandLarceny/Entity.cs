using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class Entity : GameObject
	{

        protected Vector2 m_speed;

		public Entity(Vector2 a_posV2, String a_sprite)
			: base(a_posV2, a_sprite)
		{

		}

        public override void update(GameTime a_gameTime)
        {
            base.update(a_gameTime);
            m_position.plusWith(m_speed);
        }

        public override void draw(GameTime a_gameTime)
        {

        }
	}
}
