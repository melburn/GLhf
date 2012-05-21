using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	public class Text : GuiObject
	{
		#region Members
		private SpriteFont m_spriteFont;
		private string m_font;
		private string m_text;
		private bool m_worldFont;
		private float m_rotation = 0.0f;
		private Vector2 m_textOffset;
		#endregion

		#region Constructor & Load
		public Text(Vector2 a_position, string a_text, string a_spriteFont, Color a_color, bool a_worldFont)
			: base(a_position, null)
		{
			if (a_worldFont)
			{
				m_position = new CartesianCoordinate(a_position - Game.getInstance().getResolution() / 2);
			}
			else
			{
				m_position = new CartesianCoordinate(a_position - Game.getInstance().getResolution() / 2, Game.getInstance().m_camera.getPosition());
			}
			m_text = a_text;
			m_textOffset = Vector2.Zero;
			m_font = a_spriteFont;
			m_color = a_color;
			m_worldFont = a_worldFont;
			m_layer = 0.001f;
			loadContent();
		}

		public Text(Vector2 a_position, Vector2 a_offset, string a_text, string a_spriteFont, Color a_color, bool a_worldFont)
			: base(a_position, "")
		{
			if (a_worldFont)
			{
				m_position = new CartesianCoordinate(a_position - Game.getInstance().getResolution() / 2);
			}
			else
			{
				m_position = new CartesianCoordinate(a_position - Game.getInstance().getResolution() / 2, Game.getInstance().m_camera.getPosition());
			}
			m_position.plusWith(a_offset);
			m_text = a_text;
			m_textOffset = a_offset;
			m_font = a_spriteFont;
			m_color = a_color;
			m_worldFont = a_worldFont;
			m_layer = 0.001f;
			loadContent();
		}

		public Text(Vector2 a_position, string a_text, string a_spriteFont, Color a_color, bool a_worldFont, float a_layer)
			: base(a_position, "")
		{
			if (a_worldFont)
			{
				m_position = new CartesianCoordinate(a_position - Game.getInstance().getResolution() / 2);
			}
			else
			{
				m_position = new CartesianCoordinate(a_position - Game.getInstance().getResolution() / 2, Game.getInstance().m_camera.getPosition());
			}
			m_text = a_text;
			m_textOffset = Vector2.Zero;
			m_font = a_spriteFont;
			m_color = a_color;
			m_worldFont = a_worldFont;
			m_layer = a_layer;
			loadContent();
		}

		public override void loadContent()
		{
			if (m_font == null || m_font.Equals(""))
			{
				m_font = "VerdanaBold";
			}
			m_spriteFont = Game.getInstance().Content.Load<SpriteFont>("Fonts\\" + m_font);
		}
		#endregion

		#region Text-related Methods
		public void erase()
		{
			if (m_text.Length > 0)
			{
				m_text = m_text.Remove(m_text.Length - 1);
			}
		}

		public void addText(char a_char)
		{
			m_text += a_char;
		}

		public void addText(string a_string)
		{
			m_text += a_string;
		}

		public void setText(string a_text)
		{
			m_text = a_text;
		}

		public string getText()
		{
			return m_text;
		}

		public SpriteFont getFont()
		{
			return m_spriteFont;
		}

		public Vector2 measureString()
		{
			if (m_font.Equals("MotorwerkLarge"))
			{
				//TODO Fulhax
				Vector2 t_return = m_spriteFont.MeasureString(m_text);
				t_return.Y -= 37;
				return t_return;
			}
			else
			{
				return m_spriteFont.MeasureString(m_text);
			}
		}
		#endregion

		#region Position-methods
		public void move(Vector2 a_moveLength)
		{
			m_position.plusWith(a_moveLength);
			if (m_font.Equals("MotorwerkLarge"))
			{
				m_position.plusYWith(-5);
			}
		}

		public void setOffset(Vector2 a_offset)
		{
			m_position.plusWith(a_offset);
		}

		public void setPosition(Vector2 a_position)
		{
			m_position = new CartesianCoordinate(a_position + m_textOffset, Game.getInstance().m_camera.getPosition());
			if (m_font.Equals("MotorwerkLarge"))
			{
				m_position.plusYWith(-5);
			}
		}

		public override Rectangle getBox()
		{
			if (m_font.Equals("MotorwerkLarge"))
			{
				Rectangle t_return = new Rectangle((int)m_position.getGlobalX(), (int)m_position.getGlobalY(), (int)measureString().X, (int)measureString().Y);
				t_return.Y += 30;
				t_return.Height -= 10;
				return t_return;
			}
			else
			{
				return new Rectangle((int)m_position.getGlobalX(), (int)m_position.getGlobalY(), (int)measureString().X, (int)measureString().Y);
			}
		}
		#endregion

		public override void draw(GameTime a_gameTime)
		{
			if (m_text == null || !m_visible)
			{
				return;
			}
				
			SpriteBatch t_spriteBatch = Game.getInstance().getSpriteBatch();

			if (m_worldFont)
			{
				t_spriteBatch.DrawString(m_spriteFont, m_text, m_position.getGlobalCartesian(), m_color);
			}
			else
			{	
				float t_zoom = Game.getInstance().m_camera.getZoom();
				Vector2 t_cartCoord;
				t_cartCoord.X = m_position.getLocalX() / t_zoom + m_position.getParentPosition().getGlobalX();
				t_cartCoord.Y = m_position.getLocalY() / t_zoom + m_position.getParentPosition().getGlobalY();

				try
				{
					t_spriteBatch.DrawString(m_spriteFont, m_text, t_cartCoord, m_color, m_rotation, Vector2.Zero, 1.0f / t_zoom, SpriteEffects.None, m_layer);
				}
				catch (System.ArgumentException)
				{
					erase();
				}
			}
		}
	}
}
