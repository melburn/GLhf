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
			m_objectId = ++s_lastId;
		}

		public virtual void linkObject()
		{

		}

		public void flip()
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
			Vector2 t_imgPosition;
			t_imgPosition.X = m_position.getGlobalX() + m_imgOffsetX;
			t_imgPosition.Y = m_position.getGlobalY() + m_imgOffsetY;

			m_img.draw(t_imgPosition, m_rotate, m_rotationPoint, m_color, m_spriteEffects, m_layer, m_XScale, m_YScale);
		}
		public bool isDead()
		{
			return m_dead;
		}
		public virtual void kill() {
			m_dead = true;
		}
		public void setLayer(float a_layer)
		{
			m_layer = a_layer;
		}
		
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
		public virtual void addRotation(float a_rotation)
		{
			m_rotate = (m_rotate + a_rotation) % ((float)Math.PI * 2);
		}

		public float getLayer() {
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
		public void changePositionType()
		{
			if (m_position is CartesianCoordinate)
				m_position = new PolarCoordinate(m_position.getLocalPolarCoordinates(), m_position.getParentPosition());
			else
				m_position = new CartesianCoordinate(m_position.getLocalCartesianCoordinates(), m_position.getParentPosition());
		}
	}
}
