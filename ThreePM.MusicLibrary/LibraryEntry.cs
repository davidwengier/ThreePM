using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace ThreePM.MusicLibrary
{
    public class LibraryEntry : ThreePM.MusicPlayer.SongInfo
    {
        #region Declarations

        private readonly int _playCount;
        private readonly string _lyrics;

        #endregion

        #region Properties

        public string Lyrics
        {
            get { return _lyrics; }
        }

        public int PlayCount
        {
            get { return _playCount; }
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
            _lyrics = lyrics;
            ComputeToStringString();
        }

        internal LibraryEntry(DataRow dr)
        {
            if (dr["Artist"] != DBNull.Value)
            {
                _artist = Convert.ToString(dr["Artist"]);
            }
            if (dr["Title"] != DBNull.Value)
            {
                _title = Convert.ToString(dr["Title"]);
            }
            if (dr["Album"] != DBNull.Value)
            {
                _album = Convert.ToString(dr["Album"]);
            }
            if (dr["TrackNumber"] != DBNull.Value)
            {
                _trackNumber = Convert.ToInt32(dr["TrackNumber"]);
            }
            if (dr["Year"] != DBNull.Value)
            {
                _year = Convert.ToInt32(dr["Year"]);
            }
            if (dr["PlayCount"] != DBNull.Value)
            {
                _playCount = Convert.ToInt32(dr["PlayCount"]);
            }
            if (dr["Genre"] != DBNull.Value)
            {
                _genre = Convert.ToString(dr["Genre"]);
            }
            if (dr["Filename"] != DBNull.Value)
            {
                _fileName = Convert.ToString(dr["Filename"]);
            }
            if (dr["AlbumArtist"] != DBNull.Value)
            {
                _albumArtist = Convert.ToString(dr["AlbumArtist"]);
            }
            if (dr["Duration"] != DBNull.Value)
            {
                _duration = Convert.ToSingle(dr["Duration"]);
            }
            if (dr["Ignored"] != DBNull.Value)
            {
                _ignored = Convert.ToBoolean(dr["Ignored"]);
            }
            if (dr["Lyrics"] != DBNull.Value)
            {
                _lyrics = Convert.ToString(dr["Lyrics"]);
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
