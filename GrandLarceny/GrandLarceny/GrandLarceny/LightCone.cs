﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	public class LightCone : NonMovingObject
	{
		private float m_length;
		private float m_width;

		[NonSerialized]
		private GameObject m_backLight;

		private String m_backLightSprite;
		private float m_backLayer;
	

		[NonSerialized]
		private bool m_collisionIsUpdated;

		public LightCone(GameObject a_parent, string a_sprite, float a_offset, float a_layer, float a_length, float a_width, float a_backLayer, string a_backSprite) :
			base((Position)new PolarCoordinate(a_offset, a_parent.getRotation(), a_parent.getPosition()), a_sprite, a_layer, a_parent.getRotation())
		{
			if (a_length <= 0)
			{
				throw new ArgumentException("length has to be positive. was " + a_length);
			}
			if (a_width <= 0)
			{
				throw new ArgumentException("width has to be positive. was " + a_width);
			}
			m_length = a_length;
			m_width = a_width;
			m_XScale = a_length / m_img.getSize().X;
			m_YScale = a_width / m_img.getSize().Y;
			m_imgOffsetY = -m_rotationPoint.Y * m_YScale;
			m_backLayer = a_backLayer;
			if (a_backSprite != null)
			{
				m_backLightSprite = a_backSprite;
				m_backLight = new GameObject(new CartesianCoordinate(Vector2.Zero, m_position), m_backLightSprite, a_backLayer);
				m_backLight.getPosition().setLocalCartesianCoordinates(-m_backLight.getImg().getSize() / 2f);
			}
		}

		public override void setRotation(float a_rotation)
		{
			if (m_rotate != a_rotation)
			{
				m_position.setSlope(a_rotation);
				m_rotate = a_rotation;
				m_collisionIsUpdated = false;
			}
		}
		public override void addRotation(float a_rotation)
		{
			base.addRotation(a_rotation);
			m_position.setSlope(a_rotation);
			m_collisionIsUpdated = false;
		}
		public override void loadContent()
		{
			base.loadContent();
			m_collisionIsUpdated = false;
			m_rotationPoint.X = 0;
			m_rotationPoint.Y = m_img.getSize().Y / 2;
			m_imgOffsetY = -m_rotationPoint.Y*m_YScale;
			if (m_backLightSprite != null)
			{
				m_backLight = new GameObject(new CartesianCoordinate(Vector2.Zero, m_position), m_backLightSprite, m_backLayer);
				m_backLight.getPosition().setLocalCartesianCoordinates(-m_backLight.getImg().getSize() / 2f);
			}
		}

		public override CollisionShape getHitBox()
		{
			if(!m_collisionIsUpdated)
			{
				m_collisionShape = new CollisionTriangle(getTrianglePointsOffset(), m_position);
				m_collisionIsUpdated = true;
			}
			return m_collisionShape;
		}

		private Vector2[] getTrianglePointsOffset()
		{
			Vector2[] t_ret = new Vector2[3];
			t_ret[0] = Vector2.Zero;
			t_ret[1] = new Vector2((float)(m_length * Math.Cos(m_rotate) + (m_width / 3) * Math.Cos(1.5 * Math.PI + m_rotate)),
				(float)(m_length * Math.Sin(m_rotate) + (m_width / 2) * Math.Sin(1.5 * Math.PI + m_rotate)));
			t_ret[2] = new Vector2((float)(m_length * Math.Cos(m_rotate) + (m_width / 3) * Math.Cos(0.5 * Math.PI + m_rotate)),
				(float)(m_length * Math.Sin(m_rotate) + (m_width / 2) * Math.Sin(0.5 * Math.PI + m_rotate)));

			return t_ret;
		}


		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player && CollisionManager.Collides(this.getHitBox(), a_collid.getHitBox()))
			{
				Player t_player = (Player)a_collid;
				if (t_player.getCurrentState() != Player.State.Hiding)
				{
					t_player.setIsInLight(true);
				}
				else
				{
					if (t_player.getHidingImage().Equals(Player.STANDHIDINGIMAGE))
					{
						if (t_player.isFacingRight() && t_player.getPosition().getGlobalX() + t_player.getHitBox().getOutBox().Width+72 > m_position.getGlobalX())
						{
							t_player.setIsInLight(true);
						}
						else if (!t_player.isFacingRight() && t_player.getPosition().getGlobalX() - 72 < m_position.getGlobalX())
						{
							t_player.setIsInLight(true);
						}
					}
				}
			}
		}

		public override void draw(GameTime a_gameTime)
		{
			base.draw(a_gameTime);
			if (m_backLight != null)
			{
				m_backLight.draw(a_gameTime);
			}
		}
	}
}
