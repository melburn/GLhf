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

		public override void update(GameTime a_gameTime)
		{
			for (int i = 0; i < m_buttons.Count(); i++)
			{
				m_buttons.ElementAt(i).update();
			}
			foreach (GuiObject t_guiObject in m_guiList)
			{
				t_guiObject.update(a_gameTime);
			}
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
		}
	}
}
