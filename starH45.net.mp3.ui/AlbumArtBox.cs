using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using starH45.net.mp3.player;
using System.IO;
using starH45.net.mp3.ui;
using starH45.net.mp3.utilities;

namespace starH45.net.mp3
{
	public partial class AlbumArtBox : PictureBox
	{
		private SongInfo m_song;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SongInfo Song
		{
			get
			{
				return m_song;
			}
			set
			{
				m_song = value;
				if (value != null)
				{
					LoadSong();
				}
			}
		}

		private void LoadSong()
		{
			if (Song == null) return;
			if (Song.HasFrontCover)
			{
				this.Image = Song.GetFrontCover(Math.Min(Width, Height), Math.Min(Width, Height));
			}
			else
			{
				this.Image = AlbumArtHelper.GetAlbumArt(Song.FileName, Math.Min(Width, Height), Math.Min(Width, Height));
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
