using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace GrandLarceny
{
	public class Sound
	{
		private SoundEffect m_sound;

		public Sound(string a_path)
		{
			m_sound = Game.getInstance().Content.Load<SoundEffect>("Sounds//" + a_path);
		}

		public void play()
		{
			m_sound.Play();
		}
	}
}
