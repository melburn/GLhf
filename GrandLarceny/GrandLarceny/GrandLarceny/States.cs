using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	public abstract class States
	{
		public States()
		{
			
		}

		public virtual void load()
		{
		}

		public abstract void setPlayer(Player a_player);
		public abstract void update(GameTime a_gameTime);
		public abstract void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch);
	}
}
