using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ThreePM.MusicLibrary;
using ThreePM.MusicPlayer;
using ThreePM.UI;

namespace ThreePM
{
    public partial class LibraryForm : ThreePM.BaseForm
    {
        #region Constants

        private const string EmptyNodeText = "++";

        #endregion Constants

        #region Declarations

        private TreeNode _artistsAlbumsNode;
        private TreeNode _albumsNode;
        private TreeNode _contributingArtistsNode;
        private TreeNode _internetRadioNode;
        private TreeNode _playListNode;
        private TreeNode _searchNode;
        private TreeNode _nowPlayingNode;
        private TreeNode _genresNode;
        private TreeNode _yearsNode;
        private TreeNode _statisticsNode;
        private TreeNode _ignoredTracksNode;
        private TreeNode _deletedTracksNode;

        private int _numRecentSongs = 50;
        private bool _autoChangeToNowPlaying;
        private bool _autoChangeToPlaylistAfterQueuing;
        private bool _autoTrackNowPlayingInTree;
        private bool _trackByArtist;
        private bool _trackByAlbum;
        private bool _trackContributing;
        private bool _hideIgnoredSongs;

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
            _autoChangeToNowPlaying = Convert.ToBoolean(Registry.GetValue("LibraryForm.NowPlayingAfterPlay", true));
            _autoChangeToPlaylistAfterQueuing = Convert.ToBoolean(Registry.GetValue("LibraryForm.PlaylistAfterQueue", false));
            _autoTrackNowPlayingInTree = Convert.ToBoolean(Registry.GetValue("LibraryForm.TrackNowPlayingInTree", true));
            _trackByArtist = Convert.ToBoolean(Registry.GetValue("LibraryForm.TrackByArtist", false));
            _trackByAlbum = Convert.ToBoolean(Registry.GetValue("LibraryForm.TrackByAlbum", true));

            _hideIgnoredSongs = Convert.ToBoolean(Registry.GetValue("LibraryForm.HideIgnoredSongs", false));
            _trackContributing = Convert.ToBoolean(Registry.GetValue("LibraryForm.TrackContributing", false));
            splitContainer.Panel1Collapsed = !Convert.ToBoolean(Registry.GetValue("LibraryForm.ShowTree", true));
            songListView.ShowAlbumArt = Convert.ToBoolean(Registry.GetValue("LibraryForm.ShowAlbumArt", true));
            songListView.AlbumArtSize = Convert.ToInt32(Registry.GetValue("LibraryForm.AlbumArtSize", 100));
            _numRecentSongs = Convert.ToInt32(Registry.GetValue("LibraryForm.SongsToShowInRecentLists", 50));
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
                Player_SongOpened(this, new SongEventArgs(this.Player.CurrentSong));
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        protected override void InitLibrary()
        {
            playlistControl.Library = this.Library;
            searchControl.Library = this.Library;
            infoControl.Library = this.Library;
            songListView.Library = this.Library;
            statisticsControl.Library = this.Library;

            this.Library.LibraryUpdated += new EventHandler<LibraryEntryEventArgs>(Library_LibraryUpdated);
            LoadLibraryTree();
        }

        protected override void UnInitLibrary()
        {
            this.Library.LibraryUpdated -= new EventHandler<LibraryEntryEventArgs>(Library_LibraryUpdated);
        }

        protected override void InitPlayer()
        {
            playlistControl.Player = this.Player;
            searchControl.Player = this.Player;
            infoControl.Player = this.Player;
            songListView.Player = this.Player;
            statisticsControl.Player = this.Player;

            this.Player.SongOpened += new EventHandler<SongEventArgs>(Player_SongOpened);
            if (this.Player.CurrentSong != null)
            {
                Player_SongOpened(this, new SongEventArgs(this.Player.CurrentSong));
            }
        }

        protected override void UnInitPlayer()
        {
            this.Player.SongOpened -= new EventHandler<SongEventArgs>(Player_SongOpened);
        }

        #endregion

        #region Library Loading Methods

