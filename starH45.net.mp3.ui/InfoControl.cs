using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using starH45.net.mp3.player;
using System.IO;

namespace starH45.net.mp3.ui
{
	public partial class InfoControl : UserControl
	{
		private player.Player m_player;
		private library.Library m_library;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public player.Player Player
		{
			get { return m_player; }
			set
			{
				m_player = value;
				InitPlayer();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public library.Library Library
		{
			get { return m_library; }
			set
			{
				m_library = value;
				InitLibrary();
			}
		}

		public InfoControl()
		{
			InitializeComponent();
		}

		private void InitLibrary()
		{
			Library.PlayCountUpdated += new EventHandler<starH45.net.mp3.library.LibraryEntryEventArgs>(Library_LibraryUpdated);
			Library.LibraryUpdated += new EventHandler<starH45.net.mp3.library.LibraryEntryEventArgs>(Library_LibraryUpdated);
		}

		void Library_LibraryUpdated(object sender, starH45.net.mp3.library.LibraryEntryEventArgs e)
		{
			if (e.LibraryEntry == null)
				return;
			if (e.LibraryEntry.FileName.Equals(m_player.CurrentSong.FileName))
			{
				LoadSong();
			}
		}

		private void InitPlayer()
		{
			Player.LoadingSong += new EventHandler<FileEventArgs>(m_player_LoadingSong);
			Player.SongOpened += new EventHandler<SongEventArgs>(m_player_SongOpened);
			if (Player.CurrentSong != null)
			{
				LoadSong();
			}
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				Library.PlayCountUpdated -= new EventHandler<starH45.net.mp3.library.LibraryEntryEventArgs>(Library_LibraryUpdated);
				Library.LibraryUpdated -= new EventHandler<starH45.net.mp3.library.LibraryEntryEventArgs>(Library_LibraryUpdated);
				Player.LoadingSong -= new EventHandler<FileEventArgs>(m_player_LoadingSong);
				Player.SongOpened -= new EventHandler<SongEventArgs>(m_player_SongOpened);

				components.Dispose();
			}
			base.Dispose(disposing);
		}

		void m_player_SongOpened(object sender, SongEventArgs e)
		{
			LoadSong();
		}

		void m_player_LoadingSong(object sender, FileEventArgs e)
		{
			lblTitle.Text = "Title:";
			lblArtist.Text = "Artist:";
			lblAlbum.Text = "Album:";
			lblTrack.Text = "Track:";
			lblAlbumArtist.Text = "Album Artist:";
			lblGenre.Text = "Genre:";
			lblYear.Text = "Year:";
			lblPlayCount.Text = "Play Count:";
			lblDuration.Text = "Duration:";
			lblFilename.Text = "Filename: " + e.Filename;
			lblFilename.Refresh();
		}



		private void btnFolder_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(Path.GetDirectoryName(Player.CurrentSong.FileName));
		}

		private void LoadSong()
		{
			lblTitle.Text = "Title: " + Player.CurrentSong.Title;
			lblArtist.Text = "Artist: " + Player.CurrentSong.Artist;
			lblAlbum.Text = "Album: " + Player.CurrentSong.Album;
			lblTrack.Text = "Track: " + Player.CurrentSong.TrackNumber;
			lblFilename.Text = "Filename: " + Player.CurrentSong.FileName;
			lblYear.Text = "Year: " + Player.CurrentSong.Year;
			lblGenre.Text = "Genre: " + Player.CurrentSong.Genre;
			lblAlbumArtist.Text = "Album Artist: " + Player.CurrentSong.AlbumArtist;
			lblDuration.Text = "Duration: " + Player.CurrentSong.DurationDescription;

			lblPlayCount.Text = "Play Count: " + Library.GetPlayCount(Player.CurrentSong.FileName).ToString();

			albumArtBox1.Song = Player.CurrentSong;
		}
	}
}
