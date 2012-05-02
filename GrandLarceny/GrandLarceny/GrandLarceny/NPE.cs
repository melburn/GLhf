using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GrandLarceny.AI;

namespace GrandLarceny
{
	[Serializable()]
	public class NPE : MovingObject
	{
		protected AIState m_aiState;

		//kan sättas till false för att deaktivera ai
		protected Boolean m_aiActive = true;

		public NPE(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
		}

		public override void update(GameTime a_gameTime)
		{
			m_lastPosition = m_position.getGlobalCartesian();
			if (m_aiState != null && m_aiActive)
			{
				m_aiState = m_aiState.execute(this);
			}
			base.update(a_gameTime);
		}

		public AIState getAIState()
		{
			return m_aiState;
		}

		public void setAIState(AIState a_aIState)
		{
			m_aiState = a_aIState;
		}
	}
}
