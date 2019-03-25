using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ThreePM.MusicPlayer
{
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public sealed class Playlist : IEnumerable<SongInfo>
#pragma warning restore CA1710 // Identifiers should have correct suffix
    {
        #region Declarations

        private bool _eventsEnabled = true;
        private Random _random;
        private ISynchronizeInvoke _synchronizingObject;
        private readonly List<SongInfo> _songs = new List<SongInfo>();
        private PlaylistStyle _playListStyle = PlaylistStyle.Normal;
        private int _index;
        private readonly Player _player;

        #region Events

        public event EventHandler PlaylistChanged;
        public event EventHandler PlaylistStyleChanged;

        #endregion

        #endregion

        #region Properties

        public bool EventsEnabled
        {
            get { return _eventsEnabled; }
            set
            {
                _eventsEnabled = value;
                if (value) OnPlaylistChanged();
            }
        }

        public int LoopPosition
        {
            get
            {
                if (_playListStyle == PlaylistStyle.Looping || _playListStyle == PlaylistStyle.RandomLooping)
                {
                    return _index;
                }
                else
                {
                    return -1;
                }
            }
            set
            {
                if (_playListStyle == PlaylistStyle.Looping)
                {
                    _index = value;
                }
            }
        }

        public PlaylistStyle PlaylistStyle
        {
            get
            {
                return _playListStyle;
            }
            set
            {
                _playListStyle = value;
                OnPlaylistStyleChanged();
            }
        }


        internal ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                return _synchronizingObject;
            }
            set
            {
                _synchronizingObject = value;
            }
        }

        #endregion

        #region Constructor

        internal Playlist(Player player)
        {
            _player = player;
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return _songs.Count;
            }
        }

        #endregion

        #region Public Methods

        public void AddAt(int index, SongInfo song)
        {
            _songs.Insert(index, song);
            OnPlaylistChanged();
        }

        public void AddToStart(SongInfo song)
        {
            _songs.Insert(0, song);
            OnPlaylistChanged();
        }

        public void AddToEnd(SongInfo song)
        {
            _songs.Add(song);
            OnPlaylistChanged();
        }

        public void AddToStart(SongInfo[] songs)
        {
            for (int i = songs.Length - 1; i >= 0; i--)
            {
                _songs.Insert(0, songs[i]);
            }
            OnPlaylistChanged();
        }

        public void AddToEnd(SongInfo[] songs)
        {
            foreach (SongInfo song in songs)
            {
                _songs.Add(song);
            }
            OnPlaylistChanged();
        }

        public void Clear()
        {
            _songs.Clear();
            _index = 0;
            OnPlaylistChanged();
        }

        public void MoveUp(int index)
        {
            if (index == 0) return;
            SongInfo temp = _songs[index];
            _songs[index] = _songs[index - 1];
            _songs[index - 1] = temp;
            OnPlaylistChanged();
        }

        public void MoveDown(int index)
        {
            if (index == (this.Count - 1)) return;
            SongInfo temp = _songs[index];
            _songs[index] = _songs[index + 1];
            _songs[index + 1] = temp;
            OnPlaylistChanged();
        }

        public void Remove(int index)
        {
            _songs.RemoveAt(index);
            if (_index == _songs.Count)
            {
                _index = 0;
            }
            OnPlaylistChanged();
        }

        public void Remove(string filename)
        {
            int index = -1;
            bool found = false;
            foreach (SongInfo song in _songs)
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
            foreach (SongInfo song in _songs)
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
            return _songs.ToArray();
        }

        #endregion

        #region Private Methods

        private void OnPlaylistChanged()
        {
            if (!_eventsEnabled) return;

            EventHandler handler = PlaylistChanged;
            if (handler != null)
            {
                EventArgs e = EventArgs.Empty;
                if ((_synchronizingObject != null) && _synchronizingObject.InvokeRequired)
                {
                    _synchronizingObject.BeginInvoke(handler, new object[] { this, e });
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
                if ((_synchronizingObject != null) && _synchronizingObject.InvokeRequired)
                {
                    _synchronizingObject.BeginInvoke(handler, new object[] { this, e });
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

            lock (_songs)
            {
                switch (_playListStyle)
                {
                    case PlaylistStyle.Normal:
                    {
                        _index = 0;
                        remove = true;
                        break;
                    }
                    case PlaylistStyle.Random:
                    case PlaylistStyle.RandomLooping:
                    {
                        if (_random == null)
                        {
                            _random = new Random();
                        }
                        _index = _random.Next(0, _songs.Count);
                        remove = (_playListStyle == PlaylistStyle.Random);
                        break;
                    }
                    case PlaylistStyle.Looping:
                    {
                        _index++;
                        if (_index == _songs.Count)
                        {
                            _index = 0;
                        }
                        remove = false;
                        break;
                    }
                }
                result = _songs[_index];
                if (remove)
                {
                    Remove(_index);
                }
            }
            return result;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetAllSongs().GetEnumerator();
        }

        #endregion

        #region IEnumerable<SongInfo> Members

        public IEnumerator<SongInfo> GetEnumerator()
        {
            foreach (SongInfo s in _songs)
            {
                yield return s;
            }
        }

        #endregion

        private IEnumerable<SongInfo> GetAllSongs()
        {
            foreach (SongInfo s in _songs)
            {
                if (s != null)
                {
                    yield return s;
                }
            }
        }

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

                this.EventsEnabled = false;
                Clear();
                foreach (string s in playlist)
                {
                    if (!s.StartsWith("#"))
                    {
                        string filename = Path.GetFullPath(s.Trim());
                        // see if we can be nice, and get this file from the library
                        SongInfo info = _player.Library.GetSong(filename);
                        if (info == null)
                        {
                            info = new SongInfo(filename);
                        }
                        AddToEnd(info);
                    }
                }
                this.EventsEnabled = true;
            }
        }

        public void SaveToFile(string file)
        {
            using (var s = new StreamWriter(File.OpenWrite(file)))
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
