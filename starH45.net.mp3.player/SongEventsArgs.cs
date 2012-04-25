using System;
using System.Collections.Generic;
using System.Text;

namespace starH45.net.mp3.player
{
    public class SongEventsArgs : EventArgs
    {
        private SongInfo m_song;

        public SongInfo Song
        {
            get { return m_song; }
        }

        public SongEventsArgs(SongInfo song)
        {
            m_song = song;
        }
    }
}
