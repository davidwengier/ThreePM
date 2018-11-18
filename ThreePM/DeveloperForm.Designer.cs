namespace ThreePM
{
	partial class DeveloperForm
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
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.albumPanel1 = new ThreePM.AlbumPanel();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(16, 61);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(606, 381);
			this.dataGridView1.TabIndex = 3;
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(53, 33);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(516, 22);
			this.textBox1.TabIndex = 1;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(575, 32);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(47, 22);
			this.button1.TabIndex = 2;
			this.button1.Text = "GO";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.Red;
			this.label1.Location = new System.Drawing.Point(14, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(33, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "SQL:";
			// 
			// albumPanel1
			// 
			this.albumPanel1.AlbumContextMenuStrip = null;
			this.albumPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.albumPanel1.Location = new System.Drawing.Point(17, 61);
			this.albumPanel1.Name = "albumPanel1";
			this.albumPanel1.Size = new System.Drawing.Size(605, 381);
			this.albumPanel1.TabIndex = 4;
			this.albumPanel1.Text = "albumPanel1";
			// 
			// DeveloperForm
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Caption = "Developer Stuff - Don\'t Touch Unless You Know What You\'re Doing (And Even Then???" +
				")";
			this.ClientSize = new System.Drawing.Size(638, 457);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.albumPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.InternalBorderColor = System.Drawing.Color.Silver;
			this.InternalBorderSize = 2;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DeveloperForm";
			this.Sizable = true;
			this.SnapTo = false;
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
		private AlbumPanel albumPanel1;
	}
}