using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	public class LevelMenu : MenuState
	{
		#region Members
		private TextField m_newLevelName;
		private Text m_levelText;
		private Button m_btnTFAccept;
		private TextButton m_btnPlay;
		private TextButton m_btnDevelop;
		private TextButton m_btnExit;
		private Button chosenLevel;
		private TimeSpan m_textTimeOut;
		#endregion

		#region Constructor & Load

		public LevelMenu() : base()
		{
			
		}

		public override void load()
		{
			base.load();

			m_levelText = new Text(new Vector2(405, 80), "New Level:", "VerdanaBold", Color.White, false);
			m_newLevelName = new TextField(new Vector2(400, 100), 200, 32, true, true, true, 20);
			string[] t_ext = { ".lvl" };
			if (!Directory.Exists("Content//levels//CustomLevels//"))
			{
				System.IO.Directory.CreateDirectory("Content//levels//CustomLevels//");
			}
			string[] t_fileList = Directory.GetFiles("Content//levels//CustomLevels//");
			
			m_buttons = GuiListFactory.createListFromStringArray(t_fileList, t_ext, "btn_test_empty");
			GuiListFactory.setListPosition(m_buttons, new Vector2(25, 25));
			GuiListFactory.setTextOffset(m_buttons, new Vector2(10, 10));
			GuiListFactory.setButtonDistance(m_buttons, new Vector2(0, 60));

			foreach (Button t_button in m_buttons)
			{
				t_button.m_clickEvent += new Button.clickDelegate(toggleLevel);
			}

			m_buttons.AddLast(m_btnTFAccept = new Button("btn_textfield_accept", new Vector2(600, 100)));
			m_btnTFAccept.m_clickEvent += new Button.clickDelegate(createNewLevel);

			m_btnPlay = new TextButton(new Vector2(500, 200), "Start Level", "MotorwerkLarge", m_normal, m_hover, m_pressed, Color.Red);
			m_btnDevelop = new TextButton(new Vector2(500, 260), "Edit Level", "MotorwerkLarge", m_normal, m_hover, m_pressed, Color.Red);
			m_btnExit = new TextButton(new Vector2(20, Game.getInstance().getResolution().Y - 120), "Exit", "MotorwerkLarge");
			m_btnPlay.m_clickEvent += new TextButton.clickDelegate(startLevelClick);
			m_btnDevelop.m_clickEvent += new TextButton.clickDelegate(editLevelClick);
			m_btnExit.m_clickEvent += new TextButton.clickDelegate(exitClick);
		}
		#endregion

		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{
			if (chosenLevel != null)
			{
				m_btnPlay.update();
				m_btnDevelop.update();
			}

			m_newLevelName.update(a_gameTime);
			if (a_gameTime.TotalGameTime > m_textTimeOut)
			{
				m_levelText.setText("New Level:");
				m_levelText.setColor(Color.White);
			}
			if (KeyboardHandler.keyClicked(Keys.Enter) && m_newLevelName.isWriting())
			{
				createNewLevel(m_btnTFAccept);
			}
			m_btnExit.update();
			base.update(a_gameTime);
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			if (chosenLevel != null)
			{
				m_btnPlay.draw(a_gameTime, a_spriteBatch);
				m_btnDevelop.draw(a_gameTime, a_spriteBatch);
			}

			m_newLevelName.draw(a_gameTime);
			m_levelText.draw(a_gameTime);
			m_btnExit.draw(a_gameTime, a_spriteBatch);
			base.draw(a_gameTime, a_spriteBatch);
		}
		#endregion

		#region Main Menu Methods (MMM...Bio)
		public void exitClick(Button a_b)
		{
			Music.getInstance().stop();
			Game.getInstance().setState(new MainMenu());
		}

		public void startLevelClick(Button a_b)
		{
			Music.getInstance().stop();
			Game.getInstance().setState(new GameState("CustomLevels//" + chosenLevel.getText() + ".lvl"));
		}

		private void editLevelClick(Button a_button)
		{
			Music.getInstance().stop();
			Game.getInstance().setState(new DevelopmentState("CustomLevels//" + chosenLevel.getText() + ".lvl"));
		}

		private void createNewLevel(Button a_button)
		{
			String t_fileName = "Content\\levels\\CustomLevels\\" + m_newLevelName.getText() + ".lvl";

			if (File.Exists(t_fileName))
			{
				m_levelText.setText("Level already exists!");
				m_levelText.setColor(Color.Red);
				m_textTimeOut = Game.getInstance().getTotalGameTime() + new TimeSpan(0, 0, 3);
			}
			else
			{
				FileStream t_file = File.Create("Content\\levels\\CustomLevels\\" + m_newLevelName.getText() + ".lvl");
				t_file.Close();
				Game.getInstance().setState(new DevelopmentState("\\CustomLevels\\" + m_newLevelName.getText() + ".lvl"));
			}
		}

		private void toggleLevel(Button a_button)
		{
			foreach (Button t_button in m_buttons)
			{
				t_button.setState(Button.State.Normal);
			}
			a_button.setState(Button.State.Toggled);
			chosenLevel = a_button;
		}
		#endregion
	}
}
