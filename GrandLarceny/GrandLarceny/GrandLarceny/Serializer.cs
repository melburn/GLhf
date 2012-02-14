using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace GrandLarceny
{
	class Serializer
	{
		private static Serializer m_instace;

		private Serializer()
		{
		}

		public static Serializer getInstace()
		{
			if (m_instace == null)
			{
				m_instace = new Serializer();
			}

			return m_instace;
		}

		public void SaveLevel(string a_fileName, Level a_save)
		{
			Stream t_stream = null;
			try
			{
				t_stream = File.Open("Content//Levels//" + a_fileName, FileMode.Create);
				BinaryFormatter t_bFormatter = new BinaryFormatter();
				t_bFormatter.Serialize(t_stream, a_save);
			}
			catch
			{
				System.Console.WriteLine("Fail to SaveLevel(Serializer) : ");
			}

			if (t_stream != null)
			{
				System.Console.WriteLine("asdf");
				t_stream.Close();
			}
		}

		public Level loadLevel(string a_fileName)
		{
			Level t_loadingLevel = null;
			Stream t_stream = null;
			try
			{
				t_stream = File.Open("Content//Levels//" + a_fileName, FileMode.Open);
				BinaryFormatter t_bFormatter = new BinaryFormatter();
				t_loadingLevel = (Level)t_bFormatter.Deserialize(t_stream);
			}
			catch (FileLoadException e)
			{
				System.Console.WriteLine("Fail to LoadLevel(DeSerialize) : " + e);
			}
			catch (FileNotFoundException e)
			{
				System.Console.WriteLine("Fail to find file : " + e);
			}
			catch (SerializationException e)
			{
				System.Console.WriteLine("Fail to DeSerialize : " + e);
			}
			finally
			{
				t_loadingLevel = new Level();
			}
			
			

			if (t_stream != null)
			{
				t_stream.Close();
			}

			return t_loadingLevel;

		}
	}
}
