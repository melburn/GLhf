﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	public abstract class States
	{
		protected bool m_loaded;

		protected LinkedList<GameObject>[]			m_gameObjectList;
		protected LinkedList<GuiObject>				m_guiList;
		protected LinkedList<LinkedList<Button>>	m_buttonList;
		protected Stack<GameObject>[] m_removeList;


		public States()
		{
			m_loaded = false;
			m_gameObjectList	= new LinkedList<GameObject>[5];
			m_guiList			= new LinkedList<GuiObject>();
			m_buttonList		= new LinkedList<LinkedList<Button>>();
		}

		public virtual void load()
		{
			m_loaded = true;
			m_removeList = new Stack<GameObject>[m_gameObjectList.Length];
			for (int i = 0; i < m_gameObjectList.Length; ++i)
			{
				m_removeList[i] = new Stack<GameObject>();
			}
		}

		public virtual void update(GameTime a_gameTime) {
			int t_currentList = -1;
			foreach (LinkedList<GameObject> t_list in m_gameObjectList)
			{
				++t_currentList;
				while (m_removeList[t_currentList].Count > 0)
				{
					t_list.Remove(m_removeList[t_currentList].Pop());
				}
			}
			foreach (GuiObject t_guiObject in m_guiList)
			{
				t_guiObject.update(a_gameTime);
			}
		}
		public abstract void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch);

		public virtual void setPlayer(Player a_player)
		{
		}

		public virtual Player getPlayer()
		{
			return null;
		}

		public virtual void addObject(GameObject a_object)
		{
		}

		public virtual void removeObject(GameObject a_object)
		{
		}

		public virtual void addObject(GameObject a_object, int a_layer)
		{
		}

		public virtual void removeObject(GameObject a_object, int a_layer)
		{
		}

		public virtual LinkedList<GameObject>[] getObjectList()
		{
			return null;
		}

		public virtual LinkedList<GameObject> getCurrentList()
		{
			return null;
		}

		public virtual void changeLayer(int a_newLayer)
		{
		}

		public virtual void addGuiObject(GuiObject a_go)
		{
			throw new NotImplementedException();
		}
		
		internal virtual GameObject getObjectById(int a_id)
		{
			foreach (LinkedList<GameObject> t_goList in m_gameObjectList)
			{
				foreach (GameObject t_go in t_goList)
				{
					if (a_id == t_go.getId())
					{
						return t_go;
					}
				}
			}
			return null;
		}

		public virtual bool collidedWithGui(Vector2 a_coordinate)
		{
			foreach (GuiObject t_guiObject in m_guiList)
			{
				if (t_guiObject.getBox().Contains((int)a_coordinate.X, (int)a_coordinate.Y))
				{
					return true;
				}
			}

			foreach (LinkedList<Button> t_buttonList in m_buttonList)
			{
				foreach (Button t_button in t_buttonList)
				{
					if (t_button.getBox().Contains((int)a_coordinate.X, (int)a_coordinate.Y))
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual bool collidedWithObject(Vector2 a_coordinate)
		{
			foreach (GameObject t_gameObject in m_gameObjectList[Game.getInstance().m_camera.getLayer()])
			{
				if (((Entity)t_gameObject).getHitBox().contains(a_coordinate)) {
					return true;
				}
			}
			return false;
		}

		public Vector2 getTileVector(Vector2 a_pixelPosition)
		{
			Vector2 t_ret = a_pixelPosition;
			t_ret.X = (float)(Math.Floor(a_pixelPosition.X / Game.TILE_WIDTH));
			t_ret.Y = (float)(Math.Floor(a_pixelPosition.Y / Game.TILE_HEIGHT));
			return t_ret;
		}

		public Vector2 getTileCoordinates(Vector2 a_pixelPosition)
		{
			Vector2 t_ret = getTileVector(a_pixelPosition);
			t_ret.X *= Game.TILE_WIDTH;
			t_ret.Y *= Game.TILE_HEIGHT;
			return t_ret;
		}

		public bool isLoaded()
		{
			return m_loaded;
		}

		public virtual void moveObjectToLayer(GameObject a_go, int a_layer)
		{
			throw new NotImplementedException();
		}

		public virtual bool objectIsOnLayer(GameObject a_obj, int a_player)
		{
			throw new NotImplementedException();
		}
	}
}
