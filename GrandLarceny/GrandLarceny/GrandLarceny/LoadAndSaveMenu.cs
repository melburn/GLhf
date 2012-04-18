using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace GrandLarceny
{
	public class LoadAndSaveMenu : MenuState
	{
		private bool m_willSave;
		private States m_backState;
		public LoadAndSaveMenu(bool a_willSave, States a_backState)
		{
			m_willSave = a_willSave;
			m_backState = a_backState;

			string t_ext = ".prog";
			if (!Directory.Exists("Content//levels//"))
			{
				Directory.CreateDirectory("Content//levels//");
			}
			string[] t_saveFiles = Directory.GetFiles("Content//levels//");
			string[] t_progressFiles = new string[3];
			int t_index = 0;
			foreach (string t_s in t_saveFiles)
			{
				if (t_s.EndsWith(t_ext) && !(t_s.EndsWith("Checkpoint.prog")))
				{
					t_progressFiles[t_index] = t_s;
					t_index++;
				}
			}

			Button t_slot1 = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", Vector2.Zero, "Slot 1", "VerdanaBold", Color.White, Vector2.Zero);
			if (t_progressFiles[0] != null)
			{
				t_slot1.setText(t_progressFiles[0]);
			}
			m_buttons.AddLast(t_slot1);
			Button t_slot2 = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", Vector2.Zero, "Slot 2", "VerdanaBold", Color.White, Vector2.Zero);
			if (t_progressFiles[1] != null)
			{
				t_slot2.setText(t_progressFiles[1]);
			}
			m_buttons.AddLast(t_slot2);
			Button t_slot3 = new Button("btn_asset_list_normal", "btn_asset_list_hover", "btn_asset_list_pressed", "btn_asset_list_toggle", Vector2.Zero, "Slot 3", "VerdanaBold", Color.White, Vector2.Zero);
			if (t_progressFiles[2] != null)
			{
				t_slot3.setText(t_progressFiles[2]);
			}
			m_buttons.AddLast(t_slot3);
			
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
		}

		public override void load()
		{
 			 base.load();
			
			 GuiListFactory.setListPosition(m_buttons, new Vector2(Game.getInstance().getResolution().X/2 - (30 + 80), Game.getInstance().getResolution().Y/2));
			 GuiListFactory.setTextOffset(m_buttons, new Vector2(10, 0));
			 GuiListFactory.setButtonDistance(m_buttons, new Vector2(30, 60));
		}
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

		public void loadProgressClick(Button a_b)
		{
			Game.getInstance().setState(new HubMenu());
			Game.getInstance().setProgress(a_b.getText() + ".prog");
		}

		public void saveProgressClick(Button a_b)
		{
			Serializer.getInstance().saveGame(a_b.getText(), Game.getInstance().getProgress());
		}

	}
}
