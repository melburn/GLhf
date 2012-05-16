using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class Loader
	{
		static private Loader s_instance;

		private Dictionary<String, int> m_animationFrames;

		private Loader()
		{
			loadAnimationFrames();	
		}

		static public Loader getInstance()
		{
			if (s_instance == null)
			{
				s_instance = new Loader();
			}
			return s_instance;
		}

		private void loadAnimationFrames()
		{
			char[] t_splitter = { ':' };

			m_animationFrames = new Dictionary<string, int>();

			String[] t_loadedFile = File.ReadAllLines("Content//wtf//loadImages.txt");

			foreach (String f_currentLine in t_loadedFile)
			{
				String[] t_info = f_currentLine.Split(t_splitter);
				try
				{
					m_animationFrames.Add(t_info[0], int.Parse(t_info[1]));
				}
				catch (System.FormatException)
				{
					ErrorLogger.getInstance().writeString("Parse fail in loading of animationSizes at line : " + f_currentLine);
				}
				catch (System.IndexOutOfRangeException)
				{
					ErrorLogger.getInstance().writeString("Parse fail (missing colon) in loading of animationSizes at line : " + f_currentLine);
				}
			}
		}

		internal int getAnimationFrames(string a_sprite)
		{
			if (a_sprite == null || !m_animationFrames.ContainsKey(a_sprite))
			{
				return 1;
			}
			return m_animationFrames[a_sprite];
		}

		public Level loadLevel(string a_levelName)
		{
			return Serializer.getInstance().loadLevel(Serializer.getInstance().getFileToStream(a_levelName, false));
		}

		public Level loadCheckPoint()
		{
			return Serializer.getInstance().loadLevel(Game.getInstance().getCheckPointLevel(false));
		}


		public String[] readFromFile(string a_filePath)
		{
			return System.IO.File.ReadAllLines(a_filePath);
		}

		public void loadGraphicSettings(string a_file)
		{
			string[] t_loadedFile = File.ReadAllLines(a_file);
			bool t_startParse = false;

			foreach (string t_currentLine in t_loadedFile)
			{
				if (t_currentLine.Length > 2 && t_currentLine.First() == '[' && t_currentLine.Last() == ']')
				{
					if (t_currentLine.Equals("[Graphics]"))
					{
						t_startParse = true;
						continue;
					}
				}
				if (!t_startParse)
				{
					continue;
				}
				string[] t_setting = t_currentLine.Split('=');
				if (t_setting[0].Equals("ScreenWidth"))
				{
					Game.getInstance().m_graphics.PreferredBackBufferWidth = int.Parse(t_setting[1]);
				}
				else if (t_setting[0].Equals("ScreenHeight"))
				{
					Game.getInstance().m_graphics.PreferredBackBufferHeight = int.Parse(t_setting[1]);
					Game.getInstance().m_camera.setZoom(Game.getInstance().getResolution().Y / 720);
				}
				else if (t_setting[0].Equals("Fullscreen"))
				{
					Game.getInstance().m_graphics.IsFullScreen = bool.Parse(t_setting[1]);
				}
				else if (t_setting[0].Equals("Antialias"))
				{
					Game.getInstance().m_graphics.PreferMultiSampling = bool.Parse(t_setting[1]);
				}
				else if (t_setting[0].Equals("VSync"))
				{
					Game.getInstance().m_graphics.SynchronizeWithVerticalRetrace = bool.Parse(t_setting[1]);
				}
				else if (t_setting[0].StartsWith("["))
				{
					break;
				}
				else
				{
					//ErrorLogger.getInstance().writeString("Found unknown setting while loading GameState" + t_setting[0]);
				}
			}
			Game.getInstance().m_graphics.ApplyChanges();
		}

		public string[] getSettingsBlock(string a_block, string a_file)
		{
			LinkedList<string> t_returnValue = new LinkedList<string>();
			bool t_startParse = false;

			foreach (string t_setting in File.ReadAllLines(a_file))
			{
				if (!t_startParse && t_setting.Equals("[" + a_block + "]"))
				{
					t_startParse = true;
					continue;
				}
				if (!t_setting.StartsWith("["))
				{
					if (!t_setting.Equals(""))
					{
						t_returnValue.AddLast(t_setting);
					}
				}
				else
				{
					break;
				}
			}

			return t_returnValue.ToArray();
		}
	}
}
