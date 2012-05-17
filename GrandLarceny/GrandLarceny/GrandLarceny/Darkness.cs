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
			loadContent();
		}
		public override void loadContent()
		{
			base.loadContent();
			m_imgOffsetX = 36 -m_img.getSize().X / 2;
			m_imgOffsetY = 36 -m_img.getSize().Y / 2;
			m_position = new CartesianCoordinate(Game.getInstance().getState().getPlayer().getPosition().getGlobalCartesian());
		}
		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
			m_position.setGlobalCartesian(Vector2.Lerp(m_position.getGlobalCartesian(),Game.getInstance().getState().getPlayer().getPosition().getGlobalCartesian(), ((float)a_gameTime.ElapsedGameTime.Milliseconds) / 100f));
		}

	}
}
