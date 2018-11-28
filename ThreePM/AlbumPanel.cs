using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ThreePM.MusicPlayer;
using ThreePM.UI;
using ThreePM.Utilities;

namespace ThreePM
{
    public class AlbumPanel : Control
    {
        #region Enums

        public enum PaintMode
        {
            Normal,
            ALittleFancy,
            UltraFancy
        }

        private enum ScrollDirection
        {
            None,
            Left,
            Right
        }

        #endregion Enums

        #region Constants

        private const int SpaceBetweenAlbums = 3;
        private const int CenterItemIndex = 4;
        private const int TransitionTime = 300;
        private const int TransitionTimerInterval = 10;
        private const int SmallestAlbumSize = 40;

        // Widths as % of total width
        private readonly int[] _albumPercentWidths = new int[] { 8, 10, 12, 14, 16, 14, 12, 10, 8 };

        #endregion Constants

        #region Declarations

        private AlbumArtLoader _loader = new AlbumArtLoader();
        private SongInfo[] _dataSource;
        private int _currentIndex = 0;
        private List<SongListViewItem> _items;
        private Ticker _ticker;
        private ContextMenuStrip _albumContextMenuStrip;

        private readonly PaintMode _paintMode = PaintMode.ALittleFancy;

        private bool _stopScrollingDammit = false;
        private ScrollDirection _scrollDirection = ScrollDirection.None;
        private Timer _timer = new Timer();

        private Timer _updateTimer = new Timer();
        private int _originalOffset = 0;
        private int _offset = 0;

        private bool _viewingDetails = false;

        private MusicLibrary.Library _library;

        #endregion

        #region Properties

