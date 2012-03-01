using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text.RegularExpressions;

namespace GrandLarceny
{
	class DevelopmentState : States
	{
		#region Members
		private LinkedList<GameObject>[] m_gameObjectList;
		private LinkedList<GuiObject> m_guiList;
		private LinkedList<Button> m_buttonList;
		private LinkedList<Button> m_assetButtonList;
		private LinkedList<Text> m_textList;
		private LinkedList<Line> m_lineList;

		private GameObject m_selectedObject;
		private GameObject m_objectPreview;
		private GameObject m_player;
		private string m_levelToLoad;

		private Vector2 m_selectedInfoV2;
		private Vector2 m_worldMouse;
		private MouseState m_previousMouse;
		private MouseState m_currentMouse;
		private KeyboardState m_previousKeyboard;
		private KeyboardState m_currentKeyboard;

		private Text m_textCurrentMode;
		private Text m_textSelectedObjectPosition;
		private Text m_textGuardInfo;
		private GuiObject m_UItextBackground;
		private SpriteFont m_courierNew;

		private Button m_btnLadderHotkey;
		private Button m_btnPlatformHotkey;
		private Button m_btnBackgroundHotkey;
		private Button m_btnDeleteHotkey;
		private Button m_btnHeroHotkey;
		private Button m_btnSelectHotkey;
		private Button m_btnSpotlightHotkey;
		private Button m_btnGuardHotkey;
		private Button m_btnWallHotkey;
		private Button m_btnDuckHideHotkey;
		private Button m_btnStandHideHotkey;
		private Button m_btnDogHotkey;
		private Button m_btnLightSwitchHotkey;

		private Line m_leftPatrolLine;
		private Line m_rightPatrolLine;
		private Line m_dragLine = null;

		private int TILE_WIDTH = 72;
		private int TILE_HEIGHT = 72;
		private int m_currentLayer;
		private string assetToCreate = null;

		private enum State
		{
			Platform,
			Player,
			Background,
			Ladder,
			SpotLight,
			LightSwitch,
			Delete,
			None,
			Guard,
			GuardDog,
			Wall,
			GuardLeft,
			GuardRight,
			DuckHidingObject,
			StandHidingObject
		}
		private State m_itemToCreate;
		#endregion

		public DevelopmentState(string a_levelToLoad)
		{
			m_levelToLoad = a_levelToLoad;
			Game.getInstance().m_camera.getPosition().setParentPosition(null);
			Game.getInstance().m_camera.setPosition(new Vector2(Game.getInstance().getResolution().X / 2, Game.getInstance().getResolution().Y / 2));
		}

		private Vector2 getTile(Vector2 a_pixelPosition)
		{
			if (a_pixelPosition.X >= 0)
				a_pixelPosition.X = a_pixelPosition.X - (a_pixelPosition.X % TILE_WIDTH);
			else
				a_pixelPosition.X = a_pixelPosition.X - (a_pixelPosition.X % TILE_WIDTH) - TILE_WIDTH;

			if (a_pixelPosition.Y >= 0)
				a_pixelPosition.Y = a_pixelPosition.Y - (a_pixelPosition.Y % TILE_HEIGHT);
			else
				a_pixelPosition.Y = a_pixelPosition.Y - (a_pixelPosition.Y % TILE_HEIGHT) - TILE_HEIGHT;

			return a_pixelPosition;
		}

		private bool keyClicked(Keys a_key) {
			return m_currentKeyboard.IsKeyDown(a_key) && m_previousKeyboard.IsKeyUp(a_key);
		}

