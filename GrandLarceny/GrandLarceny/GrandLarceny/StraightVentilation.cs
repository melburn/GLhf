using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	[Serializable()]
	class StraightVentilation : NonMovingObject
	{
		List<Player.Direction> m_upDownList = new List<Player.Direction>();
		List<Player.Direction> m_leftRightList = new List<Player.Direction>();
		public StraightVentilation(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			
		}

		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionRectangle(-3,
						-3, 6, 6, m_position);
			m_upDownList.Add(Player.Direction.Up);
			m_upDownList.Add(Player.Direction.Down);
			m_leftRightList.Add(Player.Direction.Left);
			m_leftRightList.Add(Player.Direction.Right);
		}

		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				KeyboardState t_currKeys = Game.m_currentKeyInput;
				Player t_player = (Player)a_collider;
				if (CollisionManager.Contains(getHitBox(), t_player.getPosition().getGlobalCartesianCoordinates()))
				{
					if (1 == Math.Round(((2 * m_rotate) / Math.PI) % 4) || 3 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
					{
						t_player.setVentilationDirection(m_upDownList);
						t_player.setNextPositionX(m_position.getGlobalCartesianCoordinates().X);
					}
					else if (0 == Math.Round(((2 * m_rotate) / Math.PI) % 4) || 2 == Math.Round(((2 * m_rotate) / Math.PI) % 4))
					{
						t_player.setVentilationDirection(m_leftRightList);
						t_player.setNextPositionY(m_position.getGlobalCartesianCoordinates().Y);
					}
				}
			}
		}
	}
}
