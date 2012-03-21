using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	class CutsceneDevelopment : States
	{
		private DevelopmentState m_backState;

		private LinkedList<Button> m_buttonList;

		public CutsceneDevelopment()
		{
			m_buttonList = new LinkedList<Button>();
			Button t_buttonToAdd = new Button("Images//GUI//dev_bg_info", new Vector2(100, 100), "I DONT WANNA DO NO MORE EVENTS PLZ", null, Color.Red, new Vector2(10, 10));
			t_buttonToAdd.m_clickEvent += new Button.clickDelegate(exitState);
			m_buttonList.AddLast(t_buttonToAdd);
			
		}

		public override void update(GameTime a_gameTime)
		{
			foreach (Button t_b in m_buttonList)
			{
				t_b.update();
			}
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			m_backState.draw(a_gameTime, a_spriteBatch);
			foreach (Button t_b in m_buttonList)
			{
				t_b.draw(a_gameTime, a_spriteBatch);
			}
		}


		//knappmetoder

		public void exitState(Button a_care)
		{
			Game.getInstance().setState(m_backState);
		}
	}
}
