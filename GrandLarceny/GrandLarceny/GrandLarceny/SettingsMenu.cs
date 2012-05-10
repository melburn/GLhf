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
		private string m_valueToChange;

		private Button m_btnSetUp;
		private Button m_btnSetDown;
		private Button m_btnSetLeft;
		private Button m_btnSetRight;
		private Button m_btnSetJump;
		private Button m_btnSetAction;
		private Button m_btnSetRoll;
		private Button m_btnSetSprint;

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

			m_guiList.AddLast(m_resolutionText = new Text(new Vector2(500, 150), m_resolutions[m_resolutionIndex], "VerdanaBold", Color.Black, false));
		}

		public override void load()
		{
			base.load();
			int i = 0;
			m_settingsFile = "Content//wtf//settings.ini";
			Vector2 t_textOffset = new Vector2(40, 10);
			string[] t_currentBindings = Loader.getInstance().getSettingsBlock("Input", m_settingsFile);

			foreach (string t_string in t_currentBindings)
			{
				string[] t_settingString = t_string.Split('=');
				m_guiList.AddLast(new Text(new Vector2(400, 300 + (40 * i)), t_settingString[0], "VerdanaBold", Color.Black, false));
				m_keyList.AddLast(new Button(null, new Vector2(450, 300 + (40 * i++)), t_settingString[1], "VerdanaBold", Color.Black, t_textOffset));
			}
			/*
			m_keyList.AddLast(m_btnSetUp		= new Button(null, new Vector2(400, 300 + (40 * i++)), "Move Up", "VerdanaBold", Color.Black, t_textOffset));
			m_keyList.AddLast(m_btnSetDown		= new Button(null, new Vector2(400, 300 + (40 * i++)), "Move Down", "VerdanaBold", Color.Black, t_textOffset));
			m_keyList.AddLast(m_btnSetLeft		= new Button(null, new Vector2(400, 300 + (40 * i++)), "Move Left", "VerdanaBold", Color.Black, t_textOffset));
			m_keyList.AddLast(m_btnSetRight		= new Button(null, new Vector2(400, 300 + (40 * i++)), "Move Right", "VerdanaBold", Color.Black, t_textOffset));
			m_keyList.AddLast(m_btnSetJump		= new Button(null, new Vector2(400, 300 + (40 * i++)), "Jump", "VerdanaBold", Color.Black, t_textOffset));
			m_keyList.AddLast(m_btnSetAction	= new Button(null, new Vector2(400, 300 + (40 * i++)), "Action Button", "VerdanaBold", Color.Black, t_textOffset));
			m_keyList.AddLast(m_btnSetRoll		= new Button(null, new Vector2(400, 300 + (40 * i++)), "Roll", "VerdanaBold", Color.Black, t_textOffset));
			m_keyList.AddLast(m_btnSetSprint	= new Button(null, new Vector2(400, 300 + (40 * i++)), "Sprint", "VerdanaBold", Color.Black, t_textOffset));
			*/
			foreach (Button t_button in m_keyList)
			{
				t_button.m_clickEvent += new Button.clickDelegate(awaitInput);
			}
		}
		#endregion

		#region Setting Methods
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
			string[] t_settingsFile;
			StreamReader t_file = new StreamReader("Content//wtf//settings.ini");

			t_settingsFile = t_file.ReadToEnd().Split('\n');
			t_file.Close();
			
			StreamWriter t_fileToWrite = new StreamWriter("Content//wtf//settings.ini");
			t_fileToWrite.Flush();

			foreach (string t_string in t_settingsFile)
			{
				string t_editedString = "";
				if (t_string.StartsWith(a_setting))
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
				if (t_button.getState() == 2)
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
			GuiListFactory.setSelection(m_keyList, 3);
			if (a_button != null)
			{
				a_button.setState(2);
			}
		}

		private void unlockButtons()
		{
			GuiListFactory.setSelection(m_keyList, 0);
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
