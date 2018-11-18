using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ThreePM.MusicPlayer;
using ThreePM.MusicLibrary;
using System.IO;
using ThreePM.UI;

namespace ThreePM
{
	public partial class LibraryForm : ThreePM.BaseForm
	{
		#region Constants

		private const string EmptyNodeText = "++";

		#endregion Constants

		#region Declarations

		private TreeNode artistsAlbumsNode;
		private TreeNode albumsNode;
		private TreeNode contributingArtistsNode;
		private TreeNode internetRadioNode;
		private TreeNode playListNode;
		private TreeNode searchNode;
		private TreeNode nowPlayingNode;
		private TreeNode genresNode;
		private TreeNode yearsNode;
        private TreeNode statisticsNode;
		private TreeNode ignoredTracksNode;
		private TreeNode deletedTracksNode;

		private int m_numRecentSongs = 50;
		private bool m_autoChangeToNowPlaying;
		private bool m_autoChangeToPlaylistAfterQueuing;
		private bool m_autoTrackNowPlayingInTree;
		private bool m_trackByArtist;
		private bool m_trackByAlbum;
		private bool m_trackContributing;
		private bool m_hideIgnoredSongs;

		#endregion Declarations

		#region Constructor

		public LibraryForm()
		{
			InitializeComponent();
			infoControl.BringToFront();
			
			LoadRegistrySettings();

			splitContainer.SplitterDistance = Convert.ToInt32(Registry.GetValue("LibraryForm.SplitterDistance", splitContainer.SplitterDistance));
		}

		public void LoadRegistrySettings()
		{
			m_autoChangeToNowPlaying = Convert.ToBoolean(Registry.GetValue("LibraryForm.NowPlayingAfterPlay", true));
			m_autoChangeToPlaylistAfterQueuing = Convert.ToBoolean(Registry.GetValue("LibraryForm.PlaylistAfterQueue", false));
			m_autoTrackNowPlayingInTree = Convert.ToBoolean(Registry.GetValue("LibraryForm.TrackNowPlayingInTree", true));
			m_trackByArtist = Convert.ToBoolean(Registry.GetValue("LibraryForm.TrackByArtist", false));
			m_trackByAlbum = Convert.ToBoolean(Registry.GetValue("LibraryForm.TrackByAlbum", true));

			m_hideIgnoredSongs = Convert.ToBoolean(Registry.GetValue("LibraryForm.HideIgnoredSongs", false));
			m_trackContributing = Convert.ToBoolean(Registry.GetValue("LibraryForm.TrackContributing", false));
			splitContainer.Panel1Collapsed = !Convert.ToBoolean(Registry.GetValue("LibraryForm.ShowTree", true));
			songListView.ShowAlbumArt = Convert.ToBoolean(Registry.GetValue("LibraryForm.ShowAlbumArt", true));
			songListView.AlbumArtSize = Convert.ToInt32(Registry.GetValue("LibraryForm.AlbumArtSize", 100));
			m_numRecentSongs = Convert.ToInt32(Registry.GetValue("LibraryForm.SongsToShowInRecentLists", 50));
		}

		#endregion Constructor

		#region Overridden Methods

		public override void ReloadSkin(bool loadBlankSkinFirst)
		{
			base.ReloadSkin(loadBlankSkinFirst);
			pctShowTree.Top = 4; // InternalBorderSize - 4;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyData == Keys.F5)
			{
				Player_SongOpened(this, new SongEventArgs(Player.CurrentSong));
				e.Handled = true;
			}
			base.OnKeyDown(e);
		}

		protected override void InitLibrary()
		{
			playlistControl.Library = Library;
			searchControl.Library = Library;
			infoControl.Library = Library;
			songListView.Library = Library;
			statisticsControl.Library = Library;

			Library.LibraryUpdated += new EventHandler<LibraryEntryEventArgs>(Library_LibraryUpdated);
			LoadLibraryTree();
		}

		protected override void UnInitLibrary()
		{
			Library.LibraryUpdated -= new EventHandler<LibraryEntryEventArgs>(Library_LibraryUpdated);
		}

		protected override void InitPlayer()
		{
			playlistControl.Player = Player;
			searchControl.Player = Player;
			infoControl.Player = Player;
			songListView.Player = Player;
			statisticsControl.Player = Player;

			Player.SongOpened += new EventHandler<SongEventArgs>(Player_SongOpened);
			if (Player.CurrentSong != null)
			{
				Player_SongOpened(this, new SongEventArgs(Player.CurrentSong));
			}
		}

		protected override void UnInitPlayer()
		{
			Player.SongOpened -= new EventHandler<SongEventArgs>(Player_SongOpened);
		}

		#endregion

		#region Library Loading Methods

