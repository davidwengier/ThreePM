using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ThreePM.MusicLibrary;
using ThreePM.MusicPlayer;

namespace ThreePM
{
    public partial class AlbumListForm : ThreePM.BaseForm
    {
        public AlbumListForm()
        {
            InitializeComponent();
        }

        protected override void InitLibrary()
        {
            albumPanel1.DataSource = this.Library.GetAlbumsAsEntries();
            albumPanel1.Library = this.Library;
        }

        protected override void InitPlayer()
        {
            this.Player.SongOpened += new EventHandler<SongEventArgs>(Player_SongOpened);
            Player_SongOpened(this, new SongEventArgs(this.Player.CurrentSong));
        }

        protected override void UnInitPlayer()
        {
            this.Player.SongOpened -= new EventHandler<SongEventArgs>(Player_SongOpened);
        }

        private void Player_SongOpened(object sender, SongEventArgs e)
        {
            albumPanel1.SelectedItem = e.Song;
        }

        private void queueAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string album = albumPanel1.SelectedItem.Album;

            LibraryEntry[] entries = this.Library.GetLibrary(album, -1, false, "Album");

            this.Player.Playlist.AddToEnd(entries);
        }

        private void playAndQueueAlbumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string album = albumPanel1.SelectedItem.Album;

            LibraryEntry[] entries = this.Library.GetLibrary(album, -1, false, "Album");
            for (int i = 0; i < entries.Length; i++)
            {
                if (i == 0)
                {
                    this.Player.PlayFile(entries[i].FileName);
                }
                else
                {
                    this.Player.Playlist.AddToEnd(entries[i]);
                }
            }
        }
    }
}

