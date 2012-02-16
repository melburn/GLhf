using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class NPE : Entity
	{
		protected AIState m_aiState;
		public NPE(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
		}

		public override void update(GameTime a_gameTime)
		{
			if (m_aiState != null)
			{
				m_aiState = m_aiState.execute(this);
			}
			base.update(a_gameTime);
		}
	}
}
