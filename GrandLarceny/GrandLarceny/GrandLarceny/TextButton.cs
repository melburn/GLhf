using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	public class TextButton : Button
	{
		public new delegate void clickDelegate(Button a_button);
		public new event clickDelegate m_clickEvent;
		private Color m_normalColor;
		private Color m_hoverColor;
		private Color m_pressedColor;
		private Color m_toggleColor;

		public TextButton(Vector2 a_position, string a_text, string a_font, Color a_normal, Color a_hover, Color a_pressed, Color a_toggle) : base(a_position)
		{
			m_text = new Text(a_position, a_text, a_font, a_normal, false);
			m_normalColor = a_normal;
			m_hoverColor = a_hover;
			m_pressedColor = a_pressed;
			m_toggleColor = a_toggle;
			m_isVisible = true;
			m_bounds = m_text.getBox();
			setPosition(a_position);
		}

		public override void setState(Button.State a_state)
		{
			base.setState(a_state);
			switch (a_state)
			{
				case Button.State.Normal:
					m_text.setColor(m_normalColor);
					break;
				case Button.State.Hover:
					m_text.setColor(m_hoverColor);
					break;
				case Button.State.Pressed:
					m_text.setColor(m_pressedColor);
					break;
				case Button.State.Toggled:
					m_text.setColor(m_toggleColor);
					break;
			}
		}

		public override bool update()
		{
			if (!m_isVisible)
			{
				return false;
			}
			if (m_bounds.Contains((int)MouseHandler.getMouseCoords().X, (int)MouseHandler.getMouseCoords().Y) && base.getState() != Button.State.Pressed)
			{	
				m_isFocused = true;
				if (MouseHandler.lmbPressed())
				{
					m_isPressed = true;
					if (MouseHandler.lmbDown())
					{
						playDownSound();
					}
					m_text.setColor(m_pressedColor);
				}
				else
				{
					if (m_isPressed && MouseHandler.lmbUp())
					{
						playUpSound();
						if (m_clickEvent != null)
						{
							m_clickEvent(this);
						}
					}
					m_text.setColor(m_hoverColor);
				}
			}
			else
			{
				if (base.getState() == Button.State.Normal)
				{
					m_text.setColor(m_normalColor);
				}
			}
			return m_isPressed;
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			m_text.draw(a_gameTime);
		}
	}
}