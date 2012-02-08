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
		private LinkedList<GameObject> m_loadedGameObject;

		public Level()
		{
		}

		public void setLevelObjects(LinkedList<GameObject> a_gameObjects)
		{
			m_loadedGameObject = a_gameObjects;
		}

		public LinkedList<GameObject> getLevelObjects()
		{
			return m_loadedGameObject;
		}

		public Level(SerializationInfo info, StreamingContext context)
		{
			m_loadedGameObject = (LinkedList<GameObject>)info.GetValue("GameObjects", typeof(LinkedList<GameObject>));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("GameObjects", m_loadedGameObject);
		}

	}
}
