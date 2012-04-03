using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
{
	[Serializable()]
	public class CheckPoint : NonMovingObject
	{
		public CheckPoint(Position a_position, String a_sprite, float a_layer, float a_rotation = 0)
			: base(a_position, a_sprite, a_layer, a_rotation)
		{
		}
		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				if (CollisionManager.Collides(this.getHitBox(), a_collider.getHitBox()) && Game.isKeyPressed(GameState.getActionKey()))
				{
					Level tLevel = new Level();
					tLevel.setLevelObjects(Game.getInstance().getState().getObjectList());
					tLevel.setEvents(((GameState)Game.getInstance().getState()).getEvents());
					Serializer.getInstance().SaveLevel("Checkpoint.txt",tLevel);
				}
			}
		}

	}
}
