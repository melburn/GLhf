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
<<<<<<< HEAD
		private static Game m_myGame;
=======
<<<<<<< HEAD
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Camera camera;
		States currentState;
=======
		GraphicsDeviceManager m_graphics;
		SpriteBatch m_spriteBatch;
		GameState m_nextState;
		GameState m_currentState;
		Camera m_camera;
>>>>>>> 2ef4895f3e15af1783c0419a609ec0361f9bef4e
>>>>>>> 6015cf94da6f6044f20207bcbe61ae53ffc1a040

        private GraphicsDeviceManager m_graphics;
		private SpriteBatch m_spriteBatch;

		private GameState m_nextState;
		private GameState m_currentState;

		private Camera m_camera;

        private Game()
		{
			m_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

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
		

		protected override void Initialize()
		{
<<<<<<< HEAD
			camera = new Camera();
			currentState = new GameState();
=======
			m_camera = new Camera();
>>>>>>> 2ef4895f3e15af1783c0419a609ec0361f9bef4e
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
<<<<<<< HEAD
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				this.Exit();
			}
=======
			if (m_nextState != null)
			{
				m_currentState = m_nextState;
				m_currentState.load();
				m_nextState = null;
			}
			if (m_currentState != null)
			{
				m_currentState.update(a_gameTime);
			}

>>>>>>> 2ef4895f3e15af1783c0419a609ec0361f9bef4e
			base.Update(a_gameTime);
		}

		protected override void Draw(GameTime a_gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			
            m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, m_camera.getTransformation(m_graphics.GraphicsDevice));
			
            m_currentState.draw(a_gameTime, m_spriteBatch);
			m_spriteBatch.End();

			base.Draw(a_gameTime);
		}

		internal void setState(GameState a_newState)
		{
			m_nextState = a_newState;
		}
	}
}
