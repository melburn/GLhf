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

		public FileStream getFileToStream(string a_fileName, bool a_willCreate)
		{
			FileStream t_file;
			if (a_willCreate)
			{
				t_file = File.Open("Content//Levels//" + a_fileName, FileMode.Create);
			}
			else
			{
				t_file = File.Open("Content//Levels//" + a_fileName, FileMode.Open);
			}

			return t_file;

		}

		public void SaveLevel(Stream a_stream, Level a_save)
		{
			GameObject.resetGameObjectId();
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
			//FileStream t_fstream = null;

			//t_fstream = File.Open("Content//Levels//" + a_fileName, FileMode.Create);
			BinaryFormatter t_bFormatter = new BinaryFormatter();
			
			LinkedList<string> t_unikName = new LinkedList<string>();
			LinkedList<LinkedList<GameObject>> t_objekts = new LinkedList<LinkedList<GameObject>>();

			int index = 0;
			
			long t_fstreamDiffSize = 0;
			long t_objectListBegin = a_stream.Position;
			a_stream.Position = a_stream.Position + 4;
			long t_fstreamLastPos = a_stream.Position;
			long t_objectListSize = a_stream.Position;
			
			foreach (LinkedList<GameObject> t_goList in a_save.getGameObjects())
			{
				a_stream.Position = a_stream.Position + 4;
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
						a_stream.Write(t_msPos, 0, t_msPos.Length);
						a_stream.Write(t_stream.GetBuffer(), 0, (int)t_stream.Position);

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
				t_fstreamDiffSize = a_stream.Position - t_fstreamLastPos;
				t_layerSize = BitConverter.GetBytes((int)t_fstreamDiffSize);
				a_stream.Position = a_stream.Position - t_fstreamDiffSize;
				a_stream.Write(t_layerSize, 0, t_layerSize.Length);

				t_objectListSize += t_fstreamDiffSize;

				t_fstreamLastPos = a_stream.Length;
				a_stream.Position = a_stream.Length;
				t_unikName = new LinkedList<string>();
				index++;
				t_objekts = new LinkedList<LinkedList<GameObject>>();
			}

			a_stream.Position = t_objectListBegin;
			byte[] t_objectListSizeInByte = new byte[4];
			t_objectListSizeInByte = BitConverter.GetBytes((int)t_objectListSize);
			a_stream.Write(t_objectListSizeInByte, 0, t_objectListSizeInByte.Length);
			a_stream.Position = a_stream.Length;

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
				a_stream.Write(t_msPos, 0, t_msPos.Length);
				a_stream.Write(t_stream.GetBuffer(), 0, (int)t_stream.Position);
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

			if (a_stream != null && a_stream is FileStream)
			{
				a_stream.Close();
			}
		}

		public Level loadLevel(Stream a_stream)
		{
			Level t_loadingLevel = new Level();
			//FileStream t_fstream = null;
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
				//t_fstream = File.Open("Content//Levels//" + a_fileName, FileMode.Open);
					
				BinaryFormatter t_bFormatter = new BinaryFormatter();

				a_stream.Read(t_bytes, 0, t_bytes.Length);
				int t_gameObjectListSize = BitConverter.ToInt32(t_bytes, 0);
				
				//load GameObjects
				while (true)
				{
					float t_fstreamPos = a_stream.Position;
					t_bytes = new byte[4];
					a_stream.Read(t_bytes, 0, t_bytes.Length);
					int t_layerSize = BitConverter.ToInt32(t_bytes, 0);

					while (a_stream.Position < t_layerSize + t_fstreamPos)
					{
						t_bytes = new byte[4];
						a_stream.Read(t_bytes, 0, t_bytes.Length);
						int t_objectListSize = BitConverter.ToInt32(t_bytes, 0);

						try
						{
							t_mStream = new MemoryStream();
							byte[] t_objectListInByte = new byte[t_objectListSize];
							a_stream.Read(t_objectListInByte, 0, t_objectListSize);
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

					if (a_stream.Position >= t_gameObjectListSize)
					{
						break;
					}
				}
				t_loadingLevel.setLevelObjects(t_gameObjectsList);

				try
				{
					//load Events
					t_mStream = new MemoryStream();
					a_stream.Read(t_bytes, 0, t_bytes.Length);
					int t_eventsSize = BitConverter.ToInt32(t_bytes, 0);
					byte[] t_eventsInByte = new byte[t_eventsSize];
					a_stream.Read(t_eventsInByte, 0, t_eventsSize);
					t_mStream.Write(t_eventsInByte, 0, t_eventsSize);
					t_mStream.Position = 0;
					if (t_mStream.Length != 0) 
					{
						t_events = (LinkedList<Event>)t_bFormatter.Deserialize(t_mStream);
					}
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

			if (a_stream != null && a_stream is FileStream)
			{
				a_stream.Close();
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

		public void saveGame(Stream a_saveStream, Progress a_progress)
		{
			//FileStream t_fstream;
			BinaryFormatter t_bFormatter = new BinaryFormatter();
			try
			{
				//t_fstream = File.Open("Content//Levels//" + a_saveFileName, FileMode.Create);
				try
				{
					t_bFormatter.Serialize(a_saveStream, a_progress);
				}
				catch (SerializationException e)
				{
					ErrorLogger.getInstance().writeString("Could not serialize progress, serializeing failed: " + e);
				}
				if (a_saveStream != null && a_saveStream is FileStream)
				{
					a_saveStream.Close();
				}
			}
			catch (FileNotFoundException e)
			{
				ErrorLogger.getInstance().writeString("Could not serialize progress, file not found: " + e);
			}
			catch (FileLoadException e)
			{
				ErrorLogger.getInstance().writeString("Could not serialize progress, file fail to load: " + e);
			}
		}

		public Progress loadProgress(Stream a_saveStream)
		{
			Progress t_progg = null;
			//FileStream t_fstream;
			BinaryFormatter t_bFormatter = new BinaryFormatter();
			try
			{
				//t_fstream = File.Open("Content//Levels//" + a_saveFileName, FileMode.Open);
				try
				{
					t_progg = (Progress)t_bFormatter.Deserialize(a_saveStream);
				}
				catch (SerializationException e)
				{
					ErrorLogger.getInstance().writeString("Could not serialize progress, serializeing failed: " + e);
				}
				if (a_saveStream != null && a_saveStream is FileStream)
				{
					a_saveStream.Close();
				}
			}
			catch (FileNotFoundException e)
			{
				ErrorLogger.getInstance().writeString("Could not serialize progress, file not found: " + e);
			}
			catch (FileLoadException e)
			{
				ErrorLogger.getInstance().writeString("Could not serialize progress, file fail to load: " + e);
			}

			return t_progg;
		}
	}
}
