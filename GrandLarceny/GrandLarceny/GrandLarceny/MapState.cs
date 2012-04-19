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

		public MapState(States a_backState)
		{
			m_backState = a_backState;
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
			m_topLeftPoint = Game.getInstance().m_camera.getPosition().getLocalCartesianCoordinates()  - (((Game.getInstance().getResolution() / 2f) - new Vector2(20)) * Game.getInstance().m_camera.getZoom());
			foreach(GameObject f_go in m_backState.getObjectList()[Game.getInstance().m_camera.getLayer()])
			{
				if (f_go is Entity && !((Entity)f_go).isTransparent())
				{
					addRectangle(((Entity)f_go).getHitBox().getOutBox(), true, t_colors, t_width);
				}
				else if(f_go is Environment)
				{
					addRectangle(f_go.getBox(), false, t_colors, t_width);
				}
			}
			m_map.SetData(t_colors);
		}

		private void addRectangle(Rectangle a_rectangle, Boolean a_white, Color[] a_oldArray, int a_width)
		{
			for (int y = (int)(Math.Floor(((float)(a_rectangle.Y)) / m_zoom) - m_topLeftPoint.Y); y < (int)(Math.Ceiling(((float)(a_rectangle.Y + a_rectangle.Height)) / m_zoom) - m_topLeftPoint.Y); ++y)
			{
				if (y >= 0 && y < a_oldArray.Length / a_width)
				{
					for (int x = (int)(Math.Floor(((float)(a_rectangle.X)) / m_zoom) - m_topLeftPoint.X); x < (int)(Math.Ceiling(((float)(a_rectangle.X + a_rectangle.Width)) / m_zoom) - m_topLeftPoint.X); ++x)
					{
						if (x > 0 && x < a_width)
						{
							if (a_white)
							{
								a_oldArray[y * a_width + x] = Color.White;
							}
							else if(a_oldArray[y * a_width + x] != Color.White)
							{
								a_oldArray[y * a_width + x] = new Color(160,160,255);
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
