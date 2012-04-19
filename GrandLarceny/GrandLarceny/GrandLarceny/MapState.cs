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
		private float m_zoom = 36f;

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
				t_colors[i] = new Color(0,0,((float)i) / ((float)(t_colors.Length)));
			}
			m_topLeftPoint = Game.getInstance().m_camera.getPosition().getLocalCartesianCoordinates();
			foreach(GameObject f_go in m_backState.getObjectList()[Game.getInstance().m_camera.getLayer()])
			{
				if (f_go is Entity && !((Entity)f_go).isTransparent())
				{
					addRectangle(((Entity)f_go).getBox(), Color.White, t_colors, t_width);
				}
			}
			m_map.SetData(t_colors);
		}

		private void addRectangle(Rectangle a_rectangle, Color a_color, Color[] a_oldArray, int a_width)
		{
			for (int y = (int)Math.Floor(((float)(a_rectangle.Y)) / m_zoom); y < (int)Math.Ceiling(((float)(a_rectangle.Y + a_rectangle.Height)) / m_zoom); ++y)
			{
				if (y >= 0 && y < a_oldArray.Length / a_width)
				{
					for (int x = (int)Math.Floor(((float)(a_rectangle.X)) / m_zoom); x < (int)Math.Ceiling(((float)(a_rectangle.X + a_rectangle.Width)) / m_zoom); ++x)
					{
						if (x > 0 && x < a_width)
						{
							a_oldArray[y * a_width + x] = a_color;
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
			a_spriteBatch.Draw(m_map, new Vector2(20), Color.White);
		}
	}
}
