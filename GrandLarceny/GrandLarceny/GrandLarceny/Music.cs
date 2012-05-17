using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace GrandLarceny
{
	public class Music
	{
		private static Dictionary<string, Song> m_loadedMusic = new Dictionary<string, Song>();

		public static void loadSong(string a_path)
		{
			if (!m_loadedMusic.ContainsKey(a_path))
			{
				m_loadedMusic.Add(a_path, Game.getInstance().Content.Load<Song>("Sounds//Music//" + a_path));
			}
		}

		public static void unloadSong(string a_path)
		{
			m_loadedMusic.Remove(a_path);
		}

		public static void play(string a_music)
		{
			MediaPlayer.Play(m_loadedMusic[a_music]);
			MediaPlayer.IsRepeating = true;
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

		public static bool musicIsPlaying()
		{
			return MediaPlayer.State == MediaState.Playing;
		}
	}
}
