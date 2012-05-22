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

			Color t_normal		= new Color(187, 194, 195);
			Color t_hover		= new Color(255, 255, 255);
			Color t_pressed		= new Color(132, 137, 138);

			string t_ext = "Slot*";
			if (!Directory.Exists("Content//levels//"))
			{
				Directory.CreateDirectory("Content//levels//");
			}
			string[] t_saveFiles = Directory.GetFiles("Content//levels//", t_ext);
			if (t_saveFiles.Length > 0)
			{
				TextButton t_loadGame = new TextButton(Vector2.Zero, "Continue", "MotorwerkLarge", t_normal, t_hover, t_pressed, Color.Red);
				t_loadGame.m_clickEvent += new TextButton.clickDelegate(loadGameClick);
				m_buttons.AddLast(t_loadGame);
			}

			TextButton t_newGame = new TextButton(Vector2.Zero, "New Game", "MotorwerkLarge", t_normal, t_hover, t_pressed, Color.Red);
			t_newGame.m_clickEvent += new TextButton.clickDelegate(loadGameClick);
			m_buttons.AddLast(t_newGame);

			TextButton t_levelSelect = new TextButton(Vector2.Zero, "Level Select", "MotorwerkLarge", t_normal, t_hover, t_pressed, Color.Red);
			t_levelSelect.m_clickEvent += new TextButton.clickDelegate(levelSelectClick);
			m_buttons.AddLast(t_levelSelect);

			TextButton t_settings = new TextButton(Vector2.Zero, "Settings", "MotorwerkLarge", t_normal, t_hover, t_pressed, Color.Red);
			t_settings.m_clickEvent += new TextButton.clickDelegate(settingsClick);
			m_buttons.AddLast(t_settings);

			TextButton t_credit = new TextButton(Vector2.Zero, "Credits", "MotorwerkLarge", t_normal, t_hover, t_pressed, Color.Red);
			//t_newGame.m_clickEvent += new Button.clickDelegate(newGameClick);
			m_buttons.AddLast(t_credit);
			
			TextButton t_exitButton = new TextButton(Vector2.Zero, "Exit", "MotorwerkLarge", t_normal, t_hover, t_pressed, Color.Red);
			t_exitButton.m_clickEvent += new TextButton.clickDelegate(exitClick);
			m_buttons.AddLast(t_exitButton);
			GuiListFactory.setListPosition(m_buttons, new Vector2(15, Game.getInstance().getResolution().Y / 2));
			GuiListFactory.setButtonDistance(m_buttons, new Vector2(0, 60));

			m_buttons.ElementAt(0).setState(Button.State.Hover);

			if (Music.getInstance().musicIsPlaying())
			{
				Music.getInstance().stop();
			}
			Music.getInstance().loadSong("MenuSong");
			Music.getInstance().play("MenuSong");
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
			Music.getInstance().stop();
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
			Music.getInstance().stop();
			Game.getInstance().setState(new SettingsMenu());
		}
		
		public void levelSelectClick(Button a_button)
		{
			Music.getInstance().stop();
			Game.getInstance().setState(new LevelMenu());
		}
		#endregion
	}
}
