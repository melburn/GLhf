using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public class Entity : GameObject
	{
        //pixel per sekund
        protected Vector2 m_speed;

		//pixel per sekund per sekund
		protected float m_gravity = 0f;

		protected Vector2 m_lastPosition;

		public Entity(Vector2 a_posV2, String a_sprite)
			: base(a_posV2, a_sprite)
		{
		}

        public override void update(GameTime a_gameTime)
        {
			m_lastPosition = m_position.getGlobalCartesianCoordinates();
            base.update(a_gameTime);

			float t_deltaTime = ((float)(a_gameTime.ElapsedGameTime.Milliseconds)) / 1000.0f;
			m_speed.Y += m_gravity * t_deltaTime;
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
		internal Vector2 getLastPosition()
		{
			return m_lastPosition;
		}
        internal float getHorizontalSpeed()
        {
            return m_speed.X;
        }
	}
}
