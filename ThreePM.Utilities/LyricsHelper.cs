using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ThreePM.Utilities
{
    public class LyricsHelper
    {
        #region Static Methods

        public static string GetSongTitleRegex(ThreePM.MusicPlayer.SongInfo song)
        {
            /*


			I'*m Like A Lawyer With The Way I'*m Always Trying To Get You Off \(Me [\+\&(and)]* You\)

			 */

            string regex = song.Title.ToLower();
            regex = regex.Replace("*", "\\*");
            regex = regex.Replace("?", "\\?*");
            regex = regex.Replace("'", "'*");
            regex = regex.Replace(".", "\\.*");
            regex = regex.Replace(" + ", " and ");
            regex = regex.Replace(" & ", " and ");
            regex = regex.Replace("\"", "\\\"*");
            regex = regex.Replace("(", "\\(*");
            regex = regex.Replace(")", "\\)*");
            regex = regex.Replace("[", "\\[*");
            regex = regex.Replace("]", "\\]*");
            regex = regex.Replace("{", "\\{*");
            regex = regex.Replace("}", "\\}*");
            regex = regex.Replace("$", "\\$*");
            regex = regex.Replace(" and ", " [\\+\\&(and)]* ");
            return regex;
        }

        #endregion

        #region Declarations

        private MusicLibrary.Library _library;
        private string _status;
        private string _currentURL;
        private WebClient _wc;
        private string _currentStep = "";
        private int _currentLyricsObject;
        private List<ILyricsSiteHandler> _lyricsObjects = new List<ILyricsSiteHandler>();
        private MusicPlayer.SongInfo _song;
        private string _lastLyrics;

        public event EventHandler StatusChanged;
        public event EventHandler CurrentURLChanged;
        public event EventHandler<LyricsFoundEventArgs> LyricsFound;
        public event EventHandler LyricsNotFound;

        #endregion

        #region Properties

        public string Status
        {
            get
            {
                return _status;
            }
        }

        public string CurrentURL
        {
            get
            {
                return _currentURL;
            }
        }

        public string LastLyrics
        {
            get
            {
                return _lastLyrics;
            }
        }

        public ThreePM.MusicPlayer.SongInfo Song
        {
            get
            {
                return _song;
            }
        }

        #endregion

        #region Constructor

        public LyricsHelper(MusicLibrary.Library library)
        {
            _library = library;

            _lyricsObjects.Add(new LyricsWikiHandler());
            _lyricsObjects.Add(new LyricsDepotHandler());
            _lyricsObjects.Add(new LeosLyricsHandler());
            _lyricsObjects.Add(new LyricsManiaHandler());
        }

        #endregion

        #region Methods

        public void LoadLyrics(ThreePM.MusicPlayer.SongInfo song)
        {
            LoadLyrics(song, false, false, false);
        }

        public void LoadLyrics(ThreePM.MusicPlayer.SongInfo song, bool skipTextFile, bool onlyTextFile, bool skipOtherSongsInDatabase)
        {
            if (song.FileName.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) && string.IsNullOrEmpty(song.Title) && string.IsNullOrEmpty(song.Artist))
            {
                _lastLyrics = "";

                if (LyricsFound != null)
                {
                    var e = new LyricsFoundEventArgs("");
                    LyricsFound(this, e);
                }
                return;
            }

            _currentLyricsObject = 0;

            _song = song;

            if (onlyTextFile)
            {
                if (!CheckForLyricsTextFile())
                {
                    if (LyricsNotFound != null)
                    {
                        LyricsNotFound(this, EventArgs.Empty);
                    }
                }

                return;
            }

            // check for other songs in the DB
            if (!skipOtherSongsInDatabase && CheckForLyricsFromDatabase())
            {
                return;
            }

            if (skipTextFile || CheckForLyricsTextFile() == false)
            {
                SearchCurrentLyricsSite();
            }
        }

        private bool CheckForLyricsFromDatabase()
        {
            SetStatus("Checking for lyrics from database");
            string lyrics = _library.GetLyrics(_song.Title, _song.Artist);

            if (!string.IsNullOrEmpty(lyrics))
            {
                SetCurrentURL("Another copy of the song in the library");

                FoundLyrics(lyrics);

                return true;
            }
            return false;
        }

        private bool CheckForLyricsTextFile()
        {
            SetStatus("Checking for lyrics text file");
            string file = _song.FileName;
            if (file.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
                return false;
            file = System.IO.Path.ChangeExtension(file, ".lyrics");
            if (File.Exists(file))
            {
                string lyrics = File.ReadAllText(file);
                if (lyrics != null && lyrics.Trim().Length > 0)
                {
                    SetCurrentURL(file);

                    FoundLyrics(lyrics);
                    return true;
                }
            }
            return false;
        }

        private void FoundLyrics(string lyrics)
        {
            // write out lyrics
            SaveLyricsFile(_song, lyrics);

            _lastLyrics = lyrics;

            if (LyricsFound != null)
            {
                var e = new LyricsFoundEventArgs(lyrics);
                LyricsFound(this, e);
            }
        }

        public static void SaveLyricsFile(ThreePM.MusicPlayer.SongInfo song, string lyrics)
        {
            string file = song.FileName;
            file = System.IO.Path.ChangeExtension(file, ".lyrics");
            try
            {
                File.WriteAllText(file, lyrics);
            }
            catch { }
        }

        private void SearchCurrentLyricsSite()
        {
            if (_currentLyricsObject >= _lyricsObjects.Count)
            {
                SetStatus("Not Found");

                if (LyricsNotFound != null)
                {
                    LyricsNotFound(this, EventArgs.Empty);
                }

                return;
            }

            string url = _lyricsObjects[_currentLyricsObject].GetSearchURL(_song);
            SetStatus("Checking " + _lyricsObjects[_currentLyricsObject].SiteName);
            LoadURL(url, "search");
        }

        private void SetStatus(string status)
        {
            _status = status;
            if (StatusChanged != null)
            {
                StatusChanged(this, EventArgs.Empty);
            }
        }

        private void SetCurrentURL(string currentURL)
        {
            _currentURL = currentURL;
            if (CurrentURLChanged != null)
            {
                CurrentURLChanged(this, EventArgs.Empty);
            }
        }

        public void CancelLastRequest()
        {
            if (_wc != null)
            {
                // unhook the event handler as we dont care about this song anymore
                _wc.DownloadStringCompleted -= new DownloadStringCompletedEventHandler(Wc_DownloadStringCompleted);
                _wc.Dispose();
                _wc = null;
            }
        }

        private void LoadURL(string url, string param)
        {
            _currentStep = param;
            SetCurrentURL(url);

            try
            {
                CancelLastRequest();
                _wc = new WebClient();
                _wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-GB; rv:1.9.0.5) Gecko/2008120122 Firefox/3.0.5 (.NET CLR 3.5.30729)");
                _wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Wc_DownloadStringCompleted);
                _wc.DownloadStringAsync(new Uri(url), _wc);
            }
            catch
            {
                return;
            }
        }

        private void Wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var token = e.UserState as WebClient;


                if (e.Cancelled)
                    return;
                if (e.Error != null)
                {
                    _currentLyricsObject++;
                    SearchCurrentLyricsSite();
                    return;
                }
                _wc.Dispose();
                _wc = null;
                HandlePageDownload(e.Result);
            }
            catch { }
        }

        private void HandlePageDownload(string page)
        {
            switch (_currentStep)
            {
                case "search":
                {
                    string url;
                    LyricsSearchResults results = _lyricsObjects[_currentLyricsObject].ProcessSearchResults(_song, page, out url);
                    if (results == LyricsSearchResults.SearchAgain)
                    {
                        LoadURL(url, "search");
                    }
                    else if (results == LyricsSearchResults.Found)
                    {
                        LoadURL(url, "lyrics");
                    }
                    else if (results == LyricsSearchResults.NotFound)
                    {
                        _currentLyricsObject++;
                        SearchCurrentLyricsSite();
                    }
                    else if (results == LyricsSearchResults.FoundOnThisPage)
                    {
                        LoadLyrics(page);
                    }

                    break;
                }
                case "lyrics":
                {
                    LoadLyrics(page);
                    break;
                }
            }
        }

        private void LoadLyrics(string page)
        {
            string lyrics;
            if (_lyricsObjects[_currentLyricsObject].GetLyrics(page, out lyrics))
            {
                lyrics = System.Web.HttpUtility.HtmlDecode(lyrics);

                lyrics = lyrics.Replace("â€™", "'");
                lyrics = lyrics.Replace("’", "'");
                lyrics = lyrics.Replace("Ã¢â‚¬â„¢", "'");
                lyrics = lyrics.Replace("Ã¢â€šÂ¬Ã¢â€žÂ¢", "'");
                lyrics = lyrics.Replace("Ã¢â‚¬Å“", "\"");

                lyrics = lyrics.Replace("\n", "");
                lyrics = lyrics.Replace("\r", "");

                lyrics = lyrics.Replace("<br>", "$%^");
                lyrics = lyrics.Replace("<br/>", "$%^");
                lyrics = lyrics.Replace("<br />", "$%^");
                lyrics = lyrics.Replace("<BR>", "$%^");
                lyrics = lyrics.Replace("<BR/>", "$%^");
                lyrics = lyrics.Replace("<BR />", "$%^");

                lyrics = lyrics.Replace("$%^", Environment.NewLine);

                lyrics = lyrics.Trim();

                FoundLyrics(lyrics);
            }
            else
            {
                _currentLyricsObject++;
                SearchCurrentLyricsSite();
            }
        }

        #endregion
    }
}