		private void LoadLibraryTree()
		{
			MethodInvoker DoWork = delegate
			{
				UseWaitCursor = true;
				// Load the artists
				string[] albumArtists = Library.GetAlbumArtists();
				Array.Sort<string>(albumArtists, CompareStringsWithoutTheAndIgnoringCase);
				string[] albums = Library.GetAlbums();
				Array.Sort<string>(albums, CompareStringsWithoutTheAndIgnoringCase);
				string[] artists = Library.GetArtists();
				Array.Sort<string>(artists, CompareStringsWithoutTheAndIgnoringCase);
				string[] years = Library.GetYears();
				string[] genres = Library.GetGenres();

				tvwLibrary.Invoke((MethodInvoker)delegate
				{
					tvwLibrary.BeginUpdate();
					tvwLibrary.Nodes.Clear();
					LoadTopLevelNodes();
				});

				LoadNodes(artistsAlbumsNode, albumArtists, "Artist", true);
				LoadNodes(albumsNode, albums, "Album", false);
				LoadNodes(contributingArtistsNode, artists, "Artist", false);
				LoadNodes(yearsNode, years, "Year", false);
				LoadNodes(genresNode, genres, "Genre", false);

				tvwLibrary.BeginInvoke((MethodInvoker)delegate
				{
					lblLoading.Visible = false;
					tvwLibrary.EndUpdate();

					Player_SongOpened(this, new SongEventArgs(Player.CurrentSong));
				});

				UseWaitCursor = false;
			};

			IntPtr t = tvwLibrary.Handle;
			DoWork.BeginInvoke(null, null);
		}

		private void LoadTopLevelNodes()
		{
			nowPlayingNode = tvwLibrary.Nodes.Add("Now Playing");
			nowPlayingNode.ImageKey = "NowPlaying";
			nowPlayingNode.SelectedImageKey = nowPlayingNode.ImageKey;

			searchNode = tvwLibrary.Nodes.Add("Search Library");
			searchNode.ImageKey = "Search";
			searchNode.SelectedImageKey = searchNode.ImageKey;

			playListNode = tvwLibrary.Nodes.Add("Playlist");
			playListNode.ImageKey = "Playlist";
			playListNode.SelectedImageKey = playListNode.ImageKey;

			playListNode.Nodes.Add("Top Played");
			playListNode.Nodes.Add("Recently Played");
			playListNode.Nodes.Add("Recently Added");
            playListNode.Nodes.Add("Forgotten Gems");
	
            statisticsNode = playListNode.Nodes.Add("Statistics");

			ignoredTracksNode = playListNode.Nodes.Add("Ignored Tracks");

			deletedTracksNode = playListNode.Nodes.Add("Deleted Tracks");

			foreach (TreeNode node in playListNode.Nodes)
			{
				node.ImageKey = "Playlist";
				node.SelectedImageKey = "Playlist";
			}

			internetRadioNode = tvwLibrary.Nodes.Add("Internet Radio");
			internetRadioNode.ImageKey = "Radio";
			internetRadioNode.SelectedImageKey = internetRadioNode.ImageKey;

			artistsAlbumsNode = tvwLibrary.Nodes.Add("Artists/Albums");
			artistsAlbumsNode.ImageKey = "Music";
			artistsAlbumsNode.SelectedImageKey = artistsAlbumsNode.ImageKey;

			albumsNode = tvwLibrary.Nodes.Add("Albums");
			albumsNode.ImageKey = "Music";
			albumsNode.SelectedImageKey = albumsNode.ImageKey;

			contributingArtistsNode = tvwLibrary.Nodes.Add("Contributing Artist");
			contributingArtistsNode.ImageKey = "Music";
			contributingArtistsNode.SelectedImageKey = contributingArtistsNode.ImageKey;

			yearsNode = tvwLibrary.Nodes.Add("Years");
			yearsNode.ImageKey = "Music";
			yearsNode.SelectedImageKey = yearsNode.ImageKey;

			genresNode = tvwLibrary.Nodes.Add("Genres");
			genresNode.ImageKey = "Music";
			genresNode.SelectedImageKey = genresNode.ImageKey;
		}

		private void LoadNodes(TreeNode parent, string[] nodes, string name, bool addExpander)
		{
			Array.Sort<string>(nodes, CompareStringsWithoutTheAndIgnoringCase);
			string lastNode = "";
			tvwLibrary.BeginInvoke((MethodInvoker)delegate
			{
				parent.Nodes.Add("(No " + name + ")");
			});
			foreach (string node in nodes)
			{
				if (!String.IsNullOrEmpty(node) && !node.Equals(lastNode, StringComparison.InvariantCultureIgnoreCase))
				{
					lastNode = node;
					TreeNode newNode = null;
					tvwLibrary.Invoke((MethodInvoker)delegate
					{
						newNode = parent.Nodes.Add(node);
					});
					if (addExpander)
					{
						// Add a child node so that the artist node is expandable.  This will get cleared
						// out and replaced with albums when required
						tvwLibrary.BeginInvoke((MethodInvoker)delegate
						{
							newNode.Nodes.Add(LibraryForm.EmptyNodeText);
						});
					}
				}
			}
		}

