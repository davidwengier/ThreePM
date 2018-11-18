using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ThreePM.MusicPlayer;
using System.Runtime.InteropServices;
using ThreePM.MusicLibrary;
using System.Text.RegularExpressions;
//using Windows7.DesktopIntegration.WindowsForms;
//using Windows7.DesktopIntegration;

namespace ThreePM
{
    public partial class MainForm : BaseForm
    {
		#region Win32 Stuff

		public static Dictionary<string, Keys> Commands = new Dictionary<string, Keys>()
		{
			{ "/next", Keys.MediaNextTrack },
			{ "/play", Keys.MediaPlayPause },
			{ "/pause", Keys.MediaPlayPause },
			{ "/prev", Keys.MediaPreviousTrack},
			{ "/stop", Keys.MediaStop }
		};

		private const int WM_HOTKEY = 0x0312;
		private const int WM_USER = 0x400;

		public static int MOD_ALT = 0x1;
		public static int MOD_CONTROL = 0x2;
		public static int MOD_SHIFT = 0x4;
		public static int MOD_WIN = 0x8;

		[DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterHotKey")]
		private static extern bool User32_RegisterHotKey(System.IntPtr hWnd, int id, int fsModifiers, System.Windows.Forms.Keys vk);
		[DllImport("user32.dll", SetLastError = true, EntryPoint = "UnregisterHotKey")]
		private static extern bool User32_UnregisterHotKey(System.IntPtr hWnd, int id);

		#endregion

        #region Declarations

		//private string tempPlayList;
		private bool m_visualizationFullSpectrum = true;
		private bool m_visualizationHighQuality = true;
		private int m_visualizationNumber = 1;
		//private ThreePM.utilities.HttpServer m_server;
        private bool m_firstSongCountChanged = true;
        private bool m_showRemaining ;
		private Keys m_ignoreAndNextKey = (Keys.Z | Keys.Control | Keys.Shift);

		private NotifyForm m_notifyForm;
		private ToasterForm m_toasterForm;

        #endregion

		#region Properties

		protected override Control.ControlCollection DynamicControlsContainer
		{
			get { return ControlsPanel.Controls; }
		}

		#endregion

		#region Constructor

		public MainForm()
        {
			//Windows7Taskbar.AllowTaskbarWindowMessagesThroughUIPI();
			//Windows7Taskbar.SetCurrentProcessAppId("ThreePM");

			//tempPlayList = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ThreePM.m3u";

            InitializeComponent();

			if (!DesignMode)
			{
				ThreePM.Engine.Main.Initialize();
				Player = ThreePM.Engine.Main.Player;
				Library = ThreePM.Engine.Main.Library;
				Library.SynchronizingObject = this;
				Player.SynchronizingObject = this;

				//Player = new Player();
				//Library = new Library();

				RegisterHotKey(Keys.SelectMedia);
				RegisterHotKey(Keys.MediaPlayPause);
				RegisterHotKey(Keys.MediaNextTrack);
				RegisterHotKey(Keys.MediaPreviousTrack);
				RegisterHotKey(Keys.MediaStop);

				RegisterHotKey(m_ignoreAndNextKey);

				//Player.SongInfoFormatString = Utilities.GetValue("Player.SongInfoFormatString", "{Artist} - {Title}").ToString();
				//Player.SecondsBeforeUpdatePlayCount = Utilities.GetValue("Player.SecondsBeforeUpdatePlayCount", 20);
				//Player.IgnorePreviouslyPlayedSongsInRandomMode = Utilities.GetValue("Player.IgnorePreviouslyPlayedSongsInRandomMode", false);
				//Player.NeverPlayIgnoredSongs = Utilities.GetValue("Player.NeverPlayIgnoredSongs", false);

				Library.ScanStarting += new EventHandler(library_ScanStarting);
				Library.ScanFinished += new EventHandler(lib_ScanFinished);
				Library.SongCountChanged += new EventHandler(library_SongCountChanged);
				//Library.SynchronizingObject = this;

				Player.PositionChanged += new EventHandler(player_PositionChanged);
				Player.PositionDescriptionChanged += new EventHandler(player_PositionDescriptionChanged);
				Player.SongOpened += new System.EventHandler<SongEventArgs>(player_SongOpened);
				Player.StateChanged += new System.EventHandler(player_StateChanged);
				Player.Playlist.PlaylistStyleChanged += new EventHandler(Playlist_PlaylistStyleChanged);
				//Player.RepeatCurrentTrack = Convert.ToBoolean(Utilities.GetValue("Player.RepeatCurrentTrack", false));
				//Player.Playlist.PlaylistStyle = (PlaylistStyle)Convert.ToInt32(Utilities.GetValue("Player.PlaylistStyle", 0));
				//Player.SynchronizingObject = this;
				//Player.Library = Library;

				ThreePM.Engine.Main.Start();

				ShowToasterForm = Convert.ToBoolean(Registry.GetValue("MainForm.ShowToasterForm", true));
				ShowNotifyForm = Convert.ToBoolean(Registry.GetValue("MainForm.ShowNotifyForm", false));
				m_showRemaining = Convert.ToBoolean(Registry.GetValue("MainForm.ShowRemaining", false));
				m_visualizationNumber = Convert.ToInt32(Registry.GetValue("MainForm.VisualizationNumber", 1));
				m_visualizationHighQuality = Convert.ToBoolean(Registry.GetValue("MainForm.VisualizationHighQuality", true));
				m_visualizationFullSpectrum = Convert.ToBoolean(Registry.GetValue("MainForm.VisualizationFullSpectrum", true));
				tmrSpectrum.Interval = Convert.ToInt32(Registry.GetValue("MainForm.VisualizationSpeed", 50));
				chkOnTop.Checked = Convert.ToBoolean(Registry.GetValue("MainForm.OnTop", true));
				this.TopMost = chkOnTop.Checked;

				tckVolume.Duration = 100F;
				tckBalance.Duration = 200F;
				chkRepeat.Checked = Player.RepeatCurrentTrack;
                tckVolume.Position = Convert.ToSingle(Player.Volume);
				tckBalance.Position = Convert.ToSingle(Player.Balance + 100);

				//// load the last song that was being played
				//string file = Utilities.GetValue("Player.CurrentSong", "").ToString();
				//if (!String.IsNullOrEmpty(file))
				//{
				//    // don't count the play since we're just restarting the same song
				//    float position = Convert.ToSingle(Utilities.GetValue("Player.Position", 0f));
				//    if (Player.LoadFile(file, Convert.ToInt32(position) <= Player.SecondsBeforeUpdatePlayCount))
				//    {
				//        Player.Position = position;
				//        Player.Play();
				//    }
				//}

				
				this.Visible = true;

				Application.DoEvents();

				//Player.Playlist.LoadFromFile(tempPlayList);

				if (Convert.ToBoolean(Registry.GetValue("LibraryForm.Show", false)))
				{
					Invoke((MethodInvoker)delegate { showLibraryToolStripMenuItem.Checked = true; });
					ShowForm<LibraryForm>(showLibraryToolStripMenuItem);
				}
				// now show song info
				if (Convert.ToBoolean(Registry.GetValue("InfoForm.Show", false)))
				{
					Invoke((MethodInvoker)delegate { trackInformationToolStripMenuItem.Checked = true; });
					ShowForm<InfoForm>(trackInformationToolStripMenuItem);
				}

				if (Convert.ToBoolean(Registry.GetValue("LyricsForm.Show", false)))
				{
					Invoke((MethodInvoker)delegate { showLyricsToolStripMenuItem.Checked = true; });
					ShowForm<LyricsForm>(showLyricsToolStripMenuItem);
				}

				if (Convert.ToBoolean(Registry.GetValue("PlaylistForm.Show", false)))
				{
					Invoke((MethodInvoker)delegate { playlistToolStripMenuItem.Checked = true; });
					ShowForm<PlaylistForm>(playlistToolStripMenuItem);
				}

				if (Convert.ToBoolean(Registry.GetValue("AlbumArtForm.Show", false)))
				{
					Invoke((MethodInvoker)delegate { showAlbumArtToolStripMenuItem.Checked = true; });
					ShowForm<AlbumArtForm>(showAlbumArtToolStripMenuItem);
				}

				if (Convert.ToBoolean(Registry.GetValue("AlbumListForm.Show", false)))
				{
					Invoke((MethodInvoker)delegate { showAlbumListToolStripMenuItem.Checked = true; });
					ShowForm<AlbumListForm>(showAlbumListToolStripMenuItem);
				}

				if (Convert.ToBoolean(Registry.GetValue("EqualizerForm.Show", false)))
				{
					Invoke((MethodInvoker)delegate { showEqualizerToolStripMenuItem.Checked = true; });
					ShowForm<EqualizerForm>(showEqualizerToolStripMenuItem);
				}

				//if (Convert.ToBoolean(Utilities.GetValue("MainForm.HttpServer", false)))
				//{
				//    m_server = new ThreePM.utilities.HttpServer(Player, Library);
				//    Invoke((MethodInvoker)delegate { enableHttpServerToolStripMenuItem.Checked = true; });
				//}
				enableHttpServerToolStripMenuItem.Checked = ThreePM.Engine.Main.HttpServerEnabled;
			}
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
				ThreePM.Engine.Main.End();
				//if (m_server != null)
				//{
				//    m_server.Dispose();
				//    m_server = null;
				//}
				//Player.Dispose();
				//Library.Dispose();
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

		#region Properties

		public bool ShowToasterForm
		{
			get { return m_toasterForm != null; }
			set
			{
				Registry.SetValue("MainForm.ShowToasterForm", value);
				if (value)
				{
					m_toasterForm = new ToasterForm();
					m_toasterForm.Player = Player;
				}
				else
				{
					if (m_toasterForm != null)
					{
						m_toasterForm.Close();
						m_toasterForm.Dispose();
						m_toasterForm = null;
					}
				}
			}
		}

		public bool ShowNotifyForm
		{
			get { return m_notifyForm != null; }
			set
			{
				Registry.SetValue("MainForm.ShowNotifyForm", value);
				if (value)
				{
					m_notifyForm = new NotifyForm();
					m_notifyForm.NextSong += new EventHandler(NotifyForm_NextSong);
					m_notifyForm.ShowPlayer += new EventHandler(NotifyForm_ShowPlayer);
					if (Player != null && Player.CurrentSong != null)
					{
						m_notifyForm.Show(Player.CurrentSong);
					}
				}
				else
				{
					if (m_notifyForm != null)
					{
						m_notifyForm.NextSong -= new EventHandler(NotifyForm_NextSong);
						m_notifyForm.ShowPlayer -= new EventHandler(NotifyForm_ShowPlayer);
						m_notifyForm.Close();
						m_notifyForm.Dispose();
						m_notifyForm = null;
					}
				}
			}
		}

		public bool VisualizationHighQuality
		{
			get { return m_visualizationHighQuality; }
			set
			{
				m_visualizationHighQuality = value;
				Registry.SetValue("MainForm.VisualizationHighQuality", value);
			}
		}

		public bool VisualizationFullSpectrum
		{
			get { return m_visualizationFullSpectrum; }
			set
			{
				m_visualizationFullSpectrum = value;
				Registry.SetValue("MainForm.VisualizationFullSpectrum", value);
			}
		}

		public int VisualizationNumber
		{
			get { return m_visualizationNumber; }
			set
			{
				m_visualizationNumber = value;
				Registry.SetValue("MainForm.VisualizationNumber", m_visualizationNumber);
				tmrSpectrum.Start();
			}
		}

		public int VisualizationSpeed
		{
			get { return tmrSpectrum.Interval; }
			set
			{
				tmrSpectrum.Interval = value;
				Registry.SetValue("MainForm.VisualizationSpeed", value);
			}
		}
	

		#endregion

		#region Library Events

		void library_ScanStarting(object sender, EventArgs e)
        {
            prgSpin.Start();
        }

        private void lib_ScanFinished(object sender, EventArgs e)
        {
            prgSpin.Stop();
        }

        void library_SongCountChanged(object sender, EventArgs e)
        {
            lblSongCount.Text = Library.SongCount + " songs";
            if (m_firstSongCountChanged)
            {
                m_firstSongCountChanged = false;
                Player.Play();
            }
        }

        #endregion

        #region Player Events

		private void Playlist_PlaylistStyleChanged(object sender, EventArgs e)
		{
			switch (Player.Playlist.PlaylistStyle)
			{
				case PlaylistStyle.Normal:
				{
					ShowStatus("Playlist Style: Normal");
					break;
				}
				case PlaylistStyle.Random:
				{
					ShowStatus("Playlist Style: Random");
					break;
				}
				case PlaylistStyle.Looping:
				{
					ShowStatus("Playlist Style: Looping");
					break;
				}
				case PlaylistStyle.RandomLooping:
				{
					ShowStatus("Playlist Style: Random Looping");
					break;
				}
			}
			Registry.SetValue("Player.PlaylistStyle", (int)Player.Playlist.PlaylistStyle);
		}

        private void player_PositionChanged(object sender, System.EventArgs e)
        {
            tckPosition.Position = Player.Position;
        }

        private void player_PositionDescriptionChanged(object sender, EventArgs e)
        {
            lblPosition.Text = (m_showRemaining ? Player.RemainingDescription : Player.PositionDescription);
        }

        private void player_SongOpened(object sender, SongEventArgs e)
        {
			if (e.Song.FileName.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
			{
				Library.UpdateInternetRadio(e.Song.FileName, e.Song.Album);
			}
			else
			{
				LibraryEntry entry = Library.GetSong(e.Song.FileName) as LibraryEntry;
				if (entry != null)
				{
					Library.UpdateIfNeeded(e.Song.FileName);

					string playNum = (entry.PlayCount + 1).ToString();
					string suffix = "th";
					if (playNum.EndsWith("1") && !playNum.EndsWith("11"))
					{
						suffix = "st";
					}
					else if (playNum.EndsWith("2") && !playNum.EndsWith("12"))
					{
						suffix = "nd";
					}
					else if (playNum.EndsWith("3") && !playNum.EndsWith("13"))
					{
						suffix = "rd";
					}
					ShowStatus(playNum + suffix + " time played.");
				}
			}
			this.Text = e.Song.ToString();
			lblTitle.Text = e.Song.Title;
			lblArtist.Text = e.Song.Artist;
			tckPosition.Duration = e.Song.Duration;
			if (m_notifyForm != null)
			{
				m_notifyForm.Show(e.Song);
			}
		}

        private void player_StateChanged(object sender, EventArgs e)
        {
			btnPlay.Checked = Player.State == PlayerState.Playing;
			btnPause.Checked = Player.State == PlayerState.Paused;
			btnStop.Checked = Player.State == PlayerState.Stopped;

			if (m_visualizationNumber != 0)
			{
				if (Player.State == PlayerState.Playing)
				{
					tmrSpectrum.Start();
				}
				else
				{
					tmrSpectrum.Stop();
					pctSpectrum.Image = null;
				}
			}
        }

        #endregion

		#region Notify Form Events

		private void NotifyForm_NextSong(object sender, EventArgs e)
		{
			ShowStatus("Next");
			Player.Next();
		}

		private void NotifyForm_ShowPlayer(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Normal;
			this.BringToFront();
		}

		#endregion Notify Form Events

		#region Form Events

		private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Player.PlayFile(files[0]);
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.Link;
            }
        }

        private void frmPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
			// stop the player, so the Playlist can't change
			//Player.Pause();
			//File.Delete(tempPlayList);
			//Player.Playlist.SaveToFile(tempPlayList);

			//if (Player.CurrentSong != null)
			//{
			//    Utilities.SetValue("Player.CurrentSong", Player.CurrentSong.FileName);
			//    Utilities.SetValue("Player.Position", Player.Position);
			//}

			UnregisterHotKey(Keys.SelectMedia);
			UnregisterHotKey(Keys.MediaPlayPause);
			UnregisterHotKey(Keys.MediaNextTrack);
			UnregisterHotKey(Keys.MediaPreviousTrack);
			UnregisterHotKey(Keys.MediaStop);
			UnregisterHotKey(m_ignoreAndNextKey);
        }

