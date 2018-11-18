using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ThreePM.UI;

namespace ThreePM
{
	public partial class EqualizerForm : BaseForm
	{
		public EqualizerForm()
		{
			InitializeComponent();
			foreach (Control c in this.Controls)
			{
				Ticker tck = c as Ticker;
				if (tck != null)
				{
					tck.Duration = 30;
				}
			}
		}

		protected override void InitPlayer()
		{
			foreach (Control c in this.Controls)
			{
				Ticker tck = c as Ticker;
				if (tck != null)
				{
					tck.SetPosition(Convert.ToSingle(Registry.GetValue("EqualizerForm." +tck.Name +".Value", Player.GetEqualizerPosition(Convert.ToInt32(tck.Name.Substring(3))).ToString())) + 15);
				}
			}
		}

		private void tck_Changing(object sender, EventArgs e)
		{
			Ticker tck = sender as Ticker;
			Player.SetEqualizerPosition(Convert.ToInt32(tck.Name.Substring(3)), (float)tck.Position - 15);
			Registry.SetValue("EqualizerForm." + tck.Name + ".Value", Player.GetEqualizerPosition(Convert.ToInt32(tck.Name.Substring(3))).ToString());
		}

		private void label11_Click(object sender, EventArgs e)
		{
			foreach (Control c in this.Controls)
			{
				Ticker tck = c as Ticker;
				if (tck != null)
				{
					tck.SetPosition(15);
				}
			}
		}
	}
}
