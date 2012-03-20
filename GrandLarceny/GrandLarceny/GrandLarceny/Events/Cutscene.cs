using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny.Events
{
	class Cutscene : States
	{
		private String m_filePath;
		private States m_backState;
		private String[] m_commands;
		private int m_comDone;
		private TimeSpan m_timeForNextCommand;
		private bool m_waitingForKey;
		private String[] m_currentCommand;
		public Cutscene(States a_backState, String a_sceneToLoad)
		{
			m_backState = a_backState;
			m_comDone = 0;
			m_filePath = a_sceneToLoad;
			m_waitingForKey = false;
		}
		public override void update(GameTime a_gameTime)
		{
			if (m_waitingForKey)
			{
				if (Game.getInstance().getCurrentKeyboard().IsKeyDown(parseKey(m_commands[1])))
				{

				}
			}
			if (m_timeForNextCommand == null || m_timeForNextCommand >= a_gameTime.TotalGameTime)
			{
				while (!parseAndExecute())
				{

				}
			}
		}

		private Keys parseKey(string a_keyname)
		{
			return (Keys)Enum.Parse(typeof(Keys), a_keyname, false);
		}

		private bool parseAndExecute()
		{
			m_currentCommand = m_commands[m_comDone].Split(':');
			if (m_currentCommand[0].Equals("waitforkey", StringComparison.OrdinalIgnoreCase))
			{
				if (m_commands.Length == 1)
				{
					throw new ParseException();
				}
				m_waitingForKey = true;
				return false;
			}
			return true;
		}
		public override void load()
		{
			m_commands = Loader.getInstance().readFromFile(m_filePath);
		}
		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			m_backState.draw(a_gameTime, a_spriteBatch);
		}
	}
}
