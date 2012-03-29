using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class Rope : NonMovingObject
	{
		[NonSerialized()]
		private Line m_line;

		public Rope(Vector2 a_posV2, string a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{

		}
		public override void loadContent()
		{
			base.loadContent();
			m_line = new Line(m_position, m_position, new Vector2(36, 0), new Vector2(36, 72), Color.Black, 5, true);
			m_collisionShape = new CollisionRectangle(0, 0, 72, 72, m_position);
			m_rotationPoint = new Vector2(2.5f, m_position.getGlobalY());
		}

		public override void update(GameTime a_gameTime)
		{
			base.update(a_gameTime);
		}

		public override void draw(GameTime a_gameTime)
		{
			m_line.draw();
		}

		internal override void updateCollisionWith(Entity a_collid)
		{
			if (a_collid is Player)
			{
				Player t_player = (Player)a_collid;
				t_player.setState(Player.State.Swinging);
				t_player.getPosition().setParentPositionWithoutMoving(m_position);
				t_player.changePositionType();
				t_player.setRope(this);
			}
		}
	}
}
