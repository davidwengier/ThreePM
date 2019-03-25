using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.Misc;

namespace ThreePM.MusicPlayer
{
    public sealed class Player : IDisposable
    {
        #region Static Things

        private SYNCPROC _meta_proc;
        private int _hSync = -1;

        #region AudioScrobbler Settings

        public static bool AudioscrobblerEnabled { get; set; }
        public static string AudioscrobblerUserName { get; set; }
        public static string AudioscrobblerPassword { get; set; }

        #endregion

        private static string s_songInfoString = "{Artist} - {Title}";
        private static int s_secondsBeforeUpdatePlayCount = 20;
        private static bool s_neverPlayIgnoredSongs;
        private static bool s_ignorePreviouslyPlayedSongsInRandomMode;

        public static bool NeverPlayIgnoredSongs
        {
            get { return s_neverPlayIgnoredSongs; }
            set { s_neverPlayIgnoredSongs = value; }
        }

        public static bool IgnorePreviouslyPlayedSongsInRandomMode
        {
            get { return s_ignorePreviouslyPlayedSongsInRandomMode; }
            set { s_ignorePreviouslyPlayedSongsInRandomMode = value; }
        }

        public static string SongInfoFormatString
        {
            get { return s_songInfoString; }
            set { s_songInfoString = value; }
        }

        public static int SecondsBeforeUpdatePlayCount
        {
            get { return s_secondsBeforeUpdatePlayCount; }
            set { s_secondsBeforeUpdatePlayCount = value; }
        }

        public static string[] GetDevices()
        {
            return (from info in Bass.BASS_GetDeviceInfos() select info.name).ToArray();
        }

        public static string DescribePosition(double pos)
        {
            var t = TimeSpan.FromSeconds(pos);
            string result = "";
            if (t.Hours > 0)
            {
                result += t.Hours + ":" + t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00");
            }
            else
            {
                result = t.Minutes + ":" + t.Seconds.ToString("00");
            }
            return result;
        }

        #endregion

        #region Declarations

        private bool _playingStream = false;

        private string _forceSong = "";
        private readonly Visuals _visuals = new Visuals();
        private delegate SongInfo GetSongDelegate();
        private readonly Playlist _playlist;
        private bool _autoTrackAdvance = true;
        private bool _repeatCurrentTrack;
        private ISynchronizeInvoke _synchronizingObject;
        private string[] _supportedExtensions;
        private ILibrary _lib;
        private string _sysInfo;
        private int _stream;
        private PlayerState _state;
        private readonly System.Timers.Timer _timer;
        private double _lastPosition;
        private double _position;
        private SongInfo _currentSong;
        private readonly Stack<string> _history = new Stack<string>();

        private bool _playCountUpdated;
        private int _secondsPlayed;
        private int _timerTicks;

        private readonly Dictionary<int, int> _equalizerHandles = new Dictionary<int, int>();
        private readonly Dictionary<int, float> _lastEqualizerValues = new Dictionary<int, float>();

        private Color _visColor1 = Color.Red;
        private Color _visColor2 = Color.Green;
        private Color _visColor3 = Color.Blue;
        private Color _visBackColor = Color.Black;

        #region Events

        public event EventHandler SongForced;
        public event EventHandler CurrentSongChanged;
        public event EventHandler PositionChanged;
        public event EventHandler PositionDescriptionChanged;
        public event EventHandler RemainingDescriptionChanged;
        public event EventHandler StateChanged;
        public event EventHandler RepeatCurrentTrackChanged;
        public event EventHandler<SongEventArgs> SongFinished;
        public event EventHandler<SongEventArgs> SongOpened;
        public event EventHandler<FileEventArgs> LoadingSong;

        #endregion

        #endregion

        #region Properties

        public float GetEqualizerPosition(int freq)
        {
            var par = new BASS_DX8_PARAMEQ();
            if (Bass.BASS_FXGetParameters(GetFXHandle(freq), par))
            {
                return par.fGain;
            }
            return 0;
        }

