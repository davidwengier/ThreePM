using System;
using System.IO;
using System.Windows.Forms;

namespace ThreePM
{
	public partial class OptionsForm : BaseForm
	{
		#region Declarations

		private bool m_initialising = false;
		private MainForm frmMain;

		#endregion

		#region Constructor

		public OptionsForm()
		{
			m_initialising = true;

			InitializeComponent();

			chkLibrary_Click(this, EventArgs.Empty);

			chkShowTree.Checked = Registry.GetValue("LibraryForm.ShowTree", true);
			chkShowAlbumArt.Checked = Registry.GetValue("LibraryForm.ShowAlbumArt", true);
			rdoChangeToNowPlaying.Checked = Registry.GetValue("LibraryForm.NowPlayingAfterPlay", false);
			chkAutoChangeToPlaylist.Checked = Registry.GetValue("LibraryForm.PlaylistAfterQueue", false);
			rdoTrackInLibrary.Checked = Registry.GetValue("LibraryForm.TrackNowPlayingInTree", true);
			chkContributing.Checked = Registry.GetValue("LibraryForm.TrackContributing", false);

			chkHideIgnoredSongs.Checked = Registry.GetValue("LibraryForm.HideIgnoredSongs", false);

			if (!rdoChangeToNowPlaying.Checked && !rdoTrackInLibrary.Checked)
			{
				rdoNothing.Checked = true;
			}
			else
			{
				rdoNothing.Checked = false;
			}
			rdoByArtist.Checked = Registry.GetValue("LibraryForm.TrackByArtist", false);
			rdoByAlbum.Checked = Registry.GetValue("LibraryForm.TrackByAlbum", true);
			txtAlbumArtSize.Text = Registry.GetValue("LibraryForm.AlbumArtSize", 100).ToString();

			txtCustomRightClickOptionCommand.Text = Registry.GetValue("General.CustomRightClickOptionCommand", "");
			txtCustomRightClickOptionName.Text = Registry.GetValue("General.CustomRightClickOptionName", "");

			cboVisualization.Items.Add("(None)");

			cboVisualization.Items.Add("Spectrum");
			cboVisualization.Items.Add("Wave Form");
			cboVisualization.Items.Add("Spectrum - Wave");
			cboVisualization.Items.Add("Spectrum - ThreePM");
			cboVisualization.Items.Add("Spectrum - Song Title");
			cboVisualization.Items.Add("Spectrum - Bars");
			cboVisualization.Items.Add("Spectrum - Bars and Peaks");
			cboVisualization.Items.Add("Spectrum - Beans");
			cboVisualization.Items.Add("Spectrum - Dots");
			cboVisualization.Items.Add("Spectrum - Ellipses");

			cboSkin.Items.Add("<Default>");
			string skinDirectory = Application.StartupPath;
			if (!skinDirectory.EndsWith("\\"))
			{
				skinDirectory += "\\";
			}
			skinDirectory += "Skins\\";
			foreach (string file in Directory.GetFiles(skinDirectory, "*.skin.xml"))
			{
				cboSkin.Items.Add(Path.GetFileName(file));
			}

			cboSkin.SelectedIndex = cboSkin.FindString(Registry.GetValue("BaseForm.Skin", "Default.skin.xml"));

			if (cboSkin.SelectedIndex == -1)
			{
				cboSkin.SelectedIndex = 0;
			}

			foreach (Form f in Application.OpenForms)
			{
				if (f is MainForm)
				{
					frmMain = (MainForm)f;
				}
			}

			cboVisualization.SelectedIndex = frmMain.VisualizationNumber;
			chkHighQuality.Checked = frmMain.VisualizationHighQuality;
			chkFullSpectrum.Checked = frmMain.VisualizationFullSpectrum;
			txtVisualizationSpeed.Text = frmMain.VisualizationSpeed.ToString();

			chkNotificationForm.Checked = frmMain.ShowNotifyForm;
			chkToaster.Checked = frmMain.ShowToasterForm;
			txtToasterStayDelay.Text = Registry.GetValue("ToasterForm.StayDelay", 2000).ToString();
			txtToasterFadeDelay.Text = Registry.GetValue("ToasterForm.FadeDelay", 50).ToString();

            txtFormatString.Text = ThreePM.MusicPlayer.Player.SongInfoFormatString;
            txtSecondsBeforeUpdatePlayCount.Text = ThreePM.MusicPlayer.Player.SecondsBeforeUpdatePlayCount.ToString();
            chkIgnorePlayed.Checked = ThreePM.MusicPlayer.Player.IgnorePreviouslyPlayedSongsInRandomMode;
            rdoEver.Checked = ThreePM.MusicPlayer.Player.NeverPlayIgnoredSongs;
            rdoRandomly.Checked = !ThreePM.MusicPlayer.Player.NeverPlayIgnoredSongs;
			txtNumRecent.Text = Registry.GetValue("LibraryForm.SongsToShowInRecentLists", 50).ToString();

            chkEnableAudioScrobbler.Checked = ThreePM.MusicPlayer.Player.AudioscrobblerEnabled;
            txtAudioscrobblerUsername.Text = ThreePM.MusicPlayer.Player.AudioscrobblerUserName;
            txtAudioscrobblerPassword.Text = ThreePM.MusicPlayer.Player.AudioscrobblerPassword;

			m_initialising = false;
		}

