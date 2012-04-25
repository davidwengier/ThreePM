using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using starH45.net.mp3.player;
using System.Drawing;
using System.ComponentModel;
using starH45.net.mp3.library;
using System.Threading;
using starH45.net.mp3.utilities;

namespace starH45.net.mp3.ui
{
	internal class SongListViewListPanel : ScrollableControl
	{
		#region Declarations

		public event EventHandler ItemsMeasured;

		private bool m_mouseUpSelect;
		private bool m_creating;
		private SongInfo[] m_dataSource;
		private int m_widestalbum;
		private int m_lastClickedIndex = -1;
		private List<SongListViewItem> m_selectedItems = new List<SongListViewItem>();
		private ContextMenuStrip m_itemContextMenuStrip;
		private bool m_showAlbumArt = true;
		private int m_albumArtSize = 100;
		private Font m_headerFont;
		private Font m_currentSongFont;
		private Color m_selectedColor = Color.DarkGray;
		private List<SongListViewItem> m_items;
		private bool m_flatMode;
		private SongListView m_songListView;

		private AlbumArtLoader m_albumArtLoader = new AlbumArtLoader();

		internal int m_dragLineLeft;
		internal bool m_drawDragLine;

		internal int m_dragIndex = -1;

		internal int[] m_colAutoWidths = new int[6];	// Stores the widths of columns for auto-sizing

		private const string NoResultsString = "No tracks to display.";

		#endregion

		#region Properties

