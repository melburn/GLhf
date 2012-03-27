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
		private int m_timeForNextCommand;
		private bool m_waiting;
		private String[] m_currentCommand;
		private Dictionary<int, GuiObject> m_guis;

		public Cutscene(States a_backState, String a_sceneToLoad)
		{
			m_backState = a_backState;
			m_comDone = 0;
			m_filePath = a_sceneToLoad;
			m_waiting = false;
			m_guis = new Dictionary<int, GuiObject>();
		}
		public override void update(GameTime a_gameTime)
		{
			if (m_waiting)
			{
				if (m_currentCommand[0].Equals("waitUntil", StringComparison.OrdinalIgnoreCase))
				{
					if (m_timeForNextCommand >= a_gameTime.TotalGameTime.Milliseconds)
					{
						m_waiting = false;
						++m_comDone;
					}
				}
				else if (Game.isKeyPressed(parseKey(m_currentCommand[1])))
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
					Game.getInstance().setState(m_backState);
				}
			}
		}

		private Keys parseKey(string a_keyname)
		{
			return (Keys)Enum.Parse(typeof(Keys), a_keyname, false);
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
				m_timeForNextCommand = a_gameTime.TotalGameTime.Milliseconds + int.Parse(m_commands[1]);
				return false;
			}
			else if (m_currentCommand[0].Equals("addGUI", StringComparison.OrdinalIgnoreCase))
			{
				if (m_currentCommand.Length < 5)
				{
					throw new ParseException();
				}
				m_guis.Add(int.Parse(m_currentCommand[1]),new GuiObject(new Vector2(float.Parse(m_currentCommand[2]),float.Parse(m_currentCommand[3])),m_currentCommand[4]));
			}
			else if (m_currentCommand[0].Equals("removeGui", StringComparison.OrdinalIgnoreCase))
			{
				if (m_currentCommand.Length == 1)
				{
					throw new ParseException();
				}
				m_guis.Remove(int.Parse(m_commands[1]));
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
			else
			{
				throw new ParseException();
			}
			return true;
		}
		public override void load()
		{
			m_commands = Loader.getInstance().readFromFile(m_filePath);
			base.load();
		}
		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			m_backState.draw(a_gameTime, a_spriteBatch);
			foreach (GuiObject t_go in m_guis.Values)
			{
				t_go.draw(a_gameTime);
			}
		}
	}
}
