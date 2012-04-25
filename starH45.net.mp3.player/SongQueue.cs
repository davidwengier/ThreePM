using System;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;

namespace starH45.net.mp3.player
{
    public sealed class SongQueue : IEnumerable
    {
        #region Declarations

        private ISynchronizeInvoke m_synchronizingObject;
        private StringCollection songs = new StringCollection();

        #region Events

        public event EventHandler QueueChanged;

        #endregion

        #endregion

        #region Properties

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

        internal SongQueue()
        {
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return songs.Count;
            }
        }

        #endregion

        #region Public Methods

        public void AddToStart(string filename)
        {
            songs.Insert(0, filename);
            OnQueueChanged();
        }

        public void AddToEnd(string filename)
        {
            songs.Add(filename);
            OnQueueChanged();
        }

        public void Clear()
        {
            songs.Clear();
            OnQueueChanged();
        }

        public void MoveUp(int index)
        {
            if (index == 0) return;
            string temp = songs[index];
            songs[index] = songs[index - 1];
            songs[index - 1] = temp;
            OnQueueChanged();
        }

        public void MoveDown(int index)
        {
            if (index == (Count - 1)) return;
            string temp = songs[index];
            songs[index] = songs[index + 1];
            songs[index + 1] = temp;
            OnQueueChanged();
        }

        public void Remove(int index)
        {
            songs.RemoveAt(index);
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

        internal string GetNext()
        {
            string result;
            lock (songs)
            {
                result = songs[0];
                songs.RemoveAt(0);
                OnQueueChanged();
            }
            return result;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (string s in songs)
            {
                yield return s;
            }
        }

        #endregion
    }
}
