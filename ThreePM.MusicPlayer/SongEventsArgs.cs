using System;
using System.Collections.Generic;
using System.Text;

namespace ThreePM.player
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
