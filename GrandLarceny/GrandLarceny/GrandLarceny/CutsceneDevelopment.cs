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

		private Button m_selectedEvent;

		public CutsceneDevelopment(DevelopmentState a_backState, LinkedList<Event> a_events)
		{
			if (a_events == null)
			{
				throw new ArgumentNullException();
			}
			m_backState = a_backState;
			m_buttonList = new LinkedList<Button>();
			Button t_buttonToAdd = new Button("dev_bg_info", "dev_bg_info", "dev_bg_info", "dev_bg_info", new Vector2(0, 0), "No more event plz", null, Color.Red, new Vector2(10, 10));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(exitState);
			m_buttonList.AddLast(t_buttonToAdd);
			t_buttonToAdd = new Button("dev_bg_info", "dev_bg_info", "dev_bg_info", "dev_bg_info", new Vector2(800, 500), "Add new event", null, Color.Black, new Vector2(10, 10));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(addEvent);
			m_buttonList.AddLast(t_buttonToAdd);
			m_events = new Dictionary<Button, Event>();

			foreach (Event t_e in a_events)
			{
				addEvent(t_e);
			}
		}

		public void addEvent(Event t_e)
		{
			Button t_button = new Button("dev_bg_info", "dev_bg_info", "dev_bg_info", "dev_bg_info", new Vector2(10, 100 + (m_events.Count * 20)), "" + m_events.Count, null, Color.Yellow, new Vector2(10, 10));
			t_button.m_clickEvent += new Button.clickDelegate(selectEvent);
			m_events.Add(t_button, t_e);
			m_buttonList.AddLast(t_button);
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
			m_selectedEvent = a_button;
		}

		public void addEvent(Button a_care)
		{
			Button t_button = new Button("dev_bg_info", "dev_bg_info", "dev_bg_info", "dev_bg_info", new Vector2(10, 100 + (m_events.Count * 20)), "" + m_events.Count, null, Color.Yellow, new Vector2(10, 10));
			t_button.m_clickEvent += new Button.clickDelegate(selectEvent);
			m_events.Add(t_button, new Event(new EventTrigger[0], new EventEffect[0], false));
			m_buttonList.AddLast(t_button);
		}
	}
}
