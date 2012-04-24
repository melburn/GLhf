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
			foreach(GameObject f_go in m_backState.getObjectList()[Game.getInstance().m_camera.getLayer()])
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
			Position t_playerPos = m_backState.getPlayer().getPosition();
			Vector2 t_iteratePos;
			for (int i = 0; i < t_colors.Length; ++i)
			{
				t_iteratePos = new Vector2(i % t_width, i / t_width) * m_zoom;
				t_colors[i] = Color.Lerp(t_colors[i], Color.Red, Math.Max(0, 150 - t_playerPos.getDistanceTo((t_iteratePos + m_topLeftPoint)))/100);
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
		public override void update(Microsoft.Xna.Framework.GameTime a_gameTime)
		{
			if (Game.keyClicked(Microsoft.Xna.Framework.Input.Keys.M))
			{
				Game.getInstance().setState(m_backState);
			}
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			a_spriteBatch.Draw(m_map, Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates() - (((Game.getInstance().getResolution() / 2f) - new Vector2(20)) * Game.getInstance().m_camera.getZoom()), Color.White);
		}
	}
}
