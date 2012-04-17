using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;

namespace GrandLarceny
{
	[Serializable()]
	public class GuiListFactory
	{
		public static LinkedList<Button> createListFromDirectory(string a_path, string[] a_extension, string a_buttonGraphic)
		{
			LinkedList<Button> t_guiList = new LinkedList<Button>();
			string[] t_fileList = Directory.GetFiles(a_path);

			for (int i = 0, j = 0; i < t_fileList.Length; i++)
			{
				bool t_accepted = false;

				foreach (string t_ext in a_extension) {
					if (t_fileList[i].EndsWith(t_ext)) {
						t_accepted = true;
						continue;
					}
				}

				if (!t_accepted) {
					continue;
				}

				string[] t_splitPath = Regex.Split(t_fileList[i], "//");
				string[] t_extless = t_splitPath[t_splitPath.Length - 1].Split('.');
				t_guiList.AddLast(new Button(a_buttonGraphic, new Vector2(0, 0), t_extless[0], "VerdanaBold", Color.Black, new Vector2(0, 0)));
				j++;
			}
			return t_guiList;
		}

		public static LinkedList<Button> createNumeratedList(int a_numberOfElements, string a_buttonGraphic)
		{
			LinkedList<Button> t_guiList = new LinkedList<Button>();
			for (int i = 0; i < a_numberOfElements; i++) {
				t_guiList.AddLast(new Button(a_buttonGraphic, new Vector2(0, 0), (i + 1).ToString(), "VerdanaBold", Color.Black, new Vector2(0, 0)));
			}
			return t_guiList;
		}

		public static void setListPosition(LinkedList<Button> a_list, Vector2 a_position)
		{
			foreach (Button t_button in a_list)
			{
				t_button.setPosition(a_position);
			}
		}

		public static void setButtonDistance(LinkedList<Button> a_list, Vector2 a_distance)
		{
			int i = 0;
			foreach (Button t_button in a_list)
			{
				t_button.move(a_distance * i);
				i++;
			}
		}

		public static void setTextOffset(LinkedList<Button> a_list, Vector2 a_offset)
		{
			foreach (Button t_button in a_list)
			{
				t_button.setText(t_button.getText(), a_offset);
			}
		}
	}
}
