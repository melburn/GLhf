﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	public class LightCone : MovingObject
	{
		private float m_length;
		private float m_width;
		private GameObject m_parent;
		public LightCone(GameObject a_parent, string a_sprite, float a_layer, float a_length, float a_width) :
			base((Position)new CartesianCoordinate(Vector2.Zero,a_parent.getPosition()), a_sprite, a_layer)
		{
			if (a_length <= 0)
			{
				throw new ArgumentException("length has to be positive. was " + a_length);
			}
			if (a_width <= 0)
			{
				throw new ArgumentException("width has to be positive. was " + a_width);
			}
			m_parent = a_parent;
			m_length = a_length;
			m_width = a_width;
			m_XScale = a_length / 500;
			m_YScale = a_width / 500;
			m_rotate = a_parent.getRotation();
			m_rotationPoint.Y = m_img.getSize().Y / 2;
		}
		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			if (m_parent == null || m_parent.isDead())
			{
				m_dead = true;
			}
		}
		public void setRotation(float a_rotation)
		{
			if (m_rotate != a_rotation)
			{
				m_rotate = a_rotation;
				m_collisionShape = new CollisionTriangle(getTrianglePointsOffset(), m_position);
			}
		}
		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionTriangle(getTrianglePointsOffset(), m_position);
		}

		private Vector2[] getTrianglePointsOffset()
		{
			Vector2[] t_ret = new Vector2[3];
			t_ret[0] = Vector2.Zero;
			t_ret[1] = new Vector2((float)(0.9 * m_length * Math.Cos(m_rotate) + (m_width / 3) * Math.Cos(1.5 * Math.PI + m_rotate)),
				(float)(0.9 * m_length * Math.Sin(m_rotate) + (m_width / 2) * Math.Sin(1.5 * Math.PI + m_rotate)));
			t_ret[2] = new Vector2((float)(0.9 * m_length * Math.Cos(m_rotate) + (m_width / 3) * Math.Cos(0.5 * Math.PI + m_rotate)),
				(float)(0.9 * m_length * Math.Sin(m_rotate) + (m_width / 2) * Math.Sin(0.5 * Math.PI + m_rotate)));

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
					if(t_player.getHidingImage().Equals(Player.STANDHIDINGIMAGE))
					{
						if (t_player.isFacingRight() && t_player.getPosition().getGlobalX() > m_parent.getPosition().getGlobalX())
						{
							t_player.setIsInLight(true);
						}
						else if(!t_player.isFacingRight() && t_player.getPosition().getGlobalX() < m_parent.getPosition().getGlobalX())
						{
							t_player.setIsInLight(true);
						}
					}
				}

			}
			

		}
	}
}
