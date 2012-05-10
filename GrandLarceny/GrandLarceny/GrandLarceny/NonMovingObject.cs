using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class NonMovingObject : Entity
	{
		public NonMovingObject(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			
		}
		public NonMovingObject(Position a_position, String a_sprite, float a_layer, float a_rotation = 0)
			: base(a_position, a_sprite, a_layer, a_rotation)
		{
		}

		public override void update(GameTime a_gameTime)
		{
			m_lastPosition = m_position.getGlobalCartesian();
			base.update(a_gameTime);
		}
	}
}