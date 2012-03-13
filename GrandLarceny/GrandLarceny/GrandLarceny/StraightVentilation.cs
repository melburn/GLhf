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
		public StraightVentilation(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			
		}

		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionRectangle(-3,
						0, 6, 6, m_position);
		}

		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				KeyboardState t_currKeys = GameState.m_currentKeyInput;
				Player t_player = (Player)a_collider;
				if (CollisionManager.Contains(getHitBox(), t_player.getPosition().getGlobalCartesianCoordinates()))
				{
					if ((t_currKeys.IsKeyDown(Keys.Up)) && (m_rotate == Math.PI/2||m_rotate == Math.PI*1.5))
					{
						t_player.setVentilationDirection(Player.Direction.Up);
						t_player.setNextPositionX(m_position.getGlobalCartesianCoordinates().X);
					}
					else if ((t_currKeys.IsKeyDown(Keys.Left)) && (m_rotate == 0 || m_rotate == Math.PI))
					{
						t_player.setVentilationDirection(Player.Direction.Left);
						t_player.setNextPositionY(m_position.getGlobalCartesianCoordinates().Y);
					}
					else if ((t_currKeys.IsKeyDown(Keys.Right)) && (m_rotate == 0 || m_rotate == Math.PI))
					{
						t_player.setVentilationDirection(Player.Direction.Right);
						t_player.setNextPositionY(m_position.getGlobalCartesianCoordinates().Y);
					}
					else if ((t_currKeys.IsKeyDown(Keys.Down)) && (m_rotate == Math.PI / 2 || m_rotate == Math.PI * 1.5))
					{
						t_player.setVentilationDirection(Player.Direction.Down);
						t_player.setNextPositionX(m_position.getGlobalCartesianCoordinates().X);
					}
				}
			}
		}
	}
}
