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

		public MapState(States a_backState)
		{
			m_backState = a_backState;
		}

		public override void load()
		{
			base.load();
			m_map = new Texture2D(Game.getInstance().GraphicsDevice, ((int)(Game.getInstance().getResolution().X)) - 40, ((int)(Game.getInstance().getResolution().Y)) - 40, false, SurfaceFormat.Color);
			setMapTexture();
		}

		private void setMapTexture()
		{
			Color[] t_colors = new Color[m_map.Width * m_map.Height];
			for (int i = 0; i < t_colors.Length; ++i)
			{
				t_colors[i] = new Color(0,0,i / t_colors.Length);
			}
			m_map.SetData(t_colors);
			foreach(GameObject f_go in m_backState.getObjectList()[Game.getInstance().m_camera.getLayer()])
			{
				if (f_go is Entity && !((Entity)f_go).isTransparent())
				{
					addRectangle(((Entity)f_go).getBox());
				}
			}
		}

		private void addRectangle(Rectangle a_rectangle)
		{
			//for(int y = (int)Math.Floor(a_rectangle.Y / 36f); y < (int)Math.Ceiling((a_rectangle.Y + a_rectangle.Height) / 
		}
		public override void update(Microsoft.Xna.Framework.GameTime a_gameTime)
		{
			if (Game.isKeyPressed(Microsoft.Xna.Framework.Input.Keys.M))
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
