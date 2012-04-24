using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	public class Environment : GameObject
	{
		private Boolean m_explored;

		public Environment(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			m_explored = false;
		}

		public bool isExplored()
		{
			return m_explored;
		}
		public void setExplored(Boolean a_explored)
		{
			m_explored = a_explored;
		}
		public bool collidesWith(GameObject a_gameObject)
		{
			return a_gameObject.getBox().Intersects(getBox());
		}
	}
}
