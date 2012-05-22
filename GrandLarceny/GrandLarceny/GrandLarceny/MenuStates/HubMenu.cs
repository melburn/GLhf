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
	public class HubMenu : MenuState
	{
		#region Members
		private TextField m_newLevelName;
		private Text m_levelText;
		private Button m_btnTFAccept;
		private TimeSpan m_textTimeOut;
		private int m_currentButton = 0;

		private ParseState m_currentParse;

		private Texture2D m_bakimg;

		private enum ParseState
		{
			Settings
		}
		#endregion

		#region Constructor & Load

		public HubMenu()
		{
			
		}

		public override void load()
		{
			base.load();
			m_bakimg = Game.getInstance().Content.Load<Texture2D>("Images//Automagi//karta");

			//m_levelText = new Text(new Vector2(405, 80), "New Level:", "VerdanaBold", Color.White, false);
			//m_newLevelName = new TextField(new Vector2(400, 100), 200, 32, true, true, true, 20);
			string[] t_ext = { ".lvl" };
			if (!Directory.Exists("Content//levels//"))
			{
				System.IO.Directory.CreateDirectory("Content//levels//");
			}
			string[] t_fileList = Directory.GetFiles("Content//levels//");
			
			m_buttons = GuiListFactory.createListFromStringArray(t_fileList, t_ext, "pin");
			GuiListFactory.setListPosition(m_buttons, new Vector2(55, 55));
			GuiListFactory.setTextOffset(m_buttons, new Vector2(10, -20));
			GuiListFactory.setButtonDistance(m_buttons, new Vector2(200, 30));

			LinkedList<string> t_levelLocked = getLockedLevels();

			foreach (Button t_button in m_buttons)
			{

				if (t_levelLocked.Contains(t_button.getText()))
				{
					t_button.setState(Button.State.Toggled);
				}
				else
				{
					t_button.m_clickEvent += new Button.clickDelegate(startLevelClick);
				}
			}

			//m_buttons.AddLast(m_btnTFAccept = new Button("btn_textfield_accept", new Vector2(600, 100)));
			//m_btnTFAccept.m_clickEvent += new Button.clickDelegate(createNewLevel);
			/*Button t_saveProgressButton = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", new Vector2(500, 400), "Save Game", "VerdanaBold", Color.White, new Vector2(10, 0));
			t_saveProgressButton.m_clickEvent += new Button.clickDelegate(saveProgressClick);
			m_buttons.AddLast(t_saveProgressButton);*/

		}
		#endregion

		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{

			if (KeyboardHandler.keyClicked(Keys.Left))
			{
				moveCurrentHower(-1);
			}
			else if (KeyboardHandler.keyClicked(Keys.Right))
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
			/*m_newLevelName.update(a_gameTime);
			if (a_gameTime.TotalGameTime > m_textTimeOut)
			{
				m_levelText.setText("New Level:");
				m_levelText.setColor(Color.White);
			}
			if (KeyboardHandler.keyClicked(Keys.Enter) && m_newLevelName.isWriting())
			{
				createNewLevel(m_btnTFAccept);
			}
			 */
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (Button t_b in m_buttons)
				t_b.draw(a_gameTime, a_spriteBatch);
			//m_newLevelName.draw(a_gameTime);
			//m_levelText.draw(a_gameTime);

			Rectangle t_dest = Game.getInstance().m_camera.getRectangle();
			t_dest.Width /= 2;
			t_dest.X += t_dest.Width / 2;
			t_dest.Height /= 2;
			t_dest.Y += t_dest.Height / 2;
			a_spriteBatch.Draw(m_bakimg, t_dest, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1f);
		}
		#endregion

		#region Main Menu Methods (MMM...Bio)
		public void playClick(Button a_b)
		{
			Music.getInstance().stop();
			Game.getInstance().setState(new GameState("Level3.txt"));
		}

		public void exitClick(Button a_b)
		{
			Music.getInstance().stop();
			Game.getInstance().Exit();
		}

		public void startLevelClick(Button a_b)
		{
			Music.getInstance().stop();
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

		public void saveProgressClick(Button a_b)
		{
			Game.getInstance().setState(new LoadAndSaveMenu(true, this));

		}

		private LinkedList<string> getLockedLevels()
		{
			LinkedList<string> t_LockedLevel = new LinkedList<string>();
			string[] t_rawLevelLine = System.IO.File.ReadAllLines("Content\\levels\\LevelRequirement.txt");
			char[] t_splitter = { ':' };
			foreach (string t_level in t_rawLevelLine)
			{
				string[] t_levelAndReq = t_level.Split(t_splitter);
				int t_levelLocked = 1;
				for (int i = 1; i < t_levelAndReq.Length; ++i)
				{
					if (Game.getInstance().getProgress().hasClearedLevel(t_levelAndReq[i]))
					{
						t_levelLocked++;
					}
				}
				if (t_levelAndReq.Length > t_levelLocked)
				{
					t_LockedLevel.AddLast(t_levelAndReq[0]);
				}
			}
			return t_LockedLevel;
		}

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
				m_currentButton = m_buttons.Count - 1;
			}
			if (!m_buttons.ElementAt(m_currentButton).hasEvent())
			{
				m_currentButton += a_move;
				if (m_currentButton >= m_buttons.Count)
				{
					m_currentButton = 0;
				}
				else if (m_currentButton < 0)
				{
					m_currentButton = m_buttons.Count - 1;
				}
			}
			m_buttons.ElementAt(m_currentButton).setState(Button.State.Hover);
		}
		#endregion
	}
}
