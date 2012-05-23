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
		#region Members
		private DevelopmentState m_backState;
		private Dictionary<Button, Event> m_events;
		private Dictionary<Button, SwitchTrigger.TriggerType> m_switchTriggerButtons;

		private LinkedList<Button> m_triggerButtons;
		private LinkedList<Button> m_effectButtons;
		private LinkedList<TextButton> m_triggerMenu;
		private LinkedList<TextButton> m_effectMenu;

		private Button m_btnAddEvent;
		private Button m_btnAddTrigger;
		private Button m_btnAddEffect;
		private Button m_selectedEvent;
		private Button m_selectedTriggerEffect;
		private Button m_deleteEvent;
		private Button m_deleteTriggerEffect;
		private Button m_switchTriggerType;
		private Button m_exitEvent;

		private Box m_background;

		private Text m_textFieldInfo;
		private TextField m_textField;

		//rectangle stuff
		private Vector2 m_recPoint;
		private Line[] m_recLines;

		private State m_state;
		private enum State
		{
			neutral,		newEffect,			newTrigger,
			newCutscene,	firRectanglePoint,	secRectanglePoint,
			newEquip,		newSwitch,			selectSwitch,
			newDoorEffect,	newChase,			drawingRectangle,
			selectingObject
		}
		#endregion

		#region Constructor and Load
		public EventDevelopment(DevelopmentState a_backState, LinkedList<Event> a_events)
		{
			m_state = State.neutral;
			m_backState = a_backState;
			m_events = new Dictionary<Button, Event>();

			buildEventList(a_events);
		}

		public override void load()
		{
			base.load();
			Game.getInstance().m_camera.setPosition(Vector2.Zero);
			Vector2 t_textOffset = new Vector2(5, 2);
			m_triggerButtons = new LinkedList<Button>();
			m_effectButtons = new LinkedList<Button>();
			m_triggerMenu = new LinkedList<TextButton>();
			m_effectMenu = new LinkedList<TextButton>();
			m_recLines = new Line[4];
			m_textField = null;
			m_background = new Box(Vector2.Zero, 400, Game.getInstance().getResolution().Y, Color.Gray, false);

			m_btnAddEvent = new Button("btn_asset_list", new Vector2(0, (m_events.Keys.Count * 25)), "Add Event", "VerdanaBold", Color.Black, t_textOffset);
			m_btnAddEvent.m_clickEvent += new Button.clickDelegate(newEvent);

			m_btnAddTrigger = new Button("btn_asset_list", Vector2.Zero, "Add Trigger", "VerdanaBold", Color.Black, t_textOffset);
			m_btnAddTrigger.m_clickEvent += new Button.clickDelegate(newTrigger);

			m_btnAddEffect = new Button("btn_asset_list", Vector2.Zero, "Add Effect", "VerdanaBold", Color.Black, t_textOffset);
			m_btnAddEffect.m_clickEvent += new Button.clickDelegate(newEffect);

			m_triggerMenu.AddLast(new TextButton(new Vector2(400, 555), "Rectangle", "MotorwerkNormal"));
			m_triggerMenu.Last().m_clickEvent += new TextButton.clickDelegate(addRectangle);

			m_triggerMenu.AddLast(new TextButton(new Vector2(400, 580), "Circle (NYI)", "MotorwerkNormal"));
			m_triggerMenu.Last().m_clickEvent += new TextButton.clickDelegate(addCircle);

			m_triggerMenu.AddLast(new TextButton(new Vector2(400, 605), "Switch/Button", "MotorwerkNormal"));
			m_triggerMenu.Last().m_clickEvent += new TextButton.clickDelegate(addSwitch);

			m_triggerMenu.AddLast(new TextButton(new Vector2(400, 630), "Chase check", "MotorwerkNormal"));
			m_triggerMenu.Last().m_clickEvent += new TextButton.clickDelegate(addChase);

			m_triggerMenu.AddLast(new TextButton(new Vector2(400, 655), "IsDeadTrigger", "MotorwerkNormal"));
			m_triggerMenu.Last().m_clickEvent += new TextButton.clickDelegate(addIsDead);

			m_effectMenu.AddLast(new TextButton(new Vector2(400, 580), "Cutscene", "MotorwerkNormal"));
			m_effectMenu.Last().m_clickEvent += new TextButton.clickDelegate(addCutscene);

			m_effectMenu.AddLast(new TextButton(new Vector2(400, 605), "Equip", "MotorwerkNormal"));
			m_effectMenu.Last().m_clickEvent += new TextButton.clickDelegate(addEquip);

			m_effectMenu.AddLast(new TextButton(new Vector2(400, 630), "Door", "MotorwerkNormal"));
			m_effectMenu.Last().m_clickEvent += new TextButton.clickDelegate(addDoorEffect);

			m_deleteEvent = new Button("DevelopmentHotkeys//btn_delete_hotkey", new Vector2(250, 0));
			m_deleteEvent.m_clickEvent += new Button.clickDelegate(deleteEvent);

			m_exitEvent = new TextButton(new Vector2(20, Game.getInstance().getResolution().Y - 100), "Exit", "MotorwerkLarge");
			((TextButton)m_exitEvent).m_clickEvent += new TextButton.clickDelegate(exitState);

			m_deleteTriggerEffect = new Button("btn_small_delete", Vector2.Zero);
			m_deleteTriggerEffect.m_clickEvent += new Button.clickDelegate(deleteTriggerEffect);
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
				m_switchTriggerButtons.Add(t_buttonToAdd, t_sttt);
				i -= 100;
			}
		}

		private void addChase(Button a_button)
		{
			toggleTextField("Chase Trigger?");
		}

		private void addIsDead(Button a_button)
		{
			m_state = State.selectingObject;
		}
		#endregion

		#region Add Effect
		public void addCutscene(Button a_care)
		{
			m_state = State.newCutscene;
			toggleTextField("Filename of the Cutscene");
		}

		public void addEquip(Button a_care)
		{
			m_state = State.newEquip;
			toggleTextField("name:equip");
		}

		public void addDoorEffect(Button a_care)
		{
			m_state = State.newDoorEffect;
		}
		#endregion

		#region EventState Methods
		private void buildEventList(LinkedList<Event> a_eventList)
		{
			int i = 0;
			foreach (Event t_e in a_eventList)
			{
				Button t_button;
				m_events.Add(t_button = new Button("btn_asset_list", new Vector2(0, m_events.Keys.Count * 25), "" + i, null, Color.Yellow, new Vector2(10, 2)), t_e);;
				t_button.m_clickEvent += new Button.clickDelegate(selectEvent);
			}
		}

		private void deselectEvent()
		{
			m_selectedEvent = null;

			if (m_events.Keys.Count > 0)
			{
				m_btnAddEvent.setPosition(new Vector2(m_events.Keys.Last().getBox().X, m_events.Keys.Last().getBox().Y) + new Vector2(0, 25));
			}
			else
			{
				m_btnAddEvent.setPosition(Vector2.Zero);				
			}
			m_textField			= null;
			m_textFieldInfo		= null;
			m_selectedTriggerEffect = null;
			m_btnAddEffect.setState(0);
			m_btnAddTrigger.setState(0);
			m_recLines = new Line[4];
		}

		private void toggleTextField(string a_titleText)
		{
			if (m_textField == null)
			{
				m_textField = new TextField(new Vector2(Game.getInstance().getResolution().X / 2 - 100, 200), 200, 25, true, true, true, 0);
				m_textFieldInfo = new Text(new Vector2(Game.getInstance().getResolution().X / 2 - m_textField.getSize().X / 2, 175), a_titleText, "VerdanaBold", Color.White, false);
			}
			else
			{
				m_textField = null;
				m_textFieldInfo = null;
			}
		}
		#endregion

		#region Button Events
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
			Button t_button;
			m_events.Add(t_button = new Button("btn_asset_list", new Vector2(0, m_events.Count * 25), "" + i, "VerdanaBold", Color.Yellow, new Vector2(10, 2)), new Event(new LinkedList<EventTrigger>(), new LinkedList<EventEffect>(), true));
			t_button.m_clickEvent += new Button.clickDelegate(selectEvent);
		}

		public void newTrigger(Button a_button)
		{
			m_btnAddEffect.setState(Button.State.Normal);
			a_button.setState(Button.State.Toggled);
		}

		private void newEffect(Button a_button)
		{
			m_btnAddTrigger.setState(Button.State.Normal);
			a_button.setState(Button.State.Toggled);
		}

		public void selectEvent(Button a_button)
		{
			foreach (Button t_button in m_events.Keys)
			{
				t_button.setState(Button.State.Normal);
			}

			a_button.setState(3);
			m_triggerButtons = new LinkedList<Button>();
			m_effectButtons = new LinkedList<Button>();
			m_selectedEvent = a_button;
			m_selectedEvent.setPosition(Vector2.Zero);

			foreach (EventTrigger t_trigger in m_events[a_button].getTriggers())
			{
				m_triggerButtons.AddLast(new Button("btn_asset_list", new Vector2(m_btnAddTrigger.getBox().X, 40 + (m_triggerButtons.Count * 25)), 
					t_trigger.ToString(), "VerdanaBold", Color.Yellow, new Vector2(10, 2)));
				m_triggerButtons.Last().m_clickEvent += new Button.clickDelegate(selectEffectTrigger);
				if (t_trigger is PlayerIsWithinRectangle)
				{
					m_recLines = ((PlayerIsWithinRectangle)t_trigger).getRectangle();
				}
			}

			foreach (EventEffect t_effect in m_events[a_button].getEffects())
			{
				m_effectButtons.AddLast(new Button("btn_asset_list", new Vector2(m_btnAddTrigger.getBox().X + m_btnAddEffect.getBox().Width + 40, 40 + (m_effectButtons.Count * 25)), 
					t_effect.ToString(), "VerdanaBold", Color.Yellow, new Vector2(10, 2)));
				m_effectButtons.Last().m_clickEvent += new Button.clickDelegate(selectEffectTrigger);
			}

			if (m_triggerButtons.Count() > 0)
			{
				m_btnAddTrigger.setPosition(new Vector2(m_triggerButtons.Last().getBox().X, m_triggerButtons.Last().getBox().Y + 25));
			}
			else
			{
				m_btnAddTrigger.setPosition(new Vector2(0, 40));
			}

			if (m_effectButtons.Count() > 0)
			{
				m_btnAddEffect.setPosition(new Vector2(m_effectButtons.Last().getBox().X, m_effectButtons.Last().getBox().Y + 25));	
			}
			else
			{
				m_btnAddEffect.setPosition(new Vector2(m_btnAddTrigger.getBox().Width + 25, 40));
			}
		}

		private void selectEffectTrigger(Button a_button)
		{
			if (m_selectedTriggerEffect != null)
			{
				m_selectedTriggerEffect.setState(0);
			}
			m_selectedTriggerEffect = a_button;
			m_selectedTriggerEffect.setState(3);
		}

		private void deleteEvent(Button a_button)
		{
			m_events.Remove(m_selectedEvent);
			deselectEvent();
		}

		private void deleteTriggerEffect(Button a_button)
		{
			throw new NotImplementedException();
			/*
			if (m_triggerButtons.Contains(a_button)) {
				//m_events[m_selectedEvent].remove(
			} else if (m_effectButtons.Contains(a_button)) {

			}
			*/
			//TODO
		}

		public void selectSwitchTrigger(Button a_button)
		{
			m_state = State.selectSwitch;
			if (m_switchTriggerType != null)
			{
				m_switchTriggerType.setState(0);
			}
			m_switchTriggerType = a_button;
			m_switchTriggerType.setState(3);
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

		private GameObject selectObject(Vector2 a_point)
		{
			GameObject t_return = null;

			foreach (GameObject t_gameObject in m_backState.getObjectList()[0])
			{
				if (!(t_gameObject is Consumable))
				{
					continue;
				}
				else
				{
					if (((Consumable)t_gameObject).getImageBox().contains(a_point))
					{
						if (t_return == null || t_return.getLayer() > t_gameObject.getLayer())
						{
							t_return = t_gameObject;
						}
					}
				}
			}
			return t_return;
		}
		#endregion
		
		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{
			updateMouse();
			updateKeyboard();
			updateGUI(a_gameTime);
			m_backState.updateCamera();
		}

		private void updateMouse()
		{
			Vector2 t_mouse = MouseHandler.worldMouse();
			//-----------------------------------
			#region Middle-mouse drag
			if (MouseHandler.mmbPressed())
			{
				Vector2 t_difference = Game.getInstance().m_camera.getPosition().getGlobalCartesian();
				t_difference.X = (Mouse.GetState().X - Game.getInstance().getResolution().X / 2) / 20 / Game.getInstance().m_camera.getZoom();
				t_difference.Y = (Mouse.GetState().Y - Game.getInstance().getResolution().Y / 2) / 20 / Game.getInstance().m_camera.getZoom();
				Game.getInstance().m_camera.getPosition().plusWith(t_difference);
			}
			#endregion
			//-----------------------------------
			
			//-----------------------------------
			#region Left Mouse Button Click Down
			if (MouseHandler.lmbDown())
			{
				if (m_state == State.firRectanglePoint)
				{
					m_recPoint = MouseHandler.worldMouse();
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
					Vector2 t_mousePoint = MouseHandler.worldMouse();
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
			#endregion
			//-----------------------------------
			
			//-----------------------------------
			#region Left Mouse Button Drag
			if (MouseHandler.lmbPressed())
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
			#endregion
			//-----------------------------------
			
			//-----------------------------------
			#region Left Mouse Button Release
			if (MouseHandler.lmbUp())
			{
				if (m_state == State.drawingRectangle)
				{
					m_events[m_selectedEvent].add(new PlayerIsWithinRectangle(m_recPoint.X, m_recPoint.Y, t_mouse.X, t_mouse.Y, Game.getInstance().m_camera.getLayer()));
					m_btnAddTrigger.setState(0);
					m_state = State.newTrigger;
					m_recLines = new Line[0];
					selectEvent(m_selectedEvent);
				}
				if (m_state == State.selectingObject)
				{
					GameObject t_object = null;
					if ((t_object = selectObject(MouseHandler.worldMouse())) != null)
					{
						m_events[m_selectedEvent].add(new IsDeadTrigger(t_object, true));
						m_btnAddTrigger.setState(Button.State.Normal);
						m_state = State.newTrigger;
						selectEvent(m_selectedEvent);
					}
				}
			}
			#endregion
			//-----------------------------------
			

			if (MouseHandler.rmbDown())
			{
				deselectEvent();
			}
		}

		private void updateKeyboard()
		{
			if (KeyboardHandler.keyClicked(Keys.Enter))
			{
				if (m_textField != null && m_textField.isWriting())
				{
					switch (m_state)
					{
						case State.newEquip:
							m_events[m_selectedEvent].add(new EquipEffect(m_textField.getText(), true));					
							break;
						case State.newCutscene:
							if (File.Exists(Game.CUTSCENE_FOLDER + m_textField.getText() + ".csn"))
							{
								m_events[m_selectedEvent].add(new CutsceneEffect(m_textField.getText() + ".csn"));
							}
							else
							{
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
					m_state = State.neutral;
				}
			}
		}

		private void updateGUI(GameTime a_gameTime)
		{
			if (m_selectedEvent != null)
			{
				foreach (Button t_button in m_triggerButtons)
				{
					t_button.update();
				}
				foreach (Button t_button in m_effectButtons)
				{
					t_button.update();
				}
				m_btnAddEffect.update();
				m_btnAddTrigger.update();
				m_deleteEvent.update();
			}
			else
			{
				foreach (Button t_button in m_events.Keys)
				{
					t_button.update();
				}
				m_btnAddEvent.update();
			}

			/*
			if (m_selectedTriggerEffect != null)
			{
				m_deleteTriggerEffect.setPosition(new Vector2(m_selectedTriggerEffect.getBox().X + m_selectedTriggerEffect.getBox().Width + 3, m_selectedTriggerEffect.getBox().Y));
				m_deleteTriggerEffect.update();
			}
			*/ //TODO

			if (m_btnAddEffect.getIntState() == 3)
			{
				foreach (Button t_button in m_effectMenu)
				{
					t_button.update();
				}
			}
			else if (m_btnAddTrigger.getIntState() == 3)
			{
				foreach (Button t_button in m_triggerMenu)
				{
					t_button.update();
				}
			}

			if (m_textField != null)
			{
				m_textField.update(a_gameTime);
			}
			m_exitEvent.update();
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			m_backState.draw(a_gameTime, a_spriteBatch);
			m_background.draw(a_gameTime);

			if (m_selectedEvent != null)
			{
				foreach (Button t_button in m_triggerButtons)
				{
					t_button.draw(a_gameTime, a_spriteBatch);
				}
				foreach (Button t_button in m_effectButtons)
				{
					t_button.draw(a_gameTime, a_spriteBatch);
				}
				m_btnAddEffect.draw(a_gameTime, a_spriteBatch);
				m_btnAddTrigger.draw(a_gameTime, a_spriteBatch);
				m_selectedEvent.draw(a_gameTime, a_spriteBatch);
				m_deleteEvent.draw(a_gameTime, a_spriteBatch);
			}
			else
			{
				foreach (Button t_button in m_events.Keys)
				{
					t_button.draw(a_gameTime, a_spriteBatch);
				}
				m_btnAddEvent.draw(a_gameTime, a_spriteBatch);
			}

			/*
			if (m_selectedTriggerEffect != null)
			{
				m_deleteTriggerEffect.draw(a_gameTime, a_spriteBatch);
			}
			*/ //TODO

			if (m_btnAddEffect.getIntState() == 3)
			{
				foreach (Button t_button in m_effectMenu)
				{
					t_button.draw(a_gameTime, a_spriteBatch);
				}
			}
			else if (m_btnAddTrigger.getIntState() == 3)
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

			foreach (Line t_line in m_recLines)
			{
				if (t_line != null)
				{
					t_line.draw();
				}
			}

			if (m_textField != null)
			{
				m_textFieldInfo.draw(a_gameTime);
				m_textField.draw(a_gameTime);
			}
			m_exitEvent.draw(a_gameTime, a_spriteBatch);
		}
		#endregion
	}
}