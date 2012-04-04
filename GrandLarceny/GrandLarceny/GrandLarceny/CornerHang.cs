using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
{
	public class CornerHang : NonMovingObject
	{
		public CornerHang(Position a_position, String a_sprite, float a_layer, float a_rotation = 0)
			: base(a_position, a_sprite, a_layer, a_rotation)
		{
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			base.updateCollisionWith(a_collid);
			if (a_collid is Player)
			{
				Player t_player = (Player)a_collid;
				if (t_player.getCurrentState() == Player.State.Hanging)
				{
					t_player.setSpeedY(0);
				}
			}
		}
	}
}