		public override void load()
		{
			m_gameObjectList = Loader.getInstance().loadLevel(m_levelToLoad);
			m_guiList			= new LinkedList<GuiObject>();
			m_textList			= new LinkedList<Text>();
			m_buttonList		= new LinkedList<Button>();
			m_assetButtonList	= new LinkedList<Button>();
			m_lineList			= new LinkedList<Line>();
			m_objectPreview		= null;
			m_courierNew		= Game.getInstance().Content.Load<SpriteFont>("Fonts\\Courier New");

			foreach (LinkedList<GameObject> t_GOArr in m_gameObjectList) {
				foreach (GameObject t_gameObject in t_GOArr) {
					if (t_gameObject is Player) {
						m_player = t_gameObject;
						break;
					}
				}
			}

			m_textCurrentMode				= new Text(new Vector2(12, 10), "null", m_courierNew, Color.Black, false);
			m_textSelectedObjectPosition	= new Text(new Vector2(12, 42), "Nothing Selected", m_courierNew, Color.Black, false);
			m_textGuardInfo = new Text(new Vector2(12, 74), "", m_courierNew, Color.Black, false);
			m_textList.AddLast(m_textCurrentMode);
			m_textList.AddLast(m_textSelectedObjectPosition);
			m_textList.AddLast(m_textGuardInfo);

			m_UItextBackground = new GuiObject(new Vector2(0, 0), "dev_bg_info");
			m_guiList.AddLast(m_UItextBackground);

			Vector2 t_btnTextOffset = new Vector2(8, 50);
			m_btnLadderHotkey		= new Button("btn_ladder_hotkey", 
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 1, Game.getInstance().getResolution().Y - TILE_HEIGHT * 1), "L", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnPlatformHotkey			= new Button("btn_platform_hotkey",
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 2, Game.getInstance().getResolution().Y - TILE_HEIGHT * 1), "P", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnBackgroundHotkey	= new Button("btn_background_hotkey",
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 3, Game.getInstance().getResolution().Y - TILE_HEIGHT * 1), "B", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnDeleteHotkey		= new Button("btn_delete_hotkey",
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 4, Game.getInstance().getResolution().Y - TILE_HEIGHT * 1), "D", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnHeroHotkey			= new Button("btn_hero_hotkey",
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 5, Game.getInstance().getResolution().Y - TILE_HEIGHT * 1), "H", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnSelectHotkey		= new Button("btn_select_hotkey",
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 1, Game.getInstance().getResolution().Y - TILE_HEIGHT * 2), "S", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnSpotlightHotkey	= new Button("btn_spotlight_hotkey",
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 2, Game.getInstance().getResolution().Y - TILE_HEIGHT * 2), "T", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnGuardHotkey		= new Button("btn_guard_hotkey",
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 3, Game.getInstance().getResolution().Y - TILE_HEIGHT * 2), "G", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnWallHotkey			= new Button("btn_wall_hotkey",
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 4, Game.getInstance().getResolution().Y - TILE_HEIGHT * 2), "W", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnDuckHideHotkey		= new Button("btn_duckhide_hotkey",
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 5, Game.getInstance().getResolution().Y - TILE_HEIGHT * 2), "Shift+H", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnStandHideHotkey	= new Button("btn_standhide_hotkey",
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 1, Game.getInstance().getResolution().Y - TILE_HEIGHT * 3), "Ctrl+H", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnDogHotkey			= new Button("btn_dog_hotkey",
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 2, Game.getInstance().getResolution().Y - TILE_HEIGHT * 3), "Shift+G", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnLightSwitchHotkey	= new Button("btn_spotlight_hotkey",
				new Vector2(Game.getInstance().getResolution().X - TILE_WIDTH * 3, Game.getInstance().getResolution().Y - TILE_HEIGHT * 3), "Shift+T", "VerdanaBold", Color.White, t_btnTextOffset);

			m_buttonList.AddLast(m_btnLadderHotkey);
			m_buttonList.AddLast(m_btnPlatformHotkey);
			m_buttonList.AddLast(m_btnBackgroundHotkey);
			m_buttonList.AddLast(m_btnDeleteHotkey);
			m_buttonList.AddLast(m_btnHeroHotkey);
			m_buttonList.AddLast(m_btnSelectHotkey);
			m_buttonList.AddLast(m_btnSpotlightHotkey);
			m_buttonList.AddLast(m_btnGuardHotkey);
			m_buttonList.AddLast(m_btnWallHotkey);
			m_buttonList.AddLast(m_btnDuckHideHotkey);
			m_buttonList.AddLast(m_btnStandHideHotkey);
			m_buttonList.AddLast(m_btnDogHotkey);
			m_buttonList.AddLast(m_btnLightSwitchHotkey);

			foreach (Button t_button in m_buttonList)
				t_button.m_clickEvent += new Button.clickDelegate(guiButtonClick);

			setBuildingState(State.None);
		}

		#region Update
		public override void update(GameTime a_gameTime)
		{
			m_currentKeyboard = Keyboard.GetState();
			m_currentMouse = Mouse.GetState();

			updateCamera();
			updateKeyboard();
			updateMouse();
			updateGUI();

			m_previousKeyboard = m_currentKeyboard;
			m_previousMouse = m_currentMouse;
		}

