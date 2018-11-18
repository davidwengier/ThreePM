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
			albumPanel1.DataSource = Library.GetAlbumsAsEntries();
			albumPanel1.Library = this.Library;
		}

		protected override void InitPlayer()
		{
			Player.SongOpened += new EventHandler<SongEventArgs>(Player_SongOpened);
			Player_SongOpened(this, new SongEventArgs(Player.CurrentSong));
		}

		protected override void UnInitPlayer()
		{
			Player.SongOpened -= new EventHandler<SongEventArgs>(Player_SongOpened);
		}

		void Player_SongOpened(object sender, SongEventArgs e)
		{
			albumPanel1.SelectedItem = e.Song;
		}

		private void queueAlbumToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string album = albumPanel1.SelectedItem.Album;

			LibraryEntry [] entries = Library.GetLibrary(album, -1, false, "Album");

			Player.Playlist.AddToEnd(entries);
		}

		private void playAndQueueAlbumToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string album = albumPanel1.SelectedItem.Album;

			LibraryEntry[] entries = Library.GetLibrary(album, -1, false, "Album");
			for (int i = 0; i < entries.Length; i++)
			{
				if (i == 0)
				{
					Player.PlayFile(entries[i].FileName);
				}
				else
				{
					Player.Playlist.AddToEnd(entries[i]);
				}
			}
		}
	}
}

