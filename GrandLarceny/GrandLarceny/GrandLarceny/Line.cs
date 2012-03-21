using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	public class Line
	{
		private Texture2D m_lineTexture;
		private Position m_startPosition;
		private Position m_endPosition;
		private Color m_lineColor;
		private Vector2 m_startOffset;
		private Vector2 m_endOffset;
		private int m_width;
		private bool m_worldLine;
		
		public Line(Position a_startPosition, Position a_endPosition, Vector2 a_startOffset, Vector2 a_endOffset, Color a_color, int a_width, bool a_worldLine)	{
			m_startOffset = a_startOffset;
			m_endOffset = a_endOffset;
			m_lineTexture = new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			if (a_worldLine) {
				m_startPosition = new CartesianCoordinate(a_startOffset, a_startPosition);
				m_endPosition = new CartesianCoordinate(a_endOffset, a_endPosition);
			} else {
				m_startPosition = new CartesianCoordinate(a_startOffset, Game.getInstance().m_camera.getPosition());
				m_endPosition = new CartesianCoordinate(a_endOffset, Game.getInstance().m_camera.getPosition());
			}
			m_lineColor = a_color;
			m_lineTexture.SetData(new[] { a_color });
			m_width = a_width;
			m_worldLine = a_worldLine;
		}

		public void setEndpoint(Vector2 a_endPoint) {
			m_endPosition = new CartesianCoordinate(a_endPoint);
			m_endPosition.plusWith(m_endOffset);
		}

		public void draw() {
			float t_zoom = Game.getInstance().m_camera.getZoom();
			float t_angle = (float)Math.Atan2(m_endPosition.getGlobalY() - m_startPosition.getGlobalY(), m_endPosition.getGlobalX() - m_startPosition.getGlobalX());
			float t_length = Vector2.Distance(m_startPosition.getGlobalCartesianCoordinates() / t_zoom, m_endPosition.getGlobalCartesianCoordinates() / t_zoom);

			if (m_worldLine) {
				Game.getInstance().getSpriteBatch().Draw(m_lineTexture, m_startPosition.getGlobalCartesianCoordinates(), null, m_lineColor, t_angle, Vector2.Zero, new Vector2(t_length, m_width / t_zoom), SpriteEffects.None, 0.010f);
			} else {
				Vector2 t_cartCoord;
				t_cartCoord.X = m_startPosition.getLocalX() / t_zoom + Game.getInstance().m_camera.getPosition().getGlobalX();
				t_cartCoord.Y = m_startPosition.getLocalY() / t_zoom + Game.getInstance().m_camera.getPosition().getGlobalY();

				Game.getInstance().getSpriteBatch().Draw(m_lineTexture, t_cartCoord, null, m_lineColor, t_angle, Vector2.Zero, new Vector2(t_length, m_width / t_zoom), SpriteEffects.None, 0.010f);
			}
		}
	}
}
