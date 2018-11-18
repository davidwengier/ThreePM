using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace ThreePM.UI
{
	internal class SongListViewHeader : Control
	{
		private Color m_lineColorLight = Color.White;
		private Color m_lineColor = Color.Empty;
		private SongListView m_songListView;
		private int m_xOffset;
		private int m_col;
		private int[] m_colWidths = new int[5];
		private int m_origX;

		public Color LineColorLight
		{
			get { return m_lineColorLight; }
			set { m_lineColorLight = value; }
		}

		public Color LineColor
		{
			get { return m_lineColor; }
			set { m_lineColor = value; }
		}

		public int XOffset
		{
			get { return m_xOffset; }
			set
			{
				m_xOffset = value;
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
			get { return m_songListView; }
			internal set
			{
				m_songListView = value;
			}
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			Height = Font.Height+2;
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			if (m_lineColor == Color.Empty)
			{
				m_lineColor = Color.FromArgb(170, ForeColor);
			}
			base.OnForeColorChanged(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (m_songListView == null) return;
			using (SolidBrush foreColorBrush = new SolidBrush(ForeColor))
			{
				using (Pen linePen = new Pen(m_lineColor), linePenLight = new Pen(m_lineColorLight))
				{
					RectangleF rect = new RectangleF(XOffset, 0, m_songListView.TitleColumnWidth, Height);
					if (!m_songListView.FlatMode)
					{
						SongListViewItem.DrawColumn(e.Graphics, ref rect, "Album", m_songListView.WidestAlbum, Font, foreColorBrush);
					}
					else
					{
						rect.X += m_songListView.WidestAlbum;
					}
					rect.X += m_songListView.StatusColumnWidth;
					SongListViewItem.DrawColumn(e.Graphics, ref rect, "#", m_songListView.TrackNumberColumnWidth, Font, foreColorBrush);
					e.Graphics.DrawLine(linePen, rect.Left-1, 0, rect.Left-1, Height - 3);
					e.Graphics.DrawLine(linePenLight, rect.Left, 0, rect.Left, Height - 3);
					
					m_colWidths[0] = Convert.ToInt32(rect.Left);
					SongListViewItem.DrawColumn(e.Graphics, ref rect, "Title", m_songListView.TitleColumnWidth, Font, foreColorBrush);
					e.Graphics.DrawLine(linePen, rect.Left - 1, 0, rect.Left - 1, Height - 3);
					e.Graphics.DrawLine(linePenLight, rect.Left, 0, rect.Left, Height - 3);

					m_colWidths[1] = Convert.ToInt32(rect.Left);
					SongListViewItem.DrawColumn(e.Graphics, ref rect, "Artist", m_songListView.ArtistColumnWidth, Font, foreColorBrush);
					e.Graphics.DrawLine(linePen, rect.Left - 1, 0, rect.Left - 1, Height - 3);
					e.Graphics.DrawLine(linePenLight, rect.Left, 0, rect.Left, Height - 3);

					m_colWidths[2] = Convert.ToInt32(rect.Left);
					if (m_songListView.FlatMode)
					{
						SongListViewItem.DrawColumn(e.Graphics, ref rect, "Album", m_songListView.AlbumColumnWidth, Font, foreColorBrush);
						e.Graphics.DrawLine(linePen, rect.Left - 1, 0, rect.Left - 1, Height - 3);
						e.Graphics.DrawLine(linePenLight, rect.Left, 0, rect.Left, Height - 3);

						m_colWidths[3] = Convert.ToInt32(rect.Left);
					}

					SongListViewItem.DrawColumn(e.Graphics, ref rect, "Duration", m_songListView.DurationColumnWidth, Font, foreColorBrush);
					e.Graphics.DrawLine(linePen, rect.Left - 1, 0, rect.Left - 1, Height - 3);
					e.Graphics.DrawLine(linePenLight, rect.Left, 0, rect.Left, Height - 3);
					m_colWidths[(m_songListView.FlatMode ? 4 : 3)] = Convert.ToInt32(rect.Left);

					SongListViewItem.DrawColumn(e.Graphics, ref rect, "Play Count", -1, Font, foreColorBrush);
					e.Graphics.DrawLine(linePen, 0, Height - 2, Width, Height - 2);

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
			if (m_songListView.List.m_colAutoWidths[index] < width)
			{
				m_songListView.List.m_colAutoWidths[index] = width;
			}
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			if (this.Cursor == Cursors.VSplit)
			{
				// Auto-size the column
				switch (m_col)
				{
					case 0:
					{
						m_songListView.TrackNumberColumnWidth = m_songListView.List.m_colAutoWidths[0];
						break;
					}
					case 1:
					{
						m_songListView.TitleColumnWidth = m_songListView.List.m_colAutoWidths[1];
						break;
					}
					case 2:
					{
						m_songListView.ArtistColumnWidth = m_songListView.List.m_colAutoWidths[2];
						break;
					}
					case 3:
					{
						if (m_songListView.FlatMode)
						{
							m_songListView.AlbumColumnWidth = m_songListView.List.m_colAutoWidths[3];
						}
						else
						{
							m_songListView.DurationColumnWidth = m_songListView.List.m_colAutoWidths[4];
						}
						break;
					}
					case 4:
					{
						m_songListView.DurationColumnWidth = m_songListView.List.m_colAutoWidths[4];
						break;
					}
				}
				m_songListView.List.MeasureItems();
			}

			base.OnMouseDoubleClick(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (Cursor == Cursors.VSplit && e.Button == MouseButtons.Left)
			{
				m_songListView.List.m_drawDragLine = true;
				m_songListView.List.m_dragLineLeft = e.X;
				m_songListView.List.Invalidate();
			}
			else
			{
				Cursor = Cursors.Default;
				for (int i = 0; i < m_colWidths.Length; i++)
				{
					if (!m_songListView.FlatMode && i == 4) break;
					if (Math.Abs(e.X - m_colWidths[i]) < 3)
					{
						Cursor = Cursors.VSplit;
						m_col = i;
						m_origX = e.X;
						break;
					}
				}
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (Cursor == Cursors.VSplit)
			{
				m_songListView.List.m_drawDragLine = false;
				switch (m_col)
				{
					case 0:
					{
						m_songListView.TrackNumberColumnWidth += e.X - m_origX;
						break;
					}
					case 1:
					{
						m_songListView.TitleColumnWidth += e.X - m_origX;
						break;
					}
					case 2:
					{
						m_songListView.ArtistColumnWidth += e.X - m_origX;
						break;
					}
					case 3:
					{
						if (m_songListView.FlatMode)
						{
							m_songListView.AlbumColumnWidth += e.X - m_origX;
						}
						else
						{
							m_songListView.DurationColumnWidth += e.X - m_origX;
						}
						break;
					}
					case 4:
					{
						m_songListView.DurationColumnWidth += e.X - m_origX;
						break;
					}
				}
				m_songListView.List.MeasureItems();
			}
			base.OnMouseUp(e);
		}
	}
}