		#endregion Library Loading Methods

		#region Player and Library Event Handlers

		private void Library_LibraryUpdated(object sender, LibraryEntryEventArgs e)
		{
			if (!String.IsNullOrEmpty(e.LibraryEntry.AlbumArtist) && artistsAlbumsNode != null)
			{
				foreach (TreeNode node in artistsAlbumsNode.Nodes)
				{
					if (node.Text.Equals(e.LibraryEntry.AlbumArtist, StringComparison.InvariantCultureIgnoreCase))
					{
						if (node.Nodes.Count > 1 || node.Nodes[0].Text != EmptyNodeText)
						{
							if (!String.IsNullOrEmpty(e.LibraryEntry.Album))
							{
								foreach (TreeNode node2 in node.Nodes)
								{
									if (node2.Text.Equals(e.LibraryEntry.Album, StringComparison.InvariantCultureIgnoreCase))
									{
										break;
									}
									else if (CompareStringsWithoutTheAndIgnoringCase(node2.Text, e.LibraryEntry.Album) > 0)
									{
										node.Nodes.Insert(node2.Index, e.LibraryEntry.Album);
										break;
									}
									else if (node2.NextNode == null)
									{
										node.Nodes.Add(e.LibraryEntry.Album);
									}
								}
							}
						}

						break;
					}
					else if (CompareStringsWithoutTheAndIgnoringCase(node.Text, e.LibraryEntry.AlbumArtist) > 0)
					{
						artistsAlbumsNode.Nodes.Insert(node.Index, e.LibraryEntry.AlbumArtist).Nodes.Add(EmptyNodeText);
						break;
					}
					else if (node.NextNode == null)
					{
						artistsAlbumsNode.Nodes.Add(e.LibraryEntry.AlbumArtist).Nodes.Add(EmptyNodeText);
						break;
					}
				}
			}

			if (!String.IsNullOrEmpty(e.LibraryEntry.Album) && albumsNode != null)
			{
				foreach (TreeNode node in albumsNode.Nodes)
				{
					if (node.Text.Equals(e.LibraryEntry.Album, StringComparison.InvariantCultureIgnoreCase))
					{
						break;
					}
					else if (CompareStringsWithoutTheAndIgnoringCase(node.Text, e.LibraryEntry.Album) > 0)
					{
						albumsNode.Nodes.Insert(node.Index, e.LibraryEntry.Album);
						break;
					}
					else if (node.NextNode == null)
					{
						albumsNode.Nodes.Add(e.LibraryEntry.Album);
					}

				}
			}

			if (!String.IsNullOrEmpty(e.LibraryEntry.Artist) && contributingArtistsNode != null)
			{
				foreach (TreeNode node in contributingArtistsNode.Nodes)
				{
					if (node.Text.Equals(e.LibraryEntry.Artist, StringComparison.InvariantCultureIgnoreCase))
					{
						break;
					}
					else if (CompareStringsWithoutTheAndIgnoringCase(node.Text, e.LibraryEntry.Artist) > 0)
					{
						contributingArtistsNode.Nodes.Insert(node.Index, e.LibraryEntry.Artist);
						break;
					}
					else if (node.NextNode == null)
					{
						contributingArtistsNode.Nodes.Add(e.LibraryEntry.Artist);
					}

				}
			}

			if (!String.IsNullOrEmpty(e.LibraryEntry.Genre) && genresNode != null)
			{
				foreach (TreeNode node in genresNode.Nodes)
				{
					if (node.Text.Equals(e.LibraryEntry.Genre, StringComparison.InvariantCultureIgnoreCase))
					{
						break;
					}
					else if (node.Text.CompareTo(e.LibraryEntry.Genre) > 0)
					{
						genresNode.Nodes.Insert(node.Index, e.LibraryEntry.Genre);
						break;
					}
					else if (node.NextNode == null)
					{
						genresNode.Nodes.Add(e.LibraryEntry.Genre);
					}

				}
			}

			if (e.LibraryEntry.Year > 0 && yearsNode != null)
			{
				foreach (TreeNode node in yearsNode.Nodes)
				{
					if (node.Text == "(No Year)")
					{
						continue;
					}
					else if (Convert.ToInt32(node.Text) == e.LibraryEntry.Year)
					{
						break;
					}
					else if (Convert.ToInt32(node.Text) > e.LibraryEntry.Year)
					{
						yearsNode.Nodes.Insert(node.Index, e.LibraryEntry.Year.ToString());
						break;
					}
					else if (node.NextNode == null)
					{
						yearsNode.Nodes.Add(e.LibraryEntry.Year.ToString());
					}
				}
			}
		}

