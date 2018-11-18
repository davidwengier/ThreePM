using System;
using ThreePM.MusicPlayer;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
		private readonly int[] m_albumPercentWidths = new int[] { 8, 10, 12, 14, 16, 14, 12, 10, 8 };

		#endregion Constants

		#region Declarations

		private AlbumArtLoader m_loader = new AlbumArtLoader();
		private SongInfo[] m_dataSource;
		private int m_currentIndex = 0;
		private List<SongListViewItem> m_items;
		private Ticker m_ticker;
		private ContextMenuStrip m_albumContextMenuStrip;

		private PaintMode m_paintMode = PaintMode.ALittleFancy;

		private bool m_stopScrollingDammit = false;
		private ScrollDirection m_scrollDirection = ScrollDirection.None;
		private Timer m_timer = new Timer();

		private Timer m_updateTimer = new Timer();
		private int m_originalOffset = 0;
		private int m_offset = 0;

		private bool m_viewingDetails = false;

		private MusicLibrary.Library m_library;

		#endregion

		#region Properties

		public ContextMenuStrip AlbumContextMenuStrip
		{
			get { return m_albumContextMenuStrip; }
			set { m_albumContextMenuStrip = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SongInfo[] DataSource
		{
			get { return m_dataSource; }
			set
			{
				m_dataSource = value;
				CreateItems();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SongInfo SelectedItem
		{
			get
			{
				if (m_items == null || m_items.Count == 0)
				{
					return null;
				}
				return m_items[Convert.ToInt32(m_ticker.Position)].SongInfo;
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
			get { return m_library; }
			set
			{
				m_library = value;
			}
		}

		#endregion

		#region Constructor

		public AlbumPanel()
		{
			this.DoubleBuffered = true;
			this.ResizeRedraw = true;
			this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.StandardClick | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

			m_timer.Tick += new EventHandler(Timer_Tick);

			m_updateTimer.Interval = TransitionTimerInterval;
			m_updateTimer.Tick += new EventHandler(UpdateTimer_Tick);
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
			m_ticker = new Ticker();
			m_ticker.Width = 100;
			m_ticker.Dock = DockStyle.Bottom;
			m_ticker.ForeColor = Color.Red;
			m_ticker.FireWhileSliding = true;
			m_ticker.Duration = m_dataSource.Length - 1;
			m_ticker.PositionChanged += new EventHandler(Ticker_PositionChanged);
			this.Controls.Add(m_ticker);
			Invalidate(true);

			MethodInvoker doWork = delegate
			{
				DisposeItems();
				m_items = new List<SongListViewItem>();

				if (m_dataSource == null || m_dataSource.Length == 0) return;

				string lastAlbum = string.Empty;
				SongListViewItem prevItem = null;
				int index = 0;
				foreach (SongInfo info in m_dataSource)
				{
					//if (index > 50) break;
					string album = info.Album;
					if (String.IsNullOrEmpty(album)) album = "Unknown Album";

					if (lastAlbum != album)
					{
						SongListViewItem item = new SongListViewItem();
						item.SongInfo = info;
						item.AlbumItem = item;
						item.PreviousItem = prevItem;
						if (prevItem != null) prevItem.NextItem = item;
						prevItem = item;
						item.IsStartOfNewAlbum = true;
						item.Index = index;

						if (info.HasFrontCover)
						{
							item.AlbumArt = info.GetFrontCover(100,100);
						}
						else
						{
							m_loader.LoadAlbumArt(item, info.FileName, 100, 100);
						}
						//item.AlbumArt = AlbumArtHelper.GetAlbumArt(info.FileName, 100, 100);
						
						lastAlbum = album;
						m_items.Add(item);
						index += 1;
					}
				}

				m_ticker.Duration = m_items.Count - 1;
				m_ticker.SetPosition(m_ticker.Position);
				this.Invoke((MethodInvoker)delegate { Invalidate(true); });
			};

			doWork.BeginInvoke(null, null);
		}

		private void DisposeItems()
		{
			if (m_items != null && m_items.Count > 0)
			{
				m_items.ForEach(delegate(SongListViewItem item)
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
			m_currentIndex = Convert.ToInt32(m_ticker.Position);
			Invalidate();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			switch (m_scrollDirection)
			{
				case ScrollDirection.None:
				{
					break;
				}
				case ScrollDirection.Left:
				{
					m_ticker.SetPosition(m_ticker.Position - 1);
					break;
				}
				case ScrollDirection.Right:
				{
					m_ticker.SetPosition(m_ticker.Position + 1);
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
			if (m_items == null || m_items.Count == 0) return;

			if (m_currentIndex < 0 || m_currentIndex > m_items.Count - 1)
			{
				return;
			}

			switch (m_paintMode)
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
			using (SolidBrush foreColorBrush = new SolidBrush(ForeColor))
			{
				float x = -(4 * Width / 100) / 2;
				for (int i = m_currentIndex - 4; i <= m_currentIndex + 4; i++)
				{
					int index = i - (m_currentIndex - 4);
					int width = m_albumPercentWidths[index] * Width / 100;
					if (i >= 0 && i < m_items.Count)
					{
						if (m_items[i].AlbumArt != null)
						{
							if (m_viewingDetails && i == m_currentIndex)
							{
								// Ignore the album - it'll get drawn last
							}
							else
							{
								// Draw the album normally
								int itemHeight = width + (m_albumPercentWidths[index] * 2);
								e.Graphics.DrawImage(m_items[i].AlbumArt, x, (Height - itemHeight) / 2, width, width);
								m_items[i].AlbumRectangle = Rectangle.Ceiling(new RectangleF(x, (Height - itemHeight) / 2, width, width));
								using (Font f = new Font(Font.FontFamily, m_albumPercentWidths[index], FontStyle.Regular, GraphicsUnit.Pixel))
								{
									RectangleF rect = new RectangleF(x, ((Height - itemHeight) / 2) + width, width, f.Height);
									e.Graphics.DrawString(m_items[i].SongInfo.Album, f, foreColorBrush, rect);
									rect.Offset(0, m_albumPercentWidths[index]);
									e.Graphics.DrawString(m_items[i].SongInfo.AlbumArtist, f, foreColorBrush, rect);
								}
							}
						}
					}
					x += width;
				}

				if (m_viewingDetails)
				{
					PaintCurrentAlbumHuuuge(e);
				}
			}
		}

		private void PaintALittleFancy(PaintEventArgs e)
		{
			// Determine the offset change in pixels
			int offsetChange = Convert.ToInt32(m_originalOffset / (AlbumPanel.TransitionTime / m_updateTimer.Interval));
			if (offsetChange < 1)
			{
				// We can't change by less than one pixel!
				offsetChange = 1;
			}

			// Determine the offset at which to paint albums
			int paintOffset = 0;
			switch (m_scrollDirection)
			{
				case ScrollDirection.None:
				{
					paintOffset = 0;
					m_offset = 0;
					break;
				}
				case ScrollDirection.Left:
				{
					paintOffset = m_offset * -1;
					m_offset -= offsetChange;
					break;
				}
				case ScrollDirection.Right:
				{
					paintOffset = m_offset;
					m_offset -= offsetChange;
					break;
				}
			}

			using (SolidBrush foreColorBrush = new SolidBrush(this.ForeColor))
			{
				int centerPosition = (this.ClientSize.Width / 2) + paintOffset;
				int centerIndex = 0;
				int[] albumSizes = AlbumSizes(paintOffset, out centerIndex);

				int x = 0;

				// Start with the center album as it will dictate the positions of the rest of the albums
				int size = albumSizes[centerIndex];
				x = centerPosition - (size / 2);
				PaintAlbum(e, m_items[m_currentIndex], foreColorBrush, x, TopCurveY(centerPosition), size);

				// Start painting albums to the right of the center
				x = centerPosition + (albumSizes[centerIndex] / 2) + AlbumPanel.SpaceBetweenAlbums;
				for (int i = 1; i < albumSizes.Length - centerIndex; i++)
				{
					if (m_currentIndex + i >= m_items.Count)
					{
						break;
					}

					size = albumSizes[centerIndex + i];
					int center = x + (size / 2);
					PaintAlbum(e, m_items[m_currentIndex + i], foreColorBrush, x, TopCurveY(center), size);
					x += (size + AlbumPanel.SpaceBetweenAlbums);
				}

				// Start painting albums to the left of the center
				x = centerPosition - (albumSizes[centerIndex] / 2) - AlbumPanel.SpaceBetweenAlbums;
				for (int i = 1; centerIndex - i >= 0; i++)
				{
					if (m_currentIndex == 0)
					{
						break;
					}

					size = albumSizes[centerIndex - i];
					x -= (size + AlbumPanel.SpaceBetweenAlbums);
					int center = x + (size / 2);
					if (m_currentIndex - i > 0)
					PaintAlbum(e, m_items[m_currentIndex - i], foreColorBrush, x, TopCurveY(center), size);
				}

				if (m_viewingDetails)
				{
					PaintCurrentAlbumHuuuge(e);
				}
			}

			if (m_offset <= 0)
			{
				// Finished scrolling
				m_scrollDirection = ScrollDirection.None;
				m_updateTimer.Stop();

				// The offset may be less than 0, so re-align everything nicely
				m_offset = 0;
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

			RectangleF rect = new RectangleF(x, y + size, size, this.Font.Height);
			e.Graphics.DrawString(item.SongInfo.Album, this.Font, textBrush, rect);
			rect.Offset(0, this.Font.Height);
			e.Graphics.DrawString(item.SongInfo.AlbumArtist, this.Font, textBrush, rect);
		}

		private void PaintCurves(PaintEventArgs e)
		{
			// Paint the parabolas that the albums will follow
			List<Point> topPoints = new List<Point>();
			List<Point> bottomPoints = new List<Point>();

			for (int x = 0; x < this.ClientSize.Width; x += 2)
			{
				topPoints.Add(new Point(x, TopCurveY(x)));
				bottomPoints.Add(new Point(x, BottomCurveY(x)));
			}

			Point[] topPointArray = new Point[topPoints.Count];
			topPoints.CopyTo(topPointArray);

			Point[] bottomPointArray = new Point[bottomPoints.Count];
			bottomPoints.CopyTo(bottomPointArray);

			using (Pen pen = new Pen(this.ForeColor))
			{
				e.Graphics.DrawCurve(pen, topPointArray);
				e.Graphics.DrawCurve(pen, bottomPointArray);
			}
		}

		private void PaintCurrentAlbumHuuuge(PaintEventArgs e)
		{
			// Draw the current album fricking huge
			int itemHeight = this.Height - m_ticker.Height - 6;
			Rectangle rect = new Rectangle(Convert.ToInt32((this.Width / 2) - (itemHeight / 2)), 3, itemHeight, itemHeight);

			// First draw a black background
			using (SolidBrush black = new SolidBrush(Color.Black))
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
						new float[] {0, 0, 0, 0, 1}};		//		F'd if I know what the rest is

			ColorMatrix matrix = new ColorMatrix(points);
			ImageAttributes atts = new ImageAttributes();
			atts.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

			Bitmap albumCover = m_items[m_currentIndex].AlbumArt; //.Clone(rect, PixelFormat.DontCare);
			e.Graphics.DrawImage(albumCover, rect, 0, 0, albumCover.Width, albumCover.Height, GraphicsUnit.Pixel, atts);
			m_items[m_currentIndex].AlbumRectangle = Rectangle.Ceiling(rect);

			// And now draw the album info
			ThreePM.MusicLibrary.LibraryEntry[] songs = this.Library.QueryLibrary("Album LIKE '" + m_items[m_currentIndex].SongInfo.Album.Replace("'", "''") + "'");
			using (Font f = new Font(Font.FontFamily, 16, FontStyle.Regular, GraphicsUnit.Pixel))
			{
				using (SolidBrush foreColorBrush = new SolidBrush(ForeColor))
				{
					e.Graphics.DrawString(m_items[m_currentIndex].SongInfo.Album, f, foreColorBrush, rect.X + 3, rect.Y + 3);
					e.Graphics.DrawString(m_items[m_currentIndex].SongInfo.AlbumArtist, f, foreColorBrush, rect.X + 3, rect.Y + 23);

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
			if (m_albumContextMenuStrip != null && m_albumContextMenuStrip.Visible)
			{
				m_albumContextMenuStrip.Hide();
			}

			// Get the index of the album that was clicked
			int clickedIndex = AlbumIndexAtPoint(e.Location);
			if (clickedIndex != -1)
			{
				if (clickedIndex >= m_currentIndex - 2 && clickedIndex <= m_currentIndex + 2)
				{
					if (clickedIndex == m_currentIndex)
					{
						if (e.Button == MouseButtons.Right)
						{
							// Show the context menu for the central album
							if (m_albumContextMenuStrip != null)
							{
								m_albumContextMenuStrip.Show(this, e.Location);
							}
						}
						else
						{
							// Show the album details for the central album
							m_viewingDetails = !m_viewingDetails;
						}
					}
					else
					{
						// Make the clicked album the central one and show its details
						m_currentIndex = clickedIndex;
						m_ticker.Position = clickedIndex;
						m_viewingDetails = true;
					}
				}
				else
				{
					if (m_scrollDirection == ScrollDirection.None)
					{
						if (!m_viewingDetails)
						{
							m_currentIndex = clickedIndex;
							m_ticker.Position = clickedIndex;
						}
					}
					else
					{
						m_scrollDirection = ScrollDirection.None;
						m_timer.Stop();
						m_stopScrollingDammit = true;
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
			m_ticker.SetPosition(m_ticker.Position - (e.Delta * SystemInformation.MouseWheelScrollLines / 120));
			base.OnMouseWheel(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (m_stopScrollingDammit) // || m_viewingDetails)
			{
				m_stopScrollingDammit = false;
				return;
			}

			switch (m_paintMode)
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
				if (hoverIndex >= m_currentIndex - 2 && hoverIndex <= m_currentIndex + 2)
				{
					m_scrollDirection = ScrollDirection.None;
					m_timer.Stop();
				}
				else if (hoverIndex < m_currentIndex)
				{
					m_scrollDirection = ScrollDirection.Left;
					m_timer.Start();
				}
				else if (hoverIndex > m_currentIndex)
				{
					m_scrollDirection = ScrollDirection.Right;
					m_timer.Start();
				}
			}
			else
			{
				m_scrollDirection = ScrollDirection.None;
				m_timer.Stop();
			}
		}

		private void MouseMoveALittleFancy(MouseEventArgs e)
		{
			if (m_scrollDirection != ScrollDirection.None)
			{
				// Chill, we're already scrolling
				//m_scrollDirection = ScrollDirection.None;
				return;
			}

			int hoverIndex = AlbumIndexAtPoint(e.Location);
			if (hoverIndex != -1)
			{
				if (hoverIndex >= m_currentIndex - 2 && hoverIndex <= m_currentIndex + 2)
				{
					m_scrollDirection = ScrollDirection.None;
				}
				else if (hoverIndex < m_currentIndex)
				{
					m_scrollDirection = ScrollDirection.Left;
					TransitionToAlbum();
				}
				else if (hoverIndex > m_currentIndex)
				{
					m_scrollDirection = ScrollDirection.Right;
					TransitionToAlbum();
				}
			}
			else
			{
				m_scrollDirection = ScrollDirection.None;
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			m_scrollDirection = ScrollDirection.None;
			m_timer.Stop();

			base.OnMouseLeave(e);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Right || keyData == Keys.Down)
			{
				//m_ticker.SetPosition(m_ticker.Position + 1);
				m_scrollDirection = ScrollDirection.Right;
				TransitionToAlbum();
				return true;
			}
			else if (keyData == Keys.Left || keyData == Keys.Up)
			{
				//m_ticker.SetPosition(m_ticker.Position - 1);
				m_scrollDirection = ScrollDirection.Left;
				TransitionToAlbum();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		#endregion

		#region Methods

		private void FindAlbum(string album)
		{
			if (m_items == null || m_items.Count == 0) return;
			foreach (SongListViewItem item in m_items)
			{
				if (item.SongInfo.Album.Equals(album, StringComparison.InvariantCultureIgnoreCase))
				{
					m_ticker.SetPosition(item.Index);
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
				result = m_items[index];
			}

			return result;
		}

		private int AlbumIndexAtPoint(Point location)
		{
			int result = -1;

			for (int i = m_currentIndex - 4; i <= m_currentIndex + 4; i++)
			{
				if (i >= 0)
				{
					if (i < m_items.Count)
					{
						if (m_items[i].AlbumRectangle.Contains(location))
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
			x = Math.Abs(x - this.ClientSize.Width /2);

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
			switch (m_scrollDirection)
			{
				case ScrollDirection.None:
				{
					// Shouldn't really be here...
					return;
				}
				case ScrollDirection.Left:
				{
					m_currentIndex -= 1;
					direction = -1;
					m_ticker.Position = m_currentIndex;
					break;
				}
				case ScrollDirection.Right:
				{
					m_currentIndex += 1;
					direction = 1;
					m_ticker.Position = m_currentIndex;
					break;
				}
			}

			// Figure out how much the offset is to start with. It should be half the width of the current
			// album + half the width of the next album + the space between albums
			int centerIndex = 0;
			int[] albumSizes = AlbumSizes(0, out centerIndex);
			m_originalOffset = (albumSizes[centerIndex] / 2) + (albumSizes[centerIndex + direction] / 2);
			m_offset = m_originalOffset;

			// Start updating
			m_updateTimer.Start();
		}

		private int[] AlbumSizes(int offset, out int centerIndex)
		{
			centerIndex = 0;

			List<int> albumSizes = new List<int>();

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
