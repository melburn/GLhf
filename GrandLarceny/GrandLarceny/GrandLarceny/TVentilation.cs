using System;
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

		public override void loadContent()
		{
			base.loadContent();
			if (0 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
			{
				m_collisionShape = new CollisionRectangle(-3, -10, 6, 20, m_position);
			}
			else if (1 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
			{
				m_collisionShape = new CollisionRectangle(-10, -3, 20, 6, m_position);
			}
			else if (2 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
			{
				m_collisionShape = new CollisionRectangle(-3, -10, 6, 20, m_position);
			}
			else if (3 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
			{
				m_collisionShape = new CollisionRectangle(-10, -3, 20, 6, m_position);
			}
		}

		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				if ((CollisionManager.Contains(getHitBox(), t_player.getPosition().getGlobalCartesian())))
				// && !t_player.isFacingRight()) || (CollisionManager.Contains(getHitBox(), new Vector2(t_player.getPosition().getGlobalCartesian().X + 72, t_player.getPosition().getGlobalCartesian().Y)) && t_player.isFacingRight()))
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
