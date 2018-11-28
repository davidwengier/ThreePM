using System;
using System.Windows.Forms;

namespace ThreePM
{
    public partial class TransparentLabel : Label
    {
        public const int WM_NCHITTEST = 0x84;
        public const int HT_TRANSPARENT = -1;

        public TransparentLabel()
        {
        }

        protected override void WndProc(ref Message m)
        {
            if (!this.DesignMode && m.Msg == WM_NCHITTEST)
            {
                this.DefWndProc(ref m);
                m.Result = new IntPtr(HT_TRANSPARENT);
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
