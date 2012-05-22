using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrandLarceny
{
	[Serializable()]
	public class ConsumableGoal : Consumable
	{
		[NonSerialized]
		private Particle m_feedback;

		public ConsumableGoal(Vector2 a_position, String a_sprite, float a_layer)
			:base(a_position, a_sprite, a_layer)
		{

		}

		protected override bool collect()
		{
			int t_num = 4 - ((GameState)Game.getInstance().getState()).numberOfGoals();
			if (t_num > 0 && t_num < 4)
			{
				String t_textureName = "Images//GUI//GameGUI//stolen"+t_num+"of3";
				m_feedback = new Particle(new CartesianCoordinate(Vector2.Zero, Game.getInstance().m_camera.getPosition()), t_textureName, 33, 0.0015f);
				m_feedback.getPosition().setLocalCartesian(new Vector2(0,-200)-m_feedback.getImg().getSize() / 2);
				m_feedback.setTimer(((float)Game.getInstance().getTotalGameTime().TotalMilliseconds) + 3000f);
				Game.getInstance().getState().addObject(m_feedback);
			}
			new Sound("SecHit").play();
			return true;
		}

		public GameObject getFeedback()
		{
			return m_feedback;
		}
	}
}
