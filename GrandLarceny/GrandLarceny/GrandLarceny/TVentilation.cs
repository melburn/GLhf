﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class TVentilation : Ventilation
	{
		public TVentilation(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
				
		}

		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				if (CollisionManager.Contains(getHitBox(), t_player.getPosition().getGlobalCartesianCoordinates()))
				{
					if (0 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
					{
						t_player.setVentilationDirection(m_upRightDownList);
					}
					else if (1 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
					{
						t_player.setVentilationDirection(m_leftRightDownList);
					}
					else if (2 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
					{
						t_player.setVentilationDirection(m_upLeftDownList);
					}
					else if (3 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
					{
						t_player.setVentilationDirection(m_upLeftRightList);
					}
					t_player.setVentilationObject(this);
				}
			}
		}
	}
}