		private void Player_SongOpened(object sender, SongEventArgs e)
		{
			if (e.Song == null)
			{
				return;
			}

			if (m_autoTrackNowPlayingInTree)
			{
				if (e.Song.FileName.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
				{
					if (internetRadioNode == null)
						return;
					if (tvwLibrary.SelectedNode == internetRadioNode)
					{
						treeView1_AfterSelect(this, new TreeViewEventArgs(internetRadioNode));
					}
					else
					{
						tvwLibrary.SelectedNode = internetRadioNode;
					}
					internetRadioNode.EnsureVisible();
				}
				else
				{
					if (m_trackByArtist)
					{
						if (m_trackContributing)
						{
							FindAndSelect(contributingArtistsNode, e.Song.Artist);
						}
						else
						{
							FindAndSelect(artistsAlbumsNode, e.Song.AlbumArtist);
						}
					}
					else if (m_trackByAlbum)
					{
						if (m_trackContributing)
						{
							FindAndSelect(albumsNode, e.Song.Album);
						}
						else
						{
							FindAndSelect(artistsAlbumsNode, e.Song.AlbumArtist, e.Song.Album);
						}
					}
				}
				if (songListView.Visible)
				{
					foreach (SongListViewItem item in songListView.Items)
					{
						if (item.SongInfo.FileName.Equals(Player.CurrentSong.FileName))
						{
							item.EnsureVisible();
							break;
						}
					}
				}
			}
			else if (m_autoChangeToNowPlaying)
			{
				tvwLibrary.SelectedNode = nowPlayingNode;
			}
			else
			{
				if ((tvwLibrary.SelectedNode == statisticsNode) || (tvwLibrary.SelectedNode != null && tvwLibrary.SelectedNode.Parent != null && tvwLibrary.SelectedNode.Parent == playListNode))
				{
					treeView1_AfterSelect(this, new TreeViewEventArgs(tvwLibrary.SelectedNode));
				}
			}
		}

		#endregion

		#region TreeView Methods and Events

		private void FindAndSelect(TreeNode parentNode, string toFind)
		{
            if (parentNode == null) return;
			foreach (TreeNode node in parentNode.Nodes)
			{
				if (node.Text.Equals(toFind, StringComparison.InvariantCultureIgnoreCase))
				{
					if (tvwLibrary.SelectedNode == node)
					{
						treeView1_AfterSelect(this, new TreeViewEventArgs(node));
					}
					else
					{
						tvwLibrary.SelectedNode = node;
					}
					node.EnsureVisible();
					return;
				}
			}

			// havent found it, so use the first node (the "No" node)
			TreeNode n = parentNode.Nodes[0];
			if (tvwLibrary.SelectedNode == n)
			{
				treeView1_AfterSelect(this, new TreeViewEventArgs(n));
			}
			else
			{
				tvwLibrary.SelectedNode = n;
			}
			return;
		}

		private void FindAndSelect(TreeNode parentNode, string artist, string album)
		{
			if (parentNode == null) return;
			// Try to find the artist name
			foreach (TreeNode artistNode in parentNode.Nodes)
			{
				if (artistNode.Text.Equals(artist, StringComparison.InvariantCultureIgnoreCase))
				{
					// The artist was found, make sure their albums are loaded
					EnsureAlbumsAreLoaded(artistNode);

					// And try to find the album
					foreach (TreeNode albumNode in artistNode.Nodes)
					{
						if (albumNode.Text.Equals(album, StringComparison.InvariantCultureIgnoreCase))
						{
							if (tvwLibrary.SelectedNode == albumNode)
							{
								treeView1_AfterSelect(this, new TreeViewEventArgs(albumNode));
							}
							else
							{
								tvwLibrary.SelectedNode = albumNode;
							}
							albumNode.EnsureVisible();
							return;
						}
					}
				}
			}

			// havent found it, so use the first artistNode (the "No" artistNode)
			TreeNode n = parentNode.Nodes[0];
			if (tvwLibrary.SelectedNode == n)
			{
				treeView1_AfterSelect(this, new TreeViewEventArgs(n));
			}
			else
			{
				tvwLibrary.SelectedNode = n;
			}
			return;
		}

		private void treeView1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button ==  MouseButtons.Right)
			{
				if (tvwLibrary.GetNodeAt(e.Location) != tvwLibrary.SelectedNode)
				{
					tvwLibrary.SelectedNode = tvwLibrary.GetNodeAt(e.Location);
				}
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			songListView.FlatMode = false;
			UseWaitCursor = true;
			if (e.Node == nowPlayingNode)
			{
				infoControl.BringToFront();
			}
			else if (e.Node == searchNode)
			{
				searchControl.BringToFront();
			}
			else if (e.Node == playListNode)
			{
				playlistControl.BringToFront();
			}
			else if (e.Node.Parent == playListNode)
			{
				if (e.Node.Text.Equals("Top Played"))
				{
					songListView.BringToFront();
					songListView.FlatMode = true;
					songListView.DataSource = Library.GetByPlayCount(m_numRecentSongs);
				}
				else if (e.Node.Text.Equals("Recently Played"))
				{
					songListView.BringToFront();
					songListView.FlatMode = true;
					songListView.DataSource = Library.GetBypDatePlayed(m_numRecentSongs);
				}
				else if (e.Node.Text.Equals("Recently Added"))
				{
					songListView.BringToFront();
					songListView.FlatMode = true;
					songListView.DataSource = Library.GetBypDateAdded(m_numRecentSongs);
				}
                else if (e.Node.Text.Equals("Forgotten Gems"))
				{
					songListView.BringToFront();
					songListView.FlatMode = true;
					songListView.DataSource = Library.GetOldByPlayCount(m_numRecentSongs);
				}
                else if (e.Node == statisticsNode) 
                {
                    statisticsControl.BringToFront();
					statisticsControl.ShowStats();
                }
				else if (e.Node == ignoredTracksNode)
				{
					songListView.BringToFront();
					songListView.DataSource = Library.QueryLibrary("Ignored = 1");
				}
				else if (e.Node == deletedTracksNode)
				{
					songListView.BringToFront();
					songListView.DataSource = Library.GetDeletedSongs();
				}
			}
			else if (e.Node == internetRadioNode)
			{
				songListView.BringToFront();
				songListView.FlatMode = true;
				songListView.DataSource = Library.GetInternetRadios();
			}
			else
			{
				songListView.BringToFront();
				if (e.Node.Level == 1 && e.Node.Parent == albumsNode)
				{
					LibraryEntry[] songs;
					if (e.Node.Text == "(No Album)")
					{
						songs = Library.QueryLibrary("Album IS NULL OR Album LIKE ''", m_hideIgnoredSongs);
					}
					else
					{
						string album = e.Node.Text;
						songs = Library.QueryLibrary("Album LIKE '" + album.Replace("'", "''") + "'", m_hideIgnoredSongs);
					}
					songListView.DataSource = songs;
				}
				else if (e.Node.Level == 1 && e.Node.Parent == contributingArtistsNode)
				{
					LibraryEntry[] songs;
					if (e.Node.Text == "(No Artist)")
					{
						songs = Library.QueryLibrary("Artist IS NULL OR Artist LIKE ''", m_hideIgnoredSongs);
					}
					else
					{
						string artist = e.Node.Text;
						songs = Library.QueryLibrary("Artist LIKE '" + artist.Replace("'", "''") + "'", m_hideIgnoredSongs);
					}
					songListView.DataSource = songs;
				}
				else if (e.Node.Level == 1 && e.Node.Parent == artistsAlbumsNode)
				{
					LibraryEntry[] songs;
					if (e.Node.Text == "(No Artist)")
					{
						songs = Library.QueryLibrary("AlbumArtist IS NULL OR Artist LIKE ''", m_hideIgnoredSongs);
					}
					else
					{
						string artist = e.Node.Text;
						songs = Library.QueryLibrary("AlbumArtist LIKE '" + artist.Replace("'", "''") + "'", m_hideIgnoredSongs);
					}
					songListView.DataSource = songs;
				}
				else if (e.Node.Level == 2 && e.Node.Parent.Parent == artistsAlbumsNode)
				{
					LibraryEntry[] songs;
					if (e.Node.Text == "(No Album)")
					{
						songs = Library.QueryLibrary("AlbumArtist LIKE '" + e.Node.Parent.Text.Replace("'", "''") + "' AND (Album IS NULL OR Album LIKE '')", m_hideIgnoredSongs);
					}
					else
					{
						songs = Library.QueryLibrary("AlbumArtist LIKE '" + e.Node.Parent.Text.Replace("'", "''") + "' AND Album LIKE '" + e.Node.Text.Replace("'", "''") + "'", m_hideIgnoredSongs);
					}
					songListView.DataSource = songs;
				}
				else if (e.Node.Level == 1 && e.Node.Parent == genresNode)
				{
					LibraryEntry[] songs;
					if (e.Node.Text == "(No Genre)")
					{
						songs = Library.QueryLibrary("Genre ISNULL OR Genre LIKE ''", m_hideIgnoredSongs);
					}
					else
					{
						songs = Library.QueryLibrary("Genre LIKE '" + e.Node.Text.Replace("'", "''") + "'", m_hideIgnoredSongs);
					}
					songListView.DataSource = songs;
				}
				else if (e.Node.Level == 1 && e.Node.Parent == yearsNode)
				{
					LibraryEntry[] songs;
					if (e.Node.Text == "(No Year)")
					{
						songs = Library.QueryLibrary("Year ISNULL OR Year <= 0", m_hideIgnoredSongs);
					}
					else
					{
						songs = Library.QueryLibrary("Year LIKE " + e.Node.Text.Replace("'", "''") + "", m_hideIgnoredSongs);
					}
					songListView.DataSource = songs;
				}
				else
				{
					songListView.DataSource = null;
				}
			}

			UseWaitCursor = false;
		}

