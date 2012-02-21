using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public class Button
	{
		public delegate void clickDelegate(Button a_button);
		public event clickDelegate m_clickEvent;

		private float m_layer;

		private string m_buttonText = null;
		private Text m_text;
		private Vector2 m_textOffset = Vector2.Zero;
		private Color m_textColor = Color.Black;

		private Position m_position;
		private Vector2 m_size;

		private Texture2D m_normalTexture;
		private Texture2D m_hoverTexture;
		private Texture2D m_pressedTexture;

		private Rectangle m_bounds;

		private bool m_isFocused;
		private bool m_isPressed;

		private MouseState m_currMouseState;
		private MouseState m_prevMouseState;

		private State m_currentState = State.Normal;

		enum State
		{
			Normal,
			Hover,
			Pressed
		}

		public Button(Texture2D a_normal, Texture2D a_hover, Texture2D a_pressed, Vector2 a_position, string a_buttonText, float a_layer)
		{
			setHoverTexture(a_hover);
			setNormalTexture(a_normal);
			setPressedTexture(a_pressed);
			m_text = new Text(a_position, a_buttonText, Game.getInstance().Content.Load<SpriteFont>("Fonts//Courier New10"), Color.Black, false);
			m_position = new CartesianCoordinate(a_position);
			m_position.setParentPosition(Game.getInstance().m_camera.getPosition());
			setPosition(a_position);
			m_bounds = new Rectangle((int)a_position.X, (int)a_position.Y, (int)m_size.X, (int)m_size.Y);
			m_layer = a_layer;
		}

		public Button(string a_normal, string a_hover, string a_pressed, Vector2 a_position, string a_buttonText, float a_layer)
		{
			setNormalTexture(Game.getInstance().Content.Load<Texture2D>(a_normal));
			setHoverTexture(Game.getInstance().Content.Load<Texture2D>(a_hover));
			setPressedTexture(Game.getInstance().Content.Load<Texture2D>(a_pressed));
			m_text = new Text(a_position, m_buttonText, Game.getInstance().Content.Load<SpriteFont>("Fonts//Courier New10"), Color.Black, false);
			m_position = new CartesianCoordinate(a_position);
			m_position.setParentPosition(Game.getInstance().m_camera.getPosition());
			setPosition(a_position);
			m_bounds = new Rectangle((int)a_position.X, (int)a_position.Y, (int)m_size.X, (int)m_size.Y);
			m_layer = a_layer;
		}

		public void update()
		{
			m_prevMouseState = m_currMouseState;
			m_currMouseState = Mouse.GetState();

			if (m_bounds.Contains(Mouse.GetState().X, Mouse.GetState().Y))
			{
				m_isFocused = true;
				if (m_currMouseState != m_prevMouseState && m_currMouseState.LeftButton == ButtonState.Pressed)
				{
					m_isPressed = true;
				}
				if (m_isPressed && (m_prevMouseState.LeftButton == ButtonState.Pressed && m_currMouseState.LeftButton == ButtonState.Released))
				{
					m_isPressed = false;
					if (m_clickEvent != null)
						m_clickEvent(this);
				}
			}
			else
			{
				m_isPressed = false;
				m_isFocused = false;
			}
		}
		public void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			CartesianCoordinate t_cartCoord = new CartesianCoordinate(m_position.getLocalCartesianCoordinates() / Game.getInstance().m_camera.getZoom(), m_position.getParentPosition());
			if (m_isPressed || m_currentState == State.Pressed)
				a_spriteBatch.Draw(m_pressedTexture, t_cartCoord.getGlobalCartesianCoordinates(), null, Color.White, 0.0f, Vector2.Zero, new Vector2(1.0f / Game.getInstance().m_camera.getZoom(), 1.0f / Game.getInstance().m_camera.getZoom()), SpriteEffects.None, m_layer);
			else if (m_isFocused || m_currentState == State.Hover)
				a_spriteBatch.Draw(m_hoverTexture, t_cartCoord.getGlobalCartesianCoordinates(), null, Color.White, 0.0f, Vector2.Zero, new Vector2(1.0f / Game.getInstance().m_camera.getZoom(), 1.0f / Game.getInstance().m_camera.getZoom()), SpriteEffects.None, m_layer);
			else
				a_spriteBatch.Draw(m_normalTexture, t_cartCoord.getGlobalCartesianCoordinates(), null, Color.White, 0.0f, Vector2.Zero, new Vector2(1.0f / Game.getInstance().m_camera.getZoom(), 1.0f / Game.getInstance().m_camera.getZoom()), SpriteEffects.None, m_layer);
			if (m_text != null)
				m_text.draw(a_spriteBatch);
		}

		public void setState(int a_state)
		{
			switch (a_state) {
				case 0:
					m_currentState = State.Normal;
					break;
				case 1:
					m_currentState = State.Hover;
					break;
				case 2:
					m_currentState = State.Pressed;
					break;
				default:
					m_currentState = State.Normal;
					break;
			}
		}

		public bool isButtonPressed()
		{
			return m_isPressed;
		}

		public Position getPosition()
		{
			return m_position;
		}
		public void setPosition(Vector2 a_position)
		{
			a_position.X -= Game.getInstance().m_graphics.PreferredBackBufferWidth / 2;
			a_position.Y -= Game.getInstance().m_graphics.PreferredBackBufferHeight / 2;
			m_position.setCartesianCoordinates(a_position);
		}
		public Vector2 getSize()
		{
			return m_size;
		}
		private void setSize(Vector2 a_size)
		{
			m_size = a_size;
			m_bounds.Width = (int)a_size.X;
			m_bounds.Height = (int)a_size.Y;
		}

		public Rectangle getBox()
		{
			return new Rectangle((int)m_position.getGlobalX(), (int)m_position.getGlobalY(), (int)m_size.X, (int)m_size.Y);
		}
		private void setNormalTexture(Texture2D a_texture)
		{
			m_normalTexture = a_texture;
			Vector2 newSize = new Vector2(a_texture.Bounds.Width, a_texture.Bounds.Height);
			setSize(newSize);
		}
		private void setHoverTexture(Texture2D a_texture)
		{
			m_hoverTexture = a_texture;
		}
		private void setPressedTexture(Texture2D a_texture)
		{
			m_pressedTexture = a_texture;
		}

		public void setText(String a_string)
		{
			m_text.setText(a_string);
		}
		public void setText(string a_string, Vector2 a_offset)
		{
			m_text.setText(a_string);
			m_text.setOffset(a_offset);
		}
		public string getText()
		{
			return m_text.getText();
		}
	}
}
