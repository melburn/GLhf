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
		public const string CUTSCENE_FOLDER = "Content//wtf//Cutscenes//";

		internal GraphicsDeviceManager m_graphics;
		private SpriteBatch m_spriteBatch;
		
		private States m_nextState;
		private States m_currentState;
		public const int TILE_WIDTH = 72;
		public const int TILE_HEIGHT = 72;

		internal Camera m_camera;

		public Progress m_progress;
		public Progress m_nextProgress;
		private GameTime m_currentGameTime;

		private MemoryStream m_checkPointLevel;
		private MemoryStream m_checkPointProgress;

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
			KeyboardHandler.setCurrentKeyboard(Keyboard.GetState());
			MouseHandler.setCurrentMouse(Mouse.GetState());
			m_currentGameTime = a_gameTime;

			if (m_nextState != null)
			{
				m_currentState = m_nextState;
				AssetFactory.updateState(m_currentState);
				if (!m_currentState.isLoaded())
				{
					try
					{
						m_currentState.load();
					}
					catch (Exception e)
					{
						ErrorLogger.getInstance().writeString("While loading " + m_currentState + " got exception: " + e);
					}
				}
				m_nextState = null;
			}
			if (m_nextProgress != null)
			{
				m_progress = m_nextProgress;
				m_nextProgress = null;
			}
			#if DEBUG
			if (m_currentState != null)
			{
				m_currentState.update(a_gameTime);
			}
			#else
			if (m_currentState != null)
			{
				try
				{
					m_currentState.update(a_gameTime);
				}
				catch (Exception e)
				{
					//ErrorLogger.getInstance().writeString("While updating " + m_currentState + " got exception: " + e);
				}
			}
			#endif

			if (KeyboardHandler.keyClicked(Keys.F7)) //TODO Alfa/Beta-grej
			{
				m_nextState = new MainMenu();
			}

			MouseHandler.setPreviousMouse();
			KeyboardHandler.setPreviousKeyboard();
			base.Update(a_gameTime);
		}

		protected override void Draw(GameTime a_gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
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
			if (File.Exists(CUTSCENE_FOLDER + a_fileName))
			{
				m_nextState = new Cutscene(m_currentState, a_fileName);
			}
			else
			{
				ErrorLogger.getInstance().writeString("While setting cutscene, could not find " + CUTSCENE_FOLDER + a_fileName);
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

		public Progress getProgress()
		{
			return m_progress;
		}

		public void setProgress(string a_progressName, bool a_checkPoint)
		{
			if (a_checkPoint)
			{
				m_nextProgress = Serializer.getInstance().loadProgress(Game.getInstance().getCheckPointProgress(false));
			}
			else if (!a_progressName.Equals("temp.prog"))
			{
				m_nextProgress = Serializer.getInstance().loadProgress(Serializer.getInstance().getFileToStream(a_progressName,false));
			}
		}

		
		public TimeSpan getGameTime() 
		{
			return m_currentGameTime.TotalGameTime;
		}

		public bool hasCheckPoint()
		{
			if (m_checkPointLevel != null && m_checkPointLevel.Length != 0)
				return true;
			else
				return false;
		}

		public MemoryStream getCheckPointLevel(bool a_save)
		{
			if (a_save)
			{
				m_checkPointLevel = new MemoryStream();
			}
			else
			{
				m_checkPointLevel.Position = 0;
			}
			return m_checkPointLevel;
		}
		public MemoryStream getCheckPointProgress(bool a_save)
		{
			if (a_save)
			{
				m_checkPointProgress = new MemoryStream();
			}
			else
			{
				m_checkPointProgress.Position = 0;
			}
			return m_checkPointProgress;
		}
	}
}
