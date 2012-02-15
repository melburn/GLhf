using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public class GuiObject : GameObject
	{
		private Vector2 m_resolution;
		public GuiObject(Vector2 a_posV2, String a_sprite)
			: base(a_posV2, a_sprite, 0.001f) {
			m_resolution = new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth, Game.getInstance().m_graphics.PreferredBackBufferHeight);
		}

		public override void draw(GameTime a_gameTime)
		{
			float t_zoom = Game.getInstance().m_camera.getZoom();
			m_img.draw(new CartesianCoordinate((m_position.getLocalCartesianCoordinates() - m_resolution / 2) / t_zoom, m_position.getParentPosition()), m_rotate, m_color, m_spriteEffects, m_layer, 1.0f / t_zoom, 1.0f / t_zoom);
		}
	}
}