        public ContextMenuStrip AlbumContextMenuStrip
        {
            get { return _albumContextMenuStrip; }
            set { _albumContextMenuStrip = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SongInfo[] DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
                CreateItems();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SongInfo SelectedItem
        {
            get
            {
                if (_items == null || _items.Count == 0)
                {
                    return null;
                }
                return _items[Convert.ToInt32(_ticker.Position)].SongInfo;
            }
            set
            {
                if (value == null) return;
                FindAlbum(value.Album);
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
            }
        }

        #endregion

        #region Constructor

        public AlbumPanel()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.StandardClick | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            _timer.Tick += new EventHandler(Timer_Tick);

            _updateTimer.Interval = TransitionTimerInterval;
            _updateTimer.Tick += new EventHandler(UpdateTimer_Tick);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeItems();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void CreateItems()
        {
            _ticker = new Ticker();
            _ticker.Width = 100;
            _ticker.Dock = DockStyle.Bottom;
            _ticker.ForeColor = Color.Red;
            _ticker.FireWhileSliding = true;
            _ticker.Duration = _dataSource.Length - 1;
            _ticker.PositionChanged += new EventHandler(Ticker_PositionChanged);
            this.Controls.Add(_ticker);
            Invalidate(true);

            MethodInvoker doWork = delegate
            {
                DisposeItems();
                _items = new List<SongListViewItem>();

                if (_dataSource == null || _dataSource.Length == 0) return;

                string lastAlbum = string.Empty;
                SongListViewItem prevItem = null;
                int index = 0;
                foreach (SongInfo info in _dataSource)
                {
                    //if (index > 50) break;
                    string album = info.Album;
                    if (string.IsNullOrEmpty(album)) album = "Unknown Album";

                    if (lastAlbum != album)
                    {
                        var item = new SongListViewItem();
                        item.SongInfo = info;
                        item.AlbumItem = item;
                        item.PreviousItem = prevItem;
                        if (prevItem != null) prevItem.NextItem = item;
                        prevItem = item;
                        item.IsStartOfNewAlbum = true;
                        item.Index = index;

                        if (info.HasFrontCover)
                        {
                            item.AlbumArt = info.GetFrontCover(100, 100);
                        }
                        else
                        {
                            _loader.LoadAlbumArt(item, info.FileName, 100, 100);
                        }
                        //item.AlbumArt = AlbumArtHelper.GetAlbumArt(info.FileName, 100, 100);

                        lastAlbum = album;
                        _items.Add(item);
                        index += 1;
                    }
                }

                _ticker.Duration = _items.Count - 1;
                _ticker.SetPosition(_ticker.Position);
                this.Invoke((MethodInvoker)delegate { Invalidate(true); });
            };

            doWork.BeginInvoke(null, null);
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

        private void Ticker_PositionChanged(object sender, EventArgs e)
        {
            _currentIndex = Convert.ToInt32(_ticker.Position);
            Invalidate();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            switch (_scrollDirection)
            {
                case ScrollDirection.None:
                {
                    break;
                }
                case ScrollDirection.Left:
                {
                    _ticker.SetPosition(_ticker.Position - 1);
                    break;
                }
                case ScrollDirection.Right:
                {
                    _ticker.SetPosition(_ticker.Position + 1);
                    break;
                }
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }

        #endregion

        #region Painting Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_items == null || _items.Count == 0) return;

            if (_currentIndex < 0 || _currentIndex > _items.Count - 1)
            {
                return;
            }

            switch (_paintMode)
            {
                case PaintMode.Normal:
                {
                    PaintNormal(e);
                    break;
                }
                case PaintMode.ALittleFancy:
                {
                    PaintALittleFancy(e);
                    //PaintCurves(e);
                    break;
                }
                case PaintMode.UltraFancy:
                {
                    // Pshyeah
                    break;
                }
            }
        }

        private void PaintNormal(PaintEventArgs e)
        {
            using (var foreColorBrush = new SolidBrush(this.ForeColor))
            {
                float x = -(4 * this.Width / 100) / 2;
                for (int i = _currentIndex - 4; i <= _currentIndex + 4; i++)
                {
                    int index = i - (_currentIndex - 4);
                    int width = _albumPercentWidths[index] * this.Width / 100;
                    if (i >= 0 && i < _items.Count)
                    {
                        if (_items[i].AlbumArt != null)
                        {
                            if (_viewingDetails && i == _currentIndex)
                            {
                                // Ignore the album - it'll get drawn last
                            }
                            else
                            {
                                // Draw the album normally
                                int itemHeight = width + (_albumPercentWidths[index] * 2);
                                e.Graphics.DrawImage(_items[i].AlbumArt, x, (this.Height - itemHeight) / 2, width, width);
                                _items[i].AlbumRectangle = Rectangle.Ceiling(new RectangleF(x, (this.Height - itemHeight) / 2, width, width));
                                using (var f = new Font(this.Font.FontFamily, _albumPercentWidths[index], FontStyle.Regular, GraphicsUnit.Pixel))
                                {
                                    var rect = new RectangleF(x, ((this.Height - itemHeight) / 2) + width, width, f.Height);
                                    e.Graphics.DrawString(_items[i].SongInfo.Album, f, foreColorBrush, rect);
                                    rect.Offset(0, _albumPercentWidths[index]);
                                    e.Graphics.DrawString(_items[i].SongInfo.AlbumArtist, f, foreColorBrush, rect);
                                }
                            }
                        }
                    }
                    x += width;
                }

                if (_viewingDetails)
                {
                    PaintCurrentAlbumHuuuge(e);
                }
            }
        }

        private void PaintALittleFancy(PaintEventArgs e)
        {
            // Determine the offset change in pixels
            int offsetChange = Convert.ToInt32(_originalOffset / (AlbumPanel.TransitionTime / _updateTimer.Interval));
            if (offsetChange < 1)
            {
                // We can't change by less than one pixel!
                offsetChange = 1;
            }

            // Determine the offset at which to paint albums
            int paintOffset = 0;
            switch (_scrollDirection)
            {
                case ScrollDirection.None:
                {
                    paintOffset = 0;
                    _offset = 0;
                    break;
                }
                case ScrollDirection.Left:
                {
                    paintOffset = _offset * -1;
                    _offset -= offsetChange;
                    break;
                }
                case ScrollDirection.Right:
                {
                    paintOffset = _offset;
                    _offset -= offsetChange;
                    break;
                }
            }

            using (var foreColorBrush = new SolidBrush(this.ForeColor))
            {
                int centerPosition = (this.ClientSize.Width / 2) + paintOffset;
                int centerIndex = 0;
                int[] albumSizes = AlbumSizes(paintOffset, out centerIndex);

                int x = 0;

                // Start with the center album as it will dictate the positions of the rest of the albums
                int size = albumSizes[centerIndex];
                x = centerPosition - (size / 2);
                PaintAlbum(e, _items[_currentIndex], foreColorBrush, x, TopCurveY(centerPosition), size);

                // Start painting albums to the right of the center
                x = centerPosition + (albumSizes[centerIndex] / 2) + AlbumPanel.SpaceBetweenAlbums;
                for (int i = 1; i < albumSizes.Length - centerIndex; i++)
                {
                    if (_currentIndex + i >= _items.Count)
                    {
                        break;
                    }

                    size = albumSizes[centerIndex + i];
                    int center = x + (size / 2);
                    PaintAlbum(e, _items[_currentIndex + i], foreColorBrush, x, TopCurveY(center), size);
                    x += (size + AlbumPanel.SpaceBetweenAlbums);
                }

                // Start painting albums to the left of the center
                x = centerPosition - (albumSizes[centerIndex] / 2) - AlbumPanel.SpaceBetweenAlbums;
                for (int i = 1; centerIndex - i >= 0; i++)
                {
                    if (_currentIndex == 0)
                    {
                        break;
                    }

                    size = albumSizes[centerIndex - i];
                    x -= (size + AlbumPanel.SpaceBetweenAlbums);
                    int center = x + (size / 2);
                    if (_currentIndex - i > 0)
                        PaintAlbum(e, _items[_currentIndex - i], foreColorBrush, x, TopCurveY(center), size);
                }

                if (_viewingDetails)
                {
                    PaintCurrentAlbumHuuuge(e);
                }
            }

            if (_offset <= 0)
            {
                // Finished scrolling
                _scrollDirection = ScrollDirection.None;
                _updateTimer.Stop();

                // The offset may be less than 0, so re-align everything nicely
                _offset = 0;
                Invalidate();

                // Force a mouse move in case we are at the edge of the screen
                Point mouseLocation = PointToClient(Control.MousePosition);
                MouseMoveALittleFancy(new MouseEventArgs(MouseButtons.None, 0, mouseLocation.X, mouseLocation.Y, 0));
            }
        }

        private void PaintAlbum(PaintEventArgs e, SongListViewItem item, SolidBrush textBrush, int x, int y, int size)
        {
            e.Graphics.DrawImage(item.AlbumArt, x, y, size, size);
            item.AlbumRectangle = Rectangle.Ceiling(new RectangleF(x, y, size, size));

            var rect = new RectangleF(x, y + size, size, this.Font.Height);
            e.Graphics.DrawString(item.SongInfo.Album, this.Font, textBrush, rect);
            rect.Offset(0, this.Font.Height);
            e.Graphics.DrawString(item.SongInfo.AlbumArtist, this.Font, textBrush, rect);
        }

        private void PaintCurves(PaintEventArgs e)
        {
            // Paint the parabolas that the albums will follow
            var topPoints = new List<Point>();
            var bottomPoints = new List<Point>();

            for (int x = 0; x < this.ClientSize.Width; x += 2)
            {
                topPoints.Add(new Point(x, TopCurveY(x)));
                bottomPoints.Add(new Point(x, BottomCurveY(x)));
            }

            var topPointArray = new Point[topPoints.Count];
            topPoints.CopyTo(topPointArray);

            var bottomPointArray = new Point[bottomPoints.Count];
            bottomPoints.CopyTo(bottomPointArray);

            using (var pen = new Pen(this.ForeColor))
            {
                e.Graphics.DrawCurve(pen, topPointArray);
                e.Graphics.DrawCurve(pen, bottomPointArray);
            }
        }

        private void PaintCurrentAlbumHuuuge(PaintEventArgs e)
        {
            // Draw the current album fricking huge
            int itemHeight = this.Height - _ticker.Height - 6;
            var rect = new Rectangle(Convert.ToInt32((this.Width / 2) - (itemHeight / 2)), 3, itemHeight, itemHeight);

            // First draw a black background
            using (var black = new SolidBrush(Color.Black))
            {
                e.Graphics.FillRectangle(black, rect);
            }

            // Now draw a semi-transparent album cover
            float opacity = 0.30f;
            float[][] points = {
                        new float[] {1, 0, 0, 0, 0},
                        new float[] {0, 1, 0, 0, 0},
                        new float[] {0, 0, 1, 0, 0},
                        new float[] {0, 0, 0, opacity, 0},	// <---	That's the opacity element right there
						new float[] {0, 0, 0, 0, 1}};       //		F'd if I know what the rest is

            var matrix = new ColorMatrix(points);
            var atts = new ImageAttributes();
            atts.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            Bitmap albumCover = _items[_currentIndex].AlbumArt; //.Clone(rect, PixelFormat.DontCare);
            e.Graphics.DrawImage(albumCover, rect, 0, 0, albumCover.Width, albumCover.Height, GraphicsUnit.Pixel, atts);
            _items[_currentIndex].AlbumRectangle = Rectangle.Ceiling(rect);

            // And now draw the album info
            ThreePM.MusicLibrary.LibraryEntry[] songs = this.Library.QueryLibrary("Album LIKE '" + _items[_currentIndex].SongInfo.Album.Replace("'", "''") + "'");
            using (var f = new Font(this.Font.FontFamily, 16, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                using (var foreColorBrush = new SolidBrush(this.ForeColor))
                {
                    e.Graphics.DrawString(_items[_currentIndex].SongInfo.Album, f, foreColorBrush, rect.X + 3, rect.Y + 3);
                    e.Graphics.DrawString(_items[_currentIndex].SongInfo.AlbumArtist, f, foreColorBrush, rect.X + 3, rect.Y + 23);

                    int top = 63;
                    foreach (ThreePM.MusicLibrary.LibraryEntry song in songs)
                    {
                        string songInfo = string.Format("{0}\t{1} ({2})", song.TrackNumber, song.Title, song.Duration);
                        e.Graphics.DrawString(songInfo, f, foreColorBrush, rect.X + 3, rect.Y + top);
                        top += 20;
                    }
                }
            }
        }

        #endregion Painting Methods

        #region Overridden Methods

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();

            // Hide the context menu if it's being shown
            if (_albumContextMenuStrip != null && _albumContextMenuStrip.Visible)
            {
                _albumContextMenuStrip.Hide();
            }

            // Get the index of the album that was clicked
            int clickedIndex = AlbumIndexAtPoint(e.Location);
            if (clickedIndex != -1)
            {
                if (clickedIndex >= _currentIndex - 2 && clickedIndex <= _currentIndex + 2)
                {
                    if (clickedIndex == _currentIndex)
                    {
                        if (e.Button == MouseButtons.Right)
                        {
                            // Show the context menu for the central album
                            if (_albumContextMenuStrip != null)
                            {
                                _albumContextMenuStrip.Show(this, e.Location);
                            }
                        }
                        else
                        {
                            // Show the album details for the central album
                            _viewingDetails = !_viewingDetails;
                        }
                    }
                    else
                    {
                        // Make the clicked album the central one and show its details
                        _currentIndex = clickedIndex;
                        _ticker.Position = clickedIndex;
                        _viewingDetails = true;
                    }
                }
                else
                {
                    if (_scrollDirection == ScrollDirection.None)
                    {
                        if (!_viewingDetails)
                        {
                            _currentIndex = clickedIndex;
                            _ticker.Position = clickedIndex;
                        }
                    }
                    else
                    {
                        _scrollDirection = ScrollDirection.None;
                        _timer.Stop();
                        _stopScrollingDammit = true;
                    }
                }

                // Redraw with the new changes
                Invalidate();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            //if (m_viewingDetails)
            //{
            //    return;
            //}
            _ticker.SetPosition(_ticker.Position - (e.Delta * SystemInformation.MouseWheelScrollLines / 120));
            base.OnMouseWheel(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_stopScrollingDammit) // || m_viewingDetails)
            {
                _stopScrollingDammit = false;
                return;
            }

            switch (_paintMode)
            {
                case PaintMode.Normal:
                {
                    MouseMoveNormal(e);
                    break;
                }
                case PaintMode.ALittleFancy:
                {
                    MouseMoveALittleFancy(e);
                    break;
                }
                case PaintMode.UltraFancy:
                {
                    break;
                }
            }

            base.OnMouseMove(e);
        }

        private void MouseMoveNormal(MouseEventArgs e)
        {
            int hoverIndex = AlbumIndexAtPoint(e.Location);
            if (hoverIndex != -1)
            {
                if (hoverIndex >= _currentIndex - 2 && hoverIndex <= _currentIndex + 2)
                {
                    _scrollDirection = ScrollDirection.None;
                    _timer.Stop();
                }
                else if (hoverIndex < _currentIndex)
                {
                    _scrollDirection = ScrollDirection.Left;
                    _timer.Start();
                }
                else if (hoverIndex > _currentIndex)
                {
                    _scrollDirection = ScrollDirection.Right;
                    _timer.Start();
                }
            }
            else
            {
                _scrollDirection = ScrollDirection.None;
                _timer.Stop();
            }
        }

        private void MouseMoveALittleFancy(MouseEventArgs e)
        {
            if (_scrollDirection != ScrollDirection.None)
            {
                // Chill, we're already scrolling
                //m_scrollDirection = ScrollDirection.None;
                return;
            }

            int hoverIndex = AlbumIndexAtPoint(e.Location);
            if (hoverIndex != -1)
            {
                if (hoverIndex >= _currentIndex - 2 && hoverIndex <= _currentIndex + 2)
                {
                    _scrollDirection = ScrollDirection.None;
                }
                else if (hoverIndex < _currentIndex)
                {
                    _scrollDirection = ScrollDirection.Left;
                    TransitionToAlbum();
                }
                else if (hoverIndex > _currentIndex)
                {
                    _scrollDirection = ScrollDirection.Right;
                    TransitionToAlbum();
                }
            }
            else
            {
                _scrollDirection = ScrollDirection.None;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _scrollDirection = ScrollDirection.None;
            _timer.Stop();

            base.OnMouseLeave(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Right || keyData == Keys.Down)
            {
                //m_ticker.SetPosition(m_ticker.Position + 1);
                _scrollDirection = ScrollDirection.Right;
                TransitionToAlbum();
                return true;
            }
            else if (keyData == Keys.Left || keyData == Keys.Up)
            {
                //m_ticker.SetPosition(m_ticker.Position - 1);
                _scrollDirection = ScrollDirection.Left;
                TransitionToAlbum();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Methods

        private void FindAlbum(string album)
        {
            if (_items == null || _items.Count == 0) return;
            foreach (SongListViewItem item in _items)
            {
                if (item.SongInfo.Album.Equals(album, StringComparison.InvariantCultureIgnoreCase))
                {
                    _ticker.SetPosition(item.Index);
                    break;
                }
            }
        }

        private SongListViewItem AlbumAtPoint(Point location)
        {
            SongListViewItem result = null;

            int index = AlbumIndexAtPoint(location);
            if (index != -1)
            {
                result = _items[index];
            }

            return result;
        }

        private int AlbumIndexAtPoint(Point location)
        {
            int result = -1;

            for (int i = _currentIndex - 4; i <= _currentIndex + 4; i++)
            {
                if (i >= 0)
                {
                    if (i < _items.Count)
                    {
                        if (_items[i].AlbumRectangle.Contains(location))
                        {
                            result = i;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private int TopCurveY(int x)
        {
            x = Math.Abs(x - this.ClientSize.Width / 2);

            // y = ax^2 + k
            double a = 0.0002;
            double k = (this.ClientSize.Height / 5);
            return Convert.ToInt32((a * (x * x)) + k);
        }

        private int BottomCurveY(int x)
        {
            x = Math.Abs(x - this.ClientSize.Width / 2);

            // y = ax^2 + k
            double a = -0.0001;
            double k = (this.ClientSize.Height / 5) * 4;
            return Convert.ToInt32((a * (x * x)) + k);
        }

        private void TransitionToAlbum()
        {
            // For the time being, scroll to the next album
            int direction = 0;
            switch (_scrollDirection)
            {
                case ScrollDirection.None:
                {
                    // Shouldn't really be here...
                    return;
                }
                case ScrollDirection.Left:
                {
                    _currentIndex -= 1;
                    direction = -1;
                    _ticker.Position = _currentIndex;
                    break;
                }
                case ScrollDirection.Right:
                {
                    _currentIndex += 1;
                    direction = 1;
                    _ticker.Position = _currentIndex;
                    break;
                }
            }

            // Figure out how much the offset is to start with. It should be half the width of the current
            // album + half the width of the next album + the space between albums
            int centerIndex = 0;
            int[] albumSizes = AlbumSizes(0, out centerIndex);
            _originalOffset = (albumSizes[centerIndex] / 2) + (albumSizes[centerIndex + direction] / 2);
            _offset = _originalOffset;

            // Start updating
            _updateTimer.Start();
        }

        private int[] AlbumSizes(int offset, out int centerIndex)
        {
            centerIndex = 0;

            var albumSizes = new List<int>();

            // Start with the center album as it will dictate the sizes of the rest of the albums
            int centerPosition = (this.ClientSize.Width / 2) + offset;
            int x = centerPosition;
            albumSizes.Add(BottomCurveY(x) - TopCurveY(x));

            // For albums around the center, we start at the position the album would be if it were the
            // same size as the last one and move in to the center until it fits the curves. I hope that
            // makes sense

            // Start getting the sizes of albums to the right of the center
            x = centerPosition + (albumSizes[0] / 2) + AlbumPanel.SpaceBetweenAlbums;
            bool stillHaveAlbumsToGoSoKeepGoing = true;
            while (stillHaveAlbumsToGoSoKeepGoing)
            {
                // Get the size
                int size = 0;
                for (size = albumSizes[albumSizes.Count - 1]; size > 0; size--)
                {
                    int newCenterX = x + (size / 2);
                    int newCenterSize = BottomCurveY(newCenterX) - TopCurveY(newCenterX);
                    if (size <= newCenterSize)
                    {
                        //size = newCenterSize;
                        break;
                    }
                }

                // Decide whether to store it
                if (size > AlbumPanel.SmallestAlbumSize)
                {
                    albumSizes.Add(size);

                    // Keep going if the album's bigger than the smallest allowed size
                    stillHaveAlbumsToGoSoKeepGoing = true;
                    x += (size + AlbumPanel.SpaceBetweenAlbums);
                }
                else
                {
                    stillHaveAlbumsToGoSoKeepGoing = false;
                }
            }

            // Now start getting the sizes of albums to the left of the center
            x = centerPosition - (albumSizes[0] / 2);
            stillHaveAlbumsToGoSoKeepGoing = true;
            while (stillHaveAlbumsToGoSoKeepGoing)
            {
                int size = 0;
                for (size = albumSizes[0]; size > 0; size--)
                {
                    int newCenterX = x - Convert.ToInt32(size / 2);
                    int newCenterSize = BottomCurveY(newCenterX) - TopCurveY(newCenterX);
                    if (size <= newCenterSize)
                    {
                        size = newCenterSize;
                        break;
                    }
                }

                if (size > AlbumPanel.SmallestAlbumSize)
                {
                    albumSizes.Insert(0, size);
                    centerIndex += 1;

                    // Keep going if the album's bigger than the smallest allowed size
                    stillHaveAlbumsToGoSoKeepGoing = true;
                    x -= (size + AlbumPanel.SpaceBetweenAlbums);
                }
                else
                {
                    stillHaveAlbumsToGoSoKeepGoing = false;
                }
            }

            int[] albumSizeArray = new int[albumSizes.Count];
            albumSizes.CopyTo(albumSizeArray);

            return albumSizeArray;
        }

        #endregion
    }
}
