using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	class Darkness : Environment
	{
		public Darkness(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{
		}
		public override void loadContent()
		{
			base.loadContent();
			m_imgOffsetX = 36 -m_img.getSize().X / 2;
			m_imgOffsetY = 36 -m_img.getSize().Y / 2;
			m_position.setParentPosition(Game.getInstance().getState().getPlayer().getPosition());
		}
		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			m_position.setParentPosition(Game.getInstance().getState().getPlayer().getPosition());
		}

	}
}
