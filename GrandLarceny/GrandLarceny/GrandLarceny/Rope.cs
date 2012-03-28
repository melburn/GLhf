using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class Rope : NonMovingObject
	{
		public Rope(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{

		}
		public override void loadContent()
		{
			base.loadContent();
			m_rotationPoint = new Vector2(m_img.getSize().X / 2, m_position.getGlobalY());
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player)
			{
				Player t_player = (Player)a_collid;
				t_player.setState(Player.State.Swinging);
				t_player.getPosition().setParentPositionWithoutMoving(m_position);
				t_player.changePositionType();
				t_player.setRope(this);
			}
		}
	}
}
