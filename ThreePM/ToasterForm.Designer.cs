namespace ThreePM
{
	partial class ToasterForm
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
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblAlbum = new System.Windows.Forms.Label();
			this.pctAlbum = new ThreePM.AlbumArtBox();
			this.lblArtist = new System.Windows.Forms.Label();
			this.tmrFade = new System.Windows.Forms.Timer(this.components);
			this.tmrFadeOut = new System.Windows.Forms.Timer(this.components);
			this.tmrStay = new System.Windows.Forms.Timer(this.components);
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblNext = new System.Windows.Forms.LinkLabel();
			this.lblShowPlayer = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this.pctAlbum)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblTitle
			// 
			this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.lblTitle.AutoEllipsis = true;
			this.lblTitle.AutoSize = true;
			this.lblTitle.ForeColor = System.Drawing.Color.Red;
			this.lblTitle.Location = new System.Drawing.Point(129, 52);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(184, 14);
			this.lblTitle.TabIndex = 15;
			this.lblTitle.Text = "[Title]";
			this.lblTitle.UseMnemonic = false;
			// 
			// lblAlbum
			// 
			this.lblAlbum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.lblAlbum.AutoEllipsis = true;
			this.lblAlbum.AutoSize = true;
			this.lblAlbum.ForeColor = System.Drawing.Color.Red;
			this.lblAlbum.Location = new System.Drawing.Point(129, 38);
			this.lblAlbum.Name = "lblAlbum";
			this.lblAlbum.Size = new System.Drawing.Size(184, 14);
			this.lblAlbum.TabIndex = 14;
			this.lblAlbum.Text = "[Album]";
			this.lblAlbum.UseMnemonic = false;
			// 
			// pctAlbum
			// 
			this.pctAlbum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pctAlbum.Location = new System.Drawing.Point(3, 3);
			this.pctAlbum.Name = "pctAlbum";
			this.tableLayoutPanel1.SetRowSpan(this.pctAlbum, 6);
			this.pctAlbum.Size = new System.Drawing.Size(120, 108);
			this.pctAlbum.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pctAlbum.TabIndex = 13;
			this.pctAlbum.TabStop = false;
			// 
			// lblArtist
			// 
			this.lblArtist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.lblArtist.AutoEllipsis = true;
			this.lblArtist.AutoSize = true;
			this.lblArtist.ForeColor = System.Drawing.Color.Red;
			this.lblArtist.Location = new System.Drawing.Point(129, 24);
			this.lblArtist.Name = "lblArtist";
			this.lblArtist.Size = new System.Drawing.Size(184, 14);
			this.lblArtist.TabIndex = 12;
			this.lblArtist.Text = "[Artist]";
			this.lblArtist.UseMnemonic = false;
			// 
			// tmrFade
			// 
			this.tmrFade.Interval = 50;
			this.tmrFade.Tick += new System.EventHandler(this.tmrFade_Tick);
			// 
			// tmrFadeOut
			// 
			this.tmrFadeOut.Interval = 50;
			this.tmrFadeOut.Tick += new System.EventHandler(this.tmrFadeOut_Tick);
			// 
			// tmrStay
			// 
			this.tmrStay.Interval = 2000;
			this.tmrStay.Tick += new System.EventHandler(this.tmrStay_Tick);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tableLayoutPanel1.Controls.Add(this.pctAlbum, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.lblArtist, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.lblAlbum, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.lblTitle, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 5);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 6;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(316, 114);
			this.tableLayoutPanel1.TabIndex = 18;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.lblNext);
			this.panel1.Controls.Add(this.lblShowPlayer);
			this.panel1.Location = new System.Drawing.Point(129, 93);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(184, 18);
			this.panel1.TabIndex = 16;
			// 
			// lblNext
			// 
			this.lblNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblNext.AutoSize = true;
			this.lblNext.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.lblNext.Location = new System.Drawing.Point(49, 0);
			this.lblNext.Name = "lblNext";
			this.lblNext.Size = new System.Drawing.Size(33, 14);
			this.lblNext.TabIndex = 18;
			this.lblNext.TabStop = true;
			this.lblNext.Text = "Next";
			this.lblNext.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblNext_LinkClicked);
			// 
			// lblShowPlayer
			// 
			this.lblShowPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblShowPlayer.AutoSize = true;
			this.lblShowPlayer.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.lblShowPlayer.Location = new System.Drawing.Point(107, 0);
			this.lblShowPlayer.Name = "lblShowPlayer";
			this.lblShowPlayer.Size = new System.Drawing.Size(74, 14);
			this.lblShowPlayer.TabIndex = 19;
			this.lblShowPlayer.TabStop = true;
			this.lblShowPlayer.Text = "Show Player";
			this.lblShowPlayer.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.lblShowPlayer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblShowPlayer_LinkClicked);
			// 
			// ToasterForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Caption = "";
			this.ClientSize = new System.Drawing.Size(321, 118);
			this.ControlBox = false;
			this.Controls.Add(this.tableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.InternalBorderColor = System.Drawing.Color.Silver;
			this.InternalBorderSize = 2;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(100, 20);
			this.Name = "ToasterForm";
			this.Opacity = 0;
			this.ShowInTaskbar = false;
			this.Sizable = true;
			this.SnapTo = false;
			this.Text = "This is nothing important";
			this.TopMost = true;
			this.MouseLeave += new System.EventHandler(this.ToasterForm_MouseLeave);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ToasterForm_MouseMove);
			((System.ComponentModel.ISupportInitialize)(this.pctAlbum)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Label lblAlbum;
		private AlbumArtBox pctAlbum;
		private System.Windows.Forms.Label lblArtist;
		private System.Windows.Forms.Timer tmrFade;
		private System.Windows.Forms.Timer tmrFadeOut;
		private System.Windows.Forms.Timer tmrStay;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.LinkLabel lblNext;
		private System.Windows.Forms.LinkLabel lblShowPlayer;

	}
}