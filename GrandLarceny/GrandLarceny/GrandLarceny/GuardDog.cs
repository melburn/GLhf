﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GrandLarceny.AI;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Text.RegularExpressions;

namespace GrandLarceny
{
	[Serializable()]
	class GuardDog : GuardEntity
	{
		private const float MOVEMENTSPEED = 80;
		private const float CHARGEINGSPEED = 540;
		private const float WALKANISPEED = 7;
		private const float CHARGEANISPEED = 20;
		private const float BARKCOOLDOWN = 0.8f;
		private Boolean m_chargeing = false;
		private Boolean m_facingRight;
		private Boolean m_barking = false;
		private float m_sightRange = 300f;
		private float m_senseRange = 144f;
		private float m_chargeEndPoint;
		private float m_barkTimer = 0f;
		private Entity m_chaseTarget = null;

		[NonSerialized()]
		private Sound m_dogBark;

		public GuardDog(Vector2 a_posV2, String a_sprite, float a_leftPatrolPoint, float a_rightPatrolPoint, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			if (a_rightPatrolPoint < a_leftPatrolPoint)
			{
				throw new ArgumentException();
			}
			m_leftPatrolPoint = a_leftPatrolPoint;
			m_rightPatrolPoint = a_rightPatrolPoint;
			m_hasPatrol = (m_leftPatrolPoint != m_rightPatrolPoint);
			m_aiState = AIStatepatroling.getInstance();
			m_gravity = 1000;
		}

		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionRectangle(15, 30, m_img.getSize().X - 30, m_img.getSize().Y - 30, m_position);
			string[] t_heroSprites = Directory.GetFiles("Content//Images//Sprite//GuardDog//");
			foreach (string t_file in t_heroSprites) {
				string[] t_splitFile = Regex.Split(t_file, "//");
				string[] t_extless = t_splitFile[t_splitFile.Length - 1].Split('.');
				if (t_extless[1].Equals("xnb")) {
					Game.getInstance().Content.Load<Texture2D>("Images//Sprite//GuardDog//" + t_extless[0]);
				}
			}
			(m_dogBark = new Sound("//SoundEffects//Game//dog_bark")).loadContent();
		}

		internal bool canSensePlayer()
		{
			Player t_player = Game.getInstance().getState().getPlayer();
			return t_player != null &&
				((t_player.getPosition().getGlobalCartesian() - m_position.getGlobalCartesian()).Length() < m_senseRange ||
				(t_player.getCurrentState() != Player.State.Hiding &&
				isFacingTowards(t_player.getPosition().getGlobalX()) &&
				Math.Abs(t_player.getPosition().getGlobalX() - m_position.getGlobalX()) < m_sightRange &&
				t_player.getPosition().getGlobalY() <= m_position.getGlobalY() + 100 &&
				t_player.getPosition().getGlobalY() >= m_position.getGlobalY() - 200));
		}

		public bool isFacingTowards(float a_x)
		{
			return (a_x <= m_position.getGlobalX() && !m_facingRight)
				|| (a_x >= m_position.getGlobalX() && m_facingRight);
		}

		internal void goRight()
		{
			if (m_chargeing)
			{
				m_speed.X = CHARGEINGSPEED;
				m_img.setAnimationSpeed(CHARGEANISPEED);
			}
			else
			{
				m_speed.X = MOVEMENTSPEED;
				m_img.setAnimationSpeed(WALKANISPEED);
			}
			m_facingRight = true;
			m_barking = false;
		}

		internal void goLeft()
		{
			if (m_chargeing)
			{
				m_speed.X = -CHARGEINGSPEED;
				m_img.setAnimationSpeed(CHARGEANISPEED);
			}
			else
			{
				m_speed.X = -MOVEMENTSPEED;
				m_img.setAnimationSpeed(WALKANISPEED);
			}
			m_facingRight = false;
			m_barking = false;
		}

