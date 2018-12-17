using System;
using System.Text.RegularExpressions;

namespace ThreePM.Utilities
{
    internal class LyricsWikiHandler : ILyricsSiteHandler
    {
        public LyricsWikiHandler()
        {
        }

        public string SiteName
        {
            get { return "Lyrics Wiki"; }
        }

        public string GetSearchURL(ThreePM.MusicPlayer.SongInfo song)
        {
            string artist = song.Artist.Replace(' ', '_');
            string title = song.Title.Replace(' ', '_');
            return string.Format(@"http://www.lyricwiki.org/api.php?action=lyrics&artist={0}&song={1}&fmt=xml", System.Web.HttpUtility.UrlEncode(artist), System.Web.HttpUtility.UrlEncode(title));
        }

        public bool GetLyrics(string htmlPage, out string lyrics)
        {
            // haha, new format!
            lyrics = Regex.Match(htmlPage, "<div class='lyricbox'><div class='rtMatcher'>.*?</div>(?<lyrics1>.*?)<!--").Groups["lyrics1"].Value.Replace("\n", "<BR>");
            if (lyrics == "")
            {
                lyrics = Regex.Match(htmlPage, "<div class='lyricbox'>(?<lyrics1>.*?)<!--").Groups["lyrics1"].Value.Replace("\n", "<BR>");
            }
            return true;
        }

        public LyricsSearchResults ProcessSearchResults(ThreePM.MusicPlayer.SongInfo song, string htmlPage, out string nextURL)
        {
            nextURL = "";
            if (htmlPage == "Not found")
            {
                return LyricsSearchResults.NotFound;
            }
            else if (!htmlPage.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
            {
                nextURL = Regex.Match(htmlPage, "<url>(?<url>.*?)</url>").Groups["url"].Value;
                return LyricsSearchResults.SearchAgain;
            }
            else
            {
                return LyricsSearchResults.FoundOnThisPage;
            }
        }
    }
}
