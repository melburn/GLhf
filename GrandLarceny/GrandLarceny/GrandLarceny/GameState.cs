using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GrandLarceny.Events;
using GrandLarceny.AI;

namespace GrandLarceny
{
	public class GameState : States
	{
		private LinkedList<Environment> m_unexplored;

		private Stack<GameObject>[] m_addList;
		private LinkedList<Event> m_events;
		private string m_currentLevel;
		private bool m_loadCheckpoint;
		private int m_currentList;
		private static Keys m_upKey;
		private static Keys m_downKey;
		private static Keys m_leftKey;
		private static Keys m_rightKey;
		private static Keys m_jumpKey;
		private static Keys m_rollKey;
		private static Keys m_actionKey;
		private static Keys m_sprintKey;

		private Texture2D m_background;

		private LinkedList<ConsumableGoal> m_finishCond;

		private Player player;

		private ParseState m_currentParse;
		private enum ParseState
		{
			Settings,
			Input
		}

		public GameState(string a_levelToLoad)
		{
			m_currentLevel = a_levelToLoad;
		}

		public GameState(string a_levelToLoad, bool a_checkpoint)
		{
			m_currentLevel = a_levelToLoad;
			m_loadCheckpoint = true;
		}

		public override void load()
		{
			Game.getInstance().m_camera.setZoom(1.0f);
			if (m_loadCheckpoint)
			{
				Level t_loadedLevel = Loader.getInstance().loadCheckPoint();

				m_gameObjectList = t_loadedLevel.getGameObjects();
				m_events = t_loadedLevel.getEvents();
			}
			else if (File.Exists("Content\\levels\\" + m_currentLevel))
			{
				Level t_loadedLevel = Loader.getInstance().loadLevel(m_currentLevel);

				m_gameObjectList = t_loadedLevel.getGameObjects();
				m_events = t_loadedLevel.getEvents();
			}
			else
			{
				m_events = new LinkedList<Event>();
				m_gameObjectList = new LinkedList<GameObject>[5];
				for (int i = 0; i < m_gameObjectList.Length; ++i)
				{
					m_gameObjectList[i] = new LinkedList<GameObject>();
				}
			}
			//m_removeList = new Stack<GameObject>[m_gameObjectList.Length];
			m_addList = new Stack<GameObject>[m_gameObjectList.Length];

			Loader.getInstance().loadGraphicSettings("Content//wtf//settings.ini");

			string[] t_loadedFile = System.IO.File.ReadAllLines("Content//wtf//settings.ini");
			foreach (string t_currentLine in t_loadedFile)
			{
				if (t_currentLine.Length > 2 && t_currentLine.First() == '[' && t_currentLine.Last() == ']')
				{
					if (t_currentLine.Equals("[Input]"))
					{
						m_currentParse = ParseState.Input;
						continue;
					}
				}

				if (m_currentParse == ParseState.Input)
				{
					string[] t_input = t_currentLine.Split('=');
					if (t_input[0].Equals("Up"))
					{
						m_upKey = (Keys)Enum.Parse(typeof(Keys), t_input[1]);
					}
					else if (t_input[0].Equals("Down"))
					{
						m_downKey = (Keys)Enum.Parse(typeof(Keys), t_input[1]);
					}
					else if (t_input[0].Equals("Left"))
					{
						m_leftKey = (Keys)Enum.Parse(typeof(Keys), t_input[1]);
					}
					else if (t_input[0].Equals("Right"))
					{
						m_rightKey = (Keys)Enum.Parse(typeof(Keys), t_input[1]);
					}
					else if (t_input[0].Equals("Jump"))
					{
						m_jumpKey = (Keys)Enum.Parse(typeof(Keys), t_input[1]);
					}
					else if (t_input[0].Equals("Roll"))
					{
						m_rollKey = (Keys)Enum.Parse(typeof(Keys), t_input[1]);
					}
					else if (t_input[0].Equals("Action"))
					{
						m_actionKey = (Keys)Enum.Parse(typeof(Keys), t_input[1]);
					}
					else if (t_input[0].Equals("Sprint"))
					{
						m_sprintKey = (Keys)Enum.Parse(typeof(Keys), t_input[1]);
					}
					else if (t_input[0].StartsWith("["))
					{
						break;
					}
					else
					{
						ErrorLogger.getInstance().writeString("Found unknown keybinding while loading GameState" + t_input[0]);
					}
				}
			}
			Game.getInstance().m_graphics.ApplyChanges();

			for (int i = 0; i < m_gameObjectList.Length; ++i)
			{
				m_addList[i] = new Stack<GameObject>();
			}
			
			m_unexplored = new LinkedList<Environment>();
			m_finishCond = new LinkedList<ConsumableGoal>();

			for (int i = 0; i < m_gameObjectList.Count(); ++i)
			{
				foreach (GameObject t_go in m_gameObjectList[i])
				{
					t_go.loadContent();
					t_go.setListLayer(i);

					if (t_go is Player)
					{
						setPlayer((Player)t_go);
					}
					else if (t_go is ConsumableGoal)
					{
						m_finishCond.AddLast((ConsumableGoal)t_go);
					}
					else if (t_go is Environment && !((Environment)t_go).isExplored())
					{
						m_unexplored.AddLast((Environment)t_go);
					}
				}
			}

			foreach (Event t_e in m_events)
			{
				t_e.loadContent();
			}

			if (player != null)
			{
				Game.getInstance().m_camera.setPosition(Vector2.Zero);
				Game.getInstance().m_camera.setParentPosition(player.getPosition());
			}

			m_background = Game.getInstance().Content.Load<Texture2D>("Images//Background//starry_sky_01");

			base.load();
			addObject(new Darkness(Vector2.Zero, "Images//LightCone//ventilljus", 0.003f), 1);
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
			m_currentList = -1;

			if (KeyboardHandler.keyClicked(Keys.I))
			{
				Game.getInstance().m_camera.printInfo();
			}
			else if (KeyboardHandler.isKeyPressed(Keys.LeftControl))
			{
				if (KeyboardHandler.keyClicked(Keys.E))
				{
					if (!Game.getInstance().getProgress().hasEquipment("hookshot"))
						Game.getInstance().getProgress().setEquipment("hookshot", true);
					else
						Game.getInstance().getProgress().setEquipment("hookshot", false);
				}
				else if (KeyboardHandler.keyClicked(Keys.W))
				{
					if (!Game.getInstance().getProgress().hasEquipment("boots"))
						Game.getInstance().getProgress().setEquipment("boots", true);
					else
						Game.getInstance().getProgress().setEquipment("boots", false);
				}
			}
			else if (KeyboardHandler.keyClicked(Keys.Q))
			{
				Game.getInstance().setState(new DevelopmentState(m_currentLevel));
			}
			else if (KeyboardHandler.keyClicked(Keys.F5))
			{
				Game.getInstance().setState(new GameState(m_currentLevel));
				Game.getInstance().m_camera.setLayer(0);
			}
			else if (KeyboardHandler.keyClicked(Keys.M))
			{
				Game.getInstance().setState(new MapState(this));
			}

			foreach (LinkedList<GameObject> t_list in m_gameObjectList)
			{
				++m_currentList;
				foreach (GameObject t_gameObject in t_list)
				{
					try
					{
						t_gameObject.update(a_gameTime);
					}
					catch (Exception e)
					{
						ErrorLogger.getInstance().writeString("While updating " + t_gameObject + " got exception: " + e);
					}
				}
			}

			m_currentList = -1;
			foreach (LinkedList<GameObject> t_list in m_gameObjectList)
			{
				++m_currentList;
				foreach (GameObject t_firstGameObject in t_list)
				{
					if (t_firstGameObject is MovingObject && (Game.getInstance().m_camera.isInCamera(t_firstGameObject) ||
						(t_firstGameObject is NPE && (((NPE)t_firstGameObject).getAIState() is AIStateChasing || ((NPE)t_firstGameObject).getAIState() is AIStateChargeing))))
					{
						List<Entity> t_collided = new List<Entity>();
						foreach (GameObject t_secondGameObject in t_list)
						{
							if (t_secondGameObject is Entity && t_firstGameObject != t_secondGameObject
								&& ((Entity)t_firstGameObject).getHitBox() != null && ((Entity)t_secondGameObject).getHitBox() != null
								&& checkBigBoxCollision(((Entity)t_firstGameObject).getHitBox().getOutBox(), ((Entity)t_secondGameObject).getHitBox().getOutBox()))
							{
								t_collided.Add((Entity)t_secondGameObject);
							}
						}
						((MovingObject)t_firstGameObject).collisionCheck(t_collided);
						((Entity)t_firstGameObject).updatePosition();
					}
					else if (t_firstGameObject is Entity)
					{
						((Entity)t_firstGameObject).setGravity(0.0f);
						((Entity)t_firstGameObject).setSpeedY(0.0f);
					}

					if (t_firstGameObject.isDead() && !m_removeList[m_currentList].Contains(t_firstGameObject))
					{
						m_removeList[m_currentList].Push(t_firstGameObject);
					}
				}
				while (m_addList[m_currentList].Count > 0)
				{
					GameObject t_goToAdd = m_addList[m_currentList].Pop();
					if (!t_list.Contains(t_goToAdd))
					{
						t_goToAdd.setListLayer(m_currentList);
						t_list.AddLast(t_goToAdd);
					}
				}

				while (m_removeList[m_currentList].Count > 0)
				{
					GameObject t_objectToRemove = m_removeList[m_currentList].Pop();
					if (t_objectToRemove is ConsumableGoal)
					{
						m_finishCond.Remove((ConsumableGoal)t_objectToRemove);
						if (m_finishCond.Count == 0)
						{
							finishLevel();
							return;
						}
					}
					t_list.Remove(t_objectToRemove);
				}

				LinkedListNode<Event> t_eventNode = m_events.First;
				while (t_eventNode != null)
				{
					LinkedListNode<Event> t_next = t_eventNode.Next;
					try
					{
						if (t_eventNode.Value.Execute())
						{
							m_events.Remove(t_eventNode);
						}
					}
					catch (Exception e)
					{
						ErrorLogger.getInstance().writeString("While updating " + t_eventNode.Value + " got exception: " + e);
					}
					t_eventNode = t_next;
				}

				if (player != null)
				{
					LinkedListNode<Environment> t_enviroNode = m_unexplored.First;
					while (t_enviroNode != null)
					{
						LinkedListNode<Environment> t_next = t_enviroNode.Next;
						if (t_enviroNode.Value.collidesWith(player) && player.getListLayer() == t_enviroNode.Value.getListLayer())
						{
							t_enviroNode.Value.setExplored(true);
							m_unexplored.Remove(t_enviroNode);
						}
						t_enviroNode = t_next;
					}
				}
			}
		}

