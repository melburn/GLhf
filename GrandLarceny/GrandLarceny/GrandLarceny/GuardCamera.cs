﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class GuardCamera : NPE
	{
		[NonSerialized]
		private LightCone m_light;
		private int m_lightLink;
		private float m_leftRotation;
		private float m_rightRotation;
		private float m_rotationSpeed;
		private const float ROTATIONSPEED = 0.1f;
		public GuardCamera(Vector2 a_position, String a_sprite, float a_layer, float a_rotation, float a_leftRotation, float a_rightRotation)
			:base(a_position,a_sprite,a_layer)
		{
			if (a_rotation < 0 || a_rotation > 2 * Math.PI ||
				a_leftRotation < 0 || a_leftRotation > 2 * Math.PI ||
				a_rightRotation < 0 || a_rightRotation > 2 * Math.PI)
			{
				throw new ArgumentException("rotations has to be between 0 and 2PI");
			}
			else
			{
				m_aiState = AIStatepatroling.getInstance();
				m_rotate = a_rotation;
				m_leftRotation = a_leftRotation;
				m_rightRotation = a_rightRotation;
			}
		}
		public override void loadContent()
		{
			base.loadContent();
			if (m_lightLink > 0)
			{
				m_light = (LightCone)Game.getInstance().getState().getObjectById(m_lightLink);
			}
			else
			{
				m_light = new LightCone(this, "Images//LightCone//Ljus", m_layer + 0.001f, 300f, 300f);
				m_lightLink = m_light.getId();
			}
		}
		public bool canSeePlayer()
		{
			return m_light != null && CollisionManager.Collides(m_light.getCollisionShape(), Game.getInstance().getState().getPlayer().getCollisionShape());
		}

		public float getLeftRotationPoint()
		{
			return m_leftRotation;
		}
		public float getRightRotationPoint()
		{
			return m_rightRotation;
		}

		internal void rotateRight()
		{
			m_rotationSpeed = ROTATIONSPEED;
		}

		internal void rotateLeft()
		{
			m_rotationSpeed = -ROTATIONSPEED;
		}

		public float getRotationSpeed()
		{
			return m_rotationSpeed;
		}

		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			m_rotate += m_rotationSpeed * (a_gameTime.ElapsedGameTime.Milliseconds / 1000f);
			m_light.setRotation(m_rotate);
		}
	}
}