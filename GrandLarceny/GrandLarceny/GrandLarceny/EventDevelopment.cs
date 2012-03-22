﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GrandLarceny.Events;
using Microsoft.Xna.Framework.Input;
using GrandLarceny.Events.Effects;

namespace GrandLarceny
{
	class EventDevelopment : States
	{
		private DevelopmentState m_backState;

		private LinkedList<Button> m_buttonList;
		private LinkedList<GuiObject> m_guiList;
		private Dictionary<Button, Event> m_events;
		private Dictionary<Button, EventEffect> m_effects;
		private Dictionary<Button, EventTrigger> m_triggers;
		private Stack<Button> m_buttonsToAdd;
		private Stack<Button> m_buttonsToRemove;
		private Stack<Event> m_eventsToAdd;
		private Stack<Button> m_eventsToRemove;
		private Stack<LinkedList<Button>> m_stateButtons;

		private State m_state;
		private int m_numOfAddedEvents;

		private Button m_selectedEvent;
		private Button m_selectedEffTri;

		private Vector2 m_recPoint;
		private Line[] m_recLines;

		private enum State
		{
			neutral,
			newEffect,
			newTrigger,
			newCutscene,
			firRectanglePoint,
			secRectanglePoint
		}

		public EventDevelopment(DevelopmentState a_backState, LinkedList<Event> a_events)
		{
			if (a_events == null)
			{
				throw new ArgumentNullException();
			}
			m_numOfAddedEvents = 0;
			m_state = State.neutral;
			m_backState = a_backState;
			m_buttonList = new LinkedList<Button>();
			m_guiList = new LinkedList<GuiObject>();
			m_buttonsToAdd = new Stack<Button>();
			m_buttonsToRemove = new Stack<Button>();
			Button t_buttonToAdd = new Button("dev_bg_info", "dev_bg_info", "dev_bg_info", "dev_bg_info", new Vector2(0, 0), "No more event plz", null, Color.Red, new Vector2(10, 10));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(exitState);
			m_buttonList.AddFirst(t_buttonToAdd);

			t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser_normal", "DevelopmentHotkeys//btn_layer_chooser_hover", "DevelopmentHotkeys//btn_layer_chooser_pressed", "DevelopmentHotkeys//btn_layer_chooser_toggle", new Vector2(800, 650), "Add Eve", null, Color.Black, new Vector2(5, 5));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(addEvent);
			m_buttonList.AddFirst(t_buttonToAdd);

			t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser_normal", "DevelopmentHotkeys//btn_layer_chooser_hover", "DevelopmentHotkeys//btn_layer_chooser_pressed", "DevelopmentHotkeys//btn_layer_chooser_toggle", new Vector2(700, 650), "Delete", null, Color.Black, new Vector2(10, 5));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(deleteSelected);
			m_buttonList.AddFirst(t_buttonToAdd);

			t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser_normal", "DevelopmentHotkeys//btn_layer_chooser_hover", "DevelopmentHotkeys//btn_layer_chooser_pressed", "DevelopmentHotkeys//btn_layer_chooser_toggle", new Vector2(600, 650), "Add Eff", null, Color.Black, new Vector2(10, 5));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(addEffect);
			m_buttonList.AddFirst(t_buttonToAdd);

			t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser_normal", "DevelopmentHotkeys//btn_layer_chooser_hover", "DevelopmentHotkeys//btn_layer_chooser_pressed", "DevelopmentHotkeys//btn_layer_chooser_toggle", new Vector2(500, 650), "Add Tri", null, Color.Black, new Vector2(10, 5));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(addTrigger);
			m_buttonList.AddFirst(t_buttonToAdd);

			m_eventsToRemove = new Stack<Button>();
			m_eventsToAdd = new Stack<Event>();
			m_events = new Dictionary<Button, Event>();
			m_effects = new Dictionary<Button, EventEffect>();
			m_triggers = new Dictionary<Button, EventTrigger>();
			m_stateButtons = new Stack<LinkedList<Button>>();

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
			m_backState.updateCamera();
			if (m_state == State.firRectanglePoint && Game.lmbClicked())
			{
				m_recPoint = calculateWorldMouse();
				m_state = State.secRectanglePoint;
			}
			foreach (GuiObject t_go in m_guiList)
			{
				t_go.update(a_gameTime);
			}
			foreach (Button t_b in m_buttonList)
			{
				t_b.update();
			}
			if (Game.rmbClicked())
			{
				goUpOneState();
			}
			else if (m_state == State.newCutscene && Game.keyClicked(Keys.Enter))
			{
				addEffect(new CutsceneEffect(((TextField)m_guiList.First.Value).getText()));
				goUpOneState();
			}
			while (m_eventsToRemove.Count > 0)
			{
				Button t_bToRemove = m_eventsToRemove.Pop();
				m_events.Remove(t_bToRemove);
				m_buttonList.Remove(t_bToRemove);
			}
			while (m_buttonsToRemove.Count > 0)
			{
				m_buttonList.Remove(m_buttonsToRemove.Pop());
			}
			while (m_eventsToAdd.Count > 0)
			{
				Button t_button = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", new Vector2(0, 100 + ((m_numOfAddedEvents++) * 30)), "" + m_events.Count, null, Color.Yellow, new Vector2(10, 2));
				t_button.m_clickEvent += new Button.clickDelegate(selectEvent);
				m_events.Add(t_button, m_eventsToAdd.Pop());
				m_buttonList.AddFirst(t_button);
			}
			while (m_buttonsToAdd.Count > 0)
			{
				m_buttonList.AddFirst(m_buttonsToAdd.Pop());
			}
		}

