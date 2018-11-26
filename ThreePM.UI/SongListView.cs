using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ThreePM.MusicPlayer;
using System.IO;
using System.Drawing.Drawing2D;
using System.Threading;
using ThreePM.MusicLibrary;

namespace ThreePM.UI
{
    public partial class SongListView : UserControl
    {
        #region Declarations

        public event EventHandler ListChanged;
        public event EventHandler SongPlayed;
        public event EventHandler SongQueued;

        private bool _autoDoubleClick = true;
        private int _trackNumberColumnWidth = -1;
        private int _statusColumnWidth = -1;
        private int _titleColumnWidth = -1;
        private int _artistColumnWidth = -1;
        private int _albumColumnWidth = -1;
        private int _playCountColumnWidth = -1;
        private int _durationColumnWidth = -1;

        private Player _player;
        private Library _library;

        private Rectangle _dragBox = Rectangle.Empty;

        #endregion Declarations

        #region Properties

        public bool AutoDoubleClick
        {
            get { return _autoDoubleClick; }
            set { _autoDoubleClick = value; }
        }

        internal int TrackNumberColumnWidth
        {
            get
            {
                if (_trackNumberColumnWidth == -1)
                {
                    _trackNumberColumnWidth = RegistryHelper.GetValue("SongListView.TrackNumberColumnWidth", 25);
                }
                return _trackNumberColumnWidth;
            }
            set
            {
                _trackNumberColumnWidth = value;
                RegistryHelper.SetValue("SongListView.TrackNumberColumnWidth", value);
            }
        }

        internal int StatusColumnWidth
        {
            get
            {
                if (_statusColumnWidth == -1)
                {
                    _statusColumnWidth = RegistryHelper.GetValue("SongListView.StatusColumnWidth", 10);
                }
                return _statusColumnWidth;
            }
            set
            {
                _statusColumnWidth = value;
                RegistryHelper.SetValue("SongListView.StatusColumnWidth", value);
            }
        }

        internal int TitleColumnWidth
        {
            get
            {
                if (_titleColumnWidth == -1)
                {
                    _titleColumnWidth = RegistryHelper.GetValue("SongListView.TitleColumnWidth", 200);
                }
                return _titleColumnWidth;
            }
            set
            {
                _titleColumnWidth = value;
                RegistryHelper.SetValue("SongListView.TitleColumnWidth", value);
            }
        }

        internal int ArtistColumnWidth
        {
            get
            {
                if (_artistColumnWidth == -1)
                {
                    _artistColumnWidth = RegistryHelper.GetValue("SongListView.ArtistColumnWidth", 150);
                }
                return _artistColumnWidth;
            }
            set
            {
                _artistColumnWidth = value;
                RegistryHelper.SetValue("SongListView.ArtistColumnWidth", value);
            }
        }

        internal int AlbumColumnWidth
        {
            get
            {
                if (_albumColumnWidth == -1)
                {
                    _albumColumnWidth = RegistryHelper.GetValue("SongListView.AlbumColumnWidth", 150);
                }
                return _albumColumnWidth;
            }
            set
            {
                _albumColumnWidth = value;
                RegistryHelper.SetValue("SongListView.AlbumColumnWidth", value);
            }
        }

        internal int DurationColumnWidth
        {
            get
            {
                if (_durationColumnWidth == -1)
                {
                    _durationColumnWidth = RegistryHelper.GetValue("SongListView.DurationColumnWidth", 20);
                }
                return _durationColumnWidth;
            }
            set
            {
                _durationColumnWidth = value;
                RegistryHelper.SetValue("SongListView.DurationColumnWidth", value);
            }
        }

        internal int PlayCountColumnWidth
        {
            get
            {
                if (_playCountColumnWidth == -1)
                {
                    _playCountColumnWidth = RegistryHelper.GetValue("SongListView.PlayCountColumnWidth", 20);
                }
                return _playCountColumnWidth;
            }
            set
            {
                _playCountColumnWidth = value;
                RegistryHelper.SetValue("SongListView.PlayCountColumnWidth", value);
            }
        }

