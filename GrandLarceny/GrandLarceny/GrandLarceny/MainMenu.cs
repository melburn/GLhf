using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	class MainMenu : MenuState
	{
		public override void load()
		{
			base.load();
			Button t_button = new Button(Game.getInstance().Content.Load<Texture2D>("Images//Normal"),
				Game.getInstance().Content.Load<Texture2D>("Images//Hover"),
				Game.getInstance().Content.Load<Texture2D>("Images//Pressed"),
				new Vector2(15, 38));
			t_button.m_clickEvent += new Button.clickDelegate(playClick);
			m_buttons.Add(t_button);
		}
		public override void update(GameTime a_gameTime)
		{
			foreach (Button t_b in m_buttons)
				t_b.update();
		}

		public override void draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			foreach (Button t_b in m_buttons)
				t_b.draw(gameTime, spriteBatch);
		}
		public void playClick(Button a_b)
		{
			Game.getInstance().setState(new GameState());
		}
		public void exitClick(Button a_b)
		{
			Game.getInstance().Exit();
		}
	}
}
