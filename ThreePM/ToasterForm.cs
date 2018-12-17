using System;
using System.Windows.Forms;

namespace ThreePM
{
    public partial class ToasterForm : BaseForm
    {
        private readonly double _targetOpacity = 0.94f;

        public ToasterForm()
        {
            InitializeComponent();
            tmrStay.Interval = Registry.GetValue("ToasterForm.StayDelay", 2000);
            tmrFade.Interval = Registry.GetValue("ToasterForm.FadeDelay", 50);
            tmrFadeOut.Interval = tmrFade.Interval;
        }

        protected override void InitPlayer()
        {
            this.Player.SongOpened += new EventHandler<ThreePM.MusicPlayer.SongEventArgs>(Player_SongOpened);
            if (this.Player.CurrentSong != null)
            {
                Player_SongOpened(this, new ThreePM.MusicPlayer.SongEventArgs(this.Player.CurrentSong));
            }
        }

        protected override void UnInitPlayer()
        {
            this.Player.SongOpened -= new EventHandler<ThreePM.MusicPlayer.SongEventArgs>(Player_SongOpened);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void tmrFade_Tick(object sender, EventArgs e)
        {
            this.Opacity += 0.05;
            if (this.Opacity >= _targetOpacity)
            {
                this.Opacity = _targetOpacity;
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
                this.Caption = "";
                tmrFadeOut.Stop();
                this.Hide();
            }
        }

        private void Player_SongOpened(object sender, ThreePM.MusicPlayer.SongEventArgs e)
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
            tmrStay.Interval = Registry.GetValue("ToasterForm.StayDelay", 2000);
            tmrFade.Interval = Registry.GetValue("ToasterForm.FadeDelay", 50);
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
            this.Player.Next();
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
            this.Caption = "Now Playing";
            Refresh();
            tmrFadeOut.Stop();
            tmrStay.Stop();
            tmrFade.Start();
        }

        private void ToasterForm_MouseLeave(object sender, EventArgs e)
        {
            this.Caption = "";
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
