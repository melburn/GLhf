using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny.Events.Triggers
{
	class EntityIsWithinCircle : EventTrigger
	{
		private Vector2 m_position;
		private float m_radie;
		private Entity m_entity;

		public EntityIsWithinCircle(Vector2 a_position, float a_radie, Entity a_entity)
		{
			if (a_position == null || a_entity == null)
			{
				throw new ArgumentNullException();
			}
			if (a_radie <= 0)
			{
				throw new ArgumentException();
			}
			m_position = a_position;
			m_radie = a_radie;
			m_entity = a_entity;
		}
		public override bool isTrue()
		{
			return m_entity.getPosition().getDistanceTo(m_position) <= m_radie;
		}
	}
}
