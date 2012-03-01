using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	[Serializable()]
	class VentilationDrum : NonMovingObject
	{
		public VentilationDrum(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			
		}
		internal override void updateCollisionWith(Entity a_collider)
		{
			if (GameState.m_currentKeyInput.IsKeyDown(Keys.Up) && GameState.m_previousKeyInput.IsKeyUp(Keys.Up))
				Game.getInstance().getState().changeLayer(1);
		}
	}
}
