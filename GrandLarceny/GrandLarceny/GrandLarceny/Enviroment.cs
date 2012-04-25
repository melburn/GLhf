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
		private float m_parallaxScroll;

		public Environment(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			m_explored = false;
			m_parallaxScroll = 0;
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
				Vector2 t_imgPosition = new Vector2(
				m_position.getGlobalX() + m_imgOffsetX + (m_parallaxScroll * (m_position.getGlobalX() + (m_img.getSize().X / 2) - Game.getInstance().m_camera.getPosition().getGlobalX())),
				m_position.getGlobalY() + m_imgOffsetY);

				m_img.draw(t_imgPosition, m_rotate, m_rotationPoint, m_color, m_spriteEffects, m_layer, m_XScale, m_YScale);

				if (m_changePositionAfterDraw != Vector2.Zero)
				{
					m_position.plusWith(m_changePositionAfterDraw);
					m_changePositionAfterDraw = Vector2.Zero;
				}
			}
		}

		public void setParrScroll(float a_depth)
		{
			m_parallaxScroll = a_depth;
		}
	}
}
