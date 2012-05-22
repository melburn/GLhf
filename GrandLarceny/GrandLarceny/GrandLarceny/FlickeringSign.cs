using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class FlickeringSign : Platform
	{
		private int m_blinkTime;
		private float m_TimeToBlink;

		public FlickeringSign(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
		}

		public override void loadContent()
		{
			base.loadContent();
			m_img.stop();
			m_blinkTime = 0;
		}

		public override void update(GameTime a_gameTime)
		{
			if (m_blinkTime == 0)
			{
				m_TimeToBlink = a_gameTime.TotalGameTime.Milliseconds + Game.getInstance().getRandom().Next(10) * 100;
				m_blinkTime--;
			}
			if(m_TimeToBlink < a_gameTime.TotalGameTime.Milliseconds)
			{
				m_blinkTime = Game.getInstance().getRandom().Next(8)+1;
			}
			if (m_blinkTime > 0)
			{
				if (m_img.getSubImageIndex() == 0)
				{
					m_img.setSubImage(1);
				}
				else
				{
					m_img.setSubImage(0);
				}
				m_blinkTime--;
			}
			base.update(a_gameTime);
		}

	}
}
