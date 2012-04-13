﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GrandLarceny.AI;

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
		private float m_turnTimer;
		private float m_turnStopTime = 0.5f;
		private float m_rotationSpeed;
		private Entity m_chaseTarget;
		private const float ROTATIONSPEED = 0.7f;
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
		public override void linkObject()
		{
			base.linkObject();
			if(m_light != null)
			{
				m_lightLink = m_light.getId();
			}
		}
		public override void loadContent()
		{
			base.loadContent();
			m_rotationPoint = new Vector2(0, m_img.getSize().Y / 2);
			m_imgOffsetY = -m_rotationPoint.Y * m_YScale;
			if (m_lightLink > 0)
			{
				m_light = (LightCone)Game.getInstance().getState().getObjectById(m_lightLink);
				m_light.getPosition().setParentPosition(m_position);
			}
			else
			{
				m_light = new LightCone(this, "Images//LightCone//Ljus", m_layer + 0.001f, 300f, 100f);
				m_lightLink = m_light.getId();
				(Game.getInstance().getState()).addObject(m_light);
			}
			m_light.setRotation(m_rotate);
		}
		public bool canSeePlayer()
		{
			Player t_player = Game.getInstance().getState().getPlayer();
			if (t_player == null) {
				return false;
			}
			return m_light != null &&
				t_player.isInLight() &&
				t_player.getCurrentState() != Player.State.Hiding &&
				CollisionManager.Collides(m_light.getHitBox(), Game.getInstance().getState().getPlayer().getHitBox());
		}

		public float getLeftRotationPoint()
		{
			return m_leftRotation;
		}
		public float getRightRotationPoint()
		{
			return m_rightRotation;
		}

		internal void rotateClockW()
		{
			m_rotationSpeed = ROTATIONSPEED;
		}

		internal void rotateCounter()
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
			m_rotate = (m_rotate + (m_rotationSpeed * (a_gameTime.ElapsedGameTime.Milliseconds / 1000f))) % (float)(Math.PI * 2);
			m_light.setRotation(m_rotate);
			m_turnTimer -= (a_gameTime.ElapsedGameTime.Milliseconds / 1000f);
		}

		internal Entity getTarget()
		{
			return m_chaseTarget;
		}

		internal void chasePlayer()
		{
			Player t_player = Game.getInstance().getState().getPlayer();
			m_chaseTarget = t_player;
			
		}

		public override void kill()
		{
			base.kill();
			m_light.kill();
			Game.getInstance().getState().removeObject(m_light);
			m_light = null;
		}

		internal void stop()
		{
			m_rotationSpeed = 0;
			m_turnTimer = m_turnStopTime;
		}

		public override void addRotation(float a_rotation)
		{
			base.addRotation(a_rotation);
			m_light.setRotation(m_rotate);
		}

		public bool isTurnReady()
		{
			return m_turnTimer <= 0;
		}

		public void setRightGuardPoint(Vector2 a_position) {
			m_rightRotation = m_position.getAngleTo(a_position);
		}

		public void setLeftGuardPoint(Vector2 a_position) {
			m_leftRotation = m_position.getAngleTo(a_position);			
		}
	}
}
