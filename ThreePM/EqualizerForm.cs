using System;
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
                if (c is Ticker tck)
                {
                    tck.Duration = 30;
                }
            }
        }

        protected override void InitPlayer()
        {
            foreach (Control c in this.Controls)
            {
                if (c is Ticker tck)
                {
                    tck.SetPosition(Convert.ToSingle(Registry.GetValue("EqualizerForm." + tck.Name + ".Value", this.Player.GetEqualizerPosition(Convert.ToInt32(tck.Name.Substring(3))).ToString())) + 15);
                }
            }
        }

        private void tck_Changing(object sender, EventArgs e)
        {
            var tck = sender as Ticker;
            this.Player.SetEqualizerPosition(Convert.ToInt32(tck.Name.Substring(3)), (float)tck.Position - 15);
            Registry.SetValue("EqualizerForm." + tck.Name + ".Value", this.Player.GetEqualizerPosition(Convert.ToInt32(tck.Name.Substring(3))).ToString());
        }

        private void label11_Click(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                if (c is Ticker tck)
                {
                    tck.SetPosition(15);
                }
            }
        }
    }
}
