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

namespace starH45.net.mp3
{
	public partial class LyricsForm : starH45.net.mp3.BaseForm
	{
		private starH45.net.mp3.utilities.LyricsHelper m_lyricsHelper;
		private starH45.net.mp3.player.SongInfo m_lastSong;

		public LyricsForm()
		{
			InitializeComponent();
		
			TopMost = Utilities.GetValue("LyricsForm.TopMost", false);
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

		void m_lyricsHelper_LyricsFound(object sender, starH45.net.mp3.utilities.LyricsFoundEventArgs e)
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
			m_lyricsHelper = new starH45.net.mp3.utilities.LyricsHelper(this.Library);
			m_lyricsHelper.LyricsFound += new EventHandler<starH45.net.mp3.utilities.LyricsFoundEventArgs>(m_lyricsHelper_LyricsFound);
			m_lyricsHelper.CurrentURLChanged += new EventHandler(m_lyricsHelper_CurrentURLChanged);
			m_lyricsHelper.StatusChanged += new EventHandler(m_lyricsHelper_StatusChanged);
		}

		protected override void InitPlayer()
		{
			Player.SongOpened += new EventHandler<starH45.net.mp3.player.SongEventArgs>(Player_SongOpened);

			if (Player.CurrentSong != null)
			{
				LoadLyrics(Player.CurrentSong);
			}
		}

		protected override void UnInitPlayer()
		{
			Player.SongOpened -= new EventHandler<starH45.net.mp3.player.SongEventArgs>(Player_SongOpened);
		}

		void Player_SongOpened(object sender, starH45.net.mp3.player.SongEventArgs e)
		{
			LoadLyrics(e.Song);
		}

		private void LoadLyrics(starH45.net.mp3.player.SongInfo songInfo)
		{
			m_lastSong = songInfo;
			starH45.net.mp3.library.LibraryEntry entry = Library.GetSong(songInfo.FileName) as starH45.net.mp3.library.LibraryEntry;
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
			starH45.net.mp3.utilities.LyricsHelper.SaveLyricsFile(m_lastSong, txtLyrics.Text);
		}

		private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.TopMost = alwaysOnTopToolStripMenuItem.Checked;
			Utilities.SetValue("LyricsForm.TopMost", TopMost);
		}		
	}
}

