using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	[Serializable()]
	public class DuckHideObject : NonMovingObject
	{
		public DuckHideObject(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			
		}
		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionRectangle(m_img.getSize().X / 2 - 35, 0, 70, m_img.getSize().Y, m_position);
		}
		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				Vector2 t_playerGlobalPosition = a_collider.getPosition().getGlobalCartesianCoordinates();
				Rectangle t_playerOutBox = a_collider.getHitBox().getOutBox();
				if (CollisionManager.Contains(this.getHitBox(), 
					new Vector2(t_playerGlobalPosition.X + t_playerOutBox.Width/2, t_playerGlobalPosition.Y + t_playerOutBox.Height/2)))
				{
					if (GameState.isKeyPressed(Player.m_upKey) && !GameState.wasKeyPressed(Player.m_upKey)
						&& t_player.getLastState() != Player.State.Hiding)
					{
						t_player.setState(Player.State.Hiding);
						t_player.setHidingImage(Player.DUCKHIDINGIMAGE);
						t_player.setSpeedX(0);
					}
				}
			}
		}
	}
}
