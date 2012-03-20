using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny.Events.Triggers
{
	class PlayerIsWithinRectangle : EventTrigger
	{
		private float m_x1;
		private float m_x2;
		private float m_y1;
		private float m_y2;

		public PlayerIsWithinRectangle(float a_x1, float a_y1, float a_x2, float a_y2)
		{
			m_x1 = Math.Min(a_x1, a_x2);
			m_x2 = Math.Max(a_x1, a_x2);
			m_y1 = Math.Min(a_y1, a_y2);
			m_y2 = Math.Max(a_y1, a_y2);
		}
		public override bool isTrue()
		{
			if (Game.getInstance().getState().getPlayer() == null)
			{
				return false;
			}
			float t_playerX = Game.getInstance().getState().getPlayer().getPosition().getGlobalX();
			float t_playerY = Game.getInstance().getState().getPlayer().getPosition().getGlobalY();

			return
				t_playerX > m_x1 &&
				t_playerX < m_x2 &&
				t_playerY > m_y1 &&
				t_playerY < m_y2;
		}
	}
}
