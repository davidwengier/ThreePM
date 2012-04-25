namespace starH45.net.mp3
{
	partial class NotifyForm
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
			this.lblArtist = new System.Windows.Forms.Label();
			this.pctAlbum = new starH45.net.mp3.AlbumArtBox();
			this.lblAlbum = new System.Windows.Forms.Label();
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblNext = new System.Windows.Forms.LinkLabel();
			this.lblShowPlayer = new System.Windows.Forms.LinkLabel();
			this.ControlsPanel = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.pctAlbum)).BeginInit();
			this.ControlsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblArtist
			// 
			this.lblArtist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblArtist.AutoEllipsis = true;
			this.lblArtist.ForeColor = System.Drawing.Color.Red;
			this.lblArtist.Location = new System.Drawing.Point(72, 6);
			this.lblArtist.Name = "lblArtist";
			this.lblArtist.Size = new System.Drawing.Size(159, 18);
			this.lblArtist.TabIndex = 0;
			this.lblArtist.Text = "[Artist]";
			this.lblArtist.UseMnemonic = false;
			// 
			// pctAlbum
			// 
			this.pctAlbum.Location = new System.Drawing.Point(12, 9);
			this.pctAlbum.Name = "pctAlbum";
			this.pctAlbum.Size = new System.Drawing.Size(57, 57);
			this.pctAlbum.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pctAlbum.TabIndex = 1;
			this.pctAlbum.TabStop = false;
			// 
			// lblAlbum
			// 
			this.lblAlbum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblAlbum.AutoEllipsis = true;
			this.lblAlbum.ForeColor = System.Drawing.Color.Red;
			this.lblAlbum.Location = new System.Drawing.Point(72, 20);
			this.lblAlbum.Name = "lblAlbum";
			this.lblAlbum.Size = new System.Drawing.Size(159, 14);
			this.lblAlbum.TabIndex = 2;
			this.lblAlbum.Text = "[Album]";
			this.lblAlbum.UseMnemonic = false;
			// 
			// lblTitle
			// 
			this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblTitle.AutoEllipsis = true;
			this.lblTitle.ForeColor = System.Drawing.Color.Red;
			this.lblTitle.Location = new System.Drawing.Point(72, 34);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(159, 14);
			this.lblTitle.TabIndex = 3;
			this.lblTitle.Text = "[Title]";
			this.lblTitle.UseMnemonic = false;
			// 
			// lblNext
			// 
			this.lblNext.AutoSize = true;
			this.lblNext.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.lblNext.Location = new System.Drawing.Point(72, 48);
			this.lblNext.Name = "lblNext";
			this.lblNext.Size = new System.Drawing.Size(33, 14);
			this.lblNext.TabIndex = 4;
			this.lblNext.TabStop = true;
			this.lblNext.Text = "Next";
			this.lblNext.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblNext_LinkClicked);
			// 
			// lblShowPlayer
			// 
			this.lblShowPlayer.AutoSize = true;
			this.lblShowPlayer.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.lblShowPlayer.Location = new System.Drawing.Point(111, 48);
			this.lblShowPlayer.Name = "lblShowPlayer";
			this.lblShowPlayer.Size = new System.Drawing.Size(74, 14);
			this.lblShowPlayer.TabIndex = 5;
			this.lblShowPlayer.TabStop = true;
			this.lblShowPlayer.Text = "Show Player";
			this.lblShowPlayer.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.lblShowPlayer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblShowPlayer_LinkClicked);
			// 
			// ControlsPanel
			// 
			this.ControlsPanel.Controls.Add(this.lblShowPlayer);
			this.ControlsPanel.Controls.Add(this.lblNext);
			this.ControlsPanel.Controls.Add(this.lblTitle);
			this.ControlsPanel.Controls.Add(this.lblAlbum);
			this.ControlsPanel.Controls.Add(this.pctAlbum);
			this.ControlsPanel.Controls.Add(this.lblArtist);
			this.ControlsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ControlsPanel.Location = new System.Drawing.Point(0, 19);
			this.ControlsPanel.Name = "ControlsPanel";
			this.ControlsPanel.Size = new System.Drawing.Size(240, 77);
			this.ControlsPanel.TabIndex = 6;
			// 
			// NotifyForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Caption = "starH45.net.mp3 Notification";
			this.ClientSize = new System.Drawing.Size(241, 97);
			this.ControlBox = false;
			this.Controls.Add(this.ControlsPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NotifyForm";
			this.ShowInTaskbar = false;
			this.SnapTo = false;
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.pctAlbum)).EndInit();
			this.ControlsPanel.ResumeLayout(false);
			this.ControlsPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblArtist;
		private AlbumArtBox pctAlbum;
		private System.Windows.Forms.Label lblAlbum;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.LinkLabel lblNext;
		private System.Windows.Forms.LinkLabel lblShowPlayer;
		private System.Windows.Forms.Panel ControlsPanel;
	}
}