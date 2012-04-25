using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using starH45.net.mp3.player;
using System.IO;
using System.Drawing.Drawing2D;
using System.Threading;
using starH45.net.mp3.library;

namespace starH45.net.mp3.ui
{
	public partial class SongListView : UserControl
	{
		#region Declarations

		public event EventHandler ListChanged;
		public event EventHandler SongPlayed;
		public event EventHandler SongQueued;

		private bool m_autoDoubleClick = true;
		private int m_trackNumberColumnWidth = -1;
		private int m_statusColumnWidth = -1;
		private int m_titleColumnWidth = -1;
		private int m_artistColumnWidth = -1;
		private int m_albumColumnWidth = -1;
		private int m_playCountColumnWidth = -1;
		private int m_durationColumnWidth = -1;

		private player.Player m_player;
		private library.Library m_library;

		private Rectangle m_dragBox = Rectangle.Empty;

		#endregion Declarations

		#region Properties

		public bool AutoDoubleClick
		{
			get { return m_autoDoubleClick; }
			set { m_autoDoubleClick = value; }
		}

		internal int TrackNumberColumnWidth
		{
			get 
			{
				if (m_trackNumberColumnWidth == -1)
				{
					m_trackNumberColumnWidth = Utilities.GetValue("SongListView.TrackNumberColumnWidth", 25);
				}
				return m_trackNumberColumnWidth;
			}
			set 
			{ 
				m_trackNumberColumnWidth = value;
				Utilities.SetValue("SongListView.TrackNumberColumnWidth", value);
			}
		}

		internal int StatusColumnWidth
		{
			get 
			{
				if (m_statusColumnWidth == -1)
				{
					m_statusColumnWidth = Utilities.GetValue("SongListView.StatusColumnWidth", 10);
				}
				return m_statusColumnWidth;
			}
			set 
			{ 
				m_statusColumnWidth = value;
				Utilities.SetValue("SongListView.StatusColumnWidth", value);
			}
		}

		internal int TitleColumnWidth
		{
			get 
			{
				if (m_titleColumnWidth == -1)
				{
					m_titleColumnWidth = Utilities.GetValue("SongListView.TitleColumnWidth", 200);
				}
				return m_titleColumnWidth;
			}
			set 
			{ 
				m_titleColumnWidth = value;
				Utilities.SetValue("SongListView.TitleColumnWidth", value);
			}
		}

		internal int ArtistColumnWidth
		{
			get 
			{
				if (m_artistColumnWidth == -1)
				{
					m_artistColumnWidth = Utilities.GetValue("SongListView.ArtistColumnWidth", 150);
				}
				return m_artistColumnWidth;
			}
			set 
			{ 
				m_artistColumnWidth = value;
				Utilities.SetValue("SongListView.ArtistColumnWidth", value);
			}
		}

		internal int AlbumColumnWidth
		{
			get 
			{
				if (m_albumColumnWidth == -1)
				{
					m_albumColumnWidth = Utilities.GetValue("SongListView.AlbumColumnWidth", 150);
				}
				return m_albumColumnWidth;
			}
			set 
			{ 
				m_albumColumnWidth = value;
				Utilities.SetValue("SongListView.AlbumColumnWidth", value);
			}
		}

		internal int DurationColumnWidth
		{
			get
			{
				if (m_durationColumnWidth == -1)
				{
					m_durationColumnWidth = Utilities.GetValue("SongListView.DurationColumnWidth", 20);
				}
				return m_durationColumnWidth;
			}
			set
			{
				m_durationColumnWidth = value;
				Utilities.SetValue("SongListView.DurationColumnWidth", value);
			}
		}

