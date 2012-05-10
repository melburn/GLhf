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
		private Button m_btnSettings;

		#region Constructor & Load
		public override void load()
		{
			base.load();
			Loader.getInstance().loadGraphicSettings("Content//wtf//settings.ini");
			//new Game
			Button t_newGame = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", Vector2.Zero, "New Game", "VerdanaBold", Color.White, Vector2.Zero);
			t_newGame.m_clickEvent += new Button.clickDelegate(newGameClick);
			m_buttons.AddLast(t_newGame);
			//load game
			Button t_loadGame = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", Vector2.Zero, "Load Game", "VerdanaBold", Color.White, Vector2.Zero);
			t_loadGame.m_clickEvent += new Button.clickDelegate(loadGameClick);
			m_buttons.AddLast(t_loadGame);
			
			Button t_settingButton = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", Vector2.Zero, "Settings", "VerdanaBold", Color.White, Vector2.Zero);
			m_buttons.AddLast(t_settingButton);
			GuiListFactory.setListPosition(m_buttons, new Vector2(Game.getInstance().getResolution().X / 2 - 80, Game.getInstance().getResolution().Y / 2));
			GuiListFactory.setTextOffset(m_buttons, new Vector2(20, 0));
			GuiListFactory.setButtonDistance(m_buttons, new Vector2(0, 60));
			//setting button
			m_buttons.AddLast(m_btnSettings = new Button("btn_settings", new Vector2(100, 100)));
			m_btnSettings.m_clickEvent += new Button.clickDelegate(settingsClick);
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
			Game.getInstance().setProgress("temp.prog", false);
		}

		public void loadGameClick(Button a_b)
		{
			Game.getInstance().setState(new LoadAndSaveMenu(false, this));
		}

		public void settingsClick(Button a_button)
		{
			Game.getInstance().setState(new SettingsMenu());
		}
		#endregion
	}
}
