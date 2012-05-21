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
		#region Members
		public delegate void clickDelegate(Button a_button);
		public event clickDelegate m_clickEvent;

		private float m_layer;
		private string m_buttonTexture;

		protected Text m_text;
		private Vector2 m_textOffset = Vector2.Zero;
		private Color m_textColor = Color.Black;

		private Position m_position;
		private Vector2 m_size;
		private Vector2 m_posV2;

		private Texture2D m_normalTexture;
		private Texture2D m_hoverTexture;
		private Texture2D m_pressedTexture;
		private Texture2D m_toggleTexture;
		private Sound m_upSound;
		private Sound m_downSound;

		protected Rectangle m_bounds;

		protected bool m_isFocused;
		protected bool m_isPressed;
		protected bool m_isToggled;
		protected bool m_isVisible;

		private Keys[] m_hotkey;

		private State m_currentState = State.Normal;
		public enum State
		{
			Normal,
			Hover,
			Pressed,
			Toggled
		}
		#endregion

		#region Constructor & Load
		public Button(Vector2 a_position)
		{
			m_position = new CartesianCoordinate(a_position, Game.getInstance().m_camera.getPosition());
			setPosition(a_position);
			m_layer = 0.002f;
		}

		public Button(string a_buttonTexture, Vector2 a_position, string a_buttonText, string a_font, Color a_color, Vector2 a_offset)
		{
			if (a_font == null)
			{
				a_font = "Courier New";
			}
			m_posV2 = a_position;
			m_text = new Text(a_position, a_offset, a_buttonText, a_font, a_color, false);
			m_position = new CartesianCoordinate(a_position, Game.getInstance().m_camera.getPosition());
			setPosition(a_position);
			m_bounds = new Rectangle((int)a_position.X, (int)a_position.Y, 0, 0);
			m_layer = 0.002f;
			m_upSound = null;
			m_downSound = null;
			m_buttonTexture = a_buttonTexture;
			m_isVisible = true;
			loadContent();
		}

		public Button(string a_buttonTexture, Vector2 a_position)
		{
			m_position = new CartesianCoordinate(a_position, Game.getInstance().m_camera.getPosition());
			setPosition(a_position);
			m_bounds = new Rectangle((int)a_position.X, (int)a_position.Y, 0, 0);
			m_layer = 0.002f;
			m_upSound = null;
			m_downSound = null;
			m_buttonTexture = a_buttonTexture;
			m_isVisible = true;
			loadContent();
		}

		public Button(Vector2 a_position, string a_normal, string a_hover, string a_pressed, string a_toggle)
		{
			setNormalTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + a_normal));
			setHoverTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + a_hover));
			setPressedTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + a_pressed));
			setToggleTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + a_toggle));
			m_position = new CartesianCoordinate(a_position);
			m_position.setParentPosition(Game.getInstance().m_camera.getPosition());
			setPosition(a_position);
			m_bounds = new Rectangle((int)a_position.X, (int)a_position.Y, (int)m_size.X, (int)m_size.Y);
			m_layer = 0.002f;
			m_upSound = null;
			m_downSound = null;
			m_isVisible = true;
		}

		public void loadContent()
		{
			if (m_buttonTexture != null)
			{
				try
				{
					setNormalTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + m_buttonTexture + "_normal"));
					setHoverTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + m_buttonTexture + "_hover"));
					setPressedTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + m_buttonTexture + "_pressed"));
					setToggleTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//" + m_buttonTexture + "_toggle"));
				}
				catch (ContentLoadException cle)
				{
					ErrorLogger.getInstance().writeString("Could not find asset for: " + m_buttonTexture + "\n" + cle.ToString());
				}
			}
			else
			{
				setNormalTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//DevelopmentHotkeys//btn_select_hotkey_normal"));
				setHoverTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//DevelopmentHotkeys//btn_select_hotkey_hover"));
				setPressedTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//DevelopmentHotkeys//btn_select_hotkey_pressed"));
				setToggleTexture(Game.getInstance().Content.Load<Texture2D>("Images//GUI//DevelopmentHotkeys//btn_select_hotkey_toggle"));
			}
			m_bounds.Width = m_normalTexture.Width;
			m_bounds.Height = m_normalTexture.Height;
		}

		public void kill()
		{
			m_text.kill();
		}
		#endregion
		
		#region Update & Draw
		public virtual bool update()
		{
			if (!m_isVisible) {
				return false;
			}
			if (m_bounds.Contains(Mouse.GetState().X, Mouse.GetState().Y))
			{
				m_isFocused = true;
				if (MouseHandler.lmbDown() && m_currentState != State.Pressed)
				{
					playDownSound();
					m_isPressed = true;
				}
				if (m_isPressed && MouseHandler.lmbUp())
				{
					playUpSound();
					m_isPressed = false;
					if (m_clickEvent != null)
					{
						m_clickEvent(this);
					}
				}	
			}
			else if (hotkeyPressed())
			{
				if (m_downSound != null)
				{
					m_downSound.play();
				}
				m_isPressed = true;
				if (m_clickEvent != null)
				{
					m_clickEvent(this);
				}
			}
			else
			{
				m_isPressed = false;
				m_isFocused = false;
			}
			return m_isPressed;
		}

		public virtual void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			if (!m_isVisible)
			{
				return;
			}
			CartesianCoordinate t_cartCoord = new CartesianCoordinate(m_position.getLocalCartesian() / Game.getInstance().m_camera.getZoom(), m_position.getParentPosition());
			if (m_isPressed || m_currentState == State.Pressed)
			{
				a_spriteBatch.Draw(m_pressedTexture, t_cartCoord.getGlobalCartesian(), null, Color.White, 0.0f, 
					Vector2.Zero, new Vector2(1.0f / Game.getInstance().m_camera.getZoom(), 1.0f / Game.getInstance().m_camera.getZoom()), SpriteEffects.None, m_layer);
			}
			else if (m_isFocused || m_currentState == State.Hover)
			{
				a_spriteBatch.Draw(m_hoverTexture, t_cartCoord.getGlobalCartesian(), null, Color.White, 0.0f, 
					Vector2.Zero, new Vector2(1.0f / Game.getInstance().m_camera.getZoom(), 1.0f / Game.getInstance().m_camera.getZoom()), SpriteEffects.None, m_layer);
			}
			else if (m_isToggled || m_currentState == State.Toggled)
			{
				a_spriteBatch.Draw(m_toggleTexture, t_cartCoord.getGlobalCartesian(), null, Color.White, 0.0f, 
					Vector2.Zero, new Vector2(1.0f / Game.getInstance().m_camera.getZoom(), 1.0f / Game.getInstance().m_camera.getZoom()), SpriteEffects.None, m_layer);
			}
			else
			{
				a_spriteBatch.Draw(m_normalTexture, t_cartCoord.getGlobalCartesian(), null, Color.White, 0.0f, 
					Vector2.Zero, new Vector2(1.0f / Game.getInstance().m_camera.getZoom(), 1.0f / Game.getInstance().m_camera.getZoom()), SpriteEffects.None, m_layer);
			}
			if (m_text != null)
			{
				m_text.draw(a_gameTime);
			}
		}
		#endregion

		#region Button-methods
		public void playDownSound()
		{
			if (m_downSound != null)
			{
				m_downSound.play();
			}
		}

		public void playUpSound()
		{
			if (m_upSound != null)
			{
				m_upSound.play();
			}
		}

		public virtual void setState(int a_state)
		{
			switch (a_state)
			{
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

		public virtual void setState(State a_state)
		{
			m_currentState = a_state;
		}

		public bool isButtonPressed()
		{
			return m_isPressed;
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

		private void setToggleTexture(Texture2D a_texture)
		{
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

		public void setUpSound(string a_name)
		{
			m_upSound = new Sound(a_name);
		}

		public void setDownSound(string a_name)
		{
			m_downSound = new Sound(a_name);
		}

		public bool isVisible()
		{
			return m_isVisible;
		}

		public void setVisible(bool a_visible)
		{
			m_isVisible = a_visible;
		}

		public int getIntState()
		{
			switch (m_currentState)
			{
				case State.Normal:
					return 0;
				case State.Hover:
					return 1;
				case State.Pressed:
					return 2;
				case State.Toggled:
					return 3;
			}
			return -1;
		}

		public State getState()
		{
			return m_currentState;
		}

		public void setHotkey(Keys[] a_key, clickDelegate a_method)
		{
			m_clickEvent = new Button.clickDelegate(a_method);
			m_hotkey = a_key;
		}

		private bool hotkeyPressed()
		{
			if (m_hotkey == null)
			{
				return false;
			}
			Keys[] t_keys = KeyboardHandler.getPressedKeys();
			foreach (Keys t_key in m_hotkey)
			{
				if (t_keys.Contains(t_key))
				{
					continue;
				}
				return false;
			}

			bool t_returnValue = true;
			if (!m_hotkey.Contains(Keys.LeftShift) && !m_hotkey.Contains(Keys.RightShift))
			{
				t_returnValue = !KeyboardHandler.isKeyPressed(Keys.LeftShift) && !KeyboardHandler.isKeyPressed(Keys.LeftShift);
			}
			if (!m_hotkey.Contains(Keys.LeftControl) && !m_hotkey.Contains(Keys.RightControl) && t_returnValue)
			{
				t_returnValue = !KeyboardHandler.isKeyPressed(Keys.LeftControl) && !KeyboardHandler.isKeyPressed(Keys.RightControl);
			}
			if (!m_hotkey.Contains(Keys.LeftAlt) && !m_hotkey.Contains(Keys.RightAlt) && t_returnValue)
			{
				t_returnValue = !KeyboardHandler.isKeyPressed(Keys.LeftAlt) && !KeyboardHandler.isKeyPressed(Keys.RightAlt);
			}
			return t_returnValue;
		}

		public void updateHitbox()
		{
			m_bounds = new Rectangle((int)m_posV2.X, (int)m_posV2.Y, (int)m_size.X, (int)m_size.Y);
		}

		public void invokeClickEvent()
		{
			m_clickEvent(this);
		}


		public bool hasEvent()
		{
			return m_clickEvent != null;
		}

		public void setColor(Color a_color)
		{
			if (m_text != null)
			{
				m_text.setColor(a_color);
			}
		}
		#endregion

		#region Position Methods
		public Position getPosition()
		{
			return m_position;
		}

		public void setPosition(Vector2 a_position)
		{
			a_position -= Game.getInstance().getResolution() / 2;
			m_position = new CartesianCoordinate(a_position, Game.getInstance().m_camera.getPosition());
			m_bounds.X = (int)(a_position.X + Game.getInstance().getResolution().X / 2);
			m_bounds.Y = (int)(a_position.Y + Game.getInstance().getResolution().Y / 2);
			if (m_text != null)
			{
				m_text.setPosition(a_position + m_textOffset);
				if (this is TextButton)
				{
					m_bounds = m_text.getBox();
					m_bounds.X += (int)Game.getInstance().getResolution().X / 2;
					m_bounds.Y += (int)Game.getInstance().getResolution().Y / 2;
				}
			}
		}

		public void move(Vector2 a_moveLength)
		{
			m_position.plusWith(a_moveLength);
			m_bounds.X = (int)(m_position.getLocalX() + Game.getInstance().getResolution().X / 2);
			m_bounds.Y = (int)(m_position.getLocalY() + Game.getInstance().getResolution().Y / 2);
			if (m_text != null)
			{
				m_text.move(a_moveLength);
				if (this is TextButton)
				{
					m_bounds = m_text.getBox();
					m_bounds.X += (int)Game.getInstance().getResolution().X / 2;
					m_bounds.Y += (int)Game.getInstance().getResolution().Y / 2;
				}
			}
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
		#endregion
	}
}
