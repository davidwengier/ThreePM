using System.Text.RegularExpressions;

namespace ThreePM.Utilities
{
    internal class LeosLyricsHandler : ILyricsSiteHandler
    {
        public LeosLyricsHandler()
        {
        }

        public string SiteName
        {
            get { return "Leo's Lyrics"; }
        }

        public string GetSearchURL(ThreePM.MusicPlayer.SongInfo song)
        {
            return string.Format(@"http://www.leoslyrics.com/search.php?sartist=1&search={0}", System.Web.HttpUtility.UrlEncode(song.Artist));
        }

        public bool GetLyrics(string htmlPage, out string lyrics)
        {
            lyrics = "";
            string stringToFind = "<font face=\"Trebuchet MS, Verdana, Arial\" size=-1>";
            int startTag = htmlPage.IndexOf(stringToFind);
            if (startTag != -1)
            {
                lyrics = htmlPage.Substring(startTag + stringToFind.Length);

                startTag = lyrics.IndexOf("</font>");
                if (startTag != -1)
                {
                    lyrics = lyrics.Substring(0, startTag);
                }
                return true;
            }
            return false;
        }

        public LyricsSearchResults ProcessSearchResults(MusicPlayer.SongInfo song, string htmlPage, out string nextURL)
        {
            nextURL = "";

            string regex = LyricsHelper.GetSongTitleRegex(song);
            regex = "results.*<a href=\\\"(?<url>/listlyrics.*?)\\\"><b>" + regex;

            Match m = Regex.Match(htmlPage, regex, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (m.Groups["url"].Success)
            {
                string url = m.Groups["url"].Value;

                url = "http://www.leoslyrics.com" + url;

                nextURL = url;
                return LyricsSearchResults.Found;
            }
            return LyricsSearchResults.NotFound;
        }
    }
}
