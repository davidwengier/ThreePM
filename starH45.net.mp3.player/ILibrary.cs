using System;
using System.Collections.Generic;
using System.Text;

namespace starH45.net.mp3.player
{
	public interface ILibrary
	{
		void SetSupportedExtensions(string [] exts);

		int SongCount
		{
			get;
		}

		SongInfo GetSong(int index);

		SongInfo GetSong(string fileName);

        void UpdatePlayCount(string filename);

		void UpdatePlayDate(string filename);

		int GetPlayCount(SongInfo song);

		SongInfo GetRandomSong(bool returnIgnoredSongs, bool returnPreviouslyPlayedSongs);
	}
}
