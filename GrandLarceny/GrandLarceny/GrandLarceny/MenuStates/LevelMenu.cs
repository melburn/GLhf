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
		}
		#endregion

		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{
			foreach (Button t_b in m_buttons)
			{
				t_b.update();
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
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (Button t_b in m_buttons)
			{
				t_b.draw(a_gameTime, a_spriteBatch);
			}
			m_newLevelName.draw(a_gameTime);
			m_levelText.draw(a_gameTime);
		}
		#endregion

		#region Main Menu Methods (MMM...Bio)
		public void playClick(Button a_b)
		{
			Music.stop();
			Game.getInstance().setState(new GameState("Level3.txt"));
		}

		public void exitClick(Button a_b)
		{
			Music.stop();
			Game.getInstance().Exit();
		}

		public void startLevelClick(Button a_b)
		{
			Music.stop();
			Game.getInstance().setState(new GameState(a_b.getText() + ".lvl"));
		}

		private void createNewLevel(Button a_button)
		{
			String t_fileName = "Content\\levels\\" + m_newLevelName.getText() + ".lvl";

			if (File.Exists(t_fileName))
			{
				m_levelText.setText("Level already exists!");
				m_levelText.setColor(Color.Red);
				m_textTimeOut = Game.getInstance().getTotalGameTime() + new TimeSpan(0, 0, 3);
			}
			else
			{
				FileStream t_file = File.Create("Content\\levels\\" + m_newLevelName.getText() + ".lvl");
				t_file.Close();
				Game.getInstance().setState(new DevelopmentState(m_newLevelName.getText() + ".lvl"));
			}
		}

		private void toggleLevel(Button a_button)
		{
			foreach (Button t_button in m_buttons)
			{

			}
			a_button.setState(Button.State.Toggled);
		}
		#endregion
	}
}
