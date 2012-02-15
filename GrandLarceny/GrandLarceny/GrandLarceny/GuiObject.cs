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

		public GuiObject(Vector2 a_posV2, String a_sprite) : base(a_posV2, a_sprite, 0.002f)
		{
			m_resolution = new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth, Game.getInstance().m_graphics.PreferredBackBufferHeight);
			m_position.setParentPosition(Game.getInstance().m_camera.getPosition());
			m_position.setX(m_position.getGlobalX() - m_resolution.X / 2);
			m_position.setY(m_position.getGlobalY() - m_resolution.Y / 2);
		}

		public override void draw(GameTime a_gameTime)
		{
			float t_zoom = Game.getInstance().m_camera.getZoom();
			CartesianCoordinate t_cartCoord = new CartesianCoordinate(m_position.getLocalCartesianCoordinates() / t_zoom, m_position.getParentPosition());
			m_img.draw(t_cartCoord, m_rotate, m_color, m_spriteEffects, m_layer, 1.0f / t_zoom, 1.0f / t_zoom);
		}
	}
}
