using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny.Events.Triggers
{
	class PlayerIsWithinCircle : EventTrigger
	{
		private Vector2 m_position;
		private float m_radie;

		public PlayerIsWithinCircle(Vector2 a_position, float a_radie)
		{
			if (a_position == null)
			{
				throw new ArgumentNullException();
			}
			if (a_radie <= 0)
			{
				throw new ArgumentException();
			}
			m_position = a_position;
			m_radie = a_radie;
		}
		public override bool isTrue()
		{
			return Game.getInstance().getState().getPlayer().getPosition().getDistanceTo(m_position) <= m_radie;
		}
	}
}
