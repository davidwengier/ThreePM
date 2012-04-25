using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.Misc;
using System.Collections.Specialized;
using System.Drawing;

namespace starH45.net.mp3.player
{
	public class Player : IDisposable
	{
		#region Static Things

		private SYNCPROC meta_proc;
		private int hSync = -1;

		#region AudioScrobbler Settings

		public static bool AudioscrobblerEnabled { get; set; }
		public static string AudioscrobblerUserName { get; set; }
		public static string AudioscrobblerPassword { get; set; }

		#endregion

		private static string songInfoString = "{Artist} - {Title}";
		private static int secondsBeforeUpdatePlayCount = 20;
		private static bool neverPlayIgnoredSongs;
		private static bool ignorePreviouslyPlayedSongsInRandomMode;

		public static bool NeverPlayIgnoredSongs
		{
			get { return neverPlayIgnoredSongs; }
			set { neverPlayIgnoredSongs = value; }
		}

		public static bool IgnorePreviouslyPlayedSongsInRandomMode
		{
			get { return ignorePreviouslyPlayedSongsInRandomMode; }
			set { ignorePreviouslyPlayedSongsInRandomMode = value; }
		}

		public static string SongInfoFormatString
		{
			get { return songInfoString; }
			set { songInfoString = value; }
		}

		public static int SecondsBeforeUpdatePlayCount
		{
			get { return secondsBeforeUpdatePlayCount; }
			set { secondsBeforeUpdatePlayCount = value; }
		}

		public static string[] GetDevices()
		{
			return (from info in Bass.BASS_GetDeviceInfos() select info.name).ToArray();
		}

		public static string GetPositionDescription(double pos)
		{
			TimeSpan t = TimeSpan.FromSeconds(pos);
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

		private bool m_playingStream = false;

		private string m_forceSong = "";
		private Visuals m_visuals = new Visuals();
		private delegate SongInfo GetSongDelegate();
		private Playlist m_playlist;
		private bool m_autoTrackAdvance = true;
		private bool m_repeatCurrentTrack;
		private ISynchronizeInvoke m_synchronizingObject;
		private string[] supportedExtensions;
		private ILibrary lib;
		private string m_sysInfo;
		private int m_stream;
		private PlayerState m_state;
		private System.Timers.Timer timer;
		private double lastPosition;
		private double m_position;
		private SongInfo m_currentSong;
		private Stack<string> m_history = new Stack<string>();

		private bool m_playCountUpdated;
		private int m_secondsPlayed;
		private int m_timerTicks;

		private Dictionary<int, int> m_equalizerHandles = new Dictionary<int, int>();
		private Dictionary<int, float> m_lastEqualizerValues = new Dictionary<int, float>();

		private Color m_visColor1 = Color.Red;
		private Color m_visColor2 = Color.Green;
		private Color m_visColor3 = Color.Blue;
		private Color m_visBackColor = Color.Black;

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
			BASS_DX8_PARAMEQ par = new BASS_DX8_PARAMEQ();
			if (Bass.BASS_FXGetParameters(GetFXHandle(freq), par))
			{
				return par.fGain;
			}
			return 0;
		}

		public void SetEqualizerPosition(int freq, float position)
		{
			BASS_DX8_PARAMEQ par = new BASS_DX8_PARAMEQ(freq, 5, position);
			Bass.BASS_FXSetParameters(GetFXHandle(freq), par);
			m_lastEqualizerValues[freq] = position;
		}

		private int GetFXHandle(int freq)
		{
			int handle;
			if (!m_equalizerHandles.TryGetValue(freq, out handle))
			{
				handle = Bass.BASS_ChannelSetFX(m_stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 1);
				m_equalizerHandles[freq] = handle;
			}
			return handle;
		}

		public Color VisColor1
		{
			get { return m_visColor1; }
			set { m_visColor1 = value; }
		}

		public Color VisColor2
		{
			get { return m_visColor2; }
			set { m_visColor2 = value; }
		}

		public Color VisColor3
		{
			get { return m_visColor3; }
			set { m_visColor3 = value; }
		}

		public Color VisBackColor
		{
			get { return m_visBackColor; }
			set { m_visBackColor = value; }
		}

		public Playlist Playlist
		{
			get
			{
				return m_playlist;
			}
		}

		public bool AutoTrackAdvance
		{
			get
			{
				return m_autoTrackAdvance;
			}
			set
			{
				m_autoTrackAdvance = value;
			}
		}

		public bool RepeatCurrentTrack
		{
			get
			{
				return m_repeatCurrentTrack;
			}
			set
			{
				m_repeatCurrentTrack = value;
				OnRepeatCurrentTrackChanged();
			}
		}