		internal void stop()
		{
			m_speed.X = 0;
			if (m_aiState is AIStateChargeing)
			{
				m_aiState = AIStatepatroling.getInstance();
			}
			m_img.setAnimationSpeed(0);
		}
		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			if (m_barking)
			{
				m_barkTimer -= a_gameTime.ElapsedGameTime.Milliseconds / 1000f;
				if (m_barkTimer <= 0)
				{
					m_dogBark.play();
					Player t_player = Game.getInstance().getState().getPlayer();
					foreach (GameObject go in Game.getInstance().getState().getCurrentList())
					{
						if (go is Guard && (go.getPosition().getGlobalCartesian() - m_position.getGlobalCartesian()).Length() <= AIStateBark.BARKRADIUS && ((NPE)go).getAIState() != AIStateChasing.getInstance())
						{
							if (!t_player.isChase())
							{
								t_player.activateChaseMode((NPE)go);
							}
							((Guard)go).setChaseTarget(m_chaseTarget);
							((NPE)go).setAIState(AIStateChasing.getInstance());
						}
					}
					m_barkTimer = BARKCOOLDOWN;
				}
			}
			if (m_facingRight)
			{
				m_spriteEffects = SpriteEffects.None;
			}
			else
			{
				m_spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}

		public float getChargeingPoint()
		{
			return m_chargeEndPoint;
		}

		internal void setChargeing(bool a_chargeing)
		{
			if (m_chargeing)
			{
				if (!a_chargeing)
				{
					m_chargeing = false;
					if (m_speed.X > 0)
					{
						m_speed.X = MOVEMENTSPEED;
					}
					else if (m_speed.X < 0)
					{
						m_speed.X = -MOVEMENTSPEED;
					}
					m_img.setAnimationSpeed(WALKANISPEED);
				}
			}
			else
			{
				if (a_chargeing)
				{
					m_chargeing = true;
					if (m_speed.X > 0)
					{
						m_speed.X = CHARGEINGSPEED;
					}
					else if (m_speed.X < 0)
					{
						m_speed.X = -CHARGEINGSPEED;
					}
				}
				m_img.setAnimationSpeed(CHARGEANISPEED);
			}
		}

		internal void setChargePoint(float a_x)
		{
			m_chargeEndPoint = a_x;
		}

		public bool isBarkingPrefered()
		{
			//retunerar om hunden föredrar att skälla i denna situation.
			return (Game.getInstance().getState().getPlayer().getCurrentState() == Player.State.Hiding);
		}

		internal void chasePlayer()
		{
			Player t_player = Game.getInstance().getState().getPlayer();
			m_chaseTarget = t_player;
			
		}

		internal void forgetChaseTarget()
		{
			m_chaseTarget = null;
		}

		public bool isChargeing()
		{
			return m_chargeing;
		}