        private void LoadLibraryTree()
        {
            MethodInvoker DoWork = delegate
            {
                this.UseWaitCursor = true;
                // Load the artists
                var albumArtists = this.Library.GetAlbumArtists();
                Array.Sort<string>(albumArtists, CompareStringsWithoutTheAndIgnoringCase);
                var albums = this.Library.GetAlbums();
                Array.Sort<string>(albums, CompareStringsWithoutTheAndIgnoringCase);
                var artists = this.Library.GetArtists();
                Array.Sort<string>(artists, CompareStringsWithoutTheAndIgnoringCase);
                var years = this.Library.GetYears();
                var genres = this.Library.GetGenres();

                tvwLibrary.Invoke((MethodInvoker)delegate
                {
                    tvwLibrary.BeginUpdate();
                    tvwLibrary.Nodes.Clear();
                    LoadTopLevelNodes();
                });

                LoadNodes(_artistsAlbumsNode, albumArtists, "Artist", true);
                LoadNodes(_albumsNode, albums, "Album", false);
                LoadNodes(_contributingArtistsNode, artists, "Artist", false);
                LoadNodes(_yearsNode, years, "Year", false);
                LoadNodes(_genresNode, genres, "Genre", false);

                tvwLibrary.BeginInvoke((MethodInvoker)delegate
                {
                    lblLoading.Visible = false;
                    tvwLibrary.EndUpdate();

                    Player_SongOpened(this, new SongEventArgs(this.Player.CurrentSong));
                });

                this.UseWaitCursor = false;
            };

            IntPtr t = tvwLibrary.Handle;
            DoWork.BeginInvoke(null, null);
        }

