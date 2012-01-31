using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class Entity : GameObject
	{
        //pixel per sekund
        protected Vector2 m_speed;
		protected float t_deltaTime;

        //pixel per sekund per sekund

		public Entity(Vector2 a_posV2, String a_sprite)
			: base(a_posV2, a_sprite)
		{
			
		}

        public override void update(GameTime a_gameTime)
        {
            base.update(a_gameTime);

            t_deltaTime = ((float) (a_gameTime.ElapsedGameTime.Milliseconds)) / 1000.0f;
            m_position.plusWith(m_speed * t_deltaTime);
        }

    /*  public override void collisionCheck()
        {

        }
		*/
		
        public override void draw(GameTime a_gameTime)
        {
			base.draw(a_gameTime);
        }
	}
}
