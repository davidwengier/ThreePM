using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ThreePM
{
	public partial class TransparentPictureBox : PictureBox
	{
		public const int WM_NCHITTEST = 0x84;
		public const int HT_TRANSPARENT =-1;

		public TransparentPictureBox()
		{
		}

		protected override void WndProc(ref Message m)
		{
			if (!DesignMode && m.Msg == WM_NCHITTEST)
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
