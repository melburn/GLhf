using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace GrandLarceny
{
	class MainMenu : MenuState
	{
		#region Members

		private ParseState m_currentParse;
		private enum ParseState {
			Settings
		}
		#endregion

		#region Constructor & Load
		public override void load()
		{
			base.load();

			string[] t_loadedFile = File.ReadAllLines("Content//wtf//settings.ini");
			foreach (string t_currentLine in t_loadedFile)
			{
				if (t_currentLine.Length > 2 && t_currentLine.First() == '[' && t_currentLine.Last() == ']')
				{
					if (t_currentLine.Equals("[Graphics]"))
					{
						m_currentParse = ParseState.Settings;
					}
				}
				switch (m_currentParse)
				{
					case ParseState.Settings:
						string[] t_setting = t_currentLine.Split('=');
						if (t_setting[0].Equals("ScreenWidth"))
						{
							Game.getInstance().m_graphics.PreferredBackBufferWidth = int.Parse(t_setting[1]);
						}
						else if (t_setting[0].Equals("ScreenHeight"))
						{
							Game.getInstance().m_graphics.PreferredBackBufferHeight = int.Parse(t_setting[1]);
							Game.getInstance().m_camera.setZoom(Game.getInstance().getResolution().Y / 720);
						}
						else if (t_setting[0].Equals("Fullscreen"))
						{
							Game.getInstance().m_graphics.IsFullScreen = bool.Parse(t_setting[1]);
						}
						break;
				}
				Game.getInstance().m_graphics.ApplyChanges();
			}
			Button t_newGame = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", Vector2.Zero, "New Game", "VerdanaBold", Color.White, Vector2.Zero);
			t_newGame.m_clickEvent += new Button.clickDelegate(newGameClick);
			m_buttons.AddLast(t_newGame);
			Button t_loadGame = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", Vector2.Zero, "Load Game", "VerdanaBold", Color.White, Vector2.Zero);
			t_loadGame.m_clickEvent += new Button.clickDelegate(loadGameClick);
			m_buttons.AddLast(t_loadGame);
			Button t_settingButton = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", Vector2.Zero, "Settings", "VerdanaBold", Color.White, Vector2.Zero);
			m_buttons.AddLast(t_settingButton);
			GuiListFactory.setListPosition(m_buttons, new Vector2(Game.getInstance().getResolution().X / 2 - 80, Game.getInstance().getResolution().Y / 2));
			GuiListFactory.setTextOffset(m_buttons, new Vector2(20, 0));
			GuiListFactory.setButtonDistance(m_buttons, new Vector2(0, 60));
		}
		#endregion

		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{
			foreach (Button t_b in m_buttons)
				t_b.update();
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (Button t_b in m_buttons)
				t_b.draw(a_gameTime, a_spriteBatch);
		}
		#endregion

		#region Main Menu Methods (MMM...Bio)
		public void exitClick(Button a_b)
		{
			Game.getInstance().Exit();
		}
		public void newGameClick(Button a_b)
		{
			Game.getInstance().setState(new HubMenu());
			Game.getInstance().setProgress("temp.prog");
		}
		public void loadGameClick(Button a_b)
		{
			Game.getInstance().setState(new LoadAndSaveMenu(false, this));
		}
		#endregion
	}
}
