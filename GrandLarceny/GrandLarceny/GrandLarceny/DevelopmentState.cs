using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	class DevelopmentState : States
	{
		private LinkedList<GameObject> m_gameObjectList;
		private LinkedList<GameObject> m_killList = new LinkedList<GameObject>();
		private MouseState m_previousMouse;
		private MouseState m_currentMouse;
		private KeyboardState m_previousKeyboard;
		private KeyboardState m_currentKeyboard;

		private GameObject m_selectedObject;
		private int m_leveltoLoad;

		public DevelopmentState(int a_levelToLoad) {
			m_leveltoLoad = a_levelToLoad;
		}

		public override void load()
		{
			m_gameObjectList = Loader.getInstance().loadLevel(m_leveltoLoad);
			Game.getInstance().m_camera.setPosition(new Vector2(0, 0));
		}

		public override void update(GameTime a_gameTime)
		{
			m_currentKeyboard = Keyboard.GetState();
			m_currentMouse = Mouse.GetState();

			updateKeyboard();
			updateMouse();

			m_previousKeyboard = m_currentKeyboard;
			m_previousMouse = m_currentMouse;
		}

		private void updateKeyboard()
		{
			if (m_currentKeyboard.IsKeyDown(Keys.Right) || m_currentMouse.X > Game.getInstance().m_graphics.PreferredBackBufferWidth - 25)
				Game.getInstance().m_camera.move(new Vector2(10, 0));
			if (m_currentKeyboard.IsKeyDown(Keys.Left) || m_currentMouse.X < 25)
				Game.getInstance().m_camera.move(new Vector2(-10, 0));
			if (m_currentKeyboard.IsKeyDown(Keys.Up) || m_currentMouse.Y < 25)
				Game.getInstance().m_camera.move(new Vector2(0, -10));
			if (m_currentKeyboard.IsKeyDown(Keys.Down) || m_currentMouse.Y > Game.getInstance().m_graphics.PreferredBackBufferHeight - 25)
				Game.getInstance().m_camera.move(new Vector2(0, 10));
		}

		private void updateMouse() {
			if (m_currentMouse.LeftButton == ButtonState.Pressed 
				&& m_previousMouse.LeftButton == ButtonState.Pressed
				&& m_selectedObject != null) {
				updateMouseDrag();
			}
			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Released) 
			{
				if (m_selectedObject != null) {
					m_selectedObject.setColor(Color.White);
					m_selectedObject = null;
				}
				Rectangle t_mouseClick = new Rectangle(
					Mouse.GetState().X + (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().X - (Game.getInstance().m_graphics.PreferredBackBufferWidth / 2),
					Mouse.GetState().Y + (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().Y - (Game.getInstance().m_graphics.PreferredBackBufferHeight / 2 + 72), 
					1, 1
				);

				foreach (GameObject t_gameObject in m_gameObjectList)
				{
					if (t_mouseClick.Intersects(t_gameObject.getBox()))
					{
						m_selectedObject = t_gameObject;
						t_gameObject.setColor(Color.Yellow);
						break;	
					}
				}
			}
		}

		private void updateMouseDrag() {
			Vector2 t_v2 = new Vector2(
				m_currentMouse.X + (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().X - (Game.getInstance().m_graphics.PreferredBackBufferWidth / 2)
				, m_currentMouse.Y + (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().Y - (Game.getInstance().m_graphics.PreferredBackBufferHeight / 2 + 72)
			);
			
			if (t_v2.X % 72 > 36) {
				t_v2.X = t_v2.X + (72 - (t_v2.X % 72));	
			} else if (t_v2.Y % 72 < 36) {
				t_v2.X = t_v2.X - (t_v2.X % 72);
			} else {
				;
			}

			if (t_v2.Y % 72 > 36) {
				t_v2.Y = t_v2.Y + (72 - (t_v2.Y % 72));
			} else if (t_v2.Y % 72 < 36) {
				t_v2.Y = t_v2.Y - (t_v2.Y % 72);
			} else {
				;
			}
			
			m_selectedObject.setLeftPoint(t_v2.X - (m_selectedObject.getImg().getSize().X / 2));
			m_selectedObject.setTopPoint(t_v2.Y - (m_selectedObject.getImg().getSize().Y / 2));
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (GameObject t_gameObject in m_gameObjectList)
			{
				t_gameObject.draw(a_gameTime);
			}
		}

		public override void setPlayer(Player a_player)
		{
			
		}
	}
}