		public ISynchronizeInvoke SynchronizingObject
		{
			get
			{
				return m_synchronizingObject;
			}
			set
			{
				m_synchronizingObject = value;
				Playlist.SynchronizingObject = value;
			}
		}

		public string SystemInformation
		{
			get
			{
				return m_sysInfo;
			}
		}

		public float Balance
		{
			get
			{
				float pan = -1;
				Bass.BASS_ChannelGetAttribute(m_stream, BASSAttribute.BASS_ATTRIB_PAN, ref pan);
				return pan;
			}
			set
			{
				Bass.BASS_ChannelSetAttribute(m_stream, BASSAttribute.BASS_ATTRIB_PAN, value);
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
			}
		}

		public ILibrary Library
		{
			get
			{
				return lib;
			}
			set
			{
				lib = value;
				if (lib != null)
				{
					lib.SetSupportedExtensions(supportedExtensions);
				}
			}
		}

		public string[] SupportedExtensions
		{
			get
			{
				return supportedExtensions;
			}
		}

		public PlayerState State
		{
			get
			{
				return m_state;
			}
			private set
			{
				m_state = value;
				OnStateChanged();
			}
		}

		public double Position
		{
			get
			{
				return m_position;
			}
			set
			{
				try
				{
					Bass.BASS_ChannelSetPosition(m_stream, value);
				}
				catch { }
				System.Diagnostics.Debug.WriteLine("Resetting time played. Was: " + m_secondsPlayed.ToString());
				m_secondsPlayed = 0;
				m_position = Bass.BASS_ChannelBytes2Seconds(m_stream, Bass.BASS_ChannelGetPosition(m_stream));
				OnPositionChanged();
				OnPositionDescriptionChanged();
				lastPosition = m_position;

			}
		}

		public string PositionDescription
		{
			get
			{
				return Player.GetPositionDescription(m_position);
			}
		}

		public string RemainingDescription
		{
			get
			{
				return '-' + Player.GetPositionDescription(m_currentSong.Duration - m_position);
			}
		}

		public SongInfo CurrentSong
		{
			get
			{
				return m_currentSong;
			}
		}

