using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ThreePM.MusicLibrary;
using ThreePM.MusicPlayer;
using ThreePM.Utilities;

namespace ThreePM.UI
{
    internal class SongListViewListPanel : ScrollableControl
    {
        #region Declarations

        public event EventHandler ItemsMeasured;

        private bool _mouseUpSelect;
        private bool _creating;
        private SongInfo[] _dataSource;
        private int _widestalbum;
        private int _lastClickedIndex = -1;
        private List<SongListViewItem> _selectedItems = new List<SongListViewItem>();
        private ContextMenuStrip _itemContextMenuStrip;
        private bool _showAlbumArt = true;
        private int _albumArtSize = 100;
        private Font _headerFont;
        private Font _currentSongFont;
        private Color _selectedColor = Color.DarkGray;
        private List<SongListViewItem> _items;
        private bool _flatMode;
        private SongListView _songListView;

        private AlbumArtLoader _albumArtLoader = new AlbumArtLoader();

        internal int DragLineLeft;
        internal bool DrawDragLine;

        internal int DragIndex = -1;

        internal int[] ColAutoWidths = new int[6];    // Stores the widths of columns for auto-sizing

        private const string NoResultsString = "No tracks to display.";

        #endregion

        #region Properties

        public bool FlatMode
        {
            get { return _flatMode; }
            set
            {
                _flatMode = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SongListView ListView
        {
            get { return _songListView; }
            internal set { _songListView = value; }
        }

        internal int WidestAlbum
        {
            get { return _widestalbum; }
        }

        public ContextMenuStrip ItemContextMenuStrip
        {
            get { return _itemContextMenuStrip; }
            set { _itemContextMenuStrip = value; }
        }

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set { _selectedColor = value; }
        }

        public int AlbumArtSize
        {
            get { return _albumArtSize; }
            set
            {
                _albumArtSize = value;
                CreateItems();
            }
        }

        public bool ShowAlbumArt
        {
            get { return _showAlbumArt; }
            set
            {
                _showAlbumArt = value;
                CreateItems();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SongInfo[] DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                this.AutoScrollPosition = new Point(0, 0);
                CreateItems();
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            if (_headerFont == null)
                _headerFont = new Font(this.Font, FontStyle.Bold);
            if (_currentSongFont == null)
                _currentSongFont = new Font(this.Font, FontStyle.Bold);
            // We have to call MeasureItems so it re-calcs the font height
            MeasureItems();
        }

        public Font HeaderFont
        {
            get { return _headerFont; }
            set
            {
                if (_headerFont != null) _headerFont.Dispose();
                _headerFont = value;
                // We have to call MeasureItems so it re-calcs the font height
                MeasureItems();
            }
        }

        public Font CurrentSongFont
        {
            get { return _currentSongFont; }
            set
            {
                if (_currentSongFont != null) _currentSongFont.Dispose();
                _currentSongFont = value;
                // We have to call MeasureItems so it re-calcs the font height
                MeasureItems();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<SongListViewItem> Items
        {
            get { return _items; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<SongListViewItem> SelectedItems
        {
            get { return _selectedItems; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int[] SelectedIndices
        {
            get
            {
                int[] result = new int[this.SelectedItems.Count];
                for (int i = 0; i < this.SelectedItems.Count; i++)
                {
                    result[i] = this.SelectedItems[i].Index;
                }
                Array.Sort<int>(result);
                return result;
            }
        }

        #endregion

        #region Constructor

        public SongListViewListPanel()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.StandardClick | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }



        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_headerFont != null) _headerFont.Dispose();
                if (_currentSongFont != null) _currentSongFont.Dispose();
                DisposeItems();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves the item at the specified location.
        /// </summary>
        /// <param name="x">The x-coordinate of the location to search for an item (expressed in client coordinates).</param>
        /// <param name="y">The y-coordinate of the location to search for an item (expressed in client coordinates).</param>
        /// <returns>
        /// A SongListViewItem that represents the item at the specified position. If there is no item 
        /// at the specified location, the method returns null.
        /// </returns>
        public SongListViewItem GetItemAt(int x, int y)
        {
            SongListViewItem result = null;

            for (int i = 0; i < _items.Count; i++)
            {
                SongListViewItem item = _items[i];
                if (item.Rectangle.Top <= y && item.Rectangle.Bottom >= y && item.Rectangle.Left <= x)
                {
                    result = item;
                    break;
                }
            }

            return result;
        }

        #endregion Methods

        #region Input Handling Methods

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();

            int x = e.X - this.AutoScrollPosition.X;
            int y = e.Y - this.AutoScrollPosition.Y;

            SongListViewItem item = GetItemAt(x, y);
            if (item != null)
            {
                // If the user moused down on a selected item, wait until mouse up to change the selection
                // so that dragging works smoothly.  This seems to be what the ListView does
                if (_selectedItems.Contains(item))
                {
                    _mouseUpSelect = true;
                }
                else
                {
                    HandleMouseSelection(e, item);
                }

                if (e.Button == MouseButtons.Right)
                {
                    if (_itemContextMenuStrip != null)
                    {
                        _itemContextMenuStrip.Show(this, e.Location);
                    }
                }
            }
            else
            {
                _selectedItems.Clear();
                _lastClickedIndex = -1;
                Invalidate();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_mouseUpSelect)
            {
                int x = e.X - this.AutoScrollPosition.X;
                int y = e.Y - this.AutoScrollPosition.Y;

                SongListViewItem item = GetItemAt(x, y);
                if (item != null)
                {
                    HandleMouseSelection(e, item);
                    _mouseUpSelect = false;
                }
            }

            base.OnMouseUp(e);
        }

        private void HandleMouseSelection(MouseEventArgs e, SongListViewItem item)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                HandleControlMouseSelection(item);
            }
            else if (Control.ModifierKeys == Keys.Shift && _lastClickedIndex != -1)
            {
                HandleShiftMouseSelection(item);
            }
            else
            {
                bool rightButtonClicked = (e.Button == MouseButtons.Right);
                HandleNormalMouseSelection(rightButtonClicked, item);
            }

            this.Invalidate();
        }

        private void HandleControlMouseSelection(SongListViewItem item)
        {
            // If the user's holding down ctrl, add or remove this item but ignore the others

            if (_selectedItems.Contains(item))
            {
                _selectedItems.Remove(item);
            }
            else
            {
                _selectedItems.Add(item);
            }
            _lastClickedIndex = item.Index;
        }

        private void HandleShiftMouseSelection(SongListViewItem clickedItem)
        {
            // If the user's holding down shift, add everything between this item and the last clicked item

            _selectedItems.Clear();

            // Get the index of the selected item
            int selectedIndex = clickedItem.Index;

            _items.ForEach(delegate (SongListViewItem item)
            {
                if ((item.Index >= _lastClickedIndex && item.Index <= selectedIndex) ||
                    (item.Index <= _lastClickedIndex && item.Index >= selectedIndex))
                {
                    _selectedItems.Add(item);
                }
            });
        }

        private void HandleNormalMouseSelection(bool rightButtonClicked, SongListViewItem item)
        {
            // If the user's not holding down a key, just select this item and deselect every other one
            // I lied a little just there.  If the user is right-clicking on an already selected item,
            // we want to leave the selection as is

            if (rightButtonClicked && _selectedItems.Contains(item))
            {
                // Pretty much don't do anything...
            }
            else
            {
                _selectedItems.Clear();
                _selectedItems.Add(item);
                _lastClickedIndex = item.Index;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.SelectedItems.Count > 0)
            {
                SongListViewItem oldItem = null;
                SongListViewItem newItem = null;

                switch (keyData)
                {
                    case Keys.Up:
                    {
                        oldItem = this.SelectedItems[0];
                        newItem = oldItem.PreviousItem;
                        break;
                    }
                    case Keys.Down:
                    {
                        oldItem = this.SelectedItems[0];
                        newItem = oldItem.NextItem;
                        break;
                    }
                }

                if (oldItem != null && newItem != null)
                {
                    newItem.EnsureVisible();

                    this.SelectedItems.Clear();
                    this.SelectedItems.Add(newItem);
                    _lastClickedIndex = newItem.Index;
                    Invalidate();

                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion Input Handling Methods

        #region Private Methods

        internal void CreateItems()
        {
            CreateItemsSynchronously();
        }

        private void CreateItemsSynchronously()
        {
            lock (this)
            {
                _albumArtLoader.Clear();
                _creating = true;
                DisposeItems();
                _items = new List<SongListViewItem>();
                ColAutoWidths = new int[6];

                if (_dataSource != null && _dataSource.Length > 0)
                {
                    string lastAlbum = string.Empty;
                    string lastDir = string.Empty;
                    SongListViewItem albumItem = null;

                    SongListViewItem prevItem = null;

                    int flatAlbumWidth = this.Font.Height;
                    int index = 0;
                    foreach (SongInfo info in _dataSource)
                    {
                        var item = new SongListViewItem(this);
                        item.SongInfo = info;

                        item.PreviousItem = prevItem;
                        if (prevItem != null) prevItem.NextItem = item;
                        prevItem = item;

                        if (string.IsNullOrEmpty(item.AlbumDisplay)) item.AlbumDisplay = "Unknown Album";
                        if (_flatMode || (lastAlbum != item.SongInfo.Album || lastDir != System.IO.Path.GetDirectoryName(info.FileName)))
                        {

                            // if its not flat mode, and the album name is the same, tack on the path
                            if (!_flatMode && lastAlbum == item.SongInfo.Album)
                            {
                                item.AlbumDisplay = item.AlbumDisplay + " (" + System.IO.Path.GetPathRoot(info.FileName) + ")";
                                if (item.PreviousItem != null && !item.PreviousItem.AlbumItem.AlbumDisplay.EndsWith(" (" + System.IO.Path.GetPathRoot(item.PreviousItem.SongInfo.FileName) + ")"))
                                {
                                    item.PreviousItem.AlbumItem.AlbumDisplay = item.PreviousItem.AlbumItem.AlbumDisplay + " (" + System.IO.Path.GetPathRoot(item.PreviousItem.SongInfo.FileName) + ")";
                                }
                            }

                            albumItem = item;
                            item.IsStartOfNewAlbum = true;

                            if (_showAlbumArt && _flatMode)
                            {
                                item.AlbumArtSize = flatAlbumWidth;
                                // This is never true, because our SongInfo objects come from the database.. should be store album art?
                                if (info.HasFrontCover)
                                {
                                    item.AlbumArt = info.GetFrontCover(flatAlbumWidth, flatAlbumWidth);
                                }
                                else
                                {
                                    _albumArtLoader.LoadAlbumArt(item, info.FileName, flatAlbumWidth, flatAlbumWidth);
                                }
                            }
                            else
                            {
                                if (_showAlbumArt && _albumArtSize > 0)
                                {
                                    item.AlbumArtSize = _albumArtSize;
                                    if (info.HasFrontCover)
                                    {
                                        item.AlbumArt = info.GetFrontCover(_albumArtSize, _albumArtSize);
                                    }
                                    else
                                    {
                                        _albumArtLoader.LoadAlbumArt(item, info.FileName, _albumArtSize, _albumArtSize);
                                    }
                                }
                            }

                            lastAlbum = item.SongInfo.Album;
                            lastDir = System.IO.Path.GetDirectoryName(info.FileName);
                        }

                        item.AlbumItem = albumItem;

                        item.Index = index;

                        _items.Add(item);

                        index += 1;

                        // Set the auto-sizing info for the columns
                        int spacer = 1;
                        Graphics g;
                        try
                        {
                            g = this.CreateGraphics();
                        }
                        catch (ThreadStateException)
                        {
                            return;
                        }
                        Font titleFont = this.Font;
                        if (this.ListView.Player.CurrentSong != null && this.ListView.Player.CurrentSong.FileName.Equals(info.FileName))
                        {
                            titleFont = this.CurrentSongFont;
                        }
                        Font regularFont = this.Font;

                        if (string.IsNullOrEmpty(info.Title))
                        {
                            MaybeSetColumnAutoWidth(0, (int)g.MeasureString(info.FileName, titleFont).Width + spacer);

                            // Skip the next couple of columns because they are taken up by the file name
                            if (info is LibraryEntry entry)
                            {
                                MaybeSetColumnAutoWidth(5, (int)g.MeasureString(entry.PlayCount.ToString(), regularFont).Width + spacer);
                            }
                        }
                        else
                        {
                            if (info.TrackNumber > 0)
                            {
                                MaybeSetColumnAutoWidth(0, (int)g.MeasureString(info.TrackNumber.ToString(), regularFont).Width + spacer);
                            }
                            MaybeSetColumnAutoWidth(1, (int)g.MeasureString(info.Title, titleFont).Width + spacer);
                            MaybeSetColumnAutoWidth(2, (int)g.MeasureString(info.Artist, regularFont).Width + spacer);

                            if (this.FlatMode)
                            {
                                MaybeSetColumnAutoWidth(3, (int)g.MeasureString(info.Album, regularFont).Width + spacer);
                            }

                            MaybeSetColumnAutoWidth(4, (int)g.MeasureString(info.DurationDescription, regularFont).Width + spacer);

                            if (info is LibraryEntry entry)
                            {
                                MaybeSetColumnAutoWidth(5, (int)g.MeasureString(entry.PlayCount.ToString(), regularFont).Width + spacer);
                            }
                        }
                    }
                }
            }

            MeasureItems();

            this.SelectedItems.Clear();
            _lastClickedIndex = -1;

            _creating = false;
        }

        private void MaybeSetColumnAutoWidth(int index, int width)
        {
            if (ColAutoWidths[index] < width)
            {
                ColAutoWidths[index] = width;
            }
        }

        internal void ReCreateAndMeasure()
        {
            _creating = true;
            SongListViewItem prevItem = null;

            int i = 0;
            foreach (SongListViewItem item in _items)
            {
                if (prevItem != null)
                {
                    prevItem.NextItem = item;
                }
                item.PreviousItem = prevItem;
                item.Index = i;
                prevItem = item;
                i++;
            }
            MeasureItems();
            _creating = false;
        }

        internal void InvalidateItem(SongListViewItem item)
        {
            var r = new Rectangle(item.Rectangle.Location, item.Rectangle.Size);
            r.Offset(this.AutoScrollPosition);
            this.Invalidate(r);
            if (item.IsStartOfNewAlbum)
            {
                r = new Rectangle(item.AlbumRectangle.Location, item.AlbumRectangle.Size);
                r.Offset(this.AutoScrollPosition);
                this.Invalidate(r);
            }
        }

        private void DisposeItems()
        {
            if (_items != null && _items.Count > 0)
            {
                _items.ForEach(delegate (SongListViewItem item)
                {
                    if (item.AlbumArt != null)
                    {
                        item.AlbumArt.Dispose();
                        item.AlbumArt = null;
                    }
                });
            }
        }

        internal void MeasureItems()
        {
            int right = 0;
            int bottom = 0;

            if (_items == null || _items.Count == 0)
            {
                // create a temp bitmap to work with
                var btm = new Bitmap(10, 10, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var g = Graphics.FromImage(btm);
                SizeF s = SongListViewItem.MeasureColumn(g, NoResultsString, -1, this.Font);
                right = Convert.ToInt32(s.Width);
                bottom = Convert.ToInt32(s.Height);
            }
            else
            {

                // create a temp bitmap to work with
                var btm = new Bitmap(10, 10, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var g = Graphics.FromImage(btm);

                _widestalbum = 0;

                // just measure the albums first, so we know how wide the widest one is
                if (_showAlbumArt)
                {
                    _items.ForEach(delegate (SongListViewItem item)
                    {
                        if (item.IsStartOfNewAlbum)
                        {
                            item.MeasureAlbum(g, 0, 0, _headerFont, this.Font);
                            _widestalbum = Math.Max(_widestalbum, item.AlbumRectangle.Right + 1);
                        }
                    });
                }
                else
                {
                    _widestalbum = 0;
                }

                // now unfortunately we have to remeasure everything, since the albums have moved
                int y = 0;
                int lastAlbumBottom = 0;
                _items.ForEach(delegate (SongListViewItem item)
                {
                    if (item.IsStartOfNewAlbum)
                    {
                        if (y <= lastAlbumBottom) y = lastAlbumBottom;
                        // move this album down a bit from the previous one
                        if (!_flatMode)
                        {
                            y += 10;
                        }
                        item.MeasureAlbum(g, 0, y, _headerFont, this.Font);
                        right = Math.Max(right, item.AlbumRectangle.Right);
                        bottom = Math.Max(bottom, item.AlbumRectangle.Bottom);
                        lastAlbumBottom = item.AlbumRectangle.Bottom;
                    }
                    item.MeasureItem(g, _widestalbum, y, this.Font);

                    y = item.Rectangle.Bottom;

                    right = Math.Max(right, item.Rectangle.Right);
                    bottom = Math.Max(bottom, item.Rectangle.Bottom);
                });
            }
            MethodInvoker DoWork = delegate { this.AutoScrollMinSize = new Size(right + 10, bottom + 10); };
            if (this.InvokeRequired)
            {
                Invoke(DoWork);
            }
            else
            {
                DoWork();
            }

            ItemsMeasured?.Invoke(this, EventArgs.Empty);

            Invalidate();
        }

        #endregion

        #region Overridden Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_items == null) return;

            try
            {
                Graphics g = e.Graphics;

                int x = this.AutoScrollPosition.X;
                int y = this.AutoScrollPosition.Y;

                var clip = new Rectangle(e.ClipRectangle.X - this.AutoScrollPosition.X, e.ClipRectangle.Y - this.AutoScrollPosition.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);
                using (SolidBrush foreColorBrush = new SolidBrush(this.ForeColor), ignoredBrush = new SolidBrush(Color.FromArgb(100, this.ForeColor)))
                {
                    if ((_items == null || _items.Count == 0) && !_creating)
                    {
                        g.DrawString(NoResultsString, this.Font, foreColorBrush, 0, 0);
                    }
                    else
                    {
                        _items.ForEach(delegate (SongListViewItem item)
                        {
                            if (item.IsStartOfNewAlbum)
                            {
                                if (clip.IntersectsWith(item.AlbumRectangle))
                                {
                                    item.PaintAlbum(e.Graphics, x, y, foreColorBrush);
                                }
                            }
                            if (clip.IntersectsWith(item.Rectangle) || _selectedItems.Contains(item))
                            {
                                item.PaintItem(e.Graphics, x, y, item.SongInfo.Ignored ? ignoredBrush : foreColorBrush);
                            }
                            if (item.Index == DragIndex)
                            {
                                using (var linePen = new Pen(Color.FromArgb(170, this.ForeColor)))
                                {
                                    g.DrawLine(linePen, 0, item.Rectangle.Y + y, this.Width, item.Rectangle.Y + y);
                                }
                            }
                        });
                    }

                    if (DrawDragLine)
                    {
                        using (var linePen = new Pen(Color.FromArgb(170, this.ForeColor)))
                        {
                            g.DrawLine(linePen, DragLineLeft, 0, DragLineLeft, this.Height);
                        }
                    }
                }
            }
            catch { }
        }

        #endregion
    }
}
