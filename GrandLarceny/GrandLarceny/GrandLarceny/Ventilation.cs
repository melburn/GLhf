using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny
{
	[Serializable()]
	class Ventilation : NonMovingObject
	{
		internal static List<Player.Direction> m_upDownList = new List<Player.Direction>() { Player.Direction.Up, Player.Direction.Down };
		internal static List<Player.Direction> m_leftRightList = new List<Player.Direction>() { Player.Direction.Left, Player.Direction.Right };
		internal static List<Player.Direction> m_upLeftList = new List<Player.Direction>() { Player.Direction.Up, Player.Direction.Left };
		internal static List<Player.Direction> m_upRightList = new List<Player.Direction>() { Player.Direction.Up, Player.Direction.Right };
		internal static List<Player.Direction> m_leftDownList = new List<Player.Direction>() { Player.Direction.Left, Player.Direction.Down };
		internal static List<Player.Direction> m_rightDownList = new List<Player.Direction>() { Player.Direction.Right, Player.Direction.Down };
		internal static List<Player.Direction> m_upLeftDownList = new List<Player.Direction>() { Player.Direction.Up, Player.Direction.Left, Player.Direction.Down };
		internal static List<Player.Direction> m_upRightDownList = new List<Player.Direction>() { Player.Direction.Up, Player.Direction.Right, Player.Direction.Down };
		internal static List<Player.Direction> m_upLeftRightList = new List<Player.Direction>() { Player.Direction.Up, Player.Direction.Left, Player.Direction.Right };
		internal static List<Player.Direction> m_leftRightDownList = new List<Player.Direction>() { Player.Direction.Right, Player.Direction.Left, Player.Direction.Down };
		internal static List<Player.Direction> m_upleftRightDownList = new List<Player.Direction>() { Player.Direction.Up, Player.Direction.Left, Player.Direction.Right, Player.Direction.Down };
		public Ventilation(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
		}
		public override void loadContent()
		{
			base.loadContent();
			m_collisionShape = new CollisionRectangle(-10,
						-10, 20, 20, m_position);
		}
	}
}
