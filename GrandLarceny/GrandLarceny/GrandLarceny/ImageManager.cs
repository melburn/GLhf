using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	class ImageManager
	{
		private Texture2D m_image;
		private int m_animationWidth;
		private int m_animationFrames;
        public bool m_looping;
        private bool m_stopped = false;

        //millisecond per frame
        public float m_animationSpeed;

        //säger vilken subbild i animationen den ligger på, med decimaler.
		public float m_subImageNumber;

		public ImageManager(String a_sprite)
		{
			setSprite(a_sprite);
			m_animationSpeed = 33f;
			m_looping = true;
		}

		public void update(GameTime a_gameTime)
		{
			if (!m_stopped)
			{
				m_subImageNumber += m_animationSpeed * a_gameTime.ElapsedGameTime.Milliseconds;
				if (m_subImageNumber >= m_animationFrames)
				{
					if (m_looping)
					{
						m_subImageNumber = m_subImageNumber % m_animationFrames;
					}
					else
					{
						m_subImageNumber = m_animationFrames;
						m_stopped = true;
					}
				}
			}
		}

		public void draw(Position a_position, float a_rotation, Color a_color, SpriteEffects a_spriteEffect = SpriteEffects.None, int a_layer = 0) {
			if (a_color == null)
			{
				a_color = Color.White;
			}
			Vector2 t_worldPosV2 = a_position.getGlobalCartesianCoordinates();

			Game.getInstance().getSpriteBatch().Draw(
				m_image,
				new Rectangle((int)t_worldPosV2.X, (int)t_worldPosV2.Y, m_animationWidth, m_image.Height),
                new Rectangle(m_animationWidth * ((int)(m_subImageNumber)), 0, m_image.Width, m_image.Height),
				a_color,
				a_rotation,
                new Vector2(m_animationWidth / 2, m_image.Height / 2),
				a_spriteEffect,
				a_layer
			);
		}

		public void setSprite(String a_sprite)
		{
			if (a_sprite == null)
			{
				m_image = null;
				m_stopped = true;
			}
			else
			{
				m_stopped = false;
				m_image = Game.getInstance().Content.Load<Texture2D>(a_sprite);
				m_animationFrames = Loader.getInstance().getAnimationFrames(a_sprite);
				m_animationWidth = m_image.Width / m_animationFrames;
				m_subImageNumber = 0;
			}
		}

		public void stop()
		{
			m_stopped = true;
		}
	
		public void run()
		{
			m_stopped = (m_image == null);
		}

		public Vector2 getSize() {
			return new Vector2(m_animationWidth, m_image.Height);
		}
	}
}
