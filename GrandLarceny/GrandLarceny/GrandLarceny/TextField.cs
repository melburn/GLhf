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
		private bool	m_acceptSpecials;
		private string	m_currentLocale;
		private int		m_maxLength;
		private Keys[]	m_acptLetters = { 
			Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
			Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
			Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
		};
		private Keys[]	m_acptNumbers = {
			Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
		};
		private Dictionary<Keys, TimeSpan> m_lastPressedKeys;
		private List<Keys> m_repeatKeys;
		private TimeSpan m_repeatTime;

		public TextField(Vector2 a_position, int a_width, int a_height, bool a_acceptLetters, bool a_acceptNumbers, bool a_acceptSpecials, int a_maxLength) 
			: base(a_position, "")
		{
			m_textToShow	= new Text(a_position, new Vector2(4, 2), "", "VerdanaBold", Color.Black, false);
			m_box			= new Box(a_position, a_width, a_height, Color.White, Color.Black, 2, false);
			m_writing		= false;
			m_acceptLetters	= a_acceptLetters;
			m_acceptNumbers = a_acceptNumbers;
			m_acceptSpecials = a_acceptSpecials;
			m_maxLength = a_maxLength;
			m_currentLocale = "euSv";
		}

		public override void loadContent()
		{
			base.loadContent();
			m_repeatKeys = new List<Keys>();
			m_repeatTime = new TimeSpan(0, 0, 0, 0, 500);
			m_lastPressedKeys = new Dictionary<Keys,TimeSpan>();
		}

		private bool contains(Keys a_key, Keys[] a_keyset) {
			foreach (Keys t_key in a_keyset) {
				if (t_key == a_key) {
					return true;
				}
			}
			return false;
		}

		public override void update(GameTime a_gameTime) {
			if (Game.m_currentMouse.LeftButton == ButtonState.Pressed && Game.m_previousMouse.LeftButton == ButtonState.Released) {
				if (m_box.contains(Game.getInstance().getState().calculateWorldMouse())) {
					m_writing = true;
					m_lastPressedKeys = new Dictionary<Keys,TimeSpan>();
					m_box.setLineColor(Color.Orange);
				} else {
					m_writing = false;
					m_box.setLineColor(Color.Black);
				}
			}
			if (m_writing) {
				foreach (KeyValuePair<Keys, TimeSpan> t_keyPair in m_lastPressedKeys) {
					if (!Game.isKeyPressed(t_keyPair.Key)) {
						m_repeatKeys.Add(t_keyPair.Key);
					} else if (t_keyPair.Value + m_repeatTime < a_gameTime.TotalGameTime) {
						m_repeatKeys.Add(t_keyPair.Key);
					}
				}
				foreach (Keys t_key in m_repeatKeys) {
					m_lastPressedKeys.Remove(t_key);
				}
				m_repeatKeys.Clear();
				if (m_currentLocale.Equals("euSv")) {
					updateSweden(a_gameTime);
				}
				if (m_maxLength != 0 && m_textToShow.getText().Length > m_maxLength) {
					
				} else if (m_textToShow.getText().Length * 10 > m_box.getWidth() + (m_textToShow.getText().Length)) {
					m_textToShow.erase();
				}
			}
		}

		public override void draw(GameTime a_gameTime) {
			m_textToShow.draw(a_gameTime);
			m_box.draw();
		}

		private void updateSweden(GameTime a_gameTime) {
			Keys[] t_keys = Game.m_currentKeyInput.GetPressedKeys();
			foreach (Keys t_key in t_keys) {
				if (!m_lastPressedKeys.ContainsKey(t_key)) {
					if (t_key == Keys.Back) {
						m_textToShow.erase();
					} else if (contains(t_key, m_acptLetters) && m_acceptLetters) {
						if (Game.isKeyPressed(Keys.LeftShift) || Game.isKeyPressed(Keys.RightShift)) {
							m_textToShow.addText((char)t_key);
						} else {
							m_textToShow.addText(char.ToLower((char)t_key));
						}
					} else if (contains(t_key, m_acptNumbers)) {
						if (m_acceptSpecials && (Game.isKeyPressed(Keys.LeftShift) || Game.isKeyPressed(Keys.RightShift))) {
							switch (t_key) {
								case Keys.D0:
									m_textToShow.addText("=");
									break;
								case Keys.D1:
									m_textToShow.addText("!");
									break;
								case Keys.D2:
									m_textToShow.addText("\"");
									break;
								case Keys.D3:
									m_textToShow.addText("#");
									break;
								case Keys.D4:
									m_textToShow.addText("¤");
									break;
								case Keys.D5:
									m_textToShow.addText("%");
									break;
								case Keys.D6:
									m_textToShow.addText("&");
									break;
								case Keys.D7:
									m_textToShow.addText("/");
									break;
								case Keys.D8:
									m_textToShow.addText("(");
									break;
								case Keys.D9:
									m_textToShow.addText(")");
									break;
							}
						} else {
							string t_string = t_key.ToString().Replace("D", string.Empty);
							m_textToShow.addText(t_string);
						}
					} else if (t_key == Keys.Space) {
						m_textToShow.addText(" ");
					} else {
						if (Game.isKeyPressed(Keys.LeftShift) || Game.isKeyPressed(Keys.RightShift)) {
							switch (t_key) {
								case Keys.OemSemicolon:
									m_textToShow.addText("^");
									break;
								case Keys.OemCloseBrackets:
									m_textToShow.addText("Å");
									break;
								case Keys.OemTilde:
									m_textToShow.addText("Ö");
									break;
								case Keys.OemQuotes:
									m_textToShow.addText("Ä");
									break;
								case Keys.OemComma:
									m_textToShow.addText(";");
									break;
								case Keys.OemPeriod:
									m_textToShow.addText(":");
									break;
								case Keys.OemMinus:
									m_textToShow.addText("_");
									break;
								case Keys.OemQuestion:
									m_textToShow.addText("*");
									break;
								case Keys.OemPlus:
									m_textToShow.addText("?");
									break;
								case Keys.OemOpenBrackets:
									m_textToShow.addText("`");
									break;
							}
						} else {
							switch (t_key) {
								case Keys.OemSemicolon:
									m_textToShow.addText("¨");
									break;
								case Keys.OemCloseBrackets:
									m_textToShow.addText("å");
									break;
								case Keys.OemTilde:
									m_textToShow.addText("ö");
									break;
								case Keys.OemQuotes:
									m_textToShow.addText("ä");
									break;
								case Keys.OemComma:
									m_textToShow.addText(",");
									break;
								case Keys.OemPeriod:
									m_textToShow.addText(".");
									break;
								case Keys.OemPipe:
									m_textToShow.addText("'");
									break;
								case Keys.OemMinus:
									m_textToShow.addText("-");
									break;
								case Keys.OemQuestion:
									m_textToShow.addText("'");
									break;
								case Keys.OemPlus:
									m_textToShow.addText("+");
									break;
								case Keys.OemOpenBrackets:
									m_textToShow.addText("´");
									break;
							}									
						}
					}
					if(!(Game.wasKeyPressed(t_key) && !m_lastPressedKeys.ContainsKey(t_key)))
						m_lastPressedKeys.Add(t_key, a_gameTime.TotalGameTime);
				}
			}
		}
	}
}
