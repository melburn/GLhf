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

		public LinkedList<GameObject> LoadedGameObject
		{
			set { m_loadedGameObject = value; }
			get { return m_loadedGameObject; }
		}

		public Level()
		{
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
