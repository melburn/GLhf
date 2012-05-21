﻿using System;
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

		public override void addObject(GameObject a_object)
		{
			//throw new NotImplementedException();
		}
	}
}
