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
	public class MainMenu : MenuState
	{
		private Button m_btnSettings;
		private int m_currentButton = 0;

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
				Button t_loadGame = new Button(Vector2.Zero, "continue", "continue_selected", "continue_selected", "continue_selected");
				t_loadGame.m_clickEvent += new Button.clickDelegate(loadGameClick);
				m_buttons.AddLast(t_loadGame);
			}*/
			//new Game

			Button t_newGame = new Button(Vector2.Zero, "newgame", "newgame_selected", "newgame_selected", "newgame_selected");
			t_newGame.m_clickEvent += new Button.clickDelegate(loadGameClick);
			m_buttons.AddLast(t_newGame);

			TextButton t_levelSelect = new TextButton(Vector2.Zero, "Level Select", "Motorwerk", Color.White, Color.Yellow, Color.Blue, Color.Red);
			//Button t_levelSelect = new Button(Vector2.Zero, "newgame", "newgame_selected", "newgame_selected", "newgame_selected");
			t_levelSelect.m_clickEvent += new TextButton.clickDelegate(levelSelectClick);
			m_buttons.AddLast(t_levelSelect);

			//new Game
			Button t_credit = new Button(Vector2.Zero, "credits", "credits_selected", "credits_selected", "credits_selected");
			//t_newGame.m_clickEvent += new Button.clickDelegate(newGameClick);
			m_buttons.AddLast(t_credit);
			
			//exit game
			Button t_exitButton = new Button(Vector2.Zero, "exit", "exit_selected", "exit_selected", "exit_selected");
			t_exitButton.m_clickEvent += new Button.clickDelegate(exitClick);
			m_buttons.AddLast(t_exitButton);
			GuiListFactory.setListPosition(m_buttons, new Vector2(Game.getInstance().getResolution().X / 2 - (t_newGame.getSize().X/2), Game.getInstance().getResolution().Y / 2));
			GuiListFactory.setButtonDistance(m_buttons, new Vector2(0, 60));
			//setting button
			m_buttons.AddLast(m_btnSettings = new Button("btn_settings", new Vector2(100, 100)));
			m_btnSettings.m_clickEvent += new Button.clickDelegate(settingsClick);

			m_buttons.ElementAt(0).setState(Button.State.Hover);

			if (Music.musicIsPlaying())
			{
				Music.stop();
			}
			Music.loadSong("MenuSong");
			Music.play("MenuSong");
		}
		#endregion

		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{
			if (KeyboardHandler.keyClicked(Keys.Up))
			{
				moveCurrentHover(-1);
			}
			else if (KeyboardHandler.keyClicked(Keys.Down))
			{
				moveCurrentHover(+1);
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

		public void moveCurrentHover(int a_move)
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
			Music.stop();
			Game.getInstance().setState(new SettingsMenu());
		}
		
		public void levelSelectClick(Button a_button)
		{
			Music.stop();
			Game.getInstance().setState(new LevelMenu());
		}
		#endregion
	}
}
