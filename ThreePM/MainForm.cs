using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ThreePM.MusicLibrary;
using ThreePM.MusicPlayer;
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
        private bool _visualizationFullSpectrum = true;
        private bool _visualizationHighQuality = true;
        private int _visualizationNumber = 1;
        //private ThreePM.utilities.HttpServer m_server;
        private bool _firstSongCountChanged = true;
        private bool _showRemaining;
        private Keys _ignoreAndNextKey = (Keys.Z | Keys.Control | Keys.Shift);

        private ToasterForm _toasterForm;

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

            if (!this.DesignMode)
            {
                ThreePM.Engine.Main.Initialize();
                this.Player = ThreePM.Engine.Main.Player;
                this.Library = ThreePM.Engine.Main.Library;
                this.Library.SynchronizingObject = this;
                this.Player.SynchronizingObject = this;

                //Player = new Player();
                //Library = new Library();

                RegisterHotKey(Keys.SelectMedia);
                RegisterHotKey(Keys.MediaPlayPause);
                RegisterHotKey(Keys.MediaNextTrack);
                RegisterHotKey(Keys.MediaPreviousTrack);
                RegisterHotKey(Keys.MediaStop);

                RegisterHotKey(_ignoreAndNextKey);

                this.Library.ScanStarting += new EventHandler(Library_ScanStarting);
                this.Library.ScanFinished += new EventHandler(lib_ScanFinished);
                this.Library.SongCountChanged += new EventHandler(Library_SongCountChanged);

                this.Player.PositionChanged += new EventHandler(player_PositionChanged);
                this.Player.PositionDescriptionChanged += new EventHandler(player_PositionDescriptionChanged);
                this.Player.SongOpened += new System.EventHandler<SongEventArgs>(player_SongOpened);
                this.Player.StateChanged += new System.EventHandler(player_StateChanged);
                this.Player.Playlist.PlaylistStyleChanged += new EventHandler(Playlist_PlaylistStyleChanged);

                ThreePM.Engine.Main.Start();

                this.ShowToasterForm = Convert.ToBoolean(Registry.GetValue("MainForm.ShowToasterForm", true));
                _showRemaining = Convert.ToBoolean(Registry.GetValue("MainForm.ShowRemaining", false));
                _visualizationNumber = Convert.ToInt32(Registry.GetValue("MainForm.VisualizationNumber", 1));
                _visualizationHighQuality = Convert.ToBoolean(Registry.GetValue("MainForm.VisualizationHighQuality", true));
                _visualizationFullSpectrum = Convert.ToBoolean(Registry.GetValue("MainForm.VisualizationFullSpectrum", true));
                tmrSpectrum.Interval = Convert.ToInt32(Registry.GetValue("MainForm.VisualizationSpeed", 50));
                chkOnTop.Checked = Convert.ToBoolean(Registry.GetValue("MainForm.OnTop", true));
                this.TopMost = chkOnTop.Checked;

                tckVolume.Duration = 100F;
                tckBalance.Duration = 200F;
                chkRepeat.Checked = this.Player.RepeatCurrentTrack;
                tckVolume.Position = Convert.ToSingle(Player.Volume);
                tckBalance.Position = Convert.ToSingle(this.Player.Balance + 100);

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

                enableHttpServerToolStripMenuItem.Checked = ThreePM.Engine.Main.HttpServerEnabled;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ThreePM.Engine.Main.End();
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
            get { return _toasterForm != null; }
            set
            {
                Registry.SetValue("MainForm.ShowToasterForm", value);
                if (value)
                {
                    _toasterForm = new ToasterForm();
                    _toasterForm.Player = this.Player;
                }
                else
                {
                    if (_toasterForm != null)
                    {
                        _toasterForm.Close();
                        _toasterForm.Dispose();
                        _toasterForm = null;
                    }
                }
            }
        }

        public bool VisualizationHighQuality
        {
            get { return _visualizationHighQuality; }
            set
            {
                _visualizationHighQuality = value;
                Registry.SetValue("MainForm.VisualizationHighQuality", value);
            }
        }

        public bool VisualizationFullSpectrum
        {
            get { return _visualizationFullSpectrum; }
            set
            {
                _visualizationFullSpectrum = value;
                Registry.SetValue("MainForm.VisualizationFullSpectrum", value);
            }
        }

        public int VisualizationNumber
        {
            get { return _visualizationNumber; }
            set
            {
                _visualizationNumber = value;
                Registry.SetValue("MainForm.VisualizationNumber", _visualizationNumber);
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

        private void Library_ScanStarting(object sender, EventArgs e)
        {
            prgSpin.Start();
        }

        private void lib_ScanFinished(object sender, EventArgs e)
        {
            prgSpin.Stop();
        }

        private void Library_SongCountChanged(object sender, EventArgs e)
        {
            lblSongCount.Text = this.Library.SongCount + " songs";
            if (_firstSongCountChanged)
            {
                _firstSongCountChanged = false;
                this.Player.Play();
            }
        }

        #endregion

        #region Player Events

        private void Playlist_PlaylistStyleChanged(object sender, EventArgs e)
        {
            switch (this.Player.Playlist.PlaylistStyle)
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
            Registry.SetValue("Player.PlaylistStyle", (int)this.Player.Playlist.PlaylistStyle);
        }

        private void player_PositionChanged(object sender, System.EventArgs e)
        {
            tckPosition.Position = this.Player.Position;
        }

        private void player_PositionDescriptionChanged(object sender, EventArgs e)
        {
            lblPosition.Text = (_showRemaining ? this.Player.RemainingDescription : this.Player.PositionDescription);
        }

        private void player_SongOpened(object sender, SongEventArgs e)
        {
            if (e.Song.FileName.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Library.UpdateInternetRadio(e.Song.FileName, e.Song.Album);
            }
            else
            {
                if (this.Library.GetSong(e.Song.FileName) is LibraryEntry entry)
                {
                    this.Library.UpdateIfNeeded(e.Song.FileName);

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
        }

        private void player_StateChanged(object sender, EventArgs e)
        {
            btnPlay.Checked = this.Player.State == PlayerState.Playing;
            btnPause.Checked = this.Player.State == PlayerState.Paused;
            btnStop.Checked = this.Player.State == PlayerState.Stopped;

            if (_visualizationNumber != 0)
            {
                if (this.Player.State == PlayerState.Playing)
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
            this.Player.Next();
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
            this.Player.PlayFile(files[0]);
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
            UnregisterHotKey(Keys.SelectMedia);
            UnregisterHotKey(Keys.MediaPlayPause);
            UnregisterHotKey(Keys.MediaNextTrack);
            UnregisterHotKey(Keys.MediaPreviousTrack);
            UnregisterHotKey(Keys.MediaStop);
            UnregisterHotKey(_ignoreAndNextKey);
        }

        #endregion

        #region Key Handling Methods

        private Dictionary<Keys, bool> _hotKeySet = new Dictionary<Keys, bool>();

        public void RegisterHotKey(System.Windows.Forms.Keys key)
        {
            if (_hotKeySet.ContainsKey(key) && _hotKeySet[key])
            {
                UnregisterHotKey(key);
            }

            _hotKeySet[key] = User32_RegisterHotKey(this.Handle, 1000 + Convert.ToInt32(key), 0, key);
        }

        public void UnregisterHotKey(Keys key)
        {
            if (_hotKeySet.ContainsKey(key) && _hotKeySet[key])
            {
                _hotKeySet[key] = !User32_UnregisterHotKey(this.Handle, 1000 + Convert.ToInt32(key));
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
            //    JumpListManager manager = this.CreateJumpListManager();
            //    manager.UserRemovedItems += (o, e) =>
            //    {
            //        // we have to hook this up, but i dont care
            //    };
            //    manager.ClearAllDestinations();

            //    string myExe = System.Reflection.Assembly.GetEntryAssembly().Location;

            //    manager.AddCustomDestination(new ShellLink()
            //    {
            //        Category = "Controls",
            //        Path = myExe,
            //        Title = "Previous",
            //        IconLocation = myExe,
            //        IconIndex = 7,
            //        Arguments = "/prev"
            //    });
            //    manager.AddCustomDestination(new ShellLink()
            //    {
            //        Category = "Controls",
            //        Path = myExe,
            //        Title = "Play",
            //        IconLocation = myExe,
            //        IconIndex = 5,
            //        Arguments = "/play"
            //    });
            //    manager.AddCustomDestination(new ShellLink()
            //    {
            //        Category = "Controls",
            //        Path = myExe,
            //        Title = "Pause",
            //        IconLocation = myExe,
            //        IconIndex = 4,
            //        Arguments = "/pause"
            //    });
            //    manager.AddCustomDestination(new ShellLink()
            //    {
            //        Category = "Controls",
            //        Path = myExe,
            //        Title = "Stop",
            //        IconLocation = myExe,
            //        IconIndex = 10,
            //        Arguments = "/stop"
            //    });
            //    manager.AddCustomDestination(new ShellLink()
            //    {
            //        Category = "Controls",
            //        Path = myExe,
            //        Title = "Next",
            //        IconLocation = myExe,
            //        IconIndex = 3,
            //        Arguments = "/next"
            //    });
            //    manager.EnabledAutoDestinationType = ApplicationDestinationType.Recent;
            //    manager.Refresh();
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
                if (this.Player.State == PlayerState.Paused || this.Player.State == PlayerState.Stopped)
                {
                    this.Player.Play();
                }
                else
                {
                    this.Player.Pause();
                }
                return true;
            }
            else if (keyData == Keys.MediaPreviousTrack)
            {
                this.Player.ForceSong(this.Player.CurrentSong.FileName);
                this.Player.Previous();
            }
            else if (keyData == Keys.MediaNextTrack)
            {
                this.Player.Next();
                return true;
            }
            else if (keyData == Keys.MediaStop)
            {
                this.Player.Stop();
                return true;
            }
            else if (keyData == Keys.SelectMedia)
            {
                // show jump to file box
                JumpToFile();
                return true;
            }
            else if (keyData == _ignoreAndNextKey)
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
                this.Player.OnSongOpened();
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
                    if (this.Player.State == PlayerState.Paused || this.Player.State == PlayerState.Stopped)
                    {
                        this.Player.Play();
                    }
                    else
                    {
                        this.Player.Pause();
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
            if (this.Player.CurrentSong != null)
            {
                this.Library.Ignore(this.Player.CurrentSong.FileName);
                this.Player.Next();
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        private void JumpToFile()
        {
            foreach (Form f in this.OwnedForms)
            {
                if (f is SearchForm)
                {

                    f.Activate();
                    f.Select();
                    return;
                }
            }
            ShowForm<SearchForm>(songSearchToolStripMenuItem);
            foreach (Form f in this.OwnedForms)
            {
                if (f is SearchForm)
                {
                    //Attach foreground window thread to our thread
                    AttachThreadInput(GetWindowThreadProcessId(GetForegroundWindow(), out uint nul), GetCurrentThreadId(), true);

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
            if (this.Player.Position > 4)
            {
                this.Player.Position = 0;
                return;
            }
            ShowStatus("Previous");
            this.Player.ForceSong(this.Player.CurrentSong.FileName);
            this.Player.Previous();
        }

        private void btnPlay_Click(object sender, System.EventArgs e)
        {
            ShowStatus("Play");
            if (this.Player.State == PlayerState.Paused || this.Player.State == PlayerState.Stopped)
            {
                this.Player.Play();
            }
            else
            {
                this.Player.Pause();
            }
        }

        private void btnPause_Click(object sender, System.EventArgs e)
        {
            ShowStatus("Pause");
            if (this.Player.State == PlayerState.Paused || this.Player.State == PlayerState.Stopped)
            {
                this.Player.Play();
            }
            else
            {
                this.Player.Pause();
            }
        }

        private void btnStop_Click(object sender, System.EventArgs e)
        {
            ShowStatus("Stop");
            this.Player.Stop();
        }

        private void btnNext_Click(object sender, System.EventArgs e)
        {
            ShowStatus("Next");
            this.Player.Next();
        }

        private void tckPosition_PositionChanged(object sender, System.EventArgs e)
        {
            this.Player.Position = tckPosition.Position;
        }

        private void tckPosition_PositionChanging(object sender, System.EventArgs e)
        {
            double pos = tckPosition.Position;
            double rem = tckPosition.Duration - pos;
            ShowStatus("Seek: " + Player.GetPositionDescription(pos) + " / -" + Player.GetPositionDescription(rem));
        }

        private void lblPosition_Click(object sender, System.EventArgs e)
        {
            _showRemaining = !_showRemaining;

            string toolTip = "Time " + (_showRemaining ? "Remaining" : "Elapsed");
            toolTip1.SetToolTip(lblPosition, toolTip);

            Registry.SetValue("MainForm.ShowRemaining", _showRemaining);
            player_PositionDescriptionChanged(sender, e);
            ShowStatus("Show remainging time: " + (_showRemaining ? "Yes" : "No"));
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
            this.Player.Balance = Convert.ToInt32(tckBalance.Position) - 100;
            string bal;
            if (this.Player.Balance == 0)
            {
                bal = "Centre";
            }
            else if (this.Player.Balance < 0)
            {
                bal = "Left " + Math.Abs(this.Player.Balance) + "%";
            }
            else
            {
                bal = "Right " + this.Player.Balance + "%";
            }
            ShowStatus("Balance: " + bal);
        }

        private void chkOnTop_Click(object sender, EventArgs e)
        {
            this.TopMost = chkOnTop.Checked;
            alwaysOnTopToolStripMenuItem.Checked = chkOnTop.Checked;
            ShowStatus("Always on top: " + (this.TopMost ? "Yes" : "No"));
            Registry.SetValue("MainForm.OnTop", chkOnTop.Checked);
        }

        private void chkRepeat_Click(object sender, System.EventArgs e)
        {
            this.Player.RepeatCurrentTrack = chkRepeat.Checked;
            ShowStatus("Repeat Current Track: " + (this.Player.RepeatCurrentTrack ? "Yes" : "No"));
            Registry.SetValue("Player.RepeatCurrentTrack", this.Player.RepeatCurrentTrack);
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
            if (this.Player.CurrentSong == null) return;

            string album = this.Player.CurrentSong.Album;

            LibraryEntry[] songs = this.Library.QueryLibrary("Album LIKE '" + album.Replace("'", "''") + "'", "TrackNumber", true);

            // First adds the songs that are greater in tracknumber
            foreach (LibraryEntry item in songs)
            {

                if (item.TrackNumber > this.Player.CurrentSong.TrackNumber
                    && Path.GetDirectoryName(this.Player.CurrentSong.FileName) == Path.GetDirectoryName(item.FileName))
                {
                    this.Player.Playlist.AddToEnd(item);
                }
            }

            // ...then adds the songs that are less or equal in tracknumber
            foreach (LibraryEntry item in songs)
            {
                if (item.TrackNumber <= this.Player.CurrentSong.TrackNumber
                    && Path.GetDirectoryName(this.Player.CurrentSong.FileName) == Path.GetDirectoryName(item.FileName))
                {

                    if (item.FileName.Equals(this.Player.CurrentSong.FileName) || this.Player.Playlist.Contains(item.FileName))
                    {
                        // do nothing
                    }
                    else
                    {
                        this.Player.Playlist.AddToEnd(item);
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
            MessageBox.Show(this, this.Player.SystemInformation, "System Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void watchFoldersToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            foreach (Form f in this.OwnedForms)
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
            foreach (Form f in this.OwnedForms)
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
            f.Library = this.Library;
            f.Player = this.Player;
            if (trackInformationToolStripMenuItem != null)
            {
                Registry.SetValue(f.Name + ".Show", true);
                f.FormClosed += delegate
                {
                    if (this.InvokeRequired)
                    {
                        Invoke((MethodInvoker)delegate { trackInformationToolStripMenuItem.Checked = false; });
                    }
                    else
                    {
                        trackInformationToolStripMenuItem.Checked = false;
                    }
                };
            }
            if (this.InvokeRequired)
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
            switch (_visualizationNumber)
            {
                case 1:
                {
                    pctSpectrum.Image = this.Player.DrawSpectrum(pctSpectrum.Width, pctSpectrum.Height, _visualizationHighQuality, _visualizationFullSpectrum);
                    break;
                }
                case 2:
                {
                    pctSpectrum.Image = this.Player.DrawWaveForm(pctSpectrum.Width, pctSpectrum.Height, _visualizationHighQuality, _visualizationFullSpectrum);
                    break;
                }
                case 3:
                {
                    pctSpectrum.Image = this.Player.DrawSpectrumWave(pctSpectrum.Width, pctSpectrum.Height, _visualizationHighQuality, _visualizationFullSpectrum);
                    break;
                }
                case 4:
                {
                    pctSpectrum.Image = this.Player.DrawSpectrumText(pctSpectrum.Width, pctSpectrum.Height, _visualizationHighQuality, _visualizationFullSpectrum);
                    break;
                }
                case 5:
                {
                    pctSpectrum.Image = this.Player.DrawSpectrumSongName(pctSpectrum.Width, pctSpectrum.Height, _visualizationHighQuality, _visualizationFullSpectrum);
                    break;
                }
                case 6:
                {
                    pctSpectrum.Image = this.Player.DrawSpectrumLine(pctSpectrum.Width, pctSpectrum.Height, _visualizationHighQuality, _visualizationFullSpectrum);
                    break;
                }
                case 7:
                {
                    pctSpectrum.Image = this.Player.DrawSpectrumLinePeak(pctSpectrum.Width, pctSpectrum.Height, _visualizationHighQuality, _visualizationFullSpectrum);
                    break;
                }
                case 8:
                {
                    pctSpectrum.Image = this.Player.DrawSpectrumBean(pctSpectrum.Width, pctSpectrum.Height, _visualizationHighQuality, _visualizationFullSpectrum);
                    break;
                }
                case 9:
                {
                    // this one is very similar to the above
                    pctSpectrum.Image = this.Player.DrawSpectrumDot(pctSpectrum.Width, pctSpectrum.Height, _visualizationHighQuality, _visualizationFullSpectrum);
                    break;
                }
                case 10:
                {
                    // this one is stupid
                    pctSpectrum.Image = this.Player.DrawSpectrumEllipse(pctSpectrum.Width, pctSpectrum.Height, _visualizationHighQuality, _visualizationFullSpectrum);
                    break;
                }
                default:
                {
                    tmrSpectrum.Stop();
                    _visualizationNumber = 0;
                    Registry.SetValue("MainForm.VisualizationNumber", _visualizationNumber);
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
