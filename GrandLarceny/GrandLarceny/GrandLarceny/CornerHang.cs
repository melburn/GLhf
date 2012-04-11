using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class CornerHang : NonMovingObject
	{
		public CornerHang(Vector2 a_position, String a_sprite, float a_layer, float a_rotation = 0)
			: base(new CartesianCoordinate(a_position), a_sprite, a_layer, a_rotation)
		{
		}

		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionRectangle(0, 0, m_img.getSize().X, 30, m_position);
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			base.updateCollisionWith(a_collid);
			if (a_collid is Player)
			{
				Player t_player = (Player)a_collid;
				if (t_player.getCurrentState() == Player.State.Slide)
				{
					
					t_player.setNextPositionY(t_player.getLastPosition().Y);
					
				}
			}
		}
	}
}
