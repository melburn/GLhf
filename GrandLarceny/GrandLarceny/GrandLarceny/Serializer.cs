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
		private static Serializer m_instance;

		private Serializer()
		{
		}

		public static Serializer getInstance()
		{
			if (m_instance == null)
			{
				m_instance = new Serializer();
			}

			return m_instance;
		}

		public void SaveLevel(string a_fileName, Level a_save)
		{
			foreach (LinkedList<GameObject> t_goSaveList in a_save.getLevelLists())
			{
				foreach (GameObject t_go in t_goSaveList)
				{
					t_go.saveObject();
				}
			}

			foreach (LinkedList<GameObject> t_goSaveList in a_save.getLevelLists())
			{
				foreach (GameObject t_go in t_goSaveList)
				{
					t_go.linkObject();
				}
			}
			MemoryStream t_stream = null;
			FileStream t_fstream = null;
			LinkedList<string> t_unikName = new LinkedList<string>();
			LinkedList<LinkedList<GameObject>> t_objekts = new LinkedList<LinkedList<GameObject>>();

			t_fstream = File.Open("Content//Levels//" + a_fileName, FileMode.Create);
			BinaryFormatter t_bFormatter = new BinaryFormatter();

			int index = 0;
			long t_fstreamLastPos = 0;
			long t_fstreamDiffSize = 0;
			GameObject.resetGameObjectId();
			

			foreach (LinkedList<GameObject> t_goList in a_save.getLevelLists())
			{

				t_fstream.Position = t_fstream.Position + 4;
				foreach(GameObject t_go in t_goList)
				{
					if (!t_unikName.Contains(t_go.GetType().Name))
					{
						t_unikName.AddLast(t_go.GetType().Name);
						t_objekts.AddLast(new LinkedList<GameObject>());
					}
					for (int i = 0; i < t_unikName.Count; ++i)
					{
						if (t_unikName.ElementAt<string>(i).Equals(t_go.GetType().Name))
						{
							t_objekts.ElementAt<LinkedList<GameObject>>(i).AddLast(t_go);
						}
					}
				}

				foreach (LinkedList<GameObject> t_serializeList in t_objekts)
				{
					try
					{
						t_stream = new MemoryStream();
						t_bFormatter.Serialize(t_stream, t_serializeList);
						byte[] t_msPos = new byte[4];
						t_msPos = BitConverter.GetBytes((int)t_stream.Position);
						t_fstream.Write(t_msPos, 0, t_msPos.Length);
						t_fstream.Write(t_stream.GetBuffer(), 0, (int)t_stream.Position);

					}
					catch (FileNotFoundException)
					{
						System.Console.WriteLine("Fail to SaveLevel(Serializer) : "+index+" Who are fuck : "+t_serializeList.ElementAt<GameObject>(0).GetType().Name);
					}
					if (t_stream != null)
					{
						t_stream.Close();
					}

				}

				byte[] t_layerSize = new byte[4];
				t_fstreamDiffSize = t_fstream.Position - t_fstreamLastPos;
				t_layerSize = BitConverter.GetBytes((int)t_fstreamDiffSize);
				t_fstream.Position = t_fstream.Position - t_fstreamDiffSize;
				t_fstream.Write(t_layerSize, 0, t_layerSize.Length);

				t_fstreamLastPos = t_fstream.Length;
				t_fstream.Position = t_fstream.Length;
				t_unikName = new LinkedList<string>();
				index++;
				t_objekts = new LinkedList<LinkedList<GameObject>>();
			}


			if (t_stream != null)
			{
				t_stream.Close();
			}
			if (t_fstream != null)
			{
				t_fstream.Close();
			}
		}


		public Level loadLevel(string a_fileName)
		{
			Level t_loadingLevel = new Level();
			FileStream t_fstream = null;
			MemoryStream t_mStream = null;
			LinkedList<GameObject> t_gameObject = new LinkedList<GameObject>();
			LinkedList<GameObject>[] t_gameObjectsList = new LinkedList<GameObject>[5];
			t_gameObjectsList[0] = new LinkedList<GameObject>();
			t_gameObjectsList[1] = new LinkedList<GameObject>();
			t_gameObjectsList[2] = new LinkedList<GameObject>();
			t_gameObjectsList[3] = new LinkedList<GameObject>();
			t_gameObjectsList[4] = new LinkedList<GameObject>();
			
			byte[] t_bytes = new byte[4];

			int t_layerIndex = 0;
			try
			{
				t_fstream = File.Open("Content//Levels//" + a_fileName, FileMode.Open);
			

				BinaryFormatter t_bFormatter = new BinaryFormatter();
			
				for (; ; )
				{
					float t_fstreamPos = t_fstream.Position;
					t_fstream.Read(t_bytes, 0, t_bytes.Length);
					int t_layerSize = BitConverter.ToInt32(t_bytes, 0);


					for (; ; )
					{
						if (t_fstream.Position >= t_layerSize + t_fstreamPos)
						{
							break;
						}

						t_bytes = new byte[4];
						t_fstream.Read(t_bytes, 0, t_bytes.Length);
						int t_objectListSize = BitConverter.ToInt32(t_bytes, 0);

						try
						{
							t_mStream = new MemoryStream();
							byte[] t_objectListInByte = new byte[t_objectListSize];
							t_fstream.Read(t_objectListInByte, 0, t_objectListSize);
							t_mStream.Write(t_objectListInByte, 0, t_objectListSize);

							t_mStream.Position = 0;
							t_gameObject = (LinkedList<GameObject>)t_bFormatter.Deserialize(t_mStream);
							foreach (GameObject t_gb in t_gameObject)
							{
								t_gameObjectsList[t_layerIndex].AddLast(t_gb);
							}
							if (t_mStream != null)
							{
								t_mStream.Close();
							}
						}
						catch (SerializationException e)
						{
							System.Console.WriteLine("Fail to DeSerialize : " + e);
							t_fstream.Position = t_fstream.Position + t_objectListSize;
						}
					}

					t_layerIndex++;
					if (t_fstream.Position >= t_fstream.Length)
					{
						break;
					}

				}
				t_loadingLevel.setLevelObjects(t_gameObjectsList);
			

			}
			catch (FileLoadException e)
			{
				System.Console.WriteLine("Fail to LoadLevel(DeSerialize) : " + e);
			}
			catch (FileNotFoundException e)
			{
				System.Console.WriteLine("Fail to find file : " + e);
			}

			


			if (t_fstream != null)
			{
				t_fstream.Close();
			}
			if (t_mStream != null)
			{
				t_mStream.Close();
			}
			
			if(t_loadingLevel == null)
			{
				t_loadingLevel = new Level();
			}

			return t_loadingLevel;

		}
	}
}
