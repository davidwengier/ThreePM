using System;

namespace ThreePM.MusicPlayer
{
	public class AudioscrobblerException : Exception
	{
		public AudioscrobblerException(string message)
			: base(message)
		{
		}
	}
}
