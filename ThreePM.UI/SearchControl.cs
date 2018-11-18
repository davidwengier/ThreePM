using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ThreePM.MusicPlayer;
using ThreePM.MusicLibrary;
using System.IO;

namespace ThreePM.UI
{
	public partial class SearchControl : UserControl
	{
		#region Events

		public event EventHandler SongPlayed;
		public event EventHandler SongQueued;
		
		#endregion Events

		#region Enums and Constants

		private enum SearchType
		{
			AllText,
			Title,
			Artist,
			Album,
			Lyrics
		};

		#endregion Enums and Constants

		#region Declarations

		private int m_startAt = 0;

		private MusicPlayer.Player m_player;
		private MusicLibrary.Library m_library;

		private SearchType m_searchType = SearchType.AllText;

		#endregion Declarations

		#region Properties

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MusicLibrary.Library Library
		{
			get { return m_library; }
			set { m_library = value; songListView1.Library = m_library; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MusicPlayer.Player Player
		{
			get { return m_player; }
			set { m_player = value; songListView1.Player = m_player; }
		}

		#endregion Properties

		#region Constructor

		public SearchControl()
		{
			InitializeComponent();
		}

		#endregion Constructor

		private void btnPlay_Click(object sender, EventArgs e)
		{
			if (songListView1.SelectedItems.Count == 0) return;
			Player.PlayFile(songListView1.SelectedItems[0].SongInfo.FileName);
			if (SongPlayed != null)
			{
				SongPlayed(this, EventArgs.Empty);
			}
		}

		private void btnPlaylist_Click(object sender, System.EventArgs e)
		{
			foreach (SongListViewItem s in songListView1.SelectedItems)
			{
				Player.Playlist.AddToEnd(s.SongInfo );
			}
			if (SongQueued != null)
			{
				SongQueued(this, EventArgs.Empty);
			}
		}

		private void songListView1_SongQueued(object sender, EventArgs e)
		{
			if (SongQueued != null)
			{
				SongQueued(this, EventArgs.Empty);
			}
		}

		private void songListView1_SongPlayed(object sender, EventArgs e)
		{
			if (SongPlayed != null)
			{
				SongPlayed(this, EventArgs.Empty);
			}
		}

		#region Search Box Methods

		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
			m_startAt = 0;
			m_searchType = SearchType.AllText;
			tmrSearch.Stop();
			tmrSearch.Start();
		}

		private void txtSearch_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Down)
			{
				if (songListView1.Items.Count > 0)
				{
					songListView1.Focus();
					songListView1.SelectedItems.Clear();
					songListView1.SelectedItems.Add(songListView1.Items[0]);
					songListView1.Invalidate(true);
					e.Handled = true;
				}
			}
		}

		private void txtTitleSearch_TextChanged(object sender, EventArgs e)
		{
			m_startAt = 0;
			m_searchType = SearchType.Title;
			tmrSearch.Stop();
			tmrSearch.Start();
		}

		private void txtArtistSearch_TextChanged(object sender, EventArgs e)
		{
			m_startAt = 0;
			m_searchType = SearchType.Artist;
			tmrSearch.Stop();
			tmrSearch.Start();
		}

		private void txtAlbumSearch_TextChanged(object sender, EventArgs e)
		{
			m_startAt = 0;
			m_searchType = SearchType.Album;
			tmrSearch.Stop();
			tmrSearch.Start();
		}

		private void tmrSearch_Tick(object sender, EventArgs e)
		{
			tmrSearch.Stop();

			switch (m_searchType)
			{
				case SearchType.AllText:
				{
					songListView1.DataSource = Library.GetLibrary(txtSearch.Text, 50, true, m_startAt);
					break;
				}
				case SearchType.Title:
				{
					songListView1.DataSource = Library.GetLibrary(txtTitleSearch.Text, 50, true, "Title", m_startAt);
					break;
				}
				case SearchType.Artist:
				{
					songListView1.DataSource = Library.GetLibrary(txtArtistSearch.Text, 50, true, "Artist", m_startAt);
					break;
				}
				case SearchType.Album:
				{
					songListView1.DataSource = Library.GetLibrary(txtAlbumSearch.Text, 50, true, "Album", m_startAt);
					break;
				}
				case SearchType.Lyrics:
				{
					songListView1.DataSource = Library.GetLibrary(txtLyrics.Text, 50, true, "Lyrics", m_startAt);
					break;
				}
			}
		}

		#endregion Search Box Methods

		#region Context Menu Methods

		private void mnuPlayFile_Click(object sender, EventArgs e)
		{
			if (songListView1.SelectedItems.Count != 0)
			{
				SongListViewItem item = songListView1.SelectedItems[0];
				Player.PlayFile(item.SongInfo.FileName);
			}
		}

		private void mnuQueueSong_Click(object sender, EventArgs e)
		{
			if (songListView1.SelectedItems.Count != 0)
			{
				foreach (SongListViewItem item in songListView1.SelectedItems)
				{
					if (Player.Playlist.Contains(item.SongInfo.FileName))
					{
						Player.Playlist.Remove(item.SongInfo.FileName);
					}
					else
					{
						Player.Playlist.AddToEnd(item.SongInfo);
					}
				}
			}
		}

		private void mnuQueueAlbum_Click(object sender, EventArgs e)
		{
			if (songListView1.SelectedItems.Count != 0)
			{
				// Just get the album of the first selected song
				SongListViewItem selectedItem = songListView1.SelectedItems[0];
				string album = selectedItem.SongInfo.Album;
				LibraryEntry[] songs = Library.QueryLibrary("Album LIKE '" + album.Replace("'", "''") + "'");

				foreach (LibraryEntry item in songs)
				{
					if (item.FileName.Equals(Player.CurrentSong.FileName) || Player.Playlist.Contains(item.FileName))
					{
						// do nothing
					}
					else
					{
						Player.Playlist.AddToEnd(item);
					}
				}
			}
		}

		#endregion Context Menu Methods

		private void txtLyrics_TextChanged(object sender, EventArgs e)
		{
			m_startAt = 0;
			m_searchType = SearchType.Lyrics;
			tmrSearch.Stop();
			tmrSearch.Start();
		}

        private void mnuForceSong_Click(object sender, EventArgs e)
        {
            m_player.ForceSong(songListView1.SelectedItems[0].SongInfo.FileName);
        }

		private void btnNext_Click(object sender, EventArgs e)
		{
			m_startAt += 50;
			tmrSearch_Tick(this, EventArgs.Empty);
		}
	}
}
