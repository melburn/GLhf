using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	class GameObject
	{
        protected bool m_dead = false;
        protected Position m_position;
        protected ImageManager m_img;

        protected float m_rotate;
        protected int m_layer;
        protected Color m_color;
        protected SpriteEffects m_spriteEffects;

		public GameObject(Vector2 a_posV2, String a_sprite)
		{
			m_position = new CartesianCoordinate(a_posV2);
            m_img = new ImageManager(a_sprite);
			m_rotate = 0.0f;
			m_layer = 0;
			m_color = Color.White;
			m_spriteEffects = SpriteEffects.None;
		}

		public Position getPosition()
		{
			return m_position;
		}

		public Rectangle getBox()
		{
			return new Rectangle((int)getTopLeftPoint().X, (int)getTopLeftPoint().Y, (int)m_img.getSize().X, (int)m_img.getSize().Y);
		}

		public virtual void update(GameTime a_gameTime)
		{
            m_img.update(a_gameTime);
		}

		public virtual void draw(GameTime a_gameTime)
		{
            m_img.draw(m_position, m_rotate, m_color, m_spriteEffects, m_layer);
		}
		public bool isDead()
		{
			return m_dead;
		}
		public void setLayer(int a_layer) {
			m_layer = a_layer;
		}

		internal virtual void collisionCheck(List<Entity> t_secondGameObject)
		{
		}
		public ImageManager getImg()
		{
			return m_img;
		}
		public Vector2 getTopLeftPoint()
		{
			return m_position.getGlobalCartesianCoordinates() - (m_img.getSize() / 2);
		}
		public void setTopLeftPoint(Vector2 a_position)
		{
			m_position.setCartesianCoordinates(a_position + (m_img.getSize() / 2));
		}
		public void setLeftPoint(float a_x)
		{
			m_position.setX(a_x + (m_img.getSize().X / 2));
		}
		public void setTopPoint(float a_y)
		{
			m_position.setY(a_y + (m_img.getSize().Y / 2));
		}

		public float getLeftPoint()
		{
			return m_position.getX() - (m_img.getSize().X / 2);
		}
		public float getRightPoint()
		{
			return m_position.getX() + (m_img.getSize().X / 2);
		}
		public float getTopPoint()
		{
			return m_position.getY() - (m_img.getSize().Y / 2);
		}
		public float getBottomPoint()
		{
			return m_position.getY() + (m_img.getSize().Y / 2);
		}
	}
}
