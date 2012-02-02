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
                    Console.Out.WriteLine("Parse fail : "+ f_currentLine);
                }
                catch (System.IndexOutOfRangeException)
                {
                    Console.Out.WriteLine("det finns inga colon : " + f_currentLine);
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

		public LinkedList<GameObject> loadLevel(int a_levelToLoad)
		{
			char[] t_splitter = { ':' };
			LinkedList<GameObject> t_loadedList = new LinkedList<GameObject>();
			String[] t_loadedFile = System.IO.File.ReadAllLines("Content//Levels//Level" + a_levelToLoad + ".txt");

			foreach (String t_currentLine in t_loadedFile) 
			{
				String[] t_info = t_currentLine.Split(t_splitter);
				try
				{
					if (t_info[0].Equals("Environment"))
					{
						t_loadedList.AddLast(new Environment(new Vector2(int.Parse(t_info[1]), int.Parse(t_info[2])), t_info[3]));
						t_loadedList.Last().setLayer(1);
					}
					if (t_info[0].Equals("Platform")) {
						t_loadedList.AddLast(new Platform(new Vector2(int.Parse(t_info[1]), int.Parse(t_info[2])), t_info[3]));
					}
				} 
				catch (System.FormatException fe)
				{
					Console.Out.WriteLine(fe.Message);
				}
				catch (System.IndexOutOfRangeException ioore)
				{
					Console.Out.WriteLine(ioore.Message);
				}
			}
			return t_loadedList;
		}
	}
}
