using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	class TextField {
		private Text	m_textToShow;
		private Box		m_box;
		private bool	m_writing;

		public TextField(Vector2 a_position, int a_width, int a_height) {
			m_textToShow	= new Text(a_position, "", "VerdanaBold", Color.Black, false);
			m_box			= new Box(a_position, a_width, a_height, Color.White, Color.Black, 2);
			m_writing		= false;
		}

		public void update() {
			if (m_writing) {
				
			}
		}

		public void draw() {
			m_textToShow.draw();
			m_box.draw();
		}
	}
}
