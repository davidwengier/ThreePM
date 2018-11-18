using System;
using System.Collections.Generic;
using System.Text;

namespace ThreePM.Utilities
{
	public class LyricsFoundEventArgs : EventArgs
	{
		private string m_lyrics;

		public string Lyrics
		{
			get
			{
				return m_lyrics;
			}
		}

		public LyricsFoundEventArgs(string lyrics)
		{
			m_lyrics = lyrics;
		}
	}
}
