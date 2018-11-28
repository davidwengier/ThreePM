using System;
using System.ComponentModel;
using System.Windows.Forms;
using ThreePM.MusicPlayer;
using ThreePM.Utilities;

namespace ThreePM
{
    public partial class AlbumArtBox : PictureBox
    {
        private SongInfo _song;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SongInfo Song
        {
            get
            {
                return _song;
            }
            set
            {
                _song = value;
                if (value != null)
                {
                    LoadSong();
                }
            }
        }

        private void LoadSong()
        {
            if (this.Song == null) return;
            if (this.Song.HasFrontCover)
            {
                this.Image = this.Song.GetFrontCover(Math.Min(this.Width, this.Height), Math.Min(this.Width, this.Height));
            }
            else
            {
                this.Image = AlbumArtHelper.GetAlbumArt(this.Song.FileName, Math.Min(this.Width, this.Height), Math.Min(this.Width, this.Height));
            }
        }

        public AlbumArtBox()
        {
            this.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LoadSong();
        }
    }
}