		private void finishLevel()
		{
			Game.getInstance().getProgress().setLevelCleared(m_currentLevel.Remove(m_currentLevel.Length - 4));
			Game.getInstance().setState(new HubMenu());
		}
		/*
		Draw-metod, loopar igenom alla objekt och ber dem ritas ut på skärmen 
		*/
		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			getPlayer().draw(a_gameTime);
			foreach (GameObject t_gameObject in m_gameObjectList[Game.getInstance().m_camera.getLayer()])
			{
				try
				{
					t_gameObject.draw(a_gameTime);
				}
				catch (Exception e)
				{
					ErrorLogger.getInstance().writeString("While drawing " + t_gameObject + " got exception: " + e);
				}
			}
			foreach (GuiObject t_go in m_guiList)
			{
				if (!t_go.isDead())
				{
					try
					{
						t_go.draw(a_gameTime);
					}
					catch (Exception e)
					{
						ErrorLogger.getInstance().writeString("While drawing " + t_go + " got exception: " + e);
					}
				}
			}
			if (m_background != null && Game.getInstance().m_camera.getLayer() == 0)
			{
				Rectangle m_dest = Game.getInstance().m_camera.getRectangle();
				m_dest.Width /= 2;
				m_dest.X += m_dest.Width / 2;
				m_dest.Height /= 2;
				m_dest.Y += m_dest.Height / 2;
				a_spriteBatch.Draw(m_background, m_dest, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);
			}
		}

