using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	public class MenuState : States
	{
		internal LinkedList<Button> m_buttons = new LinkedList<Button>();
		protected Color m_normal	= new Color(187, 194, 195);
		protected Color m_hover		= new Color(255, 255, 255);
		protected Color m_pressed	= new Color(132, 137, 138);
		protected Color m_toggle	= new Color(0, 0, 255);
		protected static PanningBackground m_panningBackground;
		protected int m_currentButton = 0;

		public MenuState()
		{
			if (m_panningBackground == null)
			{
				m_panningBackground = new PanningBackground();
			}
		}

		public override void load()
		{
			base.load();
			if (m_currentButton >= 0 && m_currentButton < m_buttons.Count)
			{
				m_buttons.ElementAt(m_currentButton).setState(Button.State.Hover);
			}
		}
		public override void update(GameTime a_gameTime)
		{
			for (int i = 0; i < m_buttons.Count(); i++)
			{
				m_buttons.ElementAt(i).update();
				if (i != m_currentButton && m_buttons.ElementAt(i).getState() == Button.State.Hover)
				{
					moveCurrentHoverTo(i);
				}
			}
			foreach (GuiObject t_guiObject in m_guiList)
			{
				t_guiObject.update(a_gameTime);
			}
			m_panningBackground.update(a_gameTime);
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (Button t_button in m_buttons)
			{
				t_button.draw(a_gameTime, a_spriteBatch);
			}
			foreach (GuiObject t_guiObject in m_guiList)
			{
				t_guiObject.draw(a_gameTime);
			}
			m_panningBackground.draw(a_gameTime, a_spriteBatch);
		}
		public void moveCurrentHoverTo(int a_index)
		{
			m_buttons.ElementAt(m_currentButton).setState(Button.State.Normal);
			m_currentButton = a_index;
			if (a_index >= 0)
			{
				m_buttons.ElementAt(m_currentButton).setState(Button.State.Hover);
			}
		}
		public void moveCurrentHover(int a_move)
		{
			if (m_currentButton == -1)
			{
				if (a_move < 0)
				{
					moveCurrentHoverTo(m_buttons.Count - a_move);
				}
				else
				{
					moveCurrentHoverTo(a_move % m_buttons.Count);
				}
			}
			else
			{
				int t_newI = m_currentButton + a_move;
				moveCurrentHoverTo((int)nfmod(t_newI,m_buttons.Count));
			}
			/*m_buttons.ElementAt(m_currentButton).setState(Button.State.Normal);
			m_currentButton += a_move;
			if (m_currentButton >= m_buttons.Count)
			{
				m_currentButton = 0;
			}
			else if (m_currentButton < 0)
			{
				m_currentButton = m_buttons.Count-1;
			}
			m_buttons.ElementAt(m_currentButton).setState(Button.State.Hover);*/
		}

		double nfmod(double a, double b)
		{
			return a - b * Math.Floor(a / b);
		}
	}
}
