using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	public class DeathScene : States
	{
		private LinkedList<GameObject>[] m_gameObjects;
		public DeathScene(LinkedList<GameObject>[] a_gameObjects)
		{
			m_gameObjects = a_gameObjects;
		}


		public override void update(GameTime a_gameTime)
		{
			GameTime t_slowTime = new GameTime(a_gameTime.TotalGameTime, new TimeSpan(a_gameTime.ElapsedGameTime.Ticks / 3));
			foreach (LinkedList<GameObject> t_llgo in m_gameObjects)
			{
				foreach (GameObject t_go in t_llgo)
				{
					try
					{
						t_go.update(t_slowTime);
					}
					catch (Exception e)
					{
						ErrorLogger.getInstance().writeString("While updating " + t_go + " from DeathScene got exception: " + e);
					}
				}
			}
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (GameObject t_gameObject in m_gameObjects[Game.getInstance().m_camera.getLayer()])
			{
				try
				{
					t_gameObject.draw(a_gameTime);
				}
				catch (Exception e)
				{
					ErrorLogger.getInstance().writeString("While drawing " + t_gameObject + " from DeathScene got exception: " + e);
				}
			}
		}
	}
}
