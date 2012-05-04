using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text.RegularExpressions;
using GrandLarceny.Events;

namespace GrandLarceny
{
	public class DevelopmentState : States
	{
		#region Members
		private LinkedList<Event> m_events;

		private LinkedList<Button> m_staticButton;
		private LinkedList<Button> m_buildingButtons;
		private LinkedList<Button> m_ventButtons;
		private LinkedList<Button> m_guardButtons;
		private LinkedList<Button> m_hideButtons;
		private LinkedList<Button> m_collButtons;
		private LinkedList<Button> m_assetButtonList;
		private LinkedList<Button> m_layerButtonList;
		private LinkedList<Line> m_lineList;

		private GameObject m_selectedObject;
		private GameObject m_objectPreview;
		private GameObject m_player;
		private GameObject m_copyTarget;

		private Vector2 m_selectedInfoV2;
		private Vector2 m_worldMouse;
		private Vector2 m_dragOffset;
		private Vector2 m_dragFrom;
		private bool m_firstDrag = true;

		private Text m_textObjectInfo;
		private Text m_textGuardInfo;

		private Line m_leftGuardPoint;
		private Line m_rightGuardPoint;

		private Box m_statusBar;

		private Text m_layerLabel;
		private Text m_parallaxLabel;
		private TextField m_layerTextField;
		private TextField m_parallaxScrollTF;
		
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
		private Button m_btnForegroundHotkey;
		private Button m_btnRopeHotkey;
		private Button m_btnSecDoorHotkey;
		private Button m_btnCornerHangHotkey;
		private Button m_btnCheckPointHotkey;
		private Button m_btnPropHotkey;

		/*
		-----------------------------------
		Ventilation buttons
		-----------------------------------
		*/
		private Button m_btnCrossVentHotkey;
		private Button m_btnCornerVentHotkey;
		private Button m_btnTVentHotkey;
		private Button m_btnStraVentHotkey;
		private Button m_btnEndVentHotkey;

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
		
		/*
		-----------------------------------
		Guard buttons
		-----------------------------------
		*/
		private Button m_btnCollHotkey;
		private Button m_btnHeartHotkey;

		private Line m_dragLine = null;

		private Sound m_sndKeyclick;
		private Sound m_sndSave;

		private int m_currentLayer = 0;
		private string m_levelToLoad;
		private bool m_building;

		private State m_itemToCreate;
		private enum State
		{
			Platform,		Player,		Background,		Foreground,
			Ladder,			SpotLight,	LightSwitch,	Delete,
			None,			Guard,		GuardDog,		Wall,
			Ventilation,	Camera,		CrossVent,		TVent,
			StraVent,		CornerVent, Ventrance,		Window,
			DuckHidingObject,		StandHidingObject,	Rope,
			SecDoor,		CornerHang,	Checkpoint,		Prop,
			Heart,			Key,		EndVent
		}

		private MenuState m_menuState;
		private enum MenuState {
			Ventilation,	Guard,		Hide,		Inactive,
			Normal,			Collectible
		}
		#endregion

		#region Load&Initiate
		public DevelopmentState(string a_levelToLoad)
		{
			m_levelToLoad = a_levelToLoad;
			Game.getInstance().m_camera.getPosition().setParentPosition(null);
		}

