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

		private int m_level;

		private Vector2 m_position;
		private Vector2 m_size;

		private Texture2D m_normalTexture;
		private Texture2D m_hoverTexture;
		private Texture2D m_pressedTexture;

		private Rectangle m_bounds;

		private bool m_isFocused;
		private bool m_isPressed;

		private MouseState m_currMouseState;
		private MouseState m_prevMouseState;

		public Button(Texture2D a_normal, Texture2D a_hover, Texture2D a_pressed, Vector2 a_position, int a_level)
		{
			setHoverTexture(a_hover);
			setNormalTexture(a_normal);
			setPressedTexture(a_pressed);
			setPosition(a_position);
			m_level = a_level;
		}

		public void update()
		{
			m_prevMouseState = m_currMouseState;
			m_currMouseState = Mouse.GetState();
			Vector2 t_worldMouse;
			t_worldMouse.X =
				Mouse.GetState().X / Game.getInstance().m_camera.getZoom()
				+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().X
				- ((Game.getInstance().m_graphics.PreferredBackBufferWidth / 2) / Game.getInstance().m_camera.getZoom());
			t_worldMouse.Y =
				Mouse.GetState().Y / Game.getInstance().m_camera.getZoom()
				+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().Y
				- ((Game.getInstance().m_graphics.PreferredBackBufferHeight / 2) / Game.getInstance().m_camera.getZoom() + 72);

			if (m_bounds.Contains((int)t_worldMouse.X, (int)t_worldMouse.Y))
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
			if (m_isPressed)
				a_spriteBatch.Draw(m_pressedTexture, m_position, null, Color.White, 0.0f, Vector2.Zero, new Vector2(1.0f, 1.0f), SpriteEffects.None, 0.0f);
			else if (m_isFocused)
				a_spriteBatch.Draw(m_hoverTexture, m_position, null, Color.White, 0.0f, Vector2.Zero, new Vector2(1.0f, 1.0f), SpriteEffects.None, 0.0f);
			else
				a_spriteBatch.Draw(m_normalTexture, m_position, null, Color.White, 0.0f, Vector2.Zero, new Vector2(1.0f, 1.0f), SpriteEffects.None, 0.0f);
		}

		public bool isButtonPressed()
		{
			return m_isPressed;
		}

		public Vector2 getPosition()
		{
			return m_position;
		}
		public void setPosition(Vector2 a_position)
		{
			m_position = a_position;
			m_bounds.X = (int)a_position.X;
			m_bounds.Y = (int)a_position.Y;
		}
		public int getLevel()
		{
			return m_level;
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
	}
}
