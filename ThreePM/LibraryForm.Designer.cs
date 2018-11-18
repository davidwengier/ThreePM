using ThreePM.UI;
namespace ThreePM
{
	partial class LibraryForm
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
				//albumArtThread.Abort();
				//albumArtThread.Join();
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LibraryForm));
			this.tvwLibrary = new System.Windows.Forms.TreeView();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.queueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.imlLibrary = new System.Windows.Forms.ImageList(this.components);
			this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.playFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.queueFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ignoreUnIgnoreSongToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteUnDeleteSongToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuQueueAlbum = new System.Windows.Forms.ToolStripMenuItem();
			this.openContainingFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.downloadAlbumArtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.downloadLyricsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuEditTrackInformation = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuReadTrackInformation = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.jumpToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.artistalbumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.albumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.artistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.albumArtistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.genreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.yearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.customSep = new System.Windows.Forms.ToolStripSeparator();
			this.customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.playlistControl = new ThreePM.UI.PlaylistControl();
			this.searchControl = new ThreePM.UI.SearchControl();
			this.infoControl = new ThreePM.UI.InfoControl();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.lblLoading = new System.Windows.Forms.Label();
			this.statisticsControl = new ThreePM.UI.StatisticsControl();
			this.songListView = new ThreePM.UI.SongListView();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.pctShowTree = new System.Windows.Forms.PictureBox();
			this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.nowPlayingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.searchLibraryContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.playlistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.topPlayedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recentlyPlayedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recentlyAddedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ignoredTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deletedTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.jumpTo2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.contextMenuStrip2.SuspendLayout();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctShowTree)).BeginInit();
			this.contextMenuStrip3.SuspendLayout();
			this.SuspendLayout();
			// 
			// tvwLibrary
			// 
			this.tvwLibrary.BackColor = System.Drawing.Color.Black;
			this.tvwLibrary.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tvwLibrary.ContextMenuStrip = this.contextMenuStrip1;
			this.tvwLibrary.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvwLibrary.ForeColor = System.Drawing.Color.Red;
			this.tvwLibrary.FullRowSelect = true;
			this.tvwLibrary.HideSelection = false;
			this.tvwLibrary.ImageKey = "Music";
			this.tvwLibrary.ImageList = this.imlLibrary;
			this.tvwLibrary.LineColor = System.Drawing.Color.Red;
			this.tvwLibrary.Location = new System.Drawing.Point(0, 0);
			this.tvwLibrary.Name = "tvwLibrary";
			this.tvwLibrary.SelectedImageKey = "Music";
			this.tvwLibrary.Size = new System.Drawing.Size(220, 377);
			this.tvwLibrary.TabIndex = 0;
			this.tvwLibrary.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.tvwLibrary.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
			this.tvwLibrary.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tvwLibrary_AfterExpand);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.queueToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(110, 26);
			// 
			// queueToolStripMenuItem
			// 
			this.queueToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("queueToolStripMenuItem.Image")));
			this.queueToolStripMenuItem.Name = "queueToolStripMenuItem";
			this.queueToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
			this.queueToolStripMenuItem.Text = "Queue";
			this.queueToolStripMenuItem.Click += new System.EventHandler(this.queueToolStripMenuItem_Click);
			// 
			// imlLibrary
			// 
			this.imlLibrary.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlLibrary.ImageStream")));
			this.imlLibrary.TransparentColor = System.Drawing.Color.Transparent;
			this.imlLibrary.Images.SetKeyName(0, "Search");
			this.imlLibrary.Images.SetKeyName(1, "Music");
			this.imlLibrary.Images.SetKeyName(2, "NowPlaying");
			this.imlLibrary.Images.SetKeyName(3, "Playlist");
			this.imlLibrary.Images.SetKeyName(4, "Play");
			this.imlLibrary.Images.SetKeyName(5, "Queue");
			this.imlLibrary.Images.SetKeyName(6, "Played");
			this.imlLibrary.Images.SetKeyName(7, "Radio");
			// 
			// contextMenuStrip2
			// 
			this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playFileToolStripMenuItem,
            this.queueFileToolStripMenuItem,
            this.ignoreUnIgnoreSongToolStripMenuItem,
            this.deleteUnDeleteSongToolStripMenuItem,
            this.mnuQueueAlbum,
            this.openContainingFolderToolStripMenuItem,
            this.downloadAlbumArtToolStripMenuItem,
            this.downloadLyricsToolStripMenuItem,
            this.toolStripSeparator1,
            this.mnuEditTrackInformation,
            this.mnuReadTrackInformation,
            this.toolStripSeparator2,
            this.jumpToToolStripMenuItem,
            this.customSep,
            this.customToolStripMenuItem});
			this.contextMenuStrip2.Name = "contextMenuStrip2";
			this.contextMenuStrip2.Size = new System.Drawing.Size(220, 286);
			this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
			// 
			// playFileToolStripMenuItem
			// 
			this.playFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("playFileToolStripMenuItem.Image")));
			this.playFileToolStripMenuItem.Name = "playFileToolStripMenuItem";
			this.playFileToolStripMenuItem.ShortcutKeyDisplayString = "Enter";
			this.playFileToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.playFileToolStripMenuItem.Text = "Play File";
			this.playFileToolStripMenuItem.Click += new System.EventHandler(this.playFileToolStripMenuItem_Click);
			// 
			// queueFileToolStripMenuItem
			// 
			this.queueFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("queueFileToolStripMenuItem.Image")));
			this.queueFileToolStripMenuItem.Name = "queueFileToolStripMenuItem";
			this.queueFileToolStripMenuItem.RightToLeftAutoMirrorImage = true;
			this.queueFileToolStripMenuItem.ShortcutKeyDisplayString = "Q";
			this.queueFileToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.queueFileToolStripMenuItem.Text = "Queue/Un-Queue Song";
			this.queueFileToolStripMenuItem.Click += new System.EventHandler(this.queueFileToolStripMenuItem_Click);
			// 
			// ignoreUnIgnoreSongToolStripMenuItem
			// 
			this.ignoreUnIgnoreSongToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ignoreUnIgnoreSongToolStripMenuItem.Image")));
			this.ignoreUnIgnoreSongToolStripMenuItem.Name = "ignoreUnIgnoreSongToolStripMenuItem";
			this.ignoreUnIgnoreSongToolStripMenuItem.ShortcutKeyDisplayString = "I";
			this.ignoreUnIgnoreSongToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.ignoreUnIgnoreSongToolStripMenuItem.Text = "Ignore/Un-Ignore Song";
			this.ignoreUnIgnoreSongToolStripMenuItem.Click += new System.EventHandler(this.ignoreUnIgnoreSongToolStripMenuItem_Click);
			// 
			// deleteUnDeleteSongToolStripMenuItem
			// 
			this.deleteUnDeleteSongToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteUnDeleteSongToolStripMenuItem.Image")));
			this.deleteUnDeleteSongToolStripMenuItem.Name = "deleteUnDeleteSongToolStripMenuItem";
			this.deleteUnDeleteSongToolStripMenuItem.ShortcutKeyDisplayString = "Del";
			this.deleteUnDeleteSongToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.deleteUnDeleteSongToolStripMenuItem.Text = "Delete/Un-Delete Song";
			this.deleteUnDeleteSongToolStripMenuItem.Click += new System.EventHandler(this.deleteUnDeleteSongToolStripMenuItem_Click);
			// 
			// mnuQueueAlbum
			// 
			this.mnuQueueAlbum.Image = ((System.Drawing.Image)(resources.GetObject("mnuQueueAlbum.Image")));
			this.mnuQueueAlbum.Name = "mnuQueueAlbum";
			this.mnuQueueAlbum.Size = new System.Drawing.Size(219, 22);
			this.mnuQueueAlbum.Text = "Queue Album";
			this.mnuQueueAlbum.Click += new System.EventHandler(this.mnuQueueAlbum_Click);
			// 
			// openContainingFolderToolStripMenuItem
			// 
			this.openContainingFolderToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openContainingFolderToolStripMenuItem.Image")));
			this.openContainingFolderToolStripMenuItem.Name = "openContainingFolderToolStripMenuItem";
			this.openContainingFolderToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.openContainingFolderToolStripMenuItem.Text = "Open Containing Folder";
			this.openContainingFolderToolStripMenuItem.Click += new System.EventHandler(this.openContainingFolderToolStripMenuItem_Click);
			// 
			// downloadAlbumArtToolStripMenuItem
			// 
			this.downloadAlbumArtToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("downloadAlbumArtToolStripMenuItem.Image")));
			this.downloadAlbumArtToolStripMenuItem.Name = "downloadAlbumArtToolStripMenuItem";
			this.downloadAlbumArtToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.downloadAlbumArtToolStripMenuItem.Text = "Download Album Art";
			this.downloadAlbumArtToolStripMenuItem.Click += new System.EventHandler(this.downloadAlbumArtToolStripMenuItem_Click);
			// 
			// downloadLyricsToolStripMenuItem
			// 
			this.downloadLyricsToolStripMenuItem.Name = "downloadLyricsToolStripMenuItem";
			this.downloadLyricsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.downloadLyricsToolStripMenuItem.Text = "Download Lyrics";
			this.downloadLyricsToolStripMenuItem.Click += new System.EventHandler(this.downloadLyricsToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(216, 6);
			// 
			// mnuEditTrackInformation
			// 
			this.mnuEditTrackInformation.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditTrackInformation.Image")));
			this.mnuEditTrackInformation.Name = "mnuEditTrackInformation";
			this.mnuEditTrackInformation.Size = new System.Drawing.Size(219, 22);
			this.mnuEditTrackInformation.Text = "Edit Track Information";
			this.mnuEditTrackInformation.Click += new System.EventHandler(this.mnuEditTrackInformation_Click);
			// 
			// mnuReadTrackInformation
			// 
			this.mnuReadTrackInformation.Name = "mnuReadTrackInformation";
			this.mnuReadTrackInformation.Size = new System.Drawing.Size(219, 22);
			this.mnuReadTrackInformation.Text = "Read Track Information";
			this.mnuReadTrackInformation.Click += new System.EventHandler(this.mnuReadTrackInformation_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(216, 6);
			// 
			// jumpToToolStripMenuItem
			// 
			this.jumpToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.artistalbumToolStripMenuItem,
            this.albumToolStripMenuItem,
            this.artistToolStripMenuItem,
            this.albumArtistToolStripMenuItem,
            this.genreToolStripMenuItem,
            this.yearToolStripMenuItem});
			this.jumpToToolStripMenuItem.Name = "jumpToToolStripMenuItem";
			this.jumpToToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.jumpToToolStripMenuItem.Text = "Jump To...";
			// 
			// artistalbumToolStripMenuItem
			// 
			this.artistalbumToolStripMenuItem.Name = "artistalbumToolStripMenuItem";
			this.artistalbumToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.artistalbumToolStripMenuItem.Text = "<artist>\\<album>";
			this.artistalbumToolStripMenuItem.Click += new System.EventHandler(this.artistalbumToolStripMenuItem_Click);
			// 
			// albumToolStripMenuItem
			// 
			this.albumToolStripMenuItem.Name = "albumToolStripMenuItem";
			this.albumToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.albumToolStripMenuItem.Text = "<album>";
			this.albumToolStripMenuItem.Click += new System.EventHandler(this.albumToolStripMenuItem_Click);
			// 
			// artistToolStripMenuItem
			// 
			this.artistToolStripMenuItem.Name = "artistToolStripMenuItem";
			this.artistToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.artistToolStripMenuItem.Text = "<artist>";
			this.artistToolStripMenuItem.Click += new System.EventHandler(this.artistToolStripMenuItem_Click);
			// 
			// albumArtistToolStripMenuItem
			// 
			this.albumArtistToolStripMenuItem.Name = "albumArtistToolStripMenuItem";
			this.albumArtistToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.albumArtistToolStripMenuItem.Text = "<albumartist>";
			this.albumArtistToolStripMenuItem.Click += new System.EventHandler(this.albumArtistToolStripMenuItem_Click);
			// 
			// genreToolStripMenuItem
			// 
			this.genreToolStripMenuItem.Name = "genreToolStripMenuItem";
			this.genreToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.genreToolStripMenuItem.Text = "<genre>";
			this.genreToolStripMenuItem.Click += new System.EventHandler(this.genreToolStripMenuItem_Click);
			// 
			// yearToolStripMenuItem
			// 
			this.yearToolStripMenuItem.Name = "yearToolStripMenuItem";
			this.yearToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.yearToolStripMenuItem.Text = "<year>";
			this.yearToolStripMenuItem.Click += new System.EventHandler(this.yearToolStripMenuItem_Click);
			// 
			// customSep
			// 
			this.customSep.Name = "customSep";
			this.customSep.Size = new System.Drawing.Size(216, 6);
			// 
			// customToolStripMenuItem
			// 
			this.customToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("customToolStripMenuItem.Image")));
			this.customToolStripMenuItem.Name = "customToolStripMenuItem";
			this.customToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.customToolStripMenuItem.Text = "<custom>";
			this.customToolStripMenuItem.Click += new System.EventHandler(this.customToolStripMenuItem_Click);
			// 
			// playlistControl
			// 
			this.playlistControl.BackColor = System.Drawing.Color.Black;
			this.playlistControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.playlistControl.Font = new System.Drawing.Font("Tahoma", 9F);
			this.playlistControl.ForeColor = System.Drawing.Color.Red;
			this.playlistControl.Location = new System.Drawing.Point(0, 0);
			this.playlistControl.Name = "playlistControl";
			this.playlistControl.Size = new System.Drawing.Size(517, 377);
			this.playlistControl.TabIndex = 2;
			// 
			// searchControl
			// 
			this.searchControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.searchControl.BackColor = System.Drawing.Color.Black;
			this.searchControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.searchControl.Font = new System.Drawing.Font("Tahoma", 9F);
			this.searchControl.ForeColor = System.Drawing.Color.Red;
			this.searchControl.Location = new System.Drawing.Point(0, 0);
			this.searchControl.Name = "searchControl";
			this.searchControl.Size = new System.Drawing.Size(517, 377);
			this.searchControl.TabIndex = 3;
			this.searchControl.SongQueued += new System.EventHandler(this.searchControl1_SongQueued);
			// 
			// infoControl
			// 
			this.infoControl.BackColor = System.Drawing.Color.Black;
			this.infoControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.infoControl.Font = new System.Drawing.Font("Tahoma", 9F);
			this.infoControl.ForeColor = System.Drawing.Color.Red;
			this.infoControl.Location = new System.Drawing.Point(0, 0);
			this.infoControl.Name = "infoControl";
			this.infoControl.Size = new System.Drawing.Size(517, 377);
			this.infoControl.TabIndex = 1;
			// 
			// splitContainer
			// 
			this.splitContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer.Location = new System.Drawing.Point(2, 20);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.lblLoading);
			this.splitContainer.Panel1.Controls.Add(this.tvwLibrary);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.playlistControl);
			this.splitContainer.Panel2.Controls.Add(this.infoControl);
			this.splitContainer.Panel2.Controls.Add(this.statisticsControl);
			this.splitContainer.Panel2.Controls.Add(this.songListView);
			this.splitContainer.Panel2.Controls.Add(this.searchControl);
			this.splitContainer.Size = new System.Drawing.Size(741, 377);
			this.splitContainer.SplitterDistance = 220;
			this.splitContainer.TabIndex = 5;
			this.splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
			// 
			// lblLoading
			// 
			this.lblLoading.AutoSize = true;
			this.lblLoading.BackColor = System.Drawing.Color.Black;
			this.lblLoading.ForeColor = System.Drawing.Color.Red;
			this.lblLoading.Location = new System.Drawing.Point(8, 8);
			this.lblLoading.Name = "lblLoading";
			this.lblLoading.Size = new System.Drawing.Size(100, 14);
			this.lblLoading.TabIndex = 1;
			this.lblLoading.Text = "Loading Library...";
			// 
			// statisticsControl
			// 
			this.statisticsControl.BackColor = System.Drawing.Color.Black;
			this.statisticsControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.statisticsControl.Font = new System.Drawing.Font("Tahoma", 9F);
			this.statisticsControl.ForeColor = System.Drawing.Color.Red;
			this.statisticsControl.Location = new System.Drawing.Point(0, 0);
			this.statisticsControl.Name = "statisticsControl";
			this.statisticsControl.Size = new System.Drawing.Size(517, 377);
			this.statisticsControl.TabIndex = 1;
			// 
			// songListView
			// 
			this.songListView.AlbumArtSize = 100;
			this.songListView.AutoDoubleClick = true;
			this.songListView.AutoScroll = true;
			this.songListView.BackColor = System.Drawing.Color.Black;
			this.songListView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.songListView.CurrentSongFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.songListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.songListView.FlatMode = false;
			this.songListView.Font = new System.Drawing.Font("Tahoma", 9F);
			this.songListView.ForeColor = System.Drawing.Color.Red;
			this.songListView.HeaderFont = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
			this.songListView.ItemContextMenuStrip = this.contextMenuStrip2;
			this.songListView.Location = new System.Drawing.Point(0, 0);
			this.songListView.Name = "songListView";
			this.songListView.SelectedColor = System.Drawing.Color.DarkGray;
			this.songListView.ShowAlbumArt = true;
			this.songListView.Size = new System.Drawing.Size(517, 377);
			this.songListView.TabIndex = 8;
			// 
			// pctShowTree
			// 
			this.pctShowTree.ContextMenuStrip = this.contextMenuStrip3;
			this.pctShowTree.Image = ((System.Drawing.Image)(resources.GetObject("pctShowTree.Image")));
			this.pctShowTree.Location = new System.Drawing.Point(19, 5);
			this.pctShowTree.Name = "pctShowTree";
			this.pctShowTree.Size = new System.Drawing.Size(10, 10);
			this.pctShowTree.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pctShowTree.TabIndex = 7;
			this.pctShowTree.TabStop = false;
			this.toolTip1.SetToolTip(this.pctShowTree, "Show/Hide Library Tree");
			this.pctShowTree.Click += new System.EventHandler(this.pctShowTree_Click);
			// 
			// contextMenuStrip3
			// 
			this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nowPlayingToolStripMenuItem,
            this.searchLibraryContextMenuItem,
            this.playlistToolStripMenuItem,
            this.topPlayedToolStripMenuItem,
            this.recentlyPlayedToolStripMenuItem,
            this.recentlyAddedToolStripMenuItem,
            this.statisticsToolStripMenuItem,
            this.ignoredTracksToolStripMenuItem,
            this.deletedTracksToolStripMenuItem,
            this.jumpTo2ToolStripMenuItem});
			this.contextMenuStrip3.Name = "contextMenuStrip3";
			this.contextMenuStrip3.Size = new System.Drawing.Size(167, 224);
			this.contextMenuStrip3.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip3_Opening);
			// 
			// nowPlayingToolStripMenuItem
			// 
			this.nowPlayingToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("nowPlayingToolStripMenuItem.Image")));
			this.nowPlayingToolStripMenuItem.Name = "nowPlayingToolStripMenuItem";
			this.nowPlayingToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.nowPlayingToolStripMenuItem.Text = "Now Playing";
			this.nowPlayingToolStripMenuItem.Click += new System.EventHandler(this.nowPlayingToolStripMenuItem_Click);
			// 
			// searchLibraryContextMenuItem
			// 
			this.searchLibraryContextMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("searchLibraryContextMenuItem.Image")));
			this.searchLibraryContextMenuItem.Name = "searchLibraryContextMenuItem";
			this.searchLibraryContextMenuItem.Size = new System.Drawing.Size(166, 22);
			this.searchLibraryContextMenuItem.Text = "Search Library";
			this.searchLibraryContextMenuItem.Click += new System.EventHandler(this.searchLibraryContextMenuItem_Click);
			// 
			// playlistToolStripMenuItem
			// 
			this.playlistToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("playlistToolStripMenuItem.Image")));
			this.playlistToolStripMenuItem.Name = "playlistToolStripMenuItem";
			this.playlistToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.playlistToolStripMenuItem.Text = "Playlist";
			this.playlistToolStripMenuItem.Click += new System.EventHandler(this.playlistToolStripMenuItem_Click);
			// 
			// topPlayedToolStripMenuItem
			// 
			this.topPlayedToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("topPlayedToolStripMenuItem.Image")));
			this.topPlayedToolStripMenuItem.Name = "topPlayedToolStripMenuItem";
			this.topPlayedToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.topPlayedToolStripMenuItem.Text = "   Top Played";
			this.topPlayedToolStripMenuItem.Click += new System.EventHandler(this.topPlayedToolStripMenuItem_Click);
			// 
			// recentlyPlayedToolStripMenuItem
			// 
			this.recentlyPlayedToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("recentlyPlayedToolStripMenuItem.Image")));
			this.recentlyPlayedToolStripMenuItem.Name = "recentlyPlayedToolStripMenuItem";
			this.recentlyPlayedToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.recentlyPlayedToolStripMenuItem.Text = "   Recently Played";
			this.recentlyPlayedToolStripMenuItem.Click += new System.EventHandler(this.recentlyPlayedToolStripMenuItem_Click);
			// 
			// recentlyAddedToolStripMenuItem
			// 
			this.recentlyAddedToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("recentlyAddedToolStripMenuItem.Image")));
			this.recentlyAddedToolStripMenuItem.Name = "recentlyAddedToolStripMenuItem";
			this.recentlyAddedToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.recentlyAddedToolStripMenuItem.Text = "   Recently Added";
			this.recentlyAddedToolStripMenuItem.Click += new System.EventHandler(this.recentlyAddedToolStripMenuItem_Click);
			// 
			// statisticsToolStripMenuItem
			// 
			this.statisticsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("statisticsToolStripMenuItem.Image")));
			this.statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
			this.statisticsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.statisticsToolStripMenuItem.Text = "   Statistics";
			this.statisticsToolStripMenuItem.Click += new System.EventHandler(this.statisticsToolStripMenuItem_Click);
			// 
			// ignoredTracksToolStripMenuItem
			// 
			this.ignoredTracksToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ignoredTracksToolStripMenuItem.Image")));
			this.ignoredTracksToolStripMenuItem.Name = "ignoredTracksToolStripMenuItem";
			this.ignoredTracksToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.ignoredTracksToolStripMenuItem.Text = "   Ignored Tracks";
			this.ignoredTracksToolStripMenuItem.Click += new System.EventHandler(this.ignoredTracksToolStripMenuItem_Click);
			// 
			// deletedTracksToolStripMenuItem
			// 
			this.deletedTracksToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deletedTracksToolStripMenuItem.Image")));
			this.deletedTracksToolStripMenuItem.Name = "deletedTracksToolStripMenuItem";
			this.deletedTracksToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.deletedTracksToolStripMenuItem.Text = "   Deleted Tracks";
			this.deletedTracksToolStripMenuItem.Click += new System.EventHandler(this.deletedTracksToolStripMenuItem_Click);
			// 
			// jumpTo2ToolStripMenuItem
			// 
			this.jumpTo2ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
			this.jumpTo2ToolStripMenuItem.Name = "jumpTo2ToolStripMenuItem";
			this.jumpTo2ToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
			this.jumpTo2ToolStripMenuItem.Text = "Jump To...";
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem2.Text = "<artist>\\<album>";
			this.toolStripMenuItem2.Click += new System.EventHandler(this.artistalbumToolStripMenuItem_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem3.Text = "<album>";
			this.toolStripMenuItem3.Click += new System.EventHandler(this.albumToolStripMenuItem_Click);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem4.Text = "<artist>";
			this.toolStripMenuItem4.Click += new System.EventHandler(this.artistToolStripMenuItem_Click);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem5.Text = "<albumartist>";
			this.toolStripMenuItem5.Click += new System.EventHandler(this.albumArtistToolStripMenuItem_Click);
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem6.Text = "<genre>";
			this.toolStripMenuItem6.Click += new System.EventHandler(this.genreToolStripMenuItem_Click);
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem7.Text = "<year>";
			this.toolStripMenuItem7.Click += new System.EventHandler(this.yearToolStripMenuItem_Click);
			// 
			// LibraryForm
			// 
			this.Caption = "Library";
			this.ClientSize = new System.Drawing.Size(746, 400);
			this.Controls.Add(this.pctShowTree);
			this.Controls.Add(this.splitContainer);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.InternalBorderColor = System.Drawing.SystemColors.ActiveBorder;
			this.InternalBorderSize = 2;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(100, 19);
			this.Name = "LibraryForm";
			this.ShowInTaskbar = false;
			this.Sizable = true;
			this.contextMenuStrip1.ResumeLayout(false);
			this.contextMenuStrip2.ResumeLayout(false);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel1.PerformLayout();
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctShowTree)).EndInit();
			this.contextMenuStrip3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView tvwLibrary;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
		private System.Windows.Forms.ToolStripMenuItem playFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem queueFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem queueToolStripMenuItem;
		private PlaylistControl playlistControl;
		private SearchControl searchControl;
		private InfoControl infoControl;
		private System.Windows.Forms.ToolStripMenuItem openContainingFolderToolStripMenuItem;
		private System.Windows.Forms.ImageList imlLibrary;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.ToolStripMenuItem downloadAlbumArtToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnuQueueAlbum;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.PictureBox pctShowTree;
		private System.Windows.Forms.ToolStripMenuItem mnuEditTrackInformation;
		private SongListView songListView;
        private StatisticsControl statisticsControl;
		private System.Windows.Forms.ToolStripMenuItem jumpToToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem artistalbumToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem albumToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem artistToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem genreToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem yearToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem albumArtistToolStripMenuItem;
		private System.Windows.Forms.Label lblLoading;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
		private System.Windows.Forms.ToolStripMenuItem nowPlayingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem playlistToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem topPlayedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recentlyPlayedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recentlyAddedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem statisticsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem jumpTo2ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
		private System.Windows.Forms.ToolStripMenuItem searchLibraryContextMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem mnuReadTrackInformation;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem ignoreUnIgnoreSongToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ignoredTracksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteUnDeleteSongToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deletedTracksToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator customSep;
		private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem downloadLyricsToolStripMenuItem;
	}
}
