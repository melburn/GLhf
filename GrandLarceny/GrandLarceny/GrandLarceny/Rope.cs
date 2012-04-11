using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class Rope : NonMovingObject
	{
		[NonSerialized()]
		private Line m_line;

		private float m_lenght;
		private float m_swingSpeed;

		private Position m_startPosition;
		private Position m_endPosition;

		private bool m_moveToStart = true;

		public Rope(Vector2 a_posV2, string a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
		}

		public override void loadContent()
		{
			base.loadContent();			
			m_startPosition = m_position;
			m_endPosition = new CartesianCoordinate(m_position.getGlobalCartesianCoordinates() + new Vector2(0, (float)Math.Max(m_lenght,72)));
			m_endPosition.setParentPositionWithoutMoving(m_startPosition);
			m_line = new Line(m_startPosition, m_endPosition, new Vector2(36, 0), new Vector2(36, 0), Color.Black, 5, true);
			m_collisionShape = new CollisionRectangle(33, 0, 6, m_lenght, m_startPosition);
			m_rotationPoint.Y = 0;
			m_rotate = (float)Math.PI / 2;
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
			if (m_moveToStart)
			{
				m_swingSpeed += (float)(Math.Cos(m_rotate) * a_gameTime.ElapsedGameTime.Milliseconds / 1000f);
				m_swingSpeed = m_swingSpeed * 0.97f;
				m_rotate += m_swingSpeed;
			}
			m_line.setEndPoint(m_line.getStartPoint().getGlobalCartesianCoordinates() + new Vector2(m_lenght * (float)Math.Cos(m_rotate), m_lenght * (float)Math.Sin(m_rotate)), Vector2.Zero);
		}

		public override void draw(GameTime a_gameTime)
		{
			m_line.draw();
		}

		public override CollisionShape getImageBox() {
			return new CollisionRectangle(0, 0, 72, 72, m_startPosition);
		}

		public void setLength(float a_length) {
			m_lenght = a_length;
			m_endPosition = new CartesianCoordinate(m_position.getGlobalCartesianCoordinates() + new Vector2(0, (float)Math.Max(m_lenght, 72)));
		}

		public void setEndpoint(Vector2 a_endPoint) {
			m_endPosition = new CartesianCoordinate(a_endPoint);
			m_endPosition.setParentPositionWithoutMoving(m_startPosition);
			m_endPosition.plusXWith(36);
			m_line.setEndPoint(m_endPosition);
			m_lenght = m_line.getStartPoint().getDistanceTo(m_line.getEndPoint());
		}

		public void setEndpoint(Position a_position) {
			m_endPosition = a_position;
			m_endPosition.setParentPositionWithoutMoving(m_startPosition);
			m_endPosition.plusXWith(36);
			m_line.setEndPoint(m_endPosition);
			m_lenght = m_line.getStartPoint().getDistanceTo(m_line.getEndPoint());
		}

		public void setEndpoint(Position a_position, Vector2 a_offset) {
			m_endPosition = new CartesianCoordinate(a_offset, a_position);
			m_endPosition.setParentPositionWithoutMoving(m_startPosition);
			m_endPosition.plusXWith(36);
			m_line.setEndPoint(m_endPosition);
			m_lenght = m_line.getStartPoint().getDistanceTo(m_line.getEndPoint());
		}

		public void moveRope(Vector2 a_position) {
			m_startPosition.setLocalX(a_position.X);
			m_startPosition.plusXWith(36);
			m_startPosition.setLocalY(a_position.Y);
		}

		public Position getEndpoint() {
			return m_endPosition;
		}

		public float getLength() {
			return m_lenght;
		}

		public void resetPosition()
		{
			m_moveToStart = true;
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player)
			{
				Player t_player = (Player)a_collid;
				if (t_player.getRope() != this && t_player.getHitBox().collidesWithLineSegment(m_line.getStartPoint().getGlobalCartesianCoordinates(), m_line.getEndPoint().getGlobalCartesianCoordinates()))
				{
					t_player.setState(Player.State.Swinging);
					t_player.changePositionType();
					m_rotate = (float)Math.Atan2(-(m_position.getGlobalY() - t_player.getPosition().getGlobalY()), -(m_position.getGlobalX() - t_player.getPosition().getGlobalX()));
					t_player.getPosition().setParentPositionWithoutMoving(m_line.getStartPoint());
					t_player.setRope(this);
					t_player.setSpeedX(0);
					t_player.setSpeedY(0);
					m_moveToStart = false;
				}
			}
		}
	}
}
