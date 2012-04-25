using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace starH45.net.mp3.library
{
	public class LibraryEntry : starH45.net.mp3.player.SongInfo
	{
		#region Declarations

		private int m_playCount;
		private string m_lyrics;

		#endregion

		#region Properties

		public string Lyrics
		{
			get { return m_lyrics; }
		}

		public int PlayCount
		{
			get { return m_playCount; }
		}

		#endregion

		#region Constructor

		public LibraryEntry(string file) : base(file)
		{
			ComputeToStringString();
		}

		public LibraryEntry(string fileName, string artist, string album, string title, int trackNumber, int year, string genre, string albumArtist)
			: base(fileName, artist, album, title, trackNumber, year, 0, genre, albumArtist, false)
		{
			ComputeToStringString();
		}

		public LibraryEntry(string fileName, string artist, string album, string title, int trackNumber, int year, string genre, string albumArtist, string lyrics)
			: base(fileName, artist, album, title, trackNumber, year, 0, genre, albumArtist, false)
		{
			m_lyrics = lyrics;
			ComputeToStringString();
		}

		internal LibraryEntry(DataRow dr)
		{
			if (dr["Artist"] != DBNull.Value)
			{
				m_artist = Convert.ToString(dr["Artist"]);
			}
			if (dr["Title"] != DBNull.Value)
			{
				m_title = Convert.ToString(dr["Title"]);
			}
			if (dr["Album"] != DBNull.Value)
			{
				m_album = Convert.ToString(dr["Album"]);
			}
			if (dr["TrackNumber"] != DBNull.Value)
			{
				m_trackNumber = Convert.ToInt32(dr["TrackNumber"]);
			}
			if (dr["Year"] != DBNull.Value)
			{
				m_year = Convert.ToInt32(dr["Year"]);
			}
			if (dr["PlayCount"] != DBNull.Value)
			{
				m_playCount = Convert.ToInt32(dr["PlayCount"]);
			}
			if (dr["Genre"] != DBNull.Value)
			{
				m_genre = Convert.ToString(dr["Genre"]);
			}
			if (dr["Filename"] != DBNull.Value)
			{
				m_fileName = Convert.ToString(dr["Filename"]);
			}
			if (dr["AlbumArtist"] != DBNull.Value)
			{
				m_albumArtist = Convert.ToString(dr["AlbumArtist"]);
			}
			if (dr["Duration"] != DBNull.Value)
			{
				m_duration = Convert.ToSingle(dr["Duration"]);
			}
			if (dr["Ignored"] != DBNull.Value)
			{
				m_ignored = Convert.ToBoolean(dr["Ignored"]);
			}
			if (dr["Lyrics"] != DBNull.Value)
			{
				m_lyrics = Convert.ToString(dr["Lyrics"]);
			}

			ComputeToStringString(); 
		}

		#endregion

		#region Public Methods

		#endregion

		#region Overridden Methods

		#endregion Overridden Methods
	}
}
