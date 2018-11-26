using System;
using System.Collections.Generic;
using System.Text;

namespace ThreePM.Utilities
{
    internal interface ILyricsSiteHandler
    {
        string SiteName
        {
            get;
        }

        string GetSearchURL(MusicPlayer.SongInfo song);

        bool GetLyrics(string htmlPage, out string lyrics);

        LyricsSearchResults ProcessSearchResults(MusicPlayer.SongInfo song, string htmlPage, out string nextURL);
    }
}