		#region Update Camera
		private void updateCamera()
		{
			if (m_currentKeyboard.IsKeyDown(Keys.Right))
				Game.getInstance().m_camera.move(new Vector2(15 / Game.getInstance().m_camera.getZoom(), 0));
			if (m_currentKeyboard.IsKeyDown(Keys.Left))
				Game.getInstance().m_camera.move(new Vector2(-15 / Game.getInstance().m_camera.getZoom(), 0));
			if (m_currentKeyboard.IsKeyDown(Keys.Up))
				Game.getInstance().m_camera.move(new Vector2(0, -15 / Game.getInstance().m_camera.getZoom()));
			if (m_currentKeyboard.IsKeyDown(Keys.Down))
				Game.getInstance().m_camera.move(new Vector2(0, 15 / Game.getInstance().m_camera.getZoom()));
			if (m_currentMouse.ScrollWheelValue > m_previousMouse.ScrollWheelValue)
				Game.getInstance().m_camera.zoomIn(0.1f);
			if (m_currentMouse.ScrollWheelValue < m_previousMouse.ScrollWheelValue)
				Game.getInstance().m_camera.zoomOut(0.1f);
		}
		#endregion

		#region Update GUI
		private void updateGUI()
		{
			if (m_objectPreview != null) {
				m_objectPreview.getPosition().setX(m_worldMouse.X + 15);
				m_objectPreview.getPosition().setY(m_worldMouse.Y + 15);
			}

			foreach (Button t_button in m_assetButtonList)
				t_button.update();
			foreach (Button t_button in m_buttonList)
				t_button.update();

			if (m_selectedObject != null)
			{
				m_selectedInfoV2.X = getTile(m_selectedObject.getPosition().getGlobalCartesianCoordinates()).X / TILE_WIDTH;
				m_selectedInfoV2.Y = getTile(m_selectedObject.getPosition().getGlobalCartesianCoordinates()).Y / TILE_HEIGHT;
				m_textSelectedObjectPosition.setText(m_selectedInfoV2.ToString());
				if (m_selectedObject is Guard) {
					Guard t_guard = (Guard)m_selectedObject;
					m_textGuardInfo.setText("R: " + t_guard.getRightpatrolPoint() / 72 + " L: " + t_guard.getLeftpatrolPoint() / 72);
				} else {
					m_textGuardInfo.setText("");
				}
			}
		}
		#endregion

		#region Update Keyboard
		private void updateKeyboard()
		{
			if (keyClicked(Keys.D1))
				m_currentLayer = 0;
			if (keyClicked(Keys.D2))
				m_currentLayer = 1;
			if (keyClicked(Keys.D3))
				m_currentLayer = 2;
			if (keyClicked(Keys.D4))
				m_currentLayer = 3;
			if (keyClicked(Keys.D5))
				m_currentLayer = 4;

			if (keyClicked(Keys.R))
				Game.getInstance().setState(new GameState(m_levelToLoad));
			if (keyClicked(Keys.P))
				setBuildingState(State.Platform);
			if (keyClicked(Keys.L))
				setBuildingState(State.Ladder);
			if (keyClicked(Keys.B))
				setBuildingState(State.Background);
			if (keyClicked(Keys.D))
				setBuildingState(State.Delete);
			if (keyClicked(Keys.H))
				setBuildingState(State.Player);
			if (keyClicked(Keys.S))
				setBuildingState(State.None);
			if (keyClicked(Keys.T))
				setBuildingState(State.SpotLight);
			if (m_currentKeyboard.IsKeyDown(Keys.LeftShift) && keyClicked(Keys.T))
				setBuildingState(State.LightSwitch);
			if (keyClicked(Keys.G))
				setBuildingState(State.Guard);
			if (keyClicked(Keys.W))
				setBuildingState(State.Wall);
			if (keyClicked(Keys.N))
				setBuildingState(State.GuardLeft);
			if (keyClicked(Keys.M))
				setBuildingState(State.GuardRight);
			if (m_currentKeyboard.IsKeyDown(Keys.LeftShift) && keyClicked(Keys.H))
				setBuildingState(State.DuckHidingObject);
			if (m_currentKeyboard.IsKeyDown(Keys.LeftControl) && keyClicked(Keys.H))
				setBuildingState(State.StandHidingObject);
			if (m_currentKeyboard.IsKeyDown(Keys.LeftShift) && keyClicked(Keys.G))
				setBuildingState(State.GuardDog);
			if (keyClicked(Keys.Space))
				if (m_gameObjectList != null)
					Game.getInstance().m_camera.setPosition(m_gameObjectList[m_currentLayer].First().getPosition().getGlobalCartesianCoordinates());

			if (m_currentKeyboard.IsKeyDown(Keys.LeftControl) && m_currentKeyboard.IsKeyDown(Keys.S) && m_previousKeyboard.IsKeyUp(Keys.S))
			{
				if (m_selectedObject != null)
				{
					m_selectedObject.setColor(Color.White);
					m_selectedObject = null;
				}
				Level t_saveLevel = new Level();
				t_saveLevel.setLevelObjects(m_gameObjectList);
				Serializer.getInstance().SaveLevel(m_levelToLoad, t_saveLevel);

			}
			if (m_currentKeyboard.IsKeyDown(Keys.LeftControl) && m_currentKeyboard.IsKeyDown(Keys.O) && m_previousKeyboard.IsKeyUp(Keys.O))
			{
				Level t_newLevel = Serializer.getInstance().loadLevel(m_levelToLoad);
				m_gameObjectList = t_newLevel.getLevelLists();
				foreach (LinkedList<GameObject> t_arr in m_gameObjectList) {
					foreach (GameObject f_gb in t_arr)
					{
						f_gb.loadContent();
					}
				}
			}
		}
		#endregion

