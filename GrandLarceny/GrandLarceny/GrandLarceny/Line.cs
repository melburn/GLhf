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
		#region Members
		private Texture2D m_lineTexture;
		private Position m_startPosition;
		private Position m_endPosition;
		private Color m_lineColor;
		private Vector2 m_startOffset;
		private Vector2 m_endOffset;
		private int m_width;
		private bool m_worldLine;
		private float m_layer;
		private bool m_hasTexture;
		#endregion
		
		#region Constructor & Load
		public Line(Position a_startPosition, Position a_endPosition, Vector2 a_startOffset, Vector2 a_endOffset, Color a_color, int a_width, bool a_worldLine)
		{
			m_startOffset	= a_startOffset;
			m_endOffset		= a_endOffset;
			m_lineColor		= a_color;
			m_width			= a_width;
			m_worldLine		= a_worldLine;
			m_layer			= 0.010f;
			m_lineTexture = new Texture2D(Game.getInstance().GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			m_lineTexture.SetData(new[] { a_color });

			if (a_worldLine)
			{
				m_startPosition = new CartesianCoordinate(a_startOffset, a_startPosition);
				m_endPosition = new CartesianCoordinate(a_endOffset, a_endPosition);
			}
			else
			{
				m_startPosition = new CartesianCoordinate(a_startOffset, Game.getInstance().m_camera.getPosition());
				m_endPosition = new CartesianCoordinate(a_endOffset, Game.getInstance().m_camera.getPosition());
			}
		}
		#endregion

		#region Start- & Endpoint Methods

		public void setTexture(string a_path)
		{
			float t_length = Vector2.Distance(m_startPosition.getGlobalCartesian(), m_endPosition.getGlobalCartesian());

			Texture2D t_texture = Game.getInstance().Content.Load<Texture2D>(a_path);
			Color[] t_colorArray = new Color[t_texture.Width * t_texture.Height];
			t_texture.GetData(t_colorArray);

			m_lineTexture = new Texture2D(Game.getInstance().GraphicsDevice, t_texture.Width, (int)t_length, false, SurfaceFormat.Color);
			Color[] t_lineTexture = new Color[m_lineTexture.Width * (int)t_length];
			m_lineTexture.GetData(t_lineTexture);

			for (int i = 0, j = 0; i < t_lineTexture.Length; i++, j++)
			{
				if (j >= t_colorArray.Length)
				{
					j = 0;
				}
				t_lineTexture[i] = t_colorArray[j];
			}

			m_lineTexture.SetData(t_lineTexture);
			m_hasTexture = true;
		}

		public void updateTexture()
		{
			float t_length = Vector2.Distance(m_startPosition.getGlobalCartesian(), m_endPosition.getGlobalCartesian());			
			Color[] t_colorArray = new Color[m_lineTexture.Width * m_lineTexture.Height];
			m_lineTexture.GetData(t_colorArray);

			m_lineTexture = new Texture2D(Game.getInstance().GraphicsDevice, m_lineTexture.Width, (int)t_length, false, SurfaceFormat.Color);
			Color[] t_lineTexture = new Color[m_lineTexture.Width * (int)t_length];
			m_lineTexture.GetData(t_lineTexture);

			for (int i = 0, j = 0; i < t_lineTexture.Length; i++, j++)
			{
				if (j >= t_colorArray.Length)
				{
					j = 0;
				}
				t_lineTexture[i] = t_colorArray[j];
			}

			m_lineTexture.SetData(t_lineTexture);
			m_hasTexture = true;
		}

		public Position getEndPoint()
		{
			return m_endPosition;
		}

		public Position getStartPoint()
		{
			return m_startPosition;
		}

		public void setEndPoint(Vector2 a_endPoint)
		{
			m_endPosition = new CartesianCoordinate(a_endPoint + m_endOffset);
		}

		public void setEndPoint(Vector2 a_endPoint, Vector2 a_endOffset)
		{
			m_endPosition = new CartesianCoordinate(a_endPoint + a_endOffset);
		}

		public void setEndPoint(Position a_position)
		{
			m_endPosition = a_position;
		}

		public void setEndPoint(Position a_position, Vector2 a_offset)
		{
			m_endPosition.setParentPosition(a_position);
			m_endOffset = a_offset;
		}

		public void setStartPoint(Vector2 a_startPoint)
		{
			m_startPosition = new CartesianCoordinate(a_startPoint + m_startOffset);
		}

		public void setOffset(Vector2 a_offset)
		{
			m_startPosition.setLocalCartesian(a_offset);
			m_endPosition.setLocalCartesian(a_offset);
		}

		public void setXOffset(float a_offset)
		{
			m_startPosition.setLocalX(a_offset);
			m_endPosition.setLocalX(a_offset);
		}

		public void setYOffset(float a_offset)
		{
			m_startPosition.setLocalY(a_offset);
			m_endPosition.setLocalY(a_offset);
		}
		#endregion

		#region Draw
		public void draw()
		{
			float t_angle = (float)Math.Atan2(m_endPosition.getGlobalY() - m_startPosition.getGlobalY(), m_endPosition.getGlobalX() - m_startPosition.getGlobalX());
			float t_length = Vector2.Distance(m_startPosition.getGlobalCartesian(), m_endPosition.getGlobalCartesian());
			Rectangle drawRect = new Rectangle((int)(m_startPosition.getGlobalX() + m_startOffset.X), (int)(m_startPosition.getGlobalY() + m_startOffset.Y), m_width, (int)t_length);

			if (m_worldLine)
			{
				if (m_hasTexture)
					Game.getInstance().getSpriteBatch().Draw(m_lineTexture, m_startPosition.getGlobalCartesian(), null, m_lineColor, t_angle - (float)(Math.PI / 2), Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, m_layer);
				else
					Game.getInstance().getSpriteBatch().Draw(m_lineTexture, m_startPosition.getGlobalCartesian(), null, m_lineColor, t_angle, Vector2.Zero, new Vector2(t_length, m_width), SpriteEffects.None, m_layer);

				//Game.getInstance().getSpriteBatch().Draw(m_lineTexture, m_startPosition.getGlobalCartesian(), null, m_lineColor, t_angle - (float)(Math.PI / 2), Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, m_layer);
			}
			else
			{
				float t_zoom = Game.getInstance().m_camera.getZoom();				
				Vector2 t_cartCoord = new Vector2(
					t_cartCoord.X = m_startPosition.getLocalX() / t_zoom + Game.getInstance().m_camera.getPosition().getGlobalX(),
					t_cartCoord.Y = m_startPosition.getLocalY() / t_zoom + Game.getInstance().m_camera.getPosition().getGlobalY()
				);

				Game.getInstance().getSpriteBatch().Draw(m_lineTexture, t_cartCoord, null, m_lineColor, t_angle, Vector2.Zero, new Vector2(t_length / t_zoom, m_width / t_zoom), SpriteEffects.None, m_layer);
			}
		}
		#endregion

		public void setColor(Color a_color)
		{
			m_lineTexture.SetData(new[] { a_color });
		}

		public void setLayer(float a_layer)
		{
			m_layer = a_layer;
		}
	}
}
