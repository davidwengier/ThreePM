using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ThreePM.MusicPlayer;

namespace ThreePM.UI
{
    public partial class InfoControl : UserControl
    {
        private Player _player;
        private MusicLibrary.Library _library;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MusicPlayer.Player Player
        {
            get { return _player; }
            set
            {
                _player = value;
                InitPlayer();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MusicLibrary.Library Library
        {
            get { return _library; }
            set
            {
                _library = value;
                InitLibrary();
            }
        }

        public InfoControl()
        {
            InitializeComponent();
        }

        private void InitLibrary()
        {
            this.Library.PlayCountUpdated += new EventHandler<ThreePM.MusicLibrary.LibraryEntryEventArgs>(Library_LibraryUpdated);
            this.Library.LibraryUpdated += new EventHandler<ThreePM.MusicLibrary.LibraryEntryEventArgs>(Library_LibraryUpdated);
        }

        private void Library_LibraryUpdated(object sender, ThreePM.MusicLibrary.LibraryEntryEventArgs e)
        {
            if (e.LibraryEntry == null)
                return;
            if (e.LibraryEntry.FileName.Equals(_player.CurrentSong.FileName))
            {
                LoadSong();
            }
        }

        private void InitPlayer()
        {
            this.Player.LoadingSong += new EventHandler<FileEventArgs>(Player_LoadingSong);
            this.Player.SongOpened += new EventHandler<SongEventArgs>(Player_SongOpened);
            if (this.Player.CurrentSong != null)
            {
                LoadSong();
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                this.Library.PlayCountUpdated -= new EventHandler<ThreePM.MusicLibrary.LibraryEntryEventArgs>(Library_LibraryUpdated);
                this.Library.LibraryUpdated -= new EventHandler<ThreePM.MusicLibrary.LibraryEntryEventArgs>(Library_LibraryUpdated);
                this.Player.LoadingSong -= new EventHandler<FileEventArgs>(Player_LoadingSong);
                this.Player.SongOpened -= new EventHandler<SongEventArgs>(Player_SongOpened);

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Player_SongOpened(object sender, SongEventArgs e)
        {
            LoadSong();
        }

        private void Player_LoadingSong(object sender, FileEventArgs e)
        {
            lblTitle.Text = "Title:";
            lblArtist.Text = "Artist:";
            lblAlbum.Text = "Album:";
            lblTrack.Text = "Track:";
            lblAlbumArtist.Text = "Album Artist:";
            lblGenre.Text = "Genre:";
            lblYear.Text = "Year:";
            lblPlayCount.Text = "Play Count:";
            lblDuration.Text = "Duration:";
            lblFilename.Text = "Filename: " + e.Filename;
            lblFilename.Refresh();
        }



        private void btnFolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Path.GetDirectoryName(this.Player.CurrentSong.FileName));
        }

        private void LoadSong()
        {
            lblTitle.Text = "Title: " + this.Player.CurrentSong.Title;
            lblArtist.Text = "Artist: " + this.Player.CurrentSong.Artist;
            lblAlbum.Text = "Album: " + this.Player.CurrentSong.Album;
            lblTrack.Text = "Track: " + this.Player.CurrentSong.TrackNumber;
            lblFilename.Text = "Filename: " + this.Player.CurrentSong.FileName;
            lblYear.Text = "Year: " + this.Player.CurrentSong.Year;
            lblGenre.Text = "Genre: " + this.Player.CurrentSong.Genre;
            lblAlbumArtist.Text = "Album Artist: " + this.Player.CurrentSong.AlbumArtist;
            lblDuration.Text = "Duration: " + this.Player.CurrentSong.DurationDescription;

            lblPlayCount.Text = "Play Count: " + this.Library.GetPlayCount(this.Player.CurrentSong.FileName).ToString();

            albumArtBox1.Song = this.Player.CurrentSong;
        }
    }
}
