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

        protected Vector2 m_speed;

		public GameObject(Vector2 a_posV2, Texture2D a_img, int a_animationWidth,  int a_animationHeight, int a_animationFrames)
		{
			m_position = new CartesianCoordinate(a_posV2);
            m_img = new ImageManager(a_img, a_animationWidth, a_animationHeight, a_animationFrames);
		}

		public virtual void update(GameTime a_gameTime)
		{
		}

		public virtual void draw(GameTime a_gameTime)
		{
            m_img.draw(m_position, m_rotate, m_color, m_spriteEffects, 1);
		}
		public bool isDead()
		{
			return m_dead;
		}
	}
}
