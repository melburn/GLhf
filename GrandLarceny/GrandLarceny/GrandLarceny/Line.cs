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
		private Position m_leftPosition;
		private Position m_rightPosition;
		private Color m_lineColor;

		public Line(Position a_leftPoint, Position a_rightPoint, Color a_color) {
			m_lineTexture = new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			m_leftPosition = a_leftPoint;
			m_rightPosition = a_rightPoint;
			m_lineColor = a_color;
			m_lineTexture.SetData(new[] { a_color });
		}

		public Line(Vector2 a_leftPoint, Vector2 a_rightPoint, Color a_color) {
			m_lineTexture = new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			m_leftPosition = new CartesianCoordinate(a_leftPoint);
			m_rightPosition = new CartesianCoordinate(a_rightPoint);
			m_lineColor = a_color;
			m_lineTexture.SetData(new[] { a_color });
		}

		public Line(Position a_startPos, float a_endPos, Color a_color, bool a_rightLine) {
			m_lineTexture = new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			if (a_rightLine) {
				m_leftPosition = new CartesianCoordinate(new Vector2(36, 36), a_startPos);
				m_rightPosition = new CartesianCoordinate(new Vector2(a_endPos - m_leftPosition.getGlobalX() + 36, 0), m_leftPosition);
			} else {
				m_rightPosition = new CartesianCoordinate(new Vector2(36, 36), a_startPos);
				m_leftPosition = new CartesianCoordinate(new Vector2(a_endPos - m_rightPosition.getGlobalX() + 36, 0), m_rightPosition);
			}
			m_lineColor = a_color;
			m_lineTexture.SetData(new[] { a_color });
		}

		public void draw() {
			float t_angle = (float)Math.Atan2(m_rightPosition.getGlobalY() - m_leftPosition.getGlobalY(), m_rightPosition.getGlobalX() - m_leftPosition.getGlobalX());
			float t_length = Vector2.Distance(m_leftPosition.getGlobalCartesianCoordinates(), m_rightPosition.getGlobalCartesianCoordinates());

			Game.getInstance().getSpriteBatch().Draw(m_lineTexture, m_leftPosition.getGlobalCartesianCoordinates(), null, m_lineColor, t_angle, Vector2.Zero, new Vector2(t_length, 5), SpriteEffects.None, 0.000f);
		}
	}
}
