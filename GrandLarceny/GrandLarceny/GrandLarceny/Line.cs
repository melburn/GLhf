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

		public Line(Position a_startPosition, Position a_endPosition, Vector2 a_startOffset, Vector2 a_endOffset, Color a_color) {
			m_startOffset = a_startOffset;
			m_endOffset = a_endOffset;
			m_lineTexture = new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			m_startPosition = new CartesianCoordinate(a_startOffset, a_startPosition);
			m_endPosition = new CartesianCoordinate(a_endOffset, a_endPosition);
			m_lineColor = a_color;
			m_lineTexture.SetData(new[] { a_color });
		}
		/*
		public Line(Vector2 a_startPoint, Vector2 a_endPoint, Vector2 a_startOffset, Vector2 a_endOffset, Color a_color) {
			m_lineTexture = new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			m_startPosition = new CartesianCoordinate(a_startPoint);
			m_endPosition = new CartesianCoordinate(a_endPoint);
			m_lineColor = a_color;
			m_lineTexture.SetData(new[] { a_color });
		}

		public Line(Position a_startPos, float a_endPos, Vector2 a_startOffset, Vector2 a_endOffset, Color a_color) {
			m_lineTexture = new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			m_startOffset = a_startOffset;
			m_endOffset = a_endOffset;
			m_startPosition = new CartesianCoordinate(m_startOffset, a_startPos);
			m_endPosition = new CartesianCoordinate(m_endOffset, m_startPosition);
			m_lineColor = a_color;
			m_lineTexture.SetData(new[] { a_color });
		}
		*/
		public void setOffset(Vector2 a_offset) {
			m_startOffset = a_offset;
		}

		public void setEndpoint(Vector2 a_endPoint) {
			m_endPosition = new CartesianCoordinate(a_endPoint);
			m_endPosition.plusWith(m_endOffset);
		}

		public void draw() {
			float t_angle = (float)Math.Atan2(m_endPosition.getGlobalY() - m_startPosition.getGlobalY(), m_endPosition.getGlobalX() - m_startPosition.getGlobalX());
			float t_length = Vector2.Distance(m_startPosition.getGlobalCartesianCoordinates(), m_endPosition.getGlobalCartesianCoordinates());

			Game.getInstance().getSpriteBatch().Draw(m_lineTexture, m_startPosition.getGlobalCartesianCoordinates(), null, m_lineColor, t_angle, Vector2.Zero, new Vector2(t_length, 5), SpriteEffects.None, 0.000f);
		}
	}
}
