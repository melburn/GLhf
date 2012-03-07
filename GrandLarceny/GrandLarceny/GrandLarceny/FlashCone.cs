using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	class FlashCone : MovingObject
	{
		private Guard m_parent;
		Boolean m_facingRight;

		[NonSerialized]
		private bool m_collisionIsUpdated;

		public FlashCone(Guard a_parent, Vector2 a_offset, string a_sprite, Boolean a_facingRight, float a_layer) :
			base(new CartesianCoordinate(a_offset, a_parent.getPosition()), a_sprite, a_layer)
		{
			m_parent = a_parent;
			setFacingRight(a_facingRight);
		}

		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			if (m_parent == null || m_parent.isDead())
			{
				m_dead = true;
			}
		}

		public void setFacingRight(bool a_right)
		{
			m_facingRight = a_right;
			if (m_facingRight)
			{
				m_spriteEffects = SpriteEffects.None;
			}
			else
			{
				m_spriteEffects = SpriteEffects.FlipHorizontally;
			}
			m_collisionIsUpdated = false;
		}

		public void setSprite(string a_path)
		{
			if (m_img.setSprite(a_path))
			{
				m_img.stop();
			}
		}

		public void setSubImage(float a_index)
		{
			m_img.setSubImage(a_index);
		}

		public override void loadContent()
		{
			base.loadContent();
			m_img.stop();
			m_collisionIsUpdated = false;
		}
		public override CollisionShape getHitBox()
		{
			if (!m_collisionIsUpdated)
			{
				m_collisionShape = new CollisionTriangle(getTrianglePointsOffset(), m_position);
				m_collisionIsUpdated = true;
			}
			return m_collisionShape;
		}

		private Vector2[] getTrianglePointsOffset()
		{
			Vector2[] t_ret = new Vector2[3];
			if (m_facingRight)
			{
				t_ret[0] = new Vector2(60, 78);
				t_ret[1] = new Vector2(112, 144);
				t_ret[2] = new Vector2(244, 138);
			}
			else
			{
				t_ret[0] = new Vector2(192, 78);
				t_ret[1] = new Vector2(140, 144);
				t_ret[2] = new Vector2(8, 138);
			}
			return t_ret;
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player && CollisionManager.Collides(this.getHitBox(), a_collid.getHitBox()))
			{
				Player t_player = (Player)a_collid;
				if (t_player.getCurrentState() != Player.State.Hiding)
				{
					t_player.setIsInLight(true);
				}
				else
				{
					if (t_player.getHidingImage().Equals(Player.STANDHIDINGIMAGE))
					{
						if (t_player.isFacingRight() && t_player.getPosition().getGlobalX() + t_player.getHitBox().getOutBox().Width > m_parent.getPosition().getGlobalX())
						{
							t_player.setIsInLight(true);
						}
						else if (!t_player.isFacingRight() && t_player.getPosition().getGlobalX() < m_parent.getPosition().getGlobalX())
						{
							t_player.setIsInLight(true);
						}
					}
				}
			}
		}
	}
}
