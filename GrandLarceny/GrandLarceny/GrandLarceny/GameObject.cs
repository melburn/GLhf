using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class GameObject
	{
		private bool m_dead = false;
        private Position m_myPos;

		public GameObject(Vector2 a_posV2)
		{
			m_myPos = new CartesianCoordinate(a_posV2);
		}

		public virtual void update(GameTime a_gameTime)
		{
			
		}

		public virtual void draw(GameTime a_gameTime)
		{

		}
		public bool isDead()
		{
			return m_dead;
		}
	}
}
