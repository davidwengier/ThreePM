namespace ThreePM
{
	partial class SearchForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
			this.searchControl1 = new ThreePM.UI.SearchControl();
			this.SuspendLayout();
			// 
			// searchControl1
			// 
			this.searchControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.searchControl1.BackColor = System.Drawing.Color.Black;
			this.searchControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.searchControl1.Font = new System.Drawing.Font("Tahoma", 9F);
			this.searchControl1.ForeColor = System.Drawing.Color.Red;
			this.searchControl1.Location = new System.Drawing.Point(2, 20);
			this.searchControl1.Name = "searchControl1";
			this.searchControl1.Size = new System.Drawing.Size(444, 289);
			this.searchControl1.TabIndex = 0;
			this.searchControl1.SongQueued += new System.EventHandler(this.searchControl1_SongQueued);
			this.searchControl1.SongPlayed += new System.EventHandler(this.searchControl1_SongPlayed);
			// 
			// SearchForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Caption = "Find Song";
			this.ClientSize = new System.Drawing.Size(449, 312);
			this.Controls.Add(this.searchControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.InternalBorderColor = System.Drawing.Color.Silver;
			this.InternalBorderSize = 2;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SearchForm";
			this.ShowInTaskbar = false;
			this.Sizable = true;
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchForm_KeyDown);
			this.ResumeLayout(false);

		}

		private ThreePM.UI.SearchControl searchControl1;
	}
}
