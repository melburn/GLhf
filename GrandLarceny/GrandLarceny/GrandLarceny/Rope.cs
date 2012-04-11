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

		private bool m_moveToStart = true;

		public Rope(Vector2 a_posV2, string a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{

		}

		public override void loadContent()
		{
			base.loadContent();
			m_line = new Line(m_position, m_position, new Vector2(36, 0), new Vector2(36, 72), Color.Black, 5, true);
			m_collisionShape = new CollisionRectangle(0, 0, 72, 72, m_position);
			m_rotationPoint.Y = 0;
			m_lenght = m_line.getStartPoint().getDistanceTo(m_line.getEndPoint());
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

		public override CollisionShape getImageBox()
		{
			return new CollisionRectangle(0, 0, 72, 72, m_position);
		}

		public void setEndpoint(Vector2 a_endPoint)
		{
			m_line.setEndPoint(a_endPoint);
		}

		public void setEndpoint(Position a_position)
		{
			m_line.setEndPoint(a_position);
		}

		public void setEndpoint(Position a_position, Vector2 a_offset)
		{
			m_line.setEndPoint(a_position, a_offset);
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
					if (Vector2.Distance(t_player.getPosition().getGlobalCartesianCoordinates(), m_line.getStartPoint().getGlobalCartesianCoordinates())
						< Math.Min(Vector2.Distance(new Vector2(t_player.getPosition().getGlobalCartesianCoordinates().X + t_player.getHitBox().getOutBox().Width, t_player.getPosition().getGlobalCartesianCoordinates().Y), m_line.getStartPoint().getGlobalCartesianCoordinates()),
						Vector2.Distance(new Vector2(t_player.getPosition().getGlobalCartesianCoordinates().X + t_player.getHitBox().getOutBox().Width / 2, t_player.getPosition().getGlobalCartesianCoordinates().Y), m_line.getStartPoint().getGlobalCartesianCoordinates())))
					{
					}
					else if (Vector2.Distance(new Vector2(t_player.getPosition().getGlobalCartesianCoordinates().X + t_player.getHitBox().getOutBox().Width, t_player.getPosition().getGlobalCartesianCoordinates().Y), m_line.getStartPoint().getGlobalCartesianCoordinates())
						< Vector2.Distance(new Vector2(t_player.getPosition().getGlobalCartesianCoordinates().X + t_player.getHitBox().getOutBox().Width / 2, t_player.getPosition().getGlobalCartesianCoordinates().Y), m_line.getStartPoint().getGlobalCartesianCoordinates()))
					{
						t_player.addPositionXAfterDraw(t_player.getHitBox().getOutBox().Width);
		//				Game.getInstance().m_camera.getPosition().plusXWith(-t_player.getHitBox().getOutBox().Width);
					}
					else
					{
						t_player.addPositionXAfterDraw(t_player.getHitBox().getOutBox().Width / 2);
		//				Game.getInstance().m_camera.getPosition().plusXWith(-t_player.getHitBox().getOutBox().Width / 2);
					}
					t_player.changePositionType();
		//			m_rotate = (float)Math.Atan2(-(m_position.getGlobalY() - t_player.getPosition().getGlobalY()), -(m_position.getGlobalX() - t_player.getPosition().getGlobalX()));
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
