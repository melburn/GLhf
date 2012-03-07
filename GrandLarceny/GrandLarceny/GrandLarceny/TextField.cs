using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class TextField {
		private Text		m_textToShow;
		private Rectangle	m_collisionBox;
		private bool		m_writing;

		public TextField(Vector2 a_position, int a_width, int a_height) {
			m_textToShow = new Text(a_position, "", "VerdanaBold", Color.Black, false);
			m_collisionBox = new Rectangle((int)a_position.X, (int)a_position.Y, a_width, a_height);
			m_writing = false;
		}
	}
}
