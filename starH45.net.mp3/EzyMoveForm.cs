using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace starH45.net.mp3
{
	public partial class EzyMoveForm : Form
	{
		#region Win32 Stuff

		public const int WM_NCLBUTTONDBLCLK = 0xA3;
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int WM_NCHITTEST = 0x84;
		public const int HT_CLIENT = 0x01;
		public const int HT_CAPTION = 0x2;

		#endregion

		public EzyMoveForm()
		{

		}

		protected override void OnLoad(EventArgs e)
		{
			if (!DesignMode)
			{
				Left = Convert.ToInt32((Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + Application.CompanyName + @"\" + Application.ProductName, this.Name + " Left", Left) ?? Left));
				Top = Convert.ToInt32((Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + Application.CompanyName + @"\" + Application.ProductName, this.Name + " Top", Top) ?? Top));
				Width = Convert.ToInt32((Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + Application.CompanyName + @"\" + Application.ProductName, this.Name + " Width", Width) ?? Width));
				Height = Convert.ToInt32((Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\" + Application.CompanyName + @"\" + Application.ProductName, this.Name + " Height", Height) ?? Height));
			}
			base.OnLoad(e);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (!DesignMode)
			{
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + Application.CompanyName + @"\" + Application.ProductName, this.Name + " Left", Left);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + Application.CompanyName + @"\" + Application.ProductName, this.Name + " Top", Top);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + Application.CompanyName + @"\" + Application.ProductName, this.Name + " Width", Width);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\" + Application.CompanyName + @"\" + Application.ProductName, this.Name + " Height", Height);
			}
			base.OnClosing(e);
		}

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			if (!DesignMode)
			{

				if (m.Msg == WM_NCHITTEST)
				{
					this.DefWndProc(ref m);
					if (m.Result == new IntPtr(HT_CLIENT))
					{
						if (Control.MouseButtons != MouseButtons.Right)
						{
							m.Result = new IntPtr(HT_CAPTION);
							return;
						}
					}
				}
				else if (m.Msg == WM_NCLBUTTONDBLCLK)
				{
					m.Result = new IntPtr(0);
					return;
				}
			}
			base.WndProc(ref m);
		}

	}
}
