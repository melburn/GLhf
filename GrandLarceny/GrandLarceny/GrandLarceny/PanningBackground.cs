using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	public class PanningBackground
	{
		private LinkedList<ParallaxEnvironment> m_parallaxEnvironments;
		private Random m_random;
		private Texture2D m_background;
		private Environment m_logo;

		public PanningBackground()
		{
			m_parallaxEnvironments = new LinkedList<ParallaxEnvironment>();
			m_random = new Random(DateTime.Now.Millisecond);
			float t_houseDistance = Game.getInstance().getResolution().X / 10;
			m_background = Game.getInstance().Content.Load<Texture2D>("Images//Background//starry_sky_01");

			for (int i = 0; i < 50; i++)
			{
				m_parallaxEnvironments.AddLast(new ParallaxEnvironment(
					new Vector2(t_houseDistance * i - Game.getInstance().m_camera.getRectangle().Width, -300), 
					"Images//Background//Parallax//bg_house_0" + randomNumber(1, 7).ToString(),
					0.950f
				));
				m_parallaxEnvironments.Last().setParrScroll(randomNumber(50, 600));
			}
			for (int i = 0; i < 25; i++)
			{
				m_parallaxEnvironments.AddLast(new ParallaxEnvironment(
					new Vector2(t_houseDistance * i - Game.getInstance().m_camera.getRectangle().Width, randomNumber(-300, 200)),
					"Images//Background//Parallax//clouds_0" + randomNumber(1, 4).ToString(),
					0.950f
				));
				m_parallaxEnvironments.Last().setParrScroll(randomNumber(50, 600));
			}
			m_logo = new Environment(new Vector2(-400, -250), "Images//GUI//logotext", 0.800f);
		}

		private int randomNumber(int a_min, int a_max)
		{
			int t_difference = 0;
			if (a_min < 0)
			{
				t_difference = 0 - a_min;
				a_min += t_difference;
				a_max += t_difference;
			}
			int t_return = m_random.Next(a_min, a_max);
			if (t_difference != 0)
			{
				return t_return - t_difference;
			}
			else
			{
				return t_return;
			}
		}

		public void update(GameTime a_gameTime)
		{
			for (int i = 0; i < m_parallaxEnvironments.Count(); i++)
			{
				if (m_parallaxEnvironments.ElementAt(i).getPosition().getGlobalX() > Game.getInstance().getResolution().X)
				{
					m_parallaxEnvironments.ElementAt(i).getPosition().setGlobalX(-Game.getInstance().getResolution().X - m_parallaxEnvironments.ElementAt(i).getBox().Width);
				}
				else
				{
					m_parallaxEnvironments.ElementAt(i).getPosition().plusXWith(0.4f);
				}
			}
		}

		public void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			for (int i = 0; i < m_parallaxEnvironments.Count(); i++)
			{
				m_parallaxEnvironments.ElementAt(i).draw(a_gameTime);
			}

			m_logo.draw(a_gameTime);

			if (m_background != null && Game.getInstance().m_camera.getLayer() == 0)
			{
				//Vector2 t_imgPosition = new Vector2(m_background.Bounds.X, m_background.Bounds.Y);

				Game.getInstance().getSpriteBatch().Draw(
					m_background,
					new Rectangle(
						(int)((m_background.Bounds.X - Game.getInstance().getResolution().X / 2)/Game.getInstance().m_camera.getZoom()), 
						(int)((m_background.Bounds.Y - Game.getInstance().getResolution().Y / 2)/Game.getInstance().m_camera.getZoom()), 
						(int)(m_background.Bounds.Width/Game.getInstance().m_camera.getZoom()), 
						(int)(m_background.Bounds.Height/Game.getInstance().m_camera.getZoom())),
					new Rectangle(m_background.Bounds.X, m_background.Bounds.Y, (int)(m_background.Bounds.Width/Game.getInstance().m_camera.getZoom()), (int)(m_background.Bounds.Height/Game.getInstance().m_camera.getZoom())),
					Color.White,
					0.0f,
					Vector2.Zero,
					SpriteEffects.None,
					1.000f
				);

				/*
				Rectangle m_dest = Game.getInstance().m_camera.getRectangle();
				m_dest.Width /= 2;
				m_dest.X += m_dest.Width / 2;
				m_dest.Height /= 2;
				m_dest.Y += m_dest.Height / 2;
				a_spriteBatch.Draw(m_background, m_dest, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);
				*/
			}
		}
	}
}
