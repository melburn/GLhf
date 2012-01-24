using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class GameObject
	{
		private bool m_dead = false;
        private Position m_myPos;
        private ImageManager m_img;

		public GameObject(Vector2 a_posV2, ImageManager a_img)
		{
			m_myPos = new CartesianCoordinate(a_posV2);
            m_img = a_img;
		}

		public virtual void update(GameTime a_gameTime)
		{
            m_myPos.rotate(a_gameTime.ElapsedGameTime.Milliseconds);
            //m_myPos.plusWith(new Vector2(-3,-3));
		}

		public virtual void draw(GameTime a_gameTime)
		{
            m_img.draw(m_myPos, 0f, Color.White, new Microsoft.Xna.Framework.Graphics.SpriteEffects(), 1);
		}
		public bool isDead()
		{
			return m_dead;
		}
	}
}
