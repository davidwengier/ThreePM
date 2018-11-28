using System;
using System.IO;
using ThreePM.MusicPlayer;

namespace ThreePM
{
    public partial class AlbumArtForm : BaseForm
    {
        #region Constructor

        public AlbumArtForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Overrides

        protected override void InitPlayer()
        {
            this.Player.SongOpened += new EventHandler<SongEventArgs>(player_SongOpened);
            if (this.Player.CurrentSong != null)
            {
                albumArtBox1.Song = this.Player.CurrentSong;
            }
        }

        protected override void UnInitPlayer()
        {
            this.Player.SongOpened -= new EventHandler<SongEventArgs>(player_SongOpened);
        }

        #endregion

        #region Event Handlers

        private void player_SongOpened(object sender, SongEventArgs e)
        {
            albumArtBox1.Song = this.Player.CurrentSong;
        }

        private void openContainingFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Path.GetDirectoryName(this.Player.CurrentSong.FileName));
        }

        private void downloadAlbumArtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new AlbumArtPicker(this.Player.CurrentSong);
            if (!f.IsDisposed)
            {
                f.Player = this.Player;
                f.Show(this.Owner);
            }
        }

        #endregion
    }
}