        internal SongListViewHeader Header
        {
            get { return header; }
        }

        internal SongListViewListPanel List
        {
            get { return list; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MusicLibrary.Library Library
        {
            get { return _library; }
            set
            {
                _library = value;
                _library.PlayCountUpdated += new EventHandler<LibraryEntryEventArgs>(Library_LibraryUpdated);
                _library.LibraryUpdated += new EventHandler<LibraryEntryEventArgs>(Library_LibraryUpdated);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MusicPlayer.Player Player
        {
            get { return _player; }
            set
            {
                _player = value;
                _player.SongOpened += new EventHandler<SongEventArgs>(Player_SongOpened);
                _player.Playlist.PlaylistChanged += new EventHandler(Playlist_PlaylistChanged);
                _player.SongForced += new EventHandler(Player_SongForced);
            }
        }

        #region Exposed Properties From List

        public bool FlatMode
        {
            get { return list.FlatMode; }
            set { list.FlatMode = value; }
        }

        internal int WidestAlbum
        {
            get { return list.WidestAlbum; }
        }

        public Font HeaderFont
        {
            get { return list.HeaderFont; }
            set { list.HeaderFont = value; }
        }

        public Font CurrentSongFont
        {
            get { return list.CurrentSongFont; }
            set { list.CurrentSongFont = value; }
        }

        public ContextMenuStrip ItemContextMenuStrip
        {
            get { return list.ItemContextMenuStrip; }
            set { list.ItemContextMenuStrip = value; }
        }

        public Color SelectedColor
        {
            get { return list.SelectedColor; }
            set { list.SelectedColor = value; }
        }

        public int AlbumArtSize
        {
            get { return list.AlbumArtSize; }
            set { list.AlbumArtSize = value; }
        }

        public bool ShowAlbumArt
        {
            get { return list.ShowAlbumArt; }
            set { list.ShowAlbumArt = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SongInfo[] DataSource
        {
            get { return list.DataSource; }
            set { list.DataSource = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<SongListViewItem> Items
        {
            get { return list.Items; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<SongListViewItem> SelectedItems
        {
            get { return list.SelectedItems; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int[] SelectedIndices
        {
            get { return list.SelectedIndices; }
        }

        #endregion

        #endregion Properties

        #region Constructor

        public SongListView()
        {
            InitializeComponent();
            header.ListView = this;
            list.ListView = this;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Player.SongOpened -= new EventHandler<SongEventArgs>(Player_SongOpened);
                Player.Playlist.PlaylistChanged -= new EventHandler(Playlist_PlaylistChanged);
                Player.SongForced -= new EventHandler(Player_SongForced);
                Library.PlayCountUpdated -= new EventHandler<LibraryEntryEventArgs>(Library_LibraryUpdated);
                Library.LibraryUpdated -= new EventHandler<LibraryEntryEventArgs>(Library_LibraryUpdated);
            }
            base.Dispose(disposing);
        }

        #endregion Constructor

        #region Public Methods

        public override void Refresh()
        {
            base.Refresh();
            List.MeasureItems();
        }

        public SongListViewItem GetItemAt(int x, int y)
        {
            return list.GetItemAt(x, y);
        }

        #endregion

        #region SongListView Drag And Drop Methods and Events

        private void SongListView_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // Get the index of the item the mouse is over
                SongListViewItem item = list.GetItemAt(e.X, e.Y);

                if (item != null)
                {
                    // Remember the point where the mouse down occurred. The DragSize indicates
                    // the size that the mouse can move before a drag event should be started
                    Size dragSize = SystemInformation.DragSize;

                    // Create a rectangle using the DragSize, with the mouse position being
                    // at the center of the rectangle
                    _dragBox = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                }
                else
                {
                    // Reset the rectangle if the mouse is not over an item in the ListBox
                    _dragBox = Rectangle.Empty;
                }
            }
        }

        private void SongListView_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag
                if (_dragBox != Rectangle.Empty && !_dragBox.Contains(e.X, e.Y))
                {
                    // Proceed with the drag-and-drop, passing in the list of selected items
                    System.Collections.Specialized.StringCollection files = new System.Collections.Specialized.StringCollection();
                    foreach (SongListViewItem item in list.SelectedItems)
                    {
                        files.Add(item.SongInfo.FileName);
                    }

                    DataObject ob = new DataObject();
                    ob.SetData(SelectedItems.ToArray());
                    ob.SetFileDropList(files);

                    list.DoDragDrop(ob, DragDropEffects.Copy | DragDropEffects.Link);
                }
            }
        }

        private void SongListView_MouseUp(object sender, MouseEventArgs e)
        {
            // Reset the drag rectangle when the mouse button is raised
            _dragBox = Rectangle.Empty;
        }

        private void list_DragOver(object sender, DragEventArgs e)
        {
            if (e.Effect == DragDropEffects.None) return;
            Point p = list.PointToClient(new Point(e.X, e.Y));
            SongListViewItem item = list.GetItemAt(p.X - list.AutoScrollPosition.X, p.Y - list.AutoScrollPosition.Y);
            if (item != null)
            {
                list.m_dragIndex = item.Index;
            }
            else
            {
                list.m_dragIndex = -1;
            }
            list.Invalidate();
        }

        private void list_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            if (AllowDrop && list.FlatMode)
            {
                if (e.Data.GetDataPresent(typeof(SongListViewItem[])))
                {
                    e.Effect = DragDropEffects.Link;
                }
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Link;
                }
            }
        }

        private void list_DragLeave(object sender, EventArgs e)
        {
            list.m_dragIndex = -1;
            list.Invalidate();
        }

        private void list_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect == DragDropEffects.None) return;
            Point p = list.PointToClient(new Point(e.X, e.Y));
            SongListViewItem item = list.GetItemAt(p.X - list.AutoScrollPosition.X, p.Y - list.AutoScrollPosition.Y);

            UseWaitCursor = true;
            List<SongListViewItem> itemsToAdd = new List<SongListViewItem>();
            if (e.Data.GetDataPresent(typeof(SongListViewItem[])))
            {
                foreach (SongListViewItem old in (SongListViewItem[])e.Data.GetData(typeof(SongListViewItem[])))
                {
                    SongListViewItem myOld = FindItem(old.SongInfo.FileName);
                    if (myOld != null)
                    {
                        list.Items.Remove(myOld);
                        itemsToAdd.Add(myOld);
                    }
                    else
                    {
                        SongListViewItem newItem = new SongListViewItem(list);
                        newItem.SongInfo = old.SongInfo;
                        newItem.AlbumDisplay = old.AlbumDisplay;
                        itemsToAdd.Add(newItem);
                    }
                }
                list.ReCreateAndMeasure();
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // get the files, add them, and stuff
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (File.Exists(file))
                    {
                        if (Array.IndexOf(Player.SupportedExtensions, "*" + Path.GetExtension(file)) != -1)
                        {
                            CreateOrAddItem(itemsToAdd, file);
                        }
                    }
                    else if (Directory.Exists(file))
                    {
                        string[] files2 = Directory.GetFiles(file, "*", SearchOption.AllDirectories);
                        foreach (string file2 in files2)
                        {
                            if (Array.IndexOf(Player.SupportedExtensions, "*" + Path.GetExtension(file2)) != -1)
                            {
                                CreateOrAddItem(itemsToAdd, file2);
                            }
                        }
                    }
                }
            }

            if (item == null)
            {
                list.Items.AddRange(itemsToAdd);
            }
            else
            {
                list.Items.InsertRange(item.Index, itemsToAdd);
            }
            list.ReCreateAndMeasure();
            list.m_dragIndex = -1;
            list.Invalidate();
            if (ListChanged != null)
            {
                ListChanged(this, EventArgs.Empty);
            }
            UseWaitCursor = false;
        }

        private void CreateOrAddItem(List<SongListViewItem> itemsToAdd, string file)
        {
            SongListViewItem myOld = FindItem(file);
            if (myOld == null)
            {
                SongListViewItem item = new SongListViewItem(list);
                SongInfo song = Library.GetSong(file);
                if (song == null)
                {
                    song = new LibraryEntry(file);
                }
                item.SongInfo = song;
                item.AlbumDisplay = item.SongInfo.Album;
                itemsToAdd.Add(item);
            }
            else
            {
                list.Items.Remove(myOld);
                itemsToAdd.Add(myOld);
            }
        }

        private SongListViewItem FindItem(string filename)
        {
            foreach (SongListViewItem item in list.Items)
            {
                if (item.SongInfo.FileName.Equals(filename))
                {
                    return item;
                }
            }
            return null;
        }

        #endregion SongListView Event Handlers

        #region List Events

        private void list_ItemsMeasured(object sender, EventArgs e)
        {
            header.XOffset = -list.AutoScrollPosition.X;
            header.Invalidate();
        }

        private void List_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                header.XOffset = -e.NewValue;
            }
        }

