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

        private GraphicsDeviceManager m_graphics;
		private SpriteBatch m_spriteBatch;
		
        private States m_nextState;
		private States m_currentState;

		private Camera m_camera;

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
		}

		public SpriteBatch getSpriteBatch() {
			return m_spriteBatch;
		}

		protected override void Initialize()
		{
			m_currentState = new GameState();
			m_camera = new Camera();
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
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				this.Exit();
			}

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
