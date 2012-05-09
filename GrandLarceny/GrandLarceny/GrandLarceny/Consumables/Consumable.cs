using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public abstract class Consumable : NonMovingObject
	{
		[NonSerialized]
		protected ImageManager m_bling;
		protected bool m_isBlinging = false;
		
		public Consumable(Vector2 a_position, String a_sprite, float a_layer)
			:base(a_position, a_sprite, a_layer)
		{

		}

		public override void loadContent()
		{
			base.loadContent();
			m_bling = new ImageManager("Images//Prop//Consumables//Sparkle");
			m_bling.setAnimationSpeed(20);
		}

		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			m_bling.update(a_gameTime);
		}

		public override void draw(GameTime a_gameTime)
		{
			base.draw(a_gameTime);
			if (m_isBlinging)
			{
				Vector2 t_imgPosition = m_position.getFlooredGlobalCartesian() + new Vector2(m_imgOffsetX, m_imgOffsetY);
				///*t_imgPosition.X = m_position.getGlobalX() + m_imgOffsetX;
				//t_imgPosition.Y = m_position.getGlobalY() + m_imgOffsetY;

				m_bling.draw(t_imgPosition, m_rotate, m_rotationPoint, m_color, m_spriteEffects, m_layer - 0.001f, m_XScale, m_YScale);
			}
		}

		public void toggleBling()
		{
			m_isBlinging = !m_isBlinging;
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player && !m_dead && collect())
			{
				m_dead = true;
			}
		}

		public override string ToString()
		{
			return base.ToString() + m_isBlinging;
		}

		abstract protected Boolean collect();
	}
}
