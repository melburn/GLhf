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
using System.IO;

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
		private LinkedList<Event> m_eventList;

		private Button m_btnAddEvent;
		private Button m_btnAddTrigger;
		private Button m_btnAddEffect;

		private Button m_selectedEvent;
		private Button m_selectedEffect;
		private Button m_selectedTrigger;

		private Text m_textFieldInfo;
		private TextField m_textField;
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

		public EventDevelopment(DevelopmentState a_backState, LinkedList<Event> a_events)
		{
			m_numOfAddedEvents = 0;
			m_state = State.neutral;
			m_backState = a_backState;
			m_buttonList.AddLast(m_eventButtons = new LinkedList<Button>());
			m_buttonsToAdd = new Stack<Button>();
			m_buttonsToRemove = new Stack<Button>();

			m_eventsToRemove = new Stack<Button>();
			m_eventsToAdd = new Stack<Event>();
			m_events = new Dictionary<Button, Event>();
			m_stateButtons = new Stack<LinkedList<Button>>();

			m_eventList = a_events;
			buildEventList();
		}

		public override void load()
		{
			Vector2 t_textOffset = new Vector2(5, 2);
			m_eventButtons = new LinkedList<Button>();
			m_triggerButtons = new LinkedList<Button>();
			m_effectButtons = new LinkedList<Button>();
			m_triggerMenu = new LinkedList<Button>();
			m_effectMenu = new LinkedList<Button>();
			m_recLines = new Line[4];
			m_textField = null;

			m_btnAddEvent = new Button("btn_asset_list", new Vector2(0, (m_eventButtons.Count * 25)), "Add Event", "VerdanaBold", Color.Black, t_textOffset);
			m_btnAddEvent.m_clickEvent += new Button.clickDelegate(newEvent);

			m_btnAddTrigger = new Button("btn_asset_list", new Vector2(0, 40 + (m_triggerButtons.Count * 25)), "Add Trigger", "VerdanaBold", Color.Black, t_textOffset);
			m_btnAddTrigger.m_clickEvent += new Button.clickDelegate(newTrigger);

			m_btnAddEffect = new Button("btn_asset_list", new Vector2(m_btnAddTrigger.getBox().Width, 40 + (m_effectButtons.Count * 25)), "Add Effect", "VerdanaBold", Color.Black, t_textOffset);
			m_btnAddEffect.m_clickEvent += new Button.clickDelegate(newEffect);

			m_triggerMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(800, 600), "Rectangle", "VerdanaBold", Color.Black, t_textOffset));
			m_triggerMenu.Last().m_clickEvent += new Button.clickDelegate(addRectangle);

			m_triggerMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(700, 600), "not done", "VerdanaBold", Color.Black, t_textOffset));
			m_triggerMenu.Last().m_clickEvent += new Button.clickDelegate(addCircle);

			m_triggerMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(600, 600), "Switch/Button", "VerdanaBold", Color.Black, t_textOffset));
			m_triggerMenu.Last().m_clickEvent += new Button.clickDelegate(addSwitch);

			m_triggerMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(500, 600), "Chase check", "VerdanaBold", Color.Black, t_textOffset));
			m_triggerMenu.Last().m_clickEvent += new Button.clickDelegate(addChase);

			m_effectMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(800, 600), "Cutscene", "VerdanaBold", Color.Black, t_textOffset));
			m_effectMenu.Last().m_clickEvent += new Button.clickDelegate(addCutscene);

			m_effectMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(700, 600), "Equip", "VerdanaBold", Color.Black, t_textOffset));
			m_effectMenu.Last().m_clickEvent += new Button.clickDelegate(addEquip);

			m_effectMenu.AddLast(new Button("DevelopmentHotkeys//btn_layer_chooser", new Vector2(600, 600), "Door", "VerdanaBold", Color.Black, t_textOffset));
			m_effectMenu.Last().m_clickEvent += new Button.clickDelegate(addDoorEffect);

			base.load();
		}

		public void addEvent(Event t_e)
		{
			m_eventsToAdd.Push(t_e);
		}

		#region List Adders
		private void buildEventList() {
			foreach (Event t_e in m_eventList)
			{
				while (m_eventsToAdd.Count > 0)
				{
					int i = 0;
					KeyValuePair<Button, Event>[] t_array = m_events.ToArray();
					for (int j = 0; j < t_array.Length; ) {
						if (i == int.Parse(t_array[j++].Key.getText())) {
							j = 0;
							i++;
						}
					}
					Button t_button = new Button("btn_asset_list", new Vector2(0, (m_numOfAddedEvents++) * 25), "" + i, null, Color.Yellow, new Vector2(10, 2));
					t_button.m_clickEvent += new Button.clickDelegate(selectEvent);
					m_events.Add(t_button, m_eventsToAdd.Pop());
					m_eventButtons.AddFirst(t_button);
				}
			}
		}

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
			Button t_button = new Button("btn_asset_list", new Vector2(0, (m_numOfAddedEvents++) * 25), "Event: " + i, "VerdanaBold", Color.Yellow, new Vector2(10, 2));
			t_button.m_clickEvent += new Button.clickDelegate(selectEvent);
			m_events.Add(t_button, new Event(new LinkedList<EventTrigger>(), new LinkedList<EventEffect>(), true));
			m_eventButtons.AddLast(t_button);
		}

		public void newTrigger(Button a_button)
		{
			a_button.setState(3);
		}

		private void newEffect(Button a_button)
		{
			a_button.setState(3);
		}
		#endregion

		#region Add Trigger
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

		private void addChase(Button a_button)
		{

		}
		#endregion

		#region Add Effect
		public void addCutscene(Button a_care)
		{
			m_state = State.newCutscene;
			m_textField = new TextField(new Vector2(Game.getInstance().getResolution().X / 2 - 100, 200), 200, 25, true, true, true, 0);
			m_textFieldInfo = new Text(new Vector2(Game.getInstance().getResolution().X / 2 - m_textField.getSize().X / 2, 175), "Filename of the Cutscene", "VerdanaBold", Color.White, false);
			m_textField.setWrite(true);
		}

		public void addEquip(Button a_care)
		{
			m_state = State.newEquip;
			m_textField = new TextField(new Vector2(Game.getInstance().getResolution().X / 2 - 100, 200), 200, 25, true, true, true, 0);
			m_textFieldInfo = new Text(new Vector2(Game.getInstance().getResolution().X / 2 - m_textField.getSize().X / 2, 175), "name:equip", "VerdanaBold", Color.White, false);
			m_textField.setWrite(true);
		}

		public void addDoorEffect(Button a_care)
		{
			m_state = State.newDoorEffect;
		}
		#endregion

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

			a_button.setState(3);
			m_triggerButtons = new LinkedList<Button>();
			m_effectButtons = new LinkedList<Button>();
			m_selectedEvent = a_button;
			m_selectedEvent.setPosition(Vector2.Zero);

			foreach (EventTrigger t_trigger in m_events[a_button].getTriggers())
			{
				m_triggerButtons.AddLast(new Button("btn_asset_list", new Vector2(m_btnAddTrigger.getBox().X, 40 + (m_triggerButtons.Count * 25)), 
					t_trigger.ToString(), "VerdanaBold", Color.Yellow, new Vector2(10, 2)));
				m_triggerButtons.Last().m_clickEvent += new Button.clickDelegate(selectTrigger);
				if (t_trigger is PlayerIsWithinRectangle) {
					m_recLines = ((PlayerIsWithinRectangle)t_trigger).getRectangle();
				}
			}

			foreach (EventEffect t_effect in m_events[m_selectedEvent].getEffects())
			{
				m_effectButtons.AddLast(new Button("btn_asset_list", new Vector2(m_btnAddEffect.getBox().X, 40 + (m_effectButtons.Count * 25)), 
					t_effect.ToString(), "VerdanaBold", Color.Yellow, new Vector2(10, 2)));
				m_effectButtons.Last().m_clickEvent += new Button.clickDelegate(selectEffect);
			}

			if (m_triggerButtons.Count() > 0) {
				m_btnAddTrigger.setPosition(new Vector2(m_triggerButtons.Last().getBox().X, m_triggerButtons.Last().getBox().Y + 25));
			} else {
				m_btnAddTrigger.setPosition(new Vector2(0, 40));
			}
			if (m_effectButtons.Count() > 0) {
				m_btnAddEffect.setPosition(new Vector2(m_effectButtons.Last().getBox().X, m_effectButtons.Last().getBox().Y + 25));	
			} else {
				m_btnAddEffect.setPosition(new Vector2(m_btnAddTrigger.getBox().Width, 40));
			}
		}

		public void selectTrigger(Button a_button)
		{
			a_button.setState(3);
		}

		private void selectEffect(Button a_button)
		{
			a_button.setState(3);
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
			updateGUI(a_gameTime);
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
			if (Game.mmbPressed())
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
			if (Game.lmbDown())
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
				if (m_state == State.newDoorEffect)
				{
					Vector2 t_mousePoint = calculateWorldMouse();
					foreach (GameObject t_go in m_backState.getCurrentList())
					{
						if (t_go is SecurityDoor && t_go.getBox().Contains((int)t_mousePoint.X, (int)t_mousePoint.Y))
						{
							m_events[m_selectedEvent].add(new DoorOpenEffect((SecurityDoor)t_go, 1, 1));
							selectEvent(m_selectedEvent);
							m_btnAddEffect.setState(0);
							break;
						}
					}
				}
			}

			/*
			-----------------------------------
			Left Mouse Button Drag
			-----------------------------------
			*/
			if (Game.lmbPressed())
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
			if (Game.lmbUp())
			{
				if (m_state == State.drawingRectangle)
				{
					m_events[m_selectedEvent].add(new PlayerIsWithinRectangle(m_recPoint.X, m_recPoint.Y, t_mouse.X, t_mouse.Y, Game.getInstance().m_camera.getLayer()));
					m_btnAddTrigger.setState(0);
					m_state = State.newTrigger;
					m_recLines = new Line[0];
					selectEvent(m_selectedEvent);
				}
			}

			if (Game.rmbDown())
			{
				m_selectedEvent = null;
				GuiListFactory.setListPosition(m_eventButtons, new Vector2(0, 0));
				GuiListFactory.setButtonDistance(m_eventButtons, new Vector2(0, 25));
				m_btnAddEvent.setPosition(new Vector2(m_eventButtons.Last().getBox().X, m_eventButtons.Last().getBox().Y) + new Vector2(0, 25));
				m_textField = null;
				m_textFieldInfo = null;
				m_selectedEffect = null;
				m_selectedTrigger = null;
				m_btnAddEffect.setState(0);
				m_btnAddTrigger.setState(0);
				m_recLines = new Line[4];
			}

			if (Game.keyClicked(Keys.Enter))
			{
				if (m_textField != null && m_textField.isWriting())
				{
					switch (m_state)
					{
						case State.newEquip:
							m_events[m_selectedEvent].add(new EquipEffect(m_textField.getText(), true));					
							break;
						case State.newCutscene:
							if (File.Exists(Game.CUTSCENE_FOLDER + m_textField.getText() + ".csn")) {
								m_events[m_selectedEvent].add(new CutsceneEffect(m_textField.getText() + ".csn"));
							} else {
								m_textFieldInfo.setText("File doesn't exist!");
								m_textFieldInfo.setColor(Color.Red);
								return;
							}
							break;
						case State.newDoorEffect:
							m_events[m_selectedEvent].add(new DoorOpenEffect(null, 1.0f, 1.0f));
							break;
					}
					m_btnAddEffect.setState(0);
					selectEvent(m_selectedEvent);
					m_textField = null;
					m_textFieldInfo = null;
				}
			}
		}

		private void updateGUI(GameTime a_gameTime)
		{
			if (m_selectedEvent != null) {
				foreach (Button t_button in m_triggerButtons)
					t_button.update();
				foreach (Button t_button in m_effectButtons)
					t_button.update();
				m_btnAddEffect.update();
				m_btnAddTrigger.update();
			} else {
				foreach (Button t_button in m_eventButtons)
					t_button.update();
				m_btnAddEvent.update();
			}

			if (m_btnAddEffect.getState() == 3)
			{
				foreach (Button t_button in m_effectMenu)
					t_button.update();
			}
			else if (m_btnAddTrigger.getState() == 3)
			{
				foreach (Button t_button in m_triggerMenu)
					t_button.update();
			}

			if (m_textField != null)
				m_textField.update(a_gameTime);
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			m_backState.draw(a_gameTime, a_spriteBatch);

			if (m_selectedEvent != null) {
				foreach (Button t_button in m_triggerButtons)
					t_button.draw(a_gameTime, a_spriteBatch);
				foreach (Button t_button in m_effectButtons)
					t_button.draw(a_gameTime, a_spriteBatch);
				m_btnAddEffect.draw(a_gameTime, a_spriteBatch);
				m_btnAddTrigger.draw(a_gameTime, a_spriteBatch);
				m_selectedEvent.draw(a_gameTime, a_spriteBatch);
			} else {
				foreach (Button t_button in m_eventButtons)
					t_button.draw(a_gameTime, a_spriteBatch);
				m_btnAddEvent.draw(a_gameTime, a_spriteBatch);
			}

			if (m_btnAddEffect.getState() == 3)
				foreach (Button t_button in m_effectMenu)
					t_button.draw(a_gameTime, a_spriteBatch);
			else if (m_btnAddTrigger.getState() == 3)
				foreach (Button t_button in m_triggerMenu)
					t_button.draw(a_gameTime, a_spriteBatch);

			foreach (GuiObject t_go in m_guiList)
				t_go.draw(a_gameTime);

			foreach (Line t_line in m_recLines)
				if (t_line != null)
					t_line.draw();
			if (m_textField != null) {
				m_textFieldInfo.draw(a_gameTime);
				m_textField.draw(a_gameTime);
			}
		}
		#endregion
	}
}