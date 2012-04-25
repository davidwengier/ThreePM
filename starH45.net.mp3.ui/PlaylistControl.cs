using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using starH45.net.mp3.player;
using starH45.net.mp3.library;

namespace starH45.net.mp3.ui
{
	public partial class PlaylistControl : UserControl
	{
		#region Declarations

		private player.Player m_player;
		private library.Library m_library;

		#endregion Declarations

		#region Properties

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public player.Player Player
		{
			get { return m_player; }
			set
			{
				m_player = value;
				InitPlayer();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public library.Library Library
		{
			get { return m_library; }
			set
			{
				m_library = value;
				songListView.Library = Library;
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
			songListView.Player = Player;

			Playlist_PlaylistStyleChanged(this, EventArgs.Empty);
			RefreshPlaylist();
		}

		private void Playlist_PlaylistStyleChanged(object sender, EventArgs e)
		{
			switch (Player.Playlist.PlaylistStyle)
			{
				case starH45.net.mp3.player.PlaylistStyle.Normal:
				{
					toolTip1.SetToolTip(btnPlaylistStyle, "Set to Random");
					break;
				}
				case starH45.net.mp3.player.PlaylistStyle.Random:
				{
					toolTip1.SetToolTip(btnPlaylistStyle, "Set to Looping");
					break;
				}
				case starH45.net.mp3.player.PlaylistStyle.Looping:
				{
					toolTip1.SetToolTip(btnPlaylistStyle, "Set to Random Looping");
					break;
				}
				case starH45.net.mp3.player.PlaylistStyle.RandomLooping:
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
			songListView.DataSource = Player.Playlist.ToArray();
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			if (songListView.SelectedIndices.Length == 0)
			{
				MessageBox.Show("Please select a song.");
				return;
			}

			if (songListView.SelectedIndices[0] - 1 < 0) return;

			Player.Playlist.EventsEnabled = false;
			List<int> itemsToSelect = new List<int>();
			foreach (int index in songListView.SelectedIndices)
			{
				itemsToSelect.Add(index - 1);
				Player.Playlist.MoveUp(index);
			}
			Player.Playlist.EventsEnabled = true;
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

			int [] selectedItems = songListView.SelectedIndices;
			if (selectedItems[selectedItems.Length - 1] + 1 >= songListView.Items.Count) return;

			Player.Playlist.EventsEnabled = false;
			List<int> itemsToSelect = new List<int>();
			for (int i = selectedItems.Length - 1; i>= 0; i--)
			{
				int index = selectedItems[i];
				itemsToSelect.Add(index + 1);
				Player.Playlist.MoveDown(index);
			}
			Player.Playlist.EventsEnabled = true;
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
			switch (Player.Playlist.PlaylistStyle)
			{
				case starH45.net.mp3.player.PlaylistStyle.Normal:
				{
					Player.Playlist.PlaylistStyle = starH45.net.mp3.player.PlaylistStyle.Random;
					break;
				}
				case starH45.net.mp3.player.PlaylistStyle.Random:
				{
					Player.Playlist.PlaylistStyle = starH45.net.mp3.player.PlaylistStyle.Looping;
					break;
				}
				case starH45.net.mp3.player.PlaylistStyle.Looping:
				{
					Player.Playlist.PlaylistStyle = starH45.net.mp3.player.PlaylistStyle.RandomLooping;
					break;
				}
				case starH45.net.mp3.player.PlaylistStyle.RandomLooping:
				{
					Player.Playlist.PlaylistStyle = starH45.net.mp3.player.PlaylistStyle.Normal;
					break;
				}
			}
		}

		private void btnClearPlaylist_Click(object sender, EventArgs e)
		{
			Player.Playlist.Clear();
		}

		private void btnOpenPlaylist_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Playlist Files|*.m3u|All Files|*.*";
			dlg.DefaultExt = "m3u";
			if (dlg.ShowDialog(this.ParentForm) == DialogResult.OK)
			{
				string file = dlg.FileName;
				Player.Playlist.LoadFromFile(file);
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "Playlist Files|*.m3u|All Files|*.*";
			dlg.DefaultExt = "m3u";
			if (dlg.ShowDialog(this.ParentForm) == DialogResult.OK)
			{
				string file = dlg.FileName;
				Player.Playlist.SaveToFile(file);
			}
		}

		private void RemoveSelectedSongs()
		{
			List<int> rem = new List<int>();
			for (int i = songListView.SelectedIndices.Length - 1; i >= 0; i--)
			{
				rem.Add(songListView.SelectedIndices[i]);
			}

			Player.Playlist.EventsEnabled = false;
			foreach (int i in rem)
			{
				Player.Playlist.Remove(i);
			}
			Player.Playlist.EventsEnabled = true;
		}

		private void songListView_ListChanged(object sender, EventArgs e)
		{
			Player.Playlist.EventsEnabled = false;
			Player.Playlist.Clear();
			foreach (SongListViewItem item in songListView.Items)
			{
				Player.Playlist.AddToEnd(item.SongInfo);
			}
			Player.Playlist.EventsEnabled = true;
		}

		private void songListView_DoubleClick(object sender, EventArgs e)
		{
			if (songListView.SelectedItems.Count == 0) return;
			SongInfo s = songListView.SelectedItems[0].SongInfo;

			if (Player.Playlist.PlaylistStyle == starH45.net.mp3.player.PlaylistStyle.Normal || Player.Playlist.PlaylistStyle == starH45.net.mp3.player.PlaylistStyle.Random)
			{
				btnRemove.PerformClick();
			}
			else if (Player.Playlist.PlaylistStyle == starH45.net.mp3.player.PlaylistStyle.Looping)
			{
				Player.Playlist.LoopPosition = songListView.SelectedIndices[0];
			}
			Player.PlayFile(s.FileName);
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