        #endregion

        #region Key Handling Methods

		private Dictionary<Keys, bool> hotKeySet = new Dictionary<Keys, bool>();

		public void RegisterHotKey(System.Windows.Forms.Keys key)
		{
			if (hotKeySet.ContainsKey(key) && hotKeySet[key])
			{
				UnregisterHotKey(key);
			}

			hotKeySet[key] = User32_RegisterHotKey(Handle, 1000 + Convert.ToInt32(key), 0, key);
		}

		public void UnregisterHotKey(Keys key)
		{
			if (hotKeySet.ContainsKey(key) && hotKeySet[key])
			{
				hotKeySet[key] = !User32_UnregisterHotKey(Handle, 1000 + Convert.ToInt32(key));
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_HOTKEY)
			{
				// MAGIC NUMBER!!!
				HandleKey((Keys)(m.WParam.ToInt32() - 1000));
			}
			else if (m.Msg == WM_USER)
			{
				// less magic
				HandleKey((Keys)(m.LParam.ToInt32()));
			}
			//else if (m.Msg == Windows7Taskbar.TaskbarButtonCreatedMessage)
			//{
			//	JumpListManager manager = this.CreateJumpListManager();
			//	manager.UserRemovedItems += (o, e) =>
			//	{
			//		// we have to hook this up, but i dont care
			//	};
			//	manager.ClearAllDestinations();

			//	string myExe = System.Reflection.Assembly.GetEntryAssembly().Location;
				
			//	manager.AddCustomDestination(new ShellLink()
			//	{
			//		Category = "Controls",
			//		Path = myExe,
			//		Title = "Previous",
			//		IconLocation = myExe,
			//		IconIndex = 7,
			//		Arguments = "/prev"
			//	});
			//	manager.AddCustomDestination(new ShellLink()
			//	{
			//		Category = "Controls",
			//		Path = myExe,
			//		Title = "Play",
			//		IconLocation = myExe,
			//		IconIndex = 5,
			//		Arguments = "/play"
			//	});
			//	manager.AddCustomDestination(new ShellLink()
			//	{
			//		Category = "Controls",
			//		Path = myExe,
			//		Title = "Pause",
			//		IconLocation = myExe,
			//		IconIndex = 4,
			//		Arguments = "/pause"
			//	});
			//	manager.AddCustomDestination(new ShellLink()
			//	{
			//		Category = "Controls",
			//		Path = myExe,
			//		Title = "Stop",
			//		IconLocation = myExe,
			//		IconIndex = 10,
			//		Arguments = "/stop"
			//	});
			//	manager.AddCustomDestination(new ShellLink()
			//	{
			//		Category = "Controls",
			//		Path = myExe,
			//		Title = "Next",
			//		IconLocation = myExe,
			//		IconIndex = 3,
			//		Arguments = "/next"
			//	});
			//	manager.EnabledAutoDestinationType = ApplicationDestinationType.Recent;
			//	manager.Refresh();
			//}
			else
			{
				base.WndProc(ref m);
			}
		}

