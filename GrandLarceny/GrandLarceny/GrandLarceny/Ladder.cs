using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	class Ladder : NonMovingObject
	{
		public Ladder(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			
		}

		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionRectangle(-3, 0, 6, m_img.getSize().Y, m_position);
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player)
			{
				Vector2 t_playerGlobalCoordinate = a_collid.getPosition().getGlobalCartesian();
				if (CollisionManager.Contains(this.getHitBox(), t_playerGlobalCoordinate))
				{
					Player t_player = (Player)a_collid;
					if (
						(KeyboardHandler.keyClicked(GameState.getUpKey()) && (t_player.getCurrentState() == Player.State.Walking || t_player.getCurrentState() == Player.State.Stop))
						|| (t_player.getCurrentState() != Player.State.Walking && t_player.getCurrentState() != Player.State.Stop)
					)
					{
						if (this.m_spriteEffects == SpriteEffects.FlipHorizontally)
							t_player.setIsOnLadderWithDirection(Player.Direction.Left);
						else
							t_player.setIsOnLadderWithDirection(Player.Direction.Right);
					}
				}
			}

		}
	}
}
