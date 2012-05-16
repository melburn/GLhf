﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class CrossVentilation : Ventilation
	{
		public CrossVentilation(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			
		}

		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				if ((CollisionManager.Contains(getHitBox(), t_player.getHitBox().getPosition().getGlobalCartesian())))
				//	&& !t_player.isFacingRight()) || (CollisionManager.Contains(getHitBox(), new Vector2(t_player.getPosition().getGlobalCartesian().X + 72, t_player.getPosition().getGlobalCartesian().Y)) && t_player.isFacingRight()))
				{
					t_player.setVentilationDirection(m_upleftRightDownList);
					t_player.setVentilationObject(this);
				}
			}
		}
	}
}
