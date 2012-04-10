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
		private string[]	m_levelList;
		private TextField	m_newLevelName;
		private Text		m_levelText;
		private Button		m_btnTFAccept;
		private TimeSpan	m_textTimeOut;
		#endregion

		#region Constructor & Load
		public override void load()
		{
			base.load();

			m_levelText		= new Text(new Vector2(405, 80), "New Level:", "VerdanaBold", Color.White, false);
			m_newLevelName	= new TextField(new Vector2(400, 100), 200, 32, true, true, true, 20);
			m_buttons.Add(m_btnTFAccept = new Button("btn_textfield_accept", new Vector2(600, 100)));
			m_btnTFAccept.m_clickEvent += new Button.clickDelegate(createNewLevel);

			try {
				m_levelList = Directory.GetFiles("Content//levels//");
			} catch (DirectoryNotFoundException) {
				System.IO.Directory.CreateDirectory("Content//levels//");
				return;
			}
			int t_count = 0;
			foreach (string t_level in m_levelList)
			{
				string[] t_splitPath = Regex.Split(t_level, "/");
				Button t_levelButton = new Button("btn_test_empty", "btn_test_empty", "btn_test_empty", "btn_test_empty", 
					new Vector2(20, 60 * t_count + 20), t_splitPath[t_splitPath.Length - 1], "VerdanaBold", Color.Black, new Vector2(10, 10));
				t_levelButton.m_clickEvent += new Button.clickDelegate(startLevelClick);
				m_buttons.Add(t_levelButton);
				t_count++;
			}
		}
		#endregion

		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{
			foreach (Button t_b in m_buttons)
				t_b.update();
			m_newLevelName.update(a_gameTime);
			if (a_gameTime.TotalGameTime > m_textTimeOut)
			{
				m_levelText.setText("New Level:");
				m_levelText.setColor(Color.White);
			}
			if (Game.keyClicked(Keys.Enter)	&& m_newLevelName.isWriting())
			{
				createNewLevel(m_btnTFAccept);
			}
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (Button t_b in m_buttons)
				t_b.draw(a_gameTime, a_spriteBatch);
			m_newLevelName.draw(a_gameTime);
			m_levelText.draw(a_gameTime);
		}
		#endregion

		#region Main Menu Methods (MMM...Bio)
		public void playClick(Button a_b)
		{
			Game.getInstance().setState(new GameState("Level3.txt"));
		}
		public void exitClick(Button a_b)
		{
			Game.getInstance().Exit();
		}
		public void startLevelClick(Button a_b)
		{
			Game.getInstance().setState(new GameState(a_b.getText()));
		}
		private void createNewLevel(Button a_button)
		{
			try
			{
				File.OpenRead("Content\\levels\\" + m_newLevelName.getText() + ".lvl");
				m_textTimeOut = Game.getInstance().getGameTime() + new TimeSpan(0, 0, 3);
				m_levelText.setColor(Color.Red);
				m_levelText.setText("Level already exists!");
			}
			catch (FileNotFoundException)
			{
				FileStream t_file = File.Create("Content\\levels\\" + m_newLevelName.getText() + ".lvl");
				Thread.Sleep(500);
				Game.getInstance().setState(new GameState(m_newLevelName.getText() + ".lvl"));
			}
		}
		#endregion
	}
}