		private void addEffect(EventEffect a_eveEffect)
		{
			if(m_selectedEvent != null)
			{
				m_events[m_selectedEvent].add(a_eveEffect);

				Button t_buttonToAdd = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", new Vector2(500, 100 + ((m_effects.Count) * 30)), a_eveEffect.ToString(), null, Color.Yellow, new Vector2(10, 2));
				t_buttonToAdd.m_clickEvent += new Button.clickDelegate(selectEffTri);
				m_buttonsToAdd.Push(t_buttonToAdd);

				m_effects.Add(t_buttonToAdd, a_eveEffect);
			}
		}


		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			m_backState.draw(a_gameTime, a_spriteBatch);
			foreach (Button t_b in m_buttonList)
			{
				t_b.draw(a_gameTime, a_spriteBatch);
			}
			foreach (GuiObject t_go in m_guiList)
			{
				t_go.draw(a_gameTime);
			}
		}

		public void goUpOneState()
		{
			if (m_state == State.newCutscene)
			{
				m_state = State.newEffect;
				m_guiList.RemoveFirst();
			}
			else if (m_state == State.newEffect)
			{
				foreach (Button t_b in m_stateButtons.Pop())
				{
					m_buttonsToRemove.Push(t_b);
				}
				m_state = State.neutral;
			}
			else if (m_state == State.newTrigger)
			{
				foreach (Button t_b in m_stateButtons.Pop())
				{
					m_buttonsToRemove.Push(t_b);
				}
				m_state = State.neutral;
			}
			else if (m_state == State.neutral)
			{
				selectEvent(null);
			}
		}

		//knappmetoder

		public void exitState(Button a_care)
		{
			LinkedList<Event> t_events = new LinkedList<Event>();
			foreach (Event t_e in m_events.Values)
			{
				t_events.AddLast(t_e);
			}
			m_backState.setEvents(t_events);
			Game.getInstance().setState(m_backState);
		}

		public void selectEvent(Button a_button)
		{
			if (a_button != m_selectedEvent && m_state == State.neutral)
			{
				if (m_selectedEvent != null)
				{
					m_selectedEvent.setState(0);
					foreach (Button t_b in m_effects.Keys)
					{
						m_buttonList.Remove(t_b);
					}
					m_effects.Clear();
					foreach (Button t_b in m_triggers.Keys)
					{
						m_buttonList.Remove(t_b);
					}
					m_triggers.Clear();
					m_selectedEffTri = null;
				}
				m_selectedEvent = a_button;
				if (a_button != null)
				{
					a_button.setState(3);
					LinkedList<EventEffect> t_effects = m_events[m_selectedEvent].getEffects();
					LinkedList<EventTrigger> t_triggers = m_events[m_selectedEvent].getTriggers();

					foreach (EventEffect t_ee in t_effects)
					{
						Button t_buttonToAdd;

						t_buttonToAdd = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", new Vector2(500, 100 + ((m_effects.Count) * 30)), t_ee.ToString(), null, Color.Yellow, new Vector2(10, 2));
						t_buttonToAdd.m_clickEvent += new Button.clickDelegate(selectEffTri);
						m_buttonsToAdd.Push(t_buttonToAdd);

						m_effects.Add(t_buttonToAdd, t_ee);
					}

					foreach (EventTrigger t_et in t_triggers)
					{
						Button t_buttonToAdd;

						t_buttonToAdd = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", new Vector2(400, 100 + ((m_triggers.Count) * 30)), t_et.ToString(), null, Color.Yellow, new Vector2(10, 2));
						t_buttonToAdd.m_clickEvent += new Button.clickDelegate(selectEffTri);
						m_buttonsToAdd.Push(t_buttonToAdd);

						m_triggers.Add(t_buttonToAdd, t_et);
					}
				}
			}
		}

