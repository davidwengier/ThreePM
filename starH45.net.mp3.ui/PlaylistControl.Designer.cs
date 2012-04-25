namespace starH45.net.mp3.ui
{
	partial class PlaylistControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlaylistControl));
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnDown = new System.Windows.Forms.Button();
			this.btnUp = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.btnPlaylistStyle = new System.Windows.Forms.Button();
			this.btnClearPlaylist = new System.Windows.Forms.Button();
			this.btnOpenPlaylist = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.songListView = new SongListView();
			this.SuspendLayout();
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemove.FlatAppearance.BorderSize = 0;
			this.btnRemove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
			this.btnRemove.Location = new System.Drawing.Point(384, 67);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(26, 24);
			this.btnRemove.TabIndex = 14;
			this.toolTip1.SetToolTip(this.btnRemove, "Delete");
			this.btnRemove.UseVisualStyleBackColor = true;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnDown
			// 
			this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDown.FlatAppearance.BorderSize = 0;
			this.btnDown.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
			this.btnDown.Location = new System.Drawing.Point(384, 37);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(26, 24);
			this.btnDown.TabIndex = 15;
			this.toolTip1.SetToolTip(this.btnDown, "Move Down");
			this.btnDown.UseVisualStyleBackColor = true;
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// btnUp
			// 
			this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUp.FlatAppearance.BorderSize = 0;
			this.btnUp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnUp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
			this.btnUp.Location = new System.Drawing.Point(384, 3);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(26, 24);
			this.btnUp.TabIndex = 13;
			this.toolTip1.SetToolTip(this.btnUp, "Move Up");
			this.btnUp.UseVisualStyleBackColor = true;
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnPlaylistStyle
			// 
			this.btnPlaylistStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPlaylistStyle.FlatAppearance.BorderSize = 0;
			this.btnPlaylistStyle.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnPlaylistStyle.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnPlaylistStyle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPlaylistStyle.Image = ((System.Drawing.Image)(resources.GetObject("btnPlaylistStyle.Image")));
			this.btnPlaylistStyle.Location = new System.Drawing.Point(384, 308);
			this.btnPlaylistStyle.Name = "btnPlaylistStyle";
			this.btnPlaylistStyle.Size = new System.Drawing.Size(26, 24);
			this.btnPlaylistStyle.TabIndex = 14;
			this.toolTip1.SetToolTip(this.btnPlaylistStyle, "Playlist Style");
			this.btnPlaylistStyle.UseVisualStyleBackColor = true;
			this.btnPlaylistStyle.Click += new System.EventHandler(this.btnPlaylistStyle_Click);
			// 
			// btnClearPlaylist
			// 
			this.btnClearPlaylist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClearPlaylist.FlatAppearance.BorderSize = 0;
			this.btnClearPlaylist.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnClearPlaylist.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnClearPlaylist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClearPlaylist.Image = ((System.Drawing.Image)(resources.GetObject("btnClearPlaylist.Image")));
			this.btnClearPlaylist.Location = new System.Drawing.Point(384, 278);
			this.btnClearPlaylist.Name = "btnClearPlaylist";
			this.btnClearPlaylist.Size = new System.Drawing.Size(26, 24);
			this.btnClearPlaylist.TabIndex = 14;
			this.toolTip1.SetToolTip(this.btnClearPlaylist, "Clear Playlist");
			this.btnClearPlaylist.UseVisualStyleBackColor = true;
			this.btnClearPlaylist.Click += new System.EventHandler(this.btnClearPlaylist_Click);
			// 
			// btnOpenPlaylist
			// 
			this.btnOpenPlaylist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOpenPlaylist.FlatAppearance.BorderSize = 0;
			this.btnOpenPlaylist.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnOpenPlaylist.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnOpenPlaylist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnOpenPlaylist.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenPlaylist.Image")));
			this.btnOpenPlaylist.Location = new System.Drawing.Point(384, 248);
			this.btnOpenPlaylist.Name = "btnOpenPlaylist";
			this.btnOpenPlaylist.Size = new System.Drawing.Size(26, 24);
			this.btnOpenPlaylist.TabIndex = 14;
			this.toolTip1.SetToolTip(this.btnOpenPlaylist, "Open Playlist");
			this.btnOpenPlaylist.UseVisualStyleBackColor = true;
			this.btnOpenPlaylist.Click += new System.EventHandler(this.btnOpenPlaylist_Click);
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.FlatAppearance.BorderSize = 0;
			this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
			this.btnSave.Location = new System.Drawing.Point(384, 218);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(26, 24);
			this.btnSave.TabIndex = 14;
			this.toolTip1.SetToolTip(this.btnSave, "Save Playlist");
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// songListView
			// 
			this.songListView.AlbumArtSize = 100;
			this.songListView.AllowDrop = true;
			this.songListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.songListView.AutoDoubleClick = false;
			this.songListView.BackColor = System.Drawing.Color.Black;
			this.songListView.CurrentSongFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.songListView.FlatMode = true;
			this.songListView.Font = new System.Drawing.Font("Tahoma", 9F);
			this.songListView.ForeColor = System.Drawing.Color.Red;
			this.songListView.HeaderFont = new System.Drawing.Font("Tahoma", 9F);
			this.songListView.ItemContextMenuStrip = null;
			this.songListView.Location = new System.Drawing.Point(3, 3);
			this.songListView.Name = "songListView";
			this.songListView.SelectedColor = System.Drawing.Color.DarkGray;
			this.songListView.ShowAlbumArt = false;
			this.songListView.Size = new System.Drawing.Size(370, 329);
			this.songListView.TabIndex = 17;
			this.songListView.DoubleClick += new System.EventHandler(this.songListView_DoubleClick);
			this.songListView.ListChanged += new System.EventHandler(this.songListView_ListChanged);
			this.songListView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.songListView_KeyUp);
			// 
			// PlaylistControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.Add(this.songListView);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnOpenPlaylist);
			this.Controls.Add(this.btnClearPlaylist);
			this.Controls.Add(this.btnPlaylistStyle);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnDown);
			this.Controls.Add(this.btnUp);
			this.Font = new System.Drawing.Font("Tahoma", 9F);
			this.ForeColor = System.Drawing.Color.Red;
			this.Name = "PlaylistControl";
			this.Size = new System.Drawing.Size(416, 336);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button btnPlaylistStyle;
		private System.Windows.Forms.Button btnClearPlaylist;
		private System.Windows.Forms.Button btnOpenPlaylist;
		private System.Windows.Forms.Button btnSave;
		private SongListView songListView;
	}
}
