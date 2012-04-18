using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class Hookshot : Rope
	{
		Player m_player;

		private bool m_isReady = false;

		private float timeToLive;

		private Vector2 m_direction = Vector2.Zero;

		public Hookshot(Vector2 a_posV2, string a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
		}

		public override void loadContent()
		{
			m_player = Game.getInstance().getState().getPlayer();
			base.loadContent();
			m_line.setStartPoint(m_player.getPosition().getGlobalCartesianCoordinates());
			m_line.setEndPoint(m_line.getStartPoint());
			m_position.setGlobalCartesianCoordinates(m_player.getPosition().getGlobalCartesianCoordinates());
			m_moveToStart = false;
			timeToLive = (float)Game.getInstance().getGameTime().TotalMilliseconds + 200;
		}

		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);/*
			m_rotate = m_rotate % ((float)Math.PI * 2);
			if (m_moveToStart && m_rotate > (float)Math.PI / 2 -0.05f && m_rotate < (float)Math.PI / 2 + 0.05f)
			{
				m_rotate = (float)Math.PI / 2;
			}
			else if (m_moveToStart && ((m_rotate > (float)Math.PI/2 + Math.PI) || ((m_rotate > Math.PI / 2 - Math.PI) && (m_rotate < Math.PI / 2))))
			{
				m_rotate += 0.04f;
			}
			else if (m_moveToStart)
			{
				m_rotate -= 0.04f;
			}*/
			if (timeToLive <= (float)Game.getInstance().getGameTime().TotalMilliseconds && !m_isReady)
				m_moveToStart = true;

			if (m_moveToStart)
			{
				Game.getInstance().getState().removeObject(this);
			}
			if (m_isReady)
			{
				m_line.setEndPoint(m_line.getStartPoint().getGlobalCartesianCoordinates() + new Vector2(m_lenght * (float)Math.Cos(m_rotate), m_lenght * (float)Math.Sin(m_rotate)), Vector2.Zero);
			}
			else
			{
				m_line.setStartPoint(m_player.getPosition().getGlobalCartesianCoordinates());
				m_line.setEndPoint(m_line.getEndPoint().getGlobalCartesianCoordinates() + (m_direction * 50), Vector2.Zero);
				m_lenght = Vector2.Distance(m_startPosition.getGlobalCartesianCoordinates(), m_endPosition.getGlobalCartesianCoordinates());
			}
		}

		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			foreach (Entity t_entity in a_collisionList)
			{
				if (!m_isReady || t_entity is Player)
					t_entity.updateCollisionWith(this);
			}
		}
		internal override void updateCollisionWith(Entity a_collid)
		{
			if(a_collid is Player && m_isReady)
				base.updateCollisionWith(a_collid);
		}
		public void setDirection(Vector2 a_direction)
		{
			m_direction = Vector2.Normalize(a_direction);
		}
		public void changeMode(Entity a_collider)
		{
			float t_startX = m_line.getStartPoint().getGlobalX();
			float t_startY = m_line.getStartPoint().getGlobalY();
			float t_endX = m_line.getEndPoint().getGlobalX();
			float t_endY = m_line.getEndPoint().getGlobalY();
			float t_colliderY = a_collider.getPosition().getGlobalY() + a_collider.getHitBox().getOutBox().Height;
			m_isReady = true;
			setStartPoint(new Vector2(((-t_startX * t_endY)	+ (t_startX	* t_colliderY) + (t_startY * t_endX) - (t_endX * t_colliderY)) / (t_startY -t_endY), t_colliderY));
			setEndPoint(m_player.getPosition().getGlobalCartesianCoordinates() + new Vector2(m_player.getHitBox().getOutBox().Width / 2, m_player.getHitBox().getOutBox().Height / 2));
			m_rotate = m_startPosition.getAngleTo(m_endPosition.getGlobalCartesianCoordinates());
			
			m_player.setState(Player.State.Swinging);
			if (!(Vector2.Distance(m_player.getPosition().getGlobalCartesianCoordinates(), m_line.getStartPoint().getGlobalCartesianCoordinates())
				< Math.Min(Vector2.Distance(new Vector2(m_player.getPosition().getGlobalCartesianCoordinates().X + m_player.getHitBox().getOutBox().Width, m_player.getPosition().getGlobalCartesianCoordinates().Y), m_line.getStartPoint().getGlobalCartesianCoordinates()),
				Vector2.Distance(new Vector2(m_player.getPosition().getGlobalCartesianCoordinates().X + m_player.getHitBox().getOutBox().Width / 2, m_player.getPosition().getGlobalCartesianCoordinates().Y), m_line.getStartPoint().getGlobalCartesianCoordinates()))))
			{
				if (Vector2.Distance(new Vector2(m_player.getPosition().getGlobalCartesianCoordinates().X + m_player.getHitBox().getOutBox().Width, m_player.getPosition().getGlobalCartesianCoordinates().Y), m_line.getStartPoint().getGlobalCartesianCoordinates())
					< Vector2.Distance(new Vector2(m_player.getPosition().getGlobalCartesianCoordinates().X + m_player.getHitBox().getOutBox().Width / 2, m_player.getPosition().getGlobalCartesianCoordinates().Y), m_line.getStartPoint().getGlobalCartesianCoordinates()))
				{
					m_player.addPositionXAfterDraw(m_player.getHitBox().getOutBox().Width);
				}
				else
				{
					m_player.addPositionXAfterDraw(m_player.getHitBox().getOutBox().Width / 2);
				}
			}
			m_player.setRope(this);
			m_player.changePositionType();
			m_player.getPosition().setParentPositionWithoutMoving(m_line.getStartPoint());
			m_player.setState(Player.State.Swinging);
			if (m_player.getPosition().getLength() < 50)
			{
				m_player.getPosition().setLength(50);
			}
			m_moveToStart = false;
		}
	}
}
