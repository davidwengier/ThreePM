namespace ThreePM.UI
{
    partial class InfoControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoControl));
            this.btnFolder = new System.Windows.Forms.Button();
            this.lblArtist = new System.Windows.Forms.Label();
            this.lblPlayCount = new System.Windows.Forms.Label();
            this.lblFilename = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblTrack = new System.Windows.Forms.Label();
            this.lblAlbum = new System.Windows.Forms.Label();
            this.albumArtBox1 = new ThreePM.AlbumArtBox();
            this.lblGenre = new System.Windows.Forms.Label();
            this.lblYear = new System.Windows.Forms.Label();
            this.lblAlbumArtist = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.albumArtBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFolder
            // 
            this.btnFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFolder.AutoSize = true;
            this.btnFolder.FlatAppearance.BorderSize = 0;
            this.btnFolder.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnFolder.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFolder.Image = ((System.Drawing.Image)(resources.GetObject("btnFolder.Image")));
            this.btnFolder.Location = new System.Drawing.Point(321, 399);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(22, 22);
            this.btnFolder.TabIndex = 20;
            this.btnFolder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // lblArtist
            // 
            this.lblArtist.AutoSize = true;
            this.lblArtist.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblArtist.ForeColor = System.Drawing.Color.Red;
            this.lblArtist.Location = new System.Drawing.Point(3, 2);
            this.lblArtist.Name = "lblArtist";
            this.lblArtist.Size = new System.Drawing.Size(40, 14);
            this.lblArtist.TabIndex = 17;
            this.lblArtist.Text = "Artist:";
            this.lblArtist.UseMnemonic = false;
            // 
            // lblPlayCount
            // 
            this.lblPlayCount.AutoSize = true;
            this.lblPlayCount.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblPlayCount.ForeColor = System.Drawing.Color.Red;
            this.lblPlayCount.Location = new System.Drawing.Point(3, 178);
            this.lblPlayCount.Name = "lblPlayCount";
            this.lblPlayCount.Size = new System.Drawing.Size(69, 14);
            this.lblPlayCount.TabIndex = 19;
            this.lblPlayCount.Text = "Play Count:";
            this.lblPlayCount.UseMnemonic = false;
            // 
            // lblFilename
            // 
            this.lblFilename.AutoSize = true;
            this.lblFilename.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblFilename.ForeColor = System.Drawing.Color.Red;
            this.lblFilename.Location = new System.Drawing.Point(3, 200);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(58, 14);
            this.lblFilename.TabIndex = 18;
            this.lblFilename.Text = "Filename:";
            this.lblFilename.UseMnemonic = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblTitle.ForeColor = System.Drawing.Color.Red;
            this.lblTitle.Location = new System.Drawing.Point(3, 24);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(35, 14);
            this.lblTitle.TabIndex = 16;
            this.lblTitle.Text = "Title:";
            this.lblTitle.UseMnemonic = false;
            // 
            // lblTrack
            // 
            this.lblTrack.AutoSize = true;
            this.lblTrack.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblTrack.ForeColor = System.Drawing.Color.Red;
            this.lblTrack.Location = new System.Drawing.Point(3, 68);
            this.lblTrack.Name = "lblTrack";
            this.lblTrack.Size = new System.Drawing.Size(41, 14);
            this.lblTrack.TabIndex = 14;
            this.lblTrack.Text = "Track:";
            this.lblTrack.UseMnemonic = false;
            // 
            // lblAlbum
            // 
            this.lblAlbum.AutoSize = true;
            this.lblAlbum.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblAlbum.ForeColor = System.Drawing.Color.Red;
            this.lblAlbum.Location = new System.Drawing.Point(3, 46);
            this.lblAlbum.Name = "lblAlbum";
            this.lblAlbum.Size = new System.Drawing.Size(45, 14);
            this.lblAlbum.TabIndex = 15;
            this.lblAlbum.Text = "Album:";
            this.lblAlbum.UseMnemonic = false;
            // 
            // albumArtBox1
            // 
            this.albumArtBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.albumArtBox1.Location = new System.Drawing.Point(74, 221);
            this.albumArtBox1.Name = "albumArtBox1";
            this.albumArtBox1.Size = new System.Drawing.Size(200, 200);
            this.albumArtBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.albumArtBox1.TabIndex = 22;
            this.albumArtBox1.TabStop = false;
            // 
            // lblGenre
            // 
            this.lblGenre.AutoSize = true;
            this.lblGenre.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblGenre.ForeColor = System.Drawing.Color.Red;
            this.lblGenre.Location = new System.Drawing.Point(3, 112);
            this.lblGenre.Name = "lblGenre";
            this.lblGenre.Size = new System.Drawing.Size(44, 14);
            this.lblGenre.TabIndex = 18;
            this.lblGenre.Text = "Genre:";
            this.lblGenre.UseMnemonic = false;
            // 
            // lblYear
            // 
            this.lblYear.AutoSize = true;
            this.lblYear.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblYear.ForeColor = System.Drawing.Color.Red;
            this.lblYear.Location = new System.Drawing.Point(3, 134);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(36, 14);
            this.lblYear.TabIndex = 18;
            this.lblYear.Text = "Year:";
            this.lblYear.UseMnemonic = false;
            // 
            // lblAlbumArtist
            // 
            this.lblAlbumArtist.AutoSize = true;
            this.lblAlbumArtist.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblAlbumArtist.ForeColor = System.Drawing.Color.Red;
            this.lblAlbumArtist.Location = new System.Drawing.Point(3, 90);
            this.lblAlbumArtist.Name = "lblAlbumArtist";
            this.lblAlbumArtist.Size = new System.Drawing.Size(78, 14);
            this.lblAlbumArtist.TabIndex = 18;
            this.lblAlbumArtist.Text = "Album Artist:";
            this.lblAlbumArtist.UseMnemonic = false;
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblDuration.ForeColor = System.Drawing.Color.Red;
            this.lblDuration.Location = new System.Drawing.Point(3, 156);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(57, 14);
            this.lblDuration.TabIndex = 19;
            this.lblDuration.Text = "Duration:";
            this.lblDuration.UseMnemonic = false;
            // 
            // InfoControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.albumArtBox1);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.lblArtist);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.lblPlayCount);
            this.Controls.Add(this.lblAlbumArtist);
            this.Controls.Add(this.lblYear);
            this.Controls.Add(this.lblGenre);
            this.Controls.Add(this.lblFilename);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblTrack);
            this.Controls.Add(this.lblAlbum);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.ForeColor = System.Drawing.Color.Red;
            this.Name = "InfoControl";
            this.Size = new System.Drawing.Size(348, 424);
            ((System.ComponentModel.ISupportInitialize)(this.albumArtBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AlbumArtBox albumArtBox1;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.Label lblArtist;
        private System.Windows.Forms.Label lblPlayCount;
        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblTrack;
        private System.Windows.Forms.Label lblAlbum;
        private System.Windows.Forms.Label lblGenre;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.Label lblAlbumArtist;
        private System.Windows.Forms.Label lblDuration;
    }
}
