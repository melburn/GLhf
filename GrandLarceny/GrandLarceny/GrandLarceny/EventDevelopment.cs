using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using GrandLarceny.Events;
using GrandLarceny.Events.Effects;
using GrandLarceny.Events.Triggers;

namespace GrandLarceny
{
	class EventDevelopment : States
	{
		private DevelopmentState m_backState;

		private Dictionary<Button, Event> m_events;
		//private Dictionary<Button, EventEffect> m_effects;
		//private Dictionary<Button, EventTrigger> m_triggers;

		//private LinkedList<Button> m_eventButtons;
		private Stack<Button> m_buttonsToAdd;
		private Stack<Button> m_buttonsToRemove;
		private Stack<Event> m_eventsToAdd;
		private Stack<Button> m_eventsToRemove;
		private Stack<LinkedList<Button>> m_stateButtons;

		private State m_state;
		private int m_numOfAddedEvents;

		/*
		-----------------------------------
		Button Collections 
		-----------------------------------
		*/
		private LinkedList<Button> m_eventButtons;
		private LinkedList<Button> m_triggerButtons;
		private LinkedList<Button> m_effectButtons;

		private LinkedList<Button> m_triggerMenu;
		private LinkedList<Button> m_effectMenu;

		private Button m_btnAddEvent;
		private Button m_btnAddTrigger;
		private Button m_btnAddEffect;

		private Button m_selectedEvent;
		private Button m_selectedEffect;
		private Button m_selectedTrigger;
		//private Button m_selectedEffTri;
		//private Button m_btnAddEvent;

		//rectangle stuff
		private Vector2 m_recPoint;
		private Line[] m_recLines;

		//switchtrigger stuff
		private Button m_switchTriggerType;
		private Dictionary<Button, SwitchTrigger.TriggerType> m_switchTriggerButtons;

		private enum State
		{
			neutral,		newEffect,			newTrigger,
			newCutscene,	firRectanglePoint,	secRectanglePoint,
			newEquip,		newSwitch,			selectSwitch,
			newDoorEffect,	newChase,			drawingRectangle
		}

		private MenuState m_menuState;
		private enum MenuState
		{
			AddEvent,		AddTrigger,			AddEffect
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
			m_buttonList.AddLast(m_eventButtons = new LinkedList<Button>());
			m_buttonsToAdd = new Stack<Button>();
			m_buttonsToRemove = new Stack<Button>();

			m_eventsToRemove = new Stack<Button>();
			m_eventsToAdd = new Stack<Event>();
			m_events = new Dictionary<Button, Event>();
			//m_effects = new Dictionary<Button, EventEffect>();
			//m_triggers = new Dictionary<Button, EventTrigger>();
			m_stateButtons = new Stack<LinkedList<Button>>();

			foreach (Event t_e in a_events)
			{
				addEvent(t_e);
			}
		}

		public override void load()
		{
			Vector2 t_textOffset = new Vector2(5, 2);
			m_menuState = MenuState.AddEvent;
			m_eventButtons = new LinkedList<Button>();
			m_triggerMenu = new LinkedList<Button>();
			m_effectMenu = new LinkedList<Button>();

			m_btnAddEvent = new Button("btn_asset_list", new Vector2(0, 100 + (m_eventsToAdd.Count * 25)), "Add Event", "VerdanaBold", Color.Black, t_textOffset);
			m_btnAddEvent.m_clickEvent += new Button.clickDelegate(newEvent);

			m_btnAddTrigger = new Button("btn_asset_list", new Vector2(250, 100 + (m_eventsToAdd.Count * 25)), "Add Trigger", "VerdanaBold", Color.Black, t_textOffset);
			m_btnAddTrigger.m_clickEvent += new Button.clickDelegate(newTrigger);

			m_btnAddEffect = new Button("btn_asset_list", new Vector2(500, 100 + (m_eventsToAdd.Count * 25)), "Add Effect", "VerdanaBold", Color.Black, t_textOffset);
			m_btnAddEffect.m_clickEvent += new Button.clickDelegate(addEffect);

			m_triggerMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(800, 600), "Rectangle", null, Color.Black, t_textOffset));
			m_triggerMenu.Last().m_clickEvent += new Button.clickDelegate(addRectangle);

