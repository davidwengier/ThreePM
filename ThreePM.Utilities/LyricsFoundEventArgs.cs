using System;

namespace ThreePM.Utilities
{
    public class LyricsFoundEventArgs : EventArgs
    {
        private readonly string _lyrics;

        public string Lyrics
        {
            get
            {
                return _lyrics;
            }
        }

        public LyricsFoundEventArgs(string lyrics)
        {
            _lyrics = lyrics;
        }
    }
}
