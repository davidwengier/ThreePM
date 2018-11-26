using System;
using System.Collections.Generic;
using System.Text;

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
