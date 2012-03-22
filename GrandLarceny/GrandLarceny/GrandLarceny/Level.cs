using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GrandLarceny.Events;

namespace GrandLarceny
{
	[Serializable]
	class Level : ISerializable
	{
		private LinkedList<GameObject>[] m_loadedList;
		private LinkedList<Event> m_events;

		public Level()
		{
			m_loadedList = new LinkedList<GameObject>[5];
			m_loadedList[0] = new LinkedList<GameObject>();
			m_loadedList[1] = new LinkedList<GameObject>();
			m_loadedList[2] = new LinkedList<GameObject>();
			m_loadedList[3] = new LinkedList<GameObject>();
			m_loadedList[4] = new LinkedList<GameObject>();
			m_events = new LinkedList<Event>();
		}

		public void setLevelObjects(LinkedList<GameObject>[] a_gameObjects)
		{
			m_loadedList = a_gameObjects;
		}

		public void setEvents(LinkedList<Event> a_events)
		{
			m_events = a_events;
		}

		public LinkedList<GameObject>[] getGameObjects()
		{
			return m_loadedList;
		}

		public LinkedList<Event> getEvents()
		{
			return m_events;
		}

		public Level(SerializationInfo info, StreamingContext context)
		{
			m_loadedList = (LinkedList<GameObject>[])info.GetValue("GameObjects", typeof(LinkedList<GameObject>[]));
			m_events = (LinkedList<Event>)info.GetValue("Events", typeof(LinkedList<Event>));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			
			info.AddValue("GameObjects", m_loadedList);
			info.AddValue("Events", m_events);
		}

	}
}
