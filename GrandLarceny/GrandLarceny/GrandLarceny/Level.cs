using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GrandLarceny
{
	[Serializable]
	class Level : ISerializable
	{
		private LinkedList<GameObject>[] m_loadedList;

		public Level()
		{
			m_loadedList = new LinkedList<GameObject>[5];
		}

		public void setLevelObjects(LinkedList<GameObject>[] a_gameObjects)
		{
			m_loadedList = a_gameObjects;
		}

		public LinkedList<GameObject>[] getLevelLists()
		{
			return m_loadedList;
		}

		public Level(SerializationInfo info, StreamingContext context)
		{
			m_loadedList = (LinkedList<GameObject>[])info.GetValue("GameObjects", typeof(LinkedList<GameObject>[]));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("GameObjects", m_loadedList);
		}

	}
}