        private bool HandleKey(Keys keyData)
        {
			if (keyData == Keys.MediaPlayPause)
			{
				if (Player.State == PlayerState.Paused || Player.State == PlayerState.Stopped)
				{
					Player.Play();
				}
				else
				{
					Player.Pause();
				}
				return true;
			}
			else if (keyData == Keys.MediaPreviousTrack)
			{
				Player.ForceSong(Player.CurrentSong.FileName);
				Player.Previous();
			}
			else if (keyData == Keys.MediaNextTrack)
			{
				Player.Next();
				return true;
			}
			else if (keyData == Keys.MediaStop)
			{
				Player.Stop();
				return true;
			}
			else if (keyData == Keys.SelectMedia)
			{
				// show jump to file box
				JumpToFile();
				return true;
			}
			else if (keyData == m_ignoreAndNextKey)
			{
				IgnoreAndNext();
				return true;
			}
			//else if (keyData == m_bruteScanKey)
			//{
			//    this.Library.BruteScan(this.Player);
			//}
			else if (keyData == (Keys.L | Keys.Control))
			{
				ShowForm<LyricsForm>(showLyricsToolStripMenuItem);
			}
			else if (keyData == (Keys.D | Keys.Control))
			{
				ShowForm<DeveloperForm>(null);
			}
			else if (keyData == (Keys.E | Keys.Control))
			{
				ShowForm<EqualizerForm>(showEqualizerToolStripMenuItem);
			}
			else if (keyData == Keys.F5)
			{
				Player.OnSongOpened();
			}

            return false;
        }

