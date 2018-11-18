using System;
using System.Collections.Generic;
using System.Text;
using ThreePM.MusicPlayer;
using System.Drawing;
using System.Drawing.Drawing2D;
using ThreePM.MusicLibrary;
using ThreePM.Utilities;

namespace ThreePM.UI
{
	public class SongListViewItem : IHaveAlbumArt
	{
		#region Declarations

		private int m_albumArtSize;
		private SongInfo m_songInfo;
		private Rectangle m_rect;
		private bool m_isStartOfNewAlbum;
		private Bitmap m_albumArt;
		private Rectangle m_albumRectangle;
		private SongListViewListPanel m_songListViewListPanel;
		private SongListViewItem m_albumItem;
		private SongListViewItem m_nextItem;
		private SongListViewItem m_previousItem;
		private int m_groupHeaderHeight;
		private int m_index = -1;
		private string m_albumDisplay;

		#endregion

		#region Properties

		public int AlbumArtSize
		{
			get { return m_albumArtSize; }
			set { m_albumArtSize = value; }
		}

		public string AlbumDisplay
		{
			get { return (m_albumDisplay ?? m_songInfo.Album); }
			set { m_albumDisplay = value; }
		}

		public Rectangle AlbumRectangle
		{
			get { return m_albumRectangle; }
			set { m_albumRectangle = value; }
		}
		
		public Bitmap AlbumArt
		{
			get { return m_albumArt; }
			set
			{
				m_albumArt = value;
				if (m_songListViewListPanel != null)
				{
					m_songListViewListPanel.InvalidateItem(this);
				}
			}
		}

		public bool IsStartOfNewAlbum
		{
			get { return m_isStartOfNewAlbum; }
			set { m_isStartOfNewAlbum = value; }
		}

		public Rectangle Rectangle
		{
			get { return m_rect; }
			set { m_rect = value; }
		}
		
		public SongInfo SongInfo
		{
			get { return m_songInfo; }
			set { m_songInfo = value; }
		}

		public SongListViewItem AlbumItem
		{
			get { return m_albumItem; }
			set { m_albumItem = value; }
		}

		public SongListViewItem NextItem
		{
			get { return m_nextItem; }
			set { m_nextItem = value; }
		}

		public SongListViewItem PreviousItem
		{
			get { return m_previousItem; }
			set { m_previousItem = value; }
		}

		public int Index
		{
			get { return m_index; }
			set { m_index = value; }
		}

		#endregion

		#region Constructor

		public SongListViewItem()
		{
		}

		internal SongListViewItem(SongListViewListPanel listViewListPanel)
		{
			m_songListViewListPanel = listViewListPanel;
		}

		#endregion

		#region Public Methods

		public void EnsureVisible()
		{
			Rectangle r = m_songListViewListPanel.ClientRectangle;
			r.Offset(-m_songListViewListPanel.AutoScrollPosition.X, -m_songListViewListPanel.AutoScrollPosition.Y);
			if (!r.Contains(Rectangle.Location))
			{
				if (Rectangle.Bottom > -m_songListViewListPanel.AutoScrollPosition.Y)
				{
					m_songListViewListPanel.AutoScrollPosition = new Point(m_songListViewListPanel.AutoScrollPosition.X, (Rectangle.Bottom - r.Height));
				}
				else
				{
					m_songListViewListPanel.AutoScrollPosition = new Point(m_songListViewListPanel.AutoScrollPosition.X, (Rectangle.Top));
				}
			}
		}

		#endregion

		#region Paint Methods

