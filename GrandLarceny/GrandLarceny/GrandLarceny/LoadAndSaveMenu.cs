using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	public class LoadAndSaveMenu : MenuState
	{
		private bool m_willSave;
		private States m_backState;
		private TextField m_newSaveName;
		private string m_saveTo;

		private Music m_menuSong;

		public LoadAndSaveMenu(bool a_willSave, States a_backState)
		{
			m_willSave = a_willSave;
			m_backState = a_backState;
			m_newSaveName = new TextField(new Vector2(400, 100), 200, 32, true, true, true, 20);
			m_newSaveName.setVisible(false);

			Button t_slot1 = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", new Vector2(450, 350), "Slot 1", "VerdanaBold", Color.White, new Vector2(10, 0));
			m_buttons.AddLast(t_slot1);
			Button t_slot2 = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", new Vector2(500, 410), "Slot 2", "VerdanaBold", Color.White, new Vector2(10, 0));
			m_buttons.AddLast(t_slot2);
			Button t_slot3 = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", new Vector2(550, 470), "Slot 3", "VerdanaBold", Color.White, new Vector2(10, 0));
			m_buttons.AddLast(t_slot3);

			updateSaveText();

			foreach(Button f_b in m_buttons)
			{
				if (m_willSave)
				{
					f_b.m_clickEvent += new Button.clickDelegate(saveProgressClick);
				}
				else
				{
					f_b.m_clickEvent += new Button.clickDelegate(loadProgressClick);
				}	
			}

			Button t_slot4 = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle",new Vector2(900, 550), "Back!!", "VerdanaBold", Color.White,new Vector2(10,0));
			m_buttons.AddLast(t_slot4);
			t_slot4.m_clickEvent += new Button.clickDelegate(backTo);
		}

		private void updateSaveText()
		{
			string t_ext = "Slot*";
			if (!Directory.Exists("Content//levels//"))
			{
				Directory.CreateDirectory("Content//levels//");
			}
			string[] t_saveFiles = Directory.GetFiles("Content//levels//", t_ext);
			string[] t_progressFiles = new string[3];

			if (t_saveFiles.Contains("Content//levels//Slot 1.prog"))
			{
				t_progressFiles[0] = "Slot 1.prog";
			}
			if (t_saveFiles.Contains("Content//levels//Slot 2.prog"))
			{
				t_progressFiles[1] = "Slot 2.prog";
			}
			if (t_saveFiles.Contains("Content//levels//Slot 3.prog"))
			{
				t_progressFiles[2] = "Slot 3.prog";
			}
			
			for (int i = 0; i < 3; ++i)
			{
				if (t_progressFiles[i] != null)
				{
					Progress tProgg = Serializer.getInstance().loadProgress(Serializer.getInstance().getFileToStream(t_progressFiles[i], false));
					m_buttons.ElementAt(i).setText(tProgg.getUserName());
				}
			}
			
		}

		public override void update(GameTime a_gameTime)
		{
			foreach (Button t_b in m_buttons)
				t_b.update();
			if (m_newSaveName.isVisible())
			{
				m_newSaveName.update(a_gameTime);
				if (m_newSaveName.isWriting() && KeyboardHandler.keyClicked(Keys.Enter))
				{
					Game.getInstance().getProgress().setUserName(m_newSaveName.getText());
					Serializer.getInstance().saveGame(Serializer.getInstance().getFileToStream(m_saveTo, true), Game.getInstance().getProgress());
					m_saveTo = null;
					m_newSaveName.setVisible(false);
					updateSaveText();
				}
				
			}
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (Button t_b in m_buttons)
				t_b.draw(a_gameTime, a_spriteBatch);

			if (m_newSaveName.isVisible())
			{
				m_newSaveName.draw(a_gameTime);
			}
		}

		public void loadProgressClick(Button a_b)
		{
			for (int i = 0; i < 3; ++i)
			{
				if (m_buttons.ElementAt(i) == a_b)
				{
					if (!File.Exists("Content//levels//Slot " + (i+1) + ".prog"))
					{
						m_newSaveName.getPosition().setGlobalX(a_b.getPosition().getGlobalX());
						m_newSaveName.getPosition().setGlobalY(a_b.getPosition().getGlobalY());
						m_newSaveName.setVisible(true);
						m_newSaveName.setText("Enter the name");
						m_newSaveName.setWrite(true);
						m_saveTo = "Slot " + (i + 1) + ".prog";
					}
					else
					{
						Game.getInstance().setProgress("Slot "+(i+1)+".prog", false);
						Game.getInstance().setState(new HubMenu());
					}
				}
			}
		}

		public void saveProgressClick(Button a_b)
		{
			int t_index = 4;
			for (int i = 0; i < 3; ++i)
			{
				if (m_buttons.ElementAt(i) == a_b)
				{
					t_index = i+1;
				}
			}

			if (!File.Exists("Content//levels//Slot "+t_index+".prog"))
			{
				m_newSaveName.getPosition().setGlobalX(a_b.getPosition().getGlobalX());
				m_newSaveName.getPosition().setGlobalY(a_b.getPosition().getGlobalY());
				m_newSaveName.setVisible(true);
				m_newSaveName.setText("Enter the name");
				m_newSaveName.setWrite(true);
				m_saveTo = "Slot "+(t_index)+".prog";
			}
			else
			{
				Serializer.getInstance().saveGame(Serializer.getInstance().getFileToStream(a_b.getText(), true), Game.getInstance().getProgress());
			}
		}

		public void backTo(Button a_b)
		{
			Game.getInstance().setState(m_backState);
		}

	}
}