		protected override bool ProcessDialogKey(Keys keyData)
		{
			bool handled = false;

			switch (keyData)
			{
				case Keys.Space:
				{
					// Pause or unpause
					if (Player.State == PlayerState.Paused || Player.State == PlayerState.Stopped)
					{
						Player.Play();
					}
					else
					{
						Player.Pause();
					}

					handled = true;

					break;
				}
				case Keys.Left:
				{
					// Skip backwards 5 seconds
					tckPosition.SetPosition(tckPosition.Position - 5);
					break;
				}
				case Keys.Right:
				{
					// Skip forwards 5 seconds
					tckPosition.SetPosition(tckPosition.Position + 5);
					break;
				}
				case Keys.Up:
				{
					// Increase volume by 5
					tckVolume.SetPosition(tckVolume.Position + 5);
					break;
				}
				case Keys.Down:
				{
					// Decrease volume by 5
					tckVolume.SetPosition(tckVolume.Position - 5);
					break;
				}
			}

			if (!handled)
			{
				handled = base.ProcessDialogKey(keyData);
			}

			return handled;
		}

		private void IgnoreAndNext()
		{
			if (Player.CurrentSong != null)
			{
				Library.Ignore(Player.CurrentSong.FileName);
				Player.Next();
			}
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();
		[DllImport("kernel32.dll")]
		static extern uint GetCurrentThreadId();
		[DllImport("user32.dll")]
		static extern IntPtr SetFocus(IntPtr hWnd);

        private void JumpToFile()
        {
            foreach (Form f in OwnedForms)
            {
				if (f is SearchForm)
				{

					f.Activate();
					f.Select();
					return;
				}
            }
            ShowForm<SearchForm>(songSearchToolStripMenuItem);
			foreach (Form f in OwnedForms)
			{
				if (f is SearchForm)
				{
					uint nul;
					//Attach foreground window thread to our thread
					AttachThreadInput(GetWindowThreadProcessId(GetForegroundWindow(), out nul), GetCurrentThreadId(), true);

					//Do our stuff here ;-)
					SetForegroundWindow(f.Handle);
					SetFocus(f.Handle); //Just playing safe
					f.BringToFront();

					//Detach the attached thread
					AttachThreadInput(GetWindowThreadProcessId(GetForegroundWindow(), out nul), GetCurrentThreadId(), false);
					return;
				}
			}
        }

        #endregion

        #region Overridden Methods

        protected override void OnKeyDown(KeyEventArgs e)
        {
			e.Handled = HandleKey(e.KeyData);
            base.OnKeyDown(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
			tckVolume.SetPosition(tckVolume.Position + (e.Delta * SystemInformation.MouseWheelScrollLines / 120));
            base.OnMouseWheel(e);
        }

        #endregion

        #region Control Events

        private void btnPrevious_Click(object sender, System.EventArgs e)
        {
			if (Player.Position > 4)
			{
				Player.Position = 0;
				return;
			}
			ShowStatus("Previous");
            Player.ForceSong(Player.CurrentSong.FileName);
            Player.Previous();
        }

        private void btnPlay_Click(object sender, System.EventArgs e)
        {
			ShowStatus("Play");
			if (Player.State == PlayerState.Paused || Player.State == PlayerState.Stopped)
			{
				Player.Play();
			}
			else
			{
				Player.Pause();
			}
        }

        private void btnPause_Click(object sender, System.EventArgs e)
        {
			ShowStatus("Pause");
			if (Player.State == PlayerState.Paused || Player.State == PlayerState.Stopped)
			{
				Player.Play();
			}
			else
			{
				Player.Pause();
			}
        }

        private void btnStop_Click(object sender, System.EventArgs e)
        {
			ShowStatus("Stop");
			Player.Stop();
        }

        private void btnNext_Click(object sender, System.EventArgs e)
        {
			ShowStatus("Next");
			Player.Next();
        }

        private void tckPosition_PositionChanged(object sender, System.EventArgs e)
        {
            Player.Position = tckPosition.Position;
        }

        private void tckPosition_PositionChanging(object sender, System.EventArgs e)
        {
			double pos = tckPosition.Position;
			double rem = tckPosition.Duration - pos;
			ShowStatus("Seek: " + Player.GetPositionDescription(pos) + " / -" + Player.GetPositionDescription(rem));
        }

        private void lblPosition_Click(object sender, System.EventArgs e)
        {
            m_showRemaining = !m_showRemaining;

			string toolTip = "Time " + (m_showRemaining ? "Remaining" : "Elapsed");
			toolTip1.SetToolTip(lblPosition, toolTip);

			Registry.SetValue("MainForm.ShowRemaining", m_showRemaining);
            player_PositionDescriptionChanged(sender, e);
            ShowStatus("Show remainging time: " + (m_showRemaining ? "Yes" : "No"));
        }

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            JumpToFile();
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void tckVolume_PositionChanged(object sender, EventArgs e)
        {
            Player.Volume = Convert.ToInt32(tckVolume.Position);
            ShowStatus("Volume: " + Player.Volume + "%");
        }

        private void tckBalance_PositionChanged(object sender, EventArgs e)
        {
            Player.Balance = Convert.ToInt32(tckBalance.Position) - 100;
            string bal;
            if (Player.Balance == 0)
            {
                bal = "Centre";
            }
            else if (Player.Balance < 0)
            {
                bal = "Left " + Math.Abs(Player.Balance) + "%";
            }
            else
            {
                bal = "Right " + Player.Balance + "%";
            }
            ShowStatus("Balance: " + bal);
        }

        private void chkOnTop_Click(object sender, EventArgs e)
        {
            this.TopMost = chkOnTop.Checked;
			alwaysOnTopToolStripMenuItem.Checked = chkOnTop.Checked;
            ShowStatus("Always on top: " + (TopMost ? "Yes" : "No"));
			Registry.SetValue("MainForm.OnTop", chkOnTop.Checked);
        }

        private void chkRepeat_Click(object sender, System.EventArgs e)
        {
			Player.RepeatCurrentTrack = chkRepeat.Checked;
			ShowStatus("Repeat Current Track: " + (Player.RepeatCurrentTrack ? "Yes" : "No"));
			Registry.SetValue("Player.RepeatCurrentTrack", Player.RepeatCurrentTrack);
        }

		private void chkPlaylist_Click(object sender, EventArgs e)
		{
			playlistToolStripMenuItem.PerformClick();
		}

		private void chkInformation_Click(object sender, EventArgs e)
		{
			trackInformationToolStripMenuItem.PerformClick();
		}

		private void chkLibrary_Click(object sender, EventArgs e)
		{
			showLibraryToolStripMenuItem.PerformClick();
		}

		private void btnIgnore_Click(object sender, EventArgs e)
		{
			IgnoreAndNext();
		}

        private void tmStatus_Tick(object sender, System.EventArgs e)
        {
            tmrStatus.Stop();
            lblStatus.Text = "";
        }

        #endregion

        #region Menu Events

		private void queueAlbumToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Player.CurrentSong == null) return;

			string album = Player.CurrentSong.Album;

			LibraryEntry[] songs = Library.QueryLibrary("Album LIKE '" + album.Replace("'", "''") + "'", "TrackNumber", true);

			// First adds the songs that are greater in tracknumber
			foreach (LibraryEntry item in songs)
			{

				if (item.TrackNumber > Player.CurrentSong.TrackNumber
					&& Path.GetDirectoryName(Player.CurrentSong.FileName) == Path.GetDirectoryName(item.FileName))
				{
					Player.Playlist.AddToEnd(item);
				}
			}

			// ...then adds the songs that are less or equal in tracknumber
			foreach (LibraryEntry item in songs)
			{
				if (item.TrackNumber <= Player.CurrentSong.TrackNumber
					&& Path.GetDirectoryName(Player.CurrentSong.FileName) == Path.GetDirectoryName(item.FileName))
				{

					if (item.FileName.Equals(Player.CurrentSong.FileName) || Player.Playlist.Contains(item.FileName))
					{
						// do nothing
					}
					else
					{
						Player.Playlist.AddToEnd(item);
					}
				}
			}
		}

