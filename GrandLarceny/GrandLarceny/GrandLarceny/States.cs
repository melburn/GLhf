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

		public virtual void setPlayer(Player a_player)
		{
		}
		public virtual Player getPlayer()
		{
			return null;
		}
		public abstract void update(GameTime a_gameTime);
		public abstract void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch);
		public virtual void addObject(GameObject a_object) { }
		public virtual void addOrRemoveObject(GameObject a_object) { }
	}
}