        private void SongListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q)
            {
                this.Player.Playlist.EventsEnabled = false;
                this.SelectedItems.ForEach(delegate (SongListViewItem item)
                {
                    if (this.Player.Playlist.Contains(item.SongInfo.FileName))
                    {
                        this.Player.Playlist.Remove(item.SongInfo.FileName);
                    }
                    else
                    {
                        this.Player.Playlist.AddToEnd(item.SongInfo);
                    }
                });
                this.Player.Playlist.EventsEnabled = true;
                if (SongQueued != null)
                {
                    SongQueued(this, EventArgs.Empty);
                }
            }
            else if (e.KeyCode == Keys.F)
            {
                if (this.SelectedItems.Count > 0)
                {
                    this.Player.ForceSong(this.SelectedItems[0].SongInfo.FileName);
                    list.InvalidateItem(this.SelectedItems[0]);
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                this.SelectedItems.ForEach(delegate (SongListViewItem item)
                {
                    this.Library.Delete(item.SongInfo.FileName);
                    list.Items.Remove(item);
                });
                list.MeasureItems();
            }
            else if (e.KeyCode == Keys.I)
            {
                this.SelectedItems.ForEach(delegate (SongListViewItem item)
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
            else if (e.KeyCode == Keys.Enter)
            {
                if (this.SelectedItems.Count > 0)
                {
                    this.Player.PlayFile(this.SelectedItems[0].SongInfo.FileName);
                    SongPlayed?.Invoke(this, EventArgs.Empty);
                }
            }
            else if (Control.ModifierKeys == Keys.Control && e.KeyCode == Keys.A)
            {
                list.SelectedItems.Clear();
                list.SelectedItems.AddRange(list.Items);
                list.Invalidate();
            }
        }

        private void SongListView_DoubleClick(object sender, EventArgs e)
        {
            if (!_autoDoubleClick)
            {
                base.OnDoubleClick(e);
            }
            if (this.SelectedItems.Count > 0)
            {
                this.Player.PlayFile(this.SelectedItems[0].SongInfo.FileName);
                SongPlayed?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Player Events

        private void Playlist_PlaylistChanged(object sender, EventArgs e)
        {
            list.Invalidate();
        }

        private void Player_SongOpened(object sender, SongEventArgs e)
        {
            SongListViewItem item = FindItem(e.Song.FileName);
            if (item != null)
            {
                list.InvalidateItem(item);
            }
        }

        private void Player_SongForced(object sender, EventArgs e)
        {
            list.Invalidate();
        }

        private void Library_LibraryUpdated(object sender, LibraryEntryEventArgs e)
        {
            if (e.LibraryEntry == null)
            {
                return;
            }
            SongListViewItem item = FindItem(e.LibraryEntry.FileName);
            if (item != null)
            {
                item.SongInfo = e.LibraryEntry;
                list.InvalidateItem(item);
            }
        }

        #endregion
    }
}
