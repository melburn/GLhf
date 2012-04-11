using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public class GuiObject : GameObject
	{
		private Rectangle m_bounds;
		private string m_guiSprite;

		public GuiObject(Vector2 a_posV2, string a_sprite) : base(a_posV2, null, 0.002f)
		{
			m_position = new CartesianCoordinate(a_posV2 - Game.getInstance().getResolution() / 2, Game.getInstance().m_camera.getPosition());
			m_bounds.X = (int)a_posV2.X;
			m_bounds.Y = (int)a_posV2.Y;
			m_guiSprite = a_sprite;
		}

		public override void loadContent()
		{
			if (m_guiSprite != null) {
				setSprite("Images//GUI//" + m_guiSprite);			
			}
			base.loadContent();
		}

		public override void draw(GameTime a_gameTime)
		{
			float t_zoom = Game.getInstance().m_camera.getZoom();
			Vector2 t_cartCoord;
			t_cartCoord.X = m_position.getLocalX() / t_zoom + m_position.getParentPosition().getGlobalX();
			t_cartCoord.Y = m_position.getLocalY() / t_zoom + m_position.getParentPosition().getGlobalY();

			m_img.draw(t_cartCoord, m_rotate, m_rotationPoint, m_color, m_spriteEffects, m_layer, 1.0f / t_zoom, 1.0f / t_zoom);
		}

		public override Rectangle getBox() {
			if (m_bounds.Width == 0 || m_bounds.Height == 0) {
				m_bounds.Width = (int)m_img.getSize().X;
				m_bounds.Height = (int)m_img.getSize().Y;
			}
			return m_bounds;
		}

		public override void update(GameTime a_gameTime) {

		}

		public void setSprite(string a_path)
		{
			m_img.setSprite("Images//GUI//" + a_path);
		}
	}
}
