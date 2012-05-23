using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace GrandLarceny
{
	public class DeathScene : States
	{
		private LinkedList<GameObject>[] m_gameObjects;
		private TimeSpan m_timer;
		private String m_levelName;
		private String m_progressName;
		private int m_currentList;
		private Player m_player;

		private Texture2D m_background;

		public DeathScene(LinkedList<GameObject>[] a_gameObjects, Texture2D a_background)
		{
			m_background = a_background;
			if (Game.getInstance().getState() is GameState)
			{
				m_levelName = ((GameState)Game.getInstance().getState()).getLevelName();
				m_progressName = Game.getInstance().getProgress().getName();
			}
			else
			{
				throw new InvalidCastException("DeathScene can only be created when in GameState");
			}
			m_gameObjects = a_gameObjects;
			m_player = Game.getInstance().getState().getPlayer();
		}


		public override void update(GameTime a_gameTime)
		{
			if (m_timer == TimeSpan.Zero)
			{
				m_timer = Game.getInstance().getTotalGameTime() + new TimeSpan(0, 0, 4);
			}
			else if (m_timer <= Game.getInstance().getTotalGameTime())
			{
				if (Game.getInstance().hasCheckPoint())
				{
					Game.getInstance().setState(new GameState(m_levelName, true));
					Game.getInstance().setProgress(m_progressName, true);

				}
				else
				{
					Game.getInstance().setState(new GameState(m_levelName));
					Game.getInstance().setProgress(m_progressName, false);
				}
				return;
			}
			GameTime t_slowTime = new GameTime(a_gameTime.TotalGameTime, new TimeSpan(a_gameTime.ElapsedGameTime.Ticks / 3));
			for(m_currentList = 0; m_currentList < 5; ++m_currentList)
			{
				foreach (GameObject t_go in m_gameObjects[m_currentList])
				{
					#if DEBUG
					t_go.update(t_slowTime);
					#elif RELEASE
					try
					{
						t_go.update(t_slowTime);
					}
					catch (Exception e)
					{
						ErrorLogger.getInstance().writeString("While updating " + t_go + " from DeathScene got exception: " + e);
					}
					#endif
				}
			}
			m_currentList = -1;
			for (m_currentList = 0; m_currentList < 5; ++m_currentList)
			{
				foreach (GameObject t_firstGameObject in m_gameObjects[m_currentList])
				{
					if (t_firstGameObject is MovingObject)
					{
						List<Entity> t_collided = new List<Entity>();
						foreach (GameObject t_secondGameObject in m_gameObjects[m_currentList])
						{
							if (t_secondGameObject is Entity && t_firstGameObject != t_secondGameObject
								&& ((Entity)t_firstGameObject).getHitBox() != null && ((Entity)t_secondGameObject).getHitBox() != null
								&& GameState.checkBigBoxCollision(((Entity)t_firstGameObject).getHitBox().getOutBox(), ((Entity)t_secondGameObject).getHitBox().getOutBox()))
							{
								t_collided.Add((Entity)t_secondGameObject);
							}
						}
						((MovingObject)t_firstGameObject).collisionCheck(t_collided);
						if (!(t_firstGameObject.getPosition() is PolarCoordinate))
						{
							((Entity)t_firstGameObject).updatePosition();
						}
					}
				}
			}
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (GameObject t_gameObject in m_gameObjects[Game.getInstance().m_camera.getLayer()])
			{
				#if DEBUG
				t_gameObject.draw(a_gameTime);
				#elif RELEASE
				try
				{
					t_gameObject.draw(a_gameTime);
				}
				catch (Exception e)
				{
					ErrorLogger.getInstance().writeString("While drawing " + t_gameObject + " from DeathScene got exception: " + e);
				}
				#endif
			}
			if (m_background != null)
			{
				Rectangle m_dest = Game.getInstance().m_camera.getRectangle();
				m_dest.Width /= 2;
				m_dest.X += m_dest.Width / 2;
				m_dest.Height /= 2;
				m_dest.Y += m_dest.Height / 2;
				a_spriteBatch.Draw(m_background, m_dest, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);
			}
		}

		public override Player getPlayer()
		{
			return m_player;
		}
	}
}
