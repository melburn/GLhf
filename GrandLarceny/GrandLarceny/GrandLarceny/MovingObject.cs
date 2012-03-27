using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class MovingObject : Entity
	{
		public MovingObject(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
		}
		public MovingObject(Position a_position, String a_sprite, float a_layer, float a_rotation = 0)
			: base(a_position, a_sprite, a_layer, a_rotation)
		{
		}

		internal virtual void collisionCheck(List<Entity> a_collisionList)
		{
			foreach (Entity t_entity in a_collisionList)
			{
				if(m_collisionShape != null)
					t_entity.updateCollisionWith(this);
			}
		}
	}
}
