using System;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;

namespace ThreePM.player
{
    public sealed class SongQueue : IEnumerable
    {
        #region Declarations

        private ISynchronizeInvoke _synchronizingObject;
        private readonly StringCollection _songs = new StringCollection();

        #region Events

        public event EventHandler QueueChanged;

        #endregion

        #endregion

        #region Properties

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

        internal SongQueue()
        {
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

        public void AddToStart(string filename)
        {
            _songs.Insert(0, filename);
            OnQueueChanged();
        }

        public void AddToEnd(string filename)
        {
            _songs.Add(filename);
            OnQueueChanged();
        }

        public void Clear()
        {
            _songs.Clear();
            OnQueueChanged();
        }

        public void MoveUp(int index)
        {
            if (index == 0) return;
            string temp = _songs[index];
            _songs[index] = _songs[index - 1];
            _songs[index - 1] = temp;
            OnQueueChanged();
        }

        public void MoveDown(int index)
        {
            if (index == (Count - 1)) return;
            string temp = _songs[index];
            _songs[index] = _songs[index + 1];
            _songs[index + 1] = temp;
            OnQueueChanged();
        }

        public void Remove(int index)
        {
            _songs.RemoveAt(index);
            OnQueueChanged();
        }

        #endregion

        #region Private Methods

        private void OnQueueChanged()
        {
            EventHandler handler = QueueChanged;
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

        internal string GetNext()
        {
            string result;
            lock (_songs)
            {
                result = _songs[0];
                _songs.RemoveAt(0);
                OnQueueChanged();
            }
            return result;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (string s in _songs)
            {
                yield return s;
            }
        }

        #endregion
    }
}
