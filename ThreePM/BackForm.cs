using System.Windows.Forms;

namespace ThreePM
{
    public partial class BackForm : Form
    {
        public BackForm()
        {
            InitializeComponent();
            //this.DoubleBuffered = true;
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    using (Pen penCurrent = new Pen(Color.Black))
        //    {
        //        Rectangle Rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
        //        e.Graphics.DrawRectangle(penCurrent, Rect);
        //    }
        //}
    }
}
