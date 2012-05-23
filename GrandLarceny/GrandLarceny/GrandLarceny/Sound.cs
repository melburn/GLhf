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
		private SoundEffectInstance m_soundInstance;
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
			m_soundInstance = m_sound.CreateInstance();
		}

		public void play()
		{
			m_soundInstance.Pitch = 0.0f;
			m_soundInstance.Volume = m_volume;
			m_soundInstance.Pan = 0.0f;
			m_soundInstance.Play();
		}
		public void stop()
		{
			m_soundInstance.Stop();
		}

		public void setVolume(int a_volume)
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
		public void setLooping(bool a_looping)
		{
			m_soundInstance.IsLooped = a_looping;
		}

		public bool isPlaying()
		{
			return m_soundInstance.State == SoundState.Playing;
		}
	}
}
