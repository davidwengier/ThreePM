using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

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

        private MusicLibrary.Library m_library;
        private string m_status;
        private string m_currentURL;
        private WebClient wc;
        private string currentStep = "";
        private int currentLyricsObject;
        private List<ILyricsSiteHandler> lyricsObjects = new List<ILyricsSiteHandler>();
        private ThreePM.MusicPlayer.SongInfo m_song;
        private string m_lastLyrics;

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
                return m_status;
            }
        }

        public string CurrentURL
        {
            get
            {
                return m_currentURL;
            }
        }

        public string LastLyrics
        {
            get
            {
                return m_lastLyrics;
            }
        }

        public ThreePM.MusicPlayer.SongInfo Song
        {
            get
            {
                return m_song;
            }
        }

        #endregion

        #region Constructor

        public LyricsHelper(MusicLibrary.Library library)
        {
            m_library = library;

            lyricsObjects.Add(new LyricsWikiHandler());
            lyricsObjects.Add(new LyricsDepotHandler());
            lyricsObjects.Add(new LeosLyricsHandler());
            lyricsObjects.Add(new LyricsManiaHandler());
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
                m_lastLyrics = "";

                if (LyricsFound != null)
                {
                    LyricsFoundEventArgs e = new LyricsFoundEventArgs("");
                    LyricsFound(this, e);
                }
                return;
            }

            currentLyricsObject = 0;

            m_song = song;

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
            string lyrics = m_library.GetLyrics(m_song.Title, m_song.Artist);

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
            string file = m_song.FileName;
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
            SaveLyricsFile(m_song, lyrics);

            m_lastLyrics = lyrics;

            if (LyricsFound != null)
            {
                LyricsFoundEventArgs e = new LyricsFoundEventArgs(lyrics);
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
            if (currentLyricsObject >= lyricsObjects.Count)
            {
                SetStatus("Not Found");

                if (LyricsNotFound != null)
                {
                    LyricsNotFound(this, EventArgs.Empty);
                }

                return;
            }

            string url = lyricsObjects[currentLyricsObject].GetSearchURL(m_song);
            SetStatus("Checking " + lyricsObjects[currentLyricsObject].SiteName);
            LoadURL(url, "search");
        }

        private void SetStatus(string status)
        {
            m_status = status;
            if (StatusChanged != null)
            {
                StatusChanged(this, EventArgs.Empty);
            }
        }

        private void SetCurrentURL(string currentURL)
        {
            m_currentURL = currentURL;
            if (CurrentURLChanged != null)
            {
                CurrentURLChanged(this, EventArgs.Empty);
            }
        }

        public void CancelLastRequest()
        {
            if (wc != null)
            {
                // unhook the event handler as we dont care about this song anymore
                wc.DownloadStringCompleted -= new DownloadStringCompletedEventHandler(Wc_DownloadStringCompleted);
                wc.Dispose();
                wc = null;
            }
        }

        private void LoadURL(string url, string param)
        {
            currentStep = param;
            SetCurrentURL(url);

            try
            {
                CancelLastRequest();
                wc = new WebClient();
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-GB; rv:1.9.0.5) Gecko/2008120122 Firefox/3.0.5 (.NET CLR 3.5.30729)");
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Wc_DownloadStringCompleted);
                wc.DownloadStringAsync(new Uri(url), wc);
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
                WebClient token = e.UserState as WebClient;


                if (e.Cancelled)
                    return;
                if (e.Error != null)
                {
                    currentLyricsObject++;
                    SearchCurrentLyricsSite();
                    return;
                }
                wc.Dispose();
                wc = null;
                HandlePageDownload(e.Result);
            }
            catch { }
        }

        private void HandlePageDownload(string page)
        {
            switch (currentStep)
            {
                case "search":
                    {
                        string url;
                        LyricsSearchResults results = lyricsObjects[currentLyricsObject].ProcessSearchResults(m_song, page, out url);
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
                            currentLyricsObject++;
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
            if (lyricsObjects[currentLyricsObject].GetLyrics(page, out lyrics))
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
                currentLyricsObject++;
                SearchCurrentLyricsSite();
            }
        }

        #endregion
    }
}
