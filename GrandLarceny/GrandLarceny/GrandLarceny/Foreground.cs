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
		}

		public LinkedList<Foreground> getForegrounds()
		{
			return m_foregrounds;
		}
	}
}
