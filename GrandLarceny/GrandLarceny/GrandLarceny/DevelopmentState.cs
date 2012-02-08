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
		private MouseState m_previousMouse;
		private MouseState m_currentMouse;
		private KeyboardState m_previousKeyboard;
		private KeyboardState m_currentKeyboard;

		private GameObject m_selectedObject;
		private int m_leveltoLoad;

		private SpriteFont m_testFont;
		private String m_selectedInfo;

		public DevelopmentState(int a_levelToLoad)
		{
			m_leveltoLoad = a_levelToLoad;
		}

		public override void load()
		{
			m_testFont = Game.getInstance().Content.Load<SpriteFont>("Fonts//Courier New");
			m_gameObjectList = Loader.getInstance().loadLevel(m_leveltoLoad);
			Game.getInstance().m_camera.setPosition(new Vector2(0, 0));
		}

		public override void update(GameTime a_gameTime)
		{
			m_currentKeyboard = Keyboard.GetState();
			m_currentMouse = Mouse.GetState();

			if (m_selectedObject != null)
			{
				m_selectedInfo = (m_selectedObject.getLeftPoint() / 72 + 0.5).ToString() + ", " + (m_selectedObject.getTopPoint() / 72 + 0.5).ToString();
			}
			else
			{
				m_selectedInfo = "Joxe!";
			}

			updateKeyboard();
			updateMouse();

			m_previousKeyboard = m_currentKeyboard;
			m_previousMouse = m_currentMouse;
		}

		private void updateKeyboard()
		{
			if (m_currentKeyboard.IsKeyDown(Keys.Right) || m_currentMouse.X > Game.getInstance().m_graphics.PreferredBackBufferWidth - 25)
				Game.getInstance().m_camera.move(new Vector2(10 / Game.getInstance().m_camera.getZoom(), 0));
			if (m_currentKeyboard.IsKeyDown(Keys.Left) || m_currentMouse.X < 25)
				Game.getInstance().m_camera.move(new Vector2(-10 / Game.getInstance().m_camera.getZoom(), 0));
			if (m_currentKeyboard.IsKeyDown(Keys.Up) || m_currentMouse.Y < 25)
				Game.getInstance().m_camera.move(new Vector2(0, -10 / Game.getInstance().m_camera.getZoom()));
			if (m_currentKeyboard.IsKeyDown(Keys.Down) || m_currentMouse.Y > Game.getInstance().m_graphics.PreferredBackBufferHeight - 25)
				Game.getInstance().m_camera.move(new Vector2(0, 10 / Game.getInstance().m_camera.getZoom()));
			if (m_currentKeyboard.IsKeyDown(Keys.Z))
				Game.getInstance().m_camera.zoomIn(0.1f);
			if (m_currentKeyboard.IsKeyDown(Keys.X))
				Game.getInstance().m_camera.zoomOut(0.1f);
		}

		private void updateMouse()
		{
			Vector2 t_worldMouse;
			t_worldMouse.X = 
				Mouse.GetState().X / Game.getInstance().m_camera.getZoom()
				+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().X
				- ((Game.getInstance().m_graphics.PreferredBackBufferWidth / 2) / Game.getInstance().m_camera.getZoom());
			t_worldMouse.Y = 
				Mouse.GetState().Y / Game.getInstance().m_camera.getZoom() 
				+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().Y
				- ((Game.getInstance().m_graphics.PreferredBackBufferHeight / 2) / Game.getInstance().m_camera.getZoom() + 72);

			if (m_currentMouse.LeftButton == ButtonState.Pressed 
				&& m_previousMouse.LeftButton == ButtonState.Pressed
				&& m_selectedObject != null)
			{
				updateMouseDrag(t_worldMouse);
			}
			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Released) 
			{
				if (m_selectedObject != null)
				{
					m_selectedObject.setColor(Color.White);
					m_selectedObject = null;
				}
				Rectangle t_mouseClick = new Rectangle((int)t_worldMouse.X, (int)t_worldMouse.Y, 1, 1);

				foreach (GameObject t_gameObject in m_gameObjectList)
				{
					if (t_mouseClick.Intersects(t_gameObject.getBox()))
					{
						m_selectedObject = t_gameObject;
						t_gameObject.setColor(Color.Yellow);
					}
				}
			}
		}

		private void updateMouseDrag(Vector2 a_worldMouse)
		{
			if (a_worldMouse.X % 72 >= 36)
			{
				a_worldMouse.X = a_worldMouse.X + (72 - (a_worldMouse.X % 72));	
			}
			else if (a_worldMouse.X % 72 < 36)
			{
				a_worldMouse.X = a_worldMouse.X - (a_worldMouse.X % 72);
			}

			if (a_worldMouse.Y % 72 >= 36)
			{
				a_worldMouse.Y = a_worldMouse.Y + (72 - (a_worldMouse.Y % 72));
			}
			else if (a_worldMouse.Y % 72 < 36)
			{
				a_worldMouse.Y = a_worldMouse.Y - (a_worldMouse.Y % 72);
			}
			
			m_selectedObject.getPosition().setX((a_worldMouse.X - (m_selectedObject.getImg().getSize().X / 2)) + 36);
			m_selectedObject.getPosition().setY((a_worldMouse.Y - (m_selectedObject.getImg().getSize().Y / 2)) + 36);
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			if (m_selectedObject != null)
			{
				a_spriteBatch.DrawString(m_testFont, m_selectedInfo, new Vector2(m_selectedObject.getRightPoint() + 20, m_selectedObject.getTopPoint()), Color.White);
			}
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
