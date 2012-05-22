using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	public class SettingsMenu : MenuState
	{
		#region Members
		private string[] m_resolutions;
		private int m_resolutionIndex;

		private Button m_btnNextResolution;
		private Button m_btnPrevResolution;
		private Button m_btnFullscreen;
		private Button m_btnAntialias;
		private Button m_btnVSync;
		private Button m_btnApply;
		private TextButton m_btnExit;
		private TextButton m_btnSave;
		private TextButton m_btnYes;
		private TextButton m_btnNo;
		private Box m_dialogBackground;

		private LinkedList<Button> m_keyList;

		private Text m_resolutionText;
		private Text m_inputFeedback;
		private Text m_countDown;
		private Text m_soundLabel;
		private Text m_musicLabel;
		private TextField m_soundTF;
		private TextField m_musicTF;

		private Dictionary<string, string> m_settingsFile;
		private Dictionary<string, string> m_defaultFile;

		private string m_settingsPath;

		private Color m_normal	= new Color(187, 194, 195);
		private Color m_hover	= new Color(255, 255, 255);
		private Color m_pressed	= new Color(132, 137, 138);

		private TimeSpan m_timeOut;
		#endregion

		#region Constructor & Load
		public SettingsMenu() : base()
		{
			m_settingsPath = "Content//wtf//settings.ini";
			m_buttonList.AddLast(m_keyList = new LinkedList<Button>());
			m_settingsFile = new Dictionary<string, string>();
			m_defaultFile = new Dictionary<string, string>();
			m_resolutions = new string[] 
			{
				"640x480"  , "800x600"	, "1024x768", "1152x864", "1280x720" , "1280x768" , "1280x800" , "1280x960" , "1280x1024",
				"1360x768" , "1366x768" , "1440x900", "1600x900", "1600x1024", "1600x1200", "1680x1050", "1920x1080", "1920x1200"
			};

			foreach (string t_string in File.ReadAllLines("Content//wtf//settings.ini"))
			{
				string[] t_stringToAdd = t_string.Split('=');

				if (t_stringToAdd.Length > 1)
				{
					m_settingsFile.Add(t_stringToAdd[0], t_stringToAdd[1]);
					m_defaultFile.Add(t_stringToAdd[0], t_stringToAdd[1]);
				}
				else
				{
					m_settingsFile.Add(t_stringToAdd[0], null);
					m_defaultFile.Add(t_stringToAdd[0], null);
				}
			}
			setResolutionLabel();
		}

		public override void load()
		{
			base.load();
			createButtons();
		}
		#endregion

		#region Setting Methods
		private void createButtons()
		{
			int i = 0;
			m_guiList = new LinkedList<GuiObject>();
			m_keyList = new LinkedList<Button>();
			m_guiList.AddLast(m_soundLabel = new Text(new Vector2(500, 150), "Sound", "VerdanaBold", m_normal, false));
			m_guiList.AddLast(m_musicLabel = new Text(new Vector2(500, 200), "Music", "VerdanaBold", m_normal, false));
			m_guiList.AddLast(m_soundTF = new TextField(new Vector2(600, 150), 100, 20, false, true, false, 3));
			m_guiList.AddLast(m_musicTF = new TextField(new Vector2(600, 200), 100, 20, false, true, false, 3));
			m_soundTF.setText(m_settingsFile["Sound"].ToString());
			m_musicTF.setText(m_settingsFile["Music"].ToString());
			m_guiList.AddLast(m_resolutionText = new Text(new Vector2(155, 160), m_resolutions[m_resolutionIndex], "VerdanaBold", Color.White, false));
			m_resolutionText.setLayer(0.112f);
			Vector2 t_textOffset = new Vector2(40, 10);

			string[] t_currentBindings = Loader.getInstance().getSettingsBlock("Input", m_settingsPath);

			foreach (string t_string in t_currentBindings)
			{
				string[] t_settingString = t_string.Split('=');
				m_guiList.AddLast(new Text(new Vector2(400, 300 + (40 * i)), t_settingString[0], "VerdanaBold", Color.White, false));
				m_keyList.AddLast(new Button(null, new Vector2(450, 300 + (40 * i++)), t_settingString[1], "VerdanaBold", Color.White, t_textOffset));
			}
			foreach (Button t_button in m_keyList)
			{
				t_button.m_clickEvent += new Button.clickDelegate(awaitInput);
			}

			m_keyList.AddLast(m_btnNextResolution	= new Button(null, new Vector2(250, 150)));
			m_keyList.AddLast(m_btnPrevResolution	= new Button(null, new Vector2(100, 150)));
			m_keyList.AddLast(m_btnFullscreen		= new Button(null, new Vector2(100, 200), "Full Screen", "VerdanaBold", Color.White, new Vector2(50, 5)));
			m_keyList.AddLast(m_btnAntialias		= new Button(null, new Vector2(100, 250), "Anti-Alias", "VerdanaBold", Color.White, new Vector2(50, 5)));
			m_keyList.AddLast(m_btnVSync			= new Button(null, new Vector2(100, 300), "Vertical Sync", "VerdanaBold", Color.White, new Vector2(50, 5)));
			m_keyList.AddLast(m_btnApply			= new Button("btn_asset_list", new Vector2(100, 350), "Apply", "VerdanaBold", Color.White, new Vector2(5, 3)));
			m_btnSave								= new TextButton(Vector2.Zero, "Accept", "MotorwerkLarge", m_normal, m_hover, m_pressed, Color.Red);
			m_btnExit								= new TextButton(Vector2.Zero, "Cancel", "MotorwerkLarge", m_normal, m_hover, m_pressed, Color.Red);
			m_btnSave.setPosition(new Vector2(15, Game.getInstance().getResolution().Y - 150));
			m_btnExit.setPosition(new Vector2(15, Game.getInstance().getResolution().Y - 90));

			m_btnNextResolution.m_clickEvent	+= new Button.clickDelegate(nextResolution);
			m_btnPrevResolution.m_clickEvent	+= new Button.clickDelegate(prevResolution);
			m_btnFullscreen.m_clickEvent		+= new Button.clickDelegate(toggleButton);
			m_btnAntialias.m_clickEvent			+= new Button.clickDelegate(toggleButton);
			m_btnVSync.m_clickEvent				+= new Button.clickDelegate(toggleButton);
			m_btnApply.m_clickEvent				+= new Button.clickDelegate(testGraphics);
			m_btnSave.m_clickEvent				+= new TextButton.clickDelegate(saveSettings);			
			m_btnExit.m_clickEvent				+= new TextButton.clickDelegate(exitSettings);

			if (Game.getInstance().m_graphics.IsFullScreen)
			{
				m_btnFullscreen.setState(Button.State.Toggled);
			}
			if (Game.getInstance().m_graphics.PreferMultiSampling)
			{
				m_btnAntialias.setState(Button.State.Toggled);
			}
			if (Game.getInstance().m_graphics.SynchronizeWithVerticalRetrace)
			{
				m_btnVSync.setState(Button.State.Toggled);
			}

			foreach (Button t_button in m_keyList)
			{
				t_button.setLayer(0.112f);
			}
		}

		private void setResolutionLabel()
		{
			string t_resolutionToFind = m_settingsFile["ScreenWidth"] + "x" + m_settingsFile["ScreenHeight"];

			for (int i = 0; i < m_resolutions.Length; i++)
			{
				if (m_resolutions[i].Equals(t_resolutionToFind))
				{
					m_resolutionIndex = i;
					break;
				}
			}
		}

		private void saveSettings(Button a_button)
		{
			StreamWriter t_file = new StreamWriter(m_settingsPath);
			t_file.Flush();
			foreach (KeyValuePair<string, string> t_kvPair in m_settingsFile)
			{
				if (t_kvPair.Key.StartsWith("["))
				{
					t_file.WriteLine(t_kvPair.Key);				
				}
				else
				{
					t_file.WriteLine(t_kvPair.Key + "=" + t_kvPair.Value);
				}
			}
			t_file.Close();
			m_btnExit.setState(Button.State.Normal);
			applyGraphics();
			exitSettings(m_btnExit);
		}

		private void awaitInput(Button a_button)
		{
			m_inputFeedback = new Text(new Vector2(Game.getInstance().getResolution().X / 2, 300), "Select input for: " + a_button.getText(), "VerdanaBold", Color.White, false);
			lockButtons(a_button);
		}

		private void setKeybinding(Keys k)
		{
			if (k == Keys.Escape)
			{
				m_inputFeedback = null;
				unlockButtons();
				return;
			}
			foreach (Button t_button in m_keyList)
			{
				if (t_button.getState() == Button.State.Toggled)
				{
					m_settingsFile[t_button.getText()] = k.ToString();
					m_inputFeedback = null;
					t_button.setText(k.ToString());
					unlockButtons();
					changedSettings();
					break;
				}
			}
		}

		private void lockButtons(Button a_button)
		{
			GuiListFactory.setSelection(m_keyList, Button.State.Pressed);
			if (a_button != null)
			{
				a_button.setState(Button.State.Toggled);
			}
		}

		private void unlockButtons()
		{
			GuiListFactory.setSelection(m_keyList, Button.State.Normal);
		}

		private void nextResolution(Button a_button)
		{
			try
			{
				m_resolutionText.setText(m_resolutions[++m_resolutionIndex]);
			}
			catch (IndexOutOfRangeException)
			{
				m_resolutionText.setText(m_resolutions[m_resolutionIndex = 0]);
			}
			m_settingsFile["ScreenWidth"]	= m_resolutionText.getText().Split('x')[0];
			m_settingsFile["ScreenHeight"]	= m_resolutionText.getText().Split('x')[1];
		}

		private void prevResolution(Button a_button)
		{
			try
			{
				m_resolutionText.setText(m_resolutions[--m_resolutionIndex]);
			}
			catch (IndexOutOfRangeException)
			{
				m_resolutionText.setText(m_resolutions[m_resolutionIndex = m_resolutions.Length - 1]);
			}
			m_settingsFile["ScreenWidth"]	= m_resolutionText.getText().Split('x')[0];
			m_settingsFile["ScreenHeight"]	= m_resolutionText.getText().Split('x')[1];
		}

		private void toggleButton(Button a_button)
		{
			if (a_button.getState() == Button.State.Toggled)
			{
				a_button.setState(Button.State.Normal);
			}
			else
			{
				a_button.setState(Button.State.Toggled);				
			}

			if (a_button == m_btnFullscreen)
			{
				m_settingsFile["Fullscreen"] = (m_btnFullscreen.getState() == Button.State.Toggled).ToString().ToLower();
			}
			else if (a_button == m_btnAntialias)
			{
				m_settingsFile["Antialias"]	= (m_btnAntialias.getState() == Button.State.Toggled).ToString().ToLower();
			}
			else if (a_button == m_btnVSync)
			{
				m_settingsFile["VSync"]	= (m_btnVSync.getState() == Button.State.Toggled).ToString().ToLower();
			}
		}

		private void exitSettings(Button a_button)
		{
			Game.getInstance().setState(new MainMenu());
		}

		private void buttonYes(Button a_button)
		{
			if (m_countDown != null)
			{
				m_countDown = null;
				removeDialog();
			}
			else
			{
				exitSettings(m_btnExit);
			}
		}

		private void buttonNo(Button a_button)
		{
			if (m_countDown != null)
			{
				resetGraphics();
				removeDialog();
			}
			else
			{
				m_btnExit.setState(Button.State.Normal);
				removeDialog();
			}
		}

		private void testGraphics(Button a_button)
		{
			applyGraphics();

			if (m_countDown == null)
			{
				createDialog("Apply Settings?");
				m_countDown = new Text(Game.getInstance().getResolution() / 2, "", "MotorwerkLarge", m_normal, false);
				m_countDown.move(new Vector2(-(m_countDown.getBox().Width / 2) - 20, -75));
				m_timeOut = Game.getInstance().getTotalGameTime() + new TimeSpan(0, 0, 10);
			}

			createButtons();
		}

		private void applyGraphics()
		{
			Game.getInstance().m_graphics.PreferredBackBufferWidth			= int.Parse(m_settingsFile["ScreenWidth"]);
			Game.getInstance().m_graphics.PreferredBackBufferHeight			= int.Parse(m_settingsFile["ScreenHeight"]);
			Game.getInstance().m_graphics.IsFullScreen						= bool.Parse(m_settingsFile["Fullscreen"]);
			Game.getInstance().m_graphics.PreferMultiSampling				= bool.Parse(m_settingsFile["Antialias"]);
			Game.getInstance().m_graphics.SynchronizeWithVerticalRetrace	= bool.Parse(m_settingsFile["VSync"]);
			Game.getInstance().m_graphics.ApplyChanges();
		}

		private void resetGraphics()
		{
			m_settingsFile["ScreenWidth"]	= m_defaultFile["ScreenWidth"];
			m_settingsFile["ScreenHeight"]	= m_defaultFile["ScreenHeight"];
			m_settingsFile["Fullscreen"]	= m_defaultFile["Fullscreen"];
			m_settingsFile["Antialias"]		= m_defaultFile["Antialias"];
			m_settingsFile["VSync"]			= m_defaultFile["VSync"];

			applyGraphics();
			setResolutionLabel();
			createButtons();
		}

		private void applySettingsBlock(string a_block, string[] a_stringArray)
		{
			string[] t_file = File.ReadAllLines(m_settingsPath);
			int i = 0;

			while (true)
			{
				if (t_file[i++].Equals("[" + a_block + "]"))
				{
					break;
				}
			}
			for (int j = 0; j < a_stringArray.Length; i++, j++)
			{
				t_file[i] = a_stringArray[j];
			}
			StreamWriter t_writeFile = new StreamWriter(m_settingsPath);
			t_writeFile.Flush();
			for (int j = 0; j < t_file.Length; j++)
			{
				t_writeFile.WriteLine(t_file[j]);
			}
			t_writeFile.Close();
		}

		private void changedSettings()
		{
			m_btnSave.setState(Button.State.Normal);
		}

		private void createDialog(string a_text)
		{
			Vector2 t_halfRes = Game.getInstance().getResolution() / 2;

			m_inputFeedback = new Text(t_halfRes, a_text, "VerdanaBold", m_normal, false);
			m_inputFeedback.move(new Vector2(-(m_inputFeedback.getBox().Width / 2), -100));
			m_buttons.AddLast(m_btnYes = new TextButton(new Vector2(t_halfRes.X - 175, t_halfRes.Y), "YES", "MotorwerkLarge", m_normal, m_hover, m_pressed, Color.Red));
			m_buttons.AddLast(m_btnNo = new TextButton(new Vector2(t_halfRes.X + 65, t_halfRes.Y), "NO", "MotorwerkLarge", m_normal, m_hover, m_pressed, Color.Red));
			m_btnYes.m_clickEvent += new TextButton.clickDelegate(buttonYes);
			m_btnNo.m_clickEvent += new TextButton.clickDelegate(buttonNo);
			m_dialogBackground = new Box(new Vector2(t_halfRes.X - 200, t_halfRes.Y - 125), 400, 220, Color.Gray, true);
		}

		private void removeDialog()
		{
			m_inputFeedback = null;
			m_buttons.Remove(m_btnYes);
			m_buttons.Remove(m_btnNo);
			m_btnYes = null;
			m_btnNo = null;
			m_dialogBackground = null;
			m_countDown = null;
		}
		#endregion
		
		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			if (KeyboardHandler.keyClicked(Keys.Escape) && m_inputFeedback == null)
			{
				exitSettings(m_btnExit);
			}
			if (KeyboardHandler.keyClicked(Keys.Enter))
			{
				if (m_soundTF.isWriting())
				{
					m_settingsFile["Sound"] = m_soundTF.getText();
					m_soundTF.setWrite(false);
				}
				else if (m_musicTF.isWriting())
				{
					m_settingsFile["Music"] = m_musicTF.getText();
					m_musicTF.setWrite(false);
				}
			}
			foreach (Button t_button in m_keyList)
			{
				t_button.update();
			}
			m_btnSave.update();
			m_btnExit.update();

			if (m_dialogBackground != null)
			{
				m_dialogBackground.update(a_gameTime);
			}

			if (m_countDown != null)
			{
				if (Game.getInstance().getTotalGameTime() > m_timeOut)
				{
					resetGraphics();
					removeDialog();
					m_countDown = null;
				}
				else
				{
					m_countDown.setText((m_timeOut.Seconds - Game.getInstance().getTotalGameTime().Seconds).ToString());
				}
			}

			if (m_inputFeedback != null)
			{
				m_inputFeedback.update(a_gameTime);

				if (KeyboardHandler.getPressedKeys().Count() > 0)
				{
					setKeybinding(KeyboardHandler.getPressedKeys()[0]);
				}
			}
		}

		public override void draw(GameTime a_gameTime, SpriteBatch a_spriteBatch)
		{
			base.draw(a_gameTime, a_spriteBatch);
			foreach (Button t_button in m_keyList)
			{
				t_button.draw(a_gameTime, a_spriteBatch);
			}
			
			m_btnSave.draw(a_gameTime, a_spriteBatch);
			m_btnExit.draw(a_gameTime, a_spriteBatch);

			if (m_dialogBackground != null)
			{
				m_dialogBackground.draw(a_gameTime);
			}

			if (m_inputFeedback != null)
			{
				m_inputFeedback.draw(a_gameTime);
			}
			if (m_countDown != null)
			{
				m_countDown.draw(a_gameTime);
			}
		}
		#endregion
	}
}