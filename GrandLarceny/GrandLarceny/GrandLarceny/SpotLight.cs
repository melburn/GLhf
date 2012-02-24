using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class SpotLight : MovingObject
	{
		LightCone m_light;
		public SpotLight(Vector2 a_position, string a_sprite, float a_layer, float a_rotation, bool a_lit) :
			base(a_position, a_sprite, a_layer)
		{
			m_rotate = a_rotation;
			m_rotationPoint.Y = m_img.getSize().Y / 2;
			if (a_lit)
			{
				m_light = new LightCone(this, "Images//LightCone//Ljus", a_layer + 0.001f, 300f, 200f);

				(Game.getInstance().getState()).addObject(m_light);
			}
		}
		public override void loadContent()
		{
			base.loadContent();
		}

		public LightCone getLightCone()
		{
			return m_light;
		}
		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			if (m_light != null)
			{
				m_light.setRotation(m_rotate);
			}
		}

		public void setLit(bool a_switchedOn)
		{
			if (a_switchedOn)
			{
				if (m_light == null)
				{
					m_light = new LightCone(this, "Images//LightCone//Ljus", m_layer, 300f, 200f);
					Game.getInstance().getState().addOrRemoveObject(m_light);
				}
			}
			else if(m_light != null)
			{
				Game.getInstance().getState().addOrRemoveObject(m_light);
				m_light = null;
			}
		}
	}
}
