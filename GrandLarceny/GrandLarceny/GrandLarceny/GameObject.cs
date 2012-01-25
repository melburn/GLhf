using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	class GameObject
	{
		private bool m_dead = false;
        private Position m_myPos;
        private ImageManager m_img;

        private float m_rotate;
        private int m_layer;
        private Color m_color;
        private SpriteEffects m_spriteEffects;

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
            m_img.draw(m_myPos, m_rotate, m_color, m_spriteEffects, 1);
		}
		public bool isDead()
		{
			return m_dead;
		}
	}
}
