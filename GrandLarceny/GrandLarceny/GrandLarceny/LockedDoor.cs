using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class LockedDoor : NonMovingObject
	{
		private bool m_unlocked;
		public LockedDoor(Vector2 a_position, String a_sprite, float a_layer)
			:base (a_position, a_sprite, a_layer)
		{
			m_unlocked = false;
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (!m_unlocked && a_collid is Player && !m_dead)
			{
				if (Game.getInstance().getProgress().decreaseConsumable("key"))
				{
					m_unlocked = true;
					m_img.setSprite("Images//Prop//SecurityDoor//door_open");
				}
				else
				{
					Player t_player = (Player)a_collid;
					Rectangle t_playerOutBox = t_player.getHitBox().getOutBox();

					float t_playerPoint = t_player.getPosition().getGlobalX();
					float t_rightPoint = m_position.getGlobalX() + ((float)getHitBox().getOutBox().Width);
					float t_centerPoint = m_position.getGlobalX() + ((float)getHitBox().getOutBox().Width / 2);

					if (t_playerPoint > t_centerPoint &&
						t_playerPoint < t_rightPoint)
					{
						t_player.setNextPositionX(t_rightPoint);
						t_player.setSpeedX(0);

					}

					else if (t_playerPoint + t_playerOutBox.Width < t_centerPoint &&
						t_playerPoint + t_playerOutBox.Width > m_position.getGlobalX())
					{
						t_player.setNextPositionX(getPosition().getGlobalX() - t_playerOutBox.Width);
						t_player.setSpeedX(0);
					}
				}
			}
		}

		public override bool isTransparent()
		{
			return false;
		}

		public bool isLocked()
		{
			return !m_unlocked;
		}
	}
}
