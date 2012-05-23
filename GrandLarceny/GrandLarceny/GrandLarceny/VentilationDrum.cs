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
		[NonSerialized]
		VentilationDrum m_pairedVentilation = null;
		[NonSerialized]
		ImageManager m_backImg;
		[NonSerialized]
		private static Sound s_unlockSound;

		int m_pairedVentilationId = 0;

		bool m_isLocked = false;

		public VentilationDrum(Vector2 a_posV2, String a_sprite, float a_layer)
			: base(a_posV2, a_sprite, a_layer)
		{

		}

		public override void loadContent()
		{
			base.loadContent();
			if (m_pairedVentilationId != 0 && m_pairedVentilation == null)
			{
				m_pairedVentilation = (VentilationDrum)Game.getInstance().getState().getObjectById(m_pairedVentilationId);
			}
			m_backImg = new ImageManager("Images//Tile//Ventilation//Straight//aventil_short_tile_w_floor");
			m_collisionShape = new CollisionRectangle(30, 30, 42, 12, m_position);
			if (s_unlockSound == null)
			{
				s_unlockSound = new Sound("Game//ledgegrab");
			}
		}
		public override void linkObject()
		{
			base.linkObject();

			m_pairedVentilation.setPairedVentilation(this);
		}
		internal override void updateCollisionWith(Entity a_collider)
		{
			if (a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				if (!m_isLocked)
				{
					if (t_player.getCurrentState() == Player.State.Ventilation && (KeyboardHandler.keyClicked(GameState.getUpKey()) || KeyboardHandler.keyClicked(GameState.getJumpKey()) || KeyboardHandler.keyClicked(GameState.getDownKey())))
					{
						if (Game.getInstance().m_camera.getLayer() == 0)
						{
							Game.getInstance().getState().moveObjectToLayer(t_player, 0);
							t_player.setNextPosition(m_position.getGlobalCartesian());
							t_player.setState(Player.State.Jumping);
						}
						else
						{
							Game.getInstance().m_camera.setLayer(0);
						}
						t_player.setCollidedWithVent(true);
						return;
					}

					if (!t_player.isStunned())
					{
						if (KeyboardHandler.keyClicked(GameState.getActionKey()))
						{
							if (Game.getInstance().m_camera.getLayer() == 0)
							{
								Game.getInstance().getState().changeLayer(1);
								t_player.setState(Player.State.Ventilation);
								t_player.setNextPosition(m_position.getGlobalCartesian());
								t_player.deactivateChaseMode();
								t_player.setSpeedX(0);
								t_player.setSpeedY(0);
							}
						}
						else
						{
							t_player.setInteractionVisibility(true);
						}
					}
				}
				else
				{
					if(t_player.getCurrentState() != Player.State.Jumping)
					{
						if (KeyboardHandler.keyClicked(GameState.getActionKey()))
						{
							s_unlockSound.play();
							toggleLocked();
						}
						else
						{
							t_player.setInteractionVisibility(true);
						}
					}
				}
				t_player.setCollidedWithVent(true);
			}
		}
		public void toggleLocked()
		{
			if (m_pairedVentilation != null)
			{
				m_pairedVentilation.setPairedVentilation(null);
				m_pairedVentilation.toggleLocked();
				m_pairedVentilation.setPairedVentilation(this);
			}
			m_isLocked = !m_isLocked;
			if (m_isLocked)
				m_img.setSprite("Images//Tile//Ventilation//Drum//ventil_locked");
			else
				m_img.setSprite("Images//Tile//Ventilation//Drum//ventil");
		}
		public bool isLocked()
		{
			return m_isLocked;
		}
		public void setPairedVentilation(VentilationDrum a_ventilation)
		{
			m_pairedVentilation = a_ventilation;
			if (a_ventilation != null)
				m_pairedVentilationId = a_ventilation.getId();
			else
				m_pairedVentilationId = 0;
		}
		public VentilationDrum getPairedVentilation()
		{
			return m_pairedVentilation;
		}
		public override void kill()
		{
			if (m_pairedVentilation != null)
			{
				((DevelopmentState)Game.getInstance().getState()).removeObject(m_pairedVentilation);
			}
			base.kill();
		}

		public override string ToString()
		{
			if (m_isLocked)
				return base.ToString() + ": Closed";
			else
				return base.ToString() + ": Open";
		}
		public override void draw(GameTime a_gameTime)
		{
			base.draw(a_gameTime);

			Vector2 t_imgPosition = m_position.getFlooredGlobalCartesian() + new Vector2(m_imgOffsetX, m_imgOffsetY);
			m_backImg.draw(t_imgPosition, m_rotate, m_rotationPoint, m_color, m_spriteEffects, 0.930f, m_XScale, m_YScale);
		}
	}
}
