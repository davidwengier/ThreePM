using System.Windows.Forms;
namespace ThreePM
{
    partial class AlbumArtForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlbumArtForm));
            this.albumArtBox1 = new ThreePM.AlbumArtBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.downloadAlbumArtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openContainingFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.albumArtBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // albumArtBox1
            // 
            this.albumArtBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.albumArtBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.albumArtBox1.Location = new System.Drawing.Point(0, 19);
            this.albumArtBox1.Name = "albumArtBox1";
            this.albumArtBox1.Size = new System.Drawing.Size(199, 199);
            this.albumArtBox1.TabIndex = 13;
            this.albumArtBox1.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadAlbumArtToolStripMenuItem,
            this.openContainingFolderToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(199, 48);
            // 
            // downloadAlbumArtToolStripMenuItem
            // 
            this.downloadAlbumArtToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("downloadAlbumArtToolStripMenuItem.Image")));
            this.downloadAlbumArtToolStripMenuItem.Name = "downloadAlbumArtToolStripMenuItem";
            this.downloadAlbumArtToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.downloadAlbumArtToolStripMenuItem.Text = "Download Album Art";
            this.downloadAlbumArtToolStripMenuItem.Click += new System.EventHandler(this.downloadAlbumArtToolStripMenuItem_Click);
            // 
            // openContainingFolderToolStripMenuItem
            // 
            this.openContainingFolderToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openContainingFolderToolStripMenuItem.Image")));
            this.openContainingFolderToolStripMenuItem.Name = "openContainingFolderToolStripMenuItem";
            this.openContainingFolderToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.openContainingFolderToolStripMenuItem.Text = "Open Containing Folder";
            this.openContainingFolderToolStripMenuItem.Click += new System.EventHandler(this.openContainingFolderToolStripMenuItem_Click);
            // 
            // AlbumArtForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Caption = "Album Art";
            this.ClientSize = new System.Drawing.Size(200, 219);
            this.Controls.Add(this.albumArtBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.InternalBorderAtTop = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AlbumArtForm";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.albumArtBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        private AlbumArtBox albumArtBox1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem downloadAlbumArtToolStripMenuItem;
        private ToolStripMenuItem openContainingFolderToolStripMenuItem;
    }
}
