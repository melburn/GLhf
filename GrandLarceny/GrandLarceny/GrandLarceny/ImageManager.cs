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

        //millisecond per frame
        private float m_animationSpeed;

        //säger vilken subbild i animationen den ligger på, med decimaler.
		private float m_subImageNumber;

        public ImageManager(String a_sprite)
        {
            m_image = Game.getInstance().Content.Load<Texture2D>(a_sprite);
            m_animationFrames = Loader.getInstance().getAnimationFrames(a_sprite);
            m_animationWidth = m_image.Width / m_animationFrames;
            m_animationSpeed = 33f;
        }
        
        public void update(GameTime a_gameTime)
        {
            m_subImageNumber += m_animationSpeed*a_gameTime.ElapsedGameTime.Milliseconds;
        }

		public void draw(Position a_position, float a_rotation, Color a_color, SpriteEffects a_spriteEffect = SpriteEffects.None, int a_layer = 0) {
			if (a_color == null) {
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
	}
}
