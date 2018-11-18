namespace ThreePM
{
	partial class LyricsForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LyricsForm));
			this.txtLyrics = new System.Windows.Forms.TextBox();
			this.txtURL = new System.Windows.Forms.TextBox();
			this.btnGo = new System.Windows.Forms.Button();
			this.btnRefresh = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.alwaysOnTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtLyrics
			// 
			this.txtLyrics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLyrics.Location = new System.Drawing.Point(5, 50);
			this.txtLyrics.Multiline = true;
			this.txtLyrics.Name = "txtLyrics";
			this.txtLyrics.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLyrics.Size = new System.Drawing.Size(382, 327);
			this.txtLyrics.TabIndex = 0;
			// 
			// txtURL
			// 
			this.txtURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtURL.Location = new System.Drawing.Point(5, 25);
			this.txtURL.Name = "txtURL";
			this.txtURL.ReadOnly = true;
			this.txtURL.Size = new System.Drawing.Size(298, 22);
			this.txtURL.TabIndex = 1;
			// 
			// btnGo
			// 
			this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnGo.FlatAppearance.BorderSize = 0;
			this.btnGo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnGo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnGo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnGo.Image = ((System.Drawing.Image)(resources.GetObject("btnGo.Image")));
			this.btnGo.Location = new System.Drawing.Point(361, 23);
			this.btnGo.Name = "btnGo";
			this.btnGo.Size = new System.Drawing.Size(26, 24);
			this.btnGo.TabIndex = 14;
			this.toolTip1.SetToolTip(this.btnGo, "Open URL");
			this.btnGo.UseVisualStyleBackColor = false;
			this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
			// 
			// btnRefresh
			// 
			this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRefresh.FlatAppearance.BorderSize = 0;
			this.btnRefresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
			this.btnRefresh.Location = new System.Drawing.Point(309, 23);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.Size = new System.Drawing.Size(26, 24);
			this.btnRefresh.TabIndex = 14;
			this.toolTip1.SetToolTip(this.btnRefresh, "Refresh Lyrics");
			this.btnRefresh.UseVisualStyleBackColor = false;
			this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.FlatAppearance.BorderSize = 0;
			this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
			this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
			this.btnSave.Location = new System.Drawing.Point(335, 23);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(26, 24);
			this.btnSave.TabIndex = 14;
			this.toolTip1.SetToolTip(this.btnSave, "Save Lyrics");
			this.btnSave.UseVisualStyleBackColor = false;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alwaysOnTopToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(154, 26);
			// 
			// alwaysOnTopToolStripMenuItem
			// 
			this.alwaysOnTopToolStripMenuItem.CheckOnClick = true;
			this.alwaysOnTopToolStripMenuItem.Name = "alwaysOnTopToolStripMenuItem";
			this.alwaysOnTopToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.alwaysOnTopToolStripMenuItem.Text = "Always on top";
			this.alwaysOnTopToolStripMenuItem.Click += new System.EventHandler(this.alwaysOnTopToolStripMenuItem_Click);
			// 
			// LyricsForm
			// 
			this.BackColor = System.Drawing.Color.Black;
			this.Caption = "Lyrics";
			this.ClientSize = new System.Drawing.Size(392, 382);
			this.ContextMenuStrip = this.contextMenuStrip1;
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnRefresh);
			this.Controls.Add(this.btnGo);
			this.Controls.Add(this.txtURL);
			this.Controls.Add(this.txtLyrics);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(100, 19);
			this.Name = "LyricsForm";
			this.ShowInTaskbar = false;
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtLyrics;
		private System.Windows.Forms.TextBox txtURL;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.Button btnRefresh;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem alwaysOnTopToolStripMenuItem;
	}
}
