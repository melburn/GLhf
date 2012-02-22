using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	class DuckHideObject : NonMovingObject
	{
		public DuckHideObject(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			
		}
		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				if (CollisionManager.Collides(this.getHitBox(), a_collider.getHitBox()))
				{
					if (GameState.m_currentKeyInput.IsKeyDown(Keys.Up) && GameState.m_previousKeyInput.IsKeyUp(Keys.Up))
					{
						t_player.setState(Player.State.Hiding);
						t_player.setLayer(m_layer + 0.1f);
						t_player.setHidingImage(Player.DUCKHIDINGIMAGE);
					}
				}
			}
		}
	}
}
