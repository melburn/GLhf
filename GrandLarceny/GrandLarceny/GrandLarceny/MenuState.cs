using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	public class MenuState : States
	{
		internal LinkedList<Button> m_buttons = new LinkedList<Button>();

		public override void update(GameTime a_gameTime)
		{

		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{

		}

		public override void addObject(GameObject a_object)
		{
			//throw new NotImplementedException();
		}
	}
}
