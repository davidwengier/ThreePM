namespace ThreePM
{
	partial class AlbumListForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlbumListForm));
			this.albumPanel1 = new ThreePM.AlbumPanel();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.playAndQueueAlbumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.queueAlbumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// albumPanel1
			// 
			this.albumPanel1.AlbumContextMenuStrip = this.contextMenuStrip1;
			this.albumPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.albumPanel1.Location = new System.Drawing.Point(2, 20);
			this.albumPanel1.Name = "albumPanel1";
			this.albumPanel1.Size = new System.Drawing.Size(528, 143);
			this.albumPanel1.TabIndex = 0;
			this.albumPanel1.Text = "albumPanel1";
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playAndQueueAlbumToolStripMenuItem,
            this.queueAlbumToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(194, 48);
			// 
			// playAndQueueAlbumToolStripMenuItem
			// 
			this.playAndQueueAlbumToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("playAndQueueAlbumToolStripMenuItem.Image")));
			this.playAndQueueAlbumToolStripMenuItem.Name = "playAndQueueAlbumToolStripMenuItem";
			this.playAndQueueAlbumToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.playAndQueueAlbumToolStripMenuItem.Text = "Play and Queue Album";
			this.playAndQueueAlbumToolStripMenuItem.Click += new System.EventHandler(this.playAndQueueAlbumToolStripMenuItem_Click);
			// 
			// queueAlbumToolStripMenuItem
			// 
			this.queueAlbumToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("queueAlbumToolStripMenuItem.Image")));
			this.queueAlbumToolStripMenuItem.Name = "queueAlbumToolStripMenuItem";
			this.queueAlbumToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.queueAlbumToolStripMenuItem.Text = "Queue Album";
			this.queueAlbumToolStripMenuItem.Click += new System.EventHandler(this.queueAlbumToolStripMenuItem_Click);
			// 
			// AlbumListForm
			// 
			this.Caption = "Album List";
			this.ClientSize = new System.Drawing.Size(533, 166);
			this.Controls.Add(this.albumPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.InternalBorderColor = System.Drawing.Color.Silver;
			this.InternalBorderSize = 2;
			this.MinimizeBox = false;
			this.Name = "AlbumListForm";
			this.ShowInTaskbar = false;
			this.Sizable = true;
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private AlbumPanel albumPanel1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem playAndQueueAlbumToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem queueAlbumToolStripMenuItem;
	}
}
