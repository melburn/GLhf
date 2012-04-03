﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny.Events.Triggers
{
	[Serializable()]
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

		public override string ToString()
		{
			return "Player within Rectangle";
		}

		public void getRectangle(Line[] a_lines)
		{
			if (a_lines == null)
			{
				throw new ArgumentNullException();
			}
			else if (a_lines.Length != 4)
			{
				throw new ArgumentException();
			}
			else
			{
				setLineElement(a_lines, 0, new Vector2(m_x1, m_y1), new Vector2(m_x2, m_y1));
				setLineElement(a_lines, 1, new Vector2(m_x1, m_y1), new Vector2(m_x1, m_y2));
				setLineElement(a_lines, 2, new Vector2(m_x2, m_y1), new Vector2(m_x2, m_y2));
				setLineElement(a_lines, 3, new Vector2(m_x1, m_y2), new Vector2(m_x2, m_y2));
			}
		}

		private void setLineElement(Line[] a_lines, int a_index, Vector2 a_start, Vector2 a_end)
		{
			if (a_lines[a_index] == null)
			{
				a_lines[a_index] = new Line(new CartesianCoordinate(a_start), new CartesianCoordinate(a_end), Vector2.Zero, Vector2.Zero, Color.MintCream, 2, true);
			}
			else
			{
				a_lines[a_index].setStartPoint(a_start);
				a_lines[a_index].setEndpoint(a_end);
			}
		}
	}
}
