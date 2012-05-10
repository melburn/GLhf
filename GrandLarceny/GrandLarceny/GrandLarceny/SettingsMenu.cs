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
	class SettingsMenu : MenuState
	{
		#region Members
		private string[] m_resolutions;
		private int m_resolutionIndex;

		private Button m_btnNextResolution;
		private Button m_btnPrevResolution;
		private Button m_btnFullscreen;
		private Button m_btnExit;
		private Button m_btnApply;

		private LinkedList<Button> m_keyList;

		private Text m_resolutionText;
		private Text m_inputFeedback;

		private string m_settingsFile;
		#endregion

		#region Constructor & Load
		public SettingsMenu() : base()
		{
			m_buttonList.AddLast(m_keyList = new LinkedList<Button>());
			m_resolutions = new string[] 
			{
				"640x480"	, "800x600"	, "1024x768"	, "1152x864"	, "1280x720"	, "1280x768"	, "1360x768",
				"1366x768"	, "1440x900", "1600x1200"	, "1680x1050"	, "1920x1080"	, "1920x1200" 
			};

			string t_resolutionToFind = getValueFromString("ScreenWidth") + "x" + getValueFromString("ScreenHeight");

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
			m_settingsFile = "Content//wtf//settings.ini";
			m_guiList = new LinkedList<GuiObject>();
			m_keyList = new LinkedList<Button>();
			m_guiList.AddLast(m_resolutionText = new Text(new Vector2(155, 160), m_resolutions[m_resolutionIndex], "VerdanaBold", Color.Black, false));
			Vector2 t_textOffset = new Vector2(40, 10);
			string[] t_currentBindings = Loader.getInstance().getSettingsBlock("Input", m_settingsFile);

			foreach (string t_string in t_currentBindings)
			{
				string[] t_settingString = t_string.Split('=');
				m_guiList.AddLast(new Text(new Vector2(400, 300 + (40 * i)), t_settingString[0], "VerdanaBold", Color.Black, false));
				m_keyList.AddLast(new Button(null, new Vector2(450, 300 + (40 * i++)), t_settingString[1], "VerdanaBold", Color.Black, t_textOffset));
			}
			foreach (Button t_button in m_keyList)
			{
				t_button.m_clickEvent += new Button.clickDelegate(awaitInput);
			}

			m_keyList.AddLast(m_btnNextResolution	= new Button(null, new Vector2(250, 150)));
			m_keyList.AddLast(m_btnPrevResolution	= new Button(null, new Vector2(100, 150)));
			m_keyList.AddLast(m_btnFullscreen		= new Button(null, new Vector2(100, 200)));
			m_keyList.AddLast(m_btnExit				= new Button("btn_event_exit", new Vector2(0, Game.getInstance().getResolution().Y - 50)));
			m_keyList.AddLast(m_btnApply			= new Button("btn_asset_list", new Vector2(140, 200), "Apply", "VerdanaBold", Color.Black, new Vector2(5, 3)));

			m_btnNextResolution.m_clickEvent	+= new Button.clickDelegate(nextResolution);
			m_btnPrevResolution.m_clickEvent	+= new Button.clickDelegate(prevResolution);
			m_btnFullscreen.m_clickEvent		+= new Button.clickDelegate(fullscreenToggle);
			m_btnExit.m_clickEvent				+= new Button.clickDelegate(exitSettings);
			m_btnApply.m_clickEvent				+= new Button.clickDelegate(applyGraphics);

			if (Game.getInstance().m_graphics.IsFullScreen)
			{
				m_btnFullscreen.setState(3);
			}
		}

		private string getValueFromString(string a_setting)
		{
			string[] t_settingsFile;
			t_settingsFile = File.ReadAllLines("Content//wtf//settings.ini");
			foreach (string t_string in t_settingsFile)
			{
				if (t_string.StartsWith(a_setting))
				{
					string[] t_splitString = t_string.Split('=');
					return t_splitString[1];
				}
			}
			return null;
		}

		private void setValueFromString(string a_setting, string a_value)
		{
			string[] t_settingsFile = File.ReadAllLines("Content//wtf//settings.ini");
			
			StreamWriter t_fileToWrite = new StreamWriter("Content//wtf//settings.ini");
			t_fileToWrite.Flush();

			foreach (string t_string in t_settingsFile)
			{
				if (t_string.StartsWith("["))
				{
					t_fileToWrite.WriteLine(t_string);
					continue;
				}
				string t_settingToFind = t_string.Split('=')[0];
				string t_editedString = "";
				if (t_string.Split('=')[0].Equals(t_settingToFind))
				{
					string[] t_splitString = t_string.Split('=');
					t_splitString[1] = a_value;
					t_editedString = t_splitString[0] + "=" + t_splitString[1];
				}
				if (!t_editedString.Equals(""))
				{
					t_fileToWrite.WriteLine(t_editedString);
				}
				else
				{
					t_fileToWrite.WriteLine(t_string);
				}
			}
			t_fileToWrite.Close();
		}

		private void awaitInput(Button a_button)
		{
			m_inputFeedback = new Text(new Vector2(300, 300), "Select input for: " + a_button.getText(), "VerdanaBold", Color.Black, false);
			lockButtons(a_button);
		}

		private void setKeybinding(Keys k)
		{
			foreach (Button t_button in m_keyList)
			{
				if (t_button.getState() == Button.State.Toggled)
				{
					setValueFromString(t_button.getText(), k.ToString());
					m_inputFeedback = null;
					t_button.setText(k.ToString());
					unlockButtons();
					break;
				}
			}
		}

		private void lockButtons(Button a_button)
		{
			GuiListFactory.setSelection(m_keyList, 2);
			if (a_button != null)
			{
				a_button.setState(3);
			}
		}

		private void unlockButtons()
		{
			GuiListFactory.setSelection(m_keyList, 0);
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

		private void exitSettings(Button a_button)
		{
			Game.getInstance().setState(new MainMenu());
		}

		private void applyGraphics(Button a_button)
		{
			setValueFromString("ScreenWidth", m_resolutionText.getText().Split('x')[0]);
			setValueFromString("ScreenHeight", m_resolutionText.getText().Split('x')[1]);
			if (m_btnFullscreen.getState() == Button.State.Toggled)
			{
				setValueFromString("Fullscreen", "true");
			}
			else
			{
				setValueFromString("Fullscreen", "false");
			}
			m_keyList = null;
			Loader.getInstance().loadGraphicSettings(m_settingsFile);
			createButtons();
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
