using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	public class Text
	{
		private SpriteFont m_spriteFont;
		private Position m_position;
		private string m_text;
		private bool m_worldFont;
		private Color m_color;
		private Vector2 m_resolution;
		private float m_layer;
		private float m_rotation = 0.0f;

		public Text(Vector2 a_position, string a_text, SpriteFont a_spriteFont, Color a_color, bool a_worldFont)
		{
			m_position = new CartesianCoordinate(a_position);
			if (!a_worldFont)
			{
				m_resolution = new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth, Game.getInstance().m_graphics.PreferredBackBufferHeight);
				m_position.setParentPosition(Game.getInstance().m_camera.getPosition());
				m_position.setX(m_position.getX() - m_resolution.X / 2);
				m_position.setY(m_position.getY() - m_resolution.Y / 2);
			}
			m_text = a_text;
			m_spriteFont = a_spriteFont;
			m_color = a_color;
			m_worldFont = a_worldFont;
			m_layer = 0.001f;
		}

		public Text(Vector2 a_position, string a_text, SpriteFont a_spriteFont, Color a_color, bool a_worldFont, float a_layer)
		{
			m_position = new CartesianCoordinate(a_position);
			if (!a_worldFont)
			{
				m_resolution = new Vector2(Game.getInstance().m_graphics.PreferredBackBufferWidth, Game.getInstance().m_graphics.PreferredBackBufferHeight);
				m_position.setParentPosition(Game.getInstance().m_camera.getPosition());
				m_position.setX(m_position.getX() - m_resolution.X / 2);
				m_position.setY(m_position.getY() - m_resolution.Y / 2);
			}
			m_text = a_text;
			m_spriteFont = a_spriteFont;
			m_color = a_color;
			m_worldFont = a_worldFont;
			m_layer = a_layer;
		}

		public void setText(string a_text)
		{
			m_text = a_text;
		}

		public void setLayer(float a_layer)
		{
			m_layer = a_layer;
		}

		public void draw(SpriteBatch a_spriteBatch)
		{
			if (m_worldFont)
			{
				a_spriteBatch.DrawString(m_spriteFont, m_text, m_position.getGlobalCartesianCoordinates(), m_color);
			}
			else
			{	
				if (m_worldFont)
				{
					a_spriteBatch.DrawString(m_spriteFont, m_text, m_position.getGlobalCartesianCoordinates(), m_color);
				}
				else
				{
					float t_zoom = Game.getInstance().m_camera.getZoom();
					CartesianCoordinate t_cartCoord = new CartesianCoordinate(m_position.getLocalCartesianCoordinates() / t_zoom, m_position.getParentPosition());
					a_spriteBatch.DrawString(m_spriteFont, m_text, t_cartCoord.getGlobalCartesianCoordinates(), m_color, m_rotation, Vector2.Zero, 1.0f / t_zoom, SpriteEffects.None, m_layer);
				}
			}
		}
	}
}
