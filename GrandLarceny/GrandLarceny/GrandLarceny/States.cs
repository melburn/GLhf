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

		public abstract void update(GameTime gameTime);
		public abstract void draw(GameTime gameTime, SpriteBatch spriteBatch);
	}
}
