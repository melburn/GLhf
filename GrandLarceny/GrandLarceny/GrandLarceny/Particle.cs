using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public class Particle : GameObject
	{
		float m_timer = 0;

		public Particle(Vector2 a_position, String a_sprite, float a_animationSpeed, float a_layer)
			:base(new CartesianCoordinate(a_position), a_sprite, a_layer)
		{
			m_img.setAnimationSpeed(a_animationSpeed);
			m_img.setLooping(false);
		}
		public Particle(Position a_position, String a_sprite, float a_animationSpeed, float a_layer)
			: base(a_position, a_sprite, a_layer)
		{
			m_img.setAnimationSpeed(a_animationSpeed);
			m_img.setLooping(false);
		}

		public void setTimer(float a_timer)
		{
			m_timer = a_timer;
		}

		public override void update(GameTime a_gameTime)
		{
			
			base.update(a_gameTime);
			if (m_img.isStopped() && m_timer <= a_gameTime.TotalGameTime.TotalMilliseconds)
			{
				m_dead = true;
			}
		}
	}
}