		public bool ifFaceingRight()
		{
			return m_facingRight;
		}

		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			Platform t_supportingPlatform = null;
			Rectangle t_outBox = getHitBox().getOutBox();
			foreach (Entity t_collision in a_collisionList)
			{
				Rectangle t_collisionRectangle = t_collision.getHitBox().getOutBox();
				if (t_collision is Wall || t_collision is Window)
				{
					if (m_speed.X < 0 && getCenterPoint().X > t_collision.getCenterPoint().X)
					{
						m_nextPosition.X = (t_collisionRectangle.X + t_collisionRectangle.Width - ((CollisionRectangle)m_collisionShape).m_xOffset);
						stop();
					}
					else if (m_speed.X > 0 && getCenterPoint().X < t_collision.getCenterPoint().X)
					{
						m_nextPosition.X = (t_collisionRectangle.X - ((CollisionRectangle)m_collisionShape).getOutBox().Width - ((CollisionRectangle)m_collisionShape).m_xOffset);
						stop();
					}
				}
				else if (t_collision is Platform)
				{
					if (t_collision.getPosition().getGlobalY() < m_position.getGlobalY() + m_img.getSize().Y - 50)
					{
						if (m_speed.X < 0 && t_outBox.X < t_collisionRectangle.X + t_collisionRectangle.Width)
						{
							m_nextPosition.X = (t_collisionRectangle.X + t_collisionRectangle.Width - ((CollisionRectangle)m_collisionShape).m_xOffset);
							stop();
						}
						else if (m_speed.X > 0 && t_outBox.X + t_outBox.Width > t_collision.getPosition().getGlobalX())
						{
							m_nextPosition.X = (t_collisionRectangle.X - ((CollisionRectangle)m_collisionShape).getOutBox().Width - ((CollisionRectangle)m_collisionShape).m_xOffset);
							stop();
						}
					}
					else
					{
						if (m_gravity > 0)
						{
							m_gravity = 0;
							m_speed.Y = 0;
							m_nextPosition.Y = (t_collision.getPosition().getGlobalY() - m_img.getSize().Y);
						}
						if (t_supportingPlatform == null ||
							(m_facingRight && t_collision.getPosition().getGlobalX() > t_supportingPlatform.getPosition().getGlobalX()) ||
							(!m_facingRight && t_collision.getPosition().getGlobalX() < t_supportingPlatform.getPosition().getGlobalX()))
						{
							t_supportingPlatform = (Platform)t_collision;
						}
					}
				}
				else if (t_collision is Player)
				{
					Player t_player = (Player)t_collision;
					if (t_player.getCurrentState() != Player.State.Rolling && t_player.getCurrentState() != Player.State.Hiding)
					{
						if (m_aiState == AIStateChargeing.getInstance())
						{
							if (m_facingRight)
							{
								t_player.dealDamageTo(new Vector2(400, -400));
							}
							else
							{
								t_player.dealDamageTo(new Vector2(-400, -400));
							}
						}
						else
						{
							chasePlayer();
							m_aiState = AIStateChargeing.getInstance();
						}
					}
				}
			}
			if (m_gravity == 0)
			{
				if (t_supportingPlatform == null)
				{
					m_gravity = 500;
				}
				else
				{
					if (m_speed.X > 0)
					{
						if (t_supportingPlatform.getPosition().getGlobalX() + t_supportingPlatform.getImg().getSize().X < m_collisionShape.getOutBox().X + m_collisionShape.getOutBox().Width)
						{
							m_nextPosition.X = (t_supportingPlatform.getPosition().getGlobalX() + t_supportingPlatform.getImg().getSize().X - m_collisionShape.getOutBox().Width);
							stop();
							m_aiState = AIStatepatroling.getInstance();
						}
					}
					else if (m_speed.X < 0)
					{
						if (t_supportingPlatform.getPosition().getGlobalX() > m_collisionShape.getOutBox().X)
						{
							m_nextPosition.X = (t_supportingPlatform.getPosition().getGlobalX());
							stop();
							m_aiState = AIStatepatroling.getInstance();
						}
					}
				}
			}
		}

		public bool isBarking()
		{
			return m_barking;
		}

		internal void startBarking()
		{
			m_dogBark.play();
			m_chargeing = false;
			m_speed.X = 0;
			m_barkTimer = BARKCOOLDOWN;
			m_barking = true;
		}

		public void setFacing(bool a_facingRight)
		{
			if (m_facingRight)
			{
				if (!a_facingRight)
				{
					if (m_speed.X > 0)
					{
						if (m_chargeing)
						{
							m_speed.X = -CHARGEINGSPEED;
						}
						else
						{
							m_speed.X = -MOVEMENTSPEED;
						}
					}
					m_facingRight = a_facingRight;
				}
			}
			else if (a_facingRight)
			{
				if (m_speed.X < 0)
				{
					if (m_chargeing)
					{
						m_speed.X = CHARGEINGSPEED;
					}
					else
					{
						m_speed.X = MOVEMENTSPEED;
					}
				}
				m_facingRight = a_facingRight;
			}
		}
	}
}
