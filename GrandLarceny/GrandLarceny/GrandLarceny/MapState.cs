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

		private Texture2D m_mapPoint;

		private States m_backState;
		private float m_zoom = 18f;

		private Vector2 m_topLeftPoint;
		private Vector2 m_centerPoint;

		private Color[] m_colors;

		private LinkedList<Vector2> m_goals;

		public MapState(States a_backState)
		{
			m_backState = a_backState;
			m_colors = new Color[] { Color.White, new Color(122, 129, 211), new Color(37,45,128)};
			m_goals = new LinkedList<Vector2>();
		}

		public override void load()
		{
			base.load();
			setCoordiantes();
			setMapTexture();
		}

		private void setMapTexture()
		{
			int t_width = ((int)(Game.getInstance().getResolution().X)) - 40;
			m_map = new Texture2D(Game.getInstance().GraphicsDevice, t_width, ((int)(Game.getInstance().getResolution().Y)) - 40, false, SurfaceFormat.Color);
			Color[] t_colors = new Color[m_map.Width * m_map.Height];
			Color t_lerp1 = new Color(0.001f, 0.001f, 0.001f, 0.3f);
			Color t_lerp2 = new Color(0.004f, 0.004f, 0.004f, 0.3f);
			for (int i = 0; i < t_colors.Length; ++i)
			{
				t_colors[i] = Color.Lerp(t_lerp1, t_lerp2 , ((float)i) / ((float)t_colors.Length));
			}

			foreach(GameObject f_go in m_backState.getObjectList()[0])
			{
				if (f_go is Entity && !((Entity)f_go).isTransparent())
				{
					addRectangle(((Entity)f_go).getHitBox().getOutBox(), 0, t_colors, t_width);
				}
				else if (f_go is Environment && ((Environment)f_go).isVisibleOnMap())
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
				else if (f_go is ConsumableGoal)
				{
					Vector2 t_worldPos = f_go.getPosition().getGlobalCartesian() + (f_go.getImg().getSize() / 2f);
					Vector2 t_screenPos = (m_centerPoint - t_worldPos) / m_zoom + new Vector2(15f);
					Vector2 t_renderPos = Game.getInstance().m_camera.getPosition().getGlobalCartesian() - ((t_screenPos - new Vector2(20f)) * Game.getInstance().m_camera.getZoom());
					m_goals.AddLast(t_renderPos);
				}
			}
			m_map.SetData(t_colors);
		}

		private void setCoordiantes()
		{
			//m_centerPoint = Game.getInstance().m_camera.getPosition().getGlobalCartesian();
			//m_topLeftPoint = m_centerPoint - ((Game.getInstance().getResolution() / 2f) * m_zoom);
			Vector2 t_bottomRight = Vector2.Zero;
			bool t_first = true;
			foreach (GameObject f_go in m_backState.getObjectList()[0])
			{
				if (t_first)
				{
					t_bottomRight = f_go.getPosition().getGlobalCartesian();
					m_topLeftPoint = f_go.getPosition().getGlobalCartesian();
					t_first = false;
				}
				else
				{
					if (f_go.getPosition().getGlobalX() < m_topLeftPoint.X)
					{
						m_topLeftPoint.X = f_go.getPosition().getGlobalX();
					}
					if (f_go.getPosition().getGlobalY() < m_topLeftPoint.Y)
					{
						m_topLeftPoint.Y = f_go.getPosition().getGlobalY();
					}
					if (f_go.getPosition().getGlobalX() > t_bottomRight.X)
					{
						t_bottomRight.X = f_go.getPosition().getGlobalX();
					}
					if (f_go.getPosition().getGlobalY() > t_bottomRight.Y)
					{
						t_bottomRight.Y = f_go.getPosition().getGlobalY();
					}
				}
			}
			m_centerPoint = (m_topLeftPoint + t_bottomRight) / 2;
			m_topLeftPoint = m_centerPoint - ((Game.getInstance().getResolution() / 2f) * m_zoom);
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
			if (KeyboardHandler.keyClicked(Microsoft.Xna.Framework.Input.Keys.Escape) ||
				KeyboardHandler.keyClicked(Microsoft.Xna.Framework.Input.Keys.Space) ||
				KeyboardHandler.keyClicked(Microsoft.Xna.Framework.Input.Keys.M))
			{
				Game.getInstance().setState(m_backState);
			}
			else
			{
				m_mapPoint = updatePointTexture(a_gameTime);
			}
		}

		public Texture2D updatePointTexture(GameTime a_gameTime)
		{
			int t_width = 30;
			int t_height = 30;
			float t_innerRadius = 1.5f * (4f + (float)Math.Cos(a_gameTime.TotalGameTime.TotalSeconds * 5.0));
			float t_outerRadius = 2f * (4f + (float)Math.Cos(a_gameTime.TotalGameTime.TotalSeconds * 5.0));
			Texture2D t_ret = new Texture2D(Game.getInstance().GraphicsDevice, t_width, t_height, false, SurfaceFormat.Color);
			Color[] t_colors = new Color[t_width * t_height];
			Position t_playerPos = m_backState.getPlayer().getPosition();
			for (int i = 0; i < t_colors.Length; ++i)
			{
				float t_distanceFromMid = (float)Math.Sqrt(Math.Pow((i % t_width) - (t_width / 2f), 2.0) + Math.Pow(Math.Floor(((float)i) / ((float)t_width)) - (t_height / 2), 2.0));
				float t_alpha = (t_distanceFromMid - t_outerRadius) / (t_innerRadius - t_outerRadius);
				t_colors[i] = Color.Lerp(Color.White, Color.Transparent, 1f - t_alpha);
			}
			t_ret.SetData(t_colors);
			return t_ret;
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			Vector2 t_mapPos = Game.getInstance().m_camera.getPosition().getGlobalCartesian() - (((Game.getInstance().getResolution() / 2f) - new Vector2(20f)) * Game.getInstance().m_camera.getZoom());

			a_spriteBatch.Draw(m_map, t_mapPos, Color.White);

			if (m_mapPoint != null)
			{
				Vector2 t_worldPos = m_backState.getPlayer().getPosition().getGlobalCartesian() + (m_backState.getPlayer().getImg().getSize() / 2f);
				Vector2 t_screenPos = (m_centerPoint - t_worldPos) / m_zoom + new Vector2(15f);
				Vector2 t_renderPos = Game.getInstance().m_camera.getPosition().getGlobalCartesian() - ((t_screenPos - new Vector2(20f)) * Game.getInstance().m_camera.getZoom());
				a_spriteBatch.Draw(m_mapPoint, t_renderPos, Color.Red);
			}
			foreach (Vector2 f_v in m_goals)
			{
				a_spriteBatch.Draw(m_mapPoint, f_v, Color.Turquoise);
			}
			m_backState.draw(a_gameTime, a_spriteBatch);
		}
	}
}