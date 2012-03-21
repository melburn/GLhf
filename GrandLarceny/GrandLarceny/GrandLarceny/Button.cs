using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace GrandLarceny
{
	public class Button
	{
		public delegate void clickDelegate(Button a_button);
		public event clickDelegate m_clickEvent;

		private float m_layer;

		private Text m_text;
		private Vector2 m_textOffset = Vector2.Zero;
		private Color m_textColor = Color.Black;

		private Position m_position;
		private Vector2 m_size;

		private Texture2D m_normalTexture;
		private Texture2D m_hoverTexture;
		private Texture2D m_pressedTexture;
		private Texture2D m_toggleTexture;
		private Sound m_upSound;
		private Sound m_downSound;

		private Rectangle m_bounds;

		private bool m_isFocused;
		private bool m_isPressed;
		private bool m_isToggled;

		private MouseState m_currMouseState;
		private MouseState m_prevMouseState;

		private State m_currentState = State.Normal;

		enum State
		{
			Normal,
			Hover,
			Pressed,
			Toggled
		}

		public Button(string a_normal, string a_hover, string a_pressed, string a_toggle, Vector2 a_position, string a_buttonText, string a_font, Color a_color, Vector2 a_offset)
		{
			setNormalTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + a_normal));
			setHoverTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + a_hover));
			setPressedTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + a_pressed));
			setToggleTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + a_toggle));
			if (a_font == null)
				a_font = "Courier New";
			m_text = new Text(a_position, a_offset, a_buttonText, a_font, a_color, false);
			m_position = new CartesianCoordinate(a_position);
			m_position.setParentPosition(Game.getInstance().m_camera.getPosition());
			setPosition(a_position);
			m_bounds = new Rectangle((int)a_position.X, (int)a_position.Y, (int)m_size.X, (int)m_size.Y);
			m_layer = 0.002f;
			m_upSound = null;
			m_downSound = null;
		}

		public Button(string a_buttonTexture, Vector2 a_position, string a_buttonText, string a_font, Color a_color, Vector2 a_offset)
		{
			try {
				setNormalTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + a_buttonTexture + "_normal"));
				setHoverTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + a_buttonTexture + "_hover"));
				setPressedTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + a_buttonTexture + "_pressed"));
				setToggleTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + a_buttonTexture + "_toggle"));
			} catch (ContentLoadException cle) {
				System.Console.WriteLine("Could not find asset for: " + a_buttonTexture + "\n" + cle.ToString());
			}
			if (a_font == null)
				a_font = "Courier New";
			m_text = new Text(a_position, a_offset, a_buttonText, a_font, a_color, false);
			m_position = new CartesianCoordinate(a_position);
			m_position.setParentPosition(Game.getInstance().m_camera.getPosition());
			setPosition(a_position);
			m_bounds = new Rectangle((int)a_position.X, (int)a_position.Y, (int)m_size.X, (int)m_size.Y);
			m_layer = 0.002f;
			m_upSound = null;
			m_downSound = null;
		}
		
		public void playDownSound() {
			if (m_downSound != null) {
				m_downSound.play();
			}
		}

		public void playUpSound() {
			if (m_upSound != null) {
				m_upSound.play();
			}
		}

		public void update()
		{
			m_prevMouseState = m_currMouseState;
			m_currMouseState = Mouse.GetState();

			if (m_bounds.Contains(Mouse.GetState().X, Mouse.GetState().Y))
			{
				m_isFocused = true;
				if (m_currMouseState.LeftButton == ButtonState.Pressed && m_prevMouseState.LeftButton == ButtonState.Released)
				{
					if (m_downSound != null) {
						m_downSound.play();
					}
					m_isPressed = true;
				}
				if (m_isPressed && (m_prevMouseState.LeftButton == ButtonState.Pressed && m_currMouseState.LeftButton == ButtonState.Released))
				{
					if (m_upSound != null) {
						m_upSound.play();
					}
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
			if (m_isPressed || m_currentState == State.Pressed) {
				a_spriteBatch.Draw(m_pressedTexture, t_cartCoord.getGlobalCartesianCoordinates(), null, Color.White, 0.0f, 
					Vector2.Zero, new Vector2(1.0f / Game.getInstance().m_camera.getZoom(), 1.0f / Game.getInstance().m_camera.getZoom()), SpriteEffects.None, m_layer);
			} else if (m_isFocused || m_currentState == State.Hover) {
				a_spriteBatch.Draw(m_hoverTexture, t_cartCoord.getGlobalCartesianCoordinates(), null, Color.White, 0.0f, 
					Vector2.Zero, new Vector2(1.0f / Game.getInstance().m_camera.getZoom(), 1.0f / Game.getInstance().m_camera.getZoom()), SpriteEffects.None, m_layer);
			} else if (m_isToggled || m_currentState == State.Toggled) {
				a_spriteBatch.Draw(m_toggleTexture, t_cartCoord.getGlobalCartesianCoordinates(), null, Color.White, 0.0f, 
					Vector2.Zero, new Vector2(1.0f / Game.getInstance().m_camera.getZoom(), 1.0f / Game.getInstance().m_camera.getZoom()), SpriteEffects.None, m_layer);
			} else {
				a_spriteBatch.Draw(m_normalTexture, t_cartCoord.getGlobalCartesianCoordinates(), null, Color.White, 0.0f, 
					Vector2.Zero, new Vector2(1.0f / Game.getInstance().m_camera.getZoom(), 1.0f / Game.getInstance().m_camera.getZoom()), SpriteEffects.None, m_layer);
			}
			if (m_text != null)
				m_text.draw(a_gameTime);
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
				case 3:
					m_currentState = State.Toggled;
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
			a_position.X -= Game.getInstance().getResolution().X / 2;
			a_position.Y -= Game.getInstance().getResolution().Y / 2;
			m_position.setLocalCartesianCoordinates(a_position);
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
			return m_bounds;
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
		private void setToggleTexture(Texture2D a_texture) {
			m_toggleTexture = a_texture;
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
		public void setUpSound(string a_path) {
			m_upSound = new Sound("SoundEffects//GUI//" + a_path);
		}
		public void setDownSound(string a_path) {
			m_downSound = new Sound("SoundEffects//GUI//" + a_path);
		}
	}
}
