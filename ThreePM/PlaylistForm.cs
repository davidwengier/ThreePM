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
			playlistControl1.Library = Library;
		}

		protected override void InitPlayer()
		{
			playlistControl1.Player = Player;
			Caption = "Playlist - Mode: " + Player.Playlist.PlaylistStyle.ToString();
			Player.Playlist.PlaylistStyleChanged += new EventHandler(Playlist_PlaylistStyleChanged);
		}

		protected override void UnInitPlayer()
		{
			Player.Playlist.PlaylistStyleChanged -= new EventHandler(Playlist_PlaylistStyleChanged);
		}

		void Playlist_PlaylistStyleChanged(object sender, EventArgs e)
		{
			Caption = "Playlist - Mode: " + Player.Playlist.PlaylistStyle.ToString();
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
					SongInfo song = Library.GetSong(file);
					if (song == null)
					{
						song = new LibraryEntry(file);
					}
					Player.Playlist.AddToEnd(song);
				}
				else if (Directory.Exists(file))
				{
					string[] files2 = Directory.GetFiles(file, "*", SearchOption.AllDirectories);
					foreach (string file2 in files2)
					{
						if (Array.IndexOf(Player.SupportedExtensions, "*" + Path.GetExtension(file2)) != -1)
						{
							SongInfo song = Library.GetSong(file2);
							if (song == null)
							{
								song = new LibraryEntry(file2);
							}
							Player.Playlist.AddToEnd(song);
						}
					}
				}
			}
		}
	}
}