		private void ignoreAndNextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			btnIgnore_Click(sender, e);
		}

		private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			chkOnTop.Checked = !chkOnTop.Checked;
			chkOnTop_Click(sender, e);
		}

        private void enableHttpServerToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Registry.SetValue("MainForm.HttpServer", enableHttpServerToolStripMenuItem.Checked);
			ThreePM.Engine.Main.HttpServerEnabled = enableHttpServerToolStripMenuItem.Checked;
        }

		private void showLyricsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showLyricsToolStripMenuItem.Checked)
			{
				ShowForm<LyricsForm>(showLyricsToolStripMenuItem);
			}
			else
			{
				HideForm<LyricsForm>();
			}
		}

		private void showEqualizerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showEqualizerToolStripMenuItem.Checked)
			{
				ShowForm<EqualizerForm>(showEqualizerToolStripMenuItem);
			}
			else
			{
				HideForm<EqualizerForm>();
			}
		}

		private void showLibraryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showLibraryToolStripMenuItem.Checked)
            {
				ShowForm<LibraryForm>(showLibraryToolStripMenuItem);
            }
            else
            {
				HideForm<LibraryForm>();
            }
		}

        private void trackInformationToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (trackInformationToolStripMenuItem.Checked)
            {
                ShowForm<InfoForm>(trackInformationToolStripMenuItem);
            }
            else
            {
                HideForm<InfoForm>();
            }
        }

        private void playlistToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
			if (playlistToolStripMenuItem.Checked)
			{
				ShowForm<PlaylistForm>(playlistToolStripMenuItem);
			}
			else
			{
				HideForm<PlaylistForm>();
			}
		}

        private void systemInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, Player.SystemInformation, "System Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void watchFoldersToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            foreach (Form f in OwnedForms)
            {
                if (f is OptionsForm)
                {
                    f.Activate();
                    return;
                }
            }
            ShowForm<OptionsForm>(watchFoldersToolStripMenuItem);
        }

		private void trackInformationToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			chkInformation.Checked = trackInformationToolStripMenuItem.Checked;
		}

		private void showLibraryToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			chkLibrary.Checked = showLibraryToolStripMenuItem.Checked;
		}

		private void playlistToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			chkPlaylist.Checked = playlistToolStripMenuItem.Checked;
		}

		private void showAlbumArtToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showAlbumArtToolStripMenuItem.Checked)
			{
				ShowForm<AlbumArtForm>(showAlbumArtToolStripMenuItem);
			}
			else
			{
				HideForm<AlbumArtForm>();
			}
		}

		private void showAlbumListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showAlbumListToolStripMenuItem.Checked)
			{
				ShowForm<AlbumListForm>(showAlbumListToolStripMenuItem);
			}
			else
			{
				HideForm<AlbumListForm>();
			}
		}

        #endregion

        #region Private Methods

        private void ShowStatus(string status)
        {
            lblStatus.Text = status;
            tmrStatus.Stop();
            tmrStatus.Start();
        }

        private void HideForm<T>() where T : Form
        {
            foreach (Form f in OwnedForms)
            {
                if (f is T)
                {
					Registry.SetValue(f.Name + ".Show", false);
                    f.Close();
                }
            }
        }

        private void ShowForm<T>(ToolStripMenuItem trackInformationToolStripMenuItem) where T : BaseForm, new()
        {
            BaseForm f = new T();
			f.Library = Library;
			f.Player = Player;
			if (trackInformationToolStripMenuItem != null)
			{
				Registry.SetValue(f.Name + ".Show", true);
				f.FormClosed += delegate
				{
					if (InvokeRequired)
					{
						Invoke((MethodInvoker)delegate { trackInformationToolStripMenuItem.Checked = false; });
					}
					else
					{
						trackInformationToolStripMenuItem.Checked = false;
					}
				};
			}
			if (InvokeRequired)
			{
				Invoke((MethodInvoker)delegate { f.Show(this); });
			}
			else
			{
				f.Show(this); ;
			}
        }
        
        #endregion

		#region Visualization Methods

		private void tmrSpectrum_Tick(object sender, EventArgs e)
		{
			switch (m_visualizationNumber)
			{
				case 1:
				{
					pctSpectrum.Image = Player.DrawSpectrum(pctSpectrum.Width, pctSpectrum.Height, m_visualizationHighQuality, m_visualizationFullSpectrum);
					break;
				}
				case 2:
				{
					pctSpectrum.Image = Player.DrawWaveForm(pctSpectrum.Width, pctSpectrum.Height, m_visualizationHighQuality, m_visualizationFullSpectrum);
					break;
				}
				case 3:
				{
					pctSpectrum.Image = Player.DrawSpectrumWave(pctSpectrum.Width, pctSpectrum.Height, m_visualizationHighQuality, m_visualizationFullSpectrum);
					break;
				}
				case 4:
				{
					pctSpectrum.Image = Player.DrawSpectrumText(pctSpectrum.Width, pctSpectrum.Height, m_visualizationHighQuality, m_visualizationFullSpectrum);
					break;
				}
				case 5:
				{
					pctSpectrum.Image = Player.DrawSpectrumSongName(pctSpectrum.Width, pctSpectrum.Height, m_visualizationHighQuality, m_visualizationFullSpectrum);
					break;
				}
				case 6:
				{
					pctSpectrum.Image = Player.DrawSpectrumLine(pctSpectrum.Width, pctSpectrum.Height, m_visualizationHighQuality, m_visualizationFullSpectrum);
					break;
				}
				case 7:
				{
					pctSpectrum.Image = Player.DrawSpectrumLinePeak(pctSpectrum.Width, pctSpectrum.Height, m_visualizationHighQuality, m_visualizationFullSpectrum);
					break;
				}
				case 8:
				{
					pctSpectrum.Image = Player.DrawSpectrumBean(pctSpectrum.Width, pctSpectrum.Height, m_visualizationHighQuality, m_visualizationFullSpectrum);
					break;
				}
				case 9:
				{
					// this one is very similar to the above
					pctSpectrum.Image = Player.DrawSpectrumDot(pctSpectrum.Width, pctSpectrum.Height, m_visualizationHighQuality, m_visualizationFullSpectrum);
					break;
				}
				case 10:
				{
					// this one is stupid
					pctSpectrum.Image = Player.DrawSpectrumEllipse(pctSpectrum.Width, pctSpectrum.Height, m_visualizationHighQuality, m_visualizationFullSpectrum);
					break;
				}
				default:
				{
					tmrSpectrum.Stop();
					m_visualizationNumber = 0;
					Registry.SetValue("MainForm.VisualizationNumber", m_visualizationNumber);
					pctSpectrum.Image = null;
					break;
				}
			}
		}

		private void pctSpectrum_MouseUp(object sender, MouseEventArgs e)
		{
			//VisualizationNumber++;
		}

		#endregion
	}
}