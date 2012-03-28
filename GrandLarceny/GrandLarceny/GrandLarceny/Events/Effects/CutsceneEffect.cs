using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny.Events.Effects
{
	[Serializable()]
	public class CutsceneEffect : EventEffect
	{
		private String m_cutsceneName;

		public CutsceneEffect(String a_cutsceneName)
		{
			if(a_cutsceneName == null)
			{
				throw new ArgumentNullException();
			}
			m_cutsceneName = a_cutsceneName;
		}

		public override void execute()
		{
			Game.getInstance().setCutscene(m_cutsceneName);
		}

		public override string ToString()
		{
			return "Cutscene: " + m_cutsceneName;
		}
	}
}
