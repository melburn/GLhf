using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrandLarceny
{
	public class Sound
	{
		private Sound m_sound;

		public Sound(string a_path)
		{
			m_sound = Game.getInstance().Content.Load<Sound>(a_path);
		}

		public void play()
		{
			m_sound.play();
		}
	}
}
