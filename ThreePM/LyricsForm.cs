using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace ThreePM
{
	public partial class LyricsForm : ThreePM.BaseForm
	{
		private ThreePM.Utilities.LyricsHelper m_lyricsHelper;
		private ThreePM.MusicPlayer.SongInfo m_lastSong;

		public LyricsForm()
		{
			InitializeComponent();
		
			TopMost = Registry.GetValue("LyricsForm.TopMost", false);
			alwaysOnTopToolStripMenuItem.Checked = TopMost;
		}

		void m_lyricsHelper_StatusChanged(object sender, EventArgs e)
		{
			SetLyricsTextBox(m_lyricsHelper.Status);
		}

		private void SetLyricsTextBox(string text)
		{
			if (txtLyrics.IsHandleCreated)
			{
				txtLyrics.Invoke((MethodInvoker)delegate
				{
					txtLyrics.Text = text;
				});
			}
			else
			{
				txtLyrics.CreateControl();
				txtLyrics.Text = text;
			}
		}

		void m_lyricsHelper_CurrentURLChanged(object sender, EventArgs e)
		{
			if (txtURL.IsHandleCreated)
			{
				txtURL.Invoke((MethodInvoker)delegate
				{
					txtURL.Text = m_lyricsHelper.CurrentURL;
				});
			}
		}

		void m_lyricsHelper_LyricsFound(object sender, ThreePM.Utilities.LyricsFoundEventArgs e)
		{
			SetLyricsTextBox(e.Lyrics);
			Library.SetLyrics(m_lastSong.Title, m_lastSong.Artist, e.Lyrics);
			btnGo.Enabled = true;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override void InitLibrary()
		{
			m_lyricsHelper = new ThreePM.Utilities.LyricsHelper(this.Library);
			m_lyricsHelper.LyricsFound += new EventHandler<ThreePM.Utilities.LyricsFoundEventArgs>(m_lyricsHelper_LyricsFound);
			m_lyricsHelper.CurrentURLChanged += new EventHandler(m_lyricsHelper_CurrentURLChanged);
			m_lyricsHelper.StatusChanged += new EventHandler(m_lyricsHelper_StatusChanged);
		}

		protected override void InitPlayer()
		{
			Player.SongOpened += new EventHandler<ThreePM.MusicPlayer.SongEventArgs>(Player_SongOpened);

			if (Player.CurrentSong != null)
			{
				LoadLyrics(Player.CurrentSong);
			}
		}

		protected override void UnInitPlayer()
		{
			Player.SongOpened -= new EventHandler<ThreePM.MusicPlayer.SongEventArgs>(Player_SongOpened);
		}

		void Player_SongOpened(object sender, ThreePM.MusicPlayer.SongEventArgs e)
		{
			LoadLyrics(e.Song);
		}

		private void LoadLyrics(ThreePM.MusicPlayer.SongInfo songInfo)
		{
			m_lastSong = songInfo;
			ThreePM.MusicLibrary.LibraryEntry entry = Library.GetSong(songInfo.FileName) as ThreePM.MusicLibrary.LibraryEntry;
			if (entry != null && !String.IsNullOrEmpty(entry.Lyrics))
			{
				m_lyricsHelper.CancelLastRequest();
				SetLyricsTextBox(entry.Lyrics);
				btnGo.Enabled = false;
				txtURL.Text = "Internal";
			}
			else
			{
				m_lyricsHelper.LoadLyrics(songInfo);
			}
		}

		private void btnGo_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(txtURL.Text);
		}

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			m_lyricsHelper.LoadLyrics(m_lastSong, true, false, true);
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			Library.SetLyrics(m_lastSong.Title, m_lastSong.Artist, txtLyrics.Text);
			ThreePM.Utilities.LyricsHelper.SaveLyricsFile(m_lastSong, txtLyrics.Text);
		}

		private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.TopMost = alwaysOnTopToolStripMenuItem.Checked;
			Registry.SetValue("LyricsForm.TopMost", TopMost);
		}		
	}
}