		internal int PlayCountColumnWidth
		{
			get 
			{
				if (m_playCountColumnWidth == -1)
				{
					m_playCountColumnWidth = Utilities.GetValue("SongListView.PlayCountColumnWidth", 20);
				}
				return m_playCountColumnWidth;
			}
			set 
			{ 
				m_playCountColumnWidth = value;
				Utilities.SetValue("SongListView.PlayCountColumnWidth", value);
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
		public library.Library Library
		{
			get { return m_library; }
			set
			{
				m_library = value;
				m_library.PlayCountUpdated += new EventHandler<LibraryEntryEventArgs>(m_library_LibraryUpdated);
				m_library.LibraryUpdated += new EventHandler<LibraryEntryEventArgs>(m_library_LibraryUpdated);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public player.Player Player
		{
			get { return m_player; }
			set
			{
				m_player = value;
				Player.SongOpened += new EventHandler<SongEventArgs>(Player_SongOpened);
				Player.Playlist.PlaylistChanged += new EventHandler(Playlist_PlaylistChanged);
				Player.SongForced += new EventHandler(Player_SongForced);
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
				Library.PlayCountUpdated -= new EventHandler<LibraryEntryEventArgs>(m_library_LibraryUpdated);
				Library.LibraryUpdated -= new EventHandler<LibraryEntryEventArgs>(m_library_LibraryUpdated);
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
					m_dragBox = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
				}
				else
				{
					// Reset the rectangle if the mouse is not over an item in the ListBox
					m_dragBox = Rectangle.Empty;
				}
			}
		}

		private void SongListView_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				// If the mouse moves outside the rectangle, start the drag
				if (m_dragBox != Rectangle.Empty && !m_dragBox.Contains(e.X, e.Y))
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
			m_dragBox = Rectangle.Empty;
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

		void list_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
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
				Player.Playlist.EventsEnabled = false;
				SelectedItems.ForEach(delegate(SongListViewItem item)
				{
					if (Player.Playlist.Contains(item.SongInfo.FileName))
					{
						Player.Playlist.Remove(item.SongInfo.FileName);
					}
					else
					{
						Player.Playlist.AddToEnd(item.SongInfo);
					}
				});
				Player.Playlist.EventsEnabled = true;
				if (SongQueued != null)
				{
					SongQueued(this, EventArgs.Empty);
				}
			}
			else if (e.KeyCode == Keys.F)
			{
				if (SelectedItems.Count > 0)
				{
					Player.ForceSong(SelectedItems[0].SongInfo.FileName);
					list.InvalidateItem(SelectedItems[0]);
				}
			}
			else if (e.KeyCode == Keys.Delete)
			{
				SelectedItems.ForEach(delegate(SongListViewItem item)
				{
					Library.Delete(item.SongInfo.FileName);
					list.Items.Remove(item);
				});
				list.MeasureItems();
			}
			else if (e.KeyCode == Keys.I)
			{
				SelectedItems.ForEach(delegate(SongListViewItem item)
				{
					if (item.SongInfo.Ignored)
					{
						Library.UnIgnore(item.SongInfo.FileName);
					}
					else
					{
						Library.Ignore(item.SongInfo.FileName);
					}
				});
			}
			else if (e.KeyCode == Keys.Enter)
			{
				if (SelectedItems.Count > 0)
				{
					Player.PlayFile(SelectedItems[0].SongInfo.FileName);
					if (SongPlayed != null)
					{
						SongPlayed(this, EventArgs.Empty);
					}
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
			if (!m_autoDoubleClick)
			{
				base.OnDoubleClick(e);
			}
			if (SelectedItems.Count > 0)
			{
				Player.PlayFile(SelectedItems[0].SongInfo.FileName);
				if (SongPlayed != null)
				{
					SongPlayed(this, EventArgs.Empty);
				}
			}
		}

		#endregion

		#region Player Events

		void Playlist_PlaylistChanged(object sender, EventArgs e)
		{
			list.Invalidate();
		}

		void Player_SongOpened(object sender, SongEventArgs e)
		{
			SongListViewItem item = FindItem(e.Song.FileName);
			if (item != null)
			{
				list.InvalidateItem(item);
			}
		}
		
		void Player_SongForced(object sender, EventArgs e)
		{
			list.Invalidate();
		}

		void m_library_LibraryUpdated(object sender, LibraryEntryEventArgs e)
		{
			if (e.LibraryEntry == null)
				return;
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
