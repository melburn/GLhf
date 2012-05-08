using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	public class KeyboardHandler {
		private static KeyboardState m_currentKeyboard;
		private static KeyboardState m_previousKeyboard;

		public static void setCurrentKeyboard(KeyboardState a_keyboard)
		{
			m_currentKeyboard = a_keyboard;
		}

		public static void setPreviousKeyboard()
		{
			m_previousKeyboard = m_currentKeyboard;
		}

		public static bool keyReleased(Keys k)
		{
			return m_currentKeyboard.IsKeyUp(k) && m_previousKeyboard.IsKeyDown(k);
		}

		public static bool isKeyPressed(Keys k)
		{
			return m_currentKeyboard.IsKeyDown(k);
		}

		public static bool wasKeyPressed(Keys key)
		{
			return m_previousKeyboard.IsKeyDown(key);
		}

		public static bool keyClicked(Keys a_key)
		{
			return m_currentKeyboard.IsKeyDown(a_key) && m_previousKeyboard.IsKeyUp(a_key);
		}

		public static bool ctrlMod()
		{
			return m_currentKeyboard.IsKeyDown(Keys.LeftControl) || m_currentKeyboard.IsKeyDown(Keys.RightControl);
		}

		public static bool shiftMod()
		{
			return m_currentKeyboard.IsKeyDown(Keys.LeftShift) || m_currentKeyboard.IsKeyDown(Keys.RightShift);
		}

		public static bool altMod()
		{
			return m_currentKeyboard.IsKeyDown(Keys.LeftAlt) || m_currentKeyboard.IsKeyDown(Keys.RightAlt);
		}

		public static Keys[] getPressedKeys()
		{
			return m_currentKeyboard.GetPressedKeys();
		}
	}
}
