using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	[Serializable()]
	public class GameObject
	{
		private static int s_lastId = 1;
		private int m_objectId;
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
		protected Vector2 m_rotationPoint = Vector2.Zero;
		protected Vector2 m_changePositionAfterDraw = Vector2.Zero;
		protected Boolean m_visible = true;

		private string m_spritePath;

		public GameObject(Vector2 a_posV2, string a_sprite, float a_layer)
		{
			m_objectId = ++s_lastId;
			m_position = new CartesianCoordinate(a_posV2);
			m_rotate = 0.0f;
			m_layer = a_layer;
			m_spritePath = a_sprite;
			loadContent();
		}
		public GameObject(Position a_position, string a_sprite, float a_layer, float a_rotation = 0)
		{
			m_objectId = ++s_lastId;
			m_rotate = a_rotation;
			m_position = a_position;
			m_layer = a_layer;
			m_spritePath = a_sprite;
			loadContent();
		}

		public virtual void saveObject()
		{
			m_spritePath = m_img.getImagePath();
			m_objectId = ++s_lastId;
		}

		public virtual void linkObject()
		{ }

		public virtual void flip()
		{
			if (m_spriteEffects == SpriteEffects.None)
				m_spriteEffects = SpriteEffects.FlipHorizontally;
			else
				m_spriteEffects = SpriteEffects.None;
		}

		public virtual void loadContent()
		{
			m_color = Color.White;
			m_img = new ImageManager(m_spritePath);
			m_rotationPoint = m_img.getSize() / 2;
			m_visible = true;
		}

		public Position getPosition()
		{
			return m_position;
		}

		public virtual Rectangle getBox()
		{
			return new Rectangle((int)getPosition().getGlobalX(), (int)getPosition().getGlobalY(), (int)m_img.getSize().X, (int)m_img.getSize().Y);
		}

		public virtual void update(GameTime a_gameTime)
		{
			m_img.update(a_gameTime);
		}

		public virtual void draw(GameTime a_gameTime)
		{
			if (m_visible)
			{
				Vector2 t_imgPosition = m_position.getFlooredGlobalCartesian() + new Vector2(m_imgOffsetX, m_imgOffsetY);
				///*t_imgPosition.X = m_position.getGlobalX() + m_imgOffsetX;
				//t_imgPosition.Y = m_position.getGlobalY() + m_imgOffsetY;

				m_img.draw(t_imgPosition, m_rotate, m_rotationPoint, m_color, m_spriteEffects, m_layer, m_XScale, m_YScale);

				if (m_changePositionAfterDraw != Vector2.Zero)
				{
					if (this is Player)
						Game.getInstance().m_camera.getPosition().plusXWith(-m_changePositionAfterDraw.X);
					m_position.plusWith(m_changePositionAfterDraw);
					m_changePositionAfterDraw = Vector2.Zero;
				}
			}
		}
		public bool isDead()
		{
			return m_dead;
		}
		public virtual void kill()
		{
			m_dead = true;
		}
		public void setLayer(float a_layer)
		{
			m_layer = a_layer;
		}

		public virtual void setColor(Color a_color)
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
		public virtual void setRotation(float a_rotation)
		{
			m_rotate = a_rotation % ((float)Math.PI * 2);
		}
		public virtual void addRotation(float a_rotation)
		{
			m_rotate = (m_rotate + a_rotation) % ((float)Math.PI * 2);
		}

		public float getLayer()
		{
			return m_layer;
		}

		public int getId()
		{
			return m_objectId;
		}

		public static void resetGameObjectId()
		{
			s_lastId = 1;
		}
		public void changePositionToPolar()
		{
			if (m_position is CartesianCoordinate)
				changePositionType();
		}
		public void changePositionToCartesian()
		{
			if (m_position is PolarCoordinate)
				changePositionType();
		}
		public virtual void changePositionType()
		{
			if (m_position is CartesianCoordinate)
				m_position = new PolarCoordinate(m_position.getLocalPolar(), m_position.getParentPosition());
			else
				m_position = new CartesianCoordinate(m_position.getLocalCartesian(), m_position.getParentPosition());
		}
		public void setImageOffset(Vector2 a_offset)
		{
			m_imgOffsetX = a_offset.X;
			m_imgOffsetY = a_offset.Y;
		}
		public void addPositionXAfterDraw(float a_addX)
		{
			m_changePositionAfterDraw.X = a_addX;
		}

		public void setVisible(bool a_visible)
		{
			m_visible = a_visible;
		}

		public override string ToString() {
			return m_objectId + ": " + m_position.getGlobalCartesian().ToString() + ":" + m_layer;
		}

		public virtual Vector2 getCenterPoint()
		{
			return m_position.getGlobalCartesian() + (m_img.getSize() / 2);
		}
	}
}
