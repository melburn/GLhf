using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	class TextField : GuiObject {
		private Text	m_textToShow;
		private Box		m_box;
		private bool	m_writing;
		private bool	m_acceptLetters;
		private bool	m_acceptNumbers;
		private Keys[]	m_acptLetters = { 
			Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
			Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
			Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
		};
		private Keys[]	m_acptNumers = {
			Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
		};

		public TextField(Vector2 a_position, int a_width, int a_height, bool a_acceptLetters, bool a_acceptNumbers) 
			: base(a_position, "")
		{
			m_textToShow	= new Text(a_position, "", "VerdanaBold", Color.Black, false);
			m_box			= new Box(a_position, a_width, a_height, Color.White, Color.Black, 2, false);
			m_writing		= false;
			m_acceptLetters	= a_acceptLetters;
			m_acceptNumbers = a_acceptNumbers;
		}

		public override void update() {
			if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
				if (m_box.contains(((DevelopmentState)Game.getInstance().getState()).calculateWorldMouse())) {
					m_writing = true;
				} else {
					m_writing = false;
				}
			}
			if (m_writing) {
				Keys[] t_keys = Game.getInstance().getCurrentKeyboard().GetPressedKeys();
				foreach (Keys t_key in t_keys) {
					if (t_key >= Keys.A && t_key <= Keys.Z && m_acceptLetters) {
						if (Game.getInstance().getCurrentKeyboard().IsKeyDown(Keys.LeftShift) || Game.getInstance().getCurrentKeyboard().IsKeyDown(Keys.RightShift)) {
							m_textToShow.addText(t_keys.ToString().ToCharArray()[0]);
						} else {
							m_textToShow.addText((t_keys.ToString().ToLower()).ToCharArray()[0]);							
						}
					}
					if (t_key >= Keys.D1 && t_key <= Keys.D0 && m_acceptNumbers) {
						m_textToShow.addText(t_key.ToString().ToCharArray()[1]);
					}
				}
			}
		}

		public override void draw(GameTime a_gameTime) {
			m_textToShow.draw(a_gameTime);
			m_box.draw();
		}
	}
}