		#region Update Mouse
		private void updateMouse() {
			m_worldMouse.X = 
				Mouse.GetState().X / Game.getInstance().m_camera.getZoom()
				+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().X
				- ((Game.getInstance().getResolution().X / 2) / Game.getInstance().m_camera.getZoom());
			m_worldMouse.Y = 
				Mouse.GetState().Y / Game.getInstance().m_camera.getZoom() 
				+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().Y
				- ((Game.getInstance().getResolution().Y / 2) / Game.getInstance().m_camera.getZoom());

			if (m_currentMouse.MiddleButton == ButtonState.Pressed && m_previousMouse.MiddleButton == ButtonState.Pressed) {
				Vector2 t_difference = Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates();
				t_difference.X = (Mouse.GetState().X - Game.getInstance().getResolution().X / 2) / 20;
				t_difference.Y = (Mouse.GetState().Y - Game.getInstance().getResolution().Y / 2) / 20;
				Game.getInstance().m_camera.getPosition().plusWith(t_difference);
			}

			if (m_currentMouse.LeftButton == ButtonState.Pressed 
				&& m_previousMouse.LeftButton == ButtonState.Released 
				&& m_itemToCreate != State.Delete 
				&& m_itemToCreate != State.None 
				&& !collidedWithGui(m_worldMouse)) 
			{
				if (assetToCreate != null) {
					switch (m_itemToCreate)
					{
						case State.Player:
							createPlayer();
							break;
						case State.Background:
							createBackground();
							break;
						case State.Ladder:
							createLadder();
							break;
						case State.Platform:
							createPlatform();
							break;
						case State.SpotLight:
							createSpotLight();
							break;
						case State.Guard:
							createGuard();
							break;
						case State.Wall:
							createWall();
							break;
						case State.DuckHidingObject:
							createDuckHidingObject();
							break;
						case State.StandHidingObject:
							createStandHideObject();
							break;
						case State.GuardDog:
							createGuardDog();
							break;
						case State.LightSwitch:
							createLightSwitch();
							break;
					}
				} else {
					setGuardPoint((NPE)m_selectedObject, m_itemToCreate == State.GuardRight);
				}
				return;
			}

			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Pressed && m_selectedObject != null) {
				if (m_itemToCreate == State.GuardLeft || m_itemToCreate == State.GuardRight)
					return;
				Vector2 t_mousePosition = getTile(m_worldMouse);
			
				if (m_selectedObject is SpotLight)
					m_selectedObject.getPosition().setX(t_mousePosition.X + m_selectedObject.getBox().Width);
				else
					m_selectedObject.getPosition().setX(t_mousePosition.X);
				m_selectedObject.getPosition().setY(t_mousePosition.Y);
			}

			if (m_currentMouse.RightButton == ButtonState.Pressed && m_previousMouse.RightButton == ButtonState.Pressed && m_selectedObject != null) {
				if (m_selectedObject is LampSwitch) {
					if (m_dragLine == null) {
						m_dragLine = new Line(m_selectedObject.getPosition(), new CartesianCoordinate(m_worldMouse), new Vector2(36, 36), Vector2.Zero, Color.Yellow, 5);
					} else {
						m_dragLine.setEndpoint(m_worldMouse);
					}
				}
				if (m_selectedObject is Guard || m_selectedObject is GuardDog) {
					if (m_dragLine == null) {
						m_dragLine = new Line(m_selectedObject.getPosition(), new CartesianCoordinate(new Vector2(m_worldMouse.X, m_selectedObject.getPosition().getGlobalY() + 36)), new Vector2(36, 36), Vector2.Zero, Color.Green, 5);
					} else {
						m_dragLine.setEndpoint(new Vector2(m_worldMouse.X, m_selectedObject.getPosition().getGlobalY() + 36));
					}
				}
			}

