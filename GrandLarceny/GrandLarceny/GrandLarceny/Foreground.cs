using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	public class Foreground : NonMovingObject
	{
		[NonSerialized]
		private LinkedList<Foreground> m_foregrounds;
		private LinkedList<int> m_foregroundsId;

		private bool m_visible;
		private bool m_fading;
		private float m_fadingTime;
		private bool m_fadingOut;
		private float m_currentfadingTime;

		public Foreground(Vector2 a_posV2, string a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			m_foregrounds = new LinkedList<Foreground>();
		}

		public override void loadContent()
		{
			base.loadContent();
			m_foregrounds = new LinkedList<Foreground>();
			m_foregroundsId = new LinkedList<int>();
			foreach (int t_i in m_foregroundsId)
			{
				m_foregrounds.AddLast((Foreground)Game.getInstance().getState().getObjectById(t_i));
			}
		}

		public override void linkObject()
		{
			base.linkObject();
			m_foregroundsId = new LinkedList<int>();
			foreach (Foreground t_fg in m_foregrounds)
			{
				m_foregroundsId.AddLast(t_fg.getId());
			}
		}

		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			float t_deltaTime = ((float)(a_gameTime.ElapsedGameTime.Milliseconds)) / 1000.0f;

			if (m_fadingTime > 0 && m_currentfadingTime == 0)
			{
				m_currentfadingTime = m_fadingTime;
			}

			if (m_currentfadingTime > 0)
			{
				m_currentfadingTime -= t_deltaTime;
				if (m_fadingOut)
				{
					byte tColor = (byte)((m_currentfadingTime / m_fadingTime) * 255);
					m_color.A = tColor;
					m_color.R = tColor;
					m_color.G = tColor;
					m_color.B = tColor;

					if (m_color.A <= 10)
					{
						m_color.A = 0;
						m_color.R = 0;
						m_color.G = 0;
						m_color.B = 0;
						m_fading = false;
						m_fadingTime = 0;
						m_currentfadingTime = 0;
					}
					
				}
				else
				{
					byte tColor = (byte)(255 - 255 * (m_currentfadingTime/m_fadingTime));
					m_color.A = tColor;
					m_color.R = tColor;
					m_color.G = tColor;
					m_color.B = tColor;
					if (m_color.A >= 245)
					{
						m_color.A = 255;
						m_color.R = 255;
						m_color.G = 255;
						m_color.B = 255;
						m_fading = false;
						m_fadingTime = 0;
						m_currentfadingTime = 0;
					}
				}
				
			}

			if (m_visible && m_fadingOut)
			{
				m_fading = true;
				m_visible = true;
				m_fadingOut = false;
				m_fadingTime = 1f;

			}
			else if (!m_visible && !m_fadingOut)
			{
				m_fading = true;
				m_fadingOut = true;
				m_fadingTime = 1f;
			}
			m_visible = true;
			
		}

		public override void draw(GameTime a_gameTime)
		{

			if (m_visible || m_fading)
			{
				base.draw(a_gameTime);
			}
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player && CollisionManager.Collides(this.getHitBox(), a_collid.getHitBox()))
			{
				m_visible = false;
				m_fading = true;

				foreach (Foreground t_fg in m_foregrounds)
				{
					t_fg.setVisible(false);
				}
				
			}
		}

		public void addForeGround(Foreground a_foreground, bool a_addBack)
		{
			LinkedList<Foreground> t_foreList = a_foreground.getForegrounds();
			bool found = false;
			foreach (Foreground t_fg in m_foregrounds)
			{
				t_foreList.Remove(t_fg);
				if (t_fg == a_foreground)
				{
					found = true;
				}
			}
			if (!found)
			{
				m_foregrounds.AddLast(a_foreground);
			}
			foreach (Foreground t_fg in t_foreList)
			{
				m_foregrounds.AddLast(t_fg);
			}

			if (a_addBack)
			{
				a_foreground.addForeGround(this, false);
			}
		}

		public void setVisible(bool a_visible)
		{
			m_visible = a_visible;
			m_fading = true;
		}

		public LinkedList<Foreground> getForegrounds()
		{
			return m_foregrounds;
		}
	}
}
