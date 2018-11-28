using System;
using System.Windows.Forms;

namespace ThreePM
{
    public partial class LyricsForm : ThreePM.BaseForm
    {
        private Utilities.LyricsHelper _lyricsHelper;
        private MusicPlayer.SongInfo _lastSong;

        public LyricsForm()
        {
            InitializeComponent();

            this.TopMost = Registry.GetValue("LyricsForm.TopMost", false);
            alwaysOnTopToolStripMenuItem.Checked = this.TopMost;
        }

        private void LyricsHelper_StatusChanged(object sender, EventArgs e)
        {
            SetLyricsTextBox(_lyricsHelper.Status);
        }

        private void SetLyricsTextBox(string text)
        {
            if (txtLyrics.IsHandleCreated)
            {
                txtLyrics.Invoke((MethodInvoker)delegate
                {
                    txtLyrics.Text = text;
                });
            }
            else
            {
                txtLyrics.CreateControl();
                txtLyrics.Text = text;
            }
        }

        private void LyricsHelper_CurrentURLChanged(object sender, EventArgs e)
        {
            if (txtURL.IsHandleCreated)
            {
                txtURL.Invoke((MethodInvoker)delegate
                {
                    txtURL.Text = _lyricsHelper.CurrentURL;
                });
            }
        }

        private void LyricsHelper_LyricsFound(object sender, ThreePM.Utilities.LyricsFoundEventArgs e)
        {
            SetLyricsTextBox(e.Lyrics);
            this.Library.SetLyrics(_lastSong.Title, _lastSong.Artist, e.Lyrics);
            btnGo.Enabled = true;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void InitLibrary()
        {
            _lyricsHelper = new ThreePM.Utilities.LyricsHelper(this.Library);
            _lyricsHelper.LyricsFound += new EventHandler<ThreePM.Utilities.LyricsFoundEventArgs>(LyricsHelper_LyricsFound);
            _lyricsHelper.CurrentURLChanged += new EventHandler(LyricsHelper_CurrentURLChanged);
            _lyricsHelper.StatusChanged += new EventHandler(LyricsHelper_StatusChanged);
        }

        protected override void InitPlayer()
        {
            this.Player.SongOpened += new EventHandler<ThreePM.MusicPlayer.SongEventArgs>(Player_SongOpened);

            if (this.Player.CurrentSong != null)
            {
                LoadLyrics(this.Player.CurrentSong);
            }
        }

        protected override void UnInitPlayer()
        {
            this.Player.SongOpened -= new EventHandler<ThreePM.MusicPlayer.SongEventArgs>(Player_SongOpened);
        }

        private void Player_SongOpened(object sender, MusicPlayer.SongEventArgs e)
        {
            LoadLyrics(e.Song);
        }

        private void LoadLyrics(ThreePM.MusicPlayer.SongInfo songInfo)
        {
            _lastSong = songInfo;
            if (this.Library.GetSong(songInfo.FileName) is ThreePM.MusicLibrary.LibraryEntry entry && !string.IsNullOrEmpty(entry.Lyrics))
            {
                _lyricsHelper.CancelLastRequest();
                SetLyricsTextBox(entry.Lyrics);
                btnGo.Enabled = false;
                txtURL.Text = "Internal";
            }
            else
            {
                _lyricsHelper.LoadLyrics(songInfo);
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(txtURL.Text);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _lyricsHelper.LoadLyrics(_lastSong, true, false, true);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Library.SetLyrics(_lastSong.Title, _lastSong.Artist, txtLyrics.Text);
            ThreePM.Utilities.LyricsHelper.SaveLyricsFile(_lastSong, txtLyrics.Text);
        }

        private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TopMost = alwaysOnTopToolStripMenuItem.Checked;
            Registry.SetValue("LyricsForm.TopMost", this.TopMost);
        }
    }
}