			if (m_currentMouse.RightButton == ButtonState.Released && m_previousMouse.RightButton == ButtonState.Pressed) {
				if (m_dragLine != null) {
					if (m_selectedObject is LampSwitch) {
						foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer]) {
							if (t_gameObject is SpotLight) {
								if (t_gameObject.getBox().Contains((int)m_worldMouse.X, (int)m_worldMouse.Y)) {
									connectSpotLight((SpotLight)t_gameObject, (LampSwitch)m_selectedObject);
									break;
								}
							}
						}
						showLightSwitchInfo((LampSwitch)m_selectedObject);
					}
					if (m_selectedObject is Guard) {
						if (m_worldMouse.X > m_selectedObject.getPosition().getGlobalX()) {
							setGuardPoint((Guard)m_selectedObject, true);
						} else {
							setGuardPoint((Guard)m_selectedObject, false);
						}
						showGuardInfo((Guard)m_selectedObject);
					}
					if (m_selectedObject is GuardDog) {
						if (m_worldMouse.X > m_selectedObject.getPosition().getGlobalX()) {
							setGuardPoint((GuardDog)m_selectedObject, true);
						} else {
							setGuardPoint((GuardDog)m_selectedObject, false);
						}
						showDogInfo((GuardDog)m_selectedObject);
					}
				}
				m_dragLine = null;
				setBuildingState(State.None);
			}

			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Released) {
				if (m_selectedObject != null) {
					clearSelectedObject();
				}

				if (!collidedWithGui(m_worldMouse)) {
					foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer]) {
						if (t_gameObject is LightCone)
							continue;
						if (t_gameObject.getBox().Contains((int)m_worldMouse.X, (int)m_worldMouse.Y)) {
							if (m_selectedObject == null || m_selectedObject.getLayer() > t_gameObject.getLayer()) {
								m_selectedObject = t_gameObject;
							}
						}
					}
					if (m_itemToCreate == State.Delete && m_selectedObject != null) {
						deleteObject(m_selectedObject);
						m_selectedInfoV2 = Vector2.Zero;
						m_selectedObject = null;
						return;
					}
					if (m_selectedObject != null) {
						if (m_selectedObject is Guard) {
							showGuardInfo((Guard)m_selectedObject);
						} else if (m_selectedObject is GuardDog) {
							showDogInfo((GuardDog)m_selectedObject);
						} else if (m_selectedObject is LampSwitch) {
							showLightSwitchInfo((LampSwitch)m_selectedObject);
						}
						m_selectedObject.setColor(Color.Yellow);
					}
				}
			}
		}
		#endregion

		#endregion

		#region Collision Check
		private bool collidedWithGui(Vector2 a_coordinate)
		{
			foreach (GuiObject t_guiObject in m_guiList)
				if (t_guiObject.getBox().Contains((int)a_coordinate.X, (int)a_coordinate.Y))
					return true;
			foreach (Button t_button in m_buttonList)
				if (t_button.getBox().Contains((int)Mouse.GetState().X, (int)Mouse.GetState().Y))
					return true;
			foreach (Button t_button in m_assetButtonList)
				if (t_button.getBox().Contains((int)Mouse.GetState().X, (int)Mouse.GetState().Y))
					return true;
			return false;
		}

		public bool collidedWithObject(Vector2 a_coordinate) {
			Rectangle t_rectangle = new Rectangle((int)getTile(a_coordinate).X, (int)getTile(a_coordinate).Y, 1, 1);

			foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer]) {
				if (t_gameObject is Environment)
					continue;
				if (t_gameObject.getBox().Contains(t_rectangle))
					return true;
			}
			return false;
		}
		#endregion

		public void guiButtonClick(Button a_button)
		{			
			if (a_button == m_btnLadderHotkey)
				setBuildingState(State.Ladder);
			if (a_button == m_btnPlatformHotkey)
				setBuildingState(State.Platform);
			if (a_button == m_btnBackgroundHotkey)
				setBuildingState(State.Background);
			if (a_button == m_btnDeleteHotkey)
				setBuildingState(State.Delete);
			if (a_button == m_btnHeroHotkey)
				setBuildingState(State.Player);
			if (a_button == m_btnSelectHotkey)
				setBuildingState(State.None);
			if (a_button == m_btnSpotlightHotkey)
				setBuildingState(State.SpotLight);
			if (a_button == m_btnGuardHotkey)
				setBuildingState(State.Guard);
			if (a_button == m_btnWallHotkey)
				setBuildingState(State.Wall);
			if (a_button == m_btnDeleteHotkey)
				setBuildingState(State.Delete);
			if (a_button == m_btnDuckHideHotkey)
				setBuildingState(State.DuckHidingObject);
			if (a_button == m_btnStandHideHotkey)
				setBuildingState(State.StandHidingObject);
			if (a_button == m_btnDogHotkey)
				setBuildingState(State.GuardDog);
			if (a_button == m_btnLightSwitchHotkey)
				setBuildingState(State.LightSwitch);
		}
		
		private void createAssetList(string a_assetDirectory) {
			m_assetButtonList = new LinkedList<Button>();
			if (a_assetDirectory == null)
				return;
			string[] t_levelList = Directory.GetFiles(a_assetDirectory);
			for (int i = 0, j = 0; i < t_levelList.Length; i++) {
				if ((t_levelList[i].EndsWith(".xnb") == false))
					continue;
				string[] t_splitPath = Regex.Split(t_levelList[i], "//");
				Button t_button = new Button(
					"btn_asset_list", new Vector2(Game.getInstance().getResolution().X - 160, 21 * j),
					t_splitPath[t_splitPath.Length - 1].Remove(t_splitPath[t_splitPath.Length - 1].Length - 4),
					"Courier New10", Color.Black, new Vector2(7, 1)
				);
				t_button.m_clickEvent += new Button.clickDelegate(selectAsset);
				m_assetButtonList.AddLast(t_button);
				j++;
			}
		}

		private void clearSelectedObject() {
			m_selectedObject.setColor(Color.White);
			m_selectedObject = null;
			m_selectedInfoV2 = Vector2.Zero;
			m_leftPatrolLine = null;
			m_rightPatrolLine = null;
			m_lineList.Clear();
		}

		private void setBuildingState(State a_state) {
			m_itemToCreate = a_state;
			assetToCreate = null;
			m_objectPreview = null;
			foreach (Button t_button in m_buttonList)
				t_button.setState(0);

			switch (m_itemToCreate) {
				case State.Platform:
					m_textCurrentMode.setText("Create Platform");
					createAssetList("Content//Images//Tile//");
					m_btnPlatformHotkey.setState(3);
					break;
				case State.Ladder:
					m_textCurrentMode.setText("Create Ladder");
					createAssetList("Content//Images//Tile//");
					m_btnLadderHotkey.setState(3);
					break;
				case State.Background:
					m_textCurrentMode.setText("Create Background");
					createAssetList("Content//Images//Background//");
					m_btnBackgroundHotkey.setState(3);
					break;
				case State.Delete:
					m_textCurrentMode.setText("Delete Object");
					createAssetList(null);
					m_btnDeleteHotkey.setState(3);
					break;
				case State.Player:
					m_textCurrentMode.setText("Create Hero");
					createAssetList("Content//Images//Sprite//");
					m_btnHeroHotkey.setState(3);
					break;
				case State.None:
					m_textCurrentMode.setText("Select");
					createAssetList(null);
					m_btnSelectHotkey.setState(3);
					break;
				case State.SpotLight:
					m_textCurrentMode.setText("Create SpotLight");
					createAssetList("Content//Images//LightCone//");
					m_btnSpotlightHotkey.setState(3);
					break;
				case State.Guard:
					m_textCurrentMode.setText("Create Guard");
					createAssetList("Content//Images//Sprite//");
					m_btnGuardHotkey.setState(3);
					break;
				case State.Wall:
					m_textCurrentMode.setText("Create Wall");
					createAssetList("Content//Images//Tile//");
					m_btnWallHotkey.setState(3);
					break;
				case State.GuardRight:
					m_textCurrentMode.setText("Set Right Point");
					break;
				case State.GuardLeft:
					m_textCurrentMode.setText("Set Left Point");
					break;
				case State.DuckHidingObject:
					m_textCurrentMode.setText("Ducking Hide Object");
					createAssetList("Content//Images//Prop//");
					m_btnDuckHideHotkey.setState(3);
					break;
				case State.StandHidingObject:
					m_textCurrentMode.setText("Standing Hide Object");
					createAssetList("Content//Images//Prop//");
					m_btnStandHideHotkey.setState(3);
					break;
				case State.GuardDog:
					m_textCurrentMode.setText("Create Guard Dog");
					createAssetList("Content//Images//Sprite//");
					m_btnDogHotkey.setState(3);
					break;
				case State.LightSwitch:
					m_textCurrentMode.setText("Create Light Switch");
					createAssetList("Content//Images//Tile//");
					m_btnLightSwitchHotkey.setState(3);
					break;
			}
			if (m_assetButtonList != null && m_assetButtonList.Count > 0) {
				selectAsset(m_assetButtonList.First());
			}
		}

		private void showGuardInfo(Guard a_guard) {
			m_textGuardInfo.setText(" L: " + a_guard.getLeftpatrolPoint() + "R: " + a_guard.getRightpatrolPoint());
			m_leftPatrolLine = new Line(a_guard.getPosition(), new CartesianCoordinate(new Vector2(a_guard.getLeftpatrolPoint(), a_guard.getPosition().getGlobalY())), new Vector2(36, 72), new Vector2(36, 72), Color.Green, 5);
			m_rightPatrolLine = new Line(a_guard.getPosition(), new CartesianCoordinate(new Vector2(a_guard.getRightpatrolPoint(), a_guard.getPosition().getGlobalY())), new Vector2(36, 72), new Vector2(36, 72), Color.Green, 5);
		}

		private void showDogInfo(GuardDog a_guard) {
			m_textGuardInfo.setText(" L: " + a_guard.getLeftpatrolPoint() + "R: " + a_guard.getRightpatrolPoint());
			m_leftPatrolLine = new Line(a_guard.getPosition(), new CartesianCoordinate(new Vector2(a_guard.getLeftpatrolPoint(), a_guard.getPosition().getGlobalY())), new Vector2(36, 72), new Vector2(36, 72), Color.Green, 5);
			m_rightPatrolLine = new Line(a_guard.getPosition(), new CartesianCoordinate(new Vector2(a_guard.getRightpatrolPoint(), a_guard.getPosition().getGlobalY())), new Vector2(36, 72), new Vector2(36, 72), Color.Green, 5);
		}

		private void showLightSwitchInfo(LampSwitch a_lightswitch) {
			if (m_lineList == null)
				m_lineList = new LinkedList<Line>();
			foreach (SpotLight t_spotLight in a_lightswitch.getConnectedSpotLights()) {
				m_lineList.AddLast(new Line(a_lightswitch.getPosition(), t_spotLight.getPosition(), new Vector2(36, 36), new Vector2(0, 0), Color.Yellow, 5));
			}
		}

		private void setGuardPoint(NPE a_guard, bool a_right) {
			if (a_guard is Guard) {
				if (a_right) {
					((Guard)a_guard).setRightGuardPoint(getTile(m_worldMouse).X);
				} else {
					((Guard)a_guard).setLeftGuardPoint(getTile(m_worldMouse).X);
				}
			} else if (a_guard is GuardDog) {
				if (a_right) {
					((GuardDog)a_guard).setRightGuardPoint(getTile(m_worldMouse).X);
				} else {
					((GuardDog)a_guard).setLeftGuardPoint(getTile(m_worldMouse).X);
				}
			} else {
				throw new ArgumentException();
			}
		}

		private void connectSpotLight(SpotLight a_spotLight, LampSwitch a_lightSwitch) {
			a_lightSwitch.connectSpotLight(a_spotLight);
		}

		private void selectAsset(Button a_button)
		{
			assetToCreate = a_button.getText();
			foreach (Button t_button in m_assetButtonList)
				t_button.setState(0);
			a_button.setState(3);
			switch (m_itemToCreate) {
				case State.Platform:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//" + assetToCreate, 0.000f);
					break;
				case State.Wall:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//" + assetToCreate, 0.000f);
					break;
				case State.Delete:
					m_objectPreview = null;
					break;
				case State.Guard:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Sprite//" + assetToCreate, 0.000f);
					break;
				case State.Ladder:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//" + assetToCreate, 0.000f);
					break;
				case State.None:
					m_objectPreview = null;
					break;
				case State.Player:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Sprite//" + assetToCreate, 0.000f);
					break;
				case State.SpotLight:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//LightCone//" + assetToCreate, 0.000f);
					break;
				case State.Background:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Background//" + assetToCreate, 0.000f);
					break;
				case State.DuckHidingObject:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Prop//" + assetToCreate, 0.000f);
					break;
				case State.StandHidingObject:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Prop//" + assetToCreate, 0.000f);
					break;
				case State.GuardDog:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Sprite//" + assetToCreate, 0.000f);
					break;
				case State.LightSwitch:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//" + assetToCreate, 0.000f);
					break;
			}
		}

		#region Create-methods
		private void createPlayer() {
			if (m_player == null) {
				if (collidedWithObject(m_worldMouse)) {
					return;
				}
				m_player = new Player(getTile(m_worldMouse), "Images//Sprite//" + assetToCreate, 0.250f);
				addObject(m_player);
			}
		}

		private void createPlatform() {
			if (collidedWithObject(m_worldMouse))
				return;
			Platform t_platform = new Platform(getTile(m_worldMouse), "Images//Tile//" + assetToCreate, 0.350f);
			addObject(t_platform);
		}

		private void createLadder() {
			if (collidedWithObject(m_worldMouse))
				return;
			Ladder t_ladder = new Ladder(getTile(m_worldMouse), "Images//Tile//" + assetToCreate, 0.350f);
			addObject(t_ladder);
		}

		private void createSpotLight() {
			if (collidedWithObject(m_worldMouse))
				return;
			SpotLight t_sl = new SpotLight(getTile(m_worldMouse), "Images//LightCone//"  + assetToCreate, 0.2f, (float)(Math.PI * 0.5f), true);
			addObject(t_sl);
		}

		private void createBackground()
		{
			Environment t_environment = new Environment(getTile(m_worldMouse), "Images//Background//"  + assetToCreate, 0.750f);
			addObject(t_environment);
		}

		private void createGuard()
		{
			if (collidedWithObject(m_worldMouse))
				return;
			Guard t_guard = new Guard(getTile(m_worldMouse), "Images//Sprite//" + assetToCreate, getTile(m_worldMouse).X, true, false, 0.300f);
			addObject(t_guard);
		}

		private void createWall()
		{
			if (collidedWithObject(m_worldMouse))
				return;
			Wall t_wall = new Wall(getTile(m_worldMouse), "Images//Tile//" + assetToCreate, 0.350f);
			addObject(t_wall);
		}

		private void deleteObject(GameObject a_gameObject)
		{
			if (a_gameObject is Player)
				m_player = null;
			a_gameObject.kill();
			if (a_gameObject is SpotLight) {
				LightCone t_lightCone = ((SpotLight)a_gameObject).getLightCone();
				if (t_lightCone != null)
					m_gameObjectList[m_currentLayer].Remove(t_lightCone);
			}
			m_gameObjectList[m_currentLayer].Remove(a_gameObject);
		}

		private void createDuckHidingObject()
		{
			if (collidedWithObject(m_worldMouse))
				return;
			DuckHideObject t_hideObject = new DuckHideObject(getTile(m_worldMouse), "Images//Prop//" + assetToCreate, 0.700f);
			addObject(t_hideObject);
		}

		private void createStandHideObject() {
			if (collidedWithObject(m_worldMouse))
				return;
			StandHideObject t_hideObject = new StandHideObject(getTile(m_worldMouse), "Images//Prop//" + assetToCreate, 0.700f);
			addObject(t_hideObject);
		}

		private void createGuardDog() {
			if (collidedWithObject(m_worldMouse))
				return;
			GuardDog t_guardDog = new GuardDog(getTile(m_worldMouse), "Images//Sprite//" + assetToCreate, getTile(m_worldMouse).X, getTile(m_worldMouse).X, 0.299f);
			addObject(t_guardDog);
		}

		private void createLightSwitch() {
			if (collidedWithObject(m_worldMouse))
				return;
			LampSwitch t_lightSwitch = new LampSwitch(getTile(m_worldMouse), "Images//Tile//" + assetToCreate, 0.700f, false);
			addObject(t_lightSwitch);
		}
		#endregion

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (Text t_textObject in m_textList)
				t_textObject.draw(a_spriteBatch);
			foreach (GuiObject t_guiObject in m_guiList)
				t_guiObject.draw(a_gameTime);
			foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer])
				t_gameObject.draw(a_gameTime);
			foreach (Button t_button in m_buttonList)
				t_button.draw(a_gameTime, a_spriteBatch);
			foreach (Button t_button in m_assetButtonList)
				t_button.draw(a_gameTime, a_spriteBatch);
			foreach (Line t_line in m_lineList)
				t_line.draw();
			if (m_objectPreview != null)
				m_objectPreview.draw(a_gameTime);
			if (m_leftPatrolLine != null)
				m_leftPatrolLine.draw();
			if (m_rightPatrolLine != null)
				m_rightPatrolLine.draw();
			if (m_dragLine != null)
				m_dragLine.draw();
		}

		public override void addObject(GameObject a_object)
		{
			m_gameObjectList[m_currentLayer].AddLast(a_object);
		}
	}
}