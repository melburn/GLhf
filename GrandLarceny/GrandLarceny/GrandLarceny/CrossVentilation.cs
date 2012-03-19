using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class CrossVentilation : NonMovingObject
	{
		List<Player.Direction> m_DirectionList = new List<Player.Direction>();
		public CrossVentilation(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			
		}

		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionRectangle(-3,
						-3, 6, 6, m_position);
			m_DirectionList.Add(Player.Direction.Up);
			m_DirectionList.Add(Player.Direction.Down);
			m_DirectionList.Add(Player.Direction.Left);
			m_DirectionList.Add(Player.Direction.Right);
		}

		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				if (CollisionManager.Contains(getHitBox(), t_player.getPosition().getGlobalCartesianCoordinates()))
				{
					t_player.setVentilationDirection(m_DirectionList);
				}
			}
		}
	}
}
