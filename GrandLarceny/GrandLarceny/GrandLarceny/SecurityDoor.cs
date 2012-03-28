using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class SecurityDoor : NonMovingObject
	{
		private Boolean m_open;
		private Boolean m_closeWhenOpen;
		public SecurityDoor(String a_sprite, Vector2 a_position, int a_layer)
			:base(a_position, a_sprite, a_layer)
		{
			m_img.setLooping(false);
			m_img.stop();
		}
		public void open()
		{
			m_open = true;
		}
		public void close()
		{
			m_open = false;
		}
	}
}
