using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class Consumable : NonMovingObject
	{
		[NonSerialized]
		protected ImageManager m_bling;
		protected bool m_isBlinging = false;
		[NonSerialized]
		protected Sound m_collectSound;
		
		public Consumable(Vector2 a_position, String a_sprite, float a_layer)
			:base(a_position, a_sprite, a_layer)
		{

		}

		public override void loadContent()
		{
			base.loadContent();
			m_bling = new ImageManager("Images//Prop//Consumables//Sparkle");
			m_bling.setAnimationSpeed(18);
			m_collectSound = new Sound("Game/Tagrej");
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
				float t_XScale = (m_XScale * m_img.getSize().X) / m_bling.getSize().X;
				float t_YScale = (m_YScale * m_img.getSize().Y) / m_bling.getSize().Y;
				m_bling.draw(t_imgPosition, m_rotate, m_rotationPoint, m_color, m_spriteEffects, m_layer - 0.001f, t_XScale, t_YScale);
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
				if (m_collectSound != null)
				{
					m_collectSound.play();
				}
			}
		}

		public override string ToString()
		{
			return base.ToString() + m_isBlinging;
		}

		virtual protected Boolean collect()
		{
			return true;
		}
	}
}
