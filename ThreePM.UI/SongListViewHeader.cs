using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ThreePM.UI
{
    internal class SongListViewHeader : Control
    {
        private Color _lineColorLight = Color.White;
        private Color _lineColor = Color.Empty;
        private SongListView _songListView;
        private int _xOffset;
        private int _col;
        private readonly int[] _colWidths = new int[5];
        private int _origX;

        public Color LineColorLight
        {
            get { return _lineColorLight; }
            set { _lineColorLight = value; }
        }

        public Color LineColor
        {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        public int XOffset
        {
            get { return _xOffset; }
            set
            {
                _xOffset = value;
                Invalidate();
            }
        }

        public SongListViewHeader()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SongListView ListView
        {
            get { return _songListView; }
            internal set
            {
                _songListView = value;
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.Height = this.Font.Height + 2;
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            if (_lineColor == Color.Empty)
            {
                _lineColor = Color.FromArgb(170, this.ForeColor);
            }
            base.OnForeColorChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_songListView == null) return;
            using (var foreColorBrush = new SolidBrush(this.ForeColor))
            {
                using (Pen linePen = new Pen(_lineColor), linePenLight = new Pen(_lineColorLight))
                {
                    var rect = new RectangleF(this.XOffset, 0, _songListView.TitleColumnWidth, this.Height);
                    if (!_songListView.FlatMode)
                    {
                        SongListViewItem.DrawColumn(e.Graphics, ref rect, "Album", _songListView.WidestAlbum, this.Font, foreColorBrush);
                    }
                    else
                    {
                        rect.X += _songListView.WidestAlbum;
                    }
                    rect.X += _songListView.StatusColumnWidth;
                    SongListViewItem.DrawColumn(e.Graphics, ref rect, "#", _songListView.TrackNumberColumnWidth, this.Font, foreColorBrush);
                    e.Graphics.DrawLine(linePen, rect.Left - 1, 0, rect.Left - 1, this.Height - 3);
                    e.Graphics.DrawLine(linePenLight, rect.Left, 0, rect.Left, this.Height - 3);

                    _colWidths[0] = Convert.ToInt32(rect.Left);
                    SongListViewItem.DrawColumn(e.Graphics, ref rect, "Title", _songListView.TitleColumnWidth, this.Font, foreColorBrush);
                    e.Graphics.DrawLine(linePen, rect.Left - 1, 0, rect.Left - 1, this.Height - 3);
                    e.Graphics.DrawLine(linePenLight, rect.Left, 0, rect.Left, this.Height - 3);

                    _colWidths[1] = Convert.ToInt32(rect.Left);
                    SongListViewItem.DrawColumn(e.Graphics, ref rect, "Artist", _songListView.ArtistColumnWidth, this.Font, foreColorBrush);
                    e.Graphics.DrawLine(linePen, rect.Left - 1, 0, rect.Left - 1, this.Height - 3);
                    e.Graphics.DrawLine(linePenLight, rect.Left, 0, rect.Left, this.Height - 3);

                    _colWidths[2] = Convert.ToInt32(rect.Left);
                    if (_songListView.FlatMode)
                    {
                        SongListViewItem.DrawColumn(e.Graphics, ref rect, "Album", _songListView.AlbumColumnWidth, this.Font, foreColorBrush);
                        e.Graphics.DrawLine(linePen, rect.Left - 1, 0, rect.Left - 1, this.Height - 3);
                        e.Graphics.DrawLine(linePenLight, rect.Left, 0, rect.Left, this.Height - 3);

                        _colWidths[3] = Convert.ToInt32(rect.Left);
                    }

                    SongListViewItem.DrawColumn(e.Graphics, ref rect, "Duration", _songListView.DurationColumnWidth, this.Font, foreColorBrush);
                    e.Graphics.DrawLine(linePen, rect.Left - 1, 0, rect.Left - 1, this.Height - 3);
                    e.Graphics.DrawLine(linePenLight, rect.Left, 0, rect.Left, this.Height - 3);
                    _colWidths[(_songListView.FlatMode ? 4 : 3)] = Convert.ToInt32(rect.Left);

                    SongListViewItem.DrawColumn(e.Graphics, ref rect, "Play Count", -1, this.Font, foreColorBrush);
                    e.Graphics.DrawLine(linePen, 0, this.Height - 2, this.Width, this.Height - 2);

                    // Check whether the header text is larger than the auto-size stuff in the columns
                    MaybeSetColumnAutoWidth(0, "#", e.Graphics);
                    MaybeSetColumnAutoWidth(1, "Title", e.Graphics);
                    MaybeSetColumnAutoWidth(2, "Artist", e.Graphics);
                    MaybeSetColumnAutoWidth(3, "Album", e.Graphics);
                    MaybeSetColumnAutoWidth(4, "Duration", e.Graphics);
                    MaybeSetColumnAutoWidth(5, "Play Count", e.Graphics);
                }
            }
        }

        private void MaybeSetColumnAutoWidth(int index, string text, Graphics g)
        {
            int spacer = 1;
            int width = (int)g.MeasureString(text, this.Font).Width + spacer;
            if (_songListView.List.ColAutoWidths[index] < width)
            {
                _songListView.List.ColAutoWidths[index] = width;
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (this.Cursor == Cursors.VSplit)
            {
                // Auto-size the column
                switch (_col)
                {
                    case 0:
                    {
                        _songListView.TrackNumberColumnWidth = _songListView.List.ColAutoWidths[0];
                        break;
                    }
                    case 1:
                    {
                        _songListView.TitleColumnWidth = _songListView.List.ColAutoWidths[1];
                        break;
                    }
                    case 2:
                    {
                        _songListView.ArtistColumnWidth = _songListView.List.ColAutoWidths[2];
                        break;
                    }
                    case 3:
                    {
                        if (_songListView.FlatMode)
                        {
                            _songListView.AlbumColumnWidth = _songListView.List.ColAutoWidths[3];
                        }
                        else
                        {
                            _songListView.DurationColumnWidth = _songListView.List.ColAutoWidths[4];
                        }
                        break;
                    }
                    case 4:
                    {
                        _songListView.DurationColumnWidth = _songListView.List.ColAutoWidths[4];
                        break;
                    }
                }
                _songListView.List.MeasureItems();
            }

            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.Cursor == Cursors.VSplit && e.Button == MouseButtons.Left)
            {
                _songListView.List.DrawDragLine = true;
                _songListView.List.DragLineLeft = e.X;
                _songListView.List.Invalidate();
            }
            else
            {
                this.Cursor = Cursors.Default;
                for (int i = 0; i < _colWidths.Length; i++)
                {
                    if (!_songListView.FlatMode && i == 4) break;
                    if (Math.Abs(e.X - _colWidths[i]) < 3)
                    {
                        this.Cursor = Cursors.VSplit;
                        _col = i;
                        _origX = e.X;
                        break;
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.Cursor == Cursors.VSplit)
            {
                _songListView.List.DrawDragLine = false;
                switch (_col)
                {
                    case 0:
                    {
                        _songListView.TrackNumberColumnWidth += e.X - _origX;
                        break;
                    }
                    case 1:
                    {
                        _songListView.TitleColumnWidth += e.X - _origX;
                        break;
                    }
                    case 2:
                    {
                        _songListView.ArtistColumnWidth += e.X - _origX;
                        break;
                    }
                    case 3:
                    {
                        if (_songListView.FlatMode)
                        {
                            _songListView.AlbumColumnWidth += e.X - _origX;
                        }
                        else
                        {
                            _songListView.DurationColumnWidth += e.X - _origX;
                        }
                        break;
                    }
                    case 4:
                    {
                        _songListView.DurationColumnWidth += e.X - _origX;
                        break;
                    }
                }
                _songListView.List.MeasureItems();
            }
            base.OnMouseUp(e);
        }
    }
}
