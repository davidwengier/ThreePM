using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ThreePM.MusicPlayer;
using System.IO;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;

namespace ThreePM
{
    public partial class InfoForm : BaseForm
    {
        public InfoForm()
        {
            InitializeComponent();
        }

		protected override void InitPlayer()
        {
			infoControl1.Player = Player;
        }

		protected override void InitLibrary()
		{
			infoControl1.Library = Library;
		}
    }
}
