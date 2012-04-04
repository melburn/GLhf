using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GrandLarceny
{
	public class Sound
	{
		private SoundEffect m_sound;
		private string m_filepath;

		public Sound(string a_path)
		{
			m_filepath = a_path;
			loadContent();
		}

		public void loadContent()
		{
			m_sound = Game.getInstance().Content.Load<SoundEffect>("Sounds//" + m_filepath);
		}

		public void play()
		{
			m_sound.Play();
		}
	}
}
