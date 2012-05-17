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
		float m_animationSpeed;

		public Particle(Vector2 a_position, String a_sprite, float a_animationSpeed, float a_layer)
			:base(new CartesianCoordinate(a_position), a_sprite, a_layer)
		{
			m_animationSpeed = a_animationSpeed;
			loadContent();
		}

		public override void loadContent()
		{
			base.loadContent();
			m_img.setAnimationSpeed(m_animationSpeed);
			m_img.setLooping(false);
		}

		public void addTimer(float a_timer)
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
