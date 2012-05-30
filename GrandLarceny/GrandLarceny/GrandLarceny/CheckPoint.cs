using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace GrandLarceny
{
	[Serializable()]
	public class CheckPoint : NonMovingObject
	{
		[NonSerialized]
		private Particle m_feedback;

		private bool m_hasSaved = true;

		public CheckPoint(Vector2 a_position, String a_sprite, float a_layer, float a_rotation = 0)
			: base(new CartesianCoordinate(a_position), a_sprite, a_layer, a_rotation)
		{
			m_hasSaved = true;
		}

		public override void draw(GameTime a_gameTime)
		{
			if (Game.getInstance().getState() is DevelopmentState)
			{
				base.draw(a_gameTime);
			}

			/*if (m_feedback != null)
			{
				m_feedback.draw(a_gameTime);
				if (m_feedback.isDead())
				{
					m_feedback = null;
				}
			}*/
		}

		internal override void updateCollisionWith(Entity a_collider)
		{
			if (m_hasSaved && a_collider is Player)
			{
				Player t_player = (Player)a_collider;
				if(CollisionManager.Collides(this.getHitBox(), a_collider.getHitBox()))
				{
					//if (KeyboardHandler.isKeyPressed(GameState.getActionKey()) && Game.getInstance().getState() is GameState)
					//{
						Level tLevel = new Level();
						tLevel.setLevelObjects(Game.getInstance().getState().getObjectList());
						tLevel.setEvents(((GameState)Game.getInstance().getState()).getEvents());

						Serializer.getInstance().SaveLevel(Game.getInstance().getCheckPointLevel(true), tLevel);
						Serializer.getInstance().saveGame(Game.getInstance().getCheckPointProgress(true), Game.getInstance().getProgress());
						m_hasSaved = false;


						String t_textureName = "Images//GUI//GameGUI//checkpoint";
						m_feedback = new Particle(new CartesianCoordinate(Vector2.Zero, Game.getInstance().m_camera.getPosition()), t_textureName, 33, 0.0015f);
						m_feedback.getPosition().setLocalCartesian(new Vector2(0, -100) - m_feedback.getImg().getSize() / 2);
						m_feedback.setTimer(((float)Game.getInstance().getTotalGameTime().TotalMilliseconds) + 3000f);
						Game.getInstance().getState().addObject(m_feedback);
						
					/*}
					else
					{
						t_player.setInteractionVisibility(true);
					}*/
				}
			}
		}
	}
}