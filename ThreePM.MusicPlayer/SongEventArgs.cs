using System;

namespace ThreePM.MusicPlayer
{
    public class SongEventArgs : EventArgs
    {
        private readonly SongInfo _song;

        public SongInfo Song
        {
            get { return _song; }
        }

        public SongEventArgs(SongInfo song)
        {
            _song = song;
        }
    }
}
