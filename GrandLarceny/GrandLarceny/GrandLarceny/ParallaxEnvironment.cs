using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class ParallaxEnvironment : Environment
	{
		private int m_parallaxScroll;
		private Vector2 m_halfImage;
		
		public ParallaxEnvironment(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			m_parallaxScroll = 0;
			m_mapVisible = false;
		}

		public override void loadContent()
		{
 			base.loadContent();
			m_halfImage = m_img.getSize() / 2;
		}

		public override void draw(GameTime a_gameTime)
		{
			if (m_visible)
			{
				if (Game.getInstance().getState() is DevelopmentState)
				{
					m_img.draw(m_position.getGlobalCartesian(), m_rotate, m_rotationPoint, m_color, m_spriteEffects, m_layer, m_XScale, m_YScale);
				}
				else
				{
					float t_scroll = ((((float)m_parallaxScroll) / 1000f) * (m_position.getGlobalX() + m_halfImage.X - Game.getInstance().m_camera.getPosition().getGlobalX()));
					float t_yscroll = ((((float)m_parallaxScroll) / 1000f) * (m_position.getGlobalY() + m_halfImage.Y - Game.getInstance().m_camera.getPosition().getGlobalY()));
					Vector2 t_imgPosition = new Vector2(m_position.getGlobalX() + m_imgOffsetX + t_scroll, m_position.getGlobalY() + m_imgOffsetY + t_yscroll);

					m_img.draw(t_imgPosition, m_rotate, m_rotationPoint, m_color, m_spriteEffects, m_layer - m_parallaxScroll / 100000f, m_XScale, m_YScale);
				}

				if (m_changePositionAfterDraw != Vector2.Zero)
				{
					m_position.plusWith(m_changePositionAfterDraw);
					m_changePositionAfterDraw = Vector2.Zero;
				}
			}
		}

		public void setParrScroll(int a_depth)
		{
			m_parallaxScroll = a_depth;
		}

		public int getParrScroll()
		{
			return m_parallaxScroll;
		}
	}
}
