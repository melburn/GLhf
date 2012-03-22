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

		public Foreground(Vector2 a_posV2, string a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
			
		}

		public override void loadContent()
		{
			base.loadContent();
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

		public override void draw(GameTime a_gameTime)
		{
			if (m_visible)
			{
				base.draw(a_gameTime);
			}
			else
			{
				m_visible = true;
			}
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player && CollisionManager.Collides(this.getHitBox(), a_collid.getHitBox()))
			{
				m_visible = false;
				foreach (Foreground t_fg in m_foregrounds)
				{
					t_fg.setVisible(false);
				}
			}

		}

		public void addForeGround(Foreground a_foreground)
		{
			m_foregrounds.AddLast(a_foreground);
		}

		public void setVisible(bool a_visible)
		{
			m_visible = a_visible;
		}
	}
}
