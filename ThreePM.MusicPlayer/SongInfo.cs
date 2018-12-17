using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;

namespace ThreePM.MusicPlayer
{
    /// <summary>
    /// Stores information about a song.
    /// </summary>
    public class SongInfo
    {
        #region Declarations

        protected string _fileName;
        protected string _artist;
        protected string _albumArtist;
        protected string _album;
        protected string _title;
        protected int _trackNumber;
        protected int _year;
        protected double _duration;
        protected string _genre;
        protected bool _hasTag = true;
        protected string _toStringString;
        protected bool _ignored;
        protected bool _hasFrontCover;
        private readonly System.Drawing.Image _frontCover;

        #endregion

        #region Properties

        public bool HasFrontCover
        {
            get { return _hasFrontCover; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public string Artist
        {
            get { return _artist; }
        }

        public string AlbumArtist
        {
            get { return (string.IsNullOrEmpty(_albumArtist) ? _artist : _albumArtist); }
        }

        public string Album
        {
            get { return _album; }
        }

        public string Title
        {
            get { return _title; }
        }

        public string Genre
        {
            get { return _genre; }
        }

        public int TrackNumber
        {
            get { return _trackNumber; }
        }

        public int Year
        {
            get { return _year; }
        }

        public double Duration
        {
            get { return _duration; }
        }

        public string DurationDescription
        {
            get { return Player.DescribePosition(_duration); }
        }

        public bool Ignored
        {
            get { return _ignored; }
        }

        #endregion

        #region Constructor

        protected SongInfo()
        {
        }

        public System.Drawing.Bitmap GetFrontCover(int width, int height)
        {
            if (_frontCover == null) return null;
            return new System.Drawing.Bitmap(_frontCover, new System.Drawing.Size(width, height));
        }

        public SongInfo(string fileName)
        {
            _fileName = fileName;

            try
            {

                var tag = new TAG_INFO(_fileName);
                int num1 = Un4seen.Bass.Bass.BASS_StreamCreateFile(_fileName, 0, 0, BASSFlag.BASS_STREAM_DECODE);
                if (num1 != 0)
                {
                    _duration = Bass.BASS_ChannelBytes2Seconds(num1, Bass.BASS_ChannelGetLength(num1));
                    BassTags.BASS_TAG_GetFromFile(num1, tag);
                    Un4seen.Bass.Bass.BASS_StreamFree(num1);
                }

                if (tag.PictureCount > 0)
                {
                    for (int i = 0; i < tag.PictureCount; i++)
                    {
                        if (tag.PictureGetType(i) == "FrontAlbumCover")
                        {
                            _hasFrontCover = true;
                            _frontCover = tag.PictureGetImage(i);
                        }
                    }
                }



                int track = 0;
                if (tag.track.IndexOf('/') != -1)
                {
                    _ = int.TryParse(tag.track.Substring(0, tag.track.IndexOf('/')), out track);
                }
                else
                {
                    _ = int.TryParse(tag.track, out track);
                }
                _ = int.TryParse(tag.year, out int year);

                _albumArtist = tag.albumartist;
                if (string.IsNullOrEmpty(_albumArtist))
                {
                    if (tag.NativeTag("TPE2") != null)
                    {
                        _albumArtist = FixString(tag.NativeTag("TPE2"));
                    }
                    else if (tag.NativeTag("WM/AlbumArtist") != null)
                    {
                        _albumArtist = FixString(tag.NativeTag("WM/AlbumArtist"));
                    }
                    else if (tag.NativeTag("ALBUM ARTIST") != null)
                    {
                        _albumArtist = FixString(tag.NativeTag("ALBUM ARTIST"));
                    }
                    // Tag & Rename writes flac files with ENSEMBLE tags not ALBUM ARTIST tags, so fallback
                    else if (tag.NativeTag("ENSEMBLE") != null)
                    {
                        _albumArtist = FixString(tag.NativeTag("ENSEMBLE"));
                    }
                }

                _album = FixString(tag.album);
                if (string.IsNullOrEmpty(_album))
                {
                    // flac files have ALBUM1 sometimes
                    if (tag.NativeTag("ALBUM1") != null)
                    {
                        _album = FixString(tag.NativeTag("ALBUM1"));
                    }
                }

                _artist = FixString(tag.artist);
                _title = FixString(tag.title);
                _trackNumber = track;
                _year = year;
                _genre = FixString(tag.genre);
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch
            {
                //System.Diagnostics.Debugger.Break();
            }

            ComputeToStringString();
        }

        public SongInfo(string fileName, string artist, string album, string title, int track, int year, float duration, string genre, string albumArtist, bool ignored)
        {
            _fileName = FixString(fileName);
            _artist = FixString(artist);
            _album = FixString(album);
            _title = FixString(title);
            _trackNumber = track;
            _year = year;
            _duration = duration;
            _genre = FixString(genre);
            _albumArtist = FixString(albumArtist);
            _ignored = ignored;

            ComputeToStringString();
        }

        protected static string FixString(object input)
        {
            if (input == null)
            {
                return null;
            }
            else
            {
                return input.ToString().Trim().Trim('\0');
            }
        }

        protected void ComputeToStringString()
        {
            _hasTag = !string.IsNullOrEmpty(_title);

            if (_hasTag)
            {
                _toStringString = Player.SongInfoFormatString;
                _toStringString = _toStringString.Replace("{Artist}", this.Artist);
                _toStringString = _toStringString.Replace("{Title}", this.Title);
                _toStringString = _toStringString.Replace("{Album}", this.Album);
                _toStringString = _toStringString.Replace("{AlbumArtist}", this.AlbumArtist);
                _toStringString = _toStringString.Replace("{Year}", this.Year.ToString());
                _toStringString = _toStringString.Replace("{Genre}", this.Genre);
                _toStringString = _toStringString.Replace("{TrackNumber}", this.TrackNumber.ToString());
                _toStringString = _toStringString.Replace("{Duration}", this.DurationDescription);
                _toStringString = _toStringString.Replace("{Ignored}", (_ignored ? "Ignored" : ""));
            }
            else
            {
                _toStringString = this.FileName;
            }
        }

        #endregion

        #region Overridden Methods

        public override string ToString()
        {
            return _toStringString;
        }

        #endregion Overridden Methods
    }
}