		public bool FlatMode
		{
			get { return m_flatMode; }
			set
			{
				m_flatMode = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SongListView ListView
		{
			get { return m_songListView; }
			internal set { m_songListView = value; }
		}

		internal int WidestAlbum
		{
			get { return m_widestalbum; }
		}

		public ContextMenuStrip ItemContextMenuStrip
		{
			get { return m_itemContextMenuStrip; }
			set { m_itemContextMenuStrip = value; }
		}

		public Color SelectedColor
		{
			get { return m_selectedColor; }
			set { m_selectedColor = value; }
		}

		public int AlbumArtSize
		{
			get { return m_albumArtSize; }
			set
			{
				m_albumArtSize = value;
				CreateItems();
			}
		}

		public bool ShowAlbumArt
		{
			get { return m_showAlbumArt; }
			set
			{
				m_showAlbumArt = value;
				CreateItems();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SongInfo[] DataSource
		{
			get { return m_dataSource; }
			set
			{
				m_dataSource = value;
				AutoScrollPosition = new Point(0, 0);
				CreateItems();
			}
		}

		protected override void OnFontChanged(EventArgs e)
		{
			if (m_headerFont == null)
				m_headerFont = new Font(Font, FontStyle.Bold);
			if (m_currentSongFont == null)
				m_currentSongFont = new Font(Font, FontStyle.Bold);
			// We have to call MeasureItems so it re-calcs the font height
			MeasureItems();
		}

		public Font HeaderFont
		{
			get { return m_headerFont; }
			set
			{
				if (m_headerFont != null) m_headerFont.Dispose();
				m_headerFont = value;
				// We have to call MeasureItems so it re-calcs the font height
				MeasureItems();
			}
		}

		public Font CurrentSongFont
		{
			get { return m_currentSongFont; }
			set
			{
				if (m_currentSongFont != null) m_currentSongFont.Dispose();
				m_currentSongFont = value;
				// We have to call MeasureItems so it re-calcs the font height
				MeasureItems();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<SongListViewItem> Items
		{
			get { return m_items; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<SongListViewItem> SelectedItems
		{
			get { return m_selectedItems; }
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
				if (m_headerFont != null) m_headerFont.Dispose();
				if (m_currentSongFont != null) m_currentSongFont.Dispose();
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

			for (int i = 0; i < m_items.Count; i++)
			{
				SongListViewItem item = m_items[i];
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
				if (m_selectedItems.Contains(item))
				{
					m_mouseUpSelect = true;
				}
				else
				{
					HandleMouseSelection(e, item);
				}

				if (e.Button == MouseButtons.Right)
				{
					if (m_itemContextMenuStrip != null)
					{
						m_itemContextMenuStrip.Show(this, e.Location);
					}
				}
			}
			else
			{
				m_selectedItems.Clear();
				m_lastClickedIndex = -1;
				Invalidate();
			}

			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (m_mouseUpSelect)
			{
				int x = e.X - this.AutoScrollPosition.X;
				int y = e.Y - this.AutoScrollPosition.Y;

				SongListViewItem item = GetItemAt(x, y);
				if (item != null)
				{
					HandleMouseSelection(e, item);
					m_mouseUpSelect = false;
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
			else if (Control.ModifierKeys == Keys.Shift && m_lastClickedIndex != -1)
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

			if (m_selectedItems.Contains(item))
			{
				m_selectedItems.Remove(item);
			}
			else
			{
				m_selectedItems.Add(item);
			}
			m_lastClickedIndex = item.Index;
		}

		private void HandleShiftMouseSelection(SongListViewItem clickedItem)
		{
			// If the user's holding down shift, add everything between this item and the last clicked item

			m_selectedItems.Clear();

			// Get the index of the selected item
			int selectedIndex = clickedItem.Index;

			m_items.ForEach(delegate(SongListViewItem item)
			{
				if ((item.Index >= m_lastClickedIndex && item.Index <= selectedIndex) ||
					(item.Index <= m_lastClickedIndex && item.Index >= selectedIndex))
				{
					m_selectedItems.Add(item);
				}
			});
		}

		private void HandleNormalMouseSelection(bool rightButtonClicked, SongListViewItem item)
		{
			// If the user's not holding down a key, just select this item and deselect every other one
			// I lied a little just there.  If the user is right-clicking on an already selected item,
			// we want to leave the selection as is

			if (rightButtonClicked && m_selectedItems.Contains(item))
			{
				// Pretty much don't do anything...
			}
			else
			{
				m_selectedItems.Clear();
				m_selectedItems.Add(item);
				m_lastClickedIndex = item.Index;
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

					SelectedItems.Clear();
					SelectedItems.Add(newItem);
					m_lastClickedIndex = newItem.Index;
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
				m_albumArtLoader.Clear();
				m_creating = true;
				DisposeItems();
				m_items = new List<SongListViewItem>();
				m_colAutoWidths = new int[6];

				if (m_dataSource != null && m_dataSource.Length > 0)
				{
					string lastAlbum = string.Empty;
					string lastDir = string.Empty;
					SongListViewItem albumItem = null;

					SongListViewItem prevItem = null;

					int flatAlbumWidth = Font.Height;
					int index = 0;
					foreach (SongInfo info in m_dataSource)
					{
						SongListViewItem item = new SongListViewItem(this);
						item.SongInfo = info;

						item.PreviousItem = prevItem;
						if (prevItem != null) prevItem.NextItem = item;
						prevItem = item;

						if (String.IsNullOrEmpty(item.AlbumDisplay)) item.AlbumDisplay = "Unknown Album";
						if (m_flatMode || (lastAlbum != item.SongInfo.Album || lastDir != System.IO.Path.GetDirectoryName(info.FileName)))
						{

							// if its not flat mode, and the album name is the same, tack on the path
							if (!m_flatMode && lastAlbum == item.SongInfo.Album)
							{
								item.AlbumDisplay = item.AlbumDisplay + " (" + System.IO.Path.GetPathRoot(info.FileName) + ")";
								if (item.PreviousItem != null && !item.PreviousItem.AlbumItem.AlbumDisplay.EndsWith(" (" + System.IO.Path.GetPathRoot(item.PreviousItem.SongInfo.FileName) + ")"))
								{
									item.PreviousItem.AlbumItem.AlbumDisplay = item.PreviousItem.AlbumItem.AlbumDisplay + " (" + System.IO.Path.GetPathRoot(item.PreviousItem.SongInfo.FileName) + ")";
								}
							}

							albumItem = item;
							item.IsStartOfNewAlbum = true;

							if (m_showAlbumArt && m_flatMode)
							{
								item.AlbumArtSize = flatAlbumWidth;
								// This is never true, because our SongInfo objects come from the database.. should be store album art?
								if (info.HasFrontCover)
								{
									item.AlbumArt = info.GetFrontCover(flatAlbumWidth, flatAlbumWidth);
								}
								else
								{
									m_albumArtLoader.LoadAlbumArt(item, info.FileName, flatAlbumWidth, flatAlbumWidth);
								}
							}
							else
							{
								if (m_showAlbumArt && m_albumArtSize > 0)
								{
									item.AlbumArtSize = m_albumArtSize;
									if (info.HasFrontCover)
									{
										item.AlbumArt = info.GetFrontCover(m_albumArtSize, m_albumArtSize);
									}
									else
									{
										m_albumArtLoader.LoadAlbumArt(item, info.FileName, m_albumArtSize, m_albumArtSize);
									}
								}
							}

							lastAlbum = item.SongInfo.Album;
							lastDir = System.IO.Path.GetDirectoryName(info.FileName);
						}

						item.AlbumItem = albumItem;

						item.Index = index;

						m_items.Add(item);

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

						if (String.IsNullOrEmpty(info.Title))
						{
							MaybeSetColumnAutoWidth(0, (int)g.MeasureString(info.FileName, titleFont).Width + spacer);

							// Skip the next couple of columns because they are taken up by the file name
							LibraryEntry entry = info as LibraryEntry;
							if (entry != null)
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

							LibraryEntry entry = info as LibraryEntry;
							if (entry != null)
							{
								MaybeSetColumnAutoWidth(5, (int)g.MeasureString(entry.PlayCount.ToString(), regularFont).Width + spacer);
							}
						}
					}
				}
			}

			MeasureItems();

			this.SelectedItems.Clear();
			m_lastClickedIndex = -1;

			m_creating = false;
		}

		private void MaybeSetColumnAutoWidth(int index, int width)
		{
			if (m_colAutoWidths[index] < width)
			{
				m_colAutoWidths[index] = width;
			}
		}

		internal void ReCreateAndMeasure()
		{
			m_creating = true;
			SongListViewItem prevItem = null;

			int i = 0;
			foreach (SongListViewItem item in m_items)
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
			m_creating = false;
		}

		internal void InvalidateItem(SongListViewItem item)
		{
			Rectangle r = new Rectangle(item.Rectangle.Location, item.Rectangle.Size);
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

		internal void MeasureItems()
		{
			int right = 0;
			int bottom = 0;

			if (m_items == null || m_items.Count == 0)
			{
				// create a temp bitmap to work with
				Bitmap btm = new Bitmap(10, 10, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				Graphics g = Graphics.FromImage(btm);
				SizeF s = SongListViewItem.MeasureColumn(g, NoResultsString, -1, Font);
				right = Convert.ToInt32(s.Width);
				bottom = Convert.ToInt32(s.Height);
			}
			else
			{

				// create a temp bitmap to work with
				Bitmap btm = new Bitmap(10, 10, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				Graphics g = Graphics.FromImage(btm);

				m_widestalbum = 0;

				// just measure the albums first, so we know how wide the widest one is
				if (m_showAlbumArt)
				{
					m_items.ForEach(delegate(SongListViewItem item)
					{
						if (item.IsStartOfNewAlbum)
						{
							item.MeasureAlbum(g, 0, 0, m_headerFont, Font);
							m_widestalbum = Math.Max(m_widestalbum, item.AlbumRectangle.Right + 1);
						}
					});
				}
				else
				{
					m_widestalbum = 0;
				}

				// now unfortunately we have to remeasure everything, since the albums have moved
				int y = 0;
				int lastAlbumBottom = 0;
				m_items.ForEach(delegate(SongListViewItem item)
				{
					if (item.IsStartOfNewAlbum)
					{
						if (y <= lastAlbumBottom) y = lastAlbumBottom;
						// move this album down a bit from the previous one
						if (!m_flatMode)
						{
							y += 10;
						}
						item.MeasureAlbum(g, 0, y, m_headerFont, Font);
						right = Math.Max(right, item.AlbumRectangle.Right);
						bottom = Math.Max(bottom, item.AlbumRectangle.Bottom);
						lastAlbumBottom = item.AlbumRectangle.Bottom;
					}
					item.MeasureItem(g, m_widestalbum, y, Font);

					y = item.Rectangle.Bottom;

					right = Math.Max(right, item.Rectangle.Right);
					bottom = Math.Max(bottom, item.Rectangle.Bottom);
				});
			}
			MethodInvoker DoWork = delegate { AutoScrollMinSize = new Size(right + 10, bottom + 10); };
			if (InvokeRequired)
			{
				Invoke(DoWork);
			}
			else
			{
				DoWork();
			}

			if (ItemsMeasured != null)
			{
				ItemsMeasured(this, EventArgs.Empty);
			}

			Invalidate();
		}

		#endregion

		#region Overridden Methods

		protected override void OnPaint(PaintEventArgs e)
		{
			if (m_items == null) return;

			try
			{
				Graphics g = e.Graphics;

				int x = AutoScrollPosition.X;
				int y = AutoScrollPosition.Y;

				Rectangle clip = new Rectangle(e.ClipRectangle.X - AutoScrollPosition.X, e.ClipRectangle.Y - AutoScrollPosition.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);
				using (SolidBrush foreColorBrush = new SolidBrush(ForeColor), ignoredBrush = new SolidBrush(Color.FromArgb(100, ForeColor)))
				{
					if ((m_items == null || m_items.Count == 0) && !m_creating)
					{
						g.DrawString(NoResultsString, Font, foreColorBrush, 0,0);
					}
					else
					{
						m_items.ForEach(delegate(SongListViewItem item)
						{
							if (item.IsStartOfNewAlbum)
							{
								if (clip.IntersectsWith(item.AlbumRectangle))
								{
									item.PaintAlbum(e.Graphics, x, y, foreColorBrush);
								}
							}
							if (clip.IntersectsWith(item.Rectangle) || m_selectedItems.Contains(item))
							{
								item.PaintItem(e.Graphics, x, y, item.SongInfo.Ignored ? ignoredBrush : foreColorBrush);
							}
							if (item.Index == m_dragIndex)
							{
								using (Pen linePen = new Pen(Color.FromArgb(170, ForeColor)))
								{
									g.DrawLine(linePen, 0, item.Rectangle.Y + y, Width, item.Rectangle.Y + y);
								}
							}
						});
					}

					if (m_drawDragLine)
					{
						using (Pen linePen = new Pen(Color.FromArgb(170, ForeColor)))
						{
							g.DrawLine(linePen, m_dragLineLeft, 0, m_dragLineLeft, Height);
						}
					}
				}
			}
			catch { }
		}

		#endregion
	}
}
