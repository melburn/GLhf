using System;
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
		public States()
		{
			m_loaded = false;
		}

		public virtual void load()
		{
			m_loaded = true;
		}

		public virtual void setPlayer(Player a_player)
		{
		}
		public virtual Player getPlayer()
		{
			return null;
		}
		public abstract void update(GameTime a_gameTime);
		public abstract void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch);
		public virtual void addObject(GameObject a_object)
		{
			throw new NotImplementedException();
		}
		public virtual void removeObject(GameObject a_object)
		{
			throw new NotImplementedException();
		}
		public virtual void addObject(GameObject a_object, int a_layer)
		{
			throw new NotImplementedException();
		}
		public virtual void removeObject(GameObject a_object, int a_layer)
		{
			throw new NotImplementedException();
		}
		public virtual LinkedList<GameObject>[] getObjectList()
		{
			return new LinkedList<GameObject>[0];
		}
		public virtual LinkedList<GameObject> getCurrentList() {
			return new LinkedList<GameObject>();
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
			throw new NotImplementedException();
		}

		public Vector2 calculateWorldMouse()
		{
			Vector2 t_worldMouse = new Vector2(
				Mouse.GetState().X / Game.getInstance().m_camera.getZoom()
					+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().X
					- ((Game.getInstance().getResolution().X / 2) / Game.getInstance().m_camera.getZoom())
				, Mouse.GetState().Y / Game.getInstance().m_camera.getZoom()
					+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().Y
					- ((Game.getInstance().getResolution().Y / 2) / Game.getInstance().m_camera.getZoom())
			);
			return t_worldMouse;
		}

		public bool isLoaded()
		{
			return m_loaded;
		}

		public virtual void moveObjectToLayer(GameObject a_go, int a_layer)
		{
			throw new NotImplementedException();
		}
	}
}