		public static bool checkBigBoxCollision(Rectangle a_first, Rectangle a_second)
		{
			return (a_first.X - 1 < a_second.X + a_second.Width &&
				a_first.X + a_first.Width + 1 > a_second.X &&
				a_first.Y - 1 < a_second.Y + a_second.Height &&
				a_first.Y + a_first.Height + 1 > a_second.Y);
		}

		public override void addObject(GameObject a_object)
		{
			m_addList[m_currentList].Push(a_object);
		}
		public override void removeObject(GameObject a_object)
		{
			m_removeList[m_currentList].Push(a_object);
		}
		public override void addObject(GameObject a_object, int a_layer)
		{
			m_addList[a_layer].Push(a_object);
		}
		public override void removeObject(GameObject a_object, int a_layer)
		{
			m_removeList[a_layer].Push(a_object);
		}
		public override Player getPlayer()
		{
			return player;
		}
		public override LinkedList<GameObject>[] getObjectList()
		{
			return m_gameObjectList;
		}
		public override LinkedList<GameObject> getCurrentList()
		{
			return m_gameObjectList[m_currentList];
		}
		public override void changeLayer(int a_newLayer)
		{
			if (player != null)
			{
				moveObjectToLayer(player, a_newLayer);
			}
			Game.getInstance().m_camera.setLayer(a_newLayer);
		}
		public override void addGuiObject(GuiObject a_go)
		{
			m_guiList.AddLast(a_go);
		}