		internal void PaintAlbum(Graphics g, int xOffset, int yOffset, Brush foreColorBrush)
		{
			RectangleF rect = new RectangleF(m_albumRectangle.X + xOffset, m_albumRectangle.Y + yOffset, 0, 0);

			if (!m_songListViewListPanel.FlatMode)
			{
				DrawColumn(g, ref rect, AlbumDisplay, -1, m_songListViewListPanel.HeaderFont, foreColorBrush);
				rect.Y += rect.Height + 2;
				using (LinearGradientBrush b = new LinearGradientBrush(new RectangleF(xOffset + 2, rect.Y, 300, 5), m_songListViewListPanel.ForeColor, m_songListViewListPanel.BackColor, LinearGradientMode.Horizontal))
				{
					g.FillRectangle(b, xOffset + 2, rect.Y, 300, 2);
				}

				rect.Y += 5;
			}
			if (m_albumArtSize > 0)
			{
				rect.X = xOffset + 2;
				//g.DrawImage(m_albumArt, Convert.ToInt32(rect.X), Convert.ToInt32(rect.Y), AlbumArtWidth, AlbumArtWidth);
				if (m_albumArt != null)
				{
					g.DrawImageUnscaled(m_albumArt, Convert.ToInt32(rect.X), Convert.ToInt32(rect.Y));
				}

				rect.X += m_albumArtSize + 2;
			}
			else
			{
				//rect.X = xOffset + 2;
			}

			if (m_songListViewListPanel.FlatMode || !m_songListViewListPanel.ShowAlbumArt)
			{
				//float infoX = rect.X;
				//int width = Convert.ToInt32(AlbumRectangle.Right);

				//DrawColumn(g, ref rect, album, width, m_songListViewListPanel.Font, foreColorBrush);
				//rect.X = infoX; rect.Y += rect.Height;
			}
			else
			{
				float infoX = rect.X;
				int width = Convert.ToInt32(AlbumRectangle.Right - infoX);

				string album = String.IsNullOrEmpty(m_songInfo.AlbumArtist) ? "Unknown Artist" : m_songInfo.AlbumArtist;
				DrawColumn(g, ref rect, album, width, m_songListViewListPanel.Font, foreColorBrush);
				rect.X = infoX; rect.Y += rect.Height;

				album = String.IsNullOrEmpty(m_songInfo.Genre) ? "Unknown Genre" : m_songInfo.Genre;
				DrawColumn(g, ref rect, album, width, m_songListViewListPanel.Font, foreColorBrush);
				rect.X = infoX; rect.Y += rect.Height;

				album = m_songInfo.Year <= 0 ? "Unknown Year" : m_songInfo.Year.ToString();
				DrawColumn(g, ref rect, album, width, m_songListViewListPanel.Font, foreColorBrush);

				rect.X = infoX; rect.Y += rect.Height;

				album = System.IO.Path.GetPathRoot(m_songInfo.FileName);
				DrawColumn(g, ref rect, album, width, m_songListViewListPanel.Font, foreColorBrush);
			}
			//g.DrawRectangle(Pens.Blue, new Rectangle(AlbumRectangle.X + xOffset, AlbumRectangle.Y + yOffset, AlbumRectangle.Width, AlbumRectangle.Height));
		}

