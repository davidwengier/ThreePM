using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace starH45.net.mp3.utilities
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

		public string GetSearchURL(starH45.net.mp3.player.SongInfo song)
		{
			string artist = song.Artist.Replace(' ', '_');
			string title = song.Title.Replace(' ', '_');
			return String.Format(@"http://www.lyricwiki.org/api.php?action=lyrics&artist={0}&song={1}&fmt=xml", System.Web.HttpUtility.UrlEncode(artist), System.Web.HttpUtility.UrlEncode(title));
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

		public LyricsSearchResults ProcessSearchResults(starH45.net.mp3.player.SongInfo song, string htmlPage, out string nextURL)
		{
			nextURL = "";  
			if (htmlPage == "Not found")
			{
				return LyricsSearchResults.NotFound;
			}
			else if (!htmlPage.StartsWith("<!DOCTYPE", StringComparison.InvariantCultureIgnoreCase))
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
