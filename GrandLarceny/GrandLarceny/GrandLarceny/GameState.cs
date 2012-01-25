using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	class GameState : States
	{
		private LinkedList<GameObject> m_gameObjectList = new LinkedList<GameObject>();
		private LinkedList<GameObject> m_killList = new LinkedList<GameObject>();
		KeyboardState m_previous;
		KeyboardState m_current;

		public GameState() 
		{
          
		}
		/*
		Update-metod, går igenom alla objekt i scenen och kallas på deras update
		och kollar sedan om de ska dö och läggs därefter i dödslistan.
		Dödslistan loopas sedan igenom och tar bort de objekt som ska dö ifrån
		objektlistan.
		*/
		public override void update(GameTime a_gameTime)
		{
			m_current = Keyboard.GetState();


			foreach (GameObject t_gameObject in m_gameObjectList)
			{
				t_gameObject.update(a_gameTime);
				if (t_gameObject.isDead())
				{
					m_killList.AddLast(t_gameObject);
				}
			}
			foreach (GameObject t_gameObject in m_killList)
			{
				m_gameObjectList.Remove(t_gameObject);
			}
			m_previous = m_current;
		}
		/*
		Draw-metod, loopar igenom alla objekt och ber dem ritas ut på skärmen 
		*/
		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (GameObject t_gameObject in m_gameObjectList)
			{
				t_gameObject.draw(a_gameTime);
			}
		}
	}
}
