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
		protected float m_gravity = 0.0f;

		protected Vector2 m_lastPosition;
		protected Vector2 m_nextPosition;
		[NonSerialized]
		protected CollisionShape m_collisionShape;
		
		public Entity(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
		}

		public Entity(Position a_position, String a_sprite, float a_layer, float a_rotation = 0)
			: base(a_position, a_sprite, a_layer, a_rotation)
		{
		}

		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionRectangle(0, 0, m_img.getSize().X, m_img.getSize().Y, m_position);
		}

		public virtual CollisionShape getImageBox()
		{
			return new CollisionRectangle(0, 0, m_img.getSize().X, m_img.getSize().Y, m_position);
		}

		public override void update(GameTime a_gameTime)
		{
			//m_lastPosition = m_position.getGlobalCartesianCoordinates();
			base.update(a_gameTime);
			
			float t_deltaTime = ((float)(a_gameTime.ElapsedGameTime.Milliseconds)) / 1000.0f;
			
			if (Game.getInstance().m_camera.isInCamera(this)) {
				
				m_speed.Y += m_gravity * t_deltaTime;
			}
				m_position.plusWith(m_speed * t_deltaTime);
				m_nextPosition = m_position.getLocalCartesianCoordinates();
			
		}

		public virtual CollisionShape getHitBox()
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

		internal virtual void updateCollisionWith(Entity a_collid)
		{

		}

		public void setSpeedX(float a_speedX)
		{
			m_speed.X = a_speedX;
		}
		public void setSpeedY(float a_speedY)
		{
			m_speed.Y = a_speedY;
		}
		public Vector2 getSpeed()
		{
			return m_speed;
		}

		public virtual bool isTransparent()
		{
			return true;
		}

		public void setNextPositionX(float a_x)
		{
			m_nextPosition.X = a_x;
		}

		public void setNextPositionY(float a_y)
		{
			m_nextPosition.Y = a_y;
		}

		public void setNextPosition(Vector2 a_newPosition)
		{
			m_nextPosition = a_newPosition;
		}

		public Vector2 getNextPosition()
		{
			return m_nextPosition;
		}

		public void updatePosition()
		{
			m_position.setGlobalCartesianCoordinates(m_nextPosition);
		}
		public override void changePositionType()
		{
			base.changePositionType();
			m_collisionShape.setPosition(m_position);
		}

		public void setGravity(float a_gravi)
		{
			m_gravity = a_gravi;
		}
	}
}
