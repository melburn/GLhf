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
		private LinkedList<GameObject> m_gameObjectList;
		//private LinkedList<GameObject> m_killList = new LinkedList<GameObject>();
		//private LinkedList<GameObject> m_addList = new LinkedList<GameObject>();
		private LinkedList<GameObject> m_changeList = new LinkedList<GameObject>();
		MouseState m_previousMouse;
		MouseState m_currentMouse;
		KeyboardState m_previous;
		KeyboardState m_current;
		private string m_currentLevel;

		private Player player;

		public GameState()
		{
			m_currentLevel = "Level3.txt";
		}

		public GameState(string a_levelToLoad)
		{
			m_currentLevel = a_levelToLoad;
		}

		public override void load()
		{
			m_gameObjectList = Loader.getInstance().loadLevel(m_currentLevel);
			if (player != null)
			{
				Game.getInstance().m_camera.setParentPosition(player.getPosition());
			}
		}

		public override void setPlayer(Player a_player)
		{
			player = a_player;
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
			m_currentMouse = Mouse.GetState();

			foreach (GameObject t_gameObject in m_gameObjectList)
			{
				t_gameObject.update(a_gameTime);
			}

			if (m_current.IsKeyDown(Keys.Q))
			{
				Game.getInstance().setState(new DevelopmentState(m_currentLevel));
			}

			if (m_current.IsKeyDown(Keys.R))
			{
				Game.getInstance().setState(new GameState());
			}

			foreach (GameObject t_firstGameObject in m_gameObjectList)
			{
				if (t_firstGameObject is MovingObject)
				{

					List<Entity> t_collided = new List<Entity>();

					if (t_firstGameObject is Entity)
					{
						foreach (GameObject t_secondGameObject in m_gameObjectList)
						{
							if (t_secondGameObject is Entity && t_firstGameObject != t_secondGameObject
								&& checkBigBoxCollision((Entity)t_firstGameObject, (Entity)t_secondGameObject))
							{
								t_collided.Add((Entity)t_secondGameObject);
							}
						}
						t_firstGameObject.collisionCheck(t_collided);
					}
				}

				if (t_firstGameObject.isDead())
				{
					m_changeList.AddLast(t_firstGameObject);
				}
			}
			m_gameObjectList.Except(m_changeList);
			m_changeList.Clear();
			m_previousMouse = m_currentMouse;
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

		public static bool checkBigBoxCollision(Entity a_first, Entity a_second)
		{
			return (a_first.getPosition().getGlobalX() - 1 < a_second.getPosition().getGlobalX() + a_second.getHitBox().getOutBox().Width &&
				a_first.getPosition().getGlobalX() + a_first.getHitBox().getOutBox().Width + 1 > a_second.getPosition().getGlobalX()) &&
				(a_first.getPosition().getGlobalY() < a_second.getPosition().getGlobalY() + a_second.getHitBox().getOutBox().Height &&
				a_first.getPosition().getGlobalY() + a_first.getHitBox().getOutBox().Height > a_second.getPosition().getGlobalY());
		}
		public override void addObject(GameObject a_object)
		{
			m_changeList.AddLast(a_object);
		}
		public override Player getPlayer()
		{
			return player;
		}
	}
}
