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
		private Button m_btnExit;
		private Button m_btnApply;
		private Button m_btnSave;

		private LinkedList<Button> m_keyList;

		private Text m_resolutionText;
		private Text m_inputFeedback;

		private Dictionary<string, string> m_settingsFile = new Dictionary<string, string>();

		private string m_settingsPath;
		#endregion

		#region Constructor & Load
		public SettingsMenu() : base()
		{
			m_settingsPath = "Content//wtf//settings.ini";
			m_buttonList.AddLast(m_keyList = new LinkedList<Button>());
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
				}
				else
				{
					m_settingsFile.Add(t_stringToAdd[0], null);
				}
			}

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
			m_guiList.AddLast(m_resolutionText = new Text(new Vector2(155, 160), m_resolutions[m_resolutionIndex], "VerdanaBold", Color.White, false));
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
			m_keyList.AddLast(m_btnFullscreen		= new Button(null, new Vector2(100, 200)));
			m_keyList.AddLast(m_btnAntialias		= new Button(null, new Vector2(100, 250)));
			m_keyList.AddLast(m_btnVSync			= new Button(null, new Vector2(100, 300)));
			m_keyList.AddLast(m_btnExit				= new Button("btn_event_exit", new Vector2(0, Game.getInstance().getResolution().Y - 50)));
			m_keyList.AddLast(m_btnApply			= new Button("btn_asset_list", new Vector2(100, 350), "Apply", "VerdanaBold", Color.White, new Vector2(5, 3)));
			m_keyList.AddLast(m_btnSave				= new Button("btn_event_exit", new Vector2(0, Game.getInstance().getResolution().Y - 150)));

			m_btnNextResolution.m_clickEvent	+= new Button.clickDelegate(nextResolution);
			m_btnPrevResolution.m_clickEvent	+= new Button.clickDelegate(prevResolution);
			m_btnFullscreen.m_clickEvent		+= new Button.clickDelegate(fullscreenToggle);
			m_btnAntialias.m_clickEvent			+= new Button.clickDelegate(antialiasToggle);
			m_btnVSync.m_clickEvent				+= new Button.clickDelegate(vsyncToggle);
			m_btnExit.m_clickEvent				+= new Button.clickDelegate(exitSettings);
			m_btnApply.m_clickEvent				+= new Button.clickDelegate(applyGraphics);
			m_btnSave.m_clickEvent				+= new Button.clickDelegate(saveSettings);

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
		}

		private void saveSettings(Button a_button)
		{
			StreamWriter t_file = new StreamWriter(m_settingsPath);
			t_file.Flush();
			foreach (KeyValuePair<string, string> t_kvPair in m_settingsFile)
			{
				t_file.WriteLine(t_kvPair.Key + "=" + t_kvPair.Value);
			}
			t_file.Close();
		}

		private void awaitInput(Button a_button)
		{
			m_inputFeedback = new Text(new Vector2(300, 300), "Select input for: " + a_button.getText(), "VerdanaBold", Color.White, false);
			lockButtons(a_button);
		}

		private void setKeybinding(Keys k)
		{
			foreach (Button t_button in m_keyList)
			{
				if (t_button.getState() == Button.State.Toggled)
				{
					m_settingsFile[t_button.getText()] = k.ToString();
					m_inputFeedback = null;
					t_button.setText(k.ToString());
					unlockButtons();
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
		}

		private void fullscreenToggle(Button a_button)
		{
			if (m_btnFullscreen.getState() == Button.State.Toggled)
			{
				m_btnFullscreen.setState(Button.State.Normal);
			}
			else
			{
				m_btnFullscreen.setState(Button.State.Toggled);				
			}
		}

		private void antialiasToggle(Button a_button)
		{
			if (m_btnAntialias.getState() == Button.State.Toggled)
			{
				m_btnAntialias.setState(Button.State.Normal);
			}
			else
			{
				m_btnAntialias.setState(Button.State.Toggled);
			}
		}

		private void vsyncToggle(Button a_button)
		{
			if (m_btnVSync.getState() == Button.State.Toggled)
			{
				m_btnVSync.setState(Button.State.Normal);
			}
			else
			{
				m_btnVSync.setState(Button.State.Toggled);
			}
		}

		private void exitSettings(Button a_button)
		{
			Game.getInstance().setState(new MainMenu());
		}

		private void applyGraphics(Button a_button)
		{
			m_settingsFile["ScreenWidth"]	= m_resolutionText.getText().Split('x')[0];
			m_settingsFile["ScreenHeight"]	= m_resolutionText.getText().Split('x')[1];
			m_settingsFile["Fullscreen"]	= (m_btnFullscreen.getState()	== Button.State.Toggled).ToString().ToLower();
			m_settingsFile["Antialias"]		= (m_btnAntialias.getState()	== Button.State.Toggled).ToString().ToLower();
			m_settingsFile["VSync"]			= (m_btnVSync.getState()		== Button.State.Toggled).ToString().ToLower();

			m_keyList = null;
			applySettingsBlock("Graphics", new string[] 
			{ 
				"ScreenWidth=" + m_settingsFile["ScreenWidth"]	, "ScreenHeight=" + m_settingsFile["ScreenHeight"], "Fullscreen=" + m_settingsFile["Fullscreen"],
				"Antialias=" + m_settingsFile["Antialias"]		, "VSync=" + m_settingsFile["VSync"]
			});
			Loader.getInstance().loadGraphicSettings(m_settingsPath);
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
		#endregion

		#region Update & Draw
		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			foreach (Button t_button in m_keyList)
			{
				t_button.update();
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
			if (m_inputFeedback != null)
			{
				m_inputFeedback.draw(a_gameTime);
			}
		}
		#endregion
	}
}