		public void selectEffTri(Button a_button)
		{
			if (m_state == State.neutral)
			{
				if (m_selectedEffTri != null)
				{
					m_selectedEffTri.setState(0);
				}
				m_selectedEffTri = a_button;
				if (m_selectedEffTri != null)
				{
					m_selectedEffTri.setState(3);
				}
			}
		}

		public void addEvent(Button a_care)
		{
			m_eventsToAdd.Push(new Event(new LinkedList<EventTrigger>(), new LinkedList<EventEffect>(), true));
		}

		public void deleteSelected(Button a_care)
		{
			if (m_selectedEvent != null && m_state == State.neutral)
			{
				if (m_selectedEffTri == null)
				{
					m_eventsToRemove.Push(m_selectedEvent);
					m_selectedEvent = null;

					foreach (Button t_b in m_effects.Keys)
					{
						m_buttonList.Remove(t_b);
					}
					m_effects.Clear();
					foreach (Button t_b in m_triggers.Keys)
					{
						m_buttonList.Remove(t_b);
					}
					m_triggers.Clear();
					m_selectedEffTri = null;
				}
				else
				{
					if (m_effects.ContainsKey(m_selectedEffTri))
					{
						m_events[m_selectedEvent].remove(m_effects[m_selectedEffTri]);
					}
					else
					{
						m_events[m_selectedEvent].remove(m_triggers[m_selectedEffTri]);
					}
					m_buttonsToRemove.Push(m_selectedEffTri);
					m_selectedEffTri = null;
				}
			}
		}

		public void addTrigger(Button a_care)
		{
			if (m_state == State.newEffect)
			{
				goUpOneState();
			}
			if (m_selectedEvent != null && m_state == State.neutral)
			{
				m_state = State.newTrigger;

				Button t_buttonToAdd;

				t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser_normal", "DevelopmentHotkeys//btn_layer_chooser_hover", "DevelopmentHotkeys//btn_layer_chooser_pressed", "DevelopmentHotkeys//btn_layer_chooser_toggle", new Vector2(800, 600), "Player Within Rectangle", null, Color.Black, new Vector2(5, 5));
				t_buttonToAdd.m_clickEvent += new Button.clickDelegate(addRectangle);
				m_buttonsToAdd.Push(t_buttonToAdd);

				t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser_normal", "DevelopmentHotkeys//btn_layer_chooser_hover", "DevelopmentHotkeys//btn_layer_chooser_pressed", "DevelopmentHotkeys//btn_layer_chooser_toggle", new Vector2(700, 600), "Player Within Circle", null, Color.Black, new Vector2(5, 5));
				t_buttonToAdd.m_clickEvent += new Button.clickDelegate(addCircle);
				m_buttonsToAdd.Push(t_buttonToAdd);

				LinkedList<Button> t_submenu = new LinkedList<Button>();
				t_submenu.AddLast(t_buttonToAdd);
				m_stateButtons.Push(t_submenu);
			}
		}

		public void addEffect(Button a_care)
		{
			if (m_state == State.newTrigger)
			{
				goUpOneState();
			}
			if (m_selectedEvent != null && m_state == State.neutral)
			{
				m_state = State.newEffect;

				Button t_buttonToAdd; 

				t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser_normal", "DevelopmentHotkeys//btn_layer_chooser_hover", "DevelopmentHotkeys//btn_layer_chooser_pressed", "DevelopmentHotkeys//btn_layer_chooser_toggle", new Vector2(800, 600), "Cutscene", null, Color.Black, new Vector2(5, 5));
				t_buttonToAdd.m_clickEvent += new Button.clickDelegate(addCutscene);
				m_buttonsToAdd.Push(t_buttonToAdd);

				LinkedList<Button> t_submenu = new LinkedList<Button>();
				t_submenu.AddLast(t_buttonToAdd);
				m_stateButtons.Push(t_submenu);
			}
		}

		public void addCutscene(Button a_care)
		{
			if (m_selectedEvent != null && m_state == State.newEffect)
			{
				m_state = State.newCutscene;

				TextField t_textField = new TextField(new Vector2(300, 200), 200, 100, true, true, true, 0);

				m_guiList.AddFirst(t_textField);
			}
		}

		public void addRectangle(Button a_care)
		{
			
		}

		public void addCircle(Button a_care)
		{
			//You wish
		}
	}
}