        private void LoadTopLevelNodes()
        {
            _nowPlayingNode = tvwLibrary.Nodes.Add("Now Playing");
            _nowPlayingNode.ImageKey = "NowPlaying";
            _nowPlayingNode.SelectedImageKey = _nowPlayingNode.ImageKey;

            _searchNode = tvwLibrary.Nodes.Add("Search Library");
            _searchNode.ImageKey = "Search";
            _searchNode.SelectedImageKey = _searchNode.ImageKey;

            _playListNode = tvwLibrary.Nodes.Add("Playlist");
            _playListNode.ImageKey = "Playlist";
            _playListNode.SelectedImageKey = _playListNode.ImageKey;

            _playListNode.Nodes.Add("Top Played");
            _playListNode.Nodes.Add("Recently Played");
            _playListNode.Nodes.Add("Recently Added");
            _playListNode.Nodes.Add("Forgotten Gems");

            _statisticsNode = _playListNode.Nodes.Add("Statistics");

            _ignoredTracksNode = _playListNode.Nodes.Add("Ignored Tracks");

            _deletedTracksNode = _playListNode.Nodes.Add("Deleted Tracks");

            foreach (TreeNode node in _playListNode.Nodes)
            {
                node.ImageKey = "Playlist";
                node.SelectedImageKey = "Playlist";
            }

            _internetRadioNode = tvwLibrary.Nodes.Add("Internet Radio");
            _internetRadioNode.ImageKey = "Radio";
            _internetRadioNode.SelectedImageKey = _internetRadioNode.ImageKey;

            _artistsAlbumsNode = tvwLibrary.Nodes.Add("Artists/Albums");
            _artistsAlbumsNode.ImageKey = "Music";
            _artistsAlbumsNode.SelectedImageKey = _artistsAlbumsNode.ImageKey;

            _albumsNode = tvwLibrary.Nodes.Add("Albums");
            _albumsNode.ImageKey = "Music";
            _albumsNode.SelectedImageKey = _albumsNode.ImageKey;

            _contributingArtistsNode = tvwLibrary.Nodes.Add("Contributing Artist");
            _contributingArtistsNode.ImageKey = "Music";
            _contributingArtistsNode.SelectedImageKey = _contributingArtistsNode.ImageKey;

            _yearsNode = tvwLibrary.Nodes.Add("Years");
            _yearsNode.ImageKey = "Music";
            _yearsNode.SelectedImageKey = _yearsNode.ImageKey;

            _genresNode = tvwLibrary.Nodes.Add("Genres");
            _genresNode.ImageKey = "Music";
            _genresNode.SelectedImageKey = _genresNode.ImageKey;
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
                if (!string.IsNullOrEmpty(node) && !node.Equals(lastNode, StringComparison.OrdinalIgnoreCase))
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
            if (!string.IsNullOrEmpty(e.LibraryEntry.AlbumArtist) && _artistsAlbumsNode != null)
            {
                foreach (TreeNode node in _artistsAlbumsNode.Nodes)
                {
                    if (node.Text.Equals(e.LibraryEntry.AlbumArtist, StringComparison.OrdinalIgnoreCase))
                    {
                        if (node.Nodes.Count > 1 || node.Nodes[0].Text != EmptyNodeText)
                        {
                            if (!string.IsNullOrEmpty(e.LibraryEntry.Album))
                            {
                                foreach (TreeNode node2 in node.Nodes)
                                {
                                    if (node2.Text.Equals(e.LibraryEntry.Album, StringComparison.OrdinalIgnoreCase))
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
                        _artistsAlbumsNode.Nodes.Insert(node.Index, e.LibraryEntry.AlbumArtist).Nodes.Add(EmptyNodeText);
                        break;
                    }
                    else if (node.NextNode == null)
                    {
                        _artistsAlbumsNode.Nodes.Add(e.LibraryEntry.AlbumArtist).Nodes.Add(EmptyNodeText);
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(e.LibraryEntry.Album) && _albumsNode != null)
            {
                foreach (TreeNode node in _albumsNode.Nodes)
                {
                    if (node.Text.Equals(e.LibraryEntry.Album, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                    else if (CompareStringsWithoutTheAndIgnoringCase(node.Text, e.LibraryEntry.Album) > 0)
                    {
                        _albumsNode.Nodes.Insert(node.Index, e.LibraryEntry.Album);
                        break;
                    }
                    else if (node.NextNode == null)
                    {
                        _albumsNode.Nodes.Add(e.LibraryEntry.Album);
                    }

                }
            }

            if (!string.IsNullOrEmpty(e.LibraryEntry.Artist) && _contributingArtistsNode != null)
            {
                foreach (TreeNode node in _contributingArtistsNode.Nodes)
                {
                    if (node.Text.Equals(e.LibraryEntry.Artist, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                    else if (CompareStringsWithoutTheAndIgnoringCase(node.Text, e.LibraryEntry.Artist) > 0)
                    {
                        _contributingArtistsNode.Nodes.Insert(node.Index, e.LibraryEntry.Artist);
                        break;
                    }
                    else if (node.NextNode == null)
                    {
                        _contributingArtistsNode.Nodes.Add(e.LibraryEntry.Artist);
                    }

                }
            }

            if (!string.IsNullOrEmpty(e.LibraryEntry.Genre) && _genresNode != null)
            {
                foreach (TreeNode node in _genresNode.Nodes)
                {
                    if (node.Text.Equals(e.LibraryEntry.Genre, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                    else if (node.Text.CompareTo(e.LibraryEntry.Genre) > 0)
                    {
                        _genresNode.Nodes.Insert(node.Index, e.LibraryEntry.Genre);
                        break;
                    }
                    else if (node.NextNode == null)
                    {
                        _genresNode.Nodes.Add(e.LibraryEntry.Genre);
                    }

                }
            }

            if (e.LibraryEntry.Year > 0 && _yearsNode != null)
            {
                foreach (TreeNode node in _yearsNode.Nodes)
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
                        _yearsNode.Nodes.Insert(node.Index, e.LibraryEntry.Year.ToString());
                        break;
                    }
                    else if (node.NextNode == null)
                    {
                        _yearsNode.Nodes.Add(e.LibraryEntry.Year.ToString());
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

            if (_autoTrackNowPlayingInTree)
            {
                if (e.Song.FileName.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                {
                    if (_internetRadioNode == null)
                        return;
                    if (tvwLibrary.SelectedNode == _internetRadioNode)
                    {
                        treeView1_AfterSelect(this, new TreeViewEventArgs(_internetRadioNode));
                    }
                    else
                    {
                        tvwLibrary.SelectedNode = _internetRadioNode;
                    }
                    _internetRadioNode.EnsureVisible();
                }
                else
                {
                    if (_trackByArtist)
                    {
                        if (_trackContributing)
                        {
                            FindAndSelect(_contributingArtistsNode, e.Song.Artist);
                        }
                        else
                        {
                            FindAndSelect(_artistsAlbumsNode, e.Song.AlbumArtist);
                        }
                    }
                    else if (_trackByAlbum)
                    {
                        if (_trackContributing)
                        {
                            FindAndSelect(_albumsNode, e.Song.Album);
                        }
                        else
                        {
                            FindAndSelect(_artistsAlbumsNode, e.Song.AlbumArtist, e.Song.Album);
                        }
                    }
                }
                if (songListView.Visible)
                {
                    foreach (SongListViewItem item in songListView.Items)
                    {
                        if (item.SongInfo.FileName.Equals(this.Player.CurrentSong.FileName))
                        {
                            item.EnsureVisible();
                            break;
                        }
                    }
                }
            }
            else if (_autoChangeToNowPlaying)
            {
                tvwLibrary.SelectedNode = _nowPlayingNode;
            }
            else
            {
                if ((tvwLibrary.SelectedNode == _statisticsNode) || (tvwLibrary.SelectedNode != null && tvwLibrary.SelectedNode.Parent != null && tvwLibrary.SelectedNode.Parent == _playListNode))
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
                if (node.Text.Equals(toFind, StringComparison.OrdinalIgnoreCase))
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
                if (artistNode.Text.Equals(artist, StringComparison.OrdinalIgnoreCase))
                {
                    // The artist was found, make sure their albums are loaded
                    EnsureAlbumsAreLoaded(artistNode);

                    // And try to find the album
                    foreach (TreeNode albumNode in artistNode.Nodes)
                    {
                        if (albumNode.Text.Equals(album, StringComparison.OrdinalIgnoreCase))
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
            if (e.Button == MouseButtons.Right)
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
            this.UseWaitCursor = true;
            if (e.Node == _nowPlayingNode)
            {
                infoControl.BringToFront();
            }
            else if (e.Node == _searchNode)
            {
                searchControl.BringToFront();
            }
            else if (e.Node == _playListNode)
            {
                playlistControl.BringToFront();
            }
            else if (e.Node.Parent == _playListNode)
            {
                if (e.Node.Text.Equals("Top Played"))
                {
                    songListView.BringToFront();
                    songListView.FlatMode = true;
                    songListView.DataSource = this.Library.GetByPlayCount(_numRecentSongs);
                }
                else if (e.Node.Text.Equals("Recently Played"))
                {
                    songListView.BringToFront();
                    songListView.FlatMode = true;
                    songListView.DataSource = this.Library.GetBypDatePlayed(_numRecentSongs);
                }
                else if (e.Node.Text.Equals("Recently Added"))
                {
                    songListView.BringToFront();
                    songListView.FlatMode = true;
                    songListView.DataSource = this.Library.GetBypDateAdded(_numRecentSongs);
                }
                else if (e.Node.Text.Equals("Forgotten Gems"))
                {
                    songListView.BringToFront();
                    songListView.FlatMode = true;
                    songListView.DataSource = this.Library.GetOldByPlayCount(_numRecentSongs);
                }
                else if (e.Node == _statisticsNode)
                {
                    statisticsControl.BringToFront();
                    statisticsControl.ShowStats();
                }
                else if (e.Node == _ignoredTracksNode)
                {
                    songListView.BringToFront();
                    songListView.DataSource = this.Library.QueryLibrary("Ignored = 1");
                }
                else if (e.Node == _deletedTracksNode)
                {
                    songListView.BringToFront();
                    songListView.DataSource = this.Library.GetDeletedSongs();
                }
            }
            else if (e.Node == _internetRadioNode)
            {
                songListView.BringToFront();
                songListView.FlatMode = true;
                songListView.DataSource = this.Library.GetInternetRadios();
            }
            else
            {
                songListView.BringToFront();
                if (e.Node.Level == 1 && e.Node.Parent == _albumsNode)
                {
                    LibraryEntry[] songs;
                    if (e.Node.Text == "(No Album)")
                    {
                        songs = this.Library.QueryLibrary("Album IS NULL OR Album LIKE ''", _hideIgnoredSongs);
                    }
                    else
                    {
                        string album = e.Node.Text;
                        songs = this.Library.QueryLibrary("Album LIKE '" + album.Replace("'", "''") + "'", _hideIgnoredSongs);
                    }
                    songListView.DataSource = songs;
                }
                else if (e.Node.Level == 1 && e.Node.Parent == _contributingArtistsNode)
                {
                    LibraryEntry[] songs;
                    if (e.Node.Text == "(No Artist)")
                    {
                        songs = this.Library.QueryLibrary("Artist IS NULL OR Artist LIKE ''", _hideIgnoredSongs);
                    }
                    else
                    {
                        string artist = e.Node.Text;
                        songs = this.Library.QueryLibrary("Artist LIKE '" + artist.Replace("'", "''") + "'", _hideIgnoredSongs);
                    }
                    songListView.DataSource = songs;
                }
                else if (e.Node.Level == 1 && e.Node.Parent == _artistsAlbumsNode)
                {
                    LibraryEntry[] songs;
                    if (e.Node.Text == "(No Artist)")
                    {
                        songs = this.Library.QueryLibrary("AlbumArtist IS NULL OR Artist LIKE ''", _hideIgnoredSongs);
                    }
                    else
                    {
                        string artist = e.Node.Text;
                        songs = this.Library.QueryLibrary("AlbumArtist LIKE '" + artist.Replace("'", "''") + "'", _hideIgnoredSongs);
                    }
                    songListView.DataSource = songs;
                }
                else if (e.Node.Level == 2 && e.Node.Parent.Parent == _artistsAlbumsNode)
                {
                    LibraryEntry[] songs;
                    if (e.Node.Text == "(No Album)")
                    {
                        songs = this.Library.QueryLibrary("AlbumArtist LIKE '" + e.Node.Parent.Text.Replace("'", "''") + "' AND (Album IS NULL OR Album LIKE '')", _hideIgnoredSongs);
                    }
                    else
                    {
                        songs = this.Library.QueryLibrary("AlbumArtist LIKE '" + e.Node.Parent.Text.Replace("'", "''") + "' AND Album LIKE '" + e.Node.Text.Replace("'", "''") + "'", _hideIgnoredSongs);
                    }
                    songListView.DataSource = songs;
                }
                else if (e.Node.Level == 1 && e.Node.Parent == _genresNode)
                {
                    LibraryEntry[] songs;
                    if (e.Node.Text == "(No Genre)")
                    {
                        songs = this.Library.QueryLibrary("Genre ISNULL OR Genre LIKE ''", _hideIgnoredSongs);
                    }
                    else
                    {
                        songs = this.Library.QueryLibrary("Genre LIKE '" + e.Node.Text.Replace("'", "''") + "'", _hideIgnoredSongs);
                    }
                    songListView.DataSource = songs;
                }
                else if (e.Node.Level == 1 && e.Node.Parent == _yearsNode)
                {
                    LibraryEntry[] songs;
                    if (e.Node.Text == "(No Year)")
                    {
                        songs = this.Library.QueryLibrary("Year ISNULL OR Year <= 0", _hideIgnoredSongs);
                    }
                    else
                    {
                        songs = this.Library.QueryLibrary("Year LIKE " + e.Node.Text.Replace("'", "''") + "", _hideIgnoredSongs);
                    }
                    songListView.DataSource = songs;
                }
                else
                {
                    songListView.DataSource = null;
                }
            }

            this.UseWaitCursor = false;
        }

        private void tvwLibrary_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            if (e.Node.Parent == _artistsAlbumsNode)
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
                string[] albums = this.Library.GetAlbums(artistNode.Text);
                string lastNode = "";
                // Sort the albums using our custom sorter that strings The from the start of strings
                Array.Sort<string>(albums, CompareStringsWithoutTheAndIgnoringCase);
                foreach (string album in albums)
                {
                    if (string.IsNullOrEmpty(album))
                    {
                        artistNode.Nodes.Add("(No Album)");
                    }
                    else
                    {
                        if (!album.Equals(lastNode, StringComparison.OrdinalIgnoreCase))
                        {
                            lastNode = album;
                            artistNode.Nodes.Add(album);
                        }
                    }
                }
            }
        }

        private static int CompareStringsWithoutTheAndIgnoringCase(string strA, string strB)
        {
            string input = strA;
            string input2 = strB;
            if (input.StartsWith("The ", StringComparison.OrdinalIgnoreCase))
            {
                input = input.Substring(4) + ", The";
            }
            if (input2.StartsWith("The ", StringComparison.OrdinalIgnoreCase))
            {
                input2 = input2.Substring(4) + ", The";
            }
            return string.Compare(input, input2, true);
        }



        #endregion

        #region Control Events

        private void searchControl1_SongQueued(object sender, EventArgs e)
        {
            if (_autoChangeToPlaylistAfterQueuing)
            {
                tvwLibrary.SelectedNode = _playListNode;
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
            this.Player.Playlist.AddToEnd(songListView.DataSource);
            //}
            if (_autoChangeToPlaylistAfterQueuing)
            {
                tvwLibrary.SelectedNode = _playListNode;
            }
        }

        private void playFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Player.PlayFile(songListView.SelectedItems[0].SongInfo.FileName);
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
                if (this.Player.Playlist.Contains(songItem.SongInfo.FileName))
                {
                    this.Player.Playlist.Remove(songItem.SongInfo.FileName);
                }
                else
                {
                    this.Player.Playlist.AddToEnd(songItem.SongInfo);
                }
                if (_autoChangeToPlaylistAfterQueuing)
                {
                    tvwLibrary.SelectedNode = _playListNode;
                }
            }
        }

        private void mnuQueueAlbum_Click(object sender, EventArgs e)
        {
            var albums = new List<string>();
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
                    LibraryEntry[] songs = this.Library.QueryLibrary("Album LIKE '" + album.Replace("'", "''") + "'");

                    this.Player.Playlist.EventsEnabled = false;
                    foreach (LibraryEntry item in songs)
                    {
                        if (item.FileName.Equals(this.Player.CurrentSong.FileName) || this.Player.Playlist.Contains(item.FileName))
                        {
                            // do nothing
                        }
                        else
                        {
                            this.Player.Playlist.AddToEnd(item);
                        }
                    }
                    this.Player.Playlist.EventsEnabled = true;

                    if (_autoChangeToPlaylistAfterQueuing)
                    {
                        tvwLibrary.SelectedNode = _playListNode;
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
            var f = new AlbumArtPicker(songListView.SelectedItems[0].SongInfo);
            if (!f.IsDisposed)
            {
                f.Player = this.Player;
                f.Show(this);
            }
        }

        private void mnuEditTrackInformation_Click(object sender, EventArgs e)
        {
            if (songListView.SelectedItems.Count > 0)
            {
                var songs = new LibraryEntry[songListView.SelectedItems.Count];
                for (int i = 0; i < songListView.SelectedItems.Count; i++)
                {
                    songs[i] = songListView.SelectedItems[i].SongInfo as LibraryEntry;
                }

                var f = new InfoEditForm(songs);
                f.Library = this.Library;
                f.Player = this.Player;
                f.Show(this);
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (songListView.SelectedItems.Count == 0 || string.IsNullOrEmpty(Registry.GetValue("General.CustomRightClickOptionCommand", "")))
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
            var song = jumpToToolStripMenuItem.Tag as SongInfo;
            FindAndSelect(_artistsAlbumsNode, song.AlbumArtist, song.Album);
        }

        private void albumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var song = jumpToToolStripMenuItem.Tag as SongInfo;
            FindAndSelect(_albumsNode, song.Album);
        }

        private void artistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var song = jumpToToolStripMenuItem.Tag as SongInfo;
            FindAndSelect(_contributingArtistsNode, song.Artist);
        }

        private void genreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var song = jumpToToolStripMenuItem.Tag as SongInfo;
            FindAndSelect(_genresNode, song.Genre);
        }

        private void yearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var song = jumpToToolStripMenuItem.Tag as SongInfo;
            FindAndSelect(_yearsNode, song.Year.ToString());
        }

        private void albumArtistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var song = jumpToToolStripMenuItem.Tag as SongInfo;
            FindAndSelect(_artistsAlbumsNode, song.AlbumArtist);
        }

        private void nowPlayingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvwLibrary.SelectedNode = _nowPlayingNode;
            if (!tvwLibrary.IsHandleCreated)
            {
                treeView1_AfterSelect(this, new TreeViewEventArgs(_nowPlayingNode));
            }
        }

        private void topPlayedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvwLibrary.SelectedNode = _playListNode.Nodes[0];
            if (!tvwLibrary.IsHandleCreated)
            {
                treeView1_AfterSelect(this, new TreeViewEventArgs(_playListNode.Nodes[0]));
            }
        }

        private void recentlyPlayedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvwLibrary.SelectedNode = _playListNode.Nodes[1];
            if (!tvwLibrary.IsHandleCreated)
            {
                treeView1_AfterSelect(this, new TreeViewEventArgs(_playListNode.Nodes[1]));
            }
        }

        private void recentlyAddedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_playListNode != null)
            {
                tvwLibrary.SelectedNode = _playListNode.Nodes[2];
                if (!tvwLibrary.IsHandleCreated)
                {
                    treeView1_AfterSelect(this, new TreeViewEventArgs(_playListNode.Nodes[2]));
                }
            }
        }

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvwLibrary.SelectedNode = _statisticsNode;
            if (!tvwLibrary.IsHandleCreated)
            {
                treeView1_AfterSelect(this, new TreeViewEventArgs(_statisticsNode));
            }
        }

        private void ignoredTracksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvwLibrary.SelectedNode = _ignoredTracksNode;
            if (!tvwLibrary.IsHandleCreated)
            {
                treeView1_AfterSelect(this, new TreeViewEventArgs(_ignoredTracksNode));
            }
        }

        private void deletedTracksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvwLibrary.SelectedNode = _deletedTracksNode;
            if (!tvwLibrary.IsHandleCreated)
            {
                treeView1_AfterSelect(this, new TreeViewEventArgs(_deletedTracksNode));
            }
        }

        private void playlistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvwLibrary.SelectedNode = _playListNode;
            if (!tvwLibrary.IsHandleCreated)
            {
                treeView1_AfterSelect(this, new TreeViewEventArgs(_playListNode));
            }
        }

        private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
        {
            if (this.Player.CurrentSong != null)
            {
                jumpTo2ToolStripMenuItem.Enabled = true;
                SongInfo song = this.Player.CurrentSong;
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
            tvwLibrary.SelectedNode = _searchNode;
            if (!tvwLibrary.IsHandleCreated)
            {
                treeView1_AfterSelect(this, new TreeViewEventArgs(_searchNode));
            }
        }

        private void mnuReadTrackInformation_Click(object sender, EventArgs e)
        {
            songListView.SelectedItems.ForEach(delegate (SongListViewItem item)
            {
                var entry = new LibraryEntry(item.SongInfo.FileName);
                this.Library.Update(entry);
            });
        }

        private void ignoreUnIgnoreSongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            songListView.SelectedItems.ForEach(delegate (SongListViewItem item)
                {
                    if (item.SongInfo.Ignored)
                    {
                        this.Library.UnIgnore(item.SongInfo.FileName);
                    }
                    else
                    {
                        this.Library.Ignore(item.SongInfo.FileName);
                    }
                });
        }

        private void deleteUnDeleteSongToolStripMenuItem_Click(object sender, EventArgs e)
        {
            songListView.SelectedItems.ForEach(delegate (SongListViewItem item)
            {
                this.Library.Delete(item.SongInfo.FileName);
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
            songListView.SelectedItems.ForEach(delegate (SongListViewItem song)
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

                var helper = new ThreePM.Utilities.LyricsHelper(this.Library);

                helper.LyricsFound += delegate
                {
                    global::System.Windows.Forms.MessageBox.Show(helper.LastLyrics, "Lyrics for: " + helper.Song.ToString());
                };

                helper.LyricsNotFound += delegate
                {
                    global::System.Windows.Forms.MessageBox.Show("Could not find lyrics", "Lyrics Not Found");
                };

                helper.LoadLyrics(song.SongInfo);
            });
        }
    }
}
