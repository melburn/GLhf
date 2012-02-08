﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	public class NPE : Entity
	{
        protected AIState m_aiState;
		public NPE(Vector2 a_posV2, String a_sprite)
			: base(a_posV2, a_sprite)
		{
		}
        public override void update(GameTime a_gameTime)
		{
			if (m_aiState != null)
			{
                m_aiState = m_aiState.Execute(this);
            }
			base.update(a_gameTime);
        }
	}
}
