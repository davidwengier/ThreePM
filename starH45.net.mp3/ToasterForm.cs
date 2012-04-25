using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace starH45.net.mp3
{
	public partial class ToasterForm : BaseForm
	{
		//[DllImport("user32.dll")]
		//private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

		private double m_targetOpacity = 0.94f;

		public ToasterForm()
		{
			InitializeComponent();
			tmrStay.Interval = Utilities.GetValue("ToasterForm.StayDelay", 2000);
			tmrFade.Interval = Utilities.GetValue("ToasterForm.FadeDelay", 50);
			tmrFadeOut.Interval = tmrFade.Interval;
		}

		protected override void InitPlayer()
		{
			Player.SongOpened += new EventHandler<starH45.net.mp3.player.SongEventArgs>(Player_SongOpened);
			if (Player.CurrentSong != null)
			{
				Player_SongOpened(this, new starH45.net.mp3.player.SongEventArgs(Player.CurrentSong));
			}
		}

		protected override void UnInitPlayer()
		{
			Player.SongOpened -= new EventHandler<starH45.net.mp3.player.SongEventArgs>(Player_SongOpened);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		private void tmrFade_Tick(object sender, EventArgs e)
		{
			this.Opacity += 0.05;
			if (this.Opacity >= m_targetOpacity)
			{
				this.Opacity = m_targetOpacity;
				tmrFade.Stop();
				tmrStay.Stop();
				tmrStay.Start();
			}
		}

		private void tmrFadeOut_Tick(object sender, EventArgs e)
		{
			this.Opacity -= 0.05;
			if (this.Opacity <= 0)
			{
				this.Opacity = 0;
				Caption = "";
				tmrFadeOut.Stop();
				this.Hide();
			}
		}

		void Player_SongOpened(object sender, starH45.net.mp3.player.SongEventArgs e)
		{
			// Set the display controls
			lblArtist.Text = e.Song.Artist;
			lblAlbum.Text = e.Song.Album;
			lblTitle.Text = e.Song.Title;
			pctAlbum.Song = e.Song;

			StartFadeIn();
		}

		private void StartFadeIn()
		{
			tmrStay.Interval = Utilities.GetValue("ToasterForm.StayDelay", 2000);
			tmrFade.Interval = Utilities.GetValue("ToasterForm.FadeDelay", 50);
			tmrFadeOut.Interval = tmrFade.Interval;

			this.Show();
			tmrFadeOut.Stop();
			tmrStay.Stop();
			tmrFade.Start();
		}

		private void tmrStay_Tick(object sender, EventArgs e)
		{
			tmrStay.Stop();
			tmrFadeOut.Start();
		}

		private void lblNext_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Player.Next();
		}

		private void lblShowPlayer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			foreach (Form f in Application.OpenForms)
			{
				if (f is MainForm)
				{
					f.BringToFront();
				}
			}
		}

		private void ToasterForm_MouseMove(object sender, MouseEventArgs e)
		{
			Caption = "Now Playing";
			Refresh();
			tmrFadeOut.Stop();
			tmrStay.Stop();
			tmrFade.Start();
		}

		private void ToasterForm_MouseLeave(object sender, EventArgs e)
		{
			Caption = "";
			Refresh();
		}

		//internal void ShowStatus(string status)
		//{
		//    lblArtist.Text = status;
		//    lblAlbum.Text = "";
		//    lblShowPlayer.Visible = false;
		//    lblTitle.Text = "";
		//    pctAlbum.Visible = false;

		//    StartFadeIn();
		//}
	}
}