		public int DeviceNumber
		{
			get { return Bass.BASS_GetDevice(); }
			set
			{
				if (m_currentSong != null)
				{
					double f = Position;
					string s = m_currentSong.FileName;
					Stop();
					Bass.BASS_Free();
					InitBassLibrary(value);
					Bass.BASS_SetDevice(value);
					if (LoadFile(s, false))
					{
						Position = f;
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
				if ((m_synchronizingObject != null) && m_synchronizingObject.InvokeRequired)
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
				FileEventArgs e = new FileEventArgs(status);
				if ((m_synchronizingObject != null) && m_synchronizingObject.InvokeRequired)
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
				if ((m_synchronizingObject != null) && m_synchronizingObject.InvokeRequired)
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
				if ((m_synchronizingObject != null) && m_synchronizingObject.InvokeRequired)
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
				if ((m_synchronizingObject != null) && m_synchronizingObject.InvokeRequired)
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
				if ((m_synchronizingObject != null) && m_synchronizingObject.InvokeRequired)
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
				if ((m_synchronizingObject != null) && m_synchronizingObject.InvokeRequired)
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
				SongEventArgs e = new SongEventArgs(m_currentSong);
				if ((m_synchronizingObject != null) && m_synchronizingObject.InvokeRequired)
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
				SongEventArgs e = new SongEventArgs(m_currentSong);
				if ((m_synchronizingObject != null) && m_synchronizingObject.InvokeRequired)
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
				if ((m_synchronizingObject != null) && m_synchronizingObject.InvokeRequired)
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
			m_playlist = new Playlist(this);

			InitBassLibrary(deviceNumber);

			timer = new System.Timers.Timer(100);
			timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

			meta_proc = new SYNCPROC(meta_sync);
		}

		private void InitBassLibrary(int deviceNumber)
		{
			BassNet.Registration("bass@wengier.com", "2X1132816322322");

			if (Bass.BASS_Init(deviceNumber, 44100, BASSInit.BASS_DEVICE_DEFAULT | BASSInit.BASS_DEVICE_LATENCY, IntPtr.Zero, new Guid()))
			{
				List<string> exts = new List<string>();
				m_sysInfo = "Player version: " + this.GetType().Assembly.GetName().Version.ToString();
				m_sysInfo += "\nBass Version: " + GetVersion(Bass.BASS_GetVersion());
				BASS_INFO info = new BASS_INFO();
				try
				{

					Bass.BASS_GetInfo(info);
					m_sysInfo += "\nInfo: " + info.ToString();
				}
				catch
				{
					m_sysInfo += "\nError when loading info.";
				}
				exts.AddRange(Bass.SupportedStreamExtensions.Split(';'));
				var h = Bass.BASS_PluginLoadDirectory(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
				if (h != null)
				{
					foreach (int handle in h.Keys)
					{
						try
						{
							BASS_PLUGININFO inf = Bass.BASS_PluginGetInfo(handle);
							m_sysInfo += "\n" + System.IO.Path.GetFileName(h[handle].ToString());
							m_sysInfo += ", Version: " + GetVersion(inf.version);
							foreach (BASS_PLUGINFORM form in inf.formats)
							{
								exts.AddRange(form.exts.Split(';'));
							}
						}
						catch
						{
							m_sysInfo += "\nError, could not load plugin: " + System.IO.Path.GetFileName(h[handle].ToString());
						}
					}
				}

				supportedExtensions = exts.ToArray();
				m_sysInfo += "\n\nSupported File Formats: " + string.Join(", ", supportedExtensions);
			}
			else
			{
				HandleBassError(true);
			}
		}

		public void Dispose()
		{
			try { meta_proc = null; }
			catch { }
			try { timer.Stop(); }
			catch { }
			try { timer.Dispose(); }
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

		private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			BASSActive active = Bass.BASS_ChannelIsActive(m_stream);
			if (active == BASSActive.BASS_ACTIVE_PLAYING)
			{
				m_position = Bass.BASS_ChannelBytes2Seconds(m_stream, Bass.BASS_ChannelGetPosition(m_stream));
				PositionChanged(this, EventArgs.Empty);

				if (Convert.ToInt32(lastPosition) != Convert.ToInt32(m_position))
				{
					OnPositionDescriptionChanged();
					lastPosition = m_position;
				}

				m_timerTicks += 100;
				if (m_timerTicks >= 1000)
				{
					m_timerTicks = 0;
					m_secondsPlayed++;
					if (m_secondsPlayed >= SecondsBeforeUpdatePlayCount && !m_playCountUpdated)
					{
						System.Diagnostics.Debug.WriteLine(SecondsBeforeUpdatePlayCount.ToString() + " seconds, updating play count");

						m_playCountUpdated = true;
						if (lib != null)
						{
							lib.UpdatePlayCount(m_currentSong.FileName);
							if (AudioscrobblerEnabled)
							{
								AudioscrobblerRequest req = new AudioscrobblerRequest();
								req.Username = AudioscrobblerUserName;
								req.Password = AudioscrobblerPassword;
								req.SubmitTrack(m_currentSong);
							}
						}
					}
				}
			}
			else if (active == (int)BASSActive.BASS_ACTIVE_STOPPED)
			{
				if (!m_playCountUpdated)
				{
					if (Convert.ToInt32(CurrentSong.Duration) <= SecondsBeforeUpdatePlayCount)
					{
						System.Diagnostics.Debug.WriteLine("Didn't reach " + SecondsBeforeUpdatePlayCount.ToString() + " seconds because song finished, updating play count");

						m_playCountUpdated = true;
						if (lib != null)
						{
							lib.UpdatePlayCount(m_currentSong.FileName);
						}
					}
				}
				timer.Stop();
				State = PlayerState.Stopped;
				OnSongFinished();
				if (m_autoTrackAdvance)
				{
					if (m_repeatCurrentTrack)
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

		#endregion

		#region Next Song Methods

		public void Next()
		{
			ThreadStart DoWork = new ThreadStart(delegate
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
			if (!string.IsNullOrEmpty(m_forceSong))
			{
				SongInfo inf = lib.GetSong(m_forceSong);
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
			if (m_playlist.Count > 0)
			{
				return m_playlist.GetNext();
			}
			return null;
		}

		private SongInfo GetRandomSong()
		{
			if (lib != null)
			{
				if (lib.SongCount > 0)
				{
					return lib.GetRandomSong(false, !IgnorePreviouslyPlayedSongsInRandomMode);
				}
			}
			return null;
		}

		#endregion

		#region Public Methods

		public void ForceSong(string filename)
		{
			if (m_forceSong.Equals(filename))
			{
				m_forceSong = "";
			}
			else
			{
				m_forceSong = filename;
			}
			OnSongForced();
		}

		public bool IsForced(string filename)
		{
			return m_forceSong.Equals(filename);
		}

		public bool HasPlayed(string p)
		{
			return m_history.Contains(p);
		}

		public int HistoryCount
		{
			get { return m_history.Count; }
		}

		public void PlayFile(string file)
		{
			ThreadStart DoWork = new ThreadStart(delegate
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
			if (url.EndsWith(".m3u", StringComparison.InvariantCultureIgnoreCase))
			{
				using (System.Net.WebClient client = new System.Net.WebClient())
				{
					realURL = client.DownloadString(url);
				}
			}
			System.Diagnostics.Debug.WriteLine("Opened: " + url);
			timer.Stop();
			if (hSync != -1)
			{
				Bass.BASS_ChannelRemoveSync(m_stream, hSync);
			}
			Bass.BASS_StreamFree(m_stream);
			// create the stream
			m_stream = Bass.BASS_StreamCreateURL(realURL, 0, BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_SAMPLE_SOFTWARE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_RESTRATE, null, IntPtr.Zero);
			if (m_stream != 0)
			{
				hSync = Bass.BASS_ChannelSetSync(m_stream, BASSSync.BASS_SYNC_META | BASSSync.BASS_SYNC_MIXTIME, 0, meta_proc, IntPtr.Zero);
				m_currentSong = new SongInfo(url, "", "", "", 0, 0, 0, "", "", false);
				meta_sync(1, 1, 1, IntPtr.Zero);
				return true;
			}
			else
			{
				HandleBassError(false);
				return false;
			}
		}

		private void DebugTags()
		{
			System.Diagnostics.Debug.WriteLine("ICY:");
			string[] tags = Bass.BASS_ChannelGetTagsICY(m_stream);
			foreach (string tag in tags)
			{
				System.Diagnostics.Debug.WriteLine(tag);
			}

			System.Diagnostics.Debug.WriteLine("META:");
			tags = Bass.BASS_ChannelGetTagsMETA(m_stream);
			foreach (string tag in tags)
			{
				System.Diagnostics.Debug.WriteLine(tag);
			}
		}

		private void meta_sync(int a, int b, int c, IntPtr d)
		{
			System.Diagnostics.Debug.WriteLine("Meta Sync: " + a + " " + b + " " + c + " " + d.ToInt32());
			TAG_INFO tgs = new TAG_INFO();
			bool result = BassTags.BASS_TAG_GetFromURL(m_stream, tgs);
			if (result)
			{
				System.Diagnostics.Debug.WriteLine(tgs.ToString());
			}

			m_currentSong = new SongInfo(m_currentSong.FileName, tgs.artist, tgs.album, tgs.title, 0, 0, 0, tgs.genre, "", false);
			m_playingStream = true;
			if (AudioscrobblerEnabled && !string.IsNullOrEmpty(m_currentSong.Title) && !string.IsNullOrEmpty(m_currentSong.Artist))
			{
				AudioscrobblerRequest req = new AudioscrobblerRequest();
				req.Username = AudioscrobblerUserName;
				req.Password = AudioscrobblerPassword;
				req.SubmitTrack(m_currentSong);
			}
			OnSongOpened();
		}

		public bool LoadFile(string file, bool updatePlayCount)
		{
			if (file.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
			{
				return LoadStream(file);
			}

			if (NeverPlayIgnoredSongs)
			{
				if (lib != null)
				{
					SongInfo song = lib.GetSong(file);
					if (song.Ignored)
					{
						return false;
					}
				}
			}

			System.Diagnostics.Debug.WriteLine("Opened: " + file);
			m_playCountUpdated = !updatePlayCount;
			if (!updatePlayCount)
			{
				System.Diagnostics.Debug.WriteLine("Not updating play count ever!");
			}
			m_secondsPlayed = 0;
			OnLoadingSong(file);
			if (file != null && System.IO.File.Exists(file))
			{
				timer.Stop();
				Bass.BASS_StreamFree(m_stream);
				// create the stream
				m_stream = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_SAMPLE_SOFTWARE | BASSFlag.BASS_SAMPLE_FLOAT);
				if (m_stream != 0)
				{
					m_playingStream = false;
					if (m_currentSong != null)
					{
						m_history.Push(m_currentSong.FileName);
					}
					m_currentSong = new SongInfo(file);

					// update the date now, we'll do the count later
					if (lib != null)
					{
						lib.UpdatePlayDate(file);
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
			ThreadStart DoWork = new ThreadStart(delegate
			{
				if (m_history.Count > 0)
				{
					string prevFile = m_history.Pop();
					if (LoadFile(prevFile))
					{
						Play();
						m_history.Pop();
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
			ThreadStart DoWork = new ThreadStart(delegate
			{
				if (m_stream == 0 && m_currentSong != null && !String.IsNullOrEmpty(m_currentSong.FileName))
				{
					// Try to reload the current file
					LoadFile(m_currentSong.FileName);
					Play();
				}
				else if (m_stream != 0)
				{
					try
					{
						Bass.BASS_ChannelPlay(m_stream, false);
						foreach (int freq in m_lastEqualizerValues.Keys.ToArray())
						{
							SetEqualizerPosition(freq, m_lastEqualizerValues[freq]);
						}
						if (m_playingStream)
						{
							if (lib != null)
							{
								lib.UpdatePlayCount(m_currentSong.FileName);
							}
						}
						else
						{
							timer.Start();
						}
						State = PlayerState.Playing;
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
			timer.Stop();
			State = PlayerState.Paused;
			if (Bass.BASS_ChannelIsActive(m_stream) == BASSActive.BASS_ACTIVE_PLAYING)
			{
				Bass.BASS_ChannelPause(m_stream);
			}
		}

		public void Stop()
		{
			System.Diagnostics.Debug.WriteLine("Resetting time played. Was: " + m_secondsPlayed.ToString());
			m_secondsPlayed = 0;
			timer.Stop();
			State = PlayerState.Stopped;
			if (m_stream != 0)
			{
				Bass.BASS_ChannelStop(m_stream);
				Bass.BASS_StreamFree(m_stream);
				m_stream = 0;
				Position = 0;
			}
		}

		#endregion

		#region Visualization Methods

		public Bitmap DrawWaveForm(int width, int height, bool highQuality, bool fullSpectrum)
		{
			return m_visuals.CreateWaveForm(m_stream, width, height, m_visColor1, m_visColor2, m_visBackColor, m_visBackColor, 1, fullSpectrum, false, highQuality);
		}

		public Bitmap DrawSpectrum(int width, int height, bool highQuality, bool fullSpectrum)
		{
			return m_visuals.CreateSpectrum(m_stream, width, height, m_visColor1, m_visColor2, m_visBackColor, false, fullSpectrum, highQuality);
		}

		public Bitmap DrawSpectrumWave(int width, int height, bool highQuality, bool fullSpectrum)
		{
			return m_visuals.CreateSpectrumWave(m_stream, width, height, m_visColor1, m_visColor2, m_visBackColor, 2, false, fullSpectrum, highQuality);
		}

		public Bitmap DrawSpectrumText(int width, int height, bool highQuality, bool fullSpectrum)
		{
			return m_visuals.CreateSpectrumText(m_stream, width, height, m_visColor1, m_visColor2, m_visBackColor, "starH45.net.mp3 ", false, fullSpectrum, highQuality);
		}

		public Bitmap DrawSpectrumSongName(int width, int height, bool highQuality, bool fullSpectrum)
		{
			if (m_currentSong == null)
				return null;
			return m_visuals.CreateSpectrumText(m_stream, width, height, m_visColor1, m_visColor2, m_visBackColor, m_currentSong.Title + " ", false, fullSpectrum, highQuality);
		}

		public Bitmap DrawSpectrumLine(int width, int height, bool highQuality, bool fullSpectrum)
		{
			return m_visuals.CreateSpectrumLine(m_stream, width, height, m_visColor1, m_visColor2, m_visBackColor, 3, 2, false, fullSpectrum, highQuality);
		}

		public Bitmap DrawSpectrumLinePeak(int width, int height, bool highQuality, bool fullSpectrum)
		{
			return m_visuals.CreateSpectrumLinePeak(m_stream, width, height, m_visColor1, m_visColor2, m_visColor3, m_visBackColor, 3, 2, 2, 20, false, fullSpectrum, highQuality);
		}

		public Bitmap DrawSpectrumBean(int width, int height, bool highQuality, bool fullSpectrum)
		{
			return m_visuals.CreateSpectrumBean(m_stream, width, height, m_visColor1, m_visColor2, m_visBackColor, 4, false, fullSpectrum, highQuality);
		}

		public Bitmap DrawSpectrumDot(int width, int height, bool highQuality, bool fullSpectrum)
		{
			return m_visuals.CreateSpectrumDot(m_stream, width, height, m_visColor1, m_visColor2, m_visBackColor, 3, 2, false, fullSpectrum, highQuality);
		}

		public Bitmap DrawSpectrumEllipse(int width, int height, bool highQuality, bool fullSpectrum)
		{
			return m_visuals.CreateSpectrumEllipse(m_stream, width, height, m_visColor1, m_visColor2, m_visBackColor, 1, 2, false, fullSpectrum, highQuality);
		}

		#endregion
	}
}
