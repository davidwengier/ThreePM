using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ThreePM.MusicPlayer
{
    public sealed class Playlist : IEnumerable<SongInfo>
    {
        #region Declarations

		private bool m_eventsEnabled = true;
		private Random m_random;
        private ISynchronizeInvoke m_synchronizingObject;
		private List<SongInfo> m_songs = new List<SongInfo>();
		private PlaylistStyle m_playListStyle = PlaylistStyle.Normal;
		private int m_index;
		private Player m_player;

        #region Events

        public event EventHandler PlaylistChanged;
		public event EventHandler PlaylistStyleChanged;

        #endregion

        #endregion

        #region Properties

		public bool EventsEnabled
		{
			get { return m_eventsEnabled; }
			set
			{
				m_eventsEnabled = value;
				if (value) OnPlaylistChanged();
			}
		}

		public int LoopPosition
		{
			get
			{
				if (m_playListStyle == PlaylistStyle.Looping || m_playListStyle == PlaylistStyle.RandomLooping)
				{
					return m_index;
				}
				else
				{
					return -1;
				}
			}
			set
			{
				if (m_playListStyle == PlaylistStyle.Looping)
				{
					m_index = value;
				}
			}
		}

		public PlaylistStyle PlaylistStyle
		{
			get
			{
				return m_playListStyle;
			}
			set
			{
				m_playListStyle = value;
				OnPlaylistStyleChanged();
			}
		}
	

        internal ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                return m_synchronizingObject;
            }
            set
            {
                m_synchronizingObject = value;
            }
        }

        #endregion

        #region Constructor

        internal Playlist(Player player)
        {
			m_player = player;
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return m_songs.Count;
            }
        }

        #endregion

        #region Public Methods

		public void AddAt(int index, SongInfo song)
		{
			m_songs.Insert(index, song);
			OnPlaylistChanged();
		}

        public void AddToStart(SongInfo song)
        {
            m_songs.Insert(0, song);
            OnPlaylistChanged();
        }

        public void AddToEnd(SongInfo song)
        {
            m_songs.Add(song);
            OnPlaylistChanged();
        }

		public void AddToStart(SongInfo[] songs)
		{
			for (int i = songs.Length - 1; i >= 0; i--)
			{
				m_songs.Insert(0, songs[i]);
			}
			OnPlaylistChanged();
		}

		public void AddToEnd(SongInfo[] songs)
		{
			foreach (SongInfo song in songs)
			{
				m_songs.Add(song);
			}
			OnPlaylistChanged();
		}

        public void Clear()
        {
            m_songs.Clear();
			m_index = 0;
            OnPlaylistChanged();
        }

        public void MoveUp(int index)
        {
            if (index == 0) return;
            SongInfo temp = m_songs[index];
            m_songs[index] = m_songs[index - 1];
            m_songs[index - 1] = temp;
            OnPlaylistChanged();
        }

        public void MoveDown(int index)
        {
            if (index == (Count - 1)) return;
            SongInfo temp = m_songs[index];
            m_songs[index] = m_songs[index + 1];
            m_songs[index + 1] = temp;
            OnPlaylistChanged();
        }

        public void Remove(int index)
        {
            m_songs.RemoveAt(index);
			if (m_index == m_songs.Count)
			{
				m_index = 0;
			}
            OnPlaylistChanged();
        }

		public void Remove(string filename)
		{
			int index = -1;
			bool found = false;
			foreach (SongInfo song in m_songs)
			{
				index++;
				if (song.FileName.Equals(filename))
				{
					found = true;
					break;
				}
			}
			if (found)
			{
				Remove(index);
			}
		}

		public bool Contains(string filename)
		{
			foreach (SongInfo song in m_songs)
			{
				if (song.FileName.Equals(filename))
				{
					return true;
				}
			}
			return false;
		}

		public SongInfo[] ToArray()
		{
			return m_songs.ToArray();
		}

        #endregion

        #region Private Methods

        private void OnPlaylistChanged()
        {
			if (!m_eventsEnabled) return;

            EventHandler handler = PlaylistChanged;
            if (handler != null)
            {
                EventArgs e = EventArgs.Empty;
                if ((m_synchronizingObject != null) && m_synchronizingObject.InvokeRequired)
                {
                    m_synchronizingObject.BeginInvoke(handler, new object[] { this, e });
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        private void OnPlaylistStyleChanged()
        {
			EventHandler handler = PlaylistStyleChanged;
            if (handler != null)
            {
                EventArgs e = EventArgs.Empty;
                if ((m_synchronizingObject != null) && m_synchronizingObject.InvokeRequired)
                {
                    m_synchronizingObject.BeginInvoke(handler, new object[] { this, e });
                }
                else
                {
                    handler(this, e);
                }
            }
        }    

		#endregion

        #region Internal Methods

        internal SongInfo GetNext()
        {
			bool remove = true;
            SongInfo result = null;

            lock (m_songs)
            {
				switch (m_playListStyle)
				{
					case PlaylistStyle.Normal:
					{
						m_index = 0;
						remove = true;
						break;
					}
					case PlaylistStyle.Random:
					case PlaylistStyle.RandomLooping:
					{
						if (m_random == null)
						{
							m_random = new Random();
						}
						m_index = m_random.Next(0, m_songs.Count);
						remove = (m_playListStyle == PlaylistStyle.Random);
						break;
					}
					case PlaylistStyle.Looping:
					{
						m_index++;
						if (m_index == m_songs.Count)
						{
							m_index = 0;
						}
						remove = false;
						break;
					}
				}
                result = m_songs[m_index];
				if (remove)
				{
					Remove(m_index);
				}
            }
            return result;
        }

        #endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (SongInfo s in m_songs)
			{
				yield return s;
			}
		}

		#endregion

		#region IEnumerable<SongInfo> Members

		public IEnumerator<SongInfo> GetEnumerator()
		{
			foreach (SongInfo s in m_songs)
			{
				yield return s;
			}
		}

		#endregion

		public void LoadFromFile(string file)
		{
			if (File.Exists(file))
			{
				string[] playlist = File.ReadAllLines(file);
				if (playlist.Length == 1 && playlist[0].IndexOf('\r') != -1)
				{
					playlist = playlist[0].Split('\r');
				}
				else if (playlist.Length == 1 && playlist[0].IndexOf('\n') != -1)
				{
					playlist = playlist[0].Split('\n');
				}

				EventsEnabled = false;
				Clear();
				foreach (string s in playlist)
				{
					if (!s.StartsWith("#"))
					{
						string filename = Path.GetFullPath(s.Trim());
						// see if we can be nice, and get this file from the library
						SongInfo info = m_player.Library.GetSong(filename);
						if (info == null)
						{
							info = new SongInfo(filename);
						}
						AddToEnd(info);
					}
				}
				EventsEnabled = true;
			}
		}

		public void SaveToFile(string file)
		{
			using (StreamWriter s = new StreamWriter(File.OpenWrite(file)))
			{
				s.WriteLine("#EXTM3U");
				foreach (SongInfo info in this)
				{
					if (info.ToString() != info.FileName)
					{
						s.WriteLine("#EXTINF:-1," + info.ToString());
					}
					s.WriteLine(info.FileName);
				}
			}
		}
	}
}
