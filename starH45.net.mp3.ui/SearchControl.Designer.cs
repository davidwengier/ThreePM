namespace starH45.net.mp3.ui
{
	partial class SearchControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchControl));
			this.btnPlaylist = new System.Windows.Forms.Button();
			this.songListView1 = new SongListView();
			this.btnPlay = new System.Windows.Forms.Button();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.cmSongs = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnuPlayFile = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuQueueSong = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuQueueAlbum = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuForceSong = new System.Windows.Forms.ToolStripMenuItem();
			this.txtTitleSearch = new System.Windows.Forms.TextBox();
			this.txtArtistSearch = new System.Windows.Forms.TextBox();
			this.txtAlbumSearch = new System.Windows.Forms.TextBox();
			this.lblArtist = new System.Windows.Forms.Label();
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblAlbum = new System.Windows.Forms.Label();
			this.lblAllText = new System.Windows.Forms.Label();
			this.tmrSearch = new System.Windows.Forms.Timer(this.components);
			this.txtLyrics = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnNext = new System.Windows.Forms.Button();
			this.cmSongs.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnPlaylist
			// 
			this.btnPlaylist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPlaylist.FlatAppearance.BorderSize = 0;
			this.btnPlaylist.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnPlaylist.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnPlaylist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPlaylist.Image = ((System.Drawing.Image)(resources.GetObject("btnPlaylist.Image")));
			this.btnPlaylist.Location = new System.Drawing.Point(380, 3);
			this.btnPlaylist.Name = "btnPlaylist";
			this.btnPlaylist.Size = new System.Drawing.Size(26, 24);
			this.btnPlaylist.TabIndex = 5;
			this.btnPlaylist.UseVisualStyleBackColor = true;
			this.btnPlaylist.Click += new System.EventHandler(this.btnPlaylist_Click);
			// 
			// btnPlay
			// 
			this.btnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPlay.FlatAppearance.BorderSize = 0;
			this.btnPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPlay.Image = ((System.Drawing.Image)(resources.GetObject("btnPlay.Image")));
			this.btnPlay.Location = new System.Drawing.Point(380, 33);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(26, 24);
			this.btnPlay.TabIndex = 6;
			this.btnPlay.UseVisualStyleBackColor = true;
			this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);

			this.songListView1.AlbumArtSize = 100;
			this.songListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.songListView1.AutoDoubleClick = true;
			this.songListView1.BackColor = System.Drawing.Color.Black;
			this.songListView1.CurrentSongFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.songListView1.FlatMode = true;
			this.songListView1.Font = new System.Drawing.Font("Tahoma", 9F);
			this.songListView1.ForeColor = System.Drawing.Color.Red;
			this.songListView1.HeaderFont = new System.Drawing.Font("Tahoma", 9F);
			this.songListView1.ItemContextMenuStrip = this.cmSongs;
			this.songListView1.Location = new System.Drawing.Point(6, 143);
			this.songListView1.Name = "songListView1";
			this.songListView1.SelectedColor = System.Drawing.Color.DarkGray;
			this.songListView1.ShowAlbumArt = true;
			this.songListView1.Size = new System.Drawing.Size(367, 142);
			this.songListView1.TabIndex = 15;
			this.songListView1.SongQueued += new System.EventHandler(this.songListView1_SongQueued);
			this.songListView1.SongPlayed += new System.EventHandler(this.songListView1_SongPlayed);
 
			// 
			// txtSearch
			// 
			this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtSearch.BackColor = System.Drawing.Color.Black;
			this.txtSearch.ForeColor = System.Drawing.Color.Red;
			this.txtSearch.Location = new System.Drawing.Point(62, 3);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(311, 22);
			this.txtSearch.TabIndex = 0;
			this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
			this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
			// 
			// cmSongs
			// 
			this.cmSongs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPlayFile,
            this.mnuQueueSong,
            this.mnuQueueAlbum,
            this.mnuForceSong});
			this.cmSongs.Name = "contextMenuStrip2";
			this.cmSongs.Size = new System.Drawing.Size(200, 92);
			// 
			// mnuPlayFile
			// 
			this.mnuPlayFile.Image = ((System.Drawing.Image)(resources.GetObject("mnuPlayFile.Image")));
			this.mnuPlayFile.Name = "mnuPlayFile";
			this.mnuPlayFile.Size = new System.Drawing.Size(199, 22);
			this.mnuPlayFile.Text = "Play File";
			this.mnuPlayFile.Click += new System.EventHandler(this.mnuPlayFile_Click);
			// 
			// mnuQueueSong
			// 
			this.mnuQueueSong.Image = ((System.Drawing.Image)(resources.GetObject("mnuQueueSong.Image")));
			this.mnuQueueSong.Name = "mnuQueueSong";
			this.mnuQueueSong.Size = new System.Drawing.Size(199, 22);
			this.mnuQueueSong.Text = "Queue/Un-Queue Song";
			this.mnuQueueSong.Click += new System.EventHandler(this.mnuQueueSong_Click);
			// 
			// mnuQueueAlbum
			// 
			this.mnuQueueAlbum.Image = ((System.Drawing.Image)(resources.GetObject("mnuQueueAlbum.Image")));
			this.mnuQueueAlbum.Name = "mnuQueueAlbum";
			this.mnuQueueAlbum.Size = new System.Drawing.Size(199, 22);
			this.mnuQueueAlbum.Text = "Queue Album";
			this.mnuQueueAlbum.Click += new System.EventHandler(this.mnuQueueAlbum_Click);
			// 
			// mnuForceSong
			// 
			this.mnuForceSong.Image = ((System.Drawing.Image)(resources.GetObject("mnuForceSong.Image")));
			this.mnuForceSong.Name = "mnuForceSong";
			this.mnuForceSong.Size = new System.Drawing.Size(199, 22);
			this.mnuForceSong.Text = "Force Song to Play Next";
			this.mnuForceSong.Click += new System.EventHandler(this.mnuForceSong_Click);
			// 
			// txtTitleSearch
			// 
			this.txtTitleSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtTitleSearch.BackColor = System.Drawing.Color.Black;
			this.txtTitleSearch.ForeColor = System.Drawing.Color.Red;
			this.txtTitleSearch.Location = new System.Drawing.Point(92, 31);
			this.txtTitleSearch.Name = "txtTitleSearch";
			this.txtTitleSearch.Size = new System.Drawing.Size(281, 22);
			this.txtTitleSearch.TabIndex = 1;
			this.txtTitleSearch.TextChanged += new System.EventHandler(this.txtTitleSearch_TextChanged);
			// 
			// txtArtistSearch
			// 
			this.txtArtistSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtArtistSearch.BackColor = System.Drawing.Color.Black;
			this.txtArtistSearch.ForeColor = System.Drawing.Color.Red;
			this.txtArtistSearch.Location = new System.Drawing.Point(92, 59);
			this.txtArtistSearch.Name = "txtArtistSearch";
			this.txtArtistSearch.Size = new System.Drawing.Size(281, 22);
			this.txtArtistSearch.TabIndex = 2;
			this.txtArtistSearch.TextChanged += new System.EventHandler(this.txtArtistSearch_TextChanged);
			// 
			// txtAlbumSearch
			// 
			this.txtAlbumSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtAlbumSearch.BackColor = System.Drawing.Color.Black;
			this.txtAlbumSearch.ForeColor = System.Drawing.Color.Red;
			this.txtAlbumSearch.Location = new System.Drawing.Point(92, 87);
			this.txtAlbumSearch.Name = "txtAlbumSearch";
			this.txtAlbumSearch.Size = new System.Drawing.Size(281, 22);
			this.txtAlbumSearch.TabIndex = 3;
			this.txtAlbumSearch.TextChanged += new System.EventHandler(this.txtAlbumSearch_TextChanged);
			// 
			// lblArtist
			// 
			this.lblArtist.AutoSize = true;
			this.lblArtist.Location = new System.Drawing.Point(41, 62);
			this.lblArtist.Name = "lblArtist";
			this.lblArtist.Size = new System.Drawing.Size(40, 14);
			this.lblArtist.TabIndex = 11;
			this.lblArtist.Text = "Artist:";
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Location = new System.Drawing.Point(41, 34);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(35, 14);
			this.lblTitle.TabIndex = 12;
			this.lblTitle.Text = "Title:";
			// 
			// lblAlbum
			// 
			this.lblAlbum.AutoSize = true;
			this.lblAlbum.Location = new System.Drawing.Point(41, 90);
			this.lblAlbum.Name = "lblAlbum";
			this.lblAlbum.Size = new System.Drawing.Size(45, 14);
			this.lblAlbum.TabIndex = 13;
			this.lblAlbum.Text = "Album:";
			// 
			// lblAllText
			// 
			this.lblAllText.AutoSize = true;
			this.lblAllText.Location = new System.Drawing.Point(3, 8);
			this.lblAllText.Name = "lblAllText";
			this.lblAllText.Size = new System.Drawing.Size(53, 14);
			this.lblAllText.TabIndex = 14;
			this.lblAllText.Text = "All Text:";
			// 
			// tmrSearch
			// 
			this.tmrSearch.Interval = 500;
			this.tmrSearch.Tick += new System.EventHandler(this.tmrSearch_Tick);
			// 
			// txtLyrics
			// 
			this.txtLyrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLyrics.BackColor = System.Drawing.Color.Black;
			this.txtLyrics.ForeColor = System.Drawing.Color.Red;
			this.txtLyrics.Location = new System.Drawing.Point(62, 115);
			this.txtLyrics.Name = "txtLyrics";
			this.txtLyrics.Size = new System.Drawing.Size(311, 22);
			this.txtLyrics.TabIndex = 3;
			this.txtLyrics.TextChanged += new System.EventHandler(this.txtLyrics_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 118);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 14);
			this.label1.TabIndex = 13;
			this.label1.Text = "Lyrics:";
			// 
			// btnNext
			// 
			this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnNext.FlatAppearance.BorderSize = 0;
			this.btnNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnNext.Image = ((System.Drawing.Image)(resources.GetObject("btnNext.Image")));
			this.btnNext.Location = new System.Drawing.Point(380, 264);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(26, 24);
			this.btnNext.TabIndex = 6;
			this.btnNext.UseVisualStyleBackColor = true;
			this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
			// 
			// SearchControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.Add(this.lblAllText);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblAlbum);
			this.Controls.Add(this.txtAlbumSearch);
			this.Controls.Add(this.lblTitle);
			this.Controls.Add(this.songListView1);
			this.Controls.Add(this.txtArtistSearch);
			this.Controls.Add(this.lblArtist);
			this.Controls.Add(this.txtLyrics);
			this.Controls.Add(this.txtTitleSearch);
			this.Controls.Add(this.btnPlaylist);
			this.Controls.Add(this.btnPlay);
			this.Controls.Add(this.txtSearch);
			this.Controls.Add(this.btnNext);
			this.Font = new System.Drawing.Font("Tahoma", 9F);
			this.ForeColor = System.Drawing.Color.Red;
			this.Name = "SearchControl";
			this.Size = new System.Drawing.Size(410, 291);
			this.cmSongs.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnPlaylist;
		private System.Windows.Forms.Button btnPlay;
		private System.Windows.Forms.TextBox txtSearch;
		private System.Windows.Forms.TextBox txtTitleSearch;
		private System.Windows.Forms.TextBox txtArtistSearch;
		private System.Windows.Forms.TextBox txtAlbumSearch;
		private System.Windows.Forms.Label lblArtist;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Label lblAlbum;
		private System.Windows.Forms.Label lblAllText;
		private System.Windows.Forms.ContextMenuStrip cmSongs;
		private System.Windows.Forms.ToolStripMenuItem mnuPlayFile;
		private System.Windows.Forms.ToolStripMenuItem mnuQueueSong;
		private System.Windows.Forms.ToolStripMenuItem mnuQueueAlbum;
		private System.Windows.Forms.Timer tmrSearch;
		private System.Windows.Forms.TextBox txtLyrics;
		private System.Windows.Forms.Label label1;
		private SongListView songListView1;
		private System.Windows.Forms.ToolStripMenuItem mnuForceSong;
		private System.Windows.Forms.Button btnNext;
	}
}
