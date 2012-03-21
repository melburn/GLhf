using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GrandLarceny.Events;

namespace GrandLarceny
{
	class CutsceneDevelopment : States
	{
		private DevelopmentState m_backState;

		private LinkedList<Button> m_buttonList;
		private Dictionary<Button, Event> m_events;

		public CutsceneDevelopment(DevelopmentState a_backState, LinkedList<Event> a_events)
		{
			m_backState = a_backState;
			m_buttonList = new LinkedList<Button>();
			Button t_buttonToAdd = new Button("dev_bg_info", "dev_bg_info", "dev_bg_info", "dev_bg_info", new Vector2(100, 100), "I DONT WANNA DO NO MORE EVENTS PLZ", null, Color.Red, new Vector2(10, 10));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(exitState);
			m_buttonList.AddLast(t_buttonToAdd);

			foreach (Event t_e in a_events)
			{

			}
		}

		public override void update(GameTime a_gameTime)
		{
			foreach (Button t_b in m_buttonList)
			{
				t_b.update();
			}
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			m_backState.draw(a_gameTime, a_spriteBatch);
			foreach (Button t_b in m_buttonList)
			{
				t_b.draw(a_gameTime, a_spriteBatch);
			}
		}


		//knappmetoder

		public void exitState(Button a_care)
		{
			Game.getInstance().setState(m_backState);
		}

		public void selectEvent(Button a_button)
		{
		}
	}
}
