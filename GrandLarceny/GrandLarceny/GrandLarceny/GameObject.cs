using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{	
	[Serializable()]
	public class GameObject
	{
		protected bool m_dead = false;
		protected Position m_position;
		[NonSerialized]
		protected ImageManager m_img;

		protected float m_rotate;
		protected float m_layer;
		protected Color m_color;
		protected SpriteEffects m_spriteEffects;
		protected float m_XScale = 1;
		protected float m_YScale = 1;
		protected float m_imgOffsetX = 0;
		protected float m_imgOffsetY = 0;

		private string m_spritePath;

		public GameObject(Vector2 a_posV2, String a_sprite, float a_layer)
		{
			m_position = new CartesianCoordinate(a_posV2);
			//m_img = new ImageManager(a_sprite);
			m_rotate = 0.0f;
			m_layer = a_layer;
			m_color = Color.White;
			m_spriteEffects = SpriteEffects.None;
			m_spritePath = a_sprite;
			loadContent();

		}
		public GameObject(Position a_position, String a_sprite, float a_layer)
		{
			m_position = a_position;
			m_rotate = 0.0f;
			m_layer = a_layer;
			m_color = Color.White;
			m_spriteEffects = SpriteEffects.None;
			m_spritePath = a_sprite;
			loadContent();

		}

		public virtual void loadContent()
		{
			m_img = new ImageManager(m_spritePath);
		}

		public Position getPosition()
		{
			return m_position;
		}

		public Rectangle getBox()
		{
			return new Rectangle((int)getPosition().getGlobalX(), (int)getPosition().getGlobalY(), (int)m_img.getSize().X, (int)m_img.getSize().Y);
		}

		public virtual void update(GameTime a_gameTime)
		{
			m_img.update(a_gameTime);
		}

		public virtual void draw(GameTime a_gameTime)
		{
			Vector2 t_imgPosition;
			t_imgPosition.X = m_position.getGlobalX() + m_imgOffsetX;
			t_imgPosition.Y = m_position.getGlobalY() + m_imgOffsetY;

			m_img.draw(t_imgPosition, m_rotate, m_color, m_spriteEffects, m_layer, m_XScale, m_YScale);
		}
		public bool isDead()
		{
			return m_dead;
		}
		public void setLayer(int a_layer)
		{
			m_layer = a_layer;
		}


		internal virtual void collisionCheck(List<Entity> a_collisionList)
		{
		}
		
		/*public Vector2 getTopLeftPoint()
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
		}*/
		public void setColor(Color a_color)
		{
			m_color = a_color;
		}

		public ImageManager getImg()
		{
			return m_img;
		}
		public float getRotation()
		{
			return m_rotate;
		}
	}
}
