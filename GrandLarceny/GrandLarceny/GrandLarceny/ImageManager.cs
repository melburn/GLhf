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
		private int m_currentAnimationColumn = 0;
		private int m_currentAnimationRow = 0;

        public ImageManager(String a_sprite)
        {
            m_image = Game.getInstance().Content.Load<Texture2D>(a_sprite);
            m_animationFrames = Game.getInstance().getAnimationFrames(a_sprite);
            m_animationWidth = m_image.Width / m_animationFrames;
            
        }
        

		public void draw(Position a_position, float a_rotation, Color a_color, SpriteEffects a_spriteEffect = SpriteEffects.None, int a_layer = 0) {
			if (a_color == null) {
				a_color = Color.White;
			}
			Vector2 t_worldPosV2 = a_position.getGlobalCartesianCoordinates();

			Game.getInstance().getSpriteBatch().Draw(
				m_image,
				new Rectangle((int)t_worldPosV2.X, (int)t_worldPosV2.Y, m_animationWidth, m_image.Height),
                new Rectangle(m_animationWidth * m_animationFrames, 0, m_image.Width, m_image.Height),
				a_color, 
				a_rotation,
                new Vector2(m_animationWidth / 2, m_image.Height / 2),
				a_spriteEffect,
				a_layer
			);
		}
	}
}
