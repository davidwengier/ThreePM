using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using ThreePM.MusicLibrary;
using ThreePM.MusicPlayer;
using ThreePM.Utilities;

namespace ThreePM.UI
{
    public class SongListViewItem : IHaveAlbumArt
    {
        #region Declarations

        private int _albumArtSize;
        private SongInfo _songInfo;
        private Rectangle _rect;
        private bool _isStartOfNewAlbum;
        private Bitmap _albumArt;
        private Rectangle _albumRectangle;
        private readonly SongListViewListPanel _songListViewListPanel;
        private SongListViewItem _albumItem;
        private SongListViewItem _nextItem;
        private SongListViewItem _previousItem;
        private int _groupHeaderHeight;
        private int _index = -1;
        private string _albumDisplay;

        #endregion

        #region Properties

        public int AlbumArtSize
        {
            get { return _albumArtSize; }
            set { _albumArtSize = value; }
        }

        public string AlbumDisplay
        {
            get { return (_albumDisplay ?? _songInfo.Album); }
            set { _albumDisplay = value; }
        }

        public Rectangle AlbumRectangle
        {
            get { return _albumRectangle; }
            set { _albumRectangle = value; }
        }

        public Bitmap AlbumArt
        {
            get { return _albumArt; }
            set
            {
                _albumArt = value;
                if (_songListViewListPanel != null)
                {
                    _songListViewListPanel.InvalidateItem(this);
                }
            }
        }

        public bool IsStartOfNewAlbum
        {
            get { return _isStartOfNewAlbum; }
            set { _isStartOfNewAlbum = value; }
        }

        public Rectangle Rectangle
        {
            get { return _rect; }
            set { _rect = value; }
        }

        public SongInfo SongInfo
        {
            get { return _songInfo; }
            set { _songInfo = value; }
        }

        public SongListViewItem AlbumItem
        {
            get { return _albumItem; }
            set { _albumItem = value; }
        }

        public SongListViewItem NextItem
        {
            get { return _nextItem; }
            set { _nextItem = value; }
        }