		private void tvwLibrary_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (e.Node.Parent == artistsAlbumsNode)
			{
				EnsureAlbumsAreLoaded(e.Node);
			}
		}

		private void EnsureAlbumsAreLoaded(TreeNode artistNode)
		{
			if (artistNode.Nodes.Count == 1 && artistNode.Nodes[0].Text == LibraryForm.EmptyNodeText)
			{
				artistNode.Nodes.Clear();

				// Load the albums for this artist
				string[] albums = Library.GetAlbums(artistNode.Text);
				string lastNode = "";
				// Sort the albums using our custom sorter that strings The from the start of strings
				Array.Sort<string>(albums, CompareStringsWithoutTheAndIgnoringCase);
				foreach (string album in albums)
				{
					if (String.IsNullOrEmpty(album))
					{
						artistNode.Nodes.Add("(No Album)");
					}
					else
					{
						if (!album.Equals(lastNode, StringComparison.InvariantCultureIgnoreCase))
						{
							lastNode = album;
							artistNode.Nodes.Add(album);
						}
					}
				}
			}
		}

		private int CompareStringsWithoutTheAndIgnoringCase(string strA, string strB)
		{
			string input = strA;
			string input2 = strB;
			if (input.StartsWith("The ", StringComparison.InvariantCultureIgnoreCase))
			{
				input = input.Substring(4) + ", The";
			}
			if (input2.StartsWith("The ", StringComparison.InvariantCultureIgnoreCase))
			{
				input2 = input2.Substring(4) + ", The";
			}
			return string.Compare(input, input2, true);
		}



