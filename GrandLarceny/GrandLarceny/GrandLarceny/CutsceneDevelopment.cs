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
		private Stack<Event> m_eventsToAdd;
		private Stack<Button> m_eventsToRemove;

		private State m_state;
		private int m_numOfAddedEvents;

		private Button m_selectedEvent;

		private enum State
		{
			neutral,
			newEffect,
			newTrigger,
			newCutscene
		}

		public CutsceneDevelopment(DevelopmentState a_backState, LinkedList<Event> a_events)
		{
			if (a_events == null)
			{
				throw new ArgumentNullException();
			}
			m_numOfAddedEvents = 0;
			m_state = State.neutral;
			m_backState = a_backState;
			m_buttonList = new LinkedList<Button>();
			Button t_buttonToAdd = new Button("dev_bg_info", "dev_bg_info", "dev_bg_info", "dev_bg_info", new Vector2(0, 0), "No more event plz", null, Color.Red, new Vector2(10, 10));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(exitState);
			m_buttonList.AddLast(t_buttonToAdd);

			t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser_normal", "DevelopmentHotkeys//btn_layer_chooser_hover", "DevelopmentHotkeys//btn_layer_chooser_pressed", "DevelopmentHotkeys//btn_layer_chooser_toggle", new Vector2(800, 650), "Add Eve", null, Color.Black, new Vector2(5, 5));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(addEvent);
			m_buttonList.AddLast(t_buttonToAdd);

			t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser_normal", "DevelopmentHotkeys//btn_layer_chooser_hover", "DevelopmentHotkeys//btn_layer_chooser_pressed", "DevelopmentHotkeys//btn_layer_chooser_toggle", new Vector2(700, 650), "Delete", null, Color.Black, new Vector2(10, 5));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(deleteEvent);
			m_buttonList.AddLast(t_buttonToAdd);

			t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser_normal", "DevelopmentHotkeys//btn_layer_chooser_hover", "DevelopmentHotkeys//btn_layer_chooser_pressed", "DevelopmentHotkeys//btn_layer_chooser_toggle", new Vector2(600, 650), "Add Eff", null, Color.Black, new Vector2(10, 5));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(addEffect);
			m_buttonList.AddLast(t_buttonToAdd);

			t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser_normal", "DevelopmentHotkeys//btn_layer_chooser_hover", "DevelopmentHotkeys//btn_layer_chooser_pressed", "DevelopmentHotkeys//btn_layer_chooser_toggle", new Vector2(500, 650), "Add Tri", null, Color.Black, new Vector2(10, 5));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(addTrigger);
			m_buttonList.AddLast(t_buttonToAdd);

			m_eventsToRemove = new Stack<Button>();
			m_eventsToAdd = new Stack<Event>();
			m_events = new Dictionary<Button, Event>();

			foreach (Event t_e in a_events)
			{
				addEvent(t_e);
			}
		}

		public void addEvent(Event t_e)
		{
			m_eventsToAdd.Push(t_e);
		}

		public override void update(GameTime a_gameTime)
		{
			foreach (Button t_b in m_buttonList)
			{
				t_b.update();
			}
			while (m_eventsToRemove.Count > 0)
			{
				Button t_bToRemove = m_eventsToRemove.Pop();
				m_events.Remove(t_bToRemove);
				m_buttonList.Remove(t_bToRemove);
			}
			while (m_eventsToAdd.Count > 0)
			{
				Button t_button = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", new Vector2(0, 100 + ((m_numOfAddedEvents++) * 30)), "" + m_events.Count, null, Color.Yellow, new Vector2(10, 2));
				t_button.m_clickEvent += new Button.clickDelegate(selectEvent);
				m_events.Add(t_button, m_eventsToAdd.Pop());
				m_buttonList.AddLast(t_button);
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
			if (m_selectedEvent != null)
			{
				m_selectedEvent.setState(0);
			}
			a_button.setState(3);
			m_selectedEvent = a_button;
		}

		public void addEvent(Button a_care)
		{
			m_eventsToAdd.Push(new Event(new LinkedList<EventTrigger>(), new LinkedList<EventEffect>(), true));
		}

		public void deleteEvent(Button a_care)
		{
			if (m_selectedEvent != null)
			{
				m_eventsToRemove.Push(m_selectedEvent);
				m_selectedEvent = null;
			}
		}

		public void addTrigger(Button a_care)
		{

		}

		public void addEffect(Button a_care)
		{
			if (m_selectedEvent != null && m_state == State.neutral)
			{
				m_state = State.newEffect;

				Button t_buttonToAdd; 

				t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser_normal", "DevelopmentHotkeys//btn_layer_chooser_hover", "DevelopmentHotkeys//btn_layer_chooser_pressed", "DevelopmentHotkeys//btn_layer_chooser_toggle", new Vector2(800, 600), "Cutscene", null, Color.Black, new Vector2(5, 5));
				t_buttonToAdd.m_clickEvent += new Button.clickDelegate(addEvent);
				m_buttonList.AddLast(t_buttonToAdd);
			}
		}

		public void addCutscene(Button a_care)
		{
			if (m_selectedEvent != null && m_state == State.newEffect)
			{
				m_state = State.newCutscene;
			}
		}
	}
}
