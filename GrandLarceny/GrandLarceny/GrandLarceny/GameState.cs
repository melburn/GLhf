using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	class GameState : States
	{
		private LinkedList<GameObject> m_gameObjectList = new LinkedList<GameObject>();
		private LinkedList<GameObject> m_killList = new LinkedList<GameObject>();
		private static GameState m_myState;

		/*
		Singleton kontruktor
		*/

		public GameState() 
		{

		}

		public override void update(GameTime a_gameTime)
		{
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
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (GameObject t_gameObject in m_gameObjectList)
			{
				t_gameObject.draw(a_gameTime);
			}
		}

		public void addGameObject(GameObject a_gameObject)
		{
			m_gameObjectList.AddLast(a_gameObject);
		}
	}
}
