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
        protected bool m_dead = false;
        protected Position m_position;
        protected ImageManager m_img;

        protected float m_rotate;
        protected int m_layer;
        protected Color m_color;
        protected SpriteEffects m_spriteEffects;

		public GameObject(Vector2 a_posV2, String a_sprite)
		{
			m_position = new CartesianCoordinate(a_posV2);
            m_img = new ImageManager(a_sprite);
			m_rotate = 0.0f;
			m_layer = 0;
			m_color = Color.White;
			m_spriteEffects = SpriteEffects.None;
		}

		public virtual void update(GameTime a_gameTime)
		{
            m_img.update(a_gameTime);
		}

		public virtual void draw(GameTime a_gameTime)
		{
            m_img.draw(m_position, m_rotate, m_color, m_spriteEffects, m_layer);
		}
		public bool isDead()
		{
			return m_dead;
		}
		public void setLayer(int a_layer) {
			m_layer = a_layer;
		}
	}
}
