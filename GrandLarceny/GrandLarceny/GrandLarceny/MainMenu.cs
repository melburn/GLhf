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
		private int m_currentButton = 0;
		private Music m_menuSong;

		#region Constructor & Load
		public override void load()
		{
			base.load();
			Loader.getInstance().loadGraphicSettings("Content//wtf//settings.ini");


			string t_ext = "Slot*";
			if (!Directory.Exists("Content//levels//"))
			{
				Directory.CreateDirectory("Content//levels//");
			}
			string[] t_saveFiles = Directory.GetFiles("Content//levels//", t_ext);
			/*if (t_saveFiles.Length > 0)
			{
				//load game
				Button t_loadGame = new Button("continue", "continue_selected", "continue_selected", "continue_selected", Vector2.Zero, "", "VerdanaBold", Color.White, Vector2.Zero);
				t_loadGame.m_clickEvent += new Button.clickDelegate(loadGameClick);
				m_buttons.AddLast(t_loadGame);
			}*/
			//new Game
			Button t_newGame = new Button("newgame", "newgame_selected", "newgame_selected", "newgame_selected", Vector2.Zero, "", "VerdanaBold", Color.White, Vector2.Zero);
			t_newGame.m_clickEvent += new Button.clickDelegate(loadGameClick);
			m_buttons.AddLast(t_newGame);

			//new Game
			Button t_Credit = new Button("credits", "credits_selected", "credits_selected", "credits_selected", Vector2.Zero, "", "VerdanaBold", Color.White, Vector2.Zero);
			//t_newGame.m_clickEvent += new Button.clickDelegate(newGameClick);
			m_buttons.AddLast(t_Credit);
			
			//exit game
			Button t_exitButton = new Button("exit", "exit_selected", "exit_selected", "exit_selected", Vector2.Zero, "", "VerdanaBold", Color.White, Vector2.Zero);
			t_exitButton.m_clickEvent += new Button.clickDelegate(exitClick);
			m_buttons.AddLast(t_exitButton);
			GuiListFactory.setListPosition(m_buttons, new Vector2(Game.getInstance().getResolution().X / 2 - (t_newGame.getSize().X/2), Game.getInstance().getResolution().Y / 2));
			GuiListFactory.setTextOffset(m_buttons, new Vector2(20, 0));
			GuiListFactory.setButtonDistance(m_buttons, new Vector2(0, 60));
			//setting button
			m_buttons.AddLast(m_btnSettings = new Button("btn_settings", new Vector2(100, 100)));
			m_btnSettings.m_clickEvent += new Button.clickDelegate(settingsClick);

			m_buttons.ElementAt(0).setState(Button.State.Hover);

			m_menuSong = new Music("MenuSong");
			m_menuSong.play();
		}
		#endregion

		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{
			if (KeyboardHandler.keyClicked(Keys.Up))
			{
				moveCurrentHower(-1);
			}
			else if (KeyboardHandler.keyClicked(Keys.Down))
			{
				moveCurrentHower(+1);
			}
			else if (KeyboardHandler.keyClicked(Keys.Enter))
			{
				m_buttons.ElementAt(m_currentButton).setState(Button.State.Pressed);
				m_buttons.ElementAt(m_currentButton).invokeClickEvent();
			}
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

		public void moveCurrentHower(int a_move)
		{
			m_buttons.ElementAt(m_currentButton).setState(Button.State.Normal);
			m_currentButton += a_move;
			if (m_currentButton >= m_buttons.Count)
			{
				m_currentButton = 0;
			}
			else if (m_currentButton < 0)
			{
				m_currentButton = m_buttons.Count-1;
			}
			m_buttons.ElementAt(m_currentButton).setState(Button.State.Hover);
		}

		public void exitClick(Button a_b)
		{
			Music.stop();
			m_menuSong.dispose();
			Game.getInstance().Exit();
		}

		public void newGameClick(Button a_b)
		{
			Game.getInstance().setState(new HubMenu(m_menuSong));
			Game.getInstance().setProgress("temp.prog", false);
		}

		public void loadGameClick(Button a_b)
		{
			Game.getInstance().setState(new LoadAndSaveMenu(false, this, m_menuSong));
		}

		public void settingsClick(Button a_button)
		{
			Music.stop();
			m_menuSong.dispose();
			Game.getInstance().setState(new SettingsMenu());
		}
		#endregion
	}
}
