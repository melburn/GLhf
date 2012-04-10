using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrandLarceny
{
	public class ErrorLogger
	{
		private static ErrorLogger s_instance;
		private Boolean m_consoleWrite = true;

		private ErrorLogger()
		{
		}

		public static ErrorLogger getInstance()
		{
			if (s_instance == null)
			{
				s_instance = new ErrorLogger();
			}
			return s_instance;
		}

		public bool writeString(String a_str, bool a_console)
		{
			try
			{
				TextWriter t_textWriter = new StreamWriter("ErrorLog.txt", true);
				t_textWriter.WriteLine(a_str);
				t_textWriter.Close();
			}
			catch
			{
				//wow, we're screwed
				System.Console.WriteLine("Failed to write error to file");
				System.Console.WriteLine(a_str);
				return false;
			}
			if (a_console)
			{
				System.Console.WriteLine(a_str);
			}
			return true;
		}

		public bool writeString(String a_str)
		{
			return writeString(a_str, m_consoleWrite);
		}
	}
}
