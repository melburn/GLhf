using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	class TextField : GuiObject
	{
		#region Members
		private Text m_textToShow;
		private Box m_box;
		private Line m_caret;
		private bool m_writing;
		private bool m_acceptLetters;
		private bool m_acceptNumbers;
		private bool m_acceptSpecials;
		private string m_currentLocale;
		private int m_maxLength;
		private Vector2 m_posV2;
		private Keys[] m_acptLetters = { 
			Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M,
			Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
			Keys.OemComma, Keys.OemPeriod, Keys.OemCloseBrackets, Keys.OemTilde, Keys.OemQuotes, Keys.OemMinus,
			Keys.OemQuestion, Keys.OemPlus, Keys.OemOpenBrackets, Keys.OemSemicolon
		};
		private Keys[] m_acptNumbers = {
			Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9
		};
		private Dictionary<Keys, TimeSpan> m_lastPressedKeys;
		private List<Keys> m_repeatKeys;
		private TimeSpan m_repeatTime;
		#endregion

		#region Constructor & Load
		public TextField(Vector2 a_position, int a_width, int a_height, bool a_acceptLetters, bool a_acceptNumbers, bool a_acceptSpecials, int a_maxLength)
			: base(a_position, "")
		{
			m_textToShow = new Text(a_position, new Vector2(4, 2), "", "VerdanaBold", Color.Black, false);
			m_box = new Box(a_position, a_width, a_height, Color.White, Color.Black, 2, false);
			m_caret = new Line(m_box.getPosition(), m_box.getPosition(), new Vector2(5, 3), new Vector2(5, a_height - 3), Color.Black, 1, true);
			m_writing = false;
			m_acceptLetters = a_acceptLetters;
			m_acceptNumbers = a_acceptNumbers;
			m_acceptSpecials = a_acceptSpecials;
			m_maxLength = a_maxLength;
			m_currentLocale = "euSv";
			m_posV2 = a_position;
		}

		public override void loadContent()
		{
			//base.loadContent();
			m_repeatKeys = new List<Keys>();
			m_repeatTime = new TimeSpan(0, 0, 0, 0, 500);
			m_lastPressedKeys = new Dictionary<Keys, TimeSpan>();
		}
		#endregion

		#region TextField Methods
		private bool contains(Keys a_key, Keys[] a_keyset)
		{
			foreach (Keys t_key in a_keyset)
			{
				if (t_key == a_key)
				{
					return true;
				}
			}
			return false;
		}

		public string getText()
		{
			return m_textToShow.getText();
		}

		public void setText(string a_string)
		{
			m_textToShow.setText(a_string);
		}

		public bool isWriting()
		{
			return m_writing;
		}

		public override Rectangle getBox()
		{
			return new Rectangle((int)m_posV2.X, (int)m_posV2.Y, (int)m_box.getWidth(), (int)m_box.getHeight());
		}

		public Vector2 getSize()
		{
			return new Vector2(m_box.getWidth(), m_box.getHeight());
		}

		public void setWrite(bool a_active)
		{
			m_writing = a_active;
		}

		public bool isVisible()
		{
			return m_visible;
		}
		#endregion

		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{
			if (MouseHandler.lmbDown())
			{
				if (getBox().Contains((int)MouseHandler.getCurPos().X, (int)MouseHandler.getCurPos().Y))
				{
					m_writing = true;
					m_lastPressedKeys = new Dictionary<Keys, TimeSpan>();
				}
				else
				{
					m_writing = false;
				}
			}

			if (m_writing)
			{
				foreach (KeyValuePair<Keys, TimeSpan> t_keyPair in m_lastPressedKeys)
				{
					if (!KeyboardHandler.isKeyPressed(t_keyPair.Key))
					{
						m_repeatKeys.Add(t_keyPair.Key);
					}
					else if (t_keyPair.Value + m_repeatTime < a_gameTime.TotalGameTime)
					{
						m_repeatKeys.Add(t_keyPair.Key);
					}
				}
				foreach (Keys t_key in m_repeatKeys)
				{
					m_lastPressedKeys.Remove(t_key);
				}
				m_repeatKeys.Clear();

				if (m_currentLocale.Equals("euSv"))
				{
					updateSweden(a_gameTime);
				}
				if (m_maxLength != 0 && m_textToShow.getText().Length > m_maxLength)
				{
					for (; m_maxLength < m_textToShow.getText().Length; )
					{
						m_textToShow.erase();
					}
				}
				else
				{
					while (m_textToShow.measureString().X > m_box.getWidth())
					{
						m_textToShow.erase();
					}
				}
				m_caret.setXOffset(m_textToShow.measureString().X + 5);
			}
		}

		public override void draw(GameTime a_gameTime)
		{
			if (!m_visible)
				return;
			m_textToShow.draw(a_gameTime);
			m_box.draw(a_gameTime);
			if (m_writing)
			{
				if (a_gameTime.TotalGameTime.Milliseconds - 500 < 0)
				{
					m_caret.setColor(Color.Transparent);
				}
				else
				{
					m_caret.setColor(Color.Black);
				}
				m_caret.draw();
			}
		}
		#endregion

		#region Swedish
		private void updateSweden(GameTime a_gameTime)
		{
			Keys[] t_keys = KeyboardHandler.getPressedKeys();
			foreach (Keys t_key in t_keys)
			{
				if (!m_lastPressedKeys.ContainsKey(t_key))
				{
					if (t_key == Keys.Back)
					{
						m_textToShow.erase();
					}
					else if (t_key == Keys.Space)
					{
						m_textToShow.addText(" ");
					}
					else if (m_acceptLetters && contains(t_key, m_acptLetters))
					{
						if (KeyboardHandler.isKeyPressed(Keys.LeftShift) || KeyboardHandler.isKeyPressed(Keys.RightShift))
						{
							switch (t_key)
							{
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
								default:
									m_textToShow.addText((char)t_key);
									break;
							}
						}
						else
						{
							switch (t_key)
							{
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
								default:
									m_textToShow.addText(char.ToLower((char)t_key));
									break;
							}
						}
					}
					else if (m_acceptSpecials || m_acceptNumbers)
					{
						if (m_acceptSpecials && (KeyboardHandler.isKeyPressed(Keys.LeftShift) || KeyboardHandler.isKeyPressed(Keys.RightShift)))
						{
							if (t_key == Keys.D0)
							{
								m_textToShow.addText("=");
							}
							else if (t_key == Keys.D1)
							{
								m_textToShow.addText("!");
							}
							else if (t_key == Keys.D2)
							{
								m_textToShow.addText("\"");
							}
							else if (t_key == Keys.D3)
							{
								m_textToShow.addText("#");
							}
							else if (t_key == Keys.D4)
							{
								m_textToShow.addText("¤");
							}
							else if (t_key == Keys.D5)
							{
								m_textToShow.addText("%");
							}
							else if (t_key == Keys.D6)
							{
								m_textToShow.addText("&");
							}
							else if (t_key == Keys.D7)
							{
								m_textToShow.addText("/");
							}
							else if (t_key == Keys.D8)
							{
								m_textToShow.addText("(");
							}
							else if (t_key == Keys.D9)
							{
								m_textToShow.addText(")");
							}
						}
						else
						{
							if (contains(t_key, m_acptNumbers))
							{
								string t_string = t_key.ToString().Replace("D", string.Empty);
								m_textToShow.addText(t_string);
							}
						}
					}
					if (!(KeyboardHandler.wasKeyPressed(t_key) && !m_lastPressedKeys.ContainsKey(t_key)))
					{
						m_lastPressedKeys.Add(t_key, a_gameTime.TotalGameTime);
					}
				}
			}
		}
		#endregion
	}
}
