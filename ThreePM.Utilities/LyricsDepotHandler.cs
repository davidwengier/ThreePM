using System.Text.RegularExpressions;

namespace ThreePM.Utilities
{
    internal class LyricsDepotHandler : ILyricsSiteHandler
    {
        public LyricsDepotHandler()
        {
        }

        public string SiteName
        {
            get { return "Lyrics Depot"; }
        }

        public string GetSearchURL(ThreePM.MusicPlayer.SongInfo song)
        {
            string artist = song.Artist.Replace(' ', '-').Replace("!", "").ToLower();

            return string.Format(@"http://www.lyricsdepot.com/{0}/", artist);
        }

        public bool GetLyrics(string htmlPage, out string lyrics)
        {
            lyrics = "";
            string regex = @"ringmatch\(\);\s+?--></script>\s+?(?<lyrics1>.*?)$";
            Match m = Regex.Match(htmlPage, regex, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (m.Groups["lyrics1"].Success)
            {
                lyrics = m.Groups["lyrics1"].Value;
            }

            return true;
        }

        public LyricsSearchResults ProcessSearchResults(ThreePM.MusicPlayer.SongInfo song, string htmlPage, out string nextURL)
        {
            // Songs:</b><br>.*<a href\=\"(?<url>.*?)\">Rhinestone Cowboy Lyrics</a>
            nextURL = "";

            string regex = LyricsHelper.GetSongTitleRegex(song);

            regex = "Songs:</b><br>.*<a href=\\\"(?<url>.*?)\\\">" + regex + " Lyrics</a>";

            Match m = Regex.Match(htmlPage, regex, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (m.Groups["url"].Success)
            {
                string url = m.Groups["url"].Value;

                url = "http://www.lyricsdepot.com" + url;

                nextURL = url;
                return LyricsSearchResults.Found;
            }
            return LyricsSearchResults.NotFound;
        }
    }
}
