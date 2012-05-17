using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	class Cutscene : States
	{
		private String m_filePath;
		private States m_backState;
		private String[] m_commands;
		private int m_comDone;
		private float m_timeForNextCommand;
		private bool m_waiting;
		private String[] m_currentCommand;
		private Dictionary<int, GuiObject> m_guis;
		private LinkedList<GameObject> m_objects;
		private Stack<GameObject> m_deleteList;

		private float m_timeStart;
		private Vector2 m_cameraMoveTo;
		private Position m_oldCamPar;

		public Cutscene(States a_backState, String a_sceneToLoad)
		{
			m_backState = a_backState;
			m_comDone = 0;
			m_filePath = a_sceneToLoad;
			m_waiting = false;
			m_guis = new Dictionary<int, GuiObject>();
			m_objects = new LinkedList<GameObject>();
			m_deleteList = new Stack<GameObject>();
		}

		public Cutscene(States a_backState, String[] a_commands)
		{
			m_backState = a_backState;
			m_comDone = 0;
			m_commands = a_commands;
			m_waiting = false;
			m_guis = new Dictionary<int, GuiObject>();
			m_objects = new LinkedList<GameObject>();
			m_deleteList = new Stack<GameObject>();
		}

		public override void update(GameTime a_gameTime)
		{
			foreach(GameObject t_gb in m_objects){
				t_gb.update(a_gameTime);
				if (t_gb.isDead())
				{
					m_deleteList.Push(t_gb);
				}
			}

			while (m_deleteList.Count > 0)
			{
				m_objects.Remove(m_deleteList.Pop());
			}

			if (m_waiting)
			{
				if (m_currentCommand[0].Equals("waitUntil", StringComparison.OrdinalIgnoreCase))
				{
					if (m_timeForNextCommand <= a_gameTime.TotalGameTime.TotalMilliseconds)
					{
						m_waiting = false;
						++m_comDone;
					}
				}
				else if (m_currentCommand[0].Equals("setCamera", StringComparison.OrdinalIgnoreCase))
				{
					float t_moveDeltaTime = ( (float)a_gameTime.TotalGameTime.TotalMilliseconds - m_timeStart) / (m_timeForNextCommand - m_timeStart);
					Position t_camera = Game.getInstance().m_camera.getPosition();
					t_camera.setGlobalCartesian(Vector2.SmoothStep(t_camera.getGlobalCartesian(), m_cameraMoveTo, t_moveDeltaTime));
					if (m_timeForNextCommand <= a_gameTime.TotalGameTime.TotalMilliseconds)
					{
						m_waiting = false;
						++m_comDone;
					}
				}
				else if (KeyboardHandler.isKeyPressed(parseKey(m_currentCommand[1])))
				{
					m_waiting = false;
					++m_comDone;
				}
			}
			else
			{
				while (m_comDone < m_commands.Length && parseAndExecute(a_gameTime))
				{
					++m_comDone;
				}
				if (m_comDone >= m_commands.Length)
				{
					Game.getInstance().m_camera.getPosition().setParentPositionWithoutMoving(m_oldCamPar);
					Game.getInstance().setState(m_backState);
				}
			}
		}

		private Keys parseKey(string a_keyname)
		{
			if(a_keyname.Equals("bindedUp"))
			{
				return GameState.getUpKey();
			}
			else if (a_keyname.Equals("bindedLeft"))
			{
				return GameState.getLeftKey();
			}
			else if (a_keyname.Equals("bindedRight"))
			{
				return GameState.getRightKey();
			}
			else if (a_keyname.Equals("bindedDown"))
			{
				return GameState.getDownKey();
			}
			else if (a_keyname.Equals("jump"))
			{
				return GameState.getJumpKey();
			}
			else if (a_keyname.Equals("roll"))
			{
				return GameState.getRollKey();
			}
			else if (a_keyname.Equals("action"))
			{
				return GameState.getActionKey();
			}
			else if (a_keyname.Equals("sprint"))
			{
				return GameState.getSprintKey();
			}
			else
			{
				return (Keys)Enum.Parse(typeof(Keys), a_keyname, false);
			}
		}

		private bool parseAndExecute(GameTime a_gameTime)
		{
			char[] t_delimiterChars = {':'};
			m_currentCommand = m_commands[m_comDone].Split(t_delimiterChars);
			if (m_currentCommand[0].Equals("waitForKey", StringComparison.OrdinalIgnoreCase))
			{
				if (m_currentCommand.Length == 1)
				{
					throw new ParseException();
				}
				m_waiting = true;
				return false;
			}
			else if (m_currentCommand[0].Equals("waitUntil", StringComparison.OrdinalIgnoreCase))
			{
				if (m_currentCommand.Length == 1)
				{
					throw new ParseException();
				}
				m_timeForNextCommand = (float)a_gameTime.TotalGameTime.TotalMilliseconds + float.Parse(m_commands[1]);
				return false;
			}
			else if (m_currentCommand[0].Equals("addGUI", StringComparison.OrdinalIgnoreCase))
			{
				if (m_currentCommand.Length < 5)
				{
					throw new ParseException();
				}
				m_guis.Add(int.Parse(m_currentCommand[1]), new GuiObject(new Vector2(float.Parse(m_currentCommand[2]), float.Parse(m_currentCommand[3])), m_currentCommand[4]));
			}
			else if (m_currentCommand[0].Equals("removeGui", StringComparison.OrdinalIgnoreCase))
			{
				if (m_currentCommand.Length == 1)
				{
					throw new ParseException();
				}
				m_guis.Remove(int.Parse(m_commands[1]));
			}
			else if (m_currentCommand[0].Equals("setLayer", StringComparison.OrdinalIgnoreCase))
			{
				if (m_currentCommand.Length < 2)
				{
					throw new ParseException();
				}
				m_guis[int.Parse(m_currentCommand[1])].setLayer(float.Parse(m_currentCommand[2]));
			}
			else if (m_currentCommand[0].Equals("addText", StringComparison.OrdinalIgnoreCase))
			{
				if (m_currentCommand.Length < 10)
				{
					throw new ParseException();
				}
				m_guis.Add(int.Parse(m_currentCommand[1]), new Text(new Vector2(float.Parse(m_currentCommand[2]), float.Parse(m_currentCommand[3])), m_currentCommand[4],
					m_currentCommand[5], new Color(float.Parse(m_currentCommand[6]),float.Parse(m_currentCommand[7]),float.Parse(m_currentCommand[8]),float.Parse(m_currentCommand[8])),false)); 
			}
			else if (m_currentCommand[0].Equals("removeText", StringComparison.OrdinalIgnoreCase))
			{
				if (m_currentCommand.Length == 1)
				{
					throw new ParseException();
				}
				m_guis.Remove(int.Parse(m_currentCommand[1]));
			}
			else if (m_currentCommand[0].Equals("sound", StringComparison.OrdinalIgnoreCase))
			{
				if (m_currentCommand.Length == 1)
				{
					throw new ParseException();
				}
				new Sound(m_currentCommand[1]).play();
			}
			else if (m_currentCommand[0].Equals("setCamera", StringComparison.OrdinalIgnoreCase))
			{
				if (m_currentCommand.Length == 1)
				{
					throw new ParseException();
				}
				m_waiting = true;
				m_cameraMoveTo = new Vector2(float.Parse(m_currentCommand[1]), float.Parse(m_currentCommand[2]));
				m_timeForNextCommand = (float)a_gameTime.TotalGameTime.TotalMilliseconds + float.Parse(m_currentCommand[3]);
				m_timeStart = (float)a_gameTime.TotalGameTime.TotalMilliseconds;
				return false;
			}
			else if (m_currentCommand[0].Equals("addParticle", StringComparison.OrdinalIgnoreCase))
			{
				if (m_currentCommand.Length == 1)
				{
					throw new ParseException();
				}
				m_objects.AddLast(new Particle(new Vector2(float.Parse(m_currentCommand[1]),float.Parse(m_currentCommand[2])),m_currentCommand[3],float.Parse(m_currentCommand[4]),float.Parse(m_currentCommand[5])));
			}
			else if (m_currentCommand[0].Equals("addCinematic", StringComparison.OrdinalIgnoreCase))
			{

				int t_BoxWidth = Game.getInstance().m_graphics.PreferredBackBufferWidth * 2;
				int t_BoxHeight = Game.getInstance().m_graphics.PreferredBackBufferHeight / 5;
				int t_windowHeight = Game.getInstance().m_graphics.PreferredBackBufferHeight;
				Box tBoxTop = new Box(new Vector2(0, 0),t_BoxWidth ,t_BoxHeight , Color.Black, false);
				m_objects.AddLast(tBoxTop);
				tBoxTop.setMove(new Vector2(-t_BoxWidth / 2, -t_windowHeight / 2 - t_BoxHeight), new Vector2(-t_BoxWidth / 2, -t_windowHeight/2), a_gameTime, 0.1f);
				Box tBoxBottom = new Box(new Vector2(0, Game.getInstance().m_graphics.PreferredBackBufferHeight - t_BoxHeight), t_BoxWidth, t_BoxHeight, Color.Black, false);
				m_objects.AddLast(tBoxBottom);
				tBoxBottom.setMove(new Vector2(-t_BoxWidth / 2, t_windowHeight / 2 ), new Vector2(-t_BoxWidth / 2, t_windowHeight/2 - t_BoxHeight), a_gameTime, 0.1f);
				
			}
			else
			{
				throw new ParseException();
			}
			return true;
		}
		public override void load()
		{
			if (m_filePath != null)
			{
				m_commands = Loader.getInstance().readFromFile(m_filePath);
			}
			m_oldCamPar = Game.getInstance().m_camera.getPosition().getParentPosition();
			Game.getInstance().m_camera.getPosition().setParentPositionWithoutMoving(null);
			base.load();
		}
		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			m_backState.draw(a_gameTime, a_spriteBatch);
			foreach (GameObject t_gb in m_objects)
			{
				t_gb.draw(a_gameTime);
			}
			foreach (GuiObject t_go in m_guis.Values)
			{
				t_go.draw(a_gameTime);
			}
		}
	}
}
