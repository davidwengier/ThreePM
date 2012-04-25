using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using starH45.net.mp3.player;
using System.IO;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;

namespace starH45.net.mp3
{
	public partial class AlbumArtForm : BaseForm
	{
		#region Constructor

		public AlbumArtForm()
		{
			InitializeComponent();
		}

		#endregion

		#region Overrides

		protected override void InitPlayer()
		{
			Player.SongOpened += new EventHandler<SongEventArgs>(player_SongOpened);
			if (Player.CurrentSong != null)
			{
				albumArtBox1.Song = Player.CurrentSong;
			}
		}

		protected override void UnInitPlayer()
		{
			Player.SongOpened -= new EventHandler<SongEventArgs>(player_SongOpened);
		}

		#endregion

		#region Event Handlers

		private void player_SongOpened(object sender, SongEventArgs e)
		{
			albumArtBox1.Song = Player.CurrentSong;
		}

		private void openContainingFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(Path.GetDirectoryName(Player.CurrentSong.FileName));
		}

		private void downloadAlbumArtToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AlbumArtPicker f = new AlbumArtPicker(Player.CurrentSong);
			if (!f.IsDisposed)
			{
				f.Player = Player;
				f.Show(this.Owner);
			}
		}

		#endregion
	}
}
