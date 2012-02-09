using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class SpotLight : Entity
	{
		Boolean m_lit;
		LightCone m_light;
		public SpotLight(Vector2 a_position, string a_sprite, float a_layer, float a_rotation, bool a_lit) :
			base(a_position, a_sprite, a_layer)
		{
			m_rotate = a_rotation;
			m_lit = a_lit;
			if (m_lit)
			{
				m_light = new LightCone(this, "LAMPA",a_layer , 100f, 50f);
				((GameState)(Game.getInstance().getState())).addObject(m_light);
			}
		}
	}
}