		#endregion

		#region Control Events

		private void searchControl1_SongQueued(object sender, EventArgs e)
		{
			if (m_autoChangeToPlaylistAfterQueuing)
			{
				tvwLibrary.SelectedNode = playListNode;
			}
		}

		private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			Registry.SetValue("LibraryForm.SplitterDistance", splitContainer.SplitterDistance);
		}

		private void pctShowTree_Click(object sender, EventArgs e)
		{
			splitContainer.Panel1Collapsed = !splitContainer.Panel1Collapsed;
			Registry.SetValue("LibraryForm.ShowTree", !splitContainer.Panel1Collapsed);
		}

		#endregion

		#region Menu Event Handlers

		private void refreshLibraryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadLibraryTree();
		}

		private void queueToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//foreach (LibraryEntry song in songListView.DataSource)
			//{
			Player.Playlist.AddToEnd(songListView.DataSource);
			//}
			if (m_autoChangeToPlaylistAfterQueuing)
			{
				tvwLibrary.SelectedNode = playListNode;
			}
		}

		private void playFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Player.PlayFile(songListView.SelectedItems[0].SongInfo.FileName);
		}

		private void openContainingFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (songListView.SelectedItems.Count > 0)
			{
				System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(songListView.SelectedItems[0].SongInfo.FileName));
			}
		}

		private void queueFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (SongListViewItem songItem in songListView.SelectedItems)
			{
				if (Player.Playlist.Contains(songItem.SongInfo.FileName))
				{
					Player.Playlist.Remove(songItem.SongInfo.FileName);
				}
				else
				{
					Player.Playlist.AddToEnd(songItem.SongInfo);
				}
				if (m_autoChangeToPlaylistAfterQueuing)
				{
					tvwLibrary.SelectedNode = playListNode;
				}
			}
		}

		private void mnuQueueAlbum_Click(object sender, EventArgs e)
		{
			List<string> albums = new List<string>();
			foreach (SongListViewItem item in songListView.SelectedItems)
			{
				if (!albums.Contains(item.SongInfo.Album))
				{
					albums.Add(item.SongInfo.Album);
				}
			}

			if (albums.Count > 0)
			{
				foreach (string album in albums)
				{
					LibraryEntry[] songs = Library.QueryLibrary("Album LIKE '" + album.Replace("'", "''") + "'");

					Player.Playlist.EventsEnabled = false;
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
					Player.Playlist.EventsEnabled = true;

					if (m_autoChangeToPlaylistAfterQueuing)
					{
						tvwLibrary.SelectedNode = playListNode;
					}
					//else
					//{
					//    // we're still in the tree, so refresh it and ensure the selected one is still selected
					//    Player_SongOpened(this, new SongEventArgs(Player.CurrentSong));
					//}
				}
			}
		}

		private void downloadAlbumArtToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AlbumArtPicker f = new AlbumArtPicker(songListView.SelectedItems[0].SongInfo);
			if (!f.IsDisposed)
			{
				f.Player = Player;
				f.Show(this);
			}
		}

		private void mnuEditTrackInformation_Click(object sender, EventArgs e)
		{
			if (songListView.SelectedItems.Count > 0)
			{
				LibraryEntry[] songs = new LibraryEntry[songListView.SelectedItems.Count];
				for (int i = 0; i < songListView.SelectedItems.Count; i++)
				{
					songs[i] = songListView.SelectedItems[i].SongInfo as LibraryEntry;
				}

				InfoEditForm f = new InfoEditForm(songs);
				f.Library = this.Library;
				f.Player = this.Player;
				f.Show(this);
			}
		}

		private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
		{
			if (songListView.SelectedItems.Count == 0 || String.IsNullOrEmpty(Registry.GetValue("General.CustomRightClickOptionCommand", "")))
			{
				customSep.Visible = false;
				customToolStripMenuItem.Visible = false;
			}
			else
			{
				customToolStripMenuItem.Text = Registry.GetValue("General.CustomRightClickOptionName", "");
			}

			if (songListView.SelectedItems.Count > 0)
			{
				jumpToToolStripMenuItem.Enabled = true;
				SongInfo song = songListView.SelectedItems[0].SongInfo;
				jumpToToolStripMenuItem.Tag = song;
				artistalbumToolStripMenuItem.Text = "Artist\\Album: " + song.AlbumArtist.Replace("&", "&&") + "\\" + song.Album.Replace("&", "&&");
				albumToolStripMenuItem.Text = "Album: " + song.Album.Replace("&", "&&");
				artistToolStripMenuItem.Text = "Artist: " + song.Artist.Replace("&", "&&");
				albumArtistToolStripMenuItem.Text = "Album Artist: " + song.AlbumArtist.Replace("&", "&&");
				genreToolStripMenuItem.Text = "Genre: " + song.Genre.Replace("&", "&&");
				yearToolStripMenuItem.Text = "Year: " + song.Year.ToString();
			}
			else
			{
				jumpToToolStripMenuItem.Enabled = false;
			}
		}

		private void artistalbumToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SongInfo song = jumpToToolStripMenuItem.Tag as SongInfo;
			FindAndSelect(artistsAlbumsNode, song.AlbumArtist, song.Album);
		}

		private void albumToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SongInfo song = jumpToToolStripMenuItem.Tag as SongInfo;
			FindAndSelect(albumsNode, song.Album);
		}

		private void artistToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SongInfo song = jumpToToolStripMenuItem.Tag as SongInfo;
			FindAndSelect(contributingArtistsNode, song.Artist);
		}

		private void genreToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SongInfo song = jumpToToolStripMenuItem.Tag as SongInfo;
			FindAndSelect(genresNode, song.Genre);
		}

		private void yearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SongInfo song = jumpToToolStripMenuItem.Tag as SongInfo;
			FindAndSelect(yearsNode, song.Year.ToString());
		}

		private void albumArtistToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SongInfo song = jumpToToolStripMenuItem.Tag as SongInfo;
			FindAndSelect(artistsAlbumsNode, song.AlbumArtist);
		}

		private void nowPlayingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			tvwLibrary.SelectedNode = nowPlayingNode;
			if (!tvwLibrary.IsHandleCreated)
			{
				treeView1_AfterSelect(this, new TreeViewEventArgs(nowPlayingNode));
			}
		}

		private void topPlayedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			tvwLibrary.SelectedNode = playListNode.Nodes[0];
			if (!tvwLibrary.IsHandleCreated)
			{
				treeView1_AfterSelect(this, new TreeViewEventArgs(playListNode.Nodes[0]));
			}
		}

		private void recentlyPlayedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			tvwLibrary.SelectedNode = playListNode.Nodes[1];
			if (!tvwLibrary.IsHandleCreated)
			{
				treeView1_AfterSelect(this, new TreeViewEventArgs(playListNode.Nodes[1]));
			}
		}

		private void recentlyAddedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (playListNode != null)
			{
				tvwLibrary.SelectedNode = playListNode.Nodes[2];
				if (!tvwLibrary.IsHandleCreated)
				{
					treeView1_AfterSelect(this, new TreeViewEventArgs(playListNode.Nodes[2]));
				}
			}
		}

		private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			tvwLibrary.SelectedNode = statisticsNode;
			if (!tvwLibrary.IsHandleCreated)
			{
				treeView1_AfterSelect(this, new TreeViewEventArgs(statisticsNode));
			}
		}

		private void ignoredTracksToolStripMenuItem_Click(object sender, EventArgs e)
		{
			tvwLibrary.SelectedNode = ignoredTracksNode;
			if (!tvwLibrary.IsHandleCreated)
			{
				treeView1_AfterSelect(this, new TreeViewEventArgs(ignoredTracksNode));
			}
		}

		private void deletedTracksToolStripMenuItem_Click(object sender, EventArgs e)
		{
			tvwLibrary.SelectedNode = deletedTracksNode;
			if (!tvwLibrary.IsHandleCreated)
			{
				treeView1_AfterSelect(this, new TreeViewEventArgs(deletedTracksNode));
			}
		}	

		private void playlistToolStripMenuItem_Click(object sender, EventArgs e)
		{
			tvwLibrary.SelectedNode = playListNode;
			if (!tvwLibrary.IsHandleCreated)
			{
				treeView1_AfterSelect(this, new TreeViewEventArgs(playListNode));
			}
		}

		private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
		{
			if (Player.CurrentSong != null)
			{
				jumpTo2ToolStripMenuItem.Enabled = true;
				SongInfo song = Player.CurrentSong;
				jumpToToolStripMenuItem.Tag = song;
				toolStripMenuItem2.Text = "Artist\\Album: " + song.AlbumArtist.Replace("&", "&&") + "\\" + song.Album.Replace("&", "&&");
				toolStripMenuItem3.Text = "Album: " + song.Album.Replace("&", "&&");
				toolStripMenuItem4.Text = "Artist: " + song.Artist.Replace("&", "&&");
				toolStripMenuItem5.Text = "Album Artist: " + song.AlbumArtist.Replace("&", "&&");
				toolStripMenuItem6.Text = "Genre: " + song.Genre.Replace("&", "&&");
				toolStripMenuItem7.Text = "Year: " + song.Year.ToString();
			}
			else
			{
				jumpTo2ToolStripMenuItem.Enabled = false;
			}
		}

		#endregion

		private void searchLibraryContextMenuItem_Click(object sender, EventArgs e)
		{
			tvwLibrary.SelectedNode = searchNode;
			if (!tvwLibrary.IsHandleCreated)
			{
				treeView1_AfterSelect(this, new TreeViewEventArgs(searchNode));
			}
		}

		private void mnuReadTrackInformation_Click(object sender, EventArgs e)
		{
			songListView.SelectedItems.ForEach(delegate(SongListViewItem item)
			{
				LibraryEntry entry = new LibraryEntry(item.SongInfo.FileName);
				Library.Update(entry);
			});			
		}

		private void ignoreUnIgnoreSongToolStripMenuItem_Click(object sender, EventArgs e)
		{
			songListView.SelectedItems.ForEach(delegate(SongListViewItem item)
				{
					if (item.SongInfo.Ignored)
					{
						Library.UnIgnore(item.SongInfo.FileName);
					}
					else
					{
						Library.Ignore(item.SongInfo.FileName);
					}
				});
		}

		private void deleteUnDeleteSongToolStripMenuItem_Click(object sender, EventArgs e)
		{
			songListView.SelectedItems.ForEach(delegate(SongListViewItem item)
			{
				Library.Delete(item.SongInfo.FileName);
				songListView.Items.Remove(item);
			});
			songListView.Refresh();
		}

		private void customToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(Registry.GetValue("General.CustomRightClickOptionCommand", ""), "\"" + songListView.SelectedItems[0].SongInfo.FileName + "\"");
		}

		private void downloadLyricsToolStripMenuItem_Click(object sender, EventArgs e)
		{
            songListView.SelectedItems.ForEach((Action<SongListViewItem>)delegate (SongListViewItem song)
			{
				if (song.SongInfo is LibraryEntry)
				{
                    var entry = song.SongInfo as LibraryEntry;
					if (!string.IsNullOrEmpty(entry.Lyrics))
					{
                        MessageBox.Show(entry.Lyrics, "Lyrics for: " + entry.ToString());
						return;
					}
				}

                ThreePM.Utilities.LyricsHelper helper = new ThreePM.Utilities.LyricsHelper(this.Library);

				helper.LyricsFound += delegate
				{
					global::System.Windows.Forms.MessageBox.Show((string)helper.LastLyrics, (string)("Lyrics for: " + helper.Song.ToString()));
				};

				helper.LyricsNotFound += delegate
				{
					global::System.Windows.Forms.MessageBox.Show((string)"Could not find lyrics", (string)"Lyrics Not Found");
				};

				helper.LoadLyrics(song.SongInfo);
			});
		}
	}
}