		#endregion

		#region Overridden Methods

		protected override void InitLibrary()
		{
			lstWatchFolders.Items.Clear();
			foreach (string dir in Library.GetWatchFolders())
			{
				lstWatchFolders.Items.Add(dir);
			}

			lstRadio.Items.Clear();
			foreach (ThreePM.MusicLibrary.LibraryEntry entry in Library.GetInternetRadios())
			{
				lstRadio.Items.Add(entry.FileName);
			}
		}

		protected override void InitPlayer()
		{
			foreach (string s in ThreePM.MusicPlayer.Player.GetDevices())
			{
                cboDevice.Items.Add(s);
			}
			m_initialising = true;
			cboDevice.SelectedIndex = Player.DeviceNumber;
			m_initialising = false;
		}

		#endregion

		#region Control Events

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				lstWatchFolders.Items.Add(dlg.SelectedPath);
				Library.AddWatchFolder(dlg.SelectedPath);
			}
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			string dir = lstWatchFolders.Text;
			lstWatchFolders.Items.RemoveAt(lstWatchFolders.SelectedIndex);
			Library.RemoveWatchFolder(dir);
		}

		private void LibraryOptions_CheckedChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;

			Registry.SetValue("LibraryForm.ShowTree", chkShowTree.Checked);
			Registry.SetValue("LibraryForm.ShowAlbumArt", chkShowAlbumArt.Checked);
			Registry.SetValue("LibraryForm.NowPlayingAfterPlay", rdoChangeToNowPlaying.Checked);
			Registry.SetValue("LibraryForm.PlaylistAfterQueue", chkAutoChangeToPlaylist.Checked);
			Registry.SetValue("LibraryForm.TrackNowPlayingInTree", rdoTrackInLibrary.Checked);
			Registry.SetValue("LibraryForm.TrackByArtist", rdoByArtist.Checked);
			Registry.SetValue("LibraryForm.TrackByAlbum", rdoByAlbum.Checked);
			Registry.SetValue("LibraryForm.HideIgnoredSongs", chkHideIgnoredSongs.Checked);

			Registry.SetValue("LibraryForm.TrackContributing", chkContributing.Checked);

			foreach (Form f in Application.OpenForms)
			{
				if (f is LibraryForm)
				{
					((LibraryForm)f).LoadRegistrySettings();
				}
			}
		}

		private void cboDevice_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			Player.DeviceNumber = cboDevice.SelectedIndex;
		}

		private void cboVisualization_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			frmMain.VisualizationNumber = cboVisualization.SelectedIndex;
		}

		private void txtVisualizationSpeed_TextChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			int i = 10;
			int.TryParse(txtVisualizationSpeed.Text, out i);
			if (i < 10) i = 10;
			frmMain.VisualizationSpeed = i;
		}

		private void chkHighQuality_CheckedChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			frmMain.VisualizationHighQuality = chkHighQuality.Checked;
		}

		private void cboSkin_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;

			Registry.SetValue("BaseForm.Skin", cboSkin.Text);
			foreach (Form form in Application.OpenForms)
			{
				if (form is BaseForm) (form as BaseForm).ReloadSkin(true);
			}
		}

		private void chkFullSpectrum_CheckedChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			frmMain.VisualizationFullSpectrum = chkFullSpectrum.Checked;
		}

		private void chkToaster_CheckedChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			frmMain.ShowToasterForm = chkToaster.Checked;
		}

		private void chkNotificationForm_CheckedChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			frmMain.ShowNotifyForm = chkNotificationForm.Checked;
		}

		private void txtFormatString_TextChanged(object sender, EventArgs e)
		{
            ThreePM.MusicPlayer.Player.SongInfoFormatString = txtFormatString.Text;
			Registry.SetValue("Player.SongInfoFormatString", txtFormatString.Text);
		}


		private void txtAlbumArtSize_TextChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			int i = 100;
			int.TryParse(txtAlbumArtSize.Text, out i);
			if (i < 10) i = 10;

			Registry.SetValue("LibraryForm.AlbumArtSize", i);

			foreach (Form f in Application.OpenForms)
			{
				if (f is LibraryForm)
				{
					((LibraryForm)f).LoadRegistrySettings();
				}
			}
		}

		private void chkAndThenSome_Click(object sender, EventArgs e)
		{
			chkWatchFolders.Checked = false;
			chkLibrary.Checked = false;
			chkTheRest.Checked = false;
			chkAndThenSome.Checked = true;
			pnlMoreMoreMore.BringToFront();
		}

		private void chkWatchFolders_Click(object sender, EventArgs e)
		{
			chkWatchFolders.Checked = true;
			chkLibrary.Checked = false;
			chkTheRest.Checked = false;
			chkAndThenSome.Checked = false;
			pnlWatchFolders.BringToFront();
		}

		private void chkLibrary_Click(object sender, EventArgs e)
		{
			chkWatchFolders.Checked = false;
			chkLibrary.Checked = true;
			chkTheRest.Checked = false;
			chkAndThenSome.Checked = false;
			pnlLibrary.BringToFront();
		}

		private void chkTheRest_Click(object sender, EventArgs e)
		{
			chkWatchFolders.Checked = false;
			chkLibrary.Checked = false;
			chkTheRest.Checked = true;
			chkAndThenSome.Checked = false;
			pnlFuckKnows.BringToFront();
		}

		private void txtToasterStayDelay_TextChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			int i = 1000;
			int.TryParse(txtToasterStayDelay.Text, out i);
			if (i < 10) i = 10;
			Registry.SetValue("ToasterForm.StayDelay", i);
		}

		private void txtToasterFadeDelay_TextChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			int i = 1000;
			int.TryParse(txtToasterFadeDelay.Text, out i);
			if (i < 10) i = 10;
			Registry.SetValue("ToasterForm.FadeDelay", i);
		}

		private void btnClearPlayCount_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure?\n\n" + (sender as Button).Tag.ToString(), "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
			Library.ClearAllPlayCounts();
		}

		private void btnClearDateAdded_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure?\n\n" + (sender as Button).Tag.ToString(), "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
			Library.ClearAllDateAddeds();
			Library.RefreshLibrary();
		}

		private void btnQueueMissingTagSongs_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure?\n\n" + (sender as Button).Tag.ToString(), "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
			Player.Playlist.Clear();
			Player.Playlist.AddToEnd(Library.QueryLibrary(@"
					(Artist ISNULL OR Artist = '')
					OR (Title ISNULL OR Title = '')
					OR (Album ISNULL OR Album = '')
					OR (AlbumArtist ISNULL OR AlbumArtist = '')"));
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure?\n\n" + (sender as Button).Tag.ToString(), "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
			Player.Playlist.Clear();
			Player.Playlist.AddToEnd(Library.QueryLibrary(@"Lyrics ISNULL OR Lyrics = ''"));
		}

		private void txtSecondsBeforeUpdatePlayCount_TextChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			int i = 20;
			int.TryParse(txtSecondsBeforeUpdatePlayCount.Text, out i);
			if (i < 1) i = 1;
			Registry.SetValue("Player.SecondsBeforeUpdatePlayCount", i);
            ThreePM.MusicPlayer.Player.SecondsBeforeUpdatePlayCount = i;
		}

		private void chkIgnorePlayed_CheckedChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			Registry.SetValue("Player.IgnorePreviouslyPlayedSongsInRandomMode", chkIgnorePlayed.Checked);
            ThreePM.MusicPlayer.Player.IgnorePreviouslyPlayedSongsInRandomMode = chkIgnorePlayed.Checked;
		}

		private void txtNumRecent_TextChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			int i = 50;
			int.TryParse(txtNumRecent.Text, out i);
			if (i < 1) i = 1;
			Registry.SetValue("LibraryForm.SongsToShowInRecentLists", i);

			foreach (Form f in Application.OpenForms)
			{
				if (f is LibraryForm)
				{
					((LibraryForm)f).LoadRegistrySettings();
				}
			}
		}

		private void rdoEver_CheckedChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			Registry.SetValue("Player.NeverPlayIgnoredSongs", rdoEver.Checked);
            ThreePM.MusicPlayer.Player.NeverPlayIgnoredSongs = rdoEver.Checked;
		}

		private void txtCustomRightClickOption_TextChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			Registry.SetValue("General.CustomRightClickOptionCommand", txtCustomRightClickOptionCommand.Text);
			Registry.SetValue("General.CustomRightClickOptionName", txtCustomRightClickOptionName.Text);
		}

		private void btnLyrics_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure?\n\n" + (sender as Button).Tag.ToString(), "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
			LyricsSearcherForm f = new LyricsSearcherForm();
			f.Library = Library;
			f.Show(this.Owner);
		}

		private void btnSaveLyrics_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure?\n\n" + (sender as Button).Tag.ToString(), "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
			Application.UseWaitCursor = true;
			ThreePM.MusicLibrary.LibraryEntry[] entries = Library.QueryLibrary("Lyrics <> '' AND Lyrics IS NOT NULL");
			foreach (ThreePM.MusicLibrary.LibraryEntry entry in entries)
			{
				ThreePM.Utilities.LyricsHelper.SaveLyricsFile(entry as ThreePM.MusicPlayer.SongInfo, entry.Lyrics);
			}
			Application.UseWaitCursor = false;
			MessageBox.Show("Saved " + entries.Length + " lyrics files.");
		}

		private void btnLyricsFromLyricsFiles_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure?\n\n" + (sender as Button).Tag.ToString(), "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
			LyricsSearcherForm f = new LyricsSearcherForm();
			f.OnlyLyricsFile = true;
			f.Library = Library;
			f.Show(this.Owner);
		}

		private void btnAssociate_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure?\n\n" + (sender as Button).Tag.ToString(), "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
			Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\explorer\\AppKey", true);
			key.DeleteSubKey("16");
			key.CreateSubKey("16");
			key = key.OpenSubKey("16", true);
			key.SetValue("ShellExecute", Application.ExecutablePath);
		}

		private void chkEnableAudioScrobbler_CheckedChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			Registry.SetValue("Player.AudioscrobblerEnabled", chkEnableAudioScrobbler.Checked);
            ThreePM.MusicPlayer.Player.AudioscrobblerEnabled = chkEnableAudioScrobbler.Checked;
		}

		private void txtAudioscrobblerUsername_TextChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			Registry.SetValue("Player.AudioscrobblerUserName", txtAudioscrobblerUsername.Text);
            ThreePM.MusicPlayer.Player.AudioscrobblerUserName = txtAudioscrobblerUsername.Text;
		}

		private void txtAudioscrobblerPassword_TextChanged(object sender, EventArgs e)
		{
			if (m_initialising) return;
			Registry.SetValue("Player.AudioscrobblerPassword", txtAudioscrobblerPassword.Text);
            ThreePM.MusicPlayer.Player.AudioscrobblerPassword = txtAudioscrobblerPassword.Text;
		}

		#endregion

		private void btnRadioAdd_Click(object sender, EventArgs e)
		{
			lstRadio.Items.Add(txtRadio.Text);
			Library.AddInternetRadio(txtRadio.Text);
		}

		private void btnRadioDelete_Click(object sender, EventArgs e)
		{
			if (lstRadio.SelectedIndex >= 0)
			{
				Library.DeleteInternetRadio(lstRadio.Text);
			}
		}
	}
}