		public override void load()
		{
			if (File.Exists("Content\\levels\\" + m_levelToLoad))
			{
				Level t_loadedLevel = Loader.getInstance().loadLevel(m_levelToLoad);

				m_gameObjectList = t_loadedLevel.getGameObjects();
				m_events = t_loadedLevel.getEvents();
			}
			else
			{
				m_events = new LinkedList<Event>();
				for (int i = 0; i < m_gameObjectList.Length; ++i)
				{
					m_gameObjectList[i] = new LinkedList<GameObject>();
				}
			}

			foreach (LinkedList<GameObject> t_ll in m_gameObjectList)
			{
				foreach (GameObject t_go in t_ll)
				{
					t_go.loadContent();
					if (t_go is Player) {
						m_player = t_go;
						Game.getInstance().m_camera.setPosition(m_player.getPosition().getGlobalCartesian());
					}
				}
			}

			foreach (Event t_e in m_events)
			{
				t_e.loadContent();
			}
			m_buttonList.AddLast(m_buildingButtons	= new LinkedList<Button>());
			m_buttonList.AddLast(m_staticButton		= new LinkedList<Button>());
			m_buttonList.AddLast(m_ventButtons		= new LinkedList<Button>());
			m_buttonList.AddLast(m_guardButtons		= new LinkedList<Button>());
			m_buttonList.AddLast(m_hideButtons		= new LinkedList<Button>());
			m_buttonList.AddLast(m_assetButtonList	= new LinkedList<Button>());
			m_buttonList.AddLast(m_collButtons		= new LinkedList<Button>());

			m_lineList			= new LinkedList<Line>();
			m_objectPreview		= null;

			m_sndKeyclick		= new Sound("SoundEffects//GUI//button");
			m_sndSave			= new Sound("SoundEffects//GUI//ZMuFir00");

			m_guiList.AddLast(m_layerLabel			= new Text(new Vector2(350, 3)	, "Layer:", "VerdanaBold", Color.Black, false));
			m_guiList.AddLast(m_parallaxLabel		= new Text(new Vector2(460, 3)	, "Parallax Value:", "VerdanaBold", Color.Black, false));
			m_guiList.AddLast(m_textObjectInfo		= new Text(new Vector2(10, 3)	, "", "VerdanaBold", Color.Black, false));
			m_guiList.AddLast(m_textGuardInfo		= new Text(new Vector2(480, 3)	, "", "VerdanaBold", Color.Black, false));
			m_guiList.AddLast(m_layerTextField		= new TextField(new Vector2(400, 0), 50, 25, false, true, false, 3));
			m_guiList.AddLast(m_parallaxScrollTF	= new TextField(new Vector2(580, 0), 70, 25, false, true, false, 3));
			m_statusBar	= new Box(new Vector2(0, 0), (int)Game.getInstance().getResolution().X, 25, Color.LightGray, false);
			m_statusBar.setLayer(0.111f);

			Vector2 t_btnTextOffset = new Vector2(21, 17);
			Vector2 t_modV2 = new Vector2(15, 0);
			//-----------------------------------
			#region Buttons that are always shown
			m_staticButton.AddLast(m_btnSelectHotkey = new Button("DevelopmentHotkeys//btn_select_hotkey", new Vector2(0, 32 * m_staticButton.Count() + 25), "S", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnDeleteHotkey = new Button("DevelopmentHotkeys//btn_delete_hotkey", new Vector2(0, 32 * m_staticButton.Count() + 25), "D", "VerdanaBold", Color.Black, t_btnTextOffset));

			m_btnSelectHotkey.setHotkey(new Keys[] { Keys.S });
			m_btnDeleteHotkey.setHotkey(new Keys[] { Keys.D });
			#endregion
			//-----------------------------------
			
			//-----------------------------------
			#region Building mode buttons
			m_staticButton.AddLast(m_btnLadderHotkey = new Button("DevelopmentHotkeys//btn_ladder_hotkey",			new Vector2(0, 32 * m_staticButton.Count() + 25), "L", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnPlatformHotkey	= new Button("DevelopmentHotkeys//btn_platform_hotkey",		new Vector2(0, 32 * m_staticButton.Count() + 25), "P", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnBackgroundHotkey = new Button("DevelopmentHotkeys//btn_background_hotkey",	new Vector2(0, 32 * m_staticButton.Count() + 25), "B", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnHeroHotkey = new Button("DevelopmentHotkeys//btn_hero_hotkey",				new Vector2(0, 32 * m_staticButton.Count() + 25), "H", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnSpotlightHotkey = new Button("DevelopmentHotkeys//btn_spotlight_hotkey",	new Vector2(0, 32 * m_staticButton.Count() + 25), "T", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnWallHotkey = new Button("DevelopmentHotkeys//btn_wall_hotkey",				new Vector2(0, 32 * m_staticButton.Count() + 25), "W", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnLightSwitchHotkey= new Button("DevelopmentHotkeys//btn_lightswitch_hotkey", new Vector2(0, 32 * m_staticButton.Count() + 25), "s+T", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2));
			m_staticButton.AddLast(m_btnVentHotkey = new Button("DevelopmentHotkeys//btn_ventilation_hotkey",		new Vector2(0, 32 * m_staticButton.Count() + 25), "V", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnWindowHotkey = new Button("DevelopmentHotkeys//btn_window_hotkey",			new Vector2(0, 32 * m_staticButton.Count() + 25), "N", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnGuardHotkey = new Button("DevelopmentHotkeys//btn_guard_hotkey",			new Vector2(0, 32 * m_staticButton.Count() + 25), "G", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnDuckHideHotkey = new Button("DevelopmentHotkeys//btn_duckhide_hotkey",		new Vector2(0, 32 * m_staticButton.Count() + 25), "A", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnForegroundHotkey = new Button("DevelopmentHotkeys//btn_foreground_hotkey",	new Vector2(0, 32 * m_staticButton.Count() + 25), "F", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnRopeHotkey = new Button("DevelopmentHotkeys//btn_rope_hotkey",				new Vector2(0, 32 * m_staticButton.Count() + 25), "O", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnSecDoorHotkey = new Button("DevelopmentHotkeys//btn_secdoor_hotkey",		new Vector2(0, 32 * m_staticButton.Count() + 25), "E", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnCornerHangHotkey = new Button("DevelopmentHotkeys//btn_doorhang_hotkey",	new Vector2(0, 32 * m_staticButton.Count() + 25), "s+W", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2));
			m_staticButton.AddLast(m_btnCheckPointHotkey = new Button("DevelopmentHotkeys//btn_checkpoint_hotkey",	new Vector2(0, 32 * m_staticButton.Count() + 25), "K", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnPropHotkey = new Button("DevelopmentHotkeys//btn_clutter_hotkey",			new Vector2(0, 32 * m_staticButton.Count() + 25), "C", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_staticButton.AddLast(m_btnCollHotkey = new Button("DevelopmentHotkeys//btn_key_hotkey",				new Vector2(0, 32 * m_staticButton.Count() + 25), "Z", "VerdanaBold", Color.Black, t_btnTextOffset));
			m_btnLadderHotkey.setHotkey(new Keys[]		{ Keys.L });
			m_btnPlatformHotkey.setHotkey(new Keys[]	{ Keys.P });
			m_btnBackgroundHotkey.setHotkey(new Keys[]	{ Keys.B });
			m_btnHeroHotkey.setHotkey(new Keys[]		{ Keys.H });
			m_btnSpotlightHotkey.setHotkey(new Keys[]	{ Keys.T });
			m_btnWallHotkey.setHotkey(new Keys[]		{ Keys.W });
			m_btnLightSwitchHotkey.setHotkey(new Keys[] { Keys.LeftShift, Keys.T });
			m_btnVentHotkey.setHotkey(new Keys[]		{ Keys.V });
			m_btnWindowHotkey.setHotkey(new Keys[]		{ Keys.N });
			m_btnGuardHotkey.setHotkey(new Keys[]		{ Keys.G });
			m_btnDuckHideHotkey.setHotkey(new Keys[]	{ Keys.A });
			m_btnForegroundHotkey.setHotkey(new Keys[]	{ Keys.F });
			m_btnRopeHotkey.setHotkey(new Keys[]		{ Keys.O });
			m_btnSecDoorHotkey.setHotkey(new Keys[]		{ Keys.E });
			m_btnCornerHangHotkey.setHotkey(new Keys[]	{ Keys.LeftShift, Keys.W });
			m_btnCheckPointHotkey.setHotkey(new Keys[]	{ Keys.K });
			m_btnPropHotkey.setHotkey(new Keys[]		{ Keys.C });
			m_btnCollHotkey.setHotkey(new Keys[]		{ Keys.Z });
									
			foreach (Button t_button in m_staticButton) {
				t_button.m_clickEvent += new Button.clickDelegate(guiButtonClick);
				t_button.setDownSound("button");
			}
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Ventilation buttons
			Vector2 t_ventMenu = new Vector2(m_btnVentHotkey.getBox().X + 32, m_btnVentHotkey.getBox().Y);
			m_ventButtons.AddLast(m_btnTVentHotkey		= new Button("DevelopmentHotkeys//btn_tvent_hotkey",
				t_ventMenu + new Vector2(m_ventButtons.Count * 32, 0), "s+V", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2));
			m_ventButtons.AddLast(m_btnStraVentHotkey	= new Button("DevelopmentHotkeys//btn_svent_hotkey",
				t_ventMenu + new Vector2(m_ventButtons.Count * 32, 0), "s+A", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2));
			m_ventButtons.AddLast(m_btnCrossVentHotkey	= new Button("DevelopmentHotkeys//btn_cvent_hotkey",
				t_ventMenu + new Vector2(m_ventButtons.Count * 32, 0), "s+C", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2));
			m_ventButtons.AddLast(m_btnCornerVentHotkey = new Button("DevelopmentHotkeys//btn_ovent_hotkey",
				t_ventMenu + new Vector2(m_ventButtons.Count * 32, 0), "s+R", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2));
			m_ventButtons.AddLast(m_btnEndVentHotkey	= new Button(null,
				t_ventMenu + new Vector2(m_ventButtons.Count * 32, 0), "s+E", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2));

			m_btnTVentHotkey.setHotkey(new Keys[]		{ Keys.LeftShift, Keys.V });
			m_btnStraVentHotkey.setHotkey(new Keys[]	{ Keys.LeftShift, Keys.S });
			m_btnCrossVentHotkey.setHotkey(new Keys[]	{ Keys.LeftShift, Keys.C });
			m_btnCornerVentHotkey.setHotkey(new Keys[]	{ Keys.LeftShift, Keys.R });
			m_btnEndVentHotkey.setHotkey(new Keys[]		{ Keys.LeftShift, Keys.E });

			foreach (Button t_button in m_ventButtons) {
				t_button.m_clickEvent += new Button.clickDelegate(guiButtonClick);
				t_button.setDownSound("button");
			}
			#endregion
			//-----------------------------------
			
			//-----------------------------------
			#region Hiding object buttons
			Vector2 t_hideMenu = new Vector2(m_btnDuckHideHotkey.getBox().X + 32, m_btnDuckHideHotkey.getBox().Y);
			m_hideButtons.AddLast(m_btnStandHideHotkey	= new Button("DevelopmentHotkeys//btn_standhide_hotkey",
				t_hideMenu + new Vector2(m_hideButtons.Count * 32, 0), "F", "VerdanaBold", Color.Black, t_btnTextOffset));

			m_btnStandHideHotkey.setHotkey(new Keys[] { Keys.F });

			foreach (Button t_button in m_hideButtons) {
				t_button.m_clickEvent += new Button.clickDelegate(guiButtonClick);
				t_button.setDownSound("button");
			}
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Guard object buttons
			Vector2 t_guardMenu = new Vector2(m_btnGuardHotkey.getBox().X + 32, m_btnGuardHotkey.getBox().Y);
			m_guardButtons.AddLast(m_btnDogHotkey			= new Button("DevelopmentHotkeys//btn_dog_hotkey",
				t_guardMenu + new Vector2(m_guardButtons.Count * 32, 0), "s+G", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2));
			m_guardButtons.AddLast(m_btnCameraHotkey		= new Button("DevelopmentHotkeys//btn_camera_hotkey",
				t_guardMenu + new Vector2(m_guardButtons.Count * 32, 0), "s+C", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2));

			m_btnDogHotkey.setHotkey(new Keys[]		{ Keys.LeftShift, Keys.G });
			m_btnCameraHotkey.setHotkey(new Keys[]	{ Keys.LeftShift, Keys.C });

			foreach (Button t_button in m_guardButtons) {
				t_button.m_clickEvent += new Button.clickDelegate(guiButtonClick);
				t_button.setDownSound("button");
			}
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Collectible object buttons
			Vector2 t_collMenu = new Vector2(m_btnCollHotkey.getBox().X + 32, m_btnCollHotkey.getBox().Y);
			m_collButtons.AddLast(m_btnHeartHotkey = new Button("DevelopmentHotkeys//btn_heart_hotkey", t_collMenu + new Vector2(m_collButtons.Count * 32, 0), "s+H", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2));

			m_btnHeartHotkey.setHotkey(new Keys[] { Keys.LeftShift, Keys.H });

			foreach (Button t_button in m_collButtons) {
				t_button.m_clickEvent += new Button.clickDelegate(guiButtonClick);
				t_button.setDownSound("button");
			}
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Layer buttons
			m_buttonList.AddLast(m_layerButtonList = GuiListFactory.createNumeratedList(5, "DevelopmentHotkeys//btn_layer_chooser"));
			GuiListFactory.setListPosition(m_layerButtonList, new Vector2(0, Game.getInstance().getResolution().Y - (m_layerButtonList.First().getBox().Height)));
			GuiListFactory.setButtonDistance(m_layerButtonList, new Vector2(73, 0));
			GuiListFactory.setTextOffset(m_layerButtonList, new Vector2(34, 8));

			int k = 1;
			foreach (Button t_button in m_layerButtonList)
			{
				t_button.setHotkey(new Keys[] { (Keys)Enum.Parse(typeof(Keys), "D" + k) });
				t_button.m_clickEvent += new Button.clickDelegate(setLayer);
			}
			#endregion
			//-----------------------------------

			setBuildingState(State.None);
			m_menuState = MenuState.Normal;

			base.load();
		}
		#endregion

		#region Update
		public override void update(GameTime a_gameTime)
		{
			updateCamera();
			updateKeyboard();
			updateMouse();
			updateGUI(a_gameTime);
		}
		#endregion

		#region Update Camera
		public void updateCamera()
		{
			if (Game.isKeyPressed(Keys.Right))
			{
				Game.getInstance().m_camera.move(new Vector2(15 / Game.getInstance().m_camera.getZoom(), 0));
			}

			if (Game.isKeyPressed(Keys.Left))
			{
				Game.getInstance().m_camera.move(new Vector2(-15 / Game.getInstance().m_camera.getZoom(), 0));
			}

			if (Game.isKeyPressed(Keys.Up))
			{
				Game.getInstance().m_camera.move(new Vector2(0, -15 / Game.getInstance().m_camera.getZoom()));
			}

			if (Game.isKeyPressed(Keys.Down))
			{
				Game.getInstance().m_camera.move(new Vector2(0, 15 / Game.getInstance().m_camera.getZoom()));
			}

			if (Game.m_currentMouse.ScrollWheelValue > Game.m_previousMouse.ScrollWheelValue)
			{
				Game.getInstance().m_camera.zoomIn(0.1f);
			}

			if (Game.m_currentMouse.ScrollWheelValue < Game.m_previousMouse.ScrollWheelValue)
			{
				Game.getInstance().m_camera.zoomOut(0.1f);
			}
		}
		#endregion

		#region Update GUI
		private void updateGUI(GameTime a_gameTime)
		{
			if (m_objectPreview != null) {
				m_objectPreview.getPosition().setLocalX(m_worldMouse.X - 36);
				m_objectPreview.getPosition().setLocalY(m_worldMouse.Y - 36);
			}

			m_statusBar.update(a_gameTime);
			if (m_selectedObject != null && m_selectedObject is Environment) {
				m_parallaxScrollTF.update(a_gameTime);
				m_parallaxLabel.update(a_gameTime);
			}

			foreach (Button t_button in m_assetButtonList) {
				t_button.update();
			}
			switch (m_menuState) {
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
				case MenuState.Collectible:
					foreach (Button t_button in m_collButtons) {
						t_button.update();
					}
					break;
				default:
					foreach (Button t_button in m_buildingButtons) {
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
				t_gui.update(a_gameTime);
			}
			m_layerTextField.update(a_gameTime);

			if (m_selectedObject != null) {
				m_selectedInfoV2 = getTileCoordinates(m_selectedObject.getPosition().getGlobalCartesian());
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
			switch (m_menuState) {
				case MenuState.Normal:
					if (a_button == m_btnSelectHotkey) {
						setBuildingState(State.None);
						return;
					}
					if (a_button == m_btnDeleteHotkey) {
						setBuildingState(State.Delete);
						return;
					}
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
					if (a_button == m_btnWallHotkey) {
						setBuildingState(State.Wall);
						return;
					}
					if (a_button == m_btnLightSwitchHotkey) {
						setBuildingState(State.LightSwitch);
						return;
					}
					if (a_button == m_btnWindowHotkey) {
						setBuildingState(State.Window);
						return;
					}
					if (a_button == m_btnForegroundHotkey) {
						setBuildingState(State.Foreground);
						return;
					}
					if (a_button == m_btnRopeHotkey) {
						setBuildingState(State.Rope);
						return;
					}
					if (a_button == m_btnSecDoorHotkey) {
						setBuildingState(State.SecDoor);
						return;
					}
					if (a_button == m_btnCornerHangHotkey) {
						setBuildingState(State.CornerHang);
						return;
					}
					if (a_button == m_btnCheckPointHotkey) {
						setBuildingState(State.Checkpoint);
						return;
					}
					if (a_button == m_btnPropHotkey) {
						setBuildingState(State.Prop);
						return;
					}
					if (a_button == m_btnCollHotkey) {
						m_menuState = MenuState.Collectible;
						setBuildingState(State.None);
						return;
					}
					if (a_button == m_btnVentHotkey) {
						m_menuState = MenuState.Ventilation;
						setBuildingState(State.None);
						return;
					}
					if (a_button == m_btnDuckHideHotkey) {
						m_menuState = MenuState.Hide;
						setBuildingState(State.None);
						return;
					}
					if (a_button == m_btnGuardHotkey) {
						m_menuState = MenuState.Guard;
						setBuildingState(State.None);
						return;
					}
					break;
				case MenuState.Hide:
					if (a_button == m_btnStandHideHotkey) {
						setBuildingState(State.StandHidingObject);
						return;
					}
					if (a_button == m_btnDuckHideHotkey) {
						setBuildingState(State.DuckHidingObject);
						return;
					}
					m_menuState = MenuState.Normal;
					guiButtonClick(a_button);
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
					m_menuState = MenuState.Normal;
					guiButtonClick(a_button);
					break;
				case MenuState.Ventilation:
					if (a_button == m_btnVentHotkey) {
						setBuildingState(State.Ventrance);					
						return;
					}
					if (a_button == m_btnCrossVentHotkey) {
						setBuildingState(State.CrossVent);
						return;
					}
					if (a_button == m_btnCornerVentHotkey) {
						setBuildingState(State.CornerVent);
						return;
					}
					if (a_button == m_btnTVentHotkey) {
						setBuildingState(State.TVent);
						return;
					}
					if (a_button == m_btnStraVentHotkey) {
						setBuildingState(State.StraVent);
						return;
					}
					if (a_button == m_btnEndVentHotkey) {
						setBuildingState(State.EndVent);
						return;
					}
					m_menuState = MenuState.Normal;
					guiButtonClick(a_button);
					break;
				case MenuState.Collectible:
					if (a_button == m_btnCollHotkey) {
						setBuildingState(State.Key);
						return;
					}
					if (a_button == m_btnHeartHotkey) {
						setBuildingState(State.Heart);
						return;
					}
					m_menuState = MenuState.Normal;
					guiButtonClick(a_button);
					break;
			}
		}

		private void createAssetList(string a_assetDirectory)
		{
			m_buttonList.Remove(m_assetButtonList);			
			if (a_assetDirectory == null)
			{
				m_assetButtonList.Clear();
				return;
			}

			string[] t_ext = { ".xnb" };
			m_assetButtonList = GuiListFactory.createListFromDirectory(a_assetDirectory, t_ext, "btn_asset_list");
			GuiListFactory.setListPosition(m_assetButtonList, new Vector2(Game.getInstance().getResolution().X - 160, 0));
			GuiListFactory.setButtonDistance(m_assetButtonList, new Vector2(0, 21));
			GuiListFactory.setTextOffset(m_assetButtonList, new Vector2(7, 1));
			foreach (Button t_button in m_assetButtonList)
			{
				t_button.m_clickEvent += new Button.clickDelegate(selectAsset);
			}
			m_buttonList.AddLast(m_assetButtonList);
		}

		private void selectAsset(Button a_button)
		{
			string t_newAsset = a_button.getText();
			foreach (Button t_button in m_assetButtonList)
			{
				t_button.setState(0);
			}
			a_button.setState(3);

			switch (m_itemToCreate) {
				case State.Platform:
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//Floor//" + t_newAsset, 0.000f);
					break;
				case State.Wall:
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//Wall//" + t_newAsset, 0.000f);
					break;
				case State.Delete:
					m_objectPreview = null;
					break;
				case State.Guard:
					m_objectPreview = new Platform(m_worldMouse, "Images//Sprite//Guard//" + t_newAsset, 0.000f);
					break;
				case State.Ladder:
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//Ladder//" + t_newAsset, 0.000f);
					break;
				case State.None:
					m_objectPreview = null;
					break;
				case State.Player:
					m_objectPreview = new Platform(m_worldMouse, "Images//Sprite//Hero//hero_stand", 0.000f);
					break;
				case State.SpotLight:
					m_objectPreview = new Platform(m_worldMouse, "Images//LightCone//" + t_newAsset, 0.000f);
					break;
				case State.Background:
					m_objectPreview = new Platform(m_worldMouse, "Images//Background//" + t_newAsset, 0.000f);
					break;
				case State.DuckHidingObject:
					m_objectPreview = new Platform(m_worldMouse, "Images//Prop//DuckHide//" + t_newAsset, 0.000f);
					break;
				case State.StandHidingObject:
					m_objectPreview = new Platform(m_worldMouse, "Images//Prop//StandHide//" + t_newAsset, 0.000f);
					break;
				case State.GuardDog:
					m_objectPreview = new Platform(m_worldMouse, "Images//Sprite//GuardDog//" + t_newAsset, 0.000f);
					break;
				case State.LightSwitch:
					m_objectPreview = new Platform(m_worldMouse, "Images//Prop//Button//" + t_newAsset, 0.000f);
					break;
				case State.CrossVent:
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//Ventilation//Cross//" + t_newAsset, 0.000f);
					break;
				case State.CornerVent:
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//Ventilation//Corner//" + t_newAsset, 0.000f);
					break;
				case State.StraVent:
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//Ventilation//Straight//" + t_newAsset, 0.000f);
					break;
				case State.TVent:
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//Ventilation//TVent//" + t_newAsset, 0.000f);
					break;
				case State.Ventrance:
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//Ventilation//Drum//" + t_newAsset, 0.000f);
					break;
				case State.Window:
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//Window//" + t_newAsset, 0.000f);
					break;
				case State.Foreground:
					m_objectPreview = new Platform(m_worldMouse, "Images//Foregrounds//" + t_newAsset, 0.000f);
					break;
				case State.CornerHang:
					m_objectPreview = new Platform(m_worldMouse, "Images//Foregrounds//" + t_newAsset, 0.000f);
					break;
				case State.Checkpoint:
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//1x1_tile_ph", 0.000f);
					break;
				case State.SecDoor:
					m_objectPreview = new Platform(m_worldMouse, "Images//Prop//SecurityDoor//" + t_newAsset, 0.000f);
					break;
				case State.Camera:
					m_objectPreview = new Platform(m_worldMouse, "Images//Sprite//Camera//camera", 0.000f);
					break;
				case State.Prop:
					m_objectPreview = new Platform(m_worldMouse, "Images//Prop//Clutter//" + t_newAsset, 0.000f);
					break;
			}
		}
		#endregion

		#region Update Keyboard
		private void updateKeyboard()
		{
			if (m_menuState != MenuState.Normal && Game.keyClicked(Keys.Escape)) {
				m_menuState = MenuState.Normal;
				guiButtonClick(m_btnSelectHotkey);
			}

			if (m_layerTextField.isWriting())
			{
				if (Game.keyClicked(Keys.Enter))
				{
					m_selectedObject.setLayer(float.Parse(m_layerTextField.getText()) / 1000);
					clearSelectedObject();
				}
				return;
			}

			if (m_parallaxScrollTF.isWriting())
			{
				if (Game.keyClicked(Keys.Enter) && m_selectedObject != null && m_selectedObject is Environment)
				{
					((Environment)m_selectedObject).setParrScroll(int.Parse(m_parallaxScrollTF.getText()));
					clearSelectedObject();
				}
				return;
			}

			if (Game.keyClicked(Keys.F5)) {
				m_currentLayer = 0;
				Game.getInstance().setState(new GameState(m_levelToLoad));
			}
			
			if (Game.keyClicked(Keys.F6))
			{
				m_currentLayer = 0;
				Game.getInstance().setState(new EventDevelopment(this, m_events));
			}
			
			if (Game.keyClicked(Keys.D1)) {
				setLayer(0);
			} else if (Game.keyClicked(Keys.D2)) {
				setLayer(1);
			} else if (Game.keyClicked(Keys.D3)) {
				setLayer(2);
			} else if (Game.keyClicked(Keys.D4)) {
				setLayer(3);
			} else if (Game.keyClicked(Keys.D5)) {
				setLayer(4);
			}
			/*
			-----------------------------------
			Keybindings for hotkeys
			-----------------------------------
			*/
			if (ctrlMod()) {
				if (Game.keyClicked(Keys.H)) {
					guiButtonClick(m_btnStandHideHotkey);
				}
				if (Game.keyClicked(Keys.S)) {
					if (m_selectedObject != null) {
						m_selectedObject.setColor(Color.White);
						m_selectedObject = null;
					}
					Level t_saveLevel = new Level();
					t_saveLevel.setLevelObjects(m_gameObjectList);
					t_saveLevel.setEvents(m_events);
					Serializer.getInstance().SaveLevel(Serializer.getInstance().getFileToStream(m_levelToLoad, true), t_saveLevel);

				}
				if (Game.keyClicked(Keys.O)) {
					Level t_newLevel = Serializer.getInstance().loadLevel(Serializer.getInstance().getFileToStream(m_levelToLoad, false));
					m_gameObjectList = t_newLevel.getGameObjects();
					foreach (LinkedList<GameObject> t_arr in m_gameObjectList) {
						foreach (GameObject f_gb in t_arr) {
							f_gb.loadContent();
						}
					}
				}
				if (Game.keyClicked(Keys.C)) {
					m_copyTarget = m_selectedObject;
				}
				if (Game.keyClicked(Keys.V)) {
					if (m_copyTarget != null) {
						AssetFactory.copyAsset(m_copyTarget);
					}
				}
				if (Game.keyClicked(Keys.N) && m_selectedObject != null) {
					if (m_selectedObject is Window) {
						((Window)m_selectedObject).toggleOpen();
					} else if (m_selectedObject is VentilationDrum) {
						((VentilationDrum)m_selectedObject).toggleLocked();
					}
					m_textObjectInfo.setText(m_selectedObject.ToString());
				}
				if (Game.keyClicked(Keys.F)) {
					if (m_selectedObject != null && m_selectedObject is Guard) {
						((Guard)m_selectedObject).toggleFlashLightAddicted();
						m_textObjectInfo.setText(m_selectedObject.ToString());
					}
				}
			}
			if (shiftMod()) {
				
			}
			if (altMod()) {
			}

			if (Game.keyClicked(Keys.R)) {
				if (m_selectedObject != null) {
					m_selectedObject.addRotation((float)(Math.PI) / 2.0f);
				}
			}
			if (Game.keyClicked(Keys.Y)) {
				if (m_selectedObject != null) {
					m_selectedObject.flip();
				}
			}
			if (Game.keyClicked(Keys.M)) {
				if (m_selectedObject != null) {
					if (m_selectedObject is LampSwitch) {
						((LampSwitch)m_selectedObject).toggleConnectToAll();
						showLightSwitchInfo((LampSwitch)m_selectedObject);
					}
				}
			}

			/*
			-----------------------------------
			Reset Camera to 0, 0 
			-----------------------------------
			*/
			if (Game.keyClicked(Keys.Space)) {
				if (m_gameObjectList != null) {
					Game.getInstance().m_camera.setPosition(Vector2.Zero);
				}
			}
		}
		#endregion

		#region Update Mouse
		private void updateMouse() {
			m_worldMouse = calculateWorldMouse();
			if (m_menuState == MenuState.Inactive) {
				return;
			}
			/*
			-----------------------------------
			Middle-mouse drag
			-----------------------------------
			*/
			if (Game.mmbPressed()) {
				Vector2 t_difference = Game.getInstance().m_camera.getPosition().getGlobalCartesian();
				t_difference.X = (Mouse.GetState().X - Game.getInstance().getResolution().X / 2) / 20 / Game.getInstance().m_camera.getZoom();
				t_difference.Y = (Mouse.GetState().Y - Game.getInstance().getResolution().Y / 2) / 20 / Game.getInstance().m_camera.getZoom();
				Game.getInstance().m_camera.getPosition().plusWith(t_difference);
			}

			/*
			-----------------------------------
			Left Mouse Button Click Down
			-----------------------------------
			*/
			if (Game.lmbDown()) {
				/*
				-----------------------------------
				Building
				-----------------------------------
				*/
				if (m_building && !collidedWithGui(Game.getMouseCoords()) && (!collidedWithObject(m_worldMouse) || m_itemToCreate == State.Background)) {
					if (m_itemToCreate != State.None && m_itemToCreate != State.Delete) {
						switch (m_itemToCreate) {
							case State.Player:
								AssetFactory.createPlayer(m_worldMouse);
								break;
							case State.Background:
								AssetFactory.createBackground(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.Ladder:
								AssetFactory.createLadder(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.Platform:
								AssetFactory.createPlatform(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.SpotLight:
								AssetFactory.createSpotLight(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.Guard:
								AssetFactory.createGuard(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.Wall:
								AssetFactory.createWall(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.DuckHidingObject:
								AssetFactory.createDuckHideObject(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.StandHidingObject:
								AssetFactory.createStandHideObject(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.GuardDog:
								AssetFactory.createGuardDog(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.LightSwitch:
								AssetFactory.createLightSwitch(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.CrossVent:
								AssetFactory.createCrossVent(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.CornerVent:
								AssetFactory.createCornerVent(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.StraVent:
								AssetFactory.createStraightVent(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.TVent:
								AssetFactory.createTVent(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.Ventrance:
								AssetFactory.createVentrance(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.Camera:
								AssetFactory.createCamera(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.Window:
								AssetFactory.createWindow(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.Foreground:
								AssetFactory.createForeground(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.SecDoor:
								AssetFactory.createSecDoor(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.CornerHang:
								AssetFactory.createCornerHang(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.Prop:
								AssetFactory.createProp(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.Rope:
								AssetFactory.createRope(m_worldMouse);
								break;
							case State.Checkpoint:
								AssetFactory.createCheckPoint(m_worldMouse);
								break;
							case State.Key:
								AssetFactory.createKey(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.Heart:
								AssetFactory.createHeart(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
							case State.EndVent:
								AssetFactory.createVentEnd(m_worldMouse, m_objectPreview.getImg().getImagePath());
								break;
						}
						return;
					}
				}
				/*
				-----------------------------------
				Selecting
				----------------------------------- 
				*/
				
				if (!m_building && !collidedWithGui(Game.getMouseCoords())) {
					if (m_selectedObject != null) {
						clearSelectedObject();
					}
					foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer]) {
						if (t_gameObject is LightCone || t_gameObject is FlashCone) {
							continue;
						} else if (t_gameObject is Environment) {
							if (t_gameObject.getBox().Contains((int)m_worldMouse.X, (int)m_worldMouse.Y)) {
								if (m_selectedObject == null || m_selectedObject.getLayer() > t_gameObject.getLayer()) {
									m_selectedObject = t_gameObject;
								}
							}
							continue;
						} else if (((Entity)t_gameObject).getImageBox().contains(m_worldMouse)) {
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
						} else if (m_selectedObject is Guard || m_selectedObject is GuardDog) {
							showGuardInfo((GuardEntity)m_selectedObject);
						} else if (m_selectedObject is LampSwitch) {
							showLightSwitchInfo((LampSwitch)m_selectedObject);
						}
						m_layerTextField.setText((m_selectedObject.getLayer() * 1000).ToString());
						m_textObjectInfo.setText(m_selectedObject.ToString());
						if (m_selectedObject is Environment)
						{
							m_parallaxScrollTF.setText(((Environment)m_selectedObject).getParrScroll().ToString());
							m_parallaxScrollTF.setVisible(true);
							m_parallaxLabel.setVisible(true);
						}
						m_selectedObject.setColor(Color.Yellow);
						m_dragFrom = m_selectedObject.getPosition().getGlobalCartesian();
					}
				}
			}

			/*
			-----------------------------------
			Left Mouse Button Release
			-----------------------------------
			*/
			if (Game.lmbUp()) {
				if (m_selectedObject != null) {
					if (m_selectedObject is Guard && m_selectedObject.getPosition().getGlobalCartesian() != m_dragFrom) {
						setGuardPoint((Guard)m_selectedObject, m_rightGuardPoint.getEndPoint().getGlobalCartesian(), true);
						setGuardPoint((Guard)m_selectedObject, m_leftGuardPoint.getEndPoint().getGlobalCartesian(), false);
						showGuardInfo((Guard)m_selectedObject);
					}
				}
				m_dragFrom = Vector2.Zero;
				m_firstDrag = true;
			}

			/*
			-----------------------------------
			Left Mouse Button Drag
			-----------------------------------
			*/
			if (Game.lmbPressed()) {
				if (m_selectedObject != null && m_menuState != MenuState.Inactive && !collidedWithGui(new Vector2(Game.m_currentMouse.X, Game.m_currentMouse.Y))) {
					if (m_firstDrag) {
						m_dragOffset = new Vector2(
							(float)Math.Floor((m_worldMouse.X - m_selectedObject.getPosition().getGlobalX()) / ((float)(Game.TILE_WIDTH))) * ((float)(Game.TILE_WIDTH)),
							(float)Math.Floor((m_worldMouse.Y - m_selectedObject.getPosition().getGlobalY()) / ((float)(Game.TILE_HEIGHT)) * ((float)(Game.TILE_HEIGHT)))
						);
						m_firstDrag = false;
					}

					Vector2 t_mousePosition;
					if (shiftMod()) {
						t_mousePosition = m_worldMouse - m_dragOffset;
					} else {
						t_mousePosition = getTileCoordinates(m_worldMouse - m_dragOffset);
					}

					if (m_selectedObject is SpotLight) {
						m_selectedObject.getPosition().setGlobalCartesian(new Vector2(t_mousePosition.X + m_selectedObject.getBox().Width,t_mousePosition.Y));
					} else if (m_selectedObject is Rope) {
						((Rope)m_selectedObject).moveRope(new Vector2(getTileCoordinates(m_worldMouse).X - 36, getTileCoordinates(m_worldMouse).Y));
					} else {
						m_selectedObject.getPosition().setGlobalCartesian(t_mousePosition);
					}
				}
			}
			
			/*
			-----------------------------------
			Right Mouse Button Down 
			-----------------------------------
			*/

			/*
			-----------------------------------
			Right Mouse Button Release
			-----------------------------------
			*/
			if (Game.rmbUp()) {
				if (m_selectedObject != null && m_selectedObject is Rope) {
					((Rope)m_selectedObject).setEndPoint(new Vector2(m_selectedObject.getPosition().getLocalX(), getTileCoordinates(m_worldMouse).Y + 72));
				}
				if (m_dragLine != null) {
					if (m_selectedObject is LampSwitch) {
						foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer]) {
							if (t_gameObject is SpotLight && ((Entity)t_gameObject).getHitBox().contains(m_worldMouse)) {
								connectSpotLight((SpotLight)t_gameObject, (LampSwitch)m_selectedObject);
								break;
							}
						}
						showLightSwitchInfo((LampSwitch)m_selectedObject);
					} else if (m_selectedObject is Guard || m_selectedObject is GuardDog) {
						setGuardPoint((GuardEntity)m_selectedObject, m_worldMouse, (m_worldMouse.X > m_selectedObject.getPosition().getGlobalX()));
						showGuardInfo((GuardEntity)m_selectedObject);
					} else if (m_selectedObject is GuardCamera) {
						setGuardCameraPoint((GuardCamera)m_selectedObject, m_worldMouse, m_worldMouse.X > m_selectedObject.getPosition().getGlobalX());
					}
					m_dragLine = null;
				} else {
					clearSelectedObject();
					setBuildingState(State.None);
					m_menuState = MenuState.Normal;
				}
				m_dragLine = null;
			}

			/*
			-----------------------------------
			Right Mouse Button Drag
			-----------------------------------
			*/
			if (Game.rmbPressed()) {
				if (m_selectedObject != null) {
					if (m_selectedObject is LampSwitch) {
						if (m_dragLine == null && ((Entity)m_selectedObject).getHitBox().contains(m_worldMouse)) {
							m_dragLine = new Line(m_selectedObject.getPosition(), new CartesianCoordinate(m_worldMouse), new Vector2(36, 36), Vector2.Zero, Color.Yellow, 5, true);
						} else if (m_dragLine != null) {
							m_dragLine.setEndPoint(m_worldMouse);
						}
					}
					if (m_selectedObject is Guard || m_selectedObject is GuardDog) {
						if (m_dragLine == null && ((Entity)m_selectedObject).getImageBox().contains(m_worldMouse)) {
							m_dragLine = new Line(m_selectedObject.getPosition(), new CartesianCoordinate(new Vector2(m_worldMouse.X, m_selectedObject.getPosition().getGlobalY() + 36)), new Vector2(36, 36), Vector2.Zero, Color.Green, 5, true);
						} else if (m_dragLine != null) {
							m_dragLine.setEndPoint(new Vector2(m_worldMouse.X, m_selectedObject.getPosition().getGlobalY() + 36));
						}
					}
					if (m_selectedObject is Rope) {
						((Rope)m_selectedObject).setEndPoint(new Vector2(m_selectedObject.getPosition().getLocalX(), m_worldMouse.Y));
					}
					if (m_selectedObject is GuardCamera) {
						if (m_dragLine == null && ((Entity)m_selectedObject).getHitBox().contains(m_worldMouse)) {
							m_dragLine = new Line(m_selectedObject.getPosition(), new CartesianCoordinate(m_worldMouse), Vector2.Zero, Vector2.Zero, Color.Red, 5, true);
						} else if (m_dragLine != null) {
							m_dragLine.setEndPoint(m_worldMouse);
						}
					}
				}
			}
		}
		#endregion

		#region Collision Check
		public override bool collidedWithObject(Vector2 a_coordinate)
		{
			foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer])
			{
				if (t_gameObject is Environment || t_gameObject is LightCone) {
					continue;
				}
				if (((Entity)t_gameObject).getImageBox().contains(a_coordinate)) {
					return true;
				}
			}
			return false;
		}
		#endregion
		
		#region Development Methods
		private void clearSelectedObject()
		{
			if (m_selectedObject != null)
			{
				m_selectedObject.setColor(Color.White);
				m_selectedObject = null;
				m_selectedInfoV2 = Vector2.Zero;
			}
			m_parallaxLabel.setVisible(false);
			m_parallaxScrollTF.setVisible(false);
			m_objectPreview = null;
			m_lineList.Clear();
		}

		private bool ctrlMod()
		{
			return Game.isKeyPressed(Keys.LeftControl) || Game.isKeyPressed(Keys.RightControl);
		}

		private bool shiftMod()
		{
			return Game.isKeyPressed(Keys.LeftShift) || Game.isKeyPressed(Keys.RightShift);
		}

		private bool altMod()
		{
			return Game.isKeyPressed(Keys.LeftAlt) || Game.isKeyPressed(Keys.RightAlt);
		}

		private void setBuildingState(State a_state) {
			m_building			= true;
			m_objectPreview		= null;
			
			if (a_state != m_itemToCreate)
			{
				clearSelectedObject();
				m_itemToCreate = a_state;
			}

			foreach (LinkedList<Button> t_buttonList in m_buttonList)
			{
				foreach (Button t_button in t_buttonList)
				{
					t_button.setState(0);
				}
			}

			switch (m_itemToCreate)
			{
				case State.Platform:
					createAssetList("Content//Images//Tile//Floor//");
					m_btnPlatformHotkey.setState(3);
					break;
				case State.Ladder:
					createAssetList("Content//Images//Tile//Ladder//");
					m_btnLadderHotkey.setState(3);
					break;
				case State.Background:
					createAssetList("Content//Images//Background//");
					m_btnBackgroundHotkey.setState(3);
					break;
				case State.Delete:
					createAssetList(null);
					m_btnDeleteHotkey.setState(3);
					m_building = false;
					break;
				case State.Player:
					createAssetList(null);
					m_btnHeroHotkey.setState(3);
					m_objectPreview = new Platform(m_worldMouse, "Images//Sprite//Hero//hero_stand", 0.000f);
					break;
				case State.None:
					createAssetList(null);
					m_btnSelectHotkey.setState(3);
					m_building = false;
					break;
				case State.SpotLight:
					createAssetList("Content//Images//LightCone//");
					m_btnSpotlightHotkey.setState(3);
					break;
				case State.Guard:
					createAssetList(null);
					m_btnGuardHotkey.setState(3);
					m_objectPreview = new Platform(m_worldMouse, "Images//Sprite//Guard//guard_idle", 0.000f);
					break;
				case State.Wall:
					createAssetList("Content//Images//Tile//Wall//");
					m_btnWallHotkey.setState(3);
					break;
				case State.DuckHidingObject:
					createAssetList("Content//Images//Prop//DuckHide//");
					m_btnDuckHideHotkey.setState(3);
					break;
				case State.StandHidingObject:
					createAssetList("Content//Images//Prop//StandHide//");
					m_btnStandHideHotkey.setState(3);
					break;
				case State.GuardDog:
					createAssetList("Content//Images//Sprite//GuardDog//");
					m_btnDogHotkey.setState(3);
					break;
				case State.LightSwitch:
					createAssetList("Content//Images//Prop//Button//");
					m_btnLightSwitchHotkey.setState(3);
					break;
				case State.CrossVent:
					createAssetList("Content//Images//Tile//Ventilation//Cross//");
					m_btnCrossVentHotkey.setState(3);
					break;
				case State.CornerVent:
					createAssetList("Content//Images//Tile//Ventilation//Corner//");
					m_btnCornerVentHotkey.setState(3);
					break;
				case State.TVent:
					createAssetList("Content//Images//Tile//Ventilation//TVent//");
					m_btnTVentHotkey.setState(3);
					break;
				case State.StraVent:
					createAssetList("Content//Images//Tile//Ventilation//Straight//");
					m_btnStraVentHotkey.setState(3);
					break;
				case State.Ventrance:
					createAssetList("Content//Images//Tile//Ventilation//Drum//");
					m_btnVentHotkey.setState(3);
					break;
				case State.Camera:
					createAssetList("Content//Images//Sprite//Camera//");
					m_btnCameraHotkey.setState(3);
					break;
				case State.Window:
					createAssetList("Content//Images//Tile//Window//");
					m_btnWindowHotkey.setState(3);
					break;
				case State.Foreground:
					createAssetList("Content//Images//Foregrounds//");
					m_btnForegroundHotkey.setState(3);
					break;
				case State.Rope:
					createAssetList(null);
					m_btnRopeHotkey.setState(3);
					break;
				case State.SecDoor:
					createAssetList("Content//Images//Prop//SecurityDoor//");
					m_btnSecDoorHotkey.setState(3);
					break;
				case State.CornerHang:
					createAssetList(null);
					m_btnCornerHangHotkey.setState(3);
					m_objectPreview = new Platform(m_worldMouse, "Images//Automagi//CornerThingy", 0.000f);
					break;
				case State.Checkpoint:
					createAssetList(null);
					m_btnCheckPointHotkey.setState(3);
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//1x1_tile_ph", 0.000f);
					break;
				case State.Prop:
					createAssetList("Content//Images//Prop//Clutter//");
					m_btnPropHotkey.setState(3);
					break;
				case State.Key:
					createAssetList(null);
					m_btnCollHotkey.setState(3);
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//1x1_tile_ph", 0.000f);
					break;
				case State.Heart:
					createAssetList(null);
					m_btnHeartHotkey.setState(3);
					m_objectPreview = new Platform(m_worldMouse, "Images//Sprite//Consumables//shinyheart", 0.000f);
					break;
				case State.EndVent:
					createAssetList(null);
					m_btnEndVentHotkey.setState(3);
					m_objectPreview = new Platform(m_worldMouse, "Images//Tile//Ventilation//EndVent//endventilation", 0.000f);
					break;
			}
			if (m_assetButtonList != null && m_assetButtonList.Count > 0)
			{
				selectAsset(m_assetButtonList.First());
			}
		}

		private void showGuardInfo(GuardEntity a_guard)
		{
			m_lineList.Clear();
			m_textGuardInfo.setText(" L: " + a_guard.getLeftPatrolPoint() + "R: " + a_guard.getRightPatrolPoint());
			m_lineList.AddLast(m_leftGuardPoint = a_guard.showLeftGuardPoint());
			m_lineList.AddLast(m_rightGuardPoint = a_guard.showRightGuardPoint());
		}

		private void showLightSwitchInfo(LampSwitch a_lightswitch)
		{
			m_lineList.Clear();
			foreach (SpotLight t_spotLight in a_lightswitch.getConnectedSpotLights())
			{
				m_lineList.AddLast(new Line(a_lightswitch.getPosition(), t_spotLight.getPosition(), new Vector2(36, 36), new Vector2(0, 0), Color.Yellow, 5, true));
			}
		}

		private void setGuardPoint(GuardEntity a_guard, Vector2 a_position, bool a_right)
		{
			if (a_right)
				a_guard.setRightGuardPoint(getTileCoordinates(a_position).X);
			else
				a_guard.setLeftGuardPoint(getTileCoordinates(a_position).X);
		}

		private void setGuardCameraPoint(GuardCamera a_guard, Vector2 a_position, bool a_right)
		{
			if (a_right)
				a_guard.setRightGuardPoint(a_position);
			else
				a_guard.setLeftGuardPoint(a_position);
		}

		private void connectSpotLight(SpotLight a_spotLight, LampSwitch a_lightSwitch)
		{
			a_lightSwitch.connectSpotLight(a_spotLight);
		}

		private void setLayer(Button a_button)
		{
			foreach (Button t_button in m_layerButtonList)
				t_button.setState(0);

			m_currentLayer = int.Parse(a_button.getText()) - 1;
			a_button.setState(3);
		}

		private void setLayer(int a_layer)
		{
			m_currentLayer = a_layer;
			foreach (Button t_button in m_layerButtonList)
				if (int.Parse(t_button.getText()) == m_currentLayer + 1)
					t_button.setState(3);
				else
					t_button.setState(0);
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
			for (int i = 0; i < m_gameObjectList.Length; i++) {			
				m_gameObjectList[i].Remove(a_object);
			}
		}

		public override void removeObject(GameObject a_object, int a_layer)
		{
			m_gameObjectList[a_layer].Remove(a_object);
		}

		public override void addGuiObject(GuiObject a_go)
		{
			m_guiList.AddLast(a_go);
		}

		public override LinkedList<GameObject>[] getObjectList()
		{
			return m_gameObjectList;
		}

		public override LinkedList<GameObject> getCurrentList()
		{
			return m_gameObjectList[m_currentLayer];
		}

		public void setEvents(LinkedList<Event> t_events)
		{
			if (t_events == null)
			{
				throw new ArgumentNullException();
			}
			m_events = t_events;
		}

		public void deleteObject(GameObject a_gameObject)
		{
			a_gameObject.kill();

			if (a_gameObject is SpotLight)
			{
				LightCone t_lightCone = ((SpotLight)a_gameObject).getLightCone();
				for (int i = 0; i < m_gameObjectList.Length; i++)
				{
					foreach (GameObject t_gameObject in m_gameObjectList[i])
					{
						if (t_gameObject is LampSwitch && ((LampSwitch)t_gameObject).isConnectedTo((SpotLight)a_gameObject))
						{
							((LampSwitch)t_gameObject).disconnectSpotLight((SpotLight)a_gameObject);
						}
					}
				}
				if (t_lightCone != null)
				{
					for (int i = 0; i < m_gameObjectList.Length; i++)
					{
						m_gameObjectList[i].Remove(t_lightCone);
					}
				}
			}
			m_gameObjectList[m_currentLayer].Remove(a_gameObject);

			if (a_gameObject is Player)
			{
				Game.getInstance().getState().setPlayer(null);
			}
		}

		public override void setPlayer(Player a_player)
		{
			m_player = a_player;
		}
		#endregion

		#region Draw
		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer])
			{
				t_gameObject.draw(a_gameTime);
			}

			if (Game.getInstance().getState() == this)
			{
				foreach (GuiObject t_guiObject in m_guiList) {
					t_guiObject.draw(a_gameTime);
				}
				foreach (Button t_button in m_staticButton) {
					t_button.draw(a_gameTime, a_spriteBatch);
				}
			
				m_statusBar.draw(a_gameTime);
				if (m_selectedObject != null) {
					m_layerTextField.draw(a_gameTime);
					if (m_selectedObject is Environment) {
						m_parallaxLabel.draw(a_gameTime);
						m_parallaxScrollTF.draw(a_gameTime);
					}
				}
				switch (m_menuState) {
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
					case MenuState.Collectible:
						foreach (Button t_button in m_collButtons) {
							t_button.draw(a_gameTime, a_spriteBatch);
						}
						break;
				}

				foreach (Button t_button in m_assetButtonList) {
					t_button.draw(a_gameTime, a_spriteBatch);
				}
				foreach (Button t_button in m_layerButtonList) {
					t_button.draw(a_gameTime, a_spriteBatch);
				}
			}
			foreach (Line t_line in m_lineList) {
				t_line.draw();
			}
			if (m_objectPreview != null) {
				m_objectPreview.draw(a_gameTime);
			}
			if (m_dragLine != null) {
				m_dragLine.draw();
			}
		}
		#endregion
	}
}