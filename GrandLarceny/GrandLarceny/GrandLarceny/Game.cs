using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace GrandLarceny
{
	public class Game : Microsoft.Xna.Framework.Game
	{
		private static Game m_myGame;

		internal GraphicsDeviceManager m_graphics;
		private SpriteBatch m_spriteBatch;
		
		private States m_nextState;
		private States m_currentState;
		public static MouseState m_previousMouse;
		public static MouseState m_currentMouse;
		public static KeyboardState m_previousKeyInput;
		public static KeyboardState m_currentKeyInput;
		public const int TILE_WIDTH = 72;
		public const int TILE_HEIGHT = 72;

		internal Camera m_camera;

		public Progress m_progress;
		public Progress m_nextProgress;
		private GameTime m_currentGameTime;

		public static Game getInstance()
		{
			if (m_myGame != null)
			{
				return m_myGame;
			}
			else
			{
				m_myGame = new Game();
				return m_myGame;
			}
		}

		private Game()
		{
			m_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			m_progress = new Progress("temp.prog");
		}

		public SpriteBatch getSpriteBatch()
		{
			return m_spriteBatch;
		}

		protected override void Initialize()
		{
			ErrorLogger.getInstance().clearFile();
			ErrorLogger.getInstance().writeString("GrandLarceny initiated at "+System.DateTime.Now);
			try
			{
				m_camera = new Camera();
				m_currentState = new MainMenu();
				m_currentState.load();
				base.Initialize();
			}
			catch (Exception e)
			{
				ErrorLogger.getInstance().writeString("While instantiating: " + e);
				ErrorLogger.getInstance().writeString("Terminating");
				Exit();
			}
		}

		protected override void LoadContent()
		{
			m_spriteBatch = new SpriteBatch(GraphicsDevice);
			m_camera.load();
		}

		protected override void UnloadContent()
		{
			ErrorLogger.getInstance().writeString("GrandLarceny terminated at " + System.DateTime.Now);
		}

		protected override void Update(GameTime a_gameTime)
		{
			if (!IsActive)
				return;
			m_currentKeyInput = Keyboard.GetState();
			m_currentMouse = Mouse.GetState();
			m_currentGameTime = a_gameTime;

			if (m_nextState != null)
			{
				m_currentState = m_nextState;
				if (!m_currentState.isLoaded())
				{
					//try
					//{
						m_currentState.load();
					//}
					//catch (Exception e)
					//{
						//ErrorLogger.getInstance().writeString("While loading " + m_currentState + " got exception: " + e);
					//}
				}
				m_nextState = null;
			}
			if (m_nextProgress != null)
			{
				m_progress = m_nextProgress;
				m_nextProgress = null;
			}

			if (m_currentState != null)
			{
				//try
				//{
					m_currentState.update(a_gameTime);
				//}
				//catch (Exception e)
				//{
					//ErrorLogger.getInstance().writeString("While updating " + m_currentState + " got exception: " + e);
				//}
			}

			if (keyClicked(Keys.F7)) //Asså det här är ju inte ok
			{
				m_nextState = new HubMenu();
			}

			m_previousMouse = m_currentMouse;
			m_previousKeyInput = m_currentKeyInput;
			base.Update(a_gameTime);
		}

		protected override void Draw(GameTime a_gameTime)
		{
			GraphicsDevice.Clear(new Color(46, 46, 73));
			m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, m_camera.getTransformation(m_graphics.GraphicsDevice));
			m_currentState.draw(a_gameTime, m_spriteBatch);
			m_spriteBatch.End();

			base.Draw(a_gameTime);
		}

		internal void setState(States a_newState)
		{
			m_nextState = a_newState;
		}

		public void setCutscene(String a_fileName)
		{
			if (File.Exists(a_fileName))
			{
				m_nextState = new Cutscene(m_currentState, a_fileName);
			}
			else
			{
				ErrorLogger.getInstance().writeString("While setting cutscene, could not find " + a_fileName);
			}
		}

		internal States getState()
		{
			return m_currentState;
		}

		public Vector2 getResolution()
		{
			return new Vector2(m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight);
		}

		public static Vector2 getMouseCoords()
		{
			return new Vector2(m_currentMouse.X, m_currentMouse.Y);
		}

		public static bool isKeyPressed(Keys key)
		{
			return m_currentKeyInput.IsKeyDown(key);
		}

		public static bool wasKeyPressed(Keys key)
		{
			return m_previousKeyInput.IsKeyDown(key);
		}

		public static bool keyClicked(Keys a_key)
		{
			return m_currentKeyInput.IsKeyDown(a_key) && m_previousKeyInput.IsKeyUp(a_key);
		}

		#region Game Mouse
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
		#endregion

		public Progress getProgress()
		{
			return m_progress;
		}

		public void setProgress(string a_progressName)
		{
			if (!a_progressName.Equals("temp.prog"))
			{
				m_nextProgress = Serializer.getInstance().loadProgress(a_progressName);
			}
		}

		public static bool isKeyReleased(Keys a_key)
		{
			return m_currentKeyInput.IsKeyUp(a_key) && m_previousKeyInput.IsKeyDown(a_key);
		}
		public TimeSpan getGameTime() 
		{
			return m_currentGameTime.TotalGameTime;
		}
	}
}
