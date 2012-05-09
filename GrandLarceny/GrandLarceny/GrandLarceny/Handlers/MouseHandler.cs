using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	public class MouseHandler
	{
		private static MouseState m_currentMouse;
		private static MouseState m_previousMouse;
		private static Vector2 m_worldMouse;

		public static void setCurrentMouse(MouseState a_mouseState)
		{
			m_currentMouse = a_mouseState;
			Camera t_camera = Game.getInstance().m_camera;
			float t_zoom = t_camera.getZoom();
			m_worldMouse = new Vector2(
				m_currentMouse.X / t_zoom + (int)t_camera.getPosition().getGlobalCartesian().X - ((Game.getInstance().getResolution().X / 2) / t_zoom) ,
				m_currentMouse.Y / t_zoom + (int)t_camera.getPosition().getGlobalCartesian().Y - ((Game.getInstance().getResolution().Y / 2) / t_zoom)
			);
		}

		public static void setPreviousMouse()
		{
			m_previousMouse = m_currentMouse;
		}

		public static Vector2 getCurPos()
		{
			return new Vector2(m_currentMouse.X, m_currentMouse.Y);
		}

		public static Vector2 getPrePos()
		{
			return new Vector2(m_previousMouse.X, m_previousMouse.Y);
		}

		public static bool lmbPressed()
		{
			return m_currentMouse.LeftButton == ButtonState.Pressed;
		}

		public static bool lmbDown()
		{
			return m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Released;
		}

		public static bool lmbUp()
		{
			return m_currentMouse.LeftButton == ButtonState.Released && m_previousMouse.LeftButton == ButtonState.Pressed;
		}

		public static bool rmbPressed()
		{
			return m_currentMouse.RightButton == ButtonState.Pressed;
		}

		public static bool rmbDown()
		{
			return m_currentMouse.RightButton == ButtonState.Pressed && m_previousMouse.RightButton == ButtonState.Released;
		}

		public static bool rmbUp()
		{
			return m_currentMouse.RightButton == ButtonState.Released && m_previousMouse.RightButton == ButtonState.Pressed;
		}

		public static bool mmbPressed()
		{
			return m_currentMouse.MiddleButton == ButtonState.Pressed;
		}

		public static bool mmbDown()
		{
			return m_currentMouse.MiddleButton == ButtonState.Pressed && m_previousMouse.MiddleButton == ButtonState.Released;
		}

		public static bool mmbUp()
		{
			return m_currentMouse.MiddleButton == ButtonState.Released && m_previousMouse.MiddleButton == ButtonState.Pressed;
		}

		public static bool scrollUp()
		{
			return m_currentMouse.ScrollWheelValue > m_previousMouse.ScrollWheelValue;
		}

		public static bool scrollDown()
		{
			return m_currentMouse.ScrollWheelValue < m_previousMouse.ScrollWheelValue;
		}

		public static int scrollDiff()
		{
			return m_currentMouse.ScrollWheelValue - m_previousMouse.ScrollWheelValue;
		}

		public static Vector2 worldMouse()
		{
			return m_worldMouse;
		}

		public static Vector2 getMouseCoords()
		{
			return new Vector2(m_currentMouse.X, m_currentMouse.Y);
		}
	}
}
