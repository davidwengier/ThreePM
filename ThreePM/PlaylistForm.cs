using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using ThreePM.MusicPlayer;
using ThreePM.MusicLibrary;

namespace ThreePM
{
    /// <summary>
    /// Description of PlaylistForm.
    /// </summary>
    public partial class PlaylistForm : BaseForm
    {
        public PlaylistForm()
        {
            InitializeComponent();
        }

        protected override void InitLibrary()
        {
            playlistControl1.Library = this.Library;
        }

        protected override void InitPlayer()
        {
            playlistControl1.Player = this.Player;
            this.Caption = "Playlist - Mode: " + this.Player.Playlist.PlaylistStyle.ToString();
            this.Player.Playlist.PlaylistStyleChanged += new EventHandler(Playlist_PlaylistStyleChanged);
        }

        protected override void UnInitPlayer()
        {
            this.Player.Playlist.PlaylistStyleChanged -= new EventHandler(Playlist_PlaylistStyleChanged);
        }

        private void Playlist_PlaylistStyleChanged(object sender, EventArgs e)
        {
            this.Caption = "Playlist - Mode: " + this.Player.Playlist.PlaylistStyle.ToString();
        }

        private void PlaylistForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void PlaylistForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    SongInfo song = this.Library.GetSong(file);
                    if (song == null)
                    {
                        song = new LibraryEntry(file);
                    }
                    this.Player.Playlist.AddToEnd(song);
                }
                else if (Directory.Exists(file))
                {
                    string[] files2 = Directory.GetFiles(file, "*", SearchOption.AllDirectories);
                    foreach (string file2 in files2)
                    {
                        if (Array.IndexOf(this.Player.SupportedExtensions, "*" + Path.GetExtension(file2)) != -1)
                        {
                            SongInfo song = this.Library.GetSong(file2);
                            if (song == null)
                            {
                                song = new LibraryEntry(file2);
                            }
                            this.Player.Playlist.AddToEnd(song);
                        }
                    }
                }
            }
        }
    }
}
