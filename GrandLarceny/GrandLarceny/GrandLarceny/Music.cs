using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace GrandLarceny
{
	class Music
	{
		private string m_filepath;
		private Song m_music;

		public Music(string a_filepath)
		{
			m_filepath = a_filepath;
			loadContent();
		}

		public void loadContent()
		{
			m_music = Game.getInstance().Content.Load<Song>("Sounds//Music//" + m_filepath);		
		}

		public void play()
		{
			MediaPlayer.Play(m_music);
		}

		public static void stop()
		{
			MediaPlayer.Stop();
		}

		public static void togglePause()
		{
			if (MediaPlayer.State == MediaState.Paused)
			{
				MediaPlayer.Resume();
			}
			else
			{
				MediaPlayer.Pause();
			}
		}
	}
}