        public void SetEqualizerPosition(int freq, float position)
        {
            var par = new BASS_DX8_PARAMEQ(freq, 5, position);
            Bass.BASS_FXSetParameters(GetFXHandle(freq), par);
            _lastEqualizerValues[freq] = position;
        }

        private int GetFXHandle(int freq)
        {
            if (!_equalizerHandles.TryGetValue(freq, out int handle))
            {
                handle = Bass.BASS_ChannelSetFX(_stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 1);
                _equalizerHandles[freq] = handle;
            }
            return handle;
        }

        public Color VisColor1
        {
            get { return _visColor1; }
            set { _visColor1 = value; }
        }

        public Color VisColor2
        {
            get { return _visColor2; }
            set { _visColor2 = value; }
        }

        public Color VisColor3
        {
            get { return _visColor3; }
            set { _visColor3 = value; }
        }

        public Color VisBackColor
        {
            get { return _visBackColor; }
            set { _visBackColor = value; }
        }

        public Playlist Playlist
        {
            get
            {
                return _playlist;
            }
        }

        public bool AutoTrackAdvance
        {
            get
            {
                return _autoTrackAdvance;
            }
            set
            {
                _autoTrackAdvance = value;
            }
        }

        public bool RepeatCurrentTrack
        {
            get
            {
                return _repeatCurrentTrack;
            }
            set
            {
                _repeatCurrentTrack = value;
                OnRepeatCurrentTrackChanged();
            }
        }

        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                return _synchronizingObject;
            }
            set
            {
                _synchronizingObject = value;
                this.Playlist.SynchronizingObject = value;
            }
        }

        public string SystemInformation
        {
            get
            {
                return _sysInfo;
            }
        }

        public float Balance
        {
            get
            {
                float pan = -1;
                Bass.BASS_ChannelGetAttribute(_stream, BASSAttribute.BASS_ATTRIB_PAN, ref pan);
                return pan;
            }
            set
            {
                Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_PAN, value);
            }
        }

        public static float Volume
        {
            get
            {
                return Bass.BASS_GetVolume();
            }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    Bass.BASS_SetVolume(value);
                }
                NotifyPropertyChanged(nameof(Volume));
            }
        }

        public ILibrary Library
        {
            get
            {
                return _lib;
            }
            set
            {
                _lib = value;
                if (_lib != null)
                {
                    _lib.SetSupportedExtensions(_supportedExtensions);
                }
            }
        }

        public string[] SupportedExtensions
        {
            get
            {
                return _supportedExtensions;
            }
        }

        public PlayerState State
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
                OnStateChanged();
            }
        }

        public double Position
        {
            get
            {
                return _position;
            }
            set
            {
                try
                {
                    Bass.BASS_ChannelSetPosition(_stream, value);
                }
                catch { }
                System.Diagnostics.Debug.WriteLine("Resetting time played. Was: " + _secondsPlayed.ToString());
                _secondsPlayed = 0;
                _position = Bass.BASS_ChannelBytes2Seconds(_stream, Bass.BASS_ChannelGetPosition(_stream));
                OnPositionChanged();
                OnPositionDescriptionChanged();
                _lastPosition = _position;

            }
        }

        public string PositionDescription
        {
            get
            {
                return Player.DescribePosition(_position);
            }
        }

        public string RemainingDescription
        {
            get
            {
                return '-' + Player.DescribePosition(_currentSong.Duration - _position);
            }
        }

        public SongInfo CurrentSong
        {
            get
            {
                return _currentSong;
            }
        }

        public int DeviceNumber
        {
            get { return Bass.BASS_GetDevice(); }
            set
            {
                if (_currentSong != null)
                {
                    double f = this.Position;
                    string s = _currentSong.FileName;
                    Stop();
                    Bass.BASS_Free();
                    InitBassLibrary(value);
                    Bass.BASS_SetDevice(value);
                    if (LoadFile(s, false))
                    {
                        this.Position = f;
                        Play();
                    }
                }
                else
                {
                    Bass.BASS_Free();
                    InitBassLibrary(value);
                    Bass.BASS_SetDevice(value);
                }
            }
        }


        #endregion

        #region OnEvent Methods

        private void OnRepeatCurrentTrackChanged()
        {
            EventHandler handler = RepeatCurrentTrackChanged;
            if (handler != null)
            {
                EventArgs e = EventArgs.Empty;
                if ((_synchronizingObject != null) && _synchronizingObject.InvokeRequired)
                {
                    this.SynchronizingObject.BeginInvoke(handler, new object[] { this, e });
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        private void OnLoadingSong(string status)
        {
            EventHandler<FileEventArgs> handler = LoadingSong;
            if (handler != null)
            {
                var e = new FileEventArgs(status);
                if ((_synchronizingObject != null) && _synchronizingObject.InvokeRequired)
                {
                    this.SynchronizingObject.BeginInvoke(handler, new object[] { this, e });
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        private void OnStateChanged()
        {
            EventHandler handler = StateChanged;
            if (handler != null)
            {
                EventArgs e = EventArgs.Empty;
                if ((_synchronizingObject != null) && _synchronizingObject.InvokeRequired)
                {
                    this.SynchronizingObject.BeginInvoke(handler, new object[] { this, e });
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        private void OnSongForced()
        {
            EventHandler handler = SongForced;
            if (handler != null)
            {
                EventArgs e = EventArgs.Empty;
                if ((_synchronizingObject != null) && _synchronizingObject.InvokeRequired)
                {
                    this.SynchronizingObject.BeginInvoke(handler, new object[] { this, e });
                }
                else
                {
                    handler(this, e);
                }
            }

        }

        private void OnPositionChanged()
        {
            EventHandler handler = PositionChanged;
            if (handler != null)
            {
                EventArgs e = EventArgs.Empty;
                if ((_synchronizingObject != null) && _synchronizingObject.InvokeRequired)
                {
                    this.SynchronizingObject.BeginInvoke(handler, new object[] { this, e });
                }
                else
                {
                    handler(this, e);
                }
            }

        }

        private void OnPositionDescriptionChanged()
        {
            EventHandler handler = PositionDescriptionChanged;
            if (handler != null)
            {
                EventArgs e = EventArgs.Empty;
                if ((_synchronizingObject != null) && _synchronizingObject.InvokeRequired)
                {
                    this.SynchronizingObject.BeginInvoke(handler, new object[] { this, e });
                }
                else
                {
                    handler(this, e);
                }
            }

            handler = RemainingDescriptionChanged;
            if (handler != null)
            {
                EventArgs e = EventArgs.Empty;
                if ((_synchronizingObject != null) && _synchronizingObject.InvokeRequired)
                {
                    this.SynchronizingObject.BeginInvoke(handler, new object[] { this, e });
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        private void OnSongFinished()
        {
            EventHandler<SongEventArgs> handler = SongFinished;
            if (handler != null)
            {
                var e = new SongEventArgs(_currentSong);
                if ((_synchronizingObject != null) && _synchronizingObject.InvokeRequired)
                {
                    this.SynchronizingObject.BeginInvoke(handler, new object[] { this, e });
                }
                else
                {
                    handler(this, e);
                }
            }

        }

        public void OnSongOpened()
        {
            EventHandler<SongEventArgs> handler = SongOpened;
            if (handler != null)
            {
                var e = new SongEventArgs(_currentSong);
                if ((_synchronizingObject != null) && _synchronizingObject.InvokeRequired)
                {
                    this.SynchronizingObject.BeginInvoke(handler, new object[] { this, e });
                }
                else
                {
                    handler(this, e);
                }
            }

            EventHandler handler2 = CurrentSongChanged;
            if (handler2 != null)
            {
                EventArgs e = EventArgs.Empty;
                if ((_synchronizingObject != null) && _synchronizingObject.InvokeRequired)
                {
                    this.SynchronizingObject.BeginInvoke(handler2, new object[] { this, e });
                }
                else
                {
                    handler2(this, e);
                }
            }
        }

        #endregion

        #region Constructor

        public Player()
            : this(-1)
        {
        }

        public Player(int deviceNumber)
        {
            _playlist = new Playlist(this);

            InitBassLibrary(deviceNumber);

            _timer = new System.Timers.Timer(100);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            _meta_proc = new SYNCPROC(meta_sync);

            void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                BASSActive active = Bass.BASS_ChannelIsActive(_stream);
                if (active == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    _position = Bass.BASS_ChannelBytes2Seconds(_stream, Bass.BASS_ChannelGetPosition(_stream));
                    PositionChanged(this, EventArgs.Empty);

                    if (Convert.ToInt32(_lastPosition) != Convert.ToInt32(_position))
                    {
                        OnPositionDescriptionChanged();
                        _lastPosition = _position;
                    }

                    _timerTicks += 100;
                    if (_timerTicks >= 1000)
                    {
                        _timerTicks = 0;
                        _secondsPlayed++;
                        if (_secondsPlayed >= SecondsBeforeUpdatePlayCount && !_playCountUpdated)
                        {
                            System.Diagnostics.Debug.WriteLine(SecondsBeforeUpdatePlayCount.ToString() + " seconds, updating play count");

                            _playCountUpdated = true;
                            if (_lib != null)
                            {
                                _lib.UpdatePlayCount(_currentSong.FileName);
                                if (AudioscrobblerEnabled)
                                {
                                    var req = new AudioscrobblerRequest();
                                    req.Username = AudioscrobblerUserName;
                                    req.Password = AudioscrobblerPassword;
                                    req.SubmitTrack(_currentSong);
                                }
                            }
                        }
                    }
                }
                else if (active == (int)BASSActive.BASS_ACTIVE_STOPPED)
                {
                    if (!_playCountUpdated)
                    {
                        if (Convert.ToInt32(this.CurrentSong.Duration) <= SecondsBeforeUpdatePlayCount)
                        {
                            System.Diagnostics.Debug.WriteLine("Didn't reach " + SecondsBeforeUpdatePlayCount.ToString() + " seconds because song finished, updating play count");

                            _playCountUpdated = true;
                            if (_lib != null)
                            {
                                _lib.UpdatePlayCount(_currentSong.FileName);
                            }
                        }
                    }
                    _timer.Stop();
                    this.State = PlayerState.Stopped;
                    OnSongFinished();
                    if (_autoTrackAdvance)
                    {
                        if (_repeatCurrentTrack)
                        {
                            Stop();
                            Play();
                        }
                        else
                        {
                            Next();
                        }
                    }
                }
            }
        }

        private void InitBassLibrary(int deviceNumber)
        {
            BassNet.Registration("bass@wengier.com", "2X1132816322322");

            if (Bass.BASS_Init(deviceNumber, 44100, BASSInit.BASS_DEVICE_DEFAULT | BASSInit.BASS_DEVICE_LATENCY, IntPtr.Zero, Guid.Empty))
            {
                var exts = new List<string>();
                _sysInfo = "Player version: " + this.GetType().Assembly.GetName().Version.ToString();
                _sysInfo += "\nBass Version: " + GetVersion(Bass.BASS_GetVersion());
                var info = new BASS_INFO();
                try
                {

                    Bass.BASS_GetInfo(info);
                    _sysInfo += "\nInfo: " + info.ToString();
                }
                catch
                {
                    _sysInfo += "\nError when loading info.";
                }
                exts.AddRange(Bass.SupportedStreamExtensions.Split(';'));
                Dictionary<int, string> h = Bass.BASS_PluginLoadDirectory(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
                if (h != null)
                {
                    foreach (int handle in h.Keys)
                    {
                        try
                        {
                            BASS_PLUGININFO inf = Bass.BASS_PluginGetInfo(handle);
                            _sysInfo += "\n" + System.IO.Path.GetFileName(h[handle].ToString());
                            _sysInfo += ", Version: " + GetVersion(inf.version);
                            foreach (BASS_PLUGINFORM form in inf.formats)
                            {
                                exts.AddRange(form.exts.Split(';'));
                            }
                        }
                        catch
                        {
                            _sysInfo += "\nError, could not load plugin: " + System.IO.Path.GetFileName(h[handle].ToString());
                        }
                    }
                }

                _supportedExtensions = exts.ToArray();
                _sysInfo += "\n\nSupported File Formats: " + string.Join(", ", _supportedExtensions);
            }
            else
            {
                HandleBassError(true);
            }
        }

        public void Dispose()
        {
            try { _meta_proc = null; }
            catch { }
            try { _timer.Stop(); }
            catch { }
            try { _timer.Dispose(); }
            catch { }
            try { Bass.BASS_Stop(); }
            catch { }
            try { Bass.BASS_Free(); }
            catch { }
            try { Bass.FreeMe(); }
            catch { }
        }

        #endregion

        #region Private Methods

        private static string GetVersion(int num1)
        {
            // magic!!
            return new Version((num1 >> 0x18) & 0xff, (num1 >> 0x10) & 0xff, (num1 >> 8) & 0xff, num1 & 0xff).ToString();
        }

        #endregion

        #region Next Song Methods

        public void Next()
        {
            var DoWork = new ThreadStart(delegate
            {
                bool success = false;
                if (!success)
                    success = TryToLoadAndPlay(GetForceSong);
                if (!success)
                    success = TryToLoadAndPlay(GetFileFromPlaylist);
                if (!success)
                    success = TryToLoadAndPlay(GetRandomSong);
            });
            DoWork.BeginInvoke(null, null);
        }

        private bool TryToLoadAndPlay(GetSongDelegate getFile)
        {
            while (true)
            {
                SongInfo file = getFile();
                if (file == null)
                {
                    return false;
                }
                else if (LoadFile(file.FileName))
                {
                    Play();
                    return true;
                }
            }
        }

        private SongInfo GetForceSong()
        {
            if (!string.IsNullOrEmpty(_forceSong))
            {
                SongInfo inf = _lib.GetSong(_forceSong);
                ForceSong("");
                if (inf != null)
                {
                    return inf;
                }
            }
            return null;
        }

        private SongInfo GetFileFromPlaylist()
        {
            if (_playlist.Count > 0)
            {
                return _playlist.GetNext();
            }
            return null;
        }

        private SongInfo GetRandomSong()
        {
            if (_lib != null)
            {
                if (_lib.SongCount > 0)
                {
                    return _lib.GetRandomSong(false, !IgnorePreviouslyPlayedSongsInRandomMode);
                }
            }
            return null;
        }

        #endregion

        #region Public Methods

        public void ForceSong(string filename)
        {
            if (_forceSong.Equals(filename))
            {
                _forceSong = "";
            }
            else
            {
                _forceSong = filename;
            }
            OnSongForced();
        }

        public bool IsForced(string filename)
        {
            return _forceSong.Equals(filename);
        }

        public bool HasPlayed(string p)
        {
            return _history.Contains(p);
        }

        public int HistoryCount
        {
            get { return _history.Count; }
        }

        public void PlayFile(string file)
        {
            var DoWork = new ThreadStart(delegate
            {
                if (LoadFile(file))
                {
                    Play();
                }
                else
                {
                    Next();
                }
            });

            DoWork.BeginInvoke(null, null);
        }

        public bool LoadFile(string file)
        {
            return LoadFile(file, true);
        }

        public bool LoadStream(string url)
        {
            string realURL = url;
            if (url.EndsWith(".m3u", StringComparison.OrdinalIgnoreCase))
            {
                using (var client = new System.Net.WebClient())
                {
                    realURL = client.DownloadString(url);
                }
            }
            System.Diagnostics.Debug.WriteLine("Opened: " + url);
            _timer.Stop();
            if (_hSync != -1)
            {
                Bass.BASS_ChannelRemoveSync(_stream, _hSync);
            }
            Bass.BASS_StreamFree(_stream);
            // create the stream
            _stream = Bass.BASS_StreamCreateURL(realURL, 0, BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_SAMPLE_SOFTWARE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_RESTRATE, null, IntPtr.Zero);
            if (_stream != 0)
            {
                _hSync = Bass.BASS_ChannelSetSync(_stream, BASSSync.BASS_SYNC_META | BASSSync.BASS_SYNC_MIXTIME, 0, _meta_proc, IntPtr.Zero);
                _currentSong = new SongInfo(url, "", "", "", 0, 0, 0, "", "", false);
                meta_sync(1, 1, 1, IntPtr.Zero);
                return true;
            }
            else
            {
                HandleBassError(false);
                return false;
            }
        }

        private void meta_sync(int a, int b, int c, IntPtr d)
        {
            System.Diagnostics.Debug.WriteLine("Meta Sync: " + a + " " + b + " " + c + " " + d.ToInt32());
            var tgs = new TAG_INFO();
            bool result = BassTags.BASS_TAG_GetFromURL(_stream, tgs);
            if (result)
            {
                System.Diagnostics.Debug.WriteLine(tgs.ToString());
            }

            _currentSong = new SongInfo(_currentSong.FileName, tgs.artist, tgs.album, tgs.title, 0, 0, 0, tgs.genre, "", false);
            _playingStream = true;
            if (AudioscrobblerEnabled && !string.IsNullOrEmpty(_currentSong.Title) && !string.IsNullOrEmpty(_currentSong.Artist))
            {
                var req = new AudioscrobblerRequest();
                req.Username = AudioscrobblerUserName;
                req.Password = AudioscrobblerPassword;
                req.SubmitTrack(_currentSong);
            }
            OnSongOpened();
        }

        public bool LoadFile(string file, bool updatePlayCount)
        {
            if (file.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                return LoadStream(file);
            }

            if (NeverPlayIgnoredSongs)
            {
                if (_lib != null)
                {
                    SongInfo song = _lib.GetSong(file);
                    if (song.Ignored)
                    {
                        return false;
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("Opened: " + file);
            _playCountUpdated = !updatePlayCount;
            if (!updatePlayCount)
            {
                System.Diagnostics.Debug.WriteLine("Not updating play count ever!");
            }
            _secondsPlayed = 0;
            OnLoadingSong(file);
            if (file != null && System.IO.File.Exists(file))
            {
                _timer.Stop();
                Bass.BASS_StreamFree(_stream);
                // create the stream
                _stream = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_SAMPLE_SOFTWARE | BASSFlag.BASS_SAMPLE_FLOAT);
                if (_stream != 0)
                {
                    _playingStream = false;
                    if (_currentSong != null)
                    {
                        _history.Push(_currentSong.FileName);
                    }
                    _currentSong = new SongInfo(file);

                    // update the date now, we'll do the count later
                    if (_lib != null)
                    {
                        _lib.UpdatePlayDate(file);
                    }

                    OnSongOpened();
                    return true;
                }
                else
                {
                    HandleBassError(false);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static void HandleBassError(bool throwException)
        {
            BASSError error = Bass.BASS_ErrorGetCode();
            string message = "Bass Error " + Convert.ToInt32(error) + ": " + error.ToString();
            System.Diagnostics.Debug.WriteLine(message);
            if (throwException)
            {
                throw new Exception(message);
            }
        }

        public void Previous()
        {
            var DoWork = new ThreadStart(delegate
            {
                if (_history.Count > 0)
                {
                    string prevFile = _history.Pop();
                    if (LoadFile(prevFile))
                    {
                        Play();
                        _history.Pop();
                    }
                    else
                    {
                        Next();
                    }
                }
            });
            DoWork.BeginInvoke(null, null);
        }

        public void Play()
        {
            var DoWork = new ThreadStart(delegate
            {
                if (_stream == 0 && _currentSong != null && !string.IsNullOrEmpty(_currentSong.FileName))
                {
                    // Try to reload the current file
                    LoadFile(_currentSong.FileName);
                    Play();
                }
                else if (_stream != 0)
                {
                    try
                    {
                        Bass.BASS_ChannelPlay(_stream, false);
                        foreach (int freq in _lastEqualizerValues.Keys.ToArray())
                        {
                            SetEqualizerPosition(freq, _lastEqualizerValues[freq]);
                        }
                        if (_playingStream)
                        {
                            if (_lib != null)
                            {
                                _lib.UpdatePlayCount(_currentSong.FileName);
                            }
                        }
                        else
                        {
                            _timer.Start();
                        }
                        this.State = PlayerState.Playing;
                    }
                    catch (Exception ex)
                    {
                        // Um...what's going on here?!
                        //System.Diagnostics.Debugger.Break();
                        System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                        Next();
                    }
                }
                else
                {
                    Next();
                }
            });
            DoWork.BeginInvoke(null, null);
        }

        public void Pause()
        {
            _timer.Stop();
            this.State = PlayerState.Paused;
            if (Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                Bass.BASS_ChannelPause(_stream);
            }
        }

        public void Stop()
        {
            System.Diagnostics.Debug.WriteLine("Resetting time played. Was: " + _secondsPlayed.ToString());
            _secondsPlayed = 0;
            _timer.Stop();
            this.State = PlayerState.Stopped;
            if (_stream != 0)
            {
                Bass.BASS_ChannelStop(_stream);
                Bass.BASS_StreamFree(_stream);
                _stream = 0;
                this.Position = 0;
            }
        }

        #endregion

        #region Visualization Methods

        private static void NotifyPropertyChanged(string v)
        {
            throw new NotImplementedException();
        }

        public Bitmap DrawWaveForm(int width, int height, bool highQuality, bool fullSpectrum)
        {
            return _visuals.CreateWaveForm(_stream, width, height, _visColor1, _visColor2, _visBackColor, _visBackColor, 1, fullSpectrum, false, highQuality);
        }

        public Bitmap DrawSpectrum(int width, int height, bool highQuality, bool fullSpectrum)
        {
            return _visuals.CreateSpectrum(_stream, width, height, _visColor1, _visColor2, _visBackColor, false, fullSpectrum, highQuality);
        }

        public Bitmap DrawSpectrumWave(int width, int height, bool highQuality, bool fullSpectrum)
        {
            return _visuals.CreateSpectrumWave(_stream, width, height, _visColor1, _visColor2, _visBackColor, 2, false, fullSpectrum, highQuality);
        }

        public Bitmap DrawSpectrumText(int width, int height, bool highQuality, bool fullSpectrum)
        {
            return _visuals.CreateSpectrumText(_stream, width, height, _visColor1, _visColor2, _visBackColor, "ThreePM ", false, fullSpectrum, highQuality);
        }

        public Bitmap DrawSpectrumSongName(int width, int height, bool highQuality, bool fullSpectrum)
        {
            if (_currentSong == null)
                return null;
            return _visuals.CreateSpectrumText(_stream, width, height, _visColor1, _visColor2, _visBackColor, _currentSong.Title + " ", false, fullSpectrum, highQuality);
        }

        public Bitmap DrawSpectrumLine(int width, int height, bool highQuality, bool fullSpectrum)
        {
            return _visuals.CreateSpectrumLine(_stream, width, height, _visColor1, _visColor2, _visBackColor, 3, 2, false, fullSpectrum, highQuality);
        }

        public Bitmap DrawSpectrumLinePeak(int width, int height, bool highQuality, bool fullSpectrum)
        {
            return _visuals.CreateSpectrumLinePeak(_stream, width, height, _visColor1, _visColor2, _visColor3, _visBackColor, 3, 2, 2, 20, false, fullSpectrum, highQuality);
        }

        public Bitmap DrawSpectrumBean(int width, int height, bool highQuality, bool fullSpectrum)
        {
            return _visuals.CreateSpectrumBean(_stream, width, height, _visColor1, _visColor2, _visBackColor, 4, false, fullSpectrum, highQuality);
        }

        public Bitmap DrawSpectrumDot(int width, int height, bool highQuality, bool fullSpectrum)
        {
            return _visuals.CreateSpectrumDot(_stream, width, height, _visColor1, _visColor2, _visBackColor, 3, 2, false, fullSpectrum, highQuality);
        }

        public Bitmap DrawSpectrumEllipse(int width, int height, bool highQuality, bool fullSpectrum)
        {
            return _visuals.CreateSpectrumEllipse(_stream, width, height, _visColor1, _visColor2, _visBackColor, 1, 2, false, fullSpectrum, highQuality);
        }

        #endregion


    }
}
