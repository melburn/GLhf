using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GrandLarceny.AI
{
	[Serializable()]
	class AIStateBark : AIState
	{
		//hur långt ifrån hunden vakterna hör hundens skall.
		static public float BARKRADIUS = 1000;

		//hur långt ifrån spelaren hunden måste vara innan han kan börja skälla.
		static public float BARKDISTANCE = 300;

		static private AIStateBark s_instance;
		private AIStateBark() { }
		static public AIStateBark getInstance()
		{
			if (s_instance == null)
			{
				s_instance = new AIStateBark();
			}
			return s_instance;
		}

		public override AIState execute(NPE a_agent, GameTime a_gameTime)
		{
			if (a_agent is GuardDog)
			{
				GuardDog t_guardDog = (GuardDog)a_agent;
				Player t_player = Game.getInstance().getState().getPlayer();
				if (t_player.getCurrentState() == Player.State.Hiding)
				{
					if (!t_guardDog.isChargeing())
					{
						t_guardDog.setChargeing(true);
					}
					if (t_guardDog.getPosition().getGlobalX() < t_player.getPosition().getGlobalX() - BARKDISTANCE)
					{
						if (t_guardDog.getHorizontalSpeed() <= 0)
						{
							t_guardDog.goRight();
						}
					}
					else if (t_guardDog.getPosition().getGlobalX() > t_player.getPosition().getGlobalX() + BARKDISTANCE)
					{
						if (t_guardDog.getHorizontalSpeed() >= 0)
						{
							t_guardDog.goLeft();
						}
					}
					else if (! t_guardDog.isBarking())
					{
						t_guardDog.startBarking();
					}
					return this;
				}
				else
				{
					if (t_guardDog.canSensePlayer())
					{
						t_guardDog.setChargePoint(Math.Sign(Game.getInstance().getState().getPlayer().getPosition().getGlobalX()
							- t_guardDog.getPosition().getGlobalX()) * AIStateChargeing.CHARGEDISTANCE
							+ Game.getInstance().getState().getPlayer().getPosition().getGlobalX());
						return AIStateChargeing.getInstance();
					}
					else
					{
						t_guardDog.forgetChaseTarget();
						return AIStatepatroling.getInstance();
					}
				}
			}
			else
			{
				throw new ArgumentException("Only dogs can bark");
			}
		}
	}
}
