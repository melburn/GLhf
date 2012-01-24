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
		private int m_animationHeight;
		private int m_animationFrames;
		private int m_currentAnimationColumn = 0;
		private int m_currentAnimationRow = 0;
		
		public ImageManager(Texture2D a_image, int a_animationWidth, int a_animationHeight, int a_animationFrames) {
			m_image = a_image;
			m_animationWidth	= a_animationWidth;
			m_animationHeight	= a_animationHeight;
			m_animationFrames	= a_animationFrames;
		}

		public void draw(Position a_position, float a_rotation, Color a_color, SpriteEffects a_spriteEffect, int a_layer) {
			if (a_spriteEffect == null) {
				a_spriteEffect = SpriteEffects.None;
			}
			if (a_layer == null) {
				a_layer = 0;
			}
			if (a_color == null) {
				a_color = Color.White;
			}
			Vector2 t_worldPosV2 = a_position.getGlobalCartesianCoordinates();

			Game.getInstance().getSpriteBatch().Draw(
				m_image,
				new Rectangle((int)t_worldPosV2.X, (int)t_worldPosV2.Y, m_animationWidth, m_animationHeight),
				null,
				a_color, 
				a_rotation,
				new Vector2(m_animationWidth / 2, m_animationHeight / 2),
				a_spriteEffect,
				a_layer
			);
			/*
			            Game1.getInstace().getSpriteBatch().Draw(t_img.getImage(),
                new Rectangle((int)m_position.X, (int)m_position.Y, t_img.getWidth(), t_img.getHeight()), //destination
                new Rectangle(t_img.getWidth() * m_currAniFrame, 0, t_img.getWidth(), t_img.getHeight()), //source
                Color.White, m_rotation, new Vector2(0,0), t_se, 0);									  //färg, rotation, origin, spriteeffect, lager
			*/ 
		}
	}
}
