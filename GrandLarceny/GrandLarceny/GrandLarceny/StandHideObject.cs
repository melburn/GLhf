using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	[Serializable()]
	public class StandHideObject : NonMovingObject
	{
		public StandHideObject(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			
		}

		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player) a_collider;

				if (Game.isKeyPressed(Player.m_upKey) && !Game.wasKeyPressed(Player.m_upKey) && !t_player.isChase()
					&& t_player.getLastState() != Player.State.Hiding && t_player.getCurrentState() != Player.State.Jumping
					&& t_player.getCurrentState() != Player.State.Slide && t_player.getCurrentState() != Player.State.Climbing)
				{
					
					Vector2 t_playerGlobal = a_collider.getPosition().getGlobalCartesianCoordinates();
					float t_myPositionX = m_position.getGlobalCartesianCoordinates().X;
					
					if (t_playerGlobal.X < t_myPositionX)
					{
					
						t_player.setNextPositionX(t_myPositionX - a_collider.getHitBox().getOutBox().Width);
						t_player.setFacingRight(true);
					}
					else
					{
						t_player.setNextPositionX(t_myPositionX + m_img.getSize().X);
						t_player.setFacingRight(false);
					}

					t_player.setState(Player.State.Hiding);
					t_player.setHidingImage(Player.STANDHIDINGIMAGE);
					t_player.setSpeedX(0);
				}
			}
		}
	}

}
