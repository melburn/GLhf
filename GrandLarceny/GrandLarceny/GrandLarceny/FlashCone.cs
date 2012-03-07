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
		public FlashCone(Guard a_parent, Vector2 a_offset, string a_sprite, float a_layer) :
			base(new CartesianCoordinate(a_offset, a_parent.getPosition()), a_sprite, a_layer)
		{
			m_parent = a_parent;
			m_img.setAnimationSpeed(0);
		}

		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			m_dead = m_parent == null || m_parent.isDead();
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
		}

		public void setSprite(string a_path)
		{
			m_img.setSprite(a_path);
		}

		public void setSubImage(float a_index)
		{
			m_img.m_subImageNumber = a_index;
		}
	}
}
