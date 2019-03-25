using System;
using ThreePM.MusicLibrary;

namespace ThreePM
{
    public partial class InfoEditForm : BaseForm
    {
        #region Genres
        /// <summary>
        /// A collection of all of the genre string values in which the key of
        /// the genre its integer value for ID3v1 encoding.
        /// </summary>
        private static readonly string[] s_genres = new string[] { "Blues",         // 0
                                             "Classic Rock",
                                             "Country",
                                             "Dance",
                                             "Disco",
                                             "Funk",
                                             "Grunge",
                                             "Hip-Hop",
                                             "Jazz",
                                             "Metal",
                                             "New Age",                    // 10
                                             "Oldies",
                                             "Other",
                                             "Pop",
                                             "R&B",
                                             "Rap",
                                             "Reggae",
                                             "Rock",
                                             "Techno",
                                             "Industrial",
                                             "Alternative",                // 20
                                             "Ska",
                                             "Death Metal",
                                             "Pranks",
                                             "Soundtrack",
                                             "Euro-Techno",
                                             "Ambient",
                                             "Trip-Hop",
                                             "Vocal",
                                             "Jazz+Funk",
                                             "Fusion",                    // 30
                                             "Trance",
                                             "Classical",
                                             "Instrumental",
                                             "Acid",
                                             "House",
                                             "Game",
                                             "Sound Clip",
                                             "Gospel",
                                             "Noise",
                                             "AlternRock",                // 40
                                             "Bass",
                                             "Soul",
                                             "Punk",
                                             "Space",
                                             "Meditative",
                                             "Instrumental Pop",
                                             "Instrumental Rock",
                                             "Ethnic",
                                             "Gothic",
                                             "Darkwave",                // 50
                                             "Techno-Industrial",
                                             "Electronic",
                                             "Pop-Folk",
                                             "Eurodance",
                                             "Dream",
                                             "Southern Rock",
                                             "Comedy",
                                             "Cult",
                                             "Gangsta",
                                             "Top 40",                    // 60
                                             "Christian Rap",
                                             @"Pop/Funk",
                                             "Jungle",
                                             "Native American",
                                             "Cabaret",
                                             "New Wave",
                                             "Psychadelic",
                                             "Rave",
                                             "Showtunes",
                                             "Trailer",                    // 70
                                             "Lo-Fi",
                                             "Tribal",
                                             "Acid Punk",
                                             "Acid Jazz",
                                             "Polka",
                                             "Retro",
                                             "Musical",
                                             "Rock & Roll",
                                             "Hard Rock",
                                             "Folk",                    // 80
                                             "Folk-Rock",
                                             "National Folk",
                                             "Swing",
                                             "Fast Fusion",
                                             "Bebob",
                                             "Latin",
                                             "Revival",
                                             "Celtic",
                                             "Bluegrass",
                                             "Avantgarde",                // 90
                                             "Gothic Rock",
                                             "Progressive Rock",
                                             "Psychedelic Rock",
                                             "Symphonic Rock",
                                             "Slow Rock",
                                             "Big Band",
                                             "Chorus",
                                             "Easy Listening",
                                             "Acoustic",
                                             "Humour",                    // 100
                                             "Speech",
                                             "Chanson",
                                             "Opera",
                                             "Chamber Music",
                                             "Sonata",
                                             "Symphony",
                                             "Booty Bass",
                                             "Primus",
                                             "Porn Groove",
                                             "Satire",                    // 110
                                             "Slow Jam",
                                             "Club",
                                             "Tango",
                                             "Samba",
                                             "Folklore",
                                             "Ballad",
                                             "Power Ballad",
                                             "Rhythmic Soul",
                                             "Freestyle",
                                             "Duet",                    // 120
                                             "Punk Rock",
                                             "Drum Solo",
                                             "Acapella",
                                             "Euro-House",
                                             "Dance Hall",
                                             "Goa",
                                             "Drum & Bass",
                                             "Club-House",
                                             "Hardcore",
                                             "Terror",                    // 130
                                             "Indie",
                                             "BritPop",
                                             "Negerpunk",
                                             "Polsk Punk",
                                             "Beat",
                                             "Christian Gangsta Rap",
                                             "Heavy Metal",
                                             "Black Metal",
                                             "Crossover",
                                             "Contemporary Christian",    // 140
                                             "Christian Rock",
                                             "Merengue",
                                             "Salsa",
                                             "Thrash Metal",
                                             "Anime",
                                             "JPop",
                                             "Synthpop" };          // 147
        #endregion

        private LibraryEntry[] _libraryEntries;

        public InfoEditForm(LibraryEntry[] libraryEntries) : base()
        {
            InitializeComponent();

            int m = 1;

            _libraryEntries = libraryEntries;

            string title = _libraryEntries[0].Title;
            String album = _libraryEntries[0].Album;
            String artist = _libraryEntries[0].Artist;
            var genre = _libraryEntries[0].Genre;
            var albumArtist = _libraryEntries[0].AlbumArtist;
            int trackNumber = _libraryEntries[0].TrackNumber;
            int year = _libraryEntries[0].Year;

            foreach (string s in s_genres)
            {
                cboGenre.Items.Add(s);
            }

            foreach (LibraryEntry song in _libraryEntries)
            {
                if (title != song.Title)
                {
                    title = "";
                }

                if (album != song.Album)
                {
                    album = "";
                }

                if (artist != song.Artist)
                {
                    artist = "";
                }

                if (trackNumber != song.TrackNumber)
                {
                    trackNumber = 0;
                }

                if (genre != song.Genre)
                {
                    genre = "";
                }

                if (albumArtist != song.AlbumArtist)
                {
                    albumArtist = "";
                }

                if (year != song.Year)
                {
                    year = 0;
                }

            }

            txtTitle.Text = title;
            txtAlbum.Text = album;
            txtArtist.Text = artist;
            if (trackNumber >= 0)
            {
                nudTrackNumber.Value = trackNumber;
            }
            if (year >= 0)
            {
                nudYear.Value = year;
            }
            cboGenre.Text = genre;
            txtAlbumArtist.Text = albumArtist;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Update the library entries
            for (int i = 0; i < _libraryEntries.Length; i++)
            {
                string fileName = _libraryEntries[i].FileName;
                string title = _libraryEntries[i].Title;
                string album = _libraryEntries[i].Album;
                string artist = _libraryEntries[i].Artist;
                int trackNumber = _libraryEntries[i].TrackNumber;
                int year = _libraryEntries[i].Year;
                string genre = _libraryEntries[i].Genre;
                string albumArtist = _libraryEntries[i].AlbumArtist;

                if (txtTitle.Text.Length > 0)
                {
                    title = txtTitle.Text;
                }

                if (txtAlbum.Text.Length > 0)
                {
                    album = txtAlbum.Text;
                }

                if (txtArtist.Text.Length > 0)
                {
                    artist = txtArtist.Text;
                }

                if (nudTrackNumber.Value > 0)
                {
                    trackNumber = Convert.ToInt32(nudTrackNumber.Value);
                }

                if (nudYear.Value > 0)
                {
                    year = Convert.ToInt32(nudYear.Value);
                }

                if (cboGenre.Text.Length > 0)
                {
                    genre = cboGenre.Text;
                }

                if (txtAlbumArtist.Text.Length > 0)
                {
                    albumArtist = txtAlbumArtist.Text;
                }

                var newSong = new LibraryEntry(fileName, artist, album, title, trackNumber, year, genre, albumArtist);
                this.Library.Update(newSong);
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
