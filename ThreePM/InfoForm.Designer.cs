namespace ThreePM
{
    partial class InfoForm
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
            this.infoControl1 = new ThreePM.UI.InfoControl();
            this.SuspendLayout();
            // 
            // infoControl1
            // 
            this.infoControl1.BackColor = System.Drawing.Color.Black;
            this.infoControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoControl1.Font = new System.Drawing.Font("Tahoma", 9F);
            this.infoControl1.ForeColor = System.Drawing.Color.Red;
            this.infoControl1.Location = new System.Drawing.Point(2, 20);
            this.infoControl1.Name = "infoControl1";
            this.infoControl1.Size = new System.Drawing.Size(324, 322);
            this.infoControl1.TabIndex = 0;
            // 
            // InfoForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Caption = "Track Information";
            this.ClientSize = new System.Drawing.Size(329, 345);
            this.Controls.Add(this.infoControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.InternalBorderColor = System.Drawing.Color.Silver;
            this.InternalBorderSize = 2;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InfoForm";
            this.ShowInTaskbar = false;
            this.Sizable = true;
            this.ResumeLayout(false);

        }


        #endregion

        private ThreePM.UI.InfoControl infoControl1;

    }
}