        public SongListViewItem PreviousItem
        {
            get { return _previousItem; }
            set { _previousItem = value; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        #endregion

        #region Constructor

        public SongListViewItem()
        {
        }

        internal SongListViewItem(SongListViewListPanel listViewListPanel)
        {
            _songListViewListPanel = listViewListPanel;
        }

        #endregion

        #region Public Methods

        public void EnsureVisible()
        {
            Rectangle r = _songListViewListPanel.ClientRectangle;
            r.Offset(-_songListViewListPanel.AutoScrollPosition.X, -_songListViewListPanel.AutoScrollPosition.Y);
            if (!r.Contains(this.Rectangle.Location))
            {
                if (this.Rectangle.Bottom > -_songListViewListPanel.AutoScrollPosition.Y)
                {
                    _songListViewListPanel.AutoScrollPosition = new Point(_songListViewListPanel.AutoScrollPosition.X, (this.Rectangle.Bottom - r.Height));
                }
                else
                {
                    _songListViewListPanel.AutoScrollPosition = new Point(_songListViewListPanel.AutoScrollPosition.X, (this.Rectangle.Top));
                }
            }
        }

        #endregion

        #region Paint Methods

        internal void PaintAlbum(Graphics g, int xOffset, int yOffset, Brush foreColorBrush)
        {
            var rect = new RectangleF(_albumRectangle.X + xOffset, _albumRectangle.Y + yOffset, 0, 0);

            if (!_songListViewListPanel.FlatMode)
            {
                DrawColumn(g, ref rect, this.AlbumDisplay, -1, _songListViewListPanel.HeaderFont, foreColorBrush);
                rect.Y += rect.Height + 2;
                using (var b = new LinearGradientBrush(new RectangleF(xOffset + 2, rect.Y, 300, 5), _songListViewListPanel.ForeColor, _songListViewListPanel.BackColor, LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(b, xOffset + 2, rect.Y, 300, 2);
                }

                rect.Y += 5;
            }
            if (_albumArtSize > 0)
            {
                rect.X = xOffset + 2;
                //g.DrawImage(m_albumArt, Convert.ToInt32(rect.X), Convert.ToInt32(rect.Y), AlbumArtWidth, AlbumArtWidth);
                if (_albumArt != null)
                {
                    g.DrawImageUnscaled(_albumArt, Convert.ToInt32(rect.X), Convert.ToInt32(rect.Y));
                }

                rect.X += _albumArtSize + 2;
            }
            else
            {
                //rect.X = xOffset + 2;
            }

            if (_songListViewListPanel.FlatMode || !_songListViewListPanel.ShowAlbumArt)
            {
                //float infoX = rect.X;
                //int width = Convert.ToInt32(AlbumRectangle.Right);

                //DrawColumn(g, ref rect, album, width, m_songListViewListPanel.Font, foreColorBrush);
                //rect.X = infoX; rect.Y += rect.Height;
            }
            else
            {
                float infoX = rect.X;
                int width = Convert.ToInt32(this.AlbumRectangle.Right - infoX);

                string album = string.IsNullOrEmpty(_songInfo.AlbumArtist) ? "Unknown Artist" : _songInfo.AlbumArtist;
                DrawColumn(g, ref rect, album, width, _songListViewListPanel.Font, foreColorBrush);
                rect.X = infoX; rect.Y += rect.Height;

                album = string.IsNullOrEmpty(_songInfo.Genre) ? "Unknown Genre" : _songInfo.Genre;
                DrawColumn(g, ref rect, album, width, _songListViewListPanel.Font, foreColorBrush);
                rect.X = infoX; rect.Y += rect.Height;

                album = _songInfo.Year <= 0 ? "Unknown Year" : _songInfo.Year.ToString();
                DrawColumn(g, ref rect, album, width, _songListViewListPanel.Font, foreColorBrush);

                rect.X = infoX; rect.Y += rect.Height;

                album = System.IO.Path.GetPathRoot(_songInfo.FileName);
                DrawColumn(g, ref rect, album, width, _songListViewListPanel.Font, foreColorBrush);
            }
            //g.DrawRectangle(Pens.Blue, new Rectangle(AlbumRectangle.X + xOffset, AlbumRectangle.Y + yOffset, AlbumRectangle.Width, AlbumRectangle.Height));
        }

        internal void PaintItem(Graphics g, int xOffset, int yOffset, Brush foreColorBrush)
        {
            Font titleFont = _songListViewListPanel.Font;
            if (_songListViewListPanel.ListView.Player.CurrentSong.FileName.Equals(_songInfo.FileName))
            {
                titleFont = _songListViewListPanel.CurrentSongFont;
            }

            var rect = new RectangleF(_rect.X + xOffset, _rect.Y + yOffset, 0, 0);

            if (_songListViewListPanel.SelectedItems.Contains(this))
            {
                using (var b = new SolidBrush(_songListViewListPanel.SelectedColor))
                {
                    g.FillRectangle(b, new Rectangle(this.Rectangle.X + xOffset, this.Rectangle.Y + yOffset, _songListViewListPanel.AutoScrollMinSize.Width, this.Rectangle.Height));
                }
            }

            RectangleF tRext = rect;
            if (_songListViewListPanel.ListView.Player.Playlist.Contains(_songInfo.FileName))
            {
                DrawColumn(g, ref rect, "+", _songListViewListPanel.ListView.StatusColumnWidth, _songListViewListPanel.Font, foreColorBrush);
            }
            rect = tRext;
            if (_songListViewListPanel.ListView.Player.IsForced(_songInfo.FileName))
            {
                DrawColumn(g, ref rect, "*", _songListViewListPanel.ListView.StatusColumnWidth, _songListViewListPanel.Font, foreColorBrush);
            }
            rect = tRext;
            if (_songListViewListPanel.ListView.Player.CurrentSong.FileName.Equals(_songInfo.FileName))
            {
                DrawColumn(g, ref rect, ">", _songListViewListPanel.ListView.StatusColumnWidth, titleFont, foreColorBrush);
            }
            else
            {
                DrawColumn(g, ref rect, " ", _songListViewListPanel.ListView.StatusColumnWidth, _songListViewListPanel.Font, foreColorBrush);
            }

            if (string.IsNullOrEmpty(_songInfo.Title))
            {
                int width = _songListViewListPanel.ListView.TitleColumnWidth +
                            _songListViewListPanel.ListView.TrackNumberColumnWidth +
                            _songListViewListPanel.ListView.ArtistColumnWidth +
                            (_songListViewListPanel.FlatMode ? _songListViewListPanel.ListView.AlbumColumnWidth : 0);
                DrawColumn(g, ref rect, _songInfo.FileName, width, titleFont, foreColorBrush);

                if (_songInfo is LibraryEntry entry)
                {
                    DrawColumn(g, ref rect, entry.PlayCount.ToString(), _songListViewListPanel.ListView.PlayCountColumnWidth, _songListViewListPanel.Font, foreColorBrush);
                }
                else
                {
                    DrawColumn(g, ref rect, " ", _songListViewListPanel.ListView.PlayCountColumnWidth, _songListViewListPanel.Font, foreColorBrush);
                }
            }
            else
            {
                DrawColumn(g, ref rect, (_songInfo.TrackNumber <= 0 ? "" : _songInfo.TrackNumber.ToString()), _songListViewListPanel.ListView.TrackNumberColumnWidth, _songListViewListPanel.Font, foreColorBrush);
                DrawColumn(g, ref rect, _songInfo.Title, _songListViewListPanel.ListView.TitleColumnWidth, titleFont, foreColorBrush);
                DrawColumn(g, ref rect, _songInfo.Artist, _songListViewListPanel.ListView.ArtistColumnWidth, _songListViewListPanel.Font, foreColorBrush);

                if (_songListViewListPanel.FlatMode)
                {
                    DrawColumn(g, ref rect, _songInfo.Album, _songListViewListPanel.ListView.AlbumColumnWidth, _songListViewListPanel.Font, foreColorBrush);
                }

                DrawColumn(g, ref rect, _songInfo.DurationDescription, _songListViewListPanel.ListView.DurationColumnWidth, _songListViewListPanel.Font, foreColorBrush);

                if (_songInfo is LibraryEntry entry)
                {
                    DrawColumn(g, ref rect, entry.PlayCount.ToString(), _songListViewListPanel.ListView.PlayCountColumnWidth, _songListViewListPanel.Font, foreColorBrush);
                }
                else
                {
                    DrawColumn(g, ref rect, " ", _songListViewListPanel.ListView.PlayCountColumnWidth, _songListViewListPanel.Font, foreColorBrush);
                }
            }

            //g.DrawRectangle(Pens.Blue, new Rectangle(Rectangle.X + xOffset, Rectangle.Y + yOffset, Rectangle.Width, Rectangle.Height));
        }

        internal static void DrawColumn(Graphics g, ref RectangleF rect, string strToDisplay, int colWidth, Font f, Brush foreColorBrush)
        {
            rect.Size = MeasureColumn(g, strToDisplay, colWidth, f);
            g.DrawString(strToDisplay, f, foreColorBrush, rect);
            rect.Offset(rect.Width, 0);
        }

        #endregion

        #region Measure Methods

        internal void MeasureAlbum(Graphics g, int xOffset, int yOffset, Font albumFont, Font f)
        {
            float width;
            float height;

            if (!_songListViewListPanel.FlatMode)
            {
                string album = this.AlbumDisplay;
                string artist = string.IsNullOrEmpty(_songInfo.AlbumArtist) ? "Unknown Artist" : _songInfo.AlbumArtist;
                string genre = string.IsNullOrEmpty(_songInfo.Genre) ? "Unknown Genre" : _songInfo.Genre;
                string year = _songInfo.Year <= 0 ? "Unknown Year" : _songInfo.Year.ToString();
                SizeF artistSize = MeasureColumn(g, artist, -1, f);
                SizeF genreSize = MeasureColumn(g, genre, -1, f);
                SizeF yearSize = MeasureColumn(g, year, -1, f);
                SizeF albumSize = MeasureColumn(g, album, -1, albumFont);

                width = Math.Max(100f, artistSize.Width);
                width = Math.Max(width, genreSize.Width);
                width = Math.Max(width, yearSize.Width);


                height = artistSize.Height * 4;

                if (_albumArtSize > 0)
                {
                    height = Math.Max(height, _albumArtSize);
                    width += _albumArtSize;
                }

                width += /*gap*/2 + /*gap*/2 + /*gap*/2;
                height += /*gap*/2 + /*gradient line*/2 + /*gap*/2 + albumSize.Height;

                _groupHeaderHeight = Convert.ToInt32(albumSize.Height + 2 + 2);
            }
            else
            {
                _groupHeaderHeight = 0;

                if (_albumArtSize > 0)
                {
                    width = _albumArtSize + 2;
                    height = _albumArtSize;
                }
                else
                {
                    //width = f.Height+ 2;
                    width = 1;
                    height = f.Height;
                }
            }

            this.AlbumRectangle = Rectangle.Ceiling(new RectangleF(xOffset, yOffset, width, height));
        }

        internal void MeasureItem(Graphics g, int xOffset, int yOffset, Font f)
        {
            if (this.IsStartOfNewAlbum)
            {
                // move the offset down by the height of the group header if its the first track
                yOffset += _groupHeaderHeight;
            }
            SizeF size;

            size = MeasureColumn(g, "A", -1, f);
            // override the width because we dont care
            size.Width = _songListViewListPanel.ListView.TrackNumberColumnWidth
                + _songListViewListPanel.ListView.StatusColumnWidth
                + _songListViewListPanel.ListView.TitleColumnWidth
                + _songListViewListPanel.ListView.ArtistColumnWidth
                + _songListViewListPanel.ListView.DurationColumnWidth
                + _songListViewListPanel.ListView.PlayCountColumnWidth;

            if (_songListViewListPanel.FlatMode)
            {
                size.Width += _songListViewListPanel.ListView.AlbumColumnWidth;
            }

            this.Rectangle = Rectangle.Ceiling(new RectangleF(xOffset, yOffset, size.Width, size.Height));
        }

        internal static SizeF MeasureColumn(Graphics g, string strToDisplay, int colWidth, Font f)
        {
            SizeF size = g.MeasureString(strToDisplay, f);
            if (colWidth == -1) return size;
            return new SizeF(colWidth, size.Height);
        }

        #endregion
    }
}
