namespace ThreePM
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.btnPrevious = new System.Windows.Forms.Button();
			this.btnNext = new System.Windows.Forms.Button();
			this.lblPosition = new System.Windows.Forms.Label();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.systemInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.enableHttpServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.alwaysOnTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ignoreAndNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.watchFoldersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.songSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.playlistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.trackInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showAlbumArtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showAlbumListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showLyricsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showEqualizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.queueAlbumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tckBalance = new ThreePM.UI.Ticker();
			this.tckVolume = new ThreePM.UI.Ticker();
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblSongCount = new System.Windows.Forms.Label();
			this.prgSpin = new ThreePM.UI.ProgressCircle();
			this.tckPosition = new ThreePM.UI.Ticker();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.chkOnTop = new System.Windows.Forms.CheckBox();
			this.chkPlaylist = new System.Windows.Forms.CheckBox();
			this.chkInformation = new System.Windows.Forms.CheckBox();
			this.chkRepeat = new System.Windows.Forms.CheckBox();
			this.btnPlay = new System.Windows.Forms.CheckBox();
			this.btnPause = new System.Windows.Forms.CheckBox();
			this.btnStop = new System.Windows.Forms.CheckBox();
			this.chkLibrary = new System.Windows.Forms.CheckBox();
			this.btnIgnore = new System.Windows.Forms.CheckBox();
			this.lblArtist = new System.Windows.Forms.Label();
			this.lblStatus = new System.Windows.Forms.Label();
			this.tmrStatus = new System.Windows.Forms.Timer(this.components);
			this.tmrSpectrum = new System.Windows.Forms.Timer(this.components);
			this.pctSpectrum = new System.Windows.Forms.PictureBox();
			this.ControlsPanel = new System.Windows.Forms.Panel();
			this.contextMenuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctSpectrum)).BeginInit();
			this.ControlsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnPrevious
			// 
			this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPrevious.AutoSize = true;
			this.btnPrevious.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnPrevious.FlatAppearance.BorderSize = 0;
			this.btnPrevious.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnPrevious.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnPrevious.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPrevious.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevious.Image")));
			this.btnPrevious.Location = new System.Drawing.Point(3, 94);
			this.btnPrevious.Name = "btnPrevious";
			this.btnPrevious.Size = new System.Drawing.Size(22, 22);
			this.btnPrevious.TabIndex = 2;
			this.toolTip1.SetToolTip(this.btnPrevious, "Previous");
			this.btnPrevious.UseVisualStyleBackColor = false;
			this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
			// 
			// btnNext
			// 
			this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnNext.AutoSize = true;
			this.btnNext.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.btnNext.FlatAppearance.BorderSize = 0;
			this.btnNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnNext.Image = ((System.Drawing.Image)(resources.GetObject("btnNext.Image")));
			this.btnNext.Location = new System.Drawing.Point(111, 94);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(22, 22);
			this.btnNext.TabIndex = 6;
			this.toolTip1.SetToolTip(this.btnNext, "Next");
			this.btnNext.UseVisualStyleBackColor = false;
			this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
			// 
			// lblPosition
			// 
			this.lblPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblPosition.BackColor = System.Drawing.Color.Black;
			this.lblPosition.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPosition.ForeColor = System.Drawing.Color.Red;
			this.lblPosition.Location = new System.Drawing.Point(319, -1);
			this.lblPosition.Name = "lblPosition";
			this.lblPosition.Size = new System.Drawing.Size(64, 23);
			this.lblPosition.TabIndex = 1;
			this.lblPosition.Text = "-00:00";
			this.lblPosition.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.toolTip1.SetToolTip(this.lblPosition, "Time Elapsed");
			this.lblPosition.UseMnemonic = false;
			this.lblPosition.Click += new System.EventHandler(this.lblPosition_Click);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.systemInfoToolStripMenuItem,
            this.enableHttpServerToolStripMenuItem,
            this.toolStripSeparator2,
            this.alwaysOnTopToolStripMenuItem,
            this.ignoreAndNextToolStripMenuItem,
            this.watchFoldersToolStripMenuItem,
            this.toolStripSeparator4,
            this.songSearchToolStripMenuItem,
            this.toolStripSeparator1,
            this.playlistToolStripMenuItem,
            this.trackInformationToolStripMenuItem,
            this.showAlbumArtToolStripMenuItem,
            this.showAlbumListToolStripMenuItem,
            this.showLibraryToolStripMenuItem,
            this.showLyricsToolStripMenuItem,
            this.showEqualizerToolStripMenuItem,
            this.toolStripSeparator3,
            this.queueAlbumToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(269, 358);
			// 
			// systemInfoToolStripMenuItem
			// 
			this.systemInfoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("systemInfoToolStripMenuItem.Image")));
			this.systemInfoToolStripMenuItem.Name = "systemInfoToolStripMenuItem";
			this.systemInfoToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.systemInfoToolStripMenuItem.Text = "System Info...";
			this.systemInfoToolStripMenuItem.Click += new System.EventHandler(this.systemInfoToolStripMenuItem_Click);
			// 
			// enableHttpServerToolStripMenuItem
			// 
			this.enableHttpServerToolStripMenuItem.CheckOnClick = true;
			this.enableHttpServerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("enableHttpServerToolStripMenuItem.Image")));
			this.enableHttpServerToolStripMenuItem.Name = "enableHttpServerToolStripMenuItem";
			this.enableHttpServerToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.enableHttpServerToolStripMenuItem.Text = "Enable HTTP Server";
			this.enableHttpServerToolStripMenuItem.Click += new System.EventHandler(this.enableHttpServerToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(265, 6);
			// 
			// alwaysOnTopToolStripMenuItem
			// 
			this.alwaysOnTopToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("alwaysOnTopToolStripMenuItem.Image")));
			this.alwaysOnTopToolStripMenuItem.Name = "alwaysOnTopToolStripMenuItem";
			this.alwaysOnTopToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.alwaysOnTopToolStripMenuItem.Text = "Always On Top";
			this.alwaysOnTopToolStripMenuItem.Click += new System.EventHandler(this.alwaysOnTopToolStripMenuItem_Click);
			// 
			// ignoreAndNextToolStripMenuItem
			// 
			this.ignoreAndNextToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ignoreAndNextToolStripMenuItem.Image")));
			this.ignoreAndNextToolStripMenuItem.Name = "ignoreAndNextToolStripMenuItem";
			this.ignoreAndNextToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl + Alt + Shift + I";
			this.ignoreAndNextToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.ignoreAndNextToolStripMenuItem.Text = "Ignore and Next";
			this.ignoreAndNextToolStripMenuItem.Click += new System.EventHandler(this.ignoreAndNextToolStripMenuItem_Click);
			// 
			// watchFoldersToolStripMenuItem
			// 
			this.watchFoldersToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("watchFoldersToolStripMenuItem.Image")));
			this.watchFoldersToolStripMenuItem.Name = "watchFoldersToolStripMenuItem";
			this.watchFoldersToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.watchFoldersToolStripMenuItem.Text = "Options";
			this.watchFoldersToolStripMenuItem.Click += new System.EventHandler(this.watchFoldersToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(265, 6);
			// 
			// songSearchToolStripMenuItem
			// 
			this.songSearchToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("songSearchToolStripMenuItem.Image")));
			this.songSearchToolStripMenuItem.Name = "songSearchToolStripMenuItem";
			this.songSearchToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.songSearchToolStripMenuItem.Text = "Song Search";
			this.songSearchToolStripMenuItem.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(265, 6);
			// 
			// playlistToolStripMenuItem
			// 
			this.playlistToolStripMenuItem.CheckOnClick = true;
			this.playlistToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("playlistToolStripMenuItem.Image")));
			this.playlistToolStripMenuItem.Name = "playlistToolStripMenuItem";
			this.playlistToolStripMenuItem.ShortcutKeyDisplayString = "";
			this.playlistToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.playlistToolStripMenuItem.Text = "Show Playlist";
			this.playlistToolStripMenuItem.CheckedChanged += new System.EventHandler(this.playlistToolStripMenuItem_CheckedChanged);
			this.playlistToolStripMenuItem.Click += new System.EventHandler(this.playlistToolStripMenuItem_Click);
			// 
			// trackInformationToolStripMenuItem
			// 
			this.trackInformationToolStripMenuItem.CheckOnClick = true;
			this.trackInformationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("trackInformationToolStripMenuItem.Image")));
			this.trackInformationToolStripMenuItem.Name = "trackInformationToolStripMenuItem";
			this.trackInformationToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.trackInformationToolStripMenuItem.Text = "Show Track Information";
			this.trackInformationToolStripMenuItem.CheckedChanged += new System.EventHandler(this.trackInformationToolStripMenuItem_CheckedChanged);
			this.trackInformationToolStripMenuItem.Click += new System.EventHandler(this.trackInformationToolStripMenuItem_Click);
			// 
			// showAlbumArtToolStripMenuItem
			// 
			this.showAlbumArtToolStripMenuItem.CheckOnClick = true;
			this.showAlbumArtToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showAlbumArtToolStripMenuItem.Image")));
			this.showAlbumArtToolStripMenuItem.Name = "showAlbumArtToolStripMenuItem";
			this.showAlbumArtToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.showAlbumArtToolStripMenuItem.Text = "Show Album Art";
			this.showAlbumArtToolStripMenuItem.Click += new System.EventHandler(this.showAlbumArtToolStripMenuItem_Click);
			// 
			// showAlbumListToolStripMenuItem
			// 
			this.showAlbumListToolStripMenuItem.CheckOnClick = true;
			this.showAlbumListToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showAlbumListToolStripMenuItem.Image")));
			this.showAlbumListToolStripMenuItem.Name = "showAlbumListToolStripMenuItem";
			this.showAlbumListToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.showAlbumListToolStripMenuItem.Text = "Show Album List";
			this.showAlbumListToolStripMenuItem.Click += new System.EventHandler(this.showAlbumListToolStripMenuItem_Click);
			// 
			// showLibraryToolStripMenuItem
			// 
			this.showLibraryToolStripMenuItem.CheckOnClick = true;
			this.showLibraryToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showLibraryToolStripMenuItem.Image")));
			this.showLibraryToolStripMenuItem.Name = "showLibraryToolStripMenuItem";
			this.showLibraryToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.showLibraryToolStripMenuItem.Text = "Show Library";
			this.showLibraryToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showLibraryToolStripMenuItem_CheckedChanged);
			this.showLibraryToolStripMenuItem.Click += new System.EventHandler(this.showLibraryToolStripMenuItem_Click);
			// 
			// showLyricsToolStripMenuItem
			// 
			this.showLyricsToolStripMenuItem.CheckOnClick = true;
			this.showLyricsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showLyricsToolStripMenuItem.Image")));
			this.showLyricsToolStripMenuItem.Name = "showLyricsToolStripMenuItem";
			this.showLyricsToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.showLyricsToolStripMenuItem.Text = "Show Lyrics";
			this.showLyricsToolStripMenuItem.Click += new System.EventHandler(this.showLyricsToolStripMenuItem_Click);
			// 
			// showEqualizerToolStripMenuItem
			// 
			this.showEqualizerToolStripMenuItem.CheckOnClick = true;
			this.showEqualizerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showEqualizerToolStripMenuItem.Image")));
			this.showEqualizerToolStripMenuItem.Name = "showEqualizerToolStripMenuItem";
			this.showEqualizerToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.showEqualizerToolStripMenuItem.Text = "Show Equalizer";
			this.showEqualizerToolStripMenuItem.Click += new System.EventHandler(this.showEqualizerToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(265, 6);
			// 
			// queueAlbumToolStripMenuItem
			// 
			this.queueAlbumToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("queueAlbumToolStripMenuItem.Image")));
			this.queueAlbumToolStripMenuItem.Name = "queueAlbumToolStripMenuItem";
			this.queueAlbumToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			this.queueAlbumToolStripMenuItem.Text = "Queue This Album";
			this.queueAlbumToolStripMenuItem.Click += new System.EventHandler(this.queueAlbumToolStripMenuItem_Click);
			// 
			// tckBalance
			// 
			this.tckBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tckBalance.Duration = 0;
			this.tckBalance.FireWhileSliding = true;
			this.tckBalance.ForeColor = System.Drawing.Color.Green;
			this.tckBalance.Location = new System.Drawing.Point(282, 120);
			this.tckBalance.Name = "tckBalance";
			this.tckBalance.Position = 0;
			this.tckBalance.Size = new System.Drawing.Size(50, 10);
			this.tckBalance.TabIndex = 12;
			this.tckBalance.Text = "ticker2";
			this.tckBalance.PositionChanged += new System.EventHandler(this.tckBalance_PositionChanged);
			// 
			// tckVolume
			// 
			this.tckVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tckVolume.Duration = 0;
			this.tckVolume.FireWhileSliding = true;
			this.tckVolume.ForeColor = System.Drawing.Color.Blue;
			this.tckVolume.Location = new System.Drawing.Point(333, 120);
			this.tckVolume.Name = "tckVolume";
			this.tckVolume.Position = 0;
			this.tckVolume.Size = new System.Drawing.Size(54, 10);
			this.tckVolume.TabIndex = 13;
			this.tckVolume.PositionChanged += new System.EventHandler(this.tckVolume_PositionChanged);
			// 
			// lblTitle
			// 
			this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblTitle.AutoEllipsis = true;
			this.lblTitle.BackColor = System.Drawing.Color.Black;
			this.lblTitle.ForeColor = System.Drawing.Color.Red;
			this.lblTitle.Location = new System.Drawing.Point(0, 2);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(314, 14);
			this.lblTitle.TabIndex = 3;
			this.lblTitle.UseMnemonic = false;
			// 
			// lblSongCount
			// 
			this.lblSongCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSongCount.AutoEllipsis = true;
			this.lblSongCount.ForeColor = System.Drawing.Color.Red;
			this.lblSongCount.Location = new System.Drawing.Point(261, 94);
			this.lblSongCount.Name = "lblSongCount";
			this.lblSongCount.Size = new System.Drawing.Size(91, 22);
			this.lblSongCount.TabIndex = 3;
			this.lblSongCount.Text = "100000 songs";
			this.lblSongCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lblSongCount.UseMnemonic = false;
			// 
			// prgSpin
			// 
			this.prgSpin.ActiveSegmentColour = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(146)))), ((int)(((byte)(33)))));
			this.prgSpin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.prgSpin.InactiveSegmentColour = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.prgSpin.Location = new System.Drawing.Point(358, 94);
			this.prgSpin.Name = "prgSpin";
			this.prgSpin.Size = new System.Drawing.Size(22, 22);
			this.prgSpin.TabIndex = 3;
			this.prgSpin.TransistionSegmentColour = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(242)))), ((int)(((byte)(121)))));
			this.prgSpin.Value = -1;
			// 
			// tckPosition
			// 
			this.tckPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tckPosition.Duration = 0;
			this.tckPosition.FireWhileSliding = false;
			this.tckPosition.ForeColor = System.Drawing.Color.Red;
			this.tckPosition.Location = new System.Drawing.Point(0, 120);
			this.tckPosition.Name = "tckPosition";
			this.tckPosition.Position = 0;
			this.tckPosition.Size = new System.Drawing.Size(281, 10);
			this.tckPosition.TabIndex = 11;
			this.tckPosition.Text = "ticker1";
			this.tckPosition.PositionChanging += new System.EventHandler(this.tckPosition_PositionChanging);
			this.tckPosition.PositionChanged += new System.EventHandler(this.tckPosition_PositionChanged);
			// 
			// chkOnTop
			// 
			this.chkOnTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.chkOnTop.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkOnTop.AutoSize = true;
			this.chkOnTop.Checked = true;
			this.chkOnTop.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkOnTop.FlatAppearance.BorderSize = 0;
			this.chkOnTop.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.chkOnTop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.chkOnTop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.chkOnTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkOnTop.Image = ((System.Drawing.Image)(resources.GetObject("chkOnTop.Image")));
			this.chkOnTop.Location = new System.Drawing.Point(358, 29);
			this.chkOnTop.Name = "chkOnTop";
			this.chkOnTop.Size = new System.Drawing.Size(22, 22);
			this.chkOnTop.TabIndex = 0;
			this.toolTip1.SetToolTip(this.chkOnTop, "Always on Top");
			this.chkOnTop.UseVisualStyleBackColor = true;
			this.chkOnTop.Click += new System.EventHandler(this.chkOnTop_Click);
			// 
			// chkPlaylist
			// 
			this.chkPlaylist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkPlaylist.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkPlaylist.AutoCheck = false;
			this.chkPlaylist.AutoSize = true;
			this.chkPlaylist.FlatAppearance.BorderSize = 0;
			this.chkPlaylist.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.chkPlaylist.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.chkPlaylist.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.chkPlaylist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkPlaylist.Image = ((System.Drawing.Image)(resources.GetObject("chkPlaylist.Image")));
			this.chkPlaylist.Location = new System.Drawing.Point(179, 94);
			this.chkPlaylist.Name = "chkPlaylist";
			this.chkPlaylist.Size = new System.Drawing.Size(22, 22);
			this.chkPlaylist.TabIndex = 8;
			this.toolTip1.SetToolTip(this.chkPlaylist, "Show Playlist");
			this.chkPlaylist.UseVisualStyleBackColor = true;
			this.chkPlaylist.Click += new System.EventHandler(this.chkPlaylist_Click);
			// 
			// chkInformation
			// 
			this.chkInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkInformation.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkInformation.AutoCheck = false;
			this.chkInformation.AutoSize = true;
			this.chkInformation.FlatAppearance.BorderSize = 0;
			this.chkInformation.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.chkInformation.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.chkInformation.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.chkInformation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkInformation.Image = ((System.Drawing.Image)(resources.GetObject("chkInformation.Image")));
			this.chkInformation.Location = new System.Drawing.Point(205, 94);
			this.chkInformation.Name = "chkInformation";
			this.chkInformation.Size = new System.Drawing.Size(22, 22);
			this.chkInformation.TabIndex = 9;
			this.toolTip1.SetToolTip(this.chkInformation, "Show Track Information");
			this.chkInformation.UseVisualStyleBackColor = true;
			this.chkInformation.Click += new System.EventHandler(this.chkInformation_Click);
			// 
			// chkRepeat
			// 
			this.chkRepeat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkRepeat.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkRepeat.AutoSize = true;
			this.chkRepeat.FlatAppearance.BorderSize = 0;
			this.chkRepeat.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.chkRepeat.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.chkRepeat.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.chkRepeat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkRepeat.Image = ((System.Drawing.Image)(resources.GetObject("chkRepeat.Image")));
			this.chkRepeat.Location = new System.Drawing.Point(151, 94);
			this.chkRepeat.Name = "chkRepeat";
			this.chkRepeat.Size = new System.Drawing.Size(22, 22);
			this.chkRepeat.TabIndex = 7;
			this.toolTip1.SetToolTip(this.chkRepeat, "Repeat Mode");
			this.chkRepeat.UseVisualStyleBackColor = true;
			this.chkRepeat.Click += new System.EventHandler(this.chkRepeat_Click);
			// 
			// btnPlay
			// 
			this.btnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPlay.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnPlay.AutoCheck = false;
			this.btnPlay.AutoSize = true;
			this.btnPlay.FlatAppearance.BorderSize = 0;
			this.btnPlay.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPlay.Image = ((System.Drawing.Image)(resources.GetObject("btnPlay.Image")));
			this.btnPlay.Location = new System.Drawing.Point(29, 94);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(22, 22);
			this.btnPlay.TabIndex = 3;
			this.toolTip1.SetToolTip(this.btnPlay, "Play");
			this.btnPlay.UseVisualStyleBackColor = true;
			this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
			// 
			// btnPause
			// 
			this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPause.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnPause.AutoCheck = false;
			this.btnPause.AutoSize = true;
			this.btnPause.FlatAppearance.BorderSize = 0;
			this.btnPause.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnPause.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnPause.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPause.Image = ((System.Drawing.Image)(resources.GetObject("btnPause.Image")));
			this.btnPause.Location = new System.Drawing.Point(56, 94);
			this.btnPause.Name = "btnPause";
			this.btnPause.Size = new System.Drawing.Size(22, 22);
			this.btnPause.TabIndex = 4;
			this.toolTip1.SetToolTip(this.btnPause, "Pause");
			this.btnPause.UseVisualStyleBackColor = true;
			this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
			// 
			// btnStop
			// 
			this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnStop.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnStop.AutoCheck = false;
			this.btnStop.AutoSize = true;
			this.btnStop.FlatAppearance.BorderSize = 0;
			this.btnStop.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
			this.btnStop.Location = new System.Drawing.Point(84, 94);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(22, 22);
			this.btnStop.TabIndex = 5;
			this.toolTip1.SetToolTip(this.btnStop, "Stop");
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// chkLibrary
			// 
			this.chkLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.chkLibrary.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkLibrary.AutoCheck = false;
			this.chkLibrary.AutoSize = true;
			this.chkLibrary.FlatAppearance.BorderSize = 0;
			this.chkLibrary.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.chkLibrary.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.chkLibrary.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.chkLibrary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkLibrary.Image = ((System.Drawing.Image)(resources.GetObject("chkLibrary.Image")));
			this.chkLibrary.Location = new System.Drawing.Point(233, 94);
			this.chkLibrary.Name = "chkLibrary";
			this.chkLibrary.Size = new System.Drawing.Size(22, 22);
			this.chkLibrary.TabIndex = 10;
			this.toolTip1.SetToolTip(this.chkLibrary, "Show Library");
			this.chkLibrary.UseVisualStyleBackColor = true;
			this.chkLibrary.Click += new System.EventHandler(this.chkLibrary_Click);
			// 
			// btnIgnore
			// 
			this.btnIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnIgnore.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnIgnore.AutoCheck = false;
			this.btnIgnore.AutoSize = true;
			this.btnIgnore.FlatAppearance.BorderSize = 0;
			this.btnIgnore.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnIgnore.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnIgnore.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnIgnore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnIgnore.Image = ((System.Drawing.Image)(resources.GetObject("btnIgnore.Image")));
			this.btnIgnore.Location = new System.Drawing.Point(358, 57);
			this.btnIgnore.Name = "btnIgnore";
			this.btnIgnore.Size = new System.Drawing.Size(22, 22);
			this.btnIgnore.TabIndex = 1;
			this.toolTip1.SetToolTip(this.btnIgnore, "Ignore Artist");
			this.btnIgnore.UseVisualStyleBackColor = true;
			this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
			// 
			// lblArtist
			// 
			this.lblArtist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblArtist.AutoEllipsis = true;
			this.lblArtist.BackColor = System.Drawing.Color.Black;
			this.lblArtist.ForeColor = System.Drawing.Color.Red;
			this.lblArtist.Location = new System.Drawing.Point(0, 18);
			this.lblArtist.Name = "lblArtist";
			this.lblArtist.Size = new System.Drawing.Size(314, 14);
			this.lblArtist.TabIndex = 3;
			this.lblArtist.UseMnemonic = false;
			// 
			// lblStatus
			// 
			this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblStatus.AutoEllipsis = true;
			this.lblStatus.BackColor = System.Drawing.Color.Black;
			this.lblStatus.ForeColor = System.Drawing.Color.Red;
			this.lblStatus.Location = new System.Drawing.Point(0, 68);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(353, 14);
			this.lblStatus.TabIndex = 3;
			this.lblStatus.UseMnemonic = false;
			// 
			// tmrStatus
			// 
			this.tmrStatus.Interval = 1000;
			this.tmrStatus.Tick += new System.EventHandler(this.tmStatus_Tick);
			// 
			// tmrSpectrum
			// 
			this.tmrSpectrum.Interval = 50;
			this.tmrSpectrum.Tick += new System.EventHandler(this.tmrSpectrum_Tick);
			// 
			// pctSpectrum
			// 
			this.pctSpectrum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.pctSpectrum.Location = new System.Drawing.Point(195, 32);
			this.pctSpectrum.Name = "pctSpectrum";
			this.pctSpectrum.Size = new System.Drawing.Size(157, 59);
			this.pctSpectrum.TabIndex = 9;
			this.pctSpectrum.TabStop = false;
			this.pctSpectrum.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pctSpectrum_MouseUp);
			// 
			// ControlsPanel
			// 
			this.ControlsPanel.Controls.Add(this.tckPosition);
			this.ControlsPanel.Controls.Add(this.lblTitle);
			this.ControlsPanel.Controls.Add(this.lblArtist);
			this.ControlsPanel.Controls.Add(this.pctSpectrum);
			this.ControlsPanel.Controls.Add(this.lblStatus);
			this.ControlsPanel.Controls.Add(this.lblPosition);
			this.ControlsPanel.Controls.Add(this.btnIgnore);
			this.ControlsPanel.Controls.Add(this.tckVolume);
			this.ControlsPanel.Controls.Add(this.tckBalance);
			this.ControlsPanel.Controls.Add(this.chkOnTop);
			this.ControlsPanel.Controls.Add(this.btnNext);
			this.ControlsPanel.Controls.Add(this.btnPlay);
			this.ControlsPanel.Controls.Add(this.btnPause);
			this.ControlsPanel.Controls.Add(this.chkPlaylist);
			this.ControlsPanel.Controls.Add(this.btnStop);
			this.ControlsPanel.Controls.Add(this.btnPrevious);
			this.ControlsPanel.Controls.Add(this.lblSongCount);
			this.ControlsPanel.Controls.Add(this.chkRepeat);
			this.ControlsPanel.Controls.Add(this.chkInformation);
			this.ControlsPanel.Controls.Add(this.prgSpin);
			this.ControlsPanel.Controls.Add(this.chkLibrary);
			this.ControlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ControlsPanel.Location = new System.Drawing.Point(0, 19);
			this.ControlsPanel.Name = "ControlsPanel";
			this.ControlsPanel.Size = new System.Drawing.Size(385, 132);
			this.ControlsPanel.TabIndex = 14;
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(386, 152);
			this.ContextMenuStrip = this.contextMenuStrip1;
			this.Controls.Add(this.ControlsPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(100, 19);
			this.Name = "MainForm";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPlayer_FormClosing);
			this.contextMenuStrip1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctSpectrum)).EndInit();
			this.ControlsPanel.ResumeLayout(false);
			this.ControlsPanel.PerformLayout();
			this.ResumeLayout(false);

		}
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem enableHttpServerToolStripMenuItem;

		private System.Windows.Forms.Timer tmrStatus;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.CheckBox chkRepeat;

		private System.Windows.Forms.ToolStripMenuItem watchFoldersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem songSearchToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem trackInformationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem playlistToolStripMenuItem;

		private System.Windows.Forms.Label lblTitle;
		private ThreePM.UI.ProgressCircle prgSpin;
		private ThreePM.UI.Ticker tckPosition;
		private System.Windows.Forms.Label lblPosition;
		private System.Windows.Forms.Button btnPrevious;
		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.Label lblSongCount;
		private ThreePM.UI.Ticker tckVolume;
		private ThreePM.UI.Ticker tckBalance;

		#endregion
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem systemInfoToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.CheckBox chkOnTop;
		private System.Windows.Forms.Label lblArtist;
		private System.Windows.Forms.CheckBox chkPlaylist;
		private System.Windows.Forms.CheckBox chkInformation;
		private System.Windows.Forms.ToolStripMenuItem showAlbumArtToolStripMenuItem;
		private System.Windows.Forms.CheckBox btnPlay;
		private System.Windows.Forms.CheckBox btnPause;
		private System.Windows.Forms.CheckBox btnStop;
		private System.Windows.Forms.ToolStripMenuItem showLibraryToolStripMenuItem;
		private System.Windows.Forms.CheckBox chkLibrary;
		private System.Windows.Forms.CheckBox btnIgnore;
		private System.Windows.Forms.Timer tmrSpectrum;
		private System.Windows.Forms.PictureBox pctSpectrum;
		private System.Windows.Forms.ToolStripMenuItem queueAlbumToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showAlbumListToolStripMenuItem;
		private System.Windows.Forms.Panel ControlsPanel;
		private System.Windows.Forms.ToolStripMenuItem alwaysOnTopToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ignoreAndNextToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem showLyricsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem showEqualizerToolStripMenuItem;

	}
}