			m_triggerMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(700, 600), "not done", null, Color.Black, t_textOffset));
			m_triggerMenu.Last().m_clickEvent += new Button.clickDelegate(addCircle);

			m_triggerMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(600, 600), "Switch/Button", null, Color.Black, t_textOffset));
			m_triggerMenu.Last().m_clickEvent += new Button.clickDelegate(addSwitch);

			m_triggerMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(500, 600), "Chase check", null, Color.Black, t_textOffset));
			m_triggerMenu.Last().m_clickEvent += new Button.clickDelegate(addChase);

			m_effectMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(800, 600), "Cutscene", null, Color.Black, t_textOffset));
			m_effectMenu.Last().m_clickEvent += new Button.clickDelegate(addCutscene);

			m_effectMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(700, 600), "Equip", null, Color.Black, t_textOffset));
			m_effectMenu.Last().m_clickEvent += new Button.clickDelegate(addEquip);

			m_effectMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(600, 600), "Door", null, Color.Black, t_textOffset));
			m_effectMenu.Last().m_clickEvent += new Button.clickDelegate(addDoorEffect);

			base.load();
		}

		public void addEvent(Event t_e)
		{
			m_eventsToAdd.Push(t_e);
		}

		#region List Adders
		public void newEvent(Button a_button)
		{
			a_button.move(new Vector2(0, 25));
			int i = 0;
			KeyValuePair<Button, Event>[] t_array = m_events.ToArray();
			for (int j = 0; j < t_array.Length; )
			{
				if (i == int.Parse(t_array[j++].Key.getText()))
				{
					j = 0;
					i++;
				}
			}
			Button t_button = new Button("btn_asset_list", new Vector2(0, 100 + ((m_numOfAddedEvents++) * 25)), "" + i, "VerdanaBold", Color.Yellow, new Vector2(10, 2));
			t_button.m_clickEvent += new Button.clickDelegate(selectEvent);
			m_events.Add(t_button, new Event(new LinkedList<EventTrigger>(), new LinkedList<EventEffect>(), true));
			m_eventButtons.AddLast(t_button);
		}

		public void newTrigger(Button a_button)
		{
			m_menuState = MenuState.AddTrigger;
			a_button.setState(3);
			/*
			a_button.move(new Vector2(0, 25));

			Button t_button = new Button("btn_asset_list", new Vector2(250, 100 + (m_triggerButtons.Count() * 25)), "", "VerdanaBold", Color.Yellow, new Vector2(10, 2));
			t_button.m_clickEvent += new Button.clickDelegate(selectTrigger);
			m_triggerButtons.AddLast(t_button);
			*/
		}

		private void newEffect(Button a_button)
		{
			a_button.move(new Vector2(0, 25));

			Button t_button = new Button("btn_asset_list", new Vector2(250, 100 + (m_effectButtons.Count() * 25)), "", "VerdanaBold", Color.Yellow, new Vector2(10, 2));
			t_button.m_clickEvent += new Button.clickDelegate(selectEffect);
			m_effectButtons.AddLast(t_button);
		}
		/*
		private void addEffect(EventEffect a_eveEffect)
		{
			if (m_selectedEvent != null)
			{
				m_events[m_selectedEvent].add(a_eveEffect);

				Button t_buttonToAdd = new Button("btn_asset_list", new Vector2(800, 100 + ((m_effects.Count) * 30)), a_eveEffect.ToString(), "VerdanaBold", Color.Yellow, new Vector2(10, 2));
				t_buttonToAdd.m_clickEvent += new Button.clickDelegate(selectTrigger);
				m_buttonsToAdd.Push(t_buttonToAdd);

				m_effects.Add(t_buttonToAdd, a_eveEffect);
			}
		}

		private void addTrigger(EventTrigger a_eveTrigger)
		{
			if (m_selectedEvent != null)
			{
				m_events[m_selectedEvent].add(a_eveTrigger);

				Button t_buttonToAdd = new Button("btn_asset_list", new Vector2(700, 100 + ((m_effects.Count) * 30)), a_eveTrigger.ToString(), "VerdanaBold", Color.Yellow, new Vector2(10, 2));
				t_buttonToAdd.m_clickEvent += new Button.clickDelegate(selectTrigger);
				m_buttonsToAdd.Push(t_buttonToAdd);

				m_triggers.Add(t_buttonToAdd, a_eveTrigger);
			}
		}
		*/
		public void addEffect(Button a_care)
		{
			if (m_state == State.newTrigger)
			{
				goUpOneState();
			}
			if (m_selectedEvent != null && m_state == State.neutral)
			{
				m_state = State.newEffect;
			}
		}

		public void addCutscene(Button a_care)
		{
			if (m_selectedEvent != null && m_state == State.newEffect)
			{
				m_state = State.newCutscene;

				TextField t_textField = new TextField(new Vector2(300, 200), 200, 100, true, true, true, 0);

				t_textField.setWrite(true);
				t_textField.setText("write the filename of the cutscene");

				m_guiList.AddFirst(t_textField);
			}
		}

		public void addEquip(Button a_care)
		{
			if (m_selectedEvent != null && m_state == State.newEffect)
			{
				m_state = State.newEquip;

				TextField t_textField = new TextField(new Vector2(300, 200), 200, 100, true, true, true, 0);

				t_textField.setWrite(true);
				t_textField.setText("write name(string):equip(bool)");

				m_guiList.AddFirst(t_textField);
			}
		}

		public void addRectangle(Button a_care)
		{
			m_state = State.firRectanglePoint;
		}

		public void addCircle(Button a_care)
		{
			//You wish
		}
		private void addSwitch(Button a_care)
		{
			if (m_state == State.newTrigger)
			{
				m_state = State.newSwitch;

				Button t_buttonToAdd;
				LinkedList<Button> t_submenu = new LinkedList<Button>();
				int i = 800;
				if (m_switchTriggerButtons == null)
				{
					m_switchTriggerButtons = new Dictionary<Button, SwitchTrigger.TriggerType>();
				}
				else
				{
					m_switchTriggerButtons.Clear();
				}
				foreach (SwitchTrigger.TriggerType t_sttt in System.Enum.GetValues(typeof(SwitchTrigger.TriggerType)))
				{
					t_buttonToAdd = new Button("DevelopmentHotkeys//btn_layer_chooser", 
						new Vector2(i, 550), t_sttt.ToString(), null, Color.Black, new Vector2(5, 5));
					t_buttonToAdd.m_clickEvent += new Button.clickDelegate(selectSwitchTrigger);
					m_buttonsToAdd.Push(t_buttonToAdd);
					t_submenu.AddLast(t_buttonToAdd);
					m_switchTriggerButtons.Add(t_buttonToAdd, t_sttt);
					i -= 100;
				}

				m_stateButtons.Push(t_submenu);
			}
		}

		private void addChase(Button a_button)
		{

		}
		#endregion

		public void goUpOneState()
		{
			if (m_menuState == MenuState.AddEffect)
			{
				m_selectedEffect = null;
				m_btnAddEffect.setState(0);
				m_menuState = MenuState.AddTrigger;
			}
			else if (m_menuState == MenuState.AddTrigger)
			{
				m_selectedTrigger = null;
				m_btnAddTrigger.setState(0);
				m_menuState = MenuState.AddEvent;
			}
			/*
			bool t_goingToPop = false;
			if (m_state == State.newCutscene || m_state == State.newEquip)
			{
				m_state = State.newEffect;
				m_guiList.RemoveFirst();
			}
			else if (m_state == State.newChase)
			{
				m_state = State.newChase;
				m_guiList.RemoveFirst();
			}
			else if (m_state == State.newEffect)
			{
				t_goingToPop = true;
				m_state = State.neutral;
			}
			else if (m_state == State.newTrigger)
			{
				t_goingToPop = true;
				m_state = State.neutral;
			}
			else if (m_state == State.neutral)
			{
				selectEvent(null);
			}
			else if (m_state == State.newSwitch)
			{
				t_goingToPop = true;
				m_state = State.newTrigger;
			}
			else if (m_state == State.selectSwitch)
			{
				m_state = State.newSwitch;
				m_switchTriggerType.setState(0);
				m_switchTriggerType = null;
			}
			else if (m_state == State.newDoorEffect)
			{
				t_goingToPop = true;
				m_state = State.newEffect;
			}

			if (t_goingToPop)
			{
				LinkedList<Button> t_pop = m_stateButtons.Pop();
				foreach (Button t_b in t_pop)
				{
					m_buttonsToRemove.Push(t_b);
				}
			}
			*/
		}

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

		#region Button Events
		public void selectEvent(Button a_button)
		{
			GuiListFactory.setSelection(m_eventButtons, 0);
			Vector2 t_listPosition = new Vector2(m_eventButtons.First().getBox().X + m_eventButtons.First().getBox().Width, m_eventButtons.First().getBox().Y);

			a_button.setState(3);
			m_menuState = MenuState.AddTrigger;
			m_triggerButtons = new LinkedList<Button>();
			m_selectedEvent = a_button;

			foreach (EventTrigger t_trigger in m_events[a_button].getTriggers())
			{
				m_triggerButtons.AddLast(new Button("btn_asset_list", new Vector2(t_listPosition.X, t_listPosition.Y + ((m_events[a_button].getTriggers().Count) * 30)), 
					t_trigger.ToString(), "VerdanaBold", Color.Yellow, new Vector2(10, 2)));
				m_triggerButtons.Last().m_clickEvent += new Button.clickDelegate(selectTrigger);
			}
		}

		public void selectTrigger(Button a_button)
		{
			GuiListFactory.setSelection(m_triggerButtons, 0);
			Vector2 t_listPosition = new Vector2(m_triggerButtons.First().getBox().X + m_triggerButtons.First().getBox().Width, m_triggerButtons.First().getBox().Y);

			a_button.setState(3);
			m_menuState = MenuState.AddEffect;
			m_effectButtons = new LinkedList<Button>();
			m_selectedTrigger = a_button;

			foreach (EventEffect t_effect in m_events[m_selectedEvent].getEffects())
			{
				m_effectButtons.AddLast(new Button("btn_asset_list", new Vector2(t_listPosition.X, t_listPosition.Y + ((m_events[m_selectedEvent].getEffects().Count) * 30)), 
					t_effect.ToString(), "VerdanaBold", Color.Yellow, new Vector2(10, 2)));
				m_effectButtons.Last().m_clickEvent += new Button.clickDelegate(selectEffect);
			}
		}

		private void selectEffect(Button a_button)
		{

		}
		#endregion

		/*
		public void deleteSelected(Button a_care)
		{
			if (m_selectedEvent != null && m_state == State.neutral)
			{
				if (m_selectedEffect == null)
				{
					m_eventsToRemove.Push(m_selectedEvent);
					m_selectedEvent = null;

					foreach (Button t_b in m_effects.Keys)
					{
						m_buttonsToRemove.Push(t_b);
					}
					m_effects.Clear();
					foreach (Button t_b in m_triggers.Keys)
					{
						m_buttonsToRemove.Push(t_b);
					}
					m_triggers.Clear();
					m_selectedEffect = null;
				}
				else
				{
					if (m_effects.ContainsKey(m_selectedEffect))
					{
						m_events[m_selectedEvent].remove(m_effects[m_selectedEffect]);
					}
					else
					{
						m_events[m_selectedEvent].remove(m_triggers[m_selectedEffect]);
					}
					m_buttonsToRemove.Push(m_selectedEffect);
					m_selectedEffect = null;
				}
			}
		}
		*/
		 
		public void selectSwitchTrigger(Button a_button)
		{
			if (m_state == State.newSwitch || m_state == State.selectSwitch)
			{
				m_state = State.selectSwitch;
				if (m_switchTriggerType != null)
				{
					m_switchTriggerType.setState(0);
				}
				m_switchTriggerType = a_button;
				m_switchTriggerType.setState(3);
			}
		}
		public void addDoorEffect(Button a_care)
		{
			if (m_state == State.newEffect)
			{
				m_state = State.newDoorEffect;
			}
		}
		public void addChaseTrigger(Button a_care)
		{
			if (m_state == State.newTrigger)
			{
				m_state = State.newChase;
				
				TextField t_textField = new TextField(new Vector2(300, 200), 200, 100, true, true, true, 0);

				t_textField.setWrite(true);
				t_textField.setText("write bool");

				m_guiList.AddFirst(t_textField);
			}
		}

		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{
			updateMouse();
			updateGUI();
			m_backState.updateCamera();
		}

		private void updateMouse()
		{
			Vector2 t_mouse = calculateWorldMouse();
			/*
			-----------------------------------
			Middle-mouse drag
			-----------------------------------
			*/
			if (Game.m_currentMouse.MiddleButton == ButtonState.Pressed && Game.m_previousMouse.MiddleButton == ButtonState.Pressed)
			{
				Vector2 t_difference = Game.getInstance().m_camera.getPosition().getGlobalCartesian();
				t_difference.X = (Mouse.GetState().X - Game.getInstance().getResolution().X / 2) / 20 / Game.getInstance().m_camera.getZoom();
				t_difference.Y = (Mouse.GetState().Y - Game.getInstance().getResolution().Y / 2) / 20 / Game.getInstance().m_camera.getZoom();
				Game.getInstance().m_camera.getPosition().plusWith(t_difference);
			}

			/*
			-----------------------------------
			Left Mouse Button Click Down
			-----------------------------------
			*/
			if (Game.m_currentMouse.LeftButton == ButtonState.Pressed && Game.m_previousMouse.LeftButton == ButtonState.Released)
			{
				if (m_state == State.firRectanglePoint)
				{
					m_recPoint = calculateWorldMouse();
					m_state = State.drawingRectangle;

					m_recLines = new Line[4];
					CartesianCoordinate t_stopidPoint = new CartesianCoordinate(m_recPoint);

					for (int i = 0; i < 4; ++i)
					{
						m_recLines[i] = new Line(t_stopidPoint, t_stopidPoint, Vector2.Zero, Vector2.Zero, Color.Yellow, 2, true);
					}
				}
			}

			/*
			-----------------------------------
			Left Mouse Button Drag
			-----------------------------------
			*/
			if (Game.m_currentMouse.LeftButton == ButtonState.Pressed && Game.m_previousMouse.LeftButton == ButtonState.Pressed)
			{
				if (m_state == State.drawingRectangle)
				{			
					m_recLines[0].setEndPoint(new Vector2(t_mouse.X, m_recPoint.Y));
					m_recLines[1].setEndPoint(new Vector2(m_recPoint.X, t_mouse.Y));
					m_recLines[2].setEndPoint(t_mouse);
					m_recLines[3].setEndPoint(t_mouse);
					m_recLines[2].setStartPoint(new Vector2(t_mouse.X, m_recPoint.Y));
					m_recLines[3].setStartPoint(new Vector2(m_recPoint.X, t_mouse.Y));
				}
			}

			/*
			-----------------------------------
			Left Mouse Button Release
			-----------------------------------
			*/
			if (Game.m_currentMouse.LeftButton == ButtonState.Released && Game.m_previousMouse.LeftButton == ButtonState.Pressed)
			{
				if (m_state == State.drawingRectangle)
				{
					m_events[m_selectedEvent].add(new PlayerIsWithinRectangle(m_recPoint.X, m_recPoint.Y, t_mouse.X, t_mouse.Y, Game.getInstance().m_camera.getLayer()));
					m_btnAddTrigger.setState(0);
					m_btnAddTrigger.move(new Vector2(0, 25));
					selectEvent(m_selectedEvent);
					m_state = State.newTrigger;
					m_recLines = new Line[0];
				}
			}

			if (Game.rmbDown())
			{
				goUpOneState();
			}
		}

		private void updateGUI()
		{
			if (m_menuState == MenuState.AddEffect)
			{
				foreach (Button t_button in m_effectButtons)
				{
					t_button.update();
				}
				m_btnAddEffect.update();
			}

			if (m_menuState == MenuState.AddTrigger)
			{
				foreach (Button t_button in m_triggerButtons)
				{
					t_button.update();
				}
				m_btnAddTrigger.update();
			}

			if (m_menuState == MenuState.AddEvent)
			{
				foreach (Button t_button in m_eventButtons)
				{
					t_button.update();
				}
				m_btnAddEvent.update();
			}

			if (m_btnAddEffect.getState() == 3)
			{
				foreach (Button t_button in m_effectMenu)
				{
					t_button.update();
				}
			}
			else if (m_btnAddTrigger.getState() == 3)
			{
				foreach (Button t_button in m_triggerMenu)
				{
					t_button.update();
				}
			}
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			m_backState.draw(a_gameTime, a_spriteBatch);

			if (m_menuState == MenuState.AddEffect)
			{
				foreach (Button t_button in m_effectButtons)
				{
					t_button.draw(a_gameTime, a_spriteBatch);
				}
				m_btnAddEffect.draw(a_gameTime, a_spriteBatch);
			}

			if (m_menuState == MenuState.AddTrigger || m_menuState == MenuState.AddEffect)
			{
				foreach (Button t_button in m_triggerButtons)
				{
					t_button.draw(a_gameTime, a_spriteBatch);
				}
				m_btnAddTrigger.draw(a_gameTime, a_spriteBatch);
			}

			if (m_menuState == MenuState.AddTrigger || m_menuState == MenuState.AddEvent || m_menuState == MenuState.AddEffect)
			{
				foreach (Button t_button in m_eventButtons)
				{
					t_button.draw(a_gameTime, a_spriteBatch);
				}
				m_btnAddEvent.draw(a_gameTime, a_spriteBatch);
			}

			if (m_btnAddEffect.getState() == 3)
			{
				foreach (Button t_button in m_effectMenu)
				{
					t_button.draw(a_gameTime, a_spriteBatch);
				}
			}
			else if (m_btnAddTrigger.getState() == 3)
			{
				foreach (Button t_button in m_triggerMenu)
				{
					t_button.draw(a_gameTime, a_spriteBatch);
				}
			}

			foreach (GuiObject t_go in m_guiList)
			{
				t_go.draw(a_gameTime);
			}

			if (m_state == State.drawingRectangle)
			{
				foreach (Line t_lineList in m_recLines)
				{
					t_lineList.draw();
				}
			}
		}
		#endregion
	}
}
