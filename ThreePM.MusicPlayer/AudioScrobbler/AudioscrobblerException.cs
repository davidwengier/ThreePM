using System;

namespace ThreePM.MusicPlayer
{
    [Serializable]
    public class AudioscrobblerException : Exception
    {
        public AudioscrobblerException()
        {
        }

        public AudioscrobblerException(string message)
            : base(message)
        {
        }

        public AudioscrobblerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
