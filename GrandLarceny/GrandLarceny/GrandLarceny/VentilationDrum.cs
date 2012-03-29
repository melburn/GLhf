using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrandLarceny
{
	[Serializable()]
	class VentilationDrum : NonMovingObject
	{
		public VentilationDrum(Vector2 a_posV2, String a_sprite, float a_layer) : base(a_posV2, a_sprite, a_layer)
		{
			
		}
		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				if (Game.keyClicked(GameState.getActionKey()) && !t_player.isStunned())
					if (Game.getInstance().m_camera.getLayer() == 0)
					{
						Game.getInstance().getState().changeLayer(1);
						t_player.setState(Player.State.Ventilation);
						t_player.setNextPosition(m_position.getGlobalCartesianCoordinates());
						t_player.deactivateChaseMode();
						t_player.setSpeedX(0);
						t_player.setSpeedY(0);
					}
					else if (Game.getInstance().m_camera.getLayer() == 1)
					{
						Game.getInstance().getState().changeLayer(0);
						t_player.setState(Player.State.Stop);

					}
			}
		}
	}
}
