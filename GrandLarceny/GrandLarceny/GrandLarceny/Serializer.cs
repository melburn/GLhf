using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GrandLarceny.Events;


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
			foreach (LinkedList<GameObject> t_goSaveList in a_save.getGameObjects())
			{
				foreach (GameObject t_go in t_goSaveList)
				{
					t_go.saveObject();
				}
			}

			foreach (LinkedList<GameObject> t_goSaveList in a_save.getGameObjects())
			{
				foreach (GameObject t_go in t_goSaveList)
				{
					t_go.linkObject();
				}
			}

			foreach (Event t_e in a_save.getEvents())
			{
				t_e.linkObject();
			}
			MemoryStream t_stream = null;
			FileStream t_fstream = null;

			t_fstream = File.Open("Content//Levels//" + a_fileName, FileMode.Create);
			BinaryFormatter t_bFormatter = new BinaryFormatter();
			

			LinkedList<string> t_unikName = new LinkedList<string>();
			LinkedList<LinkedList<GameObject>> t_objekts = new LinkedList<LinkedList<GameObject>>();

			int index = 0;
			
			long t_fstreamDiffSize = 0;
			GameObject.resetGameObjectId();
			long t_objectListBegin = t_fstream.Position;
			t_fstream.Position = t_fstream.Position + 4;
			long t_fstreamLastPos = t_fstream.Position;
			long t_objectListSize = t_fstream.Position;
			
			
			
			foreach (LinkedList<GameObject> t_goList in a_save.getGameObjects())
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
						ErrorLogger.getInstance().writeString("Fail to save serialize, FileNotFound: " + t_serializeList.ElementAt<GameObject>(0));
					}
					catch (SerializationException)
					{
						ErrorLogger.getInstance().writeString("Fail to serialize while saving: " + t_serializeList.ElementAt<GameObject>(0));
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

				t_objectListSize += t_fstreamDiffSize;

				t_fstreamLastPos = t_fstream.Length;
				t_fstream.Position = t_fstream.Length;
				t_unikName = new LinkedList<string>();
				index++;
				t_objekts = new LinkedList<LinkedList<GameObject>>();
			} 

			t_fstream.Position = t_objectListBegin;
			byte[] t_objectListSizeInByte = new byte[4];
			t_objectListSizeInByte = BitConverter.GetBytes((int)t_objectListSize);
			t_fstream.Write(t_objectListSizeInByte, 0 , t_objectListSizeInByte.Length);
			t_fstream.Position = t_fstream.Length;

			if (t_stream != null)
			{
				t_stream.Close();
			}
			// Serialize GameObject done!

			try
			{
				//Save Events
				LinkedList<Event> t_events = a_save.getEvents();
				t_stream = new MemoryStream();
				t_bFormatter.Serialize(t_stream, t_events);
				byte[] t_msPos = new byte[4];
				t_msPos = BitConverter.GetBytes((int)t_stream.Position);
				t_fstream.Write(t_msPos, 0, t_msPos.Length);
				t_fstream.Write(t_stream.GetBuffer(), 0, (int)t_stream.Position);
			}
			catch (SerializationException)
			{
				ErrorLogger.getInstance().writeString("While saving, failed to serialized event");
			}
			if (t_stream != null)
			{
				t_stream.Close();
			}
			//Serialize events done

			
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
			LinkedList<Event> t_events = new LinkedList<Event>();
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

				
				t_fstream.Read(t_bytes, 0, t_bytes.Length);
				int t_gameObjectListSize = BitConverter.ToInt32(t_bytes, 0);
				 

				//load GameObjects
				while (true)
				{
					float t_fstreamPos = t_fstream.Position;
					t_bytes = new byte[4];
					t_fstream.Read(t_bytes, 0, t_bytes.Length);
					int t_layerSize = BitConverter.ToInt32(t_bytes, 0);

					while (t_fstream.Position < t_layerSize + t_fstreamPos)
					{
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
							ErrorLogger.getInstance().writeString("Fail to DeSerialize GameObject while loading: " + e);
						}
						catch (OutOfMemoryException e)
						{
							ErrorLogger.getInstance().writeString("Fail to DeSerialize GameObject while loading: " + e);
						}
					}

					t_layerIndex++;
					if (t_fstream.Position >= t_gameObjectListSize)
					{
						break;
					}

				}
				t_loadingLevel.setLevelObjects(t_gameObjectsList);

				
				try
				{
					//load Events
					t_mStream = new MemoryStream();
					t_fstream.Read(t_bytes, 0, t_bytes.Length);
					int t_eventsSize = BitConverter.ToInt32(t_bytes, 0);
					byte[] t_eventsInByte = new byte[t_eventsSize];
					t_fstream.Read(t_eventsInByte, 0, t_eventsSize);
					t_mStream.Write(t_eventsInByte, 0, t_eventsSize);
					t_mStream.Position = 0;
					t_events = (LinkedList<Event>)t_bFormatter.Deserialize(t_mStream);
					if (t_mStream != null)
					{
						t_mStream.Close();
					}
				}
				catch (SerializationException e)
				{
					ErrorLogger.getInstance().writeString("Fail to DeSerialize Event while loading " + e);
				}
				t_loadingLevel.setEvents(t_events);
				

			}
			catch (FileLoadException e)
			{
				ErrorLogger.getInstance().writeString("Could not deserialize level: " + e);
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
