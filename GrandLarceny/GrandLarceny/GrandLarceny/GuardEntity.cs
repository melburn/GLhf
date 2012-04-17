using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class GuardEntity : NPE
	{
		protected float m_leftPatrolPoint;
		protected float m_rightPatrolPoint;
		protected Boolean m_hasPatrol;
		
		public GuardEntity(Vector2 a_posV2, string a_sprite, float a_layer) :  base(a_posV2, a_sprite, a_layer)
		{

		}

		public override void loadContent()
		{
			base.loadContent();
		}

		public bool haspatrol()
		{
			return m_hasPatrol;
		}	

		public float getLeftPatrolPoint()
		{
			return m_leftPatrolPoint;
		}
		public float getRightPatrolPoint()
		{
			return m_rightPatrolPoint;
		}

		public void setLeftGuardPoint(float a_x)
		{
			m_leftPatrolPoint = a_x;
			m_hasPatrol = (m_leftPatrolPoint != m_rightPatrolPoint);
		}

		public void setRightGuardPoint(float a_x)
		{
			m_rightPatrolPoint = a_x;
			m_hasPatrol = (m_leftPatrolPoint != m_rightPatrolPoint);
		}

		public Line showRightGuardPoint()
		{
			return new Line(m_position, m_position, new Vector2(36, 72), new Vector2(getRightPatrolPoint() - m_position.getGlobalX() + 36, 72), Color.Green, 5, true);
		}

		public Line showLeftGuardPoint()
		{
			return new Line(m_position, m_position, new Vector2(36, 72), new Vector2(getLeftPatrolPoint() - m_position.getGlobalX() + 36, 72), Color.Green, 5, true);
		}
	}
}
