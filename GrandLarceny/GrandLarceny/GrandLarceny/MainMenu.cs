using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text.RegularExpressions;

namespace GrandLarceny
{
	class MainMenu : MenuState
	{
		string[] m_levelList;
		public override void load()
		{
			base.load();
			Button t_button = new Button("btn_test", new Vector2(15, 38), null, null, Color.Black, Vector2.Zero);
			t_button.m_clickEvent += new Button.clickDelegate(playClick);
			m_buttons.Add(t_button);

			m_levelList = Directory.GetFiles("Content//levels//");

			int t_count = 0;
			foreach (string t_level in m_levelList)
			{
				t_count++;
				string[] t_splitPath = Regex.Split(t_level, "//");
				Button t_levelButton = new Button("btn_test_empty", "btn_test_empty", "btn_test_empty", "btn_test_empty", new Vector2(200, 200 * t_count), t_splitPath[t_splitPath.Length - 1], "VerdanaBold", Color.Black, new Vector2(10, 10));
				t_levelButton.m_clickEvent += new Button.clickDelegate(startLevelClick);
				m_buttons.Add(t_levelButton);
			}
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
		public void startLevelClick(Button a_b)
		{
			Game.getInstance().setState(new GameState(a_b.getText()));
		}
	}
}
