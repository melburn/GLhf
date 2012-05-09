using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	public class Environment : GameObject
	{
		private Boolean m_explored;
		private int m_parallaxScroll;

		public Environment(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			m_explored = false;
			m_parallaxScroll = 0;
		}

		public override void loadContent()
		{
			base.loadContent();
			m_layer = 0.950f;
		}

		public bool isExplored()
		{
			return m_explored;
		}
		public void setExplored(Boolean a_explored)
		{
			m_explored = a_explored;
		}
		public bool collidesWith(GameObject a_gameObject)
		{
			return a_gameObject.getBox().Intersects(getBox());
		}
		public override void draw(GameTime a_gameTime)
		{
			if (m_visible)
			{
				float t_scroll = ((((float)m_parallaxScroll) / 1000f) * (m_position.getGlobalX() + (m_img.getSize().X / 2) - Game.getInstance().m_camera.getPosition().getGlobalX()));
				float t_yscroll = ((((float)m_parallaxScroll) / 1000f) * (m_position.getGlobalY() + (m_img.getSize().Y / 2) - Game.getInstance().m_camera.getPosition().getGlobalY()));
				Vector2 t_imgPosition = new Vector2(m_position.getGlobalX() + m_imgOffsetX + t_scroll, m_position.getGlobalY() + m_imgOffsetY + t_yscroll);

				m_img.draw(t_imgPosition, m_rotate, m_rotationPoint, m_color, m_spriteEffects, m_layer - m_parallaxScroll / 100000f, m_XScale, m_YScale);

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
		public CollisionShape getImageBox()
		{
			return new CollisionRectangle(0, 0, m_img.getSize().X, m_img.getSize().Y, m_position);
		}
	}
}
