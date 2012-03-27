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

		internal Camera m_camera;

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
			m_graphics.PreferredBackBufferWidth = 1280;
			m_graphics.PreferredBackBufferHeight = 720;
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		public SpriteBatch getSpriteBatch()
		{
			return m_spriteBatch;
		}

		protected override void Initialize()
		{
			m_camera = new Camera();
			m_currentState = new MainMenu();
			m_currentState.load();
			base.Initialize();
		}

		protected override void LoadContent()
		{
			m_spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void UnloadContent()
		{
			
		}

		protected override void Update(GameTime a_gameTime)
		{
			if (!IsActive)
				return;
			m_currentKeyInput = Keyboard.GetState();
			m_currentMouse = Mouse.GetState();

			if (m_nextState != null)
			{
				m_currentState = m_nextState;
				if (!m_currentState.isLoaded())
				{
					m_currentState.load();
				}
				m_nextState = null;
			}

			if (m_currentState != null)
			{
				m_currentState.update(a_gameTime);
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
			m_nextState = new Cutscene(m_currentState, a_fileName);
		}
		internal States getState()
		{
			return m_currentState;
		}
		internal Vector2 getResolution() {
			return new Vector2(m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight);
		}
		public static Vector2 getMouseCoords() {
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
		public static bool rmbClicked()
		{
			return m_currentMouse.RightButton == ButtonState.Pressed && m_previousMouse.RightButton == ButtonState.Released;
		}

		internal static bool lmbClicked()
		{
			return m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Released;
		}
	}
}
