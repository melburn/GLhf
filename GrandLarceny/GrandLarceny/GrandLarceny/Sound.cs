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
		private static float m_volume = 1.0f;

		public Sound(string a_path)
		{
			m_filepath = a_path;
			loadContent();
		}

		public void loadContent()
		{
			m_sound = Game.getInstance().Content.Load<SoundEffect>("Sounds//SoundEffects//" + m_filepath);
		}

		public void play()
		{
			m_sound.Play(m_volume, 0.0f, 0.0f);
		}

		public static void setVolume(int a_volume)
		{
			if (a_volume < 0)
			{
				a_volume = 0;
			}
			if (a_volume > 100)
			{
				a_volume = 100;
			}

			m_volume = (float)(a_volume / 100.0f);
		}
	}
}