		public static Keys getUpKey()
		{
			return m_upKey;
		}

		public static Keys getDownKey()
		{
			return m_downKey;
		}

		public static Keys getLeftKey()
		{
			return m_leftKey;
		}

		public static Keys getRightKey()
		{
			return m_rightKey;
		}

		public static Keys getJumpKey()
		{
			return m_jumpKey;
		}

		public static Keys getRollKey()
		{
			return m_rollKey;
		}

		public static Keys getActionKey()
		{
			return m_actionKey;
		}

		public static Keys getSprintKey()
		{
			return m_sprintKey;
		}

		public void clearAggro()
		{
			foreach (LinkedList<GameObject> t_goList in m_gameObjectList)
			{
				foreach (GameObject t_go in t_goList)
				{
					if (t_go is Guard)
					{
						if (((NPE)t_go).getAIState() is AIStateChasing)
						{
							((NPE)t_go).setAIState(AIStateGoingToTheSwitch.getInstance());
						}
					}
					else if (t_go is GuardDog)
					{
						((NPE)t_go).setAIState(AIStatepatroling.getInstance());
					}
				}
			}
		}
		internal LinkedList<Event> getEvents()
		{
			return m_events;
		}
		public string getCurrentLevelName()
		{
			return m_currentLevel;
		}

		public override void moveObjectToLayer(GameObject a_go, int a_layer)
		{
			if (m_gameObjectList[a_go.getListLayer()].Contains(a_go))
			{
				removeObject(a_go, a_go.getListLayer());
				addObject(a_go, a_layer);
			}
			else
			{
				ErrorLogger.getInstance().writeString("could not find " + a_go + " while changeing its layer. searches for it in other layers");
				for (int i = 0; i < 5; ++i)
				{
					if (m_gameObjectList[i].Contains(a_go))
					{
						addObject(a_go, a_layer);
						removeObject(a_go, i);
						return;
					}
				}
				throw new ArgumentException(a_go + " was not found");
			}
		}

		public override bool objectIsOnLayer(GameObject a_obj, int a_layer)
		{
			return m_gameObjectList[a_layer].Contains(a_obj);
		}

		public string getLevelName()
		{
			return m_currentLevel;
		}

		public Texture2D getBackground()
		{
			return m_background;
		}
	}
}
