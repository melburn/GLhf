using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

			String[] t_loadedFile = System.IO.File.ReadAllLines("Content//wtf//loadImages.txt");

			foreach (String f_currentLine in t_loadedFile)
			{
				String[] t_info = f_currentLine.Split(t_splitter);
				try
				{
					m_animationFrames.Add(t_info[0], int.Parse(t_info[1]));
				}
				catch(System.FormatException)
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
	}
}
