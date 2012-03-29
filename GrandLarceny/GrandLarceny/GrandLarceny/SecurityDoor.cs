using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	[Serializable()]
	class SecurityDoor : NonMovingObject
	{
		private Boolean m_open;
		private Boolean m_closeWhenOpen;

		private float m_openSpeed;
		private float m_closeSpeed;

		private float m_lastCheckedHitBox;
		public SecurityDoor(Vector2 a_position, String a_sprite, float a_layer)
			:base(a_position, a_sprite, a_layer)
		{
			m_lastCheckedHitBox = -1;
			m_closeWhenOpen = true;
			m_open = false;
			m_openSpeed = 10;
			m_closeSpeed = 10;
		}

		public override void loadContent()
		{
			base.loadContent();

			m_img.setLooping(false);
			m_img.stop();
		}
		public void open()
		{
			if (!m_open)
			{
				m_open = true;
				m_img.setAnimationSpeed(-m_closeSpeed);
				m_img.run();
			}
		}
		public void close()
		{
			if (m_open)
			{
				m_open = false;
				m_img.setAnimationSpeed(m_openSpeed);
				m_img.run();
			}
		}
		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			if (m_img.isStopped() && m_closeWhenOpen && m_img.getSubImageIndex() != 0)
			{
				close();
			}
		}
		public override CollisionShape getHitBox()
		{
			if (m_lastCheckedHitBox != m_img.getSubImageIndex())
			{
				m_collisionShape = new CollisionRectangle(0, 0, m_img.getSize().X, m_img.getSize().Y * (((float)m_img.getLength()) / m_img.getSubImageIndex()), m_position);
				m_lastCheckedHitBox = m_img.getSubImageIndex();
			}
			return m_collisionShape;
		}
		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player)
			{
				Player t_player = (Player)a_collid;
				Rectangle t_playerOutBox = t_player.getHitBox().getOutBox();
				if (CollisionManager.Collides(this.getHitBox(), a_collid.getHitBox()))
				{

					//Colliding with ze left Wall wall
					if (t_player.getLastPosition().X + 1 >= getLastPosition().X + getHitBox().getOutBox().Width)
					{
						t_player.setNextPositionX(getPosition().getGlobalX() + getHitBox().getOutBox().Width);
						t_player.setSpeedX(0);

					}
					//Colliding with ze right Wall wall
					else if (t_player.getLastPosition().X + t_playerOutBox.Width - 1 <= getLastPosition().X)
					{
						t_player.setNextPositionX(getPosition().getGlobalX() - t_playerOutBox.Width);
						t_player.setSpeedX(0);

					}
					//Colliding with ze zeeling
					else if (t_player.getLastPosition().Y >= getLastPosition().Y + getHitBox().getOutBox().Height)
					{
						t_player.setNextPositionY(getPosition().getGlobalY() + getHitBox().getOutBox().Height);
						t_player.setSpeedY(0);
					}
				}

			}
		}

		public void setCloseWhenOpen(bool a_value)
		{
			m_closeWhenOpen = a_value;
		}

		public void setOpeningSpeed(float a_openSpeed)
		{
			m_openSpeed = a_openSpeed;
			if (m_open)
			{
				m_img.setAnimationSpeed(m_openSpeed);
			}
		}

		public void setClosingSpeed(float a_closeSpeed)
		{
			m_closeSpeed = a_closeSpeed;
			if (!m_open)
			{
				m_img.setAnimationSpeed(-m_closeSpeed);
			}
		}
	}
}
