using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class CornerVentilation : Ventilation
	{
		public CornerVentilation(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			
		}

		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				if ((CollisionManager.Contains(getHitBox(), t_player.getPosition().getGlobalCartesian()) && !t_player.isFacingRight())
					|| (CollisionManager.Contains(getHitBox(), new Vector2(t_player.getPosition().getGlobalCartesian().X + 72, t_player.getPosition().getGlobalCartesian().Y)) && t_player.isFacingRight()))
				{
					if (0 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
					{
						t_player.setVentilationDirection(m_upRightList);
					}
					else if (1 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
					{
						t_player.setVentilationDirection(m_rightDownList);
					}
					else if (2 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
					{
						t_player.setVentilationDirection(m_leftDownList);
					}
					else if (3 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
					{
						t_player.setVentilationDirection(m_upLeftList);
					}
					t_player.setVentilationObject(this);
				}
			}
		}
	}
}
