using System;
using System.Collections.Generic;
using System.Text;

namespace starH45.net.mp3.utilities
{
	internal interface ILyricsSiteHandler
	{
		string SiteName
		{
			get;
		}

		string GetSearchURL(player.SongInfo song);

		bool GetLyrics(string htmlPage, out string lyrics);

		LyricsSearchResults ProcessSearchResults(player.SongInfo song, string htmlPage, out string nextURL);
	}
}
