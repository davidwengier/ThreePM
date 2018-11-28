using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ThreePM.MusicPlayer;

namespace ThreePM.UI
{
    public partial class PlaylistControl : UserControl
    {
        #region Declarations

        private Player _player;
        private MusicLibrary.Library _library;

        #endregion Declarations

        #region Properties

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
                songListView.Library = this.Library;
            }
        }

        #endregion Properties

        #region Constructor

        public PlaylistControl()
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                this.Player.Playlist.PlaylistChanged -= new EventHandler(Playlist_PlaylistChanged);
                this.Player.Playlist.PlaylistStyleChanged -= new EventHandler(Playlist_PlaylistStyleChanged);
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        #endregion Constructor

        private void InitPlayer()
        {
            this.Player.Playlist.PlaylistChanged += new EventHandler(Playlist_PlaylistChanged);
            this.Player.Playlist.PlaylistStyleChanged += new EventHandler(Playlist_PlaylistStyleChanged);
            songListView.Player = this.Player;

            Playlist_PlaylistStyleChanged(this, EventArgs.Empty);
            RefreshPlaylist();
        }

        private void Playlist_PlaylistStyleChanged(object sender, EventArgs e)
        {
            switch (this.Player.Playlist.PlaylistStyle)
            {
                case ThreePM.MusicPlayer.PlaylistStyle.Normal:
                {
                    toolTip1.SetToolTip(btnPlaylistStyle, "Set to Random");
                    break;
                }
                case ThreePM.MusicPlayer.PlaylistStyle.Random:
                {
                    toolTip1.SetToolTip(btnPlaylistStyle, "Set to Looping");
                    break;
                }
                case ThreePM.MusicPlayer.PlaylistStyle.Looping:
                {
                    toolTip1.SetToolTip(btnPlaylistStyle, "Set to Random Looping");
                    break;
                }
                case ThreePM.MusicPlayer.PlaylistStyle.RandomLooping:
                {
                    toolTip1.SetToolTip(btnPlaylistStyle, "Set to Normal");
                    break;
                }
            }
        }

        private void Playlist_PlaylistChanged(object sender, EventArgs e)
        {
            RefreshPlaylist();
        }

        private void RefreshPlaylist()
        {
            songListView.DataSource = this.Player.Playlist.ToArray();
        }

        private void btnUp_Click(object sender, System.EventArgs e)
        {
            if (songListView.SelectedIndices.Length == 0)
            {
                MessageBox.Show("Please select a song.");
                return;
            }

            if (songListView.SelectedIndices[0] - 1 < 0) return;

            this.Player.Playlist.EventsEnabled = false;
            var itemsToSelect = new List<int>();
            foreach (int index in songListView.SelectedIndices)
            {
                itemsToSelect.Add(index - 1);
                this.Player.Playlist.MoveUp(index);
            }
            this.Player.Playlist.EventsEnabled = true;
            foreach (int index in itemsToSelect)
            {
                songListView.SelectedItems.Add(songListView.Items[index]);
            }
        }

        private void btnDown_Click(object sender, System.EventArgs e)
        {
            if (songListView.SelectedIndices.Length == 0)
            {
                MessageBox.Show("Please select a song.");
                return;
            }

            int[] selectedItems = songListView.SelectedIndices;
            if (selectedItems[selectedItems.Length - 1] + 1 >= songListView.Items.Count) return;

            this.Player.Playlist.EventsEnabled = false;
            var itemsToSelect = new List<int>();
            for (int i = selectedItems.Length - 1; i >= 0; i--)
            {
                int index = selectedItems[i];
                itemsToSelect.Add(index + 1);
                this.Player.Playlist.MoveDown(index);
            }
            this.Player.Playlist.EventsEnabled = true;
            foreach (int index in itemsToSelect)
            {
                songListView.SelectedItems.Add(songListView.Items[index]);
            }
        }

        private void btnRemove_Click(object sender, System.EventArgs e)
        {
            if (songListView.SelectedIndices.Length == 0)
            {
                MessageBox.Show("Please select a song.");
                return;
            }

            RemoveSelectedSongs();
        }

        private void btnPlaylistStyle_Click(object sender, EventArgs e)
        {
            switch (this.Player.Playlist.PlaylistStyle)
            {
                case ThreePM.MusicPlayer.PlaylistStyle.Normal:
                {
                    this.Player.Playlist.PlaylistStyle = ThreePM.MusicPlayer.PlaylistStyle.Random;
                    break;
                }
                case ThreePM.MusicPlayer.PlaylistStyle.Random:
                {
                    this.Player.Playlist.PlaylistStyle = ThreePM.MusicPlayer.PlaylistStyle.Looping;
                    break;
                }
                case ThreePM.MusicPlayer.PlaylistStyle.Looping:
                {
                    this.Player.Playlist.PlaylistStyle = ThreePM.MusicPlayer.PlaylistStyle.RandomLooping;
                    break;
                }
                case ThreePM.MusicPlayer.PlaylistStyle.RandomLooping:
                {
                    this.Player.Playlist.PlaylistStyle = ThreePM.MusicPlayer.PlaylistStyle.Normal;
                    break;
                }
            }
        }

        private void btnClearPlaylist_Click(object sender, EventArgs e)
        {
            this.Player.Playlist.Clear();
        }

        private void btnOpenPlaylist_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Playlist Files|*.m3u|All Files|*.*";
            dlg.DefaultExt = "m3u";
            if (dlg.ShowDialog(this.ParentForm) == DialogResult.OK)
            {
                string file = dlg.FileName;
                this.Player.Playlist.LoadFromFile(file);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "Playlist Files|*.m3u|All Files|*.*";
            dlg.DefaultExt = "m3u";
            if (dlg.ShowDialog(this.ParentForm) == DialogResult.OK)
            {
                string file = dlg.FileName;
                this.Player.Playlist.SaveToFile(file);
            }
        }

        private void RemoveSelectedSongs()
        {
            var rem = new List<int>();
            for (int i = songListView.SelectedIndices.Length - 1; i >= 0; i--)
            {
                rem.Add(songListView.SelectedIndices[i]);
            }

            this.Player.Playlist.EventsEnabled = false;
            foreach (int i in rem)
            {
                this.Player.Playlist.Remove(i);
            }
            this.Player.Playlist.EventsEnabled = true;
        }

        private void songListView_ListChanged(object sender, EventArgs e)
        {
            this.Player.Playlist.EventsEnabled = false;
            this.Player.Playlist.Clear();
            foreach (SongListViewItem item in songListView.Items)
            {
                this.Player.Playlist.AddToEnd(item.SongInfo);
            }
            this.Player.Playlist.EventsEnabled = true;
        }

        private void songListView_DoubleClick(object sender, EventArgs e)
        {
            if (songListView.SelectedItems.Count == 0) return;
            SongInfo s = songListView.SelectedItems[0].SongInfo;

            if (this.Player.Playlist.PlaylistStyle == ThreePM.MusicPlayer.PlaylistStyle.Normal || this.Player.Playlist.PlaylistStyle == ThreePM.MusicPlayer.PlaylistStyle.Random)
            {
                btnRemove.PerformClick();
            }
            else if (this.Player.Playlist.PlaylistStyle == ThreePM.MusicPlayer.PlaylistStyle.Looping)
            {
                this.Player.Playlist.LoopPosition = songListView.SelectedIndices[0];
            }
            this.Player.PlayFile(s.FileName);
        }

        private void songListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                // Delete = remove selected
                RemoveSelectedSongs();
            }
        }
    }
}
