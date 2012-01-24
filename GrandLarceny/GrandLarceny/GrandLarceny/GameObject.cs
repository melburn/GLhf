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

        public GameObject(int a_posX, int a_posY)
        {
            m_myPos = new Position(a_posX, a_posY);
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