		internal void PaintItem(Graphics g, int xOffset, int yOffset, Brush foreColorBrush)
		{
			Font titleFont = m_songListViewListPanel.Font;
			if (m_songListViewListPanel.ListView.Player.CurrentSong.FileName.Equals(m_songInfo.FileName))
			{
				titleFont = m_songListViewListPanel.CurrentSongFont;
			}

			RectangleF rect = new RectangleF(m_rect.X + xOffset, m_rect.Y + yOffset, 0, 0);

			if (m_songListViewListPanel.SelectedItems.Contains(this))
			{
				using (SolidBrush b = new SolidBrush(m_songListViewListPanel.SelectedColor))
				{
					g.FillRectangle(b, new Rectangle(Rectangle.X + xOffset, Rectangle.Y + yOffset, m_songListViewListPanel.AutoScrollMinSize.Width, Rectangle.Height));
				}
			}

			RectangleF tRext = rect;
			if (m_songListViewListPanel.ListView.Player.Playlist.Contains(m_songInfo.FileName))
			{
				DrawColumn(g, ref rect, "+", m_songListViewListPanel.ListView.StatusColumnWidth, m_songListViewListPanel.Font, foreColorBrush);
			}
			rect = tRext;
			if (m_songListViewListPanel.ListView.Player.IsForced(m_songInfo.FileName))
			{
				DrawColumn(g, ref rect, "*", m_songListViewListPanel.ListView.StatusColumnWidth, m_songListViewListPanel.Font, foreColorBrush);
			}
			rect = tRext;
			if (m_songListViewListPanel.ListView.Player.CurrentSong.FileName.Equals(m_songInfo.FileName))
			{
				DrawColumn(g, ref rect, ">", m_songListViewListPanel.ListView.StatusColumnWidth, titleFont, foreColorBrush);
			}
			else
			{
				DrawColumn(g, ref rect, " ", m_songListViewListPanel.ListView.StatusColumnWidth, m_songListViewListPanel.Font, foreColorBrush);
			}

			if (String.IsNullOrEmpty(m_songInfo.Title))
			{
				int width = m_songListViewListPanel.ListView.TitleColumnWidth +
							m_songListViewListPanel.ListView.TrackNumberColumnWidth +
							m_songListViewListPanel.ListView.ArtistColumnWidth +
							(m_songListViewListPanel.FlatMode ? m_songListViewListPanel.ListView.AlbumColumnWidth : 0);
				DrawColumn(g, ref rect, m_songInfo.FileName, width, titleFont, foreColorBrush);

				LibraryEntry entry = m_songInfo as LibraryEntry;
				if (entry != null)
				{
					DrawColumn(g, ref rect, entry.PlayCount.ToString(), m_songListViewListPanel.ListView.PlayCountColumnWidth, m_songListViewListPanel.Font, foreColorBrush);
				}
				else
				{
					DrawColumn(g, ref rect, " ", m_songListViewListPanel.ListView.PlayCountColumnWidth, m_songListViewListPanel.Font, foreColorBrush);
				}
			}
			else
			{
				DrawColumn(g, ref rect, (m_songInfo.TrackNumber <= 0 ? "" : m_songInfo.TrackNumber.ToString()), m_songListViewListPanel.ListView.TrackNumberColumnWidth, m_songListViewListPanel.Font, foreColorBrush);
				DrawColumn(g, ref rect, m_songInfo.Title, m_songListViewListPanel.ListView.TitleColumnWidth, titleFont, foreColorBrush);
				DrawColumn(g, ref rect, m_songInfo.Artist, m_songListViewListPanel.ListView.ArtistColumnWidth, m_songListViewListPanel.Font, foreColorBrush);

				if (m_songListViewListPanel.FlatMode)
				{
					DrawColumn(g, ref rect, m_songInfo.Album, m_songListViewListPanel.ListView.AlbumColumnWidth, m_songListViewListPanel.Font, foreColorBrush);
				}

				DrawColumn(g, ref rect, m_songInfo.DurationDescription, m_songListViewListPanel.ListView.DurationColumnWidth, m_songListViewListPanel.Font, foreColorBrush);

				LibraryEntry entry = m_songInfo as LibraryEntry;
				if (entry != null)
				{
					DrawColumn(g, ref rect, entry.PlayCount.ToString(), m_songListViewListPanel.ListView.PlayCountColumnWidth, m_songListViewListPanel.Font, foreColorBrush);
				}
				else
				{
					DrawColumn(g, ref rect, " ", m_songListViewListPanel.ListView.PlayCountColumnWidth, m_songListViewListPanel.Font, foreColorBrush);
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

			if (!m_songListViewListPanel.FlatMode)
			{
				string album = AlbumDisplay;
				string artist = String.IsNullOrEmpty(m_songInfo.AlbumArtist) ? "Unknown Artist" : m_songInfo.AlbumArtist;
				string genre = String.IsNullOrEmpty(m_songInfo.Genre) ? "Unknown Genre" : m_songInfo.Genre;
				string year = m_songInfo.Year <= 0 ? "Unknown Year" : m_songInfo.Year.ToString();
				SizeF artistSize = MeasureColumn(g, artist, -1, f);
				SizeF genreSize = MeasureColumn(g, genre, -1, f);
				SizeF yearSize = MeasureColumn(g, year, -1, f);
				SizeF albumSize = MeasureColumn(g, album, -1, albumFont);

				width = Math.Max(100f, artistSize.Width);
				width = Math.Max(width, genreSize.Width);
				width = Math.Max(width, yearSize.Width);


				height = artistSize.Height * 4;

				if (m_albumArtSize > 0)
				{
					height = Math.Max(height, m_albumArtSize);
					width += m_albumArtSize;
				}

				width += /*gap*/2 + /*gap*/2 + /*gap*/2;
				height += /*gap*/2 + /*gradient line*/2 + /*gap*/2 + albumSize.Height;

				m_groupHeaderHeight = Convert.ToInt32(albumSize.Height + 2 + 2);
			}
			else
			{
				m_groupHeaderHeight = 0;

				if (m_albumArtSize > 0)
				{
					width = m_albumArtSize + 2;
					height = m_albumArtSize;
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
				yOffset += m_groupHeaderHeight;
			}
			SizeF size;

			size = MeasureColumn(g, "A", -1, f);
			// override the width because we dont care
			size.Width = m_songListViewListPanel.ListView.TrackNumberColumnWidth
				+ m_songListViewListPanel.ListView.StatusColumnWidth
				+ m_songListViewListPanel.ListView.TitleColumnWidth
				+ m_songListViewListPanel.ListView.ArtistColumnWidth
				+ m_songListViewListPanel.ListView.DurationColumnWidth
				+ m_songListViewListPanel.ListView.PlayCountColumnWidth;

			if (m_songListViewListPanel.FlatMode)
			{
				size.Width += m_songListViewListPanel.ListView.AlbumColumnWidth;
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
