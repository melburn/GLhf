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

		private LinkedList<Button> m_assetButtonList;
		private LinkedList<Button> m_layerButtonList;
		private LinkedList<Line> m_lineList;

		private Dictionary<Button, State> m_buttonDict;

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

		private Button m_btnSelect;
		private Button m_btnVent;
		private Button m_btnDuckHide;
		private Button m_btnGuard;
		private Button m_btnConsKey;
		private Button m_btnBackground;

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
			Heart,			Key,		EndVent,		Objective,
			Shadow
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
					if (t_go is Player)
					{
						m_player = t_go;
						Game.getInstance().m_camera.setPosition(m_player.getPosition().getGlobalCartesian());
					}
				}
			}

			foreach (Event t_e in m_events)
			{
				t_e.loadContent();
			}

			m_buttonDict = new Dictionary<Button, State>();
			m_assetButtonList = new LinkedList<Button>();

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
			Button t_button;
			//-----------------------------------
			#region Buttons that are always shown
			m_buttonDict.Add(m_btnSelect = new Button("DevelopmentHotkeys//btn_select_hotkey", new Vector2(0, 32 * m_buttonDict.Count() + 25), "S", "VerdanaBold", Color.Black, t_btnTextOffset), State.None);
			m_btnSelect.setHotkey(new Keys[] { Keys.S }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_delete_hotkey", new Vector2(0, 32 * m_buttonDict.Count() + 25), "D", "VerdanaBold", Color.Black, t_btnTextOffset), State.Delete);
			t_button.setHotkey(new Keys[] { Keys.D }, guiButtonClick);
			#endregion
			//-----------------------------------
			
			//-----------------------------------
			#region Building mode buttons
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_ladder_hotkey",		new Vector2(0, 32 * m_buttonDict.Count() + 25), "L", "VerdanaBold", Color.Black, t_btnTextOffset), State.Ladder);
			t_button.setHotkey(new Keys[] { Keys.L }, guiButtonClick);
			m_buttonDict.Add(t_button	= new Button("DevelopmentHotkeys//btn_platform_hotkey",	new Vector2(0, 32 * m_buttonDict.Count() + 25), "P", "VerdanaBold", Color.Black, t_btnTextOffset), State.Platform);
			t_button.setHotkey(new Keys[] { Keys.P }, guiButtonClick);
			m_buttonDict.Add(m_btnBackground = new Button("DevelopmentHotkeys//btn_background_hotkey",	new Vector2(0, 32 * m_buttonDict.Count() + 25), "B", "VerdanaBold", Color.Black, t_btnTextOffset), State.Background);
			m_btnBackground.setHotkey(new Keys[] { Keys.B }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_hero_hotkey",		new Vector2(0, 32 * m_buttonDict.Count() + 25), "H", "VerdanaBold", Color.Black, t_btnTextOffset), State.Player);
			t_button.setHotkey(new Keys[] { Keys.H }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_spotlight_hotkey",	new Vector2(0, 32 * m_buttonDict.Count() + 25), "T", "VerdanaBold", Color.Black, t_btnTextOffset), State.SpotLight);
			t_button.setHotkey(new Keys[] { Keys.T }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_wall_hotkey",		new Vector2(0, 32 * m_buttonDict.Count() + 25), "W", "VerdanaBold", Color.Black, t_btnTextOffset), State.Wall);
			t_button.setHotkey(new Keys[] { Keys.W }, guiButtonClick);
			m_buttonDict.Add(t_button= new Button("DevelopmentHotkeys//btn_lightswitch_hotkey", new Vector2(0, 32 * m_buttonDict.Count() + 25), "s+T", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.LightSwitch);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.T }, guiButtonClick);
			m_buttonDict.Add(m_btnVent = new Button("DevelopmentHotkeys//btn_ventilation_hotkey", new Vector2(0, 32 * m_buttonDict.Count() + 25), "V", "VerdanaBold", Color.Black, t_btnTextOffset), State.Ventrance);
			m_btnVent.setHotkey(new Keys[] { Keys.V }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_window_hotkey",		new Vector2(0, 32 * m_buttonDict.Count() + 25), "N", "VerdanaBold", Color.Black, t_btnTextOffset), State.Window);
			t_button.setHotkey(new Keys[] { Keys.N }, guiButtonClick);
			m_buttonDict.Add(m_btnGuard = new Button("DevelopmentHotkeys//btn_guard_hotkey",	new Vector2(0, 32 * m_buttonDict.Count() + 25), "G", "VerdanaBold", Color.Black, t_btnTextOffset), State.Guard);
			m_btnGuard.setHotkey(new Keys[] { Keys.G }, guiButtonClick);
			m_buttonDict.Add(m_btnDuckHide = new Button("DevelopmentHotkeys//btn_duckhide_hotkey", new Vector2(0, 32 * m_buttonDict.Count() + 25), "A", "VerdanaBold", Color.Black, t_btnTextOffset), State.DuckHidingObject);
			m_btnDuckHide.setHotkey(new Keys[] { Keys.A }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_foreground_hotkey",	new Vector2(0, 32 * m_buttonDict.Count() + 25), "F", "VerdanaBold", Color.Black, t_btnTextOffset), State.Foreground);
			t_button.setHotkey(new Keys[] { Keys.F }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_rope_hotkey",		new Vector2(0, 32 * m_buttonDict.Count() + 25), "O", "VerdanaBold", Color.Black, t_btnTextOffset), State.Rope);
			t_button.setHotkey(new Keys[] { Keys.O }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_secdoor_hotkey",	new Vector2(0, 32 * m_buttonDict.Count() + 25), "E", "VerdanaBold", Color.Black, t_btnTextOffset), State.SecDoor);
			t_button.setHotkey(new Keys[] { Keys.E }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_doorhang_hotkey",	new Vector2(0, 32 * m_buttonDict.Count() + 25), "s+W", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.CornerHang);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.W }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_checkpoint_hotkey",	new Vector2(0, 32 * m_buttonDict.Count() + 25), "K", "VerdanaBold", Color.Black, t_btnTextOffset), State.Checkpoint);
			t_button.setHotkey(new Keys[] { Keys.K }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_clutter_hotkey",	new Vector2(0, 32 * m_buttonDict.Count() + 25), "C", "VerdanaBold", Color.Black, t_btnTextOffset), State.Prop);
			t_button.setHotkey(new Keys[] { Keys.C }, guiButtonClick);
			m_buttonDict.Add(m_btnConsKey = new Button("DevelopmentHotkeys//btn_key_hotkey", new Vector2(0, 32 * m_buttonDict.Count() + 25), "Z", "VerdanaBold", Color.Black, t_btnTextOffset), State.Key);
			m_btnConsKey.setHotkey(new Keys[] { Keys.Z }, guiButtonClick);
			
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Ventilation buttons
			Vector2 t_ventMenu = new Vector2(m_btnVent.getBox().X + 32, m_btnVent.getBox().Y);
			int t_buttonNumber = 0;

			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_tvent_hotkey", t_ventMenu + new Vector2(t_buttonNumber++ * 32, 0), "s+V", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.TVent);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.V }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_svent_hotkey", t_ventMenu + new Vector2(t_buttonNumber++ * 32, 0), "s+A", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.StraVent);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.A }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_cvent_hotkey", t_ventMenu + new Vector2(t_buttonNumber++ * 32, 0), "s+C", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.CrossVent);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.C }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_ovent_hotkey", t_ventMenu + new Vector2(t_buttonNumber++ * 32, 0), "s+R", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.CornerVent);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.R }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button(null, t_ventMenu + new Vector2(t_buttonNumber++ * 32, 0), "s+E", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.EndVent);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.E }, guiButtonClick);
			#endregion
			//-----------------------------------
			
			//-----------------------------------
			#region Hiding object buttons
			t_buttonNumber = 0;
			Vector2 t_hideMenu = new Vector2(m_btnDuckHide.getBox().X + 32, m_btnDuckHide.getBox().Y);

			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_standhide_hotkey", t_hideMenu + new Vector2(t_buttonNumber++ * 32, 0), "s+F", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.StandHidingObject);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.F }, guiButtonClick);
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Guard object buttons
			t_buttonNumber = 0;
			Vector2 t_guardMenu = new Vector2(m_btnGuard.getBox().X + 32, m_btnGuard.getBox().Y);

			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_dog_hotkey", t_guardMenu + new Vector2(t_buttonNumber++ * 32, 0), "s+G", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.GuardDog);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.G }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_camera_hotkey", t_guardMenu + new Vector2(t_buttonNumber++ * 32, 0), "s+Q", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.Camera);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.Q }, guiButtonClick);
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Consumable Buttons
			t_buttonNumber = 0;
			Vector2 t_consMenu = new Vector2(m_btnConsKey.getBox().X + 32, m_btnConsKey.getBox().Y);

			m_buttonDict.Add(t_button = new Button("DevelopmentHotkeys//btn_heart_hotkey", t_consMenu + new Vector2(t_buttonNumber++ * 32, 0), "s+H", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.Heart);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.H }, guiButtonClick);
			m_buttonDict.Add(t_button = new Button(null, t_consMenu + new Vector2(t_buttonNumber++ * 32, 0), "s+B", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.Objective);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.B }, guiButtonClick);
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Background Buttons
			t_buttonNumber = 0;
			Vector2 t_bgMenu = new Vector2(m_btnBackground.getBox().X + 32, m_btnBackground.getBox().Y);

			m_buttonDict.Add(t_button = new Button(null, t_bgMenu + new Vector2(t_buttonNumber++ * 32, 0), "s+X", "VerdanaBold", Color.Black, t_btnTextOffset - t_modV2), State.Shadow);
			t_button.setHotkey(new Keys[] { Keys.LeftShift, Keys.X }, guiButtonClick);
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Layer buttons
			m_buttonList.AddLast(m_layerButtonList = GuiListFactory.createNumeratedList(5, "DevelopmentHotkeys//btn_layer_chooser"));
			GuiListFactory.setListPosition(m_layerButtonList, new Vector2(0, Game.getInstance().getResolution().Y - (m_layerButtonList.First().getBox().Height)));
			GuiListFactory.setButtonDistance(m_layerButtonList, new Vector2(73, 0));
			GuiListFactory.setTextOffset(m_layerButtonList, new Vector2(34, 8));

			int k = 1;
			foreach (Button t_feButton in m_layerButtonList)
			{
				t_feButton.setHotkey(new Keys[] { (Keys)Enum.Parse(typeof(Keys), "D" + k++) }, setLayer);
			}
			#endregion
			//-----------------------------------

			setBuildingState(m_btnSelect, State.None);

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
			if (KeyboardHandler.isKeyPressed(Keys.Right))
			{
				Game.getInstance().m_camera.move(new Vector2(15 / Game.getInstance().m_camera.getZoom(), 0));
			}

			if (KeyboardHandler.isKeyPressed(Keys.Left))
			{
				Game.getInstance().m_camera.move(new Vector2(-15 / Game.getInstance().m_camera.getZoom(), 0));
			}

			if (KeyboardHandler.isKeyPressed(Keys.Up))
			{
				Game.getInstance().m_camera.move(new Vector2(0, -15 / Game.getInstance().m_camera.getZoom()));
			}

			if (KeyboardHandler.isKeyPressed(Keys.Down))
			{
				Game.getInstance().m_camera.move(new Vector2(0, 15 / Game.getInstance().m_camera.getZoom()));
			}

			if (MouseHandler.scrollUp())
			{
				Game.getInstance().m_camera.zoomIn(0.1f);
			}

			if (MouseHandler.scrollDown())
			{
				Game.getInstance().m_camera.zoomOut(0.1f);
			}
		}
		#endregion

		#region Update GUI
		private void updateGUI(GameTime a_gameTime)
		{
			if (m_objectPreview != null)
			{
				m_objectPreview.getPosition().setLocalX(m_worldMouse.X - 36);
				m_objectPreview.getPosition().setLocalY(m_worldMouse.Y - 36);
			}

			if (m_selectedObject != null && m_selectedObject is Environment)
			{
				m_parallaxScrollTF.update(a_gameTime);
				m_parallaxLabel.update(a_gameTime);
			}

			if (!m_layerTextField.isWriting() && !m_parallaxScrollTF.isWriting())
			{
				foreach (LinkedList<Button> t_buttonList in m_buttonList)
				{
					foreach (Button t_button in t_buttonList)
					{
						t_button.update();
					}
				}
				foreach (Button t_button in m_buttonDict.Keys)
				{
					t_button.update();
				}
			}

			m_layerTextField.update(a_gameTime);
			m_statusBar.update(a_gameTime);

			if (m_selectedObject != null)
			{
				m_selectedInfoV2 = getTileCoordinates(m_selectedObject.getPosition().getGlobalCartesian());
				if (m_selectedObject is Guard)
				{
					Guard t_guard = (Guard)m_selectedObject;
					m_textGuardInfo.setText("R: " + t_guard.getRightPatrolPoint() / 72 + " L: " + t_guard.getLeftPatrolPoint() / 72);
				}
				else
				{
					m_textGuardInfo.setText("");
				}
			}
		}

		public void guiButtonClick(Button a_button)
		{
			if (!a_button.isButtonPressed())
			{
				a_button.playDownSound();
			}
			setBuildingState(a_button, m_buttonDict[a_button]);
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
			string t_newAsset = "";
			Vector2 t_assetPosition = m_worldMouse + new Vector2(-36, -36);

			if (a_button != null)
			{
				t_newAsset = a_button.getText();
				foreach (Button t_button in m_assetButtonList)
				{
					t_button.setState(0);
				}
				a_button.setState(3);
			}

			switch (m_itemToCreate)
			{
				case State.Platform:
					m_objectPreview = new Platform(t_assetPosition, "Images//Tile//Floor//" + t_newAsset, 0.000f);
					break;
				case State.Wall:
					m_objectPreview = new Wall(t_assetPosition, "Images//Tile//Wall//" + t_newAsset, 0.000f);
					break;
				case State.Delete:
					m_objectPreview = null;
					break;
				case State.Guard:
					m_objectPreview = new Guard(t_assetPosition, "Images//Sprite//Guard//guard_idle", m_worldMouse.X, true, 0.000f);
					break;
				case State.Ladder:
					m_objectPreview = new Ladder(t_assetPosition, "Images//Tile//Ladder//" + t_newAsset, 0.000f);
					break;
				case State.None:
					m_objectPreview = null;
					break;
				case State.Player:
					m_objectPreview = new Player(t_assetPosition, "Images//Sprite//Hero//hero_stand", 0.000f);
					break;
				case State.SpotLight:
					m_objectPreview = new SpotLight(t_assetPosition, "Images//LightCone//" + t_newAsset, 0.000f, 0.0f, false);
					break;
				case State.Background:
					m_objectPreview = new Environment(t_assetPosition, "Images//Background//" + t_newAsset, 0.000f);
					break;
				case State.DuckHidingObject:
					m_objectPreview = new DuckHideObject(t_assetPosition, "Images//Prop//DuckHide//" + t_newAsset, 0.000f);
					break;
				case State.StandHidingObject:
					m_objectPreview = new StandHideObject(t_assetPosition, "Images//Prop//StandHide//" + t_newAsset, 0.000f);
					break;
				case State.GuardDog:
					m_objectPreview = new GuardDog(t_assetPosition, "Images//Sprite//GuardDog//" + t_newAsset, m_worldMouse.X, m_worldMouse.X, 0.000f);
					break;
				case State.LightSwitch:
					m_objectPreview = new LampSwitch(t_assetPosition, "Images//Prop//Button//" + t_newAsset, 0.000f);
					break;
				case State.CrossVent:
					m_objectPreview = new CrossVentilation(t_assetPosition, "Images//Tile//Ventilation//Cross//" + t_newAsset, 0.000f);
					break;
				case State.CornerVent:
					m_objectPreview = new CornerVentilation(t_assetPosition, "Images//Tile//Ventilation//Corner//" + t_newAsset, 0.000f);
					break;
				case State.StraVent:
					m_objectPreview = new StraightVentilation(t_assetPosition, "Images//Tile//Ventilation//Straight//" + t_newAsset, 0.000f);
					break;
				case State.TVent:
					m_objectPreview = new TVentilation(t_assetPosition, "Images//Tile//Ventilation//TVent//" + t_newAsset, 0.000f);
					break;
				case State.Ventrance:
					m_objectPreview = new VentilationDrum(t_assetPosition, "Images//Tile//Ventilation//Drum//" + t_newAsset, 0.000f);
					break;
				case State.Window:
					m_objectPreview = new Window(t_assetPosition, "Images//Tile//Window//" + t_newAsset, 0.000f);
					break;
				case State.Foreground:
					m_objectPreview = new Foreground(t_assetPosition, "Images//Foregrounds//" + t_newAsset, 0.000f);
					break;
				case State.CornerHang:
					m_objectPreview = new CornerHang(t_assetPosition, "Images//Automagi//cornerthingy", 0.000f);
					break;
				case State.Checkpoint:
					m_objectPreview = new CheckPoint(t_assetPosition, "Images//Tile//1x1_tile_ph", 0.000f);
					break;
				case State.SecDoor:
					m_objectPreview = new LockedDoor(t_assetPosition, "Images//Prop//SecurityDoor//" + t_newAsset, 0.000f);
					break;
				case State.Camera:
					m_objectPreview = new GuardCamera(t_assetPosition, "Images//Sprite//Camera//camera", 0.200f, (float)(Math.PI * 0.5), (float)(Math.PI * 0.75), (float)(Math.PI * 0.25), false);
					break;
				case State.Prop:
					m_objectPreview = new Environment(t_assetPosition, "Images//Prop//Clutter//" + t_newAsset, 0.000f);
					break;
				case State.Rope:
					m_objectPreview = new Rope(t_assetPosition, "", 0.000f);
					break;
				case State.Heart:
					m_objectPreview = new ConsumableHeart(t_assetPosition, "Images//Prop//Consumables//shinyheart", 0.000f);
					break;
				case State.Key:
					m_objectPreview = new ConsumableKey(t_assetPosition, "Images//Tile//1x1_tile_ph", 0.000f);
					break;
				case State.Objective:
					m_objectPreview = new ConsumableGoal(t_assetPosition, "Images//Prop//Consumables//Objective//" + t_newAsset, 0.000f);
					break;
				case State.Shadow:
					m_objectPreview = new CoveringShadow(t_assetPosition, "Images//Foregrounds//Shadow//" + t_newAsset, 0.0f);
					break;
				case State.EndVent:
					m_objectPreview = new VentilationEnd(t_assetPosition, "Images//Tile//Ventilation//EndVent//" + t_newAsset, 0.000f);
					break;
			}
		}
		#endregion

		#region Update Keyboard
		private void updateKeyboard()
		{
			if (KeyboardHandler.keyClicked(Keys.Escape))
			{
				guiButtonClick(m_btnSelect);
			}

			if (m_layerTextField.isWriting())
			{
				if (KeyboardHandler.keyClicked(Keys.Enter))
				{
					m_selectedObject.setLayer(float.Parse(m_layerTextField.getText()) / 1000);
					clearSelectedObject();
					m_layerTextField.setWrite(false);
				}
				return;
			}

			if (m_parallaxScrollTF.isWriting())
			{
				if (KeyboardHandler.keyClicked(Keys.Enter) && m_selectedObject != null && m_selectedObject is Environment)
				{
					((Environment)m_selectedObject).setParrScroll(Math.Max(- int.Parse(m_parallaxScrollTF.getText()), -100000));
					clearSelectedObject();
					m_parallaxScrollTF.setWrite(false);
				}
				return;
			}

			if (KeyboardHandler.keyClicked(Keys.F5))
			{
				m_currentLayer = 0;
				Game.getInstance().setState(new GameState(m_levelToLoad));
			}
			
			if (KeyboardHandler.keyClicked(Keys.F6))
			{
				m_currentLayer = 0;
				Game.getInstance().setState(new EventDevelopment(this, m_events));
			}
			
			if (KeyboardHandler.ctrlMod())
			{
				if (KeyboardHandler.keyClicked(Keys.S))
				{
					if (m_selectedObject != null)
					{
						m_selectedObject.setColor(Color.White);
						m_selectedObject = null;
					}
					Level t_saveLevel = new Level();
					t_saveLevel.setLevelObjects(m_gameObjectList);
					t_saveLevel.setEvents(m_events);
					Serializer.getInstance().SaveLevel(Serializer.getInstance().getFileToStream(m_levelToLoad, true), t_saveLevel);

				}

				if (KeyboardHandler.keyClicked(Keys.O))
				{
					Level t_newLevel = Serializer.getInstance().loadLevel(Serializer.getInstance().getFileToStream(m_levelToLoad, false));
					m_gameObjectList = t_newLevel.getGameObjects();
					foreach (LinkedList<GameObject> t_arr in m_gameObjectList)
					{
						foreach (GameObject f_gb in t_arr)
						{
							f_gb.loadContent();
						}
					}
				}

				if (KeyboardHandler.keyClicked(Keys.C))
				{
					m_copyTarget = m_selectedObject;
				}

				if (KeyboardHandler.keyClicked(Keys.V))
				{
					if (m_copyTarget != null)
					{
						AssetFactory.copyAsset(m_selectedObject.getPosition().getGlobalCartesian(), m_copyTarget);
					}
				}

				if (KeyboardHandler.keyClicked(Keys.N) && m_selectedObject != null)
				{
					if (m_selectedObject is Window)
					{
						((Window)m_selectedObject).toggleOpen();
					}
					else if (m_selectedObject is VentilationDrum)
					{
						((VentilationDrum)m_selectedObject).toggleLocked();
					}
					else if (m_selectedObject is Guard)
					{
						((Guard)m_selectedObject).toggleFlashLightAddicted();
					}
					else if (m_selectedObject is Consumable)
					{
						((Consumable)m_selectedObject).toggleBling();
					}
					m_textObjectInfo.setText(m_selectedObject.ToString());
				}

				if (KeyboardHandler.keyClicked(Keys.F))
				{
					if (m_selectedObject != null && m_selectedObject is Guard)
					{
						((Guard)m_selectedObject).toggleFlashLightAddicted();
						m_textObjectInfo.setText(m_selectedObject.ToString());
					}
				}
			}

			if (KeyboardHandler.keyClicked(Keys.R))
			{
				if (m_selectedObject != null)
				{
					m_selectedObject.addRotation((float)(Math.PI) / 2.0f);
				}
			}

			if (KeyboardHandler.keyClicked(Keys.Y))
			{
				if (m_selectedObject != null)
				{
					m_selectedObject.flip();
				}
			}

			if (KeyboardHandler.keyClicked(Keys.M))
			{
				if (m_selectedObject != null)
				{
					if (m_selectedObject is LampSwitch)
					{
						((LampSwitch)m_selectedObject).toggleConnectToAll();
						showLightSwitchInfo((LampSwitch)m_selectedObject);
					}
				}
			}

			if (KeyboardHandler.keyClicked(Keys.Space))
			{
				if (m_gameObjectList != null)
				{
					Game.getInstance().m_camera.setPosition(Vector2.Zero);
				}
			}
		}
		#endregion

		#region Update Mouse
		private void updateMouse()
		{
			m_worldMouse = MouseHandler.worldMouse();

			//-----------------------------------
			#region Middle-mouse drag
			if (MouseHandler.mmbPressed())
			{
				Vector2 t_difference = Game.getInstance().m_camera.getPosition().getGlobalCartesian();
				t_difference.X = (Mouse.GetState().X - Game.getInstance().getResolution().X / 2) / 20 / Game.getInstance().m_camera.getZoom();
				t_difference.Y = (Mouse.GetState().Y - Game.getInstance().getResolution().Y / 2) / 20 / Game.getInstance().m_camera.getZoom();
				Game.getInstance().m_camera.getPosition().plusWith(t_difference);
			}
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Left Mouse Button Down
			if (MouseHandler.lmbDown())
			{
				//-----------------------------------
				#region Building
				if (m_building && !collidedWithGui(MouseHandler.getMouseCoords()) && (!collidedWithObject(m_worldMouse) || m_itemToCreate == State.Background))
				{
					if (m_itemToCreate != State.None && m_itemToCreate != State.Delete)
					{
						AssetFactory.copyAsset(m_worldMouse, m_objectPreview);
					}
				}
				#endregion
				//-----------------------------------
				
				//-----------------------------------
				#region Selecting
				if (!m_building && !collidedWithGui(MouseHandler.getMouseCoords()))
				{
					clearSelectedObject();

					if ((m_selectedObject = selectObject(m_worldMouse)) != null)
					{
						if (m_itemToCreate == State.Delete)
						{
							deleteObject(m_selectedObject);
							return;
						}
						else if (m_selectedObject is Guard || m_selectedObject is GuardDog)
						{
							showGuardInfo((GuardEntity)m_selectedObject);
						}
						else if (m_selectedObject is LampSwitch)
						{
							showLightSwitchInfo((LampSwitch)m_selectedObject);
						}
						else if (m_selectedObject is Environment)
						{
							m_parallaxScrollTF.setText((-((Environment)m_selectedObject).getParrScroll()).ToString());
							m_parallaxScrollTF.setVisible(true);
							m_parallaxLabel.setVisible(true);
						}

						m_layerTextField.setText((m_selectedObject.getLayer() * 1000).ToString());
						m_textObjectInfo.setText(m_selectedObject.ToString());
						
						m_selectedObject.setColor(Color.Yellow);
						m_dragFrom = m_selectedObject.getPosition().getGlobalCartesian();
					}
				}
				#endregion
				//----------------------------------- 
			}
			#endregion
			//-----------------------------------
			
			//-----------------------------------
			#region Left Mouse Button Release
			if (MouseHandler.lmbUp())
			{
				if (m_selectedObject != null)
				{
					if (m_selectedObject is Guard && m_selectedObject.getPosition().getGlobalCartesian() != m_dragFrom)
					{
						setGuardPoint((Guard)m_selectedObject, m_rightGuardPoint.getEndPoint().getGlobalCartesian(), true);
						setGuardPoint((Guard)m_selectedObject, m_leftGuardPoint.getEndPoint().getGlobalCartesian(), false);
						showGuardInfo((Guard)m_selectedObject);
					}
				}
				m_dragFrom = Vector2.Zero;
				m_firstDrag = true;
			}
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Left Mouse Button Drag
			if (MouseHandler.lmbPressed())
			{
				if (m_selectedObject != null && !collidedWithGui(MouseHandler.getCurPos()))
				{
					if (m_firstDrag)
					{
						m_dragOffset = new Vector2(
							(float)Math.Floor((m_worldMouse.X - m_selectedObject.getPosition().getGlobalX()) / ((float)(Game.TILE_WIDTH))) * ((float)(Game.TILE_WIDTH)),
							(float)Math.Floor((m_worldMouse.Y - m_selectedObject.getPosition().getGlobalY()) / ((float)(Game.TILE_HEIGHT)) * ((float)(Game.TILE_HEIGHT)))
						);
						m_firstDrag = false;
					}

					Vector2 t_mousePosition;
					if (KeyboardHandler.shiftMod())
					{
						t_mousePosition = m_worldMouse - m_dragOffset;
					}
					else
					{
						t_mousePosition = getTileCoordinates(m_worldMouse - m_dragOffset);
					}

					if (m_selectedObject is SpotLight)
					{
						m_selectedObject.getPosition().setGlobalCartesian(new Vector2(t_mousePosition.X + m_selectedObject.getBox().Width,t_mousePosition.Y));
					}
					else if (m_selectedObject is Rope)
					{
						((Rope)m_selectedObject).moveRope(new Vector2(getTileCoordinates(m_worldMouse).X - 36, getTileCoordinates(m_worldMouse).Y));
					}
					else if (m_selectedObject is VentilationDrum)
					{
						m_selectedObject.getPosition().setGlobalCartesian(t_mousePosition);
						((VentilationDrum)m_selectedObject).getPairedVentilation().getPosition().setGlobalCartesian(t_mousePosition);
					}
					else
					{
						m_selectedObject.getPosition().setGlobalCartesian(t_mousePosition);
					}
				}
			}
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Right Mouse Button Release
			if (MouseHandler.rmbUp())
			{
				if (m_selectedObject != null && m_selectedObject is Rope)
				{
					((Rope)m_selectedObject).setEndPoint(new Vector2(m_selectedObject.getPosition().getLocalX(), getTileCoordinates(m_worldMouse).Y + 72));
				}

				if (m_dragLine != null)
				{
					if (m_selectedObject is LampSwitch)
					{
						foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer])
						{
							if (t_gameObject is SpotLight && ((Entity)t_gameObject).getHitBox().contains(m_worldMouse))
							{
								connectSpotLight((SpotLight)t_gameObject, (LampSwitch)m_selectedObject);
								break;
							}
						}
						showLightSwitchInfo((LampSwitch)m_selectedObject);
					}
					else if (m_selectedObject is Guard || m_selectedObject is GuardDog)
					{
						setGuardPoint((GuardEntity)m_selectedObject, m_worldMouse, (m_worldMouse.X > m_selectedObject.getPosition().getGlobalX()));
						showGuardInfo((GuardEntity)m_selectedObject);
					}
					else if (m_selectedObject is GuardCamera)
					{
						setGuardCameraPoint((GuardCamera)m_selectedObject, m_worldMouse, m_worldMouse.X > m_selectedObject.getPosition().getGlobalX());
					}
					m_dragLine = null;
				}
				else
				{
					clearSelectedObject();
					foreach (Button t_button in m_buttonDict.Keys)
					{
						t_button.setState(0);
					}
					setBuildingState(m_btnSelect, State.None);
				}
				m_dragLine = null;
			}
			#endregion
			//-----------------------------------

			//-----------------------------------
			#region Right Mouse Button Drag
			if (MouseHandler.rmbPressed())
			{
				if (m_selectedObject != null)
				{
					if (m_selectedObject is LampSwitch)
					{
						if (m_dragLine == null && ((Entity)m_selectedObject).getHitBox().contains(m_worldMouse))
						{
							m_dragLine = new Line(m_selectedObject.getPosition(), new CartesianCoordinate(m_worldMouse), new Vector2(36, 36), Vector2.Zero, Color.Yellow, 5, true);
						}
						else if (m_dragLine != null)
						{
							m_dragLine.setEndPoint(m_worldMouse);
						}
					}

					if (m_selectedObject is Guard || m_selectedObject is GuardDog)
					{
						if (m_dragLine == null && ((Entity)m_selectedObject).getImageBox().contains(m_worldMouse))
						{
							m_dragLine = new Line(m_selectedObject.getPosition(), new CartesianCoordinate(new Vector2(m_worldMouse.X, m_selectedObject.getPosition().getGlobalY() + 36)), new Vector2(36, 36), Vector2.Zero, Color.Green, 5, true);
						}
						else if (m_dragLine != null)
						{
							m_dragLine.setEndPoint(new Vector2(m_worldMouse.X, m_selectedObject.getPosition().getGlobalY() + 36));
						}
					}

					if (m_selectedObject is Rope)
					{
						((Rope)m_selectedObject).setEndPoint(new Vector2(m_selectedObject.getPosition().getLocalX(), m_worldMouse.Y));
					}

					if (m_selectedObject is GuardCamera)
					{
						if (m_dragLine == null && ((Entity)m_selectedObject).getHitBox().contains(m_worldMouse))
						{
							m_dragLine = new Line(m_selectedObject.getPosition(), new CartesianCoordinate(m_worldMouse), Vector2.Zero, Vector2.Zero, Color.Red, 5, true);
						}
						else if (m_dragLine != null)
						{
							m_dragLine.setEndPoint(m_worldMouse);
						}
					}
				}
			}
			#endregion
			//-----------------------------------
		}
		#endregion

		#region Collision Check
		public override bool collidedWithObject(Vector2 a_coordinate)
		{
			foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer])
			{
				if (t_gameObject is Environment || t_gameObject is LightCone)
				{
					continue;
				}
				if (((Entity)t_gameObject).getImageBox().contains(a_coordinate))
				{
					return true;
				}
			}
			return false;
		}

		public override bool collidedWithGui(Vector2 a_coordinate)
		{
			bool t_baseCollide;
			if (t_baseCollide = base.collidedWithGui(a_coordinate))
			{
				return t_baseCollide;
			}
			foreach (Button t_button in m_buttonDict.Keys)
			{
				if (t_button.getBox().Contains((int)a_coordinate.X, (int)a_coordinate.Y))
				{
					t_baseCollide = true;
					break;
				}
			}

			if (m_statusBar.getBox().Contains(new Point((int)a_coordinate.X, (int)a_coordinate.Y)))
			{
				t_baseCollide = true;
			}
			return t_baseCollide;
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

		private GameObject selectObject(Vector2 a_point)
		{
			GameObject t_return = null;

			foreach (GameObject t_gameObject in m_gameObjectList[m_currentLayer])
			{
				if (t_gameObject is LightCone || t_gameObject is FlashCone)
				{
					continue;
				}
				else if (t_gameObject is Environment)
				{
					if (((Environment)t_gameObject).getImageBox().contains(a_point))
					{
						if (t_return == null || t_return.getLayer() > t_gameObject.getLayer())
						{
							t_return = t_gameObject;
						}
					}
				}
				else if (((Entity)t_gameObject).getImageBox().contains(a_point))
				{
					if (t_return == null || t_return.getLayer() > t_gameObject.getLayer())
					{
						t_return = t_gameObject;
					}
				}
			}

			return t_return;
		}

		private void setBuildingState(Button a_button, State a_state)
		{
			m_building			= true;
			m_objectPreview		= null;
			
			if (a_state != m_itemToCreate)
			{
				clearSelectedObject();
				m_itemToCreate = a_state;
			}

			foreach (Button t_button in m_buttonDict.Keys)
			{
				t_button.setState(0);
			}
			a_button.setState(3);

			switch (m_itemToCreate)
			{
				case State.Platform:
					createAssetList("Content//Images//Tile//Floor//");
					break;
				case State.Ladder:
					createAssetList("Content//Images//Tile//Ladder//");
					break;
				case State.Background:
					createAssetList("Content//Images//Background//");
					break;
				case State.Delete:
					createAssetList(null);
					m_building = false;
					break;
				case State.Player:
					createAssetList(null);
					break;
				case State.None:
					createAssetList(null);
					m_building = false;
					break;
				case State.SpotLight:
					createAssetList("Content//Images//LightCone//");
					break;
				case State.Guard:
					createAssetList(null);
					break;
				case State.Wall:
					createAssetList("Content//Images//Tile//Wall//");
					break;
				case State.DuckHidingObject:
					createAssetList("Content//Images//Prop//DuckHide//");
					break;
				case State.StandHidingObject:
					createAssetList("Content//Images//Prop//StandHide//");
					break;
				case State.GuardDog:
					createAssetList("Content//Images//Sprite//GuardDog//");
					break;
				case State.LightSwitch:
					createAssetList("Content//Images//Prop//Button//");
					break;
				case State.CrossVent:
					createAssetList("Content//Images//Tile//Ventilation//Cross//");
					break;
				case State.CornerVent:
					createAssetList("Content//Images//Tile//Ventilation//Corner//");
					break;
				case State.TVent:
					createAssetList("Content//Images//Tile//Ventilation//TVent//");
					break;
				case State.StraVent:
					createAssetList("Content//Images//Tile//Ventilation//Straight//");
					break;
				case State.Ventrance:
					createAssetList("Content//Images//Tile//Ventilation//Drum//");
					break;
				case State.EndVent:
					createAssetList("Content//Images//Tile//Ventilation//EndVent//");
					break;
				case State.Camera:
					createAssetList("Content//Images//Sprite//Camera//");
					break;
				case State.Window:
					createAssetList("Content//Images//Tile//Window//");
					break;
				case State.Foreground:
					createAssetList("Content//Images//Foregrounds//");
					break;
				case State.Rope:
					createAssetList(null);
					break;
				case State.SecDoor:
					createAssetList("Content//Images//Prop//SecurityDoor//");
					break;
				case State.CornerHang:
					createAssetList(null);
					break;
				case State.Checkpoint:
					createAssetList(null);
					break;
				case State.Prop:
					createAssetList("Content//Images//Prop//Clutter//");
					break;
				case State.Key:
					createAssetList(null);
					break;
				case State.Heart:
					createAssetList(null);
					break;
				case State.Objective:
					createAssetList("Content//Images//Prop//Consumables//Objective//");
					break;
				case State.Shadow:
					createAssetList("Content//Images//Foregrounds//Shadow//");
					break;
			}
			if (m_assetButtonList != null && m_assetButtonList.Count > 0)
			{
				selectAsset(m_assetButtonList.First());
			}
			else
			{
				selectAsset(null);
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
			{
				a_guard.setRightGuardPoint(getTileCoordinates(a_position).X);
			}
			else
			{
				a_guard.setLeftGuardPoint(getTileCoordinates(a_position).X);
			}
		}

		private void setGuardCameraPoint(GuardCamera a_guard, Vector2 a_position, bool a_right)
		{
			if (a_right)
			{
				a_guard.setRightGuardPoint(a_position);
			}
			else
			{
				a_guard.setLeftGuardPoint(a_position);
			}
		}

		private void connectSpotLight(SpotLight a_spotLight, LampSwitch a_lightSwitch)
		{
			a_lightSwitch.connectSpotLight(a_spotLight);
		}

		private void setLayer(Button a_button)
		{
			foreach (Button t_button in m_layerButtonList)
			{
				t_button.setState(0);
			}

			m_currentLayer = int.Parse(a_button.getText()) - 1;
			a_button.setState(3);
		}

		private void setLayer(int a_layer)
		{
			m_currentLayer = a_layer;
			foreach (Button t_button in m_layerButtonList)
			{
				if (int.Parse(t_button.getText()) == m_currentLayer + 1)
				{
					t_button.setState(3);
				}
				else
				{
					t_button.setState(0);
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
			for (int i = 0; i < m_gameObjectList.Length; i++)
			{
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
			m_selectedInfoV2 = Vector2.Zero;
			m_selectedObject = null;
		}

		public override void setPlayer(Player a_player)
		{
			m_player = a_player;
		}

		public override Player getPlayer()
		{
			return (Player)m_player;
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
				foreach (GuiObject t_guiObject in m_guiList)
				{
					t_guiObject.draw(a_gameTime);
				}

				m_statusBar.draw(a_gameTime);

				if (m_selectedObject != null)
				{
					m_layerTextField.draw(a_gameTime);
					if (m_selectedObject is Environment)
					{
						m_parallaxLabel.draw(a_gameTime);
						m_parallaxScrollTF.draw(a_gameTime);
					}
				}

				foreach (Button t_button in m_buttonDict.Keys)
				{
					t_button.draw(a_gameTime, a_spriteBatch);
				}

				foreach (LinkedList<Button> t_buttonList in m_buttonList)
				{
					foreach (Button t_button in t_buttonList)
					{
						t_button.draw(a_gameTime, a_spriteBatch);
					}
				}
			}
			foreach (Line t_line in m_lineList)
			{
				t_line.draw();
			}
			if (m_objectPreview != null)
			{
				m_objectPreview.draw(a_gameTime);
			}
			if (m_dragLine != null)
			{
				m_dragLine.draw();
			}
		}
		#endregion
	}
}