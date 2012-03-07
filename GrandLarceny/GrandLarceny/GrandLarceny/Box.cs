using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	class Box
	{
		private Texture2D			m_boxTexture;
		private Position			m_position;
		private int					m_width;
		private int					m_height;
		private LinkedList<Line>	m_lineList;
		private Color				m_boxColor;

		public Box(Vector2 a_position, int a_width, int a_height, Color a_color) {
			m_boxTexture	= new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			m_position		= new CartesianCoordinate(a_position);
			m_boxColor		= a_color;
			m_width			= a_width;
			m_height		= a_height;
			m_boxTexture.SetData(new[] { a_color });
		}

		public Box(Vector2 a_position, int a_width, int a_height, Color a_color, Color a_lineColor, int a_lineWidth) {
			m_boxTexture	= new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			m_position		= new CartesianCoordinate(a_position);
			m_boxColor		= a_color;
			m_width			= a_width;
			m_height		= a_height;
			m_boxTexture.SetData(new[] { a_color });
			
			Vector2 topLeft = a_position;
			Vector2 topRight = a_position;
			topRight.X += a_width;
			Vector2 btmRight = topRight;
			btmRight.Y += a_height;
			Vector2 btmLeft = btmRight;
			btmLeft.X = topLeft.X;

			m_lineList = new LinkedList<Line>();
			m_lineList.AddLast(new Line(new CartesianCoordinate(topLeft), new CartesianCoordinate(topRight), Vector2.Zero, Vector2.Zero, a_lineColor, a_lineWidth));
			m_lineList.AddLast(new Line(new CartesianCoordinate(topRight), new CartesianCoordinate(btmRight), Vector2.Zero, Vector2.Zero, a_lineColor, a_lineWidth));
			m_lineList.AddLast(new Line(new CartesianCoordinate(btmRight), new CartesianCoordinate(btmLeft), Vector2.Zero, Vector2.Zero, a_lineColor, a_lineWidth));
			m_lineList.AddLast(new Line(new CartesianCoordinate(btmLeft), new CartesianCoordinate(topLeft), Vector2.Zero, Vector2.Zero, a_lineColor, a_lineWidth));
		}

		public void draw() {
			Game.getInstance().getSpriteBatch().Draw(m_boxTexture, m_position.getGlobalCartesianCoordinates(), null, m_boxColor, 0.0f, Vector2.Zero, new Vector2(m_width, m_height), SpriteEffects.None, 0.011f);
			if (m_lineList != null && m_lineList.Count > 0) {
				foreach (Line t_line in m_lineList) {
					t_line.draw();
				}
			}
		}
	}
}