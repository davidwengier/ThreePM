using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace starH45.net.mp3
{
	public partial class DeveloperForm : BaseForm
	{
		public DeveloperForm()
		{
			InitializeComponent();
		}

		protected override void InitPlayer()
		{
			Player.SongOpened += new EventHandler<starH45.net.mp3.player.SongEventArgs>(Player_SongOpened);
		}

		void Player_SongOpened(object sender, starH45.net.mp3.player.SongEventArgs e)
		{
			if (albumPanel1.DataSource != null)
			{
				albumPanel1.SelectedItem = e.Song;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (textBox1.Text == "A")
			{
				albumPanel1.BringToFront();
				albumPanel1.DataSource = Library.GetAlbumsAsEntries();
			}
			else
			{
				try
				{
					dataGridView1.DataSource = Library.GetDataSet(textBox1.Text).Tables[0];
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Oops");
				}
			}
		}
	}
}