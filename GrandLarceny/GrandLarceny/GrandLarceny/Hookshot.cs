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
			m_position.setGlobalCartesianCoordinates(m_player.getPosition().getGlobalCartesianCoordinates());
			m_moveToStart = false;
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
				Game.getInstance().getState().removeObject(this);
			}
			if (m_isReady)
			{
				m_line.setEndPoint(m_line.getStartPoint().getGlobalCartesianCoordinates() + new Vector2(m_lenght * (float)Math.Cos(m_rotate), m_lenght * (float)Math.Sin(m_rotate)), Vector2.Zero);
			}
			else
			{
				m_line.setStartPoint(m_player.getPosition().getGlobalCartesianCoordinates());
				m_line.setEndPoint(m_line.getEndPoint().getGlobalCartesianCoordinates() + (m_direction * 10));
			}
		}

		internal override void collisionCheck(List<Entity> a_collisionList)
		{
			foreach (Entity t_entity in a_collisionList)
			{
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
		public void changeMode()
		{
			m_isReady = true;
			Vector2 t_end = m_line.getEndPoint().getGlobalCartesianCoordinates();
			m_line.setEndPoint(m_line.getStartPoint());
			m_line.setStartPoint(t_end);
		}
	}
}
