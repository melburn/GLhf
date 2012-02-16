using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class Entity : GameObject
	{
		//pixel per sekund
		protected Vector2 m_speed;

		//pixel per sekund per sekund
		protected float m_gravity = 0f;

		protected Vector2 m_lastPosition;
		[NonSerialized]
		protected CollisionShape m_collisionShape;
		
		public Entity(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
		}

		public Entity(Position a_position, String a_sprite, float a_layer)
			: base(a_position, a_sprite, a_layer)
		{
		}

		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionRectangle(0, 0, m_img.getSize().X, m_img.getSize().Y, m_position);
		}

		public override void update(GameTime a_gameTime)
		{
			m_lastPosition = m_position.getGlobalCartesianCoordinates();
			base.update(a_gameTime);

			float t_deltaTime = ((float)(a_gameTime.ElapsedGameTime.Milliseconds)) / 1000.0f;
			m_speed.Y += m_gravity * t_deltaTime;
			m_position.plusWith(m_speed * t_deltaTime);
		}

		public CollisionShape getHitBox()
		{
			return m_collisionShape;
		}
		
		public override void draw(GameTime a_gameTime)
		{
			base.draw(a_gameTime);
		}
		internal Vector2 getLastPosition()
		{
			return m_lastPosition;
		}
		internal float getHorizontalSpeed()
		{
			return m_speed.X;
		}

	}
}
