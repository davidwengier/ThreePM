using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ThreePM.Utilities
{
	internal class LyricsManiaHandler : ILyricsSiteHandler
	{
		public LyricsManiaHandler()
		{
		}

		public string SiteName
		{
			get { return "Lyrics Mania"; }
		}

		public string GetSearchURL(ThreePM.MusicPlayer.SongInfo song)
		{
			string artist = song.Artist;
			if (artist.StartsWith("The "))
			{
				artist = artist.Substring(4) + " (The)";
			}
			return String.Format(@"http://www.lyricsmania.com/search.php?c=artist&k={0}", System.Web.HttpUtility.UrlEncode(artist));
		}

		public bool GetLyrics(string htmlPage, out string lyrics)
		{
			lyrics = "";
			int start = htmlPage.IndexOf("The lyrics you requested is not in our archive yet");
			if (start != -1)
			{
				return false;
			}
			else
			{
				string regex = @"<h3>(?<songtitle>.*)</h3>\W*?<table.*?</table>\W*?(?:<br>)(?:<br>)?[\s]*(.*?)<br><br>(?<lyrics>.*?)(?:<br><br>\W*?<center)";
				lyrics = Regex.Match(htmlPage, regex, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase).Groups["lyrics"].Value;
				return true;
			}
		}

		public LyricsSearchResults ProcessSearchResults(ThreePM.MusicPlayer.SongInfo song, string htmlPage, out string nextURL)
		{
			nextURL = "";

			int start = htmlPage.IndexOf(" artists found:");
			if (start != -1)
			{
				// found more than one.
				start = htmlPage.IndexOf("title=\"" + song.Artist + " lyrics\"");
				if (start != -1)
				{
					start = htmlPage.LastIndexOf("/lyrics", start);
					int end = htmlPage.IndexOf("\" title", start);
					string url = "http://www.lyricsmania.com" + htmlPage.Substring(start, end - start);
					nextURL = url;
					return LyricsSearchResults.SearchAgain;
				}
			}
			else
			{
				string regex = LyricsHelper.GetSongTitleRegex(song);

				regex = "(?<=title=\\\")" + regex;

				start = System.Text.RegularExpressions.Regex.Match(htmlPage, regex, RegexOptions.IgnoreCase).Index;

				// if the regex didnt work, try old fashioned
				if (start <= 0)
				{
					System.Diagnostics.Debug.WriteLine("Old fashioned!!");
					start = htmlPage.IndexOf("title=\"" + song.Title + " lyrics\"");
				}

				if (start != -1)
				{
					start = htmlPage.LastIndexOf("/lyrics", start);
					int end = htmlPage.IndexOf("\" title", start);
					if (end != -1)
					{
						string url = htmlPage.Substring(start, end - start);
						nextURL = "http://www.lyricsmania.com" + url;
						return LyricsSearchResults.Found;
					}
				}
			}

			return LyricsSearchResults.NotFound;
		}
	}
}
