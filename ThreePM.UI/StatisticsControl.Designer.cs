namespace ThreePM.UI
{
    partial class StatisticsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstStatistics = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lstStatistics
            // 
            this.lstStatistics.BackColor = System.Drawing.Color.Black;
            this.lstStatistics.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStatistics.ForeColor = System.Drawing.Color.Red;
            this.lstStatistics.FormattingEnabled = true;
            this.lstStatistics.IntegralHeight = false;
            this.lstStatistics.ItemHeight = 14;
            this.lstStatistics.Location = new System.Drawing.Point(0, 0);
            this.lstStatistics.Name = "lstStatistics";
            this.lstStatistics.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstStatistics.Size = new System.Drawing.Size(344, 369);
            this.lstStatistics.TabIndex = 21;
            // 
            // StatisticsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.lstStatistics);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.ForeColor = System.Drawing.Color.Red;
            this.Name = "StatisticsControl";
            this.Size = new System.Drawing.Size(344, 369);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstStatistics;
    }
}
