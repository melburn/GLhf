using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class Rope : MovingObject
	{
		[NonSerialized()]
		protected Line m_line;

		protected float m_lenght;
		protected float m_swingSpeed;

		protected Position m_startPosition;
		protected Position m_endPosition;

		protected bool m_moveToStart = true;

		public Rope(Vector2 a_posV2, string a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
		}

		public override void loadContent()
		{
			base.loadContent();
			m_startPosition = m_position;
			m_endPosition = new CartesianCoordinate(m_position.getGlobalCartesianCoordinates() + new Vector2(0, (float)Math.Max(m_lenght, 72)));
			m_endPosition.setParentPositionWithoutMoving(m_startPosition);
			m_line = new Line(m_startPosition, m_endPosition, new Vector2(36, 0), new Vector2(36, 0), Color.Black, 5, true);
			m_collisionShape = new CollisionLine(m_startPosition.getGlobalCartesianCoordinates(), m_endPosition.getGlobalCartesianCoordinates());
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
			m_collisionShape.setPosition(m_line.getStartPoint());
			((CollisionLine)m_collisionShape).setEndPosition(m_line.getEndPoint().getGlobalCartesianCoordinates());
			if (m_moveToStart)
			{
				m_swingSpeed += (float)(Math.Cos(m_rotate) * a_gameTime.ElapsedGameTime.Milliseconds / 1000f);
				m_swingSpeed = m_swingSpeed * 0.97f;
				m_rotate += m_swingSpeed;
			}
			if (!(this is Hookshot))
				m_line.setEndPoint(m_line.getStartPoint().getGlobalCartesianCoordinates() + new Vector2(m_lenght * (float)Math.Cos(m_rotate), m_lenght * (float)Math.Sin(m_rotate)), Vector2.Zero);
		}

		public override void draw(GameTime a_gameTime)
		{
			m_line.draw();
		}
		public override CollisionShape getImageBox()
		{
			return new CollisionRectangle(0, 0, 72, 72, m_startPosition);
		}

		public void setLength(float a_length)
		{
			m_lenght = a_length;
			m_endPosition = new CartesianCoordinate(m_position.getGlobalCartesianCoordinates() + new Vector2(0, (float)Math.Max(m_lenght, 72)));
		}

		public void setStartPoint(Vector2 a_startPoint)
		{
			m_startPosition = new CartesianCoordinate(a_startPoint);
			m_endPosition.setParentPositionWithoutMoving(m_startPosition);
			m_line.setStartPoint(m_startPosition.getGlobalCartesianCoordinates());
			m_lenght = m_line.getStartPoint().getDistanceTo(m_line.getEndPoint());
			m_position = m_startPosition;
		}

		public void setEndPoint(Vector2 a_endPoint)
		{
			m_endPosition = new CartesianCoordinate(a_endPoint);
			m_endPosition.setParentPositionWithoutMoving(m_startPosition);
			m_endPosition.plusXWith(36);
			m_line.setEndPoint(m_endPosition);
			m_lenght = m_line.getStartPoint().getDistanceTo(m_line.getEndPoint());
		}

		public void setEndPoint(Position a_position)
		{
			m_endPosition = a_position;
			m_endPosition.setParentPositionWithoutMoving(m_startPosition);
			m_endPosition.plusXWith(36);
			m_line.setEndPoint(m_endPosition);
			m_lenght = m_line.getStartPoint().getDistanceTo(m_line.getEndPoint());
		}

		public void setEndPoint(Position a_position, Vector2 a_offset)
		{
			m_endPosition = new CartesianCoordinate(a_offset, a_position);
			m_endPosition.setParentPositionWithoutMoving(m_startPosition);
			m_endPosition.plusXWith(36);
			m_line.setEndPoint(m_endPosition);
			m_lenght = m_line.getStartPoint().getDistanceTo(m_line.getEndPoint());
		}

		public void setEndPoint(Vector2 a_position, Vector2 a_offset)
		{
			m_endPosition = new CartesianCoordinate(a_offset, new CartesianCoordinate(a_position));
			m_endPosition.setParentPositionWithoutMoving(m_startPosition);
			m_endPosition.plusXWith(36);
			m_line.setEndPoint(m_endPosition);
			m_lenght = m_line.getStartPoint().getDistanceTo(m_line.getEndPoint());
		}

		public void moveRope(Vector2 a_position)
		{
			m_startPosition.setLocalX(a_position.X);
			m_startPosition.plusXWith(36);
			m_startPosition.setLocalY(a_position.Y);
		}

		public Position getEndpoint()
		{
			return m_endPosition;
		}

		public float getLength()
		{
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
				if (t_player.getRope() != this && t_player.getHitBox().collides(m_collisionShape))
				{
					t_player.setState(Player.State.Swinging);
					if (!(Vector2.Distance(t_player.getPosition().getGlobalCartesianCoordinates(), m_line.getStartPoint().getGlobalCartesianCoordinates())
						< Math.Min(Vector2.Distance(new Vector2(t_player.getPosition().getGlobalCartesianCoordinates().X + t_player.getHitBox().getOutBox().Width, t_player.getPosition().getGlobalCartesianCoordinates().Y), m_line.getStartPoint().getGlobalCartesianCoordinates()),
						Vector2.Distance(new Vector2(t_player.getPosition().getGlobalCartesianCoordinates().X + t_player.getHitBox().getOutBox().Width / 2, t_player.getPosition().getGlobalCartesianCoordinates().Y), m_line.getStartPoint().getGlobalCartesianCoordinates()))))
					{
						if (Vector2.Distance(new Vector2(t_player.getPosition().getGlobalCartesianCoordinates().X + t_player.getHitBox().getOutBox().Width, t_player.getPosition().getGlobalCartesianCoordinates().Y), m_line.getStartPoint().getGlobalCartesianCoordinates())
							< Vector2.Distance(new Vector2(t_player.getPosition().getGlobalCartesianCoordinates().X + t_player.getHitBox().getOutBox().Width / 2, t_player.getPosition().getGlobalCartesianCoordinates().Y), m_line.getStartPoint().getGlobalCartesianCoordinates()))
						{
							t_player.addPositionXAfterDraw(t_player.getHitBox().getOutBox().Width);
						}
						else
						{
							t_player.addPositionXAfterDraw(t_player.getHitBox().getOutBox().Width / 2);
						}
					}
					t_player.setRope(this);
					t_player.changePositionType();
					t_player.getPosition().setParentPositionWithoutMoving(m_line.getStartPoint());
					t_player.setState(Player.State.Swinging);
					if (t_player.getPosition().getLength() < 50)
					{
						t_player.getPosition().setLength(50);
					}
					m_moveToStart = false;
				}
			}
		}
	}
}
