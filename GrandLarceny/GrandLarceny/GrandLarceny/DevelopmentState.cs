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
		private Vector2 m_worldMouse;

		private State m_itemToCreate;
		private enum State
		{
			Platform,
			Background,
			Ladder,
			None
		}

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
			
			m_worldMouse.X = 
				Mouse.GetState().X / Game.getInstance().m_camera.getZoom()
				+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().X
				- ((Game.getInstance().m_graphics.PreferredBackBufferWidth / 2) / Game.getInstance().m_camera.getZoom());
			m_worldMouse.Y = 
				Mouse.GetState().Y / Game.getInstance().m_camera.getZoom() 
				+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().Y
				- ((Game.getInstance().m_graphics.PreferredBackBufferHeight / 2) / Game.getInstance().m_camera.getZoom() + 72);

			if (m_selectedObject != null)
			{
				m_selectedInfo = (m_selectedObject.getLeftPoint() / 72 + 0.5).ToString() + ", " + (m_selectedObject.getTopPoint() / 72 + 0.5).ToString();
			}
			else
			{
				m_selectedInfo = "Joxe!";
			}

			updateCamera();
			updateKeyboard();
			updateMouse();

			m_previousKeyboard = m_currentKeyboard;
			m_previousMouse = m_currentMouse;
		}

		private void updateCamera()
		{
			if (m_currentKeyboard.IsKeyDown(Keys.Right) || m_currentMouse.X > Game.getInstance().m_graphics.PreferredBackBufferWidth - 25)
				Game.getInstance().m_camera.move(new Vector2(15 / Game.getInstance().m_camera.getZoom(), 0));
			if (m_currentKeyboard.IsKeyDown(Keys.Left) || m_currentMouse.X < 25)
				Game.getInstance().m_camera.move(new Vector2(-15 / Game.getInstance().m_camera.getZoom(), 0));
			if (m_currentKeyboard.IsKeyDown(Keys.Up) || m_currentMouse.Y < 25)
				Game.getInstance().m_camera.move(new Vector2(0, -15 / Game.getInstance().m_camera.getZoom()));
			if (m_currentKeyboard.IsKeyDown(Keys.Down) || m_currentMouse.Y > Game.getInstance().m_graphics.PreferredBackBufferHeight - 25)
				Game.getInstance().m_camera.move(new Vector2(0, 15 / Game.getInstance().m_camera.getZoom()));
			if (m_currentMouse.ScrollWheelValue > m_previousMouse.ScrollWheelValue)
				Game.getInstance().m_camera.zoomIn(0.1f);
			if (m_currentMouse.ScrollWheelValue < m_previousMouse.ScrollWheelValue)
				Game.getInstance().m_camera.zoomOut(0.1f);
		}

		private void updateKeyboard()
		{
			if (m_currentKeyboard.IsKeyDown(Keys.P))
				m_itemToCreate = State.Platform;

			if (m_currentKeyboard.IsKeyDown(Keys.L))
				m_itemToCreate = State.Platform;

			if (m_currentKeyboard.IsKeyDown(Keys.B))
				m_itemToCreate = State.Background;

			if (m_currentKeyboard.IsKeyDown(Keys.D) && m_selectedObject != null) {
				m_gameObjectList.Remove(m_selectedObject);
				m_selectedObject = null;
			}

			if (m_currentKeyboard.IsKeyDown(Keys.LeftControl) && m_currentKeyboard.IsKeyDown(Keys.S) && m_previousKeyboard.IsKeyUp(Keys.S))
			{
				Level t_saveLevel = new Level();
				t_saveLevel.setLevelObjects(m_gameObjectList);
				Serializer.getInstace().SaveLevel("Level3.txt", t_saveLevel);

			}
			if (m_currentKeyboard.IsKeyDown(Keys.LeftControl) && m_currentKeyboard.IsKeyDown(Keys.O) && m_previousKeyboard.IsKeyUp(Keys.O))
			{
				Level t_newLevel = Serializer.getInstace().loadLevel("Level3.txt");
				m_gameObjectList = t_newLevel.getLevelObjects();
				foreach (GameObject f_gb in m_gameObjectList)
				{
					f_gb.initImage();
				}
			}
		}

		private void updateMouse()
		{
			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Pressed && m_selectedObject != null) 
				updateMouseDrag(m_worldMouse);
			
			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Released && m_itemToCreate != State.None)
			{
				switch (m_itemToCreate)
				{
					case State.Background:
					{
						createBackground();
						break;
					}
					case State.Ladder:
					{
						createLadder();
						break;
					}
					case State.Platform:
					{
						createPlatform();
						break;
					}
				}
			}

			if (m_currentMouse.RightButton == ButtonState.Pressed && m_itemToCreate != State.None)
				m_itemToCreate = State.None;

			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Released) 
			{
				if (m_selectedObject != null)
				{
					m_selectedObject.setColor(Color.White);
					m_selectedObject = null;
				}
				Rectangle t_mouseClick = new Rectangle((int)m_worldMouse.X, (int)m_worldMouse.Y, 1, 1);

				foreach (GameObject t_gameObject in m_gameObjectList)
				{
					if (t_mouseClick.Intersects(t_gameObject.getBox()))
					{
						m_selectedObject = t_gameObject;
					}
				}
				if (m_selectedObject != null) {
					m_selectedObject.setColor(Color.Yellow);
				}
			}
		}

		private void updateMouseDrag(Vector2 a_worldMouse)
		{
			if (a_worldMouse.X % 72 >= 36)
				a_worldMouse.X = a_worldMouse.X + (72 - (a_worldMouse.X % 72));	
			else if (a_worldMouse.X % 72 < 36)
				a_worldMouse.X = a_worldMouse.X - (a_worldMouse.X % 72);

			if (a_worldMouse.Y % 72 >= 36)
				a_worldMouse.Y = a_worldMouse.Y + (72 - (a_worldMouse.Y % 72));
			else if (a_worldMouse.Y % 72 < 36)
				a_worldMouse.Y = a_worldMouse.Y - (a_worldMouse.Y % 72);
			
			m_selectedObject.getPosition().setX((a_worldMouse.X - (m_selectedObject.getImg().getSize().X / 2)) + 36);
			m_selectedObject.getPosition().setY((a_worldMouse.Y - (m_selectedObject.getImg().getSize().Y / 2)) + 36);
		}

		private void createPlatform()
		{
			Platform t_platform = new Platform(m_worldMouse, "Images//tile");

			if (t_platform.getLeftPoint() % 72 >= 36)
				t_platform.setLeftPoint(t_platform.getLeftPoint() + (72 - (t_platform.getLeftPoint() % 72)) - 36);
			else if (t_platform.getLeftPoint() % 72 < 36)
				t_platform.setLeftPoint(t_platform.getLeftPoint() - (t_platform.getLeftPoint() % 72) + 36);

			if (t_platform.getTopPoint() % 72 >= 36)
				t_platform.setTopPoint(t_platform.getTopPoint() + (72 - (t_platform.getTopPoint() % 72)) - 36);
			else if (t_platform.getTopPoint() % 72 < 36)
				t_platform.setTopPoint(t_platform.getTopPoint() - (t_platform.getTopPoint() % 72) + 36);

			m_gameObjectList.AddLast(t_platform);
		}

		private void createLadder()
		{
			Ladder t_ladder = new Ladder(m_worldMouse, "Images//tile");

			if (t_ladder.getLeftPoint() % 72 >= 36)
				t_ladder.setLeftPoint(t_ladder.getLeftPoint() + (72 - (t_ladder.getLeftPoint() % 72)) - 36);
			else if (t_ladder.getLeftPoint() % 72 < 36)
				t_ladder.setLeftPoint(t_ladder.getLeftPoint() - (t_ladder.getLeftPoint() % 72) + 36);

			if (t_ladder.getTopPoint() % 72 >= 36)
				t_ladder.setTopPoint(t_ladder.getTopPoint() + (72 - (t_ladder.getTopPoint() % 72)) - 36);
			else if (t_ladder.getTopPoint() % 72 < 36)
				t_ladder.setTopPoint(t_ladder.getTopPoint() - (t_ladder.getTopPoint() % 72) + 36);

			m_gameObjectList.AddLast(t_ladder);
		}

		private void createBackground()
		{
			Environment t_environment = new Environment(m_worldMouse, "Images//test");

			if (t_environment.getLeftPoint() % 72 >= 36)
				t_environment.setLeftPoint(t_environment.getLeftPoint() + (72 - (t_environment.getLeftPoint() % 72)) - 36);
			else if (t_environment.getLeftPoint() % 72 < 36)
				t_environment.setLeftPoint(t_environment.getLeftPoint() - (t_environment.getLeftPoint() % 72) + 36);

			if (t_environment.getTopPoint() % 72 >= 36)
				t_environment.setTopPoint(t_environment.getTopPoint() + (72 - (t_environment.getTopPoint() % 72)) - 36);
			else if (t_environment.getTopPoint() % 72 < 36)
				t_environment.setTopPoint(t_environment.getTopPoint() - (t_environment.getTopPoint() % 72) + 36);
			
			t_environment.setLayer(1);
			m_gameObjectList.AddLast(t_environment);
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
