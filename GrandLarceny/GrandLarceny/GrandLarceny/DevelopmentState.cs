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

		private LinkedList<Button> m_staticButton;
		private LinkedList<Button> m_buildingButtons;
		private LinkedList<Button> m_ventButtons;
		private LinkedList<Button> m_guardButtons;
		private LinkedList<Button> m_hideButtons;

		private LinkedList<Button> m_assetButtonList;
		private LinkedList<Button> m_layerButtonList;
		private LinkedList<Line> m_lineList;

		private GameObject m_selectedObject;
		private GameObject m_objectPreview;
		private GameObject m_player;

		private Vector2 m_selectedInfoV2;
		private Vector2 m_worldMouse;
		private Vector2 m_dragOffset;
		private MouseState m_previousMouse;
		private MouseState m_currentMouse;
		private KeyboardState m_previousKeyboard;
		private KeyboardState m_currentKeyboard;

		private Text m_textCurrentMode;
		private Text m_textSelectedObjectPosition;
		private Text m_textGuardInfo;
		private Text m_layerInfo;
		private GuiObject m_UItextBackground;

		private TextField m_test;
		
		/*
		-----------------------------------
		Buttons that are always shown
		-----------------------------------
		*/
		private Button m_btnSelectHotkey;
		private Button m_btnDeleteHotkey;

		/*
		-----------------------------------
		Building mode buttons
		-----------------------------------
		*/
		private Button m_btnLadderHotkey;
		private Button m_btnPlatformHotkey;
		private Button m_btnBackgroundHotkey;
		private Button m_btnHeroHotkey;
		private Button m_btnSpotlightHotkey;
		private Button m_btnWallHotkey;
		private Button m_btnLightSwitchHotkey;
		private Button m_btnVentHotkey;
		private Button m_btnWindowHotkey;

		/*
		-----------------------------------
		Ventilation buttons
		-----------------------------------
		*/
		private Button m_btnCrossVent;
		private Button m_btnCornerVent;
		private Button m_btnTVent;
		private Button m_btnStraVent;

		/*
		-----------------------------------
		Hide buttons
		-----------------------------------
		*/
		private Button m_btnDuckHideHotkey;
		private Button m_btnStandHideHotkey;

		/*
		-----------------------------------
		Guard buttons
		-----------------------------------
		*/
		private Button m_btnGuardHotkey;
		private Button m_btnDogHotkey;
		private Button m_btnCameraHotkey;

		private Line m_dragLine = null;

		private Sound m_sndKeyclick;
		private Sound m_sndSave;

		private int TILE_WIDTH = 72;
		private int TILE_HEIGHT = 72;
		private int m_currentLayer = 0;
		private string assetToCreate = null;
		private string m_levelToLoad;
		private bool m_building;
		private bool m_ventilation;

		private State m_itemToCreate;
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
			DuckHidingObject,
			StandHidingObject,
			Ventilation,
			Camera,
			CrossVent,
			TVent,
			StraVent,
			CornerVent,
			Ventrance,
			Window
		}

		private MenuState m_menuState;
		private enum MenuState {
			Normal,
			Ventilation,
			Guard,
			Hide
		}
		#endregion

		#region Load&Initiate
		public DevelopmentState(string a_levelToLoad)
		{
			m_levelToLoad = a_levelToLoad;
			Game.getInstance().m_camera.getPosition().setParentPosition(null);
			Game.getInstance().m_camera.setPosition(Vector2.Zero);
		}

		public override void load()
		{
			m_guiList = new LinkedList<GuiObject>();
			m_gameObjectList = Loader.getInstance().loadLevel(m_levelToLoad);

			foreach (LinkedList<GameObject> t_ll in m_gameObjectList)
			{
				foreach (GameObject t_go in t_ll)
				{
					t_go.loadContent();

					if (t_go is Player)
					{
						Game.getInstance().getState().setPlayer((Player)t_go);
					}
				}
			}

			m_buildingButtons	= new LinkedList<Button>();
			m_staticButton		= new LinkedList<Button>();
			m_ventButtons		= new LinkedList<Button>();
			m_guardButtons		= new LinkedList<Button>();
			m_hideButtons		= new LinkedList<Button>();

			m_assetButtonList	= new LinkedList<Button>();
			m_lineList			= new LinkedList<Line>();
			m_objectPreview		= null;

			m_sndKeyclick		= new Sound("SoundEffects//GUI//button");
			m_sndSave			= new Sound("SoundEffects//GUI//ZMuFir00");

			m_test = new TextField(new Vector2(100, 100), 100, 100, true, true);
			m_guiList.AddLast(m_test);

			foreach (LinkedList<GameObject> t_GOArr in m_gameObjectList) {
				foreach (GameObject t_gameObject in t_GOArr) {
					if (t_gameObject is Player) {
						m_player = t_gameObject;
						break;
					}
				}
			}

			m_textCurrentMode				= new Text(new Vector2(12, 10), "null", "VerdanaBold", Color.Black, false);
			m_textSelectedObjectPosition	= new Text(new Vector2(12, 42), "Nothing Selected", "VerdanaBold", Color.Black, false);
			m_textGuardInfo					= new Text(new Vector2(12, 74), "", "VerdanaBold", Color.Black, false);
			m_layerInfo						= new Text(new Vector2(100, 100), (m_currentLayer + 1).ToString(), "VerdanaBold", Color.Black, false);
			m_guiList.AddLast(m_textCurrentMode);
			m_guiList.AddLast(m_textSelectedObjectPosition);
			m_guiList.AddLast(m_textGuardInfo);
			m_guiList.AddLast(m_layerInfo);

			m_UItextBackground = new GuiObject(new Vector2(0, 0), "dev_bg_info");
			m_guiList.AddLast(m_UItextBackground);

			Vector2 t_btnTextOffset = new Vector2(8, 50);
			Vector2 t_bottomRight = new Vector2(Game.getInstance().getResolution().X, Game.getInstance().getResolution().Y);
			/*
			-----------------------------------
			Buttons that are always shown
			 * m_staticKeys
			-----------------------------------
			*/
			m_btnSelectHotkey		= new Button("DevelopmentHotkeys//btn_select_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 5, t_bottomRight.Y - TILE_HEIGHT * 3), "S", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnDeleteHotkey		= new Button("DevelopmentHotkeys//btn_delete_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 4, t_bottomRight.Y - TILE_HEIGHT * 3), "D", "VerdanaBold", Color.White, t_btnTextOffset);
			
			m_staticButton.AddLast(m_btnSelectHotkey);
			m_staticButton.AddLast(m_btnDeleteHotkey);

			foreach (Button t_button in m_staticButton) {
				t_button.m_clickEvent += new Button.clickDelegate(guiButtonClick);
				t_button.setDownSound("button");
			}
			/*
			-----------------------------------
			Building mode buttons
			 * m_buildingKeys
			-----------------------------------
			*/
			m_btnLadderHotkey		= new Button("DevelopmentHotkeys//btn_ladder_hotkey", 
				new Vector2(t_bottomRight.X - TILE_WIDTH * 3, t_bottomRight.Y - TILE_HEIGHT * 2), "L", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnPlatformHotkey		= new Button("DevelopmentHotkeys//btn_platform_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 5, t_bottomRight.Y - TILE_HEIGHT * 2), "P", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnBackgroundHotkey	= new Button("DevelopmentHotkeys//btn_background_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 1, t_bottomRight.Y - TILE_HEIGHT * 1), "B", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnHeroHotkey			= new Button("DevelopmentHotkeys//btn_hero_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 1, t_bottomRight.Y - TILE_HEIGHT * 2), "H", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnSpotlightHotkey	= new Button("DevelopmentHotkeys//btn_spotlight_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 2, t_bottomRight.Y - TILE_HEIGHT * 2), "T", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnWallHotkey			= new Button("DevelopmentHotkeys//btn_wall_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 4, t_bottomRight.Y - TILE_HEIGHT * 2), "W", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnLightSwitchHotkey	= new Button("DevelopmentHotkeys//btn_spotlight_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 1, t_bottomRight.Y - TILE_HEIGHT * 3), "Shift+T", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnVentHotkey			= new Button("DevelopmentHotkeys//btn_ventilation_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 5, t_bottomRight.Y - TILE_HEIGHT * 1), "V", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnWindowHotkey		= new Button("DevelopmentHotkeys//btn_wall_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 2, t_bottomRight.Y - TILE_HEIGHT * 1), "N", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnGuardHotkey		= new Button("DevelopmentHotkeys//btn_guard_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 4, t_bottomRight.Y - TILE_HEIGHT * 1), "G", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnDuckHideHotkey			= new Button("DevelopmentHotkeys//btn_duckhide_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 3, t_bottomRight.Y - TILE_HEIGHT * 1), "A", "VerdanaBold", Color.White, t_btnTextOffset);

			m_buildingButtons.AddLast(m_btnLadderHotkey);
			m_buildingButtons.AddLast(m_btnPlatformHotkey);
			m_buildingButtons.AddLast(m_btnBackgroundHotkey);
			m_buildingButtons.AddLast(m_btnHeroHotkey);
			m_buildingButtons.AddLast(m_btnSpotlightHotkey);
			m_buildingButtons.AddLast(m_btnWallHotkey);
			m_buildingButtons.AddLast(m_btnDuckHideHotkey);
			m_buildingButtons.AddLast(m_btnLightSwitchHotkey);
			m_buildingButtons.AddLast(m_btnVentHotkey);
			m_buildingButtons.AddLast(m_btnGuardHotkey);
			m_buildingButtons.AddLast(m_btnWindowHotkey);

			foreach (Button t_button in m_buildingButtons) {
				t_button.m_clickEvent += new Button.clickDelegate(guiButtonClick);
				t_button.setDownSound("button");
			}
			/*
			-----------------------------------
			Ventilation buttons
			 * m_ventKeys
			-----------------------------------
			*/
			m_btnCrossVent	= new Button("DevelopmentHotkeys//btn_ventilation_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 5, t_bottomRight.Y - TILE_HEIGHT * 2), "T", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnCornerVent = new Button("DevelopmentHotkeys//btn_ventilation_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 4, t_bottomRight.Y - TILE_HEIGHT * 2), "A", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnTVent		= new Button("DevelopmentHotkeys//btn_ventilation_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 3, t_bottomRight.Y - TILE_HEIGHT * 2), "C", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnStraVent	= new Button("DevelopmentHotkeys//btn_ventilation_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 2, t_bottomRight.Y - TILE_HEIGHT * 2), "O", "VerdanaBold", Color.White, t_btnTextOffset);
			
			m_ventButtons.AddLast(m_btnCrossVent);
			m_ventButtons.AddLast(m_btnCornerVent);
			m_ventButtons.AddLast(m_btnTVent);
			m_ventButtons.AddLast(m_btnStraVent);

			foreach (Button t_button in m_ventButtons) {
				t_button.m_clickEvent += new Button.clickDelegate(guiButtonClick);
				t_button.setDownSound("button");
			}

			m_ventButtons.AddLast(m_btnVentHotkey);

			/*
			-----------------------------------
			Hiding object buttons
			-----------------------------------
			*/
			m_btnStandHideHotkey	= new Button("DevelopmentHotkeys//btn_standhide_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 5, t_bottomRight.Y - TILE_HEIGHT * 2), "F", "VerdanaBold", Color.White, t_btnTextOffset);

			m_hideButtons.AddLast(m_btnDuckHideHotkey);
			m_hideButtons.AddLast(m_btnStandHideHotkey);

			foreach (Button t_button in m_hideButtons) {
				t_button.m_clickEvent += new Button.clickDelegate(guiButtonClick);
				t_button.setDownSound("button");
			}

			m_hideButtons.AddLast(m_btnDuckHideHotkey);

			/*
			-----------------------------------
			Guard object buttons
			-----------------------------------
			*/
			m_btnDogHotkey			= new Button("DevelopmentHotkeys//btn_dog_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 5, t_bottomRight.Y - TILE_HEIGHT * 2), "F", "VerdanaBold", Color.White, t_btnTextOffset);
			m_btnCameraHotkey		= new Button("DevelopmentHotkeys//btn_camera_hotkey",
				new Vector2(t_bottomRight.X - TILE_WIDTH * 4, t_bottomRight.Y - TILE_HEIGHT * 2), "C", "VerdanaBold", Color.White, t_btnTextOffset);

			m_guardButtons.AddLast(m_btnDogHotkey);
			m_guardButtons.AddLast(m_btnCameraHotkey);

			foreach (Button t_button in m_guardButtons) {
				t_button.m_clickEvent += new Button.clickDelegate(guiButtonClick);
				t_button.setDownSound("button");
			}

			m_guardButtons.AddLast(m_btnGuardHotkey);
			/*
			-----------------------------------
			Layer buttons
			-----------------------------------
			*/
			m_layerButtonList = new LinkedList<Button>();
			for (int i = 5, j = 1; i > 0; i--, j++) {
				Button t_button = new Button(
					"DevelopmentHotkeys//btn_layer_chooser", 
					new Vector2(Game.getInstance().getResolution().X - (73 * i), 468), 
					j.ToString(), "VerdanaBold", Color.Black, new Vector2(34, 8)
				);
				t_button.m_clickEvent += new Button.clickDelegate(setLayer);
				m_layerButtonList.AddLast(t_button);
			}

			setBuildingState(State.None);

		}
		#endregion

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
		#endregion

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

			foreach (Button t_button in m_assetButtonList) {
				t_button.update();
			}
			switch (m_menuState) {
				case MenuState.Normal:
					foreach (Button t_button in m_buildingButtons) {
						t_button.update();
					}
					break;
				case MenuState.Guard:
					foreach (Button t_button in m_guardButtons) {
						t_button.update();
					}
					break;
				case MenuState.Hide:
					foreach (Button t_button in m_hideButtons) {
						t_button.update();
					}
					break;
				case MenuState.Ventilation:
					foreach (Button t_button in m_ventButtons) {
						t_button.update();
					}
					break;
			}
			foreach (Button t_button in m_staticButton) {
				t_button.update();
			}
			foreach (Button t_button in m_layerButtonList) {
				t_button.update();
			}
			foreach (GuiObject t_gui in m_guiList) {
				t_gui.update();
			}

			if (m_selectedObject != null) {
				m_selectedInfoV2.X = getTile(m_selectedObject.getPosition().getGlobalCartesianCoordinates()).X / TILE_WIDTH;
				m_selectedInfoV2.Y = getTile(m_selectedObject.getPosition().getGlobalCartesianCoordinates()).Y / TILE_HEIGHT;
				m_textSelectedObjectPosition.setText(m_selectedInfoV2.ToString());
				if (m_selectedObject is Guard) {
					Guard t_guard = (Guard)m_selectedObject;
					m_textGuardInfo.setText("R: " + t_guard.getRightPatrolPoint() / 72 + " L: " + t_guard.getLeftPatrolPoint() / 72);
				} else {
					m_textGuardInfo.setText("");
				}
			}
		}

		public void guiButtonClick(Button a_button) {
			if (!a_button.isButtonPressed()) {
				a_button.playDownSound();
			}
			if (a_button == m_btnSelectHotkey) {
				setBuildingState(State.None);
				return;
			}
			if (a_button == m_btnDeleteHotkey) {
				setBuildingState(State.Delete);
				return;
			}
			switch (m_menuState) {
				case MenuState.Normal:
					if (a_button == m_btnLadderHotkey) {
						setBuildingState(State.Ladder);
						return;
					}
					if (a_button == m_btnPlatformHotkey) {
						setBuildingState(State.Platform);
						return;
					}
					if (a_button == m_btnBackgroundHotkey) {
						setBuildingState(State.Background);
						return;
					}
					if (a_button == m_btnHeroHotkey) {
						setBuildingState(State.Player);
						return;
					}
					if (a_button == m_btnSpotlightHotkey) {
						setBuildingState(State.SpotLight);
						return;
					}
					if (a_button == m_btnGuardHotkey) {
						m_menuState = MenuState.Guard;
						setBuildingState(State.None);
						return;
					}
					if (a_button == m_btnWallHotkey) {
						setBuildingState(State.Wall);
						return;
					}
					if (a_button == m_btnDuckHideHotkey) {
						m_menuState = MenuState.Hide;
						setBuildingState(State.None);
						return;
					}
					if (a_button == m_btnLightSwitchHotkey) {
						setBuildingState(State.LightSwitch);
						return;
					}
					if (a_button == m_btnVentHotkey) {
						m_menuState = MenuState.Ventilation;
						setBuildingState(State.None);
						return;
					}
					if (a_button == m_btnWindowHotkey) {
						setBuildingState(State.Window);
						return;
					}
					break;
				case MenuState.Guard:
					if (a_button == m_btnGuardHotkey) {
						setBuildingState(State.Guard);
						return;
					}
					if (a_button == m_btnDogHotkey) {
						setBuildingState(State.GuardDog);
						return;
					}
					if (a_button == m_btnCameraHotkey) {
						setBuildingState(State.Camera);
						return;
					}
					break;
				case MenuState.Hide:
					if (a_button == m_btnDuckHideHotkey) {
						setBuildingState(State.DuckHidingObject);
						return;
					}
					if (a_button == m_btnStandHideHotkey) {
						setBuildingState(State.StandHidingObject);
						return;
					}
					break;
				case MenuState.Ventilation:
					if (a_button == m_btnCrossVent) {
						setBuildingState(State.CrossVent);
						return;
					}
					if (a_button == m_btnCornerVent) {
						setBuildingState(State.CornerVent);
						return;
					}
					if (a_button == m_btnTVent) {
						setBuildingState(State.TVent);
						return;
					}
					if (a_button == m_btnStraVent) {
						setBuildingState(State.StraVent);
						return;
					}
					if (a_button == m_btnVentHotkey) {
						setBuildingState(State.Ventrance);
						return;
					}
					break;
			}
		}

		private void createAssetList(string a_assetDirectory) {
			if (a_assetDirectory == null) {
				m_assetButtonList.Clear();
				return;
			}
			m_assetButtonList = new LinkedList<Button>();
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

		private void selectAsset(Button a_button)
		{
			assetToCreate = a_button.getText();
			foreach (Button t_button in m_assetButtonList)
				t_button.setState(0);
			a_button.setState(3);
			switch (m_itemToCreate) {
				case State.Platform:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//Floor//" + assetToCreate, 0.000f);
					break;
				case State.Wall:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//Wall//" + assetToCreate, 0.000f);
					break;
				case State.Delete:
					m_objectPreview = null;
					break;
				case State.Guard:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Sprite//Guard//" + assetToCreate, 0.000f);
					break;
				case State.Ladder:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//Ladder//" + assetToCreate, 0.000f);
					break;
				case State.None:
					m_objectPreview = null;
					break;
				case State.Player:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Sprite//Hero//" + assetToCreate, 0.000f);
					break;
				case State.SpotLight:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//LightCone//" + assetToCreate, 0.000f);
					break;
				case State.Background:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Background//" + assetToCreate, 0.000f);
					break;
				case State.DuckHidingObject:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Prop//DuckHide//" + assetToCreate, 0.000f);
					break;
				case State.StandHidingObject:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Prop//StandHide//" + assetToCreate, 0.000f);
					break;
				case State.GuardDog:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Sprite//GuardDog//" + assetToCreate, 0.000f);
					break;
				case State.LightSwitch:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Prop//Button//" + assetToCreate, 0.000f);
					break;
				case State.CrossVent:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//Ventilation//Cross//" + assetToCreate, 0.000f);
					break;
				case State.CornerVent:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//Ventilation//Corner//" + assetToCreate, 0.000f);
					break;
				case State.StraVent:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//Ventilation//Straight//" + assetToCreate, 0.000f);
					break;
				case State.TVent:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//Ventilation//TVent//" + assetToCreate, 0.000f);
					break;
				case State.Ventrance:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//Ventilation//Drum//" + assetToCreate, 0.000f);
					break;
				case State.Window:
					m_objectPreview = new Platform(new Vector2(m_worldMouse.X + 15, m_worldMouse.Y + 15), "Images//Tile//Window//" + assetToCreate, 0.000f);
					break;
			}
		}
		#endregion

		#region Update Keyboard
		private void updateKeyboard()
		{
			if (keyClicked(Keys.F5)) {
				m_currentLayer = 0;
				Game.getInstance().setState(new GameState(m_levelToLoad));
			}
			if (keyClicked(Keys.D1)) {
				setLayer(0);
			}
			if (keyClicked(Keys.D2)) {
				setLayer(1);
			}
			if (keyClicked(Keys.D3)) {
				setLayer(2);
			}
			if (keyClicked(Keys.D4)) {
				setLayer(3);
			}
			if (keyClicked(Keys.D5)) {
				setLayer(4);
			}
			/*
			-----------------------------------
			Keybindings for hotkeys
			-----------------------------------
			*/
			if (keyClicked(Keys.R)) {
				if (m_selectedObject != null) {
					if (m_selectedObject is Ladder) {
						m_selectedObject.flip();
						return;
					}
					m_selectedObject.addRotation((float)(Math.PI) / 2.0f);
				}
			}
			if (keyClicked(Keys.M)) {
				if (m_selectedObject != null) {
					if (m_selectedObject is LampSwitch) {
						((LampSwitch)m_selectedObject).toggleConnectToAll();
						showLightSwitchInfo((LampSwitch)m_selectedObject);
					}
				}
			}
			if (m_menuState != MenuState.Normal && keyClicked(Keys.Escape)) {
				m_menuState = MenuState.Normal;
				guiButtonClick(m_btnSelectHotkey);
			}
			if (ctrlMod()) {
				if (keyClicked(Keys.H)) {
					guiButtonClick(m_btnStandHideHotkey);
				}
				if (keyClicked(Keys.S)) {
					m_sndSave.play();
					if (m_selectedObject != null) {
						m_selectedObject.setColor(Color.White);
						m_selectedObject = null;
					}
					Level t_saveLevel = new Level();
					t_saveLevel.setLevelObjects(m_gameObjectList);
					Serializer.getInstance().SaveLevel(m_levelToLoad, t_saveLevel);
				}
			} else if (shiftMod()) {
				if (shiftMod() && keyClicked(Keys.G)) {
					guiButtonClick(m_btnDogHotkey);
				}
				if (shiftMod() && keyClicked(Keys.H)) {
					guiButtonClick(m_btnDuckHideHotkey);
				}
				if (shiftMod() && keyClicked(Keys.T)) {
					guiButtonClick(m_btnLightSwitchHotkey);
				}
			} else if (altMod()) {
				;
			} else {
				if (keyClicked(Keys.S)) {
					guiButtonClick(m_btnSelectHotkey);
				}
				if (keyClicked(Keys.D)) {
					guiButtonClick(m_btnDeleteHotkey);
				}
				switch (m_menuState) {
					case MenuState.Normal:
						if (keyClicked(Keys.P)) {
							guiButtonClick(m_btnPlatformHotkey);
						}
						if (keyClicked(Keys.L)) {
							guiButtonClick(m_btnLadderHotkey);
						}
						if (keyClicked(Keys.B)) {
							guiButtonClick(m_btnBackgroundHotkey);
						}
						if (keyClicked(Keys.H)) {
							guiButtonClick(m_btnHeroHotkey);
						}
						if (keyClicked(Keys.T)) {
							guiButtonClick(m_btnSpotlightHotkey);
						}
						if (keyClicked(Keys.G)) {
							guiButtonClick(m_btnGuardHotkey);
						}
						if (keyClicked(Keys.W)) {
							guiButtonClick(m_btnWallHotkey);
						}
						if (keyClicked(Keys.V)) {
							guiButtonClick(m_btnVentHotkey);
						}
						if (keyClicked(Keys.C)) {
							guiButtonClick(m_btnCameraHotkey);
						}
						if (keyClicked(Keys.A)) {
							guiButtonClick(m_btnDuckHideHotkey);
						}
						if (keyClicked(Keys.N)) {
							guiButtonClick(m_btnWindowHotkey);
						}
						break;
					case MenuState.Guard:
						if (keyClicked(Keys.G)) {
							guiButtonClick(m_btnGuardHotkey);
						}
						if (keyClicked(Keys.F)) {
							guiButtonClick(m_btnDogHotkey);
						}
						if (keyClicked(Keys.C)) {
							guiButtonClick(m_btnCameraHotkey);
						}
						break;
					case MenuState.Hide:
						if (keyClicked(Keys.F)) {
							guiButtonClick(m_btnStandHideHotkey);
						}
						if (keyClicked(Keys.A)) {
							guiButtonClick(m_btnDuckHideHotkey);
						}
						break;
					case MenuState.Ventilation:
						if (keyClicked(Keys.T)) {
							guiButtonClick(m_btnTVent);
						}
						if (keyClicked(Keys.A)) {
							guiButtonClick(m_btnStraVent);
						}
						if (keyClicked(Keys.C)) {
							guiButtonClick(m_btnCrossVent);
						}
						if (keyClicked(Keys.O)) {
							guiButtonClick(m_btnCornerVent);
						}
						if (keyClicked(Keys.V)) {
							guiButtonClick(m_btnVentHotkey);
						}
						break;
				}
			}

			/*
			-----------------------------------
			Reset Camera to 0, 0 
			-----------------------------------
			*/
			if (keyClicked(Keys.Space)) {
				if (m_gameObjectList != null) {
					Game.getInstance().m_camera.setPosition(Vector2.Zero);
				}
			}

			/*
			-----------------------------------
			Save and Load hotkeys 
			-----------------------------------
			*/ 

			if (ctrlMod() && keyClicked(Keys.O)) {
				Level t_newLevel = Serializer.getInstance().loadLevel(m_levelToLoad);
				m_gameObjectList = t_newLevel.getLevelLists();
				foreach (LinkedList<GameObject> t_arr in m_gameObjectList) {
					foreach (GameObject f_gb in t_arr) {
						f_gb.loadContent();
					}
				}
			}
		}
		#endregion

		#region Update Mouse
		private void updateMouse() {
			m_worldMouse = calculateWorldMouse();
			/*
			-----------------------------------
			Middle-mouse drag
			-----------------------------------
			*/
			if (m_currentMouse.MiddleButton == ButtonState.Pressed && m_previousMouse.MiddleButton == ButtonState.Pressed) {
				Vector2 t_difference = Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates();
				t_difference.X = (Mouse.GetState().X - Game.getInstance().getResolution().X / 2) / 20;
				t_difference.Y = (Mouse.GetState().Y - Game.getInstance().getResolution().Y / 2) / 20;
				Game.getInstance().m_camera.getPosition().plusWith(t_difference);
			}

			/*
			-----------------------------------
			Left Mouse Button Click Down
			-----------------------------------
			*/
			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Released) {
				/*
				-----------------------------------
				Building
				-----------------------------------
				*/
				if (m_building && !collidedWithGui(m_worldMouse)) {
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
							case State.CrossVent:
								createCrossVent();
								break;
							case State.CornerVent:
								createCornerVent();
								break;
							case State.StraVent:
								createStraightVent();
								break;
							case State.TVent:
								createTVent();
								break;
							case State.Ventrance:
								createVentrance();
								break;
							case State.Camera:
								createCamera();
								break;
							case State.Window:
								createWindow();
								break;
						}
					}
					return;
				}
				/*
				-----------------------------------
				Selecting
				----------------------------------- 
				*/
				if (!m_building && !collidedWithGui(m_worldMouse)) {
					if (m_selectedObject != null) {
						clearSelectedObject();
					}
					foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer]) {
						if (t_gameObject is LightCone || t_gameObject is FlashCone)
							continue;
						if (t_gameObject.getBox().Contains((int)m_worldMouse.X, (int)m_worldMouse.Y)) {
							if (m_selectedObject == null || m_selectedObject.getLayer() > t_gameObject.getLayer()) {
								m_selectedObject = t_gameObject;
							}
						}
					}
					if (m_selectedObject != null) {
						if (m_itemToCreate == State.Delete) {
							deleteObject(m_selectedObject);
							m_selectedInfoV2 = Vector2.Zero;
							m_selectedObject = null;
							return;
						} else if (m_selectedObject is Guard) {
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

			/*
			-----------------------------------
			Left Mouse Button Release
			-----------------------------------
			*/
			if (m_currentMouse.LeftButton == ButtonState.Released && m_previousMouse.LeftButton == ButtonState.Pressed) {
				m_dragOffset = Vector2.Zero;
			}

			/*
			-----------------------------------
			Left Mouse Button Drag
			-----------------------------------
			*/
			if (m_currentMouse.LeftButton == ButtonState.Pressed && m_previousMouse.LeftButton == ButtonState.Pressed) {
				if (m_selectedObject != null) {
					if (m_dragOffset == Vector2.Zero || m_dragOffset == null) {
						m_dragOffset = new Vector2(
							m_worldMouse.X - m_selectedObject.getPosition().getGlobalX(),
							m_worldMouse.Y - m_selectedObject.getPosition().getGlobalY()
						);
					}
					
					Vector2 t_mousePosition = getTile(m_worldMouse - m_dragOffset);

					if (m_selectedObject is SpotLight)
						m_selectedObject.getPosition().setX(t_mousePosition.X + m_selectedObject.getBox().Width);
					else
						m_selectedObject.getPosition().setX(t_mousePosition.X);
					m_selectedObject.getPosition().setY(t_mousePosition.Y);
				}
			}

			/*
			-----------------------------------
			Right Mouse Button Drag
			-----------------------------------
			*/
			if (m_currentMouse.RightButton == ButtonState.Pressed && m_previousMouse.RightButton == ButtonState.Pressed) {
				if (m_selectedObject != null) {
					if (m_selectedObject is LampSwitch) {
						if (m_dragLine == null && m_selectedObject.getBox().Contains((int)m_worldMouse.X, (int)m_worldMouse.Y)) {
							m_dragLine = new Line(m_selectedObject.getPosition(), new CartesianCoordinate(m_worldMouse), new Vector2(36, 36), Vector2.Zero, Color.Yellow, 5, true);
						}
						else if (m_dragLine != null)
						{
							m_dragLine.setEndpoint(m_worldMouse);
						}
					}
					if (m_selectedObject is Guard || m_selectedObject is GuardDog) {
						if (m_dragLine == null && m_selectedObject.getBox().Contains((int)m_worldMouse.X, (int)m_worldMouse.Y)) {
							m_dragLine = new Line(m_selectedObject.getPosition(), new CartesianCoordinate(new Vector2(m_worldMouse.X, m_selectedObject.getPosition().getGlobalY() + 36)), new Vector2(36, 36), Vector2.Zero, Color.Green, 5, true);
						} 
						else if(m_dragLine != null)
						{
							m_dragLine.setEndpoint(new Vector2(m_worldMouse.X, m_selectedObject.getPosition().getGlobalY() + 36));
						}
					}
				}
			}

			/*
			-----------------------------------
			Right Mouse Button Click Up
			-----------------------------------
			*/
			if (m_currentMouse.RightButton == ButtonState.Released && m_previousMouse.RightButton == ButtonState.Pressed) {
				if (m_dragLine != null) {
					if (m_selectedObject is LampSwitch) {
						foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer]) {
							if (t_gameObject is SpotLight && t_gameObject.getBox().Contains((int)m_worldMouse.X, (int)m_worldMouse.Y)) {
								connectSpotLight((SpotLight)t_gameObject, (LampSwitch)m_selectedObject);
								break;
							}
						}
						showLightSwitchInfo((LampSwitch)m_selectedObject);
					} else if (m_selectedObject is Guard) {
						if (m_worldMouse.X > m_selectedObject.getPosition().getGlobalX()) {
							setGuardPoint((Guard)m_selectedObject, true);
						} else {
							setGuardPoint((Guard)m_selectedObject, false);
						}
						showGuardInfo((Guard)m_selectedObject);
					} else if (m_selectedObject is GuardDog) {
						if (m_worldMouse.X > m_selectedObject.getPosition().getGlobalX()) {
							setGuardPoint((GuardDog)m_selectedObject, true);
						} else {
							setGuardPoint((GuardDog)m_selectedObject, false);
						}
						showDogInfo((GuardDog)m_selectedObject);
					}
					m_dragLine = null;
				} else {
					m_ventilation = false;
					clearSelectedObject();
					setBuildingState(State.None);
				}
				m_dragLine = null;
			}

			/*
			-----------------------------------
			Right Mouse Button Down 
			-----------------------------------
			*/
		}
		#endregion

		#region Collision Check
		private bool collidedWithGui(Vector2 a_coordinate)
		{
			foreach (GuiObject t_guiObject in m_guiList)
				if (t_guiObject.getBox().Contains((int)a_coordinate.X, (int)a_coordinate.Y))
					return true;
			if (m_ventilation) {
				foreach (Button t_button in m_ventButtons) {
					if (t_button.getBox().Contains((int)Mouse.GetState().X, (int)Mouse.GetState().Y))
						return true;
				}
			} else {
				foreach (Button t_button in m_buildingButtons) {
					if (t_button.getBox().Contains((int)Mouse.GetState().X, (int)Mouse.GetState().Y))
						return true;
				}
			}
			foreach (Button t_button in m_assetButtonList)
				if (t_button.getBox().Contains((int)Mouse.GetState().X, (int)Mouse.GetState().Y))
					return true;
			foreach (Button t_button in m_layerButtonList)
				if (t_button.getBox().Contains((int)Mouse.GetState().X, (int)Mouse.GetState().Y))
					return true;
			return false;
		}

		public bool collidedWithObject(Vector2 a_coordinate) {
			Rectangle t_rectangle = new Rectangle((int)getTile(a_coordinate).X, (int)getTile(a_coordinate).Y, 1, 1);

			foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer]) {
				if (t_gameObject is Environment || t_gameObject is LightCone)
					continue;
				if (t_gameObject.getBox().Contains(t_rectangle))
					return true;
			}
			return false;
		}
		#endregion+
		
		#region Development Methods
		private void clearSelectedObject() {
			if (m_selectedObject != null) {
				m_selectedObject.setColor(Color.White);
				m_selectedObject = null;
				m_selectedInfoV2 = Vector2.Zero;
			}
			m_objectPreview = null;
			m_lineList.Clear();
		}

		public Vector2 calculateWorldMouse() {
			Vector2 t_worldMouse = new Vector2(
				Mouse.GetState().X / Game.getInstance().m_camera.getZoom()
					+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().X
					- ((Game.getInstance().getResolution().X / 2) / Game.getInstance().m_camera.getZoom())
				, Mouse.GetState().Y / Game.getInstance().m_camera.getZoom() 
					+ (int)Game.getInstance().m_camera.getPosition().getGlobalCartesianCoordinates().Y
					- ((Game.getInstance().getResolution().Y / 2) / Game.getInstance().m_camera.getZoom())
			);
			return t_worldMouse;
		}

		private Vector2 getTile(Vector2 a_pixelPosition) {
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

		private bool ctrlMod() {
			return m_currentKeyboard.IsKeyDown(Keys.LeftControl) || m_currentKeyboard.IsKeyDown(Keys.RightControl);
		}
		private bool shiftMod() {
			return m_currentKeyboard.IsKeyDown(Keys.LeftShift) || m_currentKeyboard.IsKeyDown(Keys.RightShift);
		}
		private bool altMod() {
			return m_currentKeyboard.IsKeyDown(Keys.LeftAlt) || m_currentKeyboard.IsKeyDown(Keys.RightAlt);
		}

		private void setBuildingState(State a_state) {
			m_building			= true;
			if (a_state != m_itemToCreate) {
				clearSelectedObject();
				m_itemToCreate = a_state;
			}
			assetToCreate		= null;
			m_objectPreview		= null;

			foreach (Button t_button in m_staticButton)
				t_button.setState(0);
			switch (m_menuState) {
				case MenuState.Normal:
					foreach (Button t_button in m_buildingButtons)
						t_button.setState(0);
					break;
				case MenuState.Guard:
					foreach (Button t_button in m_guardButtons)
						t_button.setState(0);
					break;
				case MenuState.Hide:
					foreach (Button t_button in m_hideButtons)
						t_button.setState(0);
					break;
				case MenuState.Ventilation:
					foreach (Button t_button in m_ventButtons)
						t_button.setState(0);
					break;
			}
			foreach (Button t_button in m_ventButtons)
				t_button.setState(0);

			switch (m_itemToCreate) {
				case State.Platform:
					m_textCurrentMode.setText("Create Platform");
					createAssetList("Content//Images//Tile//Floor//");
					m_btnPlatformHotkey.setState(3);
					break;
				case State.Ladder:
					m_textCurrentMode.setText("Create Ladder");
					createAssetList("Content//Images//Tile//Ladder//");
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
					m_building = false;
					break;
				case State.Player:
					m_textCurrentMode.setText("Create Hero");
					createAssetList("Content//Images//Sprite//Hero//");
					m_btnHeroHotkey.setState(3);
					break;
				case State.None:
					m_textCurrentMode.setText("Select");
					createAssetList(null);
					m_btnSelectHotkey.setState(3);
					m_building = false;
					break;
				case State.SpotLight:
					m_textCurrentMode.setText("Create SpotLight");
					createAssetList("Content//Images//LightCone//");
					m_btnSpotlightHotkey.setState(3);
					break;
				case State.Guard:
					m_textCurrentMode.setText("Create Guard");
					createAssetList("Content//Images//Sprite//Guard//");
					m_btnGuardHotkey.setState(3);
					break;
				case State.Wall:
					m_textCurrentMode.setText("Create Wall");
					createAssetList("Content//Images//Tile//Wall//");
					m_btnWallHotkey.setState(3);
					break;
				case State.DuckHidingObject:
					m_textCurrentMode.setText("Ducking Hide Object");
					createAssetList("Content//Images//Prop//DuckHide//");
					m_btnDuckHideHotkey.setState(3);
					break;
				case State.StandHidingObject:
					m_textCurrentMode.setText("Standing Hide Object");
					createAssetList("Content//Images//Prop//StandHide//");
					m_btnStandHideHotkey.setState(3);
					break;
				case State.GuardDog:
					m_textCurrentMode.setText("Create Guard Dog");
					createAssetList("Content//Images//Sprite//GuardDog//");
					m_btnDogHotkey.setState(3);
					break;
				case State.LightSwitch:
					m_textCurrentMode.setText("Create Light Switch");
					createAssetList("Content//Images//Prop//Button//");
					m_btnLightSwitchHotkey.setState(3);
					break;
				case State.CrossVent:
					m_textCurrentMode.setText("Cross Ventilation");
					createAssetList("Content//Images//Tile//Ventilation//Cross//");
					m_btnCrossVent.setState(3);
					break;
				case State.CornerVent:
					m_textCurrentMode.setText("Ventilation Corner");
					createAssetList("Content//Images//Tile//Ventilation//Corner//");
					m_btnCornerVent.setState(3);
					break;
				case State.TVent:
					m_textCurrentMode.setText("T-ventilation");
					createAssetList("Content//Images//Tile//Ventilation//TVent//");
					m_btnTVent.setState(3);
					break;
				case State.StraVent:
					m_textCurrentMode.setText("Straight Ventilation");
					createAssetList("Content//Images//Tile//Ventilation//Straight//");
					m_btnStraVent.setState(3);
					break;
				case State.Ventrance:
					m_textCurrentMode.setText("Ventilation Entrance");
					createAssetList("Content//Images//Tile//Ventilation//Drum//");
					m_btnVentHotkey.setState(3);
					break;
				case State.Camera:
					m_textCurrentMode.setText("Create Camera");
					createAssetList("Content//Images//Sprite//Camera//");
					m_btnCameraHotkey.setState(3);
					break;
				case State.Window:
					m_textCurrentMode.setText("Create Window");
					createAssetList("Content//Images//Tile//Window//");
					m_btnWindowHotkey.setState(3);
					break;
			}
			if (m_assetButtonList != null && m_assetButtonList.Count > 0) {
				selectAsset(m_assetButtonList.First());
			}
		}

		private void showGuardInfo(Guard a_guard) {
			m_lineList.Clear();
			m_textGuardInfo.setText(" L: " + a_guard.getLeftPatrolPoint() + "R: " + a_guard.getRightPatrolPoint());
			m_lineList.AddLast(new Line(a_guard.getPosition(), new CartesianCoordinate(new Vector2(a_guard.getLeftPatrolPoint(), a_guard.getPosition().getGlobalY())), new Vector2(36, 72), new Vector2(36, 72), Color.Green, 5, true));
			m_lineList.AddLast(new Line(a_guard.getPosition(), new CartesianCoordinate(new Vector2(a_guard.getRightPatrolPoint(), a_guard.getPosition().getGlobalY())), new Vector2(36, 72), new Vector2(36, 72), Color.Green, 5, true));
		}

		private void showDogInfo(GuardDog a_guard) {
			m_lineList.Clear();
			m_textGuardInfo.setText(" L: " + a_guard.getLeftpatrolPoint() + "R: " + a_guard.getRightpatrolPoint());
			m_lineList.AddLast(new Line(a_guard.getPosition(), new CartesianCoordinate(new Vector2(a_guard.getLeftpatrolPoint(), a_guard.getPosition().getGlobalY())), new Vector2(36, 72), new Vector2(36, 72), Color.Green, 5, true));
			m_lineList.AddLast(new Line(a_guard.getPosition(), new CartesianCoordinate(new Vector2(a_guard.getRightpatrolPoint(), a_guard.getPosition().getGlobalY())), new Vector2(36, 72), new Vector2(36, 72), Color.Green, 5, true));
		}

		private void showLightSwitchInfo(LampSwitch a_lightswitch) {
			m_lineList.Clear();
			foreach (SpotLight t_spotLight in a_lightswitch.getConnectedSpotLights()) {
				m_lineList.AddLast(new Line(a_lightswitch.getPosition(), t_spotLight.getPosition(), new Vector2(36, 36), new Vector2(0, 0), Color.Yellow, 5, true));
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

		private void setLayer(Button a_button) {
			
			foreach (Button t_button in m_layerButtonList) {
				t_button.setState(0);
			}
			m_currentLayer = int.Parse(a_button.getText()) - 1;
			a_button.setState(3);
			m_layerInfo.setText((m_currentLayer + 1).ToString());
		}

		private void setLayer(int a_layer) {
			foreach (Button t_button in m_layerButtonList) {
				if (int.Parse(t_button.getText()) == m_currentLayer) {

				}
			}
		}

		public override void addObject(GameObject a_object)
		{
			m_gameObjectList[m_currentLayer].AddLast(a_object);
		}
		public override void addObject(GameObject a_object, int a_layer)
		{
			m_gameObjectList[a_layer].AddLast(a_object);
		}
		public override void removeObject(GameObject a_object)
		{
			m_gameObjectList[m_currentLayer].Remove(a_object);
		}
		public override void removeObject(GameObject a_object, int a_layer)
		{
			m_gameObjectList[a_layer].Remove(a_object);
		}
		public override void addGuiObject(GuiObject a_go)
		{
			m_guiList.AddLast(a_go);
		}

		internal override GameObject getObjectById(int a_id)
		{
			foreach (LinkedList<GameObject> t_goList in m_gameObjectList)
			{
				foreach (GameObject t_go in t_goList)
				{
					if (a_id == t_go.getId())
					{
						return t_go;
					}
				}
			}
			return null;
		}

		public override LinkedList<GameObject>[] getObjectList()
		{
			return m_gameObjectList;
		}
		#endregion

		#region Create-methods
		private void deleteObject(GameObject a_gameObject) {
			if (a_gameObject is Player) {
				m_player = null;
			}
			a_gameObject.kill();
			if (a_gameObject is SpotLight) {
				LightCone t_lightCone = ((SpotLight)a_gameObject).getLightCone();
				for (int i = 0; i < m_gameObjectList.Length; i++) {
					foreach (GameObject t_gameObject in m_gameObjectList[i]) {
						if (t_gameObject is LampSwitch && ((LampSwitch)t_gameObject).isConnectedTo((SpotLight)a_gameObject)) {
							((LampSwitch)t_gameObject).disconnectSpotLight((SpotLight)a_gameObject);
						}
					}
				}
				if (t_lightCone != null) {
					for (int i = 0; i < m_gameObjectList.Length; i++) {
						m_gameObjectList[i].Remove(t_lightCone);
					}
				}
			}
			m_lineList.Clear();
			m_gameObjectList[m_currentLayer].Remove(a_gameObject);
		}
		private void createPlayer() {
			if (m_player == null) {
				if (collidedWithObject(m_worldMouse))
					return;
				addObject(new Player(getTile(m_worldMouse), "Images//Sprite//Hero//" + assetToCreate, 0.300f));
			}
		}

		private void createPlatform() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new Platform(getTile(m_worldMouse), "Images//Tile//Floor//" + assetToCreate, 0.350f));
		}

		private void createLadder() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new Ladder(getTile(m_worldMouse), "Images//Tile//Ladder//" + assetToCreate, 0.350f));
		}

		private void createSpotLight() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new SpotLight(getTile(m_worldMouse), "Images//LightCone//"  + assetToCreate, 0.200f, (float)(Math.PI * 0.5f), true));
		}

		private void createBackground() {
			addObject(new Environment(getTile(m_worldMouse), "Images//Background//"  + assetToCreate, 0.999f));
		}

		private void createGuard() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new Guard(getTile(m_worldMouse), "Images//Sprite//Guard//" + assetToCreate, getTile(m_worldMouse).X, true, 0.250f));
		}

		private void createWall() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new Wall(getTile(m_worldMouse), "Images//Tile//Wall//" + assetToCreate, 0.350f));
		}

		private void createDuckHidingObject() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new DuckHideObject(getTile(m_worldMouse), "Images//Prop//DuckHide//" + assetToCreate, 0.700f));
		}

		private void createStandHideObject() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new StandHideObject(getTile(m_worldMouse), "Images//Prop//StandHide//" + assetToCreate, 0.700f));
		}

		private void createGuardDog() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new GuardDog(getTile(m_worldMouse), "Images//Sprite//GuardDog//" + assetToCreate, getTile(m_worldMouse).X, getTile(m_worldMouse).X, 0.299f));
		}

		private void createLightSwitch() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new LampSwitch(getTile(m_worldMouse), "Images//Prop//Button//" + assetToCreate, 0.750f));
		}

		private void createCrossVent() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new CrossVentilation(getTile(m_worldMouse), "Images//Tile//Ventilation//Cross//" + assetToCreate, 0.700f));
		}
		private void createTVent() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new TVentilation(getTile(m_worldMouse), "Images//Tile//Ventilation//TVent//" + assetToCreate, 0.700f));
		}
		private void createStraightVent() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new StraightVentilation(getTile(m_worldMouse), "Images//Tile//Ventilation//Straight//" + assetToCreate, 0.700f));
		}
		private void createCornerVent() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new CornerVentilation(getTile(m_worldMouse), "Images//Tile//Ventilation//Corner//" + assetToCreate, 0.700f));
		}
		private void createVentrance() {
			if (collidedWithObject(m_worldMouse))
				return;
			addObject(new VentilationDrum(getTile(m_worldMouse), "Images//Tile//Ventilation//Drum//" + assetToCreate, 0.700f));
		}

		private void createCamera()
		{
			if (!collidedWithObject(m_worldMouse))
			{
				addObject(new GuardCamera(getTile(m_worldMouse), "Images//Sprite//Camera//" + assetToCreate, 0.200f, (float)(Math.PI * 0.5), (float)(Math.PI * 0.25), (float)(Math.PI * 0.75)));
			}
		}
		private void createWindow() {
			 if (!collidedWithObject(m_worldMouse))
				addObject(new Window(getTile(m_worldMouse), "Images//Tile//Window//" + assetToCreate, 0.700f));
		}
		#endregion

		#region Draw
		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (GuiObject t_guiObject in m_guiList)
				t_guiObject.draw(a_gameTime);
			foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer])
				t_gameObject.draw(a_gameTime);
			foreach (Button t_button in m_staticButton)
				t_button.draw(a_gameTime, a_spriteBatch);

			switch (m_menuState) {
				case MenuState.Normal:
					foreach (Button t_button in m_buildingButtons) {
						t_button.draw(a_gameTime, a_spriteBatch);
					}
					break;
				case MenuState.Guard:
					foreach (Button t_button in m_guardButtons) {
						t_button.draw(a_gameTime, a_spriteBatch);
					}
					break;
				case MenuState.Hide:
					foreach (Button t_button in m_hideButtons) {
						t_button.draw(a_gameTime, a_spriteBatch);
					}
					break;
				case MenuState.Ventilation:
					foreach (Button t_button in m_ventButtons) {
						t_button.draw(a_gameTime, a_spriteBatch);
					}
					break;
			}

			foreach (Button t_button in m_assetButtonList)
				t_button.draw(a_gameTime, a_spriteBatch);
			foreach (Button t_button in m_layerButtonList)
				t_button.draw(a_gameTime, a_spriteBatch);
			foreach (Line t_line in m_lineList)
				t_line.draw();
			if (m_objectPreview != null)
				m_objectPreview.draw(a_gameTime);
			if (m_dragLine != null)
				m_dragLine.draw();
		}
		#endregion
	}
}