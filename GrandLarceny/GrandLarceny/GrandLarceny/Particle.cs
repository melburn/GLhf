using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class Particle : GameObject
	{
		public Particle(Vector2 a_position, String a_sprite, float a_animationSpeed, float a_layer)
			:base(new CartesianCoordinate(a_position), a_sprite, a_layer)
		{
			m_img.setAnimationSpeed(a_animationSpeed);
			m_img.setLooping(false);
		}

		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			if (m_img.isStopped())
			{
				m_dead = true;
			}
		}
	}
}
