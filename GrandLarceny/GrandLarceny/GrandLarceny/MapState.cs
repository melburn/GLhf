using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class MapState : States
	{
		private Texture2D m_map;
		private Texture2D m_playerPoint;

		private States m_backState;
		private float m_zoom = 18f;

		private Vector2 m_topLeftPoint;
		private Vector2 m_centerPoint;

		private Color[] m_colors;

		public MapState(States a_backState)
		{
			m_backState = a_backState;
			m_colors = new Color[] { Color.White, Color.Yellow, new Color(130, 130, 250) };
		}

		public override void load()
		{
			base.load();
			setMapTexture();
		}

		private void setMapTexture()
		{
			int t_width = ((int)(Game.getInstance().getResolution().X)) - 40;
			m_map = new Texture2D(Game.getInstance().GraphicsDevice, t_width, ((int)(Game.getInstance().getResolution().Y)) - 40, false, SurfaceFormat.Color);
			Color[] t_colors = new Color[m_map.Width * m_map.Height];
			for (int i = 0; i < t_colors.Length; ++i)
			{
				t_colors[i] = Color.Lerp(Color.LightBlue, Color.Blue, ((float)i) / ((float)t_colors.Length));
			}

			m_centerPoint = Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates();
			m_topLeftPoint = m_centerPoint - ((Game.getInstance().getResolution() / 2f) * m_zoom);

			foreach(GameObject f_go in m_backState.getObjectList()[0])
			{
				if (f_go is Entity && !((Entity)f_go).isTransparent())
				{
					addRectangle(((Entity)f_go).getHitBox().getOutBox(), 0, t_colors, t_width);
				}
				else if(f_go is Environment)
				{
					if (((Environment)f_go).isExplored())
					{
						addRectangle(f_go.getBox(), 1, t_colors, t_width);
					}
					else
					{
						addRectangle(f_go.getBox(), 2, t_colors, t_width);
					}
				}
			}
			m_map.SetData(t_colors);
		}

		private void addRectangle(Rectangle a_rectangle, int a_color, Color[] a_oldArray, int a_width)
		{
			for (int y = (int)(Math.Floor(((float)(a_rectangle.Y)) - m_topLeftPoint.Y) / m_zoom); y < (int)(Math.Ceiling((((float)(a_rectangle.Y + a_rectangle.Height)) - m_topLeftPoint.Y) / m_zoom)); ++y)
			{
				if (y >= 0 && y < a_oldArray.Length / a_width)
				{
					for (int x = (int)(Math.Floor(((float)(a_rectangle.X)) - m_topLeftPoint.X) / m_zoom); x < (int)(Math.Ceiling((((float)(a_rectangle.X + a_rectangle.Width)) - m_topLeftPoint.X) / m_zoom)); ++x)
					{
						if (x > 0 && x < a_width)
						{
							for (int i = 0; true; ++i)
							{
								if (i == a_color)
								{
									a_oldArray[y * a_width + x] = m_colors[i];
									break;
								}
								else if (a_oldArray[y * a_width + x] == m_colors[i])
								{
									break;
								}
							}
						}
					}
				}
			}
		}
		public override void update(GameTime a_gameTime)
		{
			updatePlayerPoint(a_gameTime);
			if (Game.keyClicked(Microsoft.Xna.Framework.Input.Keys.M))
			{
				Game.getInstance().setState(m_backState);
			}
		}

		public void updatePlayerPoint(GameTime a_gameTime)
		{
			int t_width = 30;
			int t_height = 30;
			float t_innerRadius = 1.5f * (4f + (float)Math.Cos(a_gameTime.TotalGameTime.TotalSeconds*5.0));
			float t_outerRadius = 2f * (4f + (float)Math.Cos(a_gameTime.TotalGameTime.TotalSeconds*5.0));
			m_playerPoint = new Texture2D(Game.getInstance().GraphicsDevice, t_width, t_height, false, SurfaceFormat.Color);
			Color[] t_colors = new Color[t_width * t_height];
			Position t_playerPos = m_backState.getPlayer().getPosition();
			for (int i = 0; i < t_colors.Length; ++i)
			{
				float t_distanceFromMid = (float)Math.Sqrt(Math.Pow((i % t_width)-(t_width/2f),2.0)+Math.Pow(Math.Floor(((float)i)/((float)t_width))-(t_height/2),2.0));
				float t_alpha = (t_distanceFromMid - t_outerRadius) / (t_innerRadius - t_outerRadius);
				t_colors[i] = Color.Lerp(Color.Red, Color.Transparent, 1f-t_alpha);
			}
			m_playerPoint.SetData(t_colors);
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			Vector2 t_mapPos = Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates() - (((Game.getInstance().getResolution() / 2f) - new Vector2(20f)) * Game.getInstance().m_camera.getZoom());
			if (m_playerPoint != null)
			{
				Vector2 t_worldPos = m_backState.getPlayer().getPosition().getGlobalCartesianCoordinates() + (m_backState.getPlayer().getImg().getSize() / 2f);
				Vector2 t_screenPos = (Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates() - t_worldPos) / m_zoom + new Vector2(15f);
				Vector2 t_renderPos = Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates() - ((t_screenPos - new Vector2(20f)) * Game.getInstance().m_camera.getZoom());
				a_spriteBatch.Draw(m_playerPoint, t_renderPos, Color.White);
			}
			a_spriteBatch.Draw(m_map, t_mapPos, Color.White);
		}
	}
}