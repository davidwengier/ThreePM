namespace starH45.net.mp3.ui
{
	partial class SongListView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.list = new SongListViewListPanel();
			this.header = new SongListViewHeader();
			this.SuspendLayout();
			// 
			// list
			// 
			this.list.AlbumArtSize = 100;
			this.list.AllowDrop = true;
			this.list.AutoScroll = true;
			this.list.AutoScrollMinSize = new System.Drawing.Size(125, 26);
			this.list.CurrentSongFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.list.Dock = System.Windows.Forms.DockStyle.Fill;
			this.list.FlatMode = false;
			this.list.HeaderFont = new System.Drawing.Font("Tahoma", 9F);
			this.list.ItemContextMenuStrip = null;
			this.list.Location = new System.Drawing.Point(0, 17);
			this.list.Name = "list";
			this.list.SelectedColor = System.Drawing.Color.DarkGray;
			this.list.ShowAlbumArt = true;
			this.list.Size = new System.Drawing.Size(150, 133);
			this.list.TabIndex = 0;
			this.list.Text = "songListViewListPanel1";
			this.list.DoubleClick += new System.EventHandler(this.SongListView_DoubleClick);
			this.list.DragOver += new System.Windows.Forms.DragEventHandler(this.list_DragOver);
			this.list.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SongListView_MouseDown);
			this.list.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SongListView_MouseMove);
			this.list.DragDrop += new System.Windows.Forms.DragEventHandler(this.list_DragDrop);
			this.list.DragEnter += new System.Windows.Forms.DragEventHandler(this.list_DragEnter);
			this.list.Scroll += new System.Windows.Forms.ScrollEventHandler(this.list_Scroll);
			this.list.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SongListView_MouseUp);
			this.list.ItemsMeasured += new System.EventHandler(this.list_ItemsMeasured);
			this.list.DragLeave += new System.EventHandler(this.list_DragLeave);
			this.list.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SongListView_KeyDown);
			// 
			// header
			// 
			this.header.Dock = System.Windows.Forms.DockStyle.Top;
			this.header.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.header.LineColorLight = System.Drawing.Color.White;
			this.header.Location = new System.Drawing.Point(0, 0);
			this.header.Name = "header";
			this.header.Size = new System.Drawing.Size(150, 17);
			this.header.TabIndex = 0;
			this.header.Text = "songListViewHeader1";
			this.header.XOffset = 0;
			// 
			// SongListView
			// 
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.Add(this.list);
			this.Controls.Add(this.header);
			this.Font = new System.Drawing.Font("Tahoma", 9F);
			this.ForeColor = System.Drawing.Color.Red;
			this.Name = "SongListView";
			this.ResumeLayout(false);

		}

		#endregion

		private SongListViewHeader header;
		private SongListViewListPanel list;
	}
}
