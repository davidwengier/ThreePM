using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading;
using ThreePM.MusicPlayer;

namespace ThreePM.MusicLibrary
{
    public sealed class Library : LibraryBase, ILibrary, IDisposable
    {
        #region Declarations

        private Random _random = new Random();
        private ISynchronizeInvoke _synchronizingObject;
        private Dictionary<int, FileSystemWatcher> _fileWatchers = new Dictionary<int, FileSystemWatcher>();
        private List<string> _supportedExtensions = new List<string>();
        private readonly object _dbSyncObject = new object();
        private readonly string _connectionString;
        private Thread _scanningThread;
        private List<DataRow> _watchFolders = new List<DataRow>();
        private Dictionary<string, bool> _existingFiles = new Dictionary<string, bool>();
        private int _songCount = -1;

        public enum NeedsUpdateResult
        {
            NeedsUpdate,
            DoesntNeedUpdate,
            FileNotFound
        }

        #region Events

        public event EventHandler ScanStarting;
        public event EventHandler<ScanStatusEventArgs> ScanStatus;
        public event EventHandler ScanFinished;
        public event EventHandler SongCountChanged;
        public event EventHandler<LibraryEntryEventArgs> LibraryUpdated;
        public event EventHandler<LibraryEntryEventArgs> PlayCountUpdated;

        #endregion

        #endregion

        #region Properties

        public ISynchronizeInvoke SynchronizingObject
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

        public int SongCount
        {
            get
            {
                if (_songCount == -1)
                {
                    lock (_dbSyncObject)
                    {
                        _songCount = Convert.ToInt32(SQLiteHelper.ExecuteScalar(_connectionString, "SELECT COUNT(LibraryID) FROM Library WHERE Deleted = 0"));
                    }
                }
                return _songCount;
            }
        }

        #endregion

        #region Constructor

        public Library()
            : this(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\library.db")
        {

        }

        public Library(string dbFile)
        {
            _connectionString = "Data Source=" + dbFile + ";Compress=True;New=True;";

            lock (_dbSyncObject)
            {
                UpdateDB();
            }
        }

        public void Dispose()
        {
            try
            {
                foreach (FileSystemWatcher watcher in _fileWatchers.Values)
                {
                    watcher.Dispose();
                }
                _fileWatchers = null;
            }
            catch { }
            try
            {
                lock (_dbSyncObject)
                {
                    if (_scanningThread != null)
                    {
                        _scanningThread.Abort();
                        _scanningThread.Join();
                    }
                }
            }
            catch { }
        }

        #endregion

        #region OnEvent Methods

        private void OnPlayCountUpdated(LibraryEntry info)
        {
            EventHandler<LibraryEntryEventArgs> handler = PlayCountUpdated;
            if (handler != null)
            {
                var e = new LibraryEntryEventArgs(info);
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

        private void OnLibraryUpdated(LibraryEntry info)
        {
            EventHandler<LibraryEntryEventArgs> handler = LibraryUpdated;
            if (handler != null)
            {
                var e = new LibraryEntryEventArgs(info);
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

        private void OnSongCountChanged()
        {
            EventHandler handler = SongCountChanged;
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

        private void OnScanFinished()
        {
            EventHandler handler = ScanFinished;
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

        private void OnScanStarting()
        {
            EventHandler handler = ScanStarting;
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

        private void OnScanStatus(string status)
        {
            EventHandler<ScanStatusEventArgs> handler = ScanStatus;
            if (handler != null)
            {
                var e = new ScanStatusEventArgs(status);
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



        #endregion

        #region Folder Scanning Methods

        public static List<string> NonExistantFolders = new List<string>();

        public void RefreshLibrary()
        {
            //System.Diagnostics.Debug.WriteLine("Refresh Library start");
            if (_scanningThread != null && _scanningThread.IsAlive)
            {
                _scanningThread.Abort();
                _scanningThread.Join();
            }

            DataSet watch;
            lock (_dbSyncObject)
            {
                watch = SQLiteHelper.ExecuteDataSet(_connectionString, "SELECT WatchFolderID, Folder FROM WatchFolders");
            }

            string crit = "";
            _watchFolders = new List<DataRow>();
            foreach (DataRow dr in watch.Tables[0].Rows)
            {
                if (Directory.Exists(dr["Folder"].ToString()))
                {
                    crit += dr["WatchFolderID"].ToString() + ", ";
                    _watchFolders.Add(dr);
                }
                else
                {
                    NonExistantFolders.Add(dr["Folder"].ToString());
                }
            }
            if (crit.Length > 0)
            {
                crit = " WHERE WatchFolderID IN (" + crit.Substring(0, crit.Length - 2) + ")";
            }
            DataSet reader;
            lock (_dbSyncObject)
            {
                reader = SQLiteHelper.ExecuteDataSet(_connectionString, "SELECT Filename FROM Library " + crit);
            }

            //System.Diagnostics.Debug.WriteLine("Loading file dictionary");
            _existingFiles = new Dictionary<string, bool>();
            foreach (DataRow dr in reader.Tables[0].Rows)
            {
                if (_existingFiles.ContainsKey(dr["Filename"].ToString()))
                {
                    // it must be in the db twice.. delete one of them?
                    lock (_dbSyncObject)
                    {
                        int id = Convert.ToInt32(SQLiteHelper.ExecuteScalar(_connectionString, "SELECT LibraryID FROM Library WHERE Filename = '" + dr["Filename"].ToString().Replace("'", "''") + "' LIMIT 1"));
                        SQLiteHelper.ExecuteNonQuery(_connectionString, "DELETE FROM Library WHERE LibraryID = " + id);
                    }
                }
                else
                {
                    _existingFiles.Add(dr["Filename"].ToString(), true);
                }
            }
            _songCount = _existingFiles.Count;
            OnSongCountChanged();

            //System.Diagnostics.Debug.WriteLine("Creating watchers");
            foreach (DataRow dr in _watchFolders)
            {
                StartWatching(Convert.ToInt32(dr["WatchFolderID"]), dr["Folder"].ToString());
            }

            //System.Diagnostics.Debug.WriteLine("Starting scan");
            ScanFoldersAsync();
        }

        public void AddWatchFolder(string dir)
        {
            int i;
            lock (_dbSyncObject)
            {
                i = Convert.ToInt32(SQLiteHelper.ExecuteScalar(_connectionString, "SELECT COUNT(WatchFolderID) FROM WatchFolders WHERE Folder = '" + dir.Replace("'", "''") + "'"));
            }
            if (i == 0 && Directory.Exists(dir))
            {
                DataSet ds;
                lock (_dbSyncObject)
                {
                    SQLiteHelper.ExecuteNonQuery(_connectionString, "INSERT INTO WatchFolders (Folder) VALUES ('" + dir.Replace("'", "''") + "')");
                    ds = SQLiteHelper.ExecuteDataSet(_connectionString, "SELECT * FROM WatchFolders WHERE Folder = '" + dir.Replace("'", "''") + "'");

                }
                _watchFolders.Add(ds.Tables[0].Rows[0]);
                StartWatching(Convert.ToInt32(ds.Tables[0].Rows[0]["WatchFolderID"]), dir);
                if (_scanningThread != null && !_scanningThread.IsAlive)
                {
                    ScanFoldersAsync();
                }
            }
        }

        public void RemoveWatchFolder(string dir)
        {
            if (_scanningThread != null && _scanningThread.IsAlive)
            {
                _scanningThread.Abort();
                _scanningThread.Join();
            }
            lock (_dbSyncObject)
            {
                int id = Convert.ToInt32(SQLiteHelper.ExecuteScalar(_connectionString, "SELECT WatchFolderID FROM WatchFolders WHERE Folder = '" + dir.Replace("'", "''") + "'"));
                SQLiteHelper.ExecuteNonQuery(_connectionString, "DELETE FROM Library WHERE WatchFolderID = " + id);
                SQLiteHelper.ExecuteNonQuery(_connectionString, "DELETE FROM WatchFolders WHERE WatchFolderID = " + id);
            }

            RefreshLibrary();
        }

        public string[] GetWatchFolders()
        {
            return GetStringArray("SELECT Folder FROM WatchFolders");
        }

        private void StartWatching(int watchFolderID, string dir)
        {
            if (!Directory.Exists(dir)) return;
            if (!_fileWatchers.ContainsKey(watchFolderID))
            {
                var watcher = new FileSystemWatcher();
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite;
                watcher.Path = dir;
                watcher.IncludeSubdirectories = true;
                watcher.Deleted += new FileSystemEventHandler(Watcher_Changed);
                watcher.Renamed += new RenamedEventHandler(Watcher_Renamed);
                watcher.Changed += new FileSystemEventHandler(Watcher_Changed);
                watcher.Created += new FileSystemEventHandler(Watcher_Changed);
                watcher.EnableRaisingEvents = true;
                _fileWatchers.Add(watchFolderID, watcher);
            }
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string sql = "UPDATE Library SET Filename = '" + e.FullPath.Replace("'", "''") + "' WHERE Filename = '" + e.OldFullPath.Replace("'", "''") + "'";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
            if (GetSong(e.FullPath) is LibraryEntry entry)
            {
                OnLibraryUpdated(entry);
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            int id = -1;
            foreach (int i in _fileWatchers.Keys)
            {
                if (Object.ReferenceEquals(_fileWatchers[i], sender))
                {
                    id = i;
                    break;
                }
            }
            if (id == -1)
            {
                throw new Exception("Couldnt find watch folder for: " + e.FullPath);
            }
            string path = e.FullPath;
            if (File.Exists(path))
            {
                AddOrUpdateFile(id, path);
            }
            else
            {
                if (e.ChangeType == WatcherChangeTypes.Deleted)
                {
                    if (Exists(e.FullPath))
                    {
                        string sql = "DELETE FROM Library WHERE Filename = '" + e.FullPath.Replace("'", "''") + "'";
                        lock (_dbSyncObject)
                        {
                            SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
                        }
                        _songCount--;
                        OnSongCountChanged();
                    }
                }
            }
        }

        public void SetSupportedExtensions(string[] exts)
        {
            foreach (string ext in exts)
            {
                _supportedExtensions.Add('.' + ext.Substring(ext.IndexOf('.') + 1).ToLower());
            }

            var del = new ThreadStart(RefreshLibrary);
            del.BeginInvoke(null, null);
        }

        private void ScanFoldersAsync()
        {
            if (_supportedExtensions.Count == 0)
            {
                throw new ArgumentException("You must set the supported extensions before you can scan for files.");
            }

            _scanningThread = new Thread(new ThreadStart(InternalScanFolder));
            _scanningThread.IsBackground = true;
            _scanningThread.Start();
        }

        private void InternalScanFolder()
        {
            OnScanStarting();

            try
            {
                for (int i = 0; i < _watchFolders.Count; i++)
                {
                    ScanFolder(_watchFolders[i]);
                }

                // remove anything left in existingFiles;
                foreach (string s in _existingFiles.Keys)
                {
                    lock (_dbSyncObject)
                    {
                        SQLiteHelper.ExecuteNonQuery(_connectionString, "DELETE FROM Library WHERE Filename = '" + s.Replace("'", "''") + "'");
                    }
                    _songCount--;
                    OnSongCountChanged();
                }
            }
            finally
            {

                OnScanFinished();
                _watchFolders.Clear();
            }
        }

        private void ScanFolder(DataRow directory)
        {
            if (!Directory.Exists(directory["Folder"].ToString())) return;

            string oldDir = "";
            foreach (String file in FileSearcher.GetFiles(new DirectoryInfo(directory["Folder"].ToString()), "*", SearchOption.AllDirectories))
            {
                var dir = Path.GetDirectoryName(file);

                if (!dir.Equals(oldDir))
                {
                    OnScanStatus(dir);
                    oldDir = dir;
                }
                AddOrUpdateFile(Convert.ToInt32(directory["WatchFolderID"]), file);
                Thread.Sleep(0);
            }
        }

        private void AddOrUpdateFile(int watchFolderID, string file)
        {
            if (_supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
            {
                NeedsUpdateResult result = NeedsUpdate(file);

                if (result == NeedsUpdateResult.FileNotFound)
                {
                    Add(watchFolderID, file);
                }
                else if (result == NeedsUpdateResult.NeedsUpdate)
                {
                    try
                    {
                        Update(file);
                    }
                    catch { }
                }
                _existingFiles.Remove(file);
            }
        }

        public void UpdateIfNeeded(string filename)
        {
            NeedsUpdateResult result = NeedsUpdate(filename);
            if (result == NeedsUpdateResult.NeedsUpdate)
            {
                try
                {
                    Update(filename);
                }
                catch { }
            }
        }

        #endregion

        #region Database Methods

        private void Update(string file)
        {
            var entry = new LibraryEntry(file);

            Update(entry);
        }

        public void Update(LibraryEntry entry)
        {
            if (entry == null) return;

            var parameters = new SQLiteParameter[] {
                new SQLiteParameter("@pFileName", entry.FileName),
                new SQLiteParameter("@pArtist", entry.Artist),
                new SQLiteParameter("@pTitle", entry.Title),
                new SQLiteParameter("@pAlbum", entry.Album),
                new SQLiteParameter("@pYear", entry.Year),
                new SQLiteParameter("@pGenre", entry.Genre),
                new SQLiteParameter("@pAlbumArtist", entry.AlbumArtist),
                new SQLiteParameter("@pTrackNumber", entry.TrackNumber),
                new SQLiteParameter("@pDuration", entry.Duration)
            };

            lock (_dbSyncObject)
            {
                string datetimeString = "datetime('now', 'localtime')";
                //if (String.IsNullOrEmpty(entry.Artist))
                //{
                //    datetimeString = "datetime('1980-01-01')";
                //}
                string sql = "" +
                    "UPDATE Library SET " +
                    "    DateAdded = " + datetimeString + ", " +
                    "    Artist = @pArtist, " +
                    "    Title = @pTitle, " +
                    "    Album = @pAlbum, " +
                    "    TrackNumber = @pTrackNumber, " +
                    "    Year = @pYear, " +
                    "    AlbumArtist = @pAlbumArtist, " +
                    "    Genre = @pGenre, " +
                    "    Duration = @pDuration " +
                    "WHERE Filename = @pFileName ";
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql, parameters);
            }

            OnLibraryUpdated(GetSong(entry.FileName) as LibraryEntry);
        }

        public void AddInternetRadio(string url)
        {
            var parameters = new SQLiteParameter[1];
            parameters[0] = new SQLiteParameter("@pUrl", url);
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, "INSERT INTO InternetRadio (Url, DateAdded, PlayCount) VALUES (@pUrl, datetime('now', 'localtime'), 0)", parameters);
            }
        }

        public void UpdateInternetRadio(string url, string title)
        {
            var parameters = new SQLiteParameter[2];
            parameters[0] = new SQLiteParameter("@pUrl", url);
            parameters[1] = new SQLiteParameter("@pTitle", title);
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, "UPDATE InternetRadio SET Title = @pTitle WHERE Url = @pUrl", parameters);
            }
        }


        public void DeleteInternetRadio(string url)
        {
            var parameters = new SQLiteParameter[1];
            parameters[0] = new SQLiteParameter("@pUrl", url);
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, "DELETE FROM InternetRadio WHERE Url = @pUrl", parameters);
            }
        }

        private void Add(int watchFolderID, string file)
        {
            // add to our in-memory library first
            _songCount++;
            OnSongCountChanged();

            var parameters = new SQLiteParameter[2];
            parameters[0] = new SQLiteParameter("@pFileName", file);
            parameters[1] = new SQLiteParameter("@pWatchFolderID", watchFolderID);

            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, "INSERT INTO Library (WatchFolderID, Filename, DateAdded, PlayCount, Ignored, Deleted) VALUES (@pWatchFolderID, @pFileName, datetime('now', 'localtime'), 0, 0, 0)", parameters);
            }
            Update(file);
        }

        private bool Exists(string file)
        {
            var parameters = new SQLiteParameter[1];
            parameters[0] = new SQLiteParameter("@pFileName", file);
            lock (_dbSyncObject)
            {
                return Convert.ToBoolean(SQLiteHelper.ExecuteScalar(_connectionString, "SELECT COUNT(LibraryID) FROM Library WHERE Filename = @pFileName", parameters));
            }
        }

        void ILibrary.UpdatePlayDate(string filename)
        {
            var parameters = new SQLiteParameter[1];
            parameters[0] = new SQLiteParameter("@pFileName", filename);
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, "UPDATE Library SET DatePlayed = datetime('now', 'localtime') WHERE Filename = @pFileName", parameters);
            }
        }

        void ILibrary.UpdatePlayCount(string filename)
        {
            var parameters = new SQLiteParameter[1];
            parameters[0] = new SQLiteParameter("@pFileName", filename);
            int i;
            lock (_dbSyncObject)
            {
                if (filename.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                {
                    i = SQLiteHelper.ExecuteNonQuery(_connectionString, "UPDATE InternetRadio SET PlayCount = PlayCount + 1 WHERE Url = @pFileName", parameters);
                }
                else
                {
                    i = SQLiteHelper.ExecuteNonQuery(_connectionString, "UPDATE Library SET PlayCount = PlayCount + 1 WHERE Filename = @pFileName", parameters);
                }
            }
            if (i > 0)
            {
                OnPlayCountUpdated(GetSong(filename) as LibraryEntry);
            }
        }

        public void Delete(string filename)
        {
            var parameters = new SQLiteParameter[1];
            parameters[0] = new SQLiteParameter("@pFilename", filename);
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, "UPDATE Library SET Deleted = NOT Deleted WHERE Filename = @pFilename", parameters);
            }
            OnLibraryUpdated(GetSong(filename) as LibraryEntry);
            OnSongCountChanged();
        }

        public void Ignore(string filename)
        {
            var parameters = new SQLiteParameter[1];
            parameters[0] = new SQLiteParameter("@pFilename", filename);
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, "UPDATE Library SET Ignored = 1 WHERE Filename = @pFilename", parameters);
            }
            OnLibraryUpdated(GetSong(filename) as LibraryEntry);
        }

        public void UnIgnore(string filename)
        {
            var parameters = new SQLiteParameter[1];
            parameters[0] = new SQLiteParameter("@pFilename", filename);
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, "UPDATE Library SET Ignored = 0 WHERE Filename = @pFilename", parameters);
            }
            OnLibraryUpdated(GetSong(filename) as LibraryEntry);
        }

        private NeedsUpdateResult NeedsUpdate(string file)
        {
            var parameters = new SQLiteParameter[1];
            parameters[0] = new SQLiteParameter("@pFileName", file);
            object o;
            lock (_dbSyncObject)
            {
                o = SQLiteHelper.ExecuteScalar(_connectionString, "SELECT DateAdded FROM Library WHERE Filename = @pFileName", parameters);
            }
            if (o != null && o != DBNull.Value)
            {
                var date = Convert.ToDateTime(o);
                DateTime fileDate = File.GetLastWriteTime(file);
                if (date < fileDate)
                {
                    return NeedsUpdateResult.NeedsUpdate;
                }
                else
                {
                    SongInfo info = GetSong(file);
                    if (string.IsNullOrEmpty(info.Title) || string.IsNullOrEmpty(info.Artist) || info.Title.Equals(Path.GetFileName(file), StringComparison.OrdinalIgnoreCase))
                    {
                        return NeedsUpdateResult.NeedsUpdate;
                    }
                    else
                    {
                        return NeedsUpdateResult.DoesntNeedUpdate;
                    }
                }
            }
            else
            {
                // No date, so it needs updating
                return NeedsUpdateResult.FileNotFound;
            }
        }

        #endregion

        #region Library Methods

        public LibraryEntry[] GetLibrary()
        {
            return GetLibrary(string.Empty);
        }

        public LibraryEntry[] GetLibrary(string filter)
        {
            return GetLibrary(filter, -1, false);
        }

        public LibraryEntry[] GetLibrary(string filter, int limit)
        {
            return GetLibrary(filter, limit, false);
        }

        public LibraryEntry[] GetLibrary(string filter, int limit, bool splitWords)
        {
            return GetLibrary(filter, limit, splitWords, -1);
        }

        public LibraryEntry[] GetLibrary(string filter, int limit, bool splitWords, int startRecord)
        {
            return GetLibrary(filter, limit, splitWords, "", startRecord);
        }

        public LibraryEntry[] GetLibrary(string filter, int limit, bool splitWords, string searchColumn)
        {
            return GetLibrary(filter, limit, splitWords, searchColumn, -1);
        }

        public LibraryEntry[] GetLibrary(string filter, int limit, bool splitWords, string searchColumn, int startRecord)
        {
            string sql = "SELECT * FROM Library WHERE Deleted = 0 ";
            if (!string.IsNullOrEmpty(filter))
            {
                string[] filters;
                if (splitWords)
                {
                    filters = filter.Split(' ');
                }
                else
                {
                    filters = new string[] { filter };
                }
                foreach (string filt in filters)
                {
                    if (!string.IsNullOrEmpty(filt))
                    {
                        if (!string.IsNullOrEmpty(searchColumn))
                        {
                            if (filt.StartsWith("-"))
                            {
                                // Search the specified columns
                                sql += " AND NOT (" + searchColumn + " LIKE '%" + filt.Substring(1).Replace("'", "''") + "%') ";
                            }
                            else
                            {
                                // Search the specified columns
                                sql += " AND (" + searchColumn + " LIKE '%" + filt.Replace("'", "''") + "%') ";
                            }
                        }
                        else
                        {
                            if (filt.StartsWith("-"))
                            {
                                // Search all columns
                                sql += " AND NOT ( ";
                                sql += "Filename LIKE '%" + filt.Substring(1).Replace("'", "''") + "%' ";
                                sql += " OR ";
                                sql += "Title LIKE '%" + filt.Substring(1).Replace("'", "''") + "%' ";
                                sql += " OR ";
                                sql += "Album LIKE '%" + filt.Substring(1).Replace("'", "''") + "%' ";
                                sql += " OR ";
                                sql += "Artist LIKE '%" + filt.Substring(1).Replace("'", "''") + "%' ";
                                sql += " OR ";
                                sql += "AlbumArtist LIKE '%" + filt.Substring(1).Replace("'", "''") + "%' ";
                                sql += ") ";
                            }
                            else
                            {
                                // Search all columns
                                sql += " AND ( ";
                                sql += "Filename LIKE '%" + filt.Replace("'", "''") + "%' ";
                                sql += " OR ";
                                sql += "Title LIKE '%" + filt.Replace("'", "''") + "%' ";
                                sql += " OR ";
                                sql += "Album LIKE '%" + filt.Replace("'", "''") + "%' ";
                                sql += " OR ";
                                sql += "Artist LIKE '%" + filt.Replace("'", "''") + "%' ";
                                sql += " OR ";
                                sql += "AlbumArtist LIKE '%" + filt.Replace("'", "''") + "%' ";
                                sql += ") ";
                            }
                        }
                    }
                }
            }
            sql += "ORDER BY Album, AlbumArtist, WatchFolderID, TrackNumber";
            if (limit > 0)
            {
                sql += " LIMIT " + limit + (startRecord <= 0 ? "" : ", " + startRecord);
            }
            return GetLibraryEntryArray(sql);
        }

        public SongInfo GetSong(int index)
        {
            if (index < this.SongCount)
            {
                string sql = "SELECT * FROM Library WHERE Deleted = 0 LIMIT 1 OFFSET " + index;
                LibraryEntry[] result = GetLibraryEntryArray(sql);
                if (result == null || result.Length == 0)
                {
                    return null;
                }
                else
                {
                    return result[0];
                }
            }
            return null;
        }

        public SongInfo GetRandomSong(bool returnIgnoredSongs, bool returnPreviouslyPlayedSongs)
        {
            while (true)
            {
                int i = _random.Next(0, this.SongCount);
                SongInfo song = GetSong(i);
                bool found = false;
                foreach (string fold in NonExistantFolders)
                {
                    if (song.FileName.StartsWith(fold))
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    continue;
                }
                if (!returnIgnoredSongs && song.Ignored)
                {
                    continue;
                }
                if (!returnPreviouslyPlayedSongs)
                {
                    if (GetPlayCount(song) != 0)
                    {
                        continue;
                    }
                }
                return song;
            }
        }



        public SongInfo GetSong(string fileName)
        {
            string sql = "SELECT * FROM Library WHERE Filename = '" + fileName.Replace("'", "''") + "';";
            LibraryEntry[] result = GetLibraryEntryArray(sql);
            if (result == null || result.Length == 0)
            {
                return null;
            }
            else
            {
                return result[0];
            }
        }

        public void ClearAllPlayCounts()
        {
            string sql = "UPDATE Library SET PlayCount = 0;";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
        }

        public void ClearAllDateAddeds()
        {
            string sql = "UPDATE Library SET DateAdded = date('1980-01-01');";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
        }

        public int GetPlayCount(SongInfo song)
        {
            if (song is LibraryEntry entry)
            {
                return entry.PlayCount;
            }
            else
            {
                return GetPlayCount(song.FileName);
            }
        }

        public int GetPlayCount(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return 0;

            string sql = "SELECT PlayCount FROM Library WHERE Filename = '" + filename.Replace("'", "''") + "';";
            object o;
            lock (_dbSyncObject)
            {
                o = SQLiteHelper.ExecuteScalar(_connectionString, sql);
            }
            if (o == null || o == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(o);
            }
        }

        public LibraryEntry[] GetByPlayCount(int num)
        {
            string sql = "SELECT * FROM Library WHERE Deleted = 0 ORDER BY PlayCount DESC LIMIT " + num;
            return GetLibraryEntryArray(sql);
        }

        public LibraryEntry[] GetBypDatePlayed(int num)
        {
            string sql = "SELECT * FROM Library WHERE Deleted = 0 ORDER BY DatePlayed DESC LIMIT " + num;
            return GetLibraryEntryArray(sql);
        }

        public LibraryEntry[] GetBypDateAdded(int num)
        {
            string sql = "SELECT * FROM Library WHERE Deleted = 0 ORDER BY DateAdded DESC LIMIT " + num;
            return GetLibraryEntryArray(sql);
        }

        public LibraryEntry[] GetOldByPlayCount(int num)
        {
            string sql = "SELECT * FROM Library WHERE Deleted = 0 AND DatePlayed < date('now','-30 day') AND DatePlayed > date('now','-90 day') ORDER BY PlayCount DESC LIMIT " + num;
            return GetLibraryEntryArray(sql);
        }

        public string[] GetAlbumArtists()
        {
            return GetStringArray("SELECT DISTINCT AlbumArtist FROM Library WHERE Deleted = 0");
        }

        public string[] GetArtists()
        {
            return GetStringArray("SELECT DISTINCT Artist FROM Library WHERE Deleted = 0");
        }

        public string[] GetAlbums()
        {
            return GetStringArray("SELECT DISTINCT Album FROM Library WHERE Deleted = 0");
        }

        public LibraryEntry[] GetAlbumsAsEntries()
        {
            string sql = "SELECT DISTINCT MIN(Filename) AS Filename, Album, AlbumArtist FROM Library WHERE Deleted = 0 GROUP BY Album, AlbumArtist ORDER BY Lower(AlbumArtist), LOWER(Album)";
            DataSet ds;
            lock (_dbSyncObject)
            {
                ds = SQLiteHelper.ExecuteDataSet(_connectionString, sql);
            }
            var result = new LibraryEntry[ds.Tables[0].Rows.Count];
            for (int i = 0; i < result.Length; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                result[i] = new LibraryEntry(dr["Filename"].ToString(), dr["AlbumArtist"].ToString(), dr["Album"].ToString(), "", 0, 0, "", dr["AlbumArtist"].ToString());
            }

            return result;
        }

        public LibraryEntry[] GetInternetRadios()
        {
            string sql = "SELECT Url, Title FROM InternetRadio ORDER BY Lower(Artist), LOWER(Title)";
            DataSet ds;
            lock (_dbSyncObject)
            {
                ds = SQLiteHelper.ExecuteDataSet(_connectionString, sql);
            }
            var result = new LibraryEntry[ds.Tables[0].Rows.Count];
            for (int i = 0; i < result.Length; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                result[i] = new LibraryEntry(dr["Url"].ToString(), "", "", dr["Title"].ToString(), 0, 0, "", "");
            }

            return result;
        }

        public LibraryEntry[] GetDeletedSongs()
        {
            string sql = "SELECT * FROM Library WHERE Deleted = 1";
            return GetLibraryEntryArray(sql);
        }

        public string[] GetAlbums(string artist)
        {
            return GetStringArray("SELECT DISTINCT Album FROM Library WHERE Deleted = 0 AND AlbumArtist LIKE '" + artist.Replace("'", "''") + "'");
        }

        public string[] GetGenres()
        {
            return GetStringArray("SELECT DISTINCT Genre FROM Library WHERE Deleted = 0");
        }

        public string[] GetYears()
        {
            return GetStringArray("SELECT DISTINCT Year FROM Library WHERE Deleted = 0 AND NOT Year ISNULL AND Year >= 0");
        }

        public LibraryEntry[] QueryLibrary(string criteria)
        {
            return QueryLibrary(criteria, "Album, AlbumArtist, WatchFolderID, TrackNumber", false);
        }

        public LibraryEntry[] QueryLibrary(string criteria, bool hideIgnoredTracks)
        {
            return QueryLibrary(criteria, "Album, AlbumArtist, WatchFolderID, TrackNumber", hideIgnoredTracks);
        }

        public LibraryEntry[] QueryLibrary(string criteria, string orderby, bool hideIgnoredTracks)
        {
            string sql = "SELECT * FROM Library WHERE Deleted = 0 AND ";
            if (hideIgnoredTracks)
            {
                sql += " Ignored = 0 AND ";
            }
            sql += "(" + criteria + ")";
            sql += " ORDER BY " + orderby;
            return GetLibraryEntryArray(sql);
        }

        private LibraryEntry[] GetLibraryEntryArray(string sql)
        {
            DataSet ds;
            lock (_dbSyncObject)
            {
                ds = SQLiteHelper.ExecuteDataSet(_connectionString, sql);
            }
            var result = new LibraryEntry[ds.Tables[0].Rows.Count];
            for (int i = 0; i < result.Length; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                result[i] = new LibraryEntry(dr);
            }

            return result;
        }

        private string[] GetStringArray(string sql)
        {
            DataSet ds;
            lock (_dbSyncObject)
            {
                ds = SQLiteHelper.ExecuteDataSet(_connectionString, sql);
            }
            string[] result = new string[ds.Tables[0].Rows.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = ds.Tables[0].Rows[i][0].ToString();
            }

            return result;
        }

        #endregion

        #region Database Version Methods

        private void UpdateDB()
        {
            string sql = "SELECT COUNT(*) FROM sqlite_master WHERE Name = 'Library' and type = 'table'";
            object o;
            lock (_dbSyncObject)
            {
                o = SQLiteHelper.ExecuteScalar(_connectionString, sql);
            }
            if (Convert.ToInt32(o) == 0)
            {
                CreateDB();
            }

            sql = "SELECT Version FROM System";

            bool upToDate = false;
            while (!upToDate)
            {
                lock (_dbSyncObject)
                {
                    o = SQLiteHelper.ExecuteScalar(_connectionString, sql);
                }

                switch (Convert.ToInt32(o))
                {
                    case 1:
                    {
                        UpgradeToVersion2();
                        break;
                    }
                    case 2:
                    {
                        UpgradeToVersion3();
                        break;
                    }
                    case 3:
                    {
                        UpgradeToVersion4();
                        break;
                    }
                    case 4:
                    {
                        UpgradeToVersion5();
                        break;
                    }
                    case 5:
                    {
                        UpgradeToVersion6();
                        break;
                    }
                    case 6:
                    {
                        UpgradeToVersion7();
                        break;
                    }
                    case 7:
                    {
                        UpgradeToVersion8();
                        break;
                    }
                    case 8:
                    {
                        UpgradeToVersion9();
                        break;
                    }
                    case 9:
                    {
                        UpgradeToVersion10();
                        break;
                    }
                    case 10:
                    {
                        UpgradeToVersion11();
                        break;
                    }
                    case 11:
                    {
                        // Do nothing - up to date
                        upToDate = true;
                        break;
                    }
                    default:
                    {
                        throw new NotSupportedException("Version number " + o.ToString() + " not supported.");
                    }
                }
            }
        }

        private void SetVersion(int verNumber)
        {
            string sql = "UPDATE System SET Version = " + verNumber;
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
        }

        private void UpgradeToVersion2()
        {
            //            string sql = @"
            //                -- Ignored Artists table
            //                CREATE TABLE IgnoredArtists (IgnoredArtistsID integer primary key, Artist varchar);
            //                CREATE INDEX IgnoredArtists_Artist ON IgnoredArtists(Artist);
            //                    ";
            //            lock (m_dbSyncObject)
            //            {
            //                SQLiteHelper.ExecuteNonQuery(m_connectionString, sql);
            //            }
            SetVersion(2);
        }

        private void UpgradeToVersion3()
        {
            string sql = @"
                -- Add some Library columns and indexes
                CREATE INDEX Library_Album ON Library(Album);
                ALTER TABLE Library ADD Genre varchar;
                ALTER TABLE Library ADD Year int;
                CREATE INDEX Library_PlayCount ON Library(PlayCount);
                CREATE INDEX Library_Genre ON Library(Genre);
                CREATE INDEX Library_Year ON Library(Year);
                CREATE INDEX Library_DateAdded ON Library(DateAdded);
                    ";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
            SetVersion(3);
        }

        private void UpgradeToVersion4()
        {
            string sql = @"
                -- Add AlbumArtist column
                ALTER TABLE Library ADD AlbumArtist varchar;
                CREATE INDEX Library_AlbumArtist ON Library(AlbumArtist);
                    ";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
            SetVersion(4);
        }

        private void UpgradeToVersion5()
        {
            string sql = @"
                -- Clear library so that AlbumArtist gets set
                UPDATE Library SET DateAdded = date('1980-01-01');
                    ";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
            SetVersion(5);
        }

        private void UpgradeToVersion6()
        {
            string sql = @"
                -- Add AlbumArtist column
                ALTER TABLE Library ADD Duration real;
                CREATE INDEX Library_Duration ON Library(Duration);
                -- Clear library so that Duration gets set
                UPDATE Library SET DateAdded = date('1980-01-01');
                    ";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
            SetVersion(6);
        }

        private void UpgradeToVersion7()
        {
            string sql = @"
                -- Add Ignored column
                ALTER TABLE Library ADD Ignored bit;
                CREATE INDEX Library_Ignored ON Library(Ignored);
                -- Clear library
                UPDATE Library SET Ignored = 0;
                UPDATE Library SET Ignored = 1 WHERE Artist IN (SELECT Artist FROM IgnoredArtists);
                UPDATE Library SET Ignored = 1 WHERE AlbumArtist IN (SELECT Artist FROM IgnoredArtists);
                -- Remove IgnoredArtists Table
                DELETE FROM IgnoredArtists;
                DROP TABLE IgnoredArtists;
                    ";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
            SetVersion(7);
        }

        private void UpgradeToVersion8()
        {
            string sql = @"
                -- Add Deleted column
                ALTER TABLE Library ADD Deleted bit;
                CREATE INDEX Library_Deleted ON Library(Deleted);
                -- Clear library
                UPDATE Library SET Deleted = 0;
                    ";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
            SetVersion(8);
        }

        private void UpgradeToVersion9()
        {
            string sql = @"
                -- Fix library
                UPDATE Library SET Deleted = 0 WHERE Deleted IS NULL;
                UPDATE Library SET Ignored = 0 WHERE Ignored IS NULL;
                    ";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
            SetVersion(9);
        }

        private void UpgradeToVersion10()
        {
            string sql = @"
                -- Add Lyrics field and index
                ALTER TABLE Library ADD Lyrics varchar;
                CREATE INDEX Library_Lyrics ON Library(Lyrics);
                    ";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
            SetVersion(10);
        }

        private void UpgradeToVersion11()
        {
            string sql = @"
                -- Add Internet Radio Table
                CREATE TABLE InternetRadio (InternetRadioID integer primary key, Url varchar(255), DateAdded datetime, DatePlayed datetime, PlayCount integer, Title varchar, Artist varchar);
                CREATE INDEX InternetRadio_Artist ON InternetRadio(Artist);
                CREATE INDEX InternetRadio_Title ON InternetRadio(Title);
                CREATE INDEX InternetRadio_DatePlayed ON InternetRadio(DatePlayed);
                CREATE INDEX InternetRadio_Url ON InternetRadio(Url);
                CREATE INDEX InternetRadio_InternetRadioID ON InternetRadio(InternetRadioID);
                    ";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
            SetVersion(11);
        }

        private void CreateDB()
        {
            string sql = @"
                -- System table
                CREATE TABLE System (Version integer); 
                INSERT INTO System (Version) VALUES (1);
                -- Library
                CREATE TABLE Library (LibraryID integer primary key, WatchFolderID integer, Filename varchar(255), DateAdded datetime, DatePlayed datetime, PlayCount integer, Title varchar, Artist varchar, Album varchar, TrackNumber integer);
                CREATE INDEX Library_Artist ON Library(Artist);
                CREATE INDEX Library_Title ON Library(Title);
                CREATE INDEX Library_DatePlayed ON Library(DatePlayed);
                CREATE INDEX Library_Filename ON Library(Filename);
                CREATE INDEX Library_LibraryID ON Library(LibraryID);
                CREATE INDEX Library_WatchFolderID ON Library(WatchFolderID);
                -- WatchFolders
                CREATE TABLE WatchFolders(WatchFolderID integer primary key, Folder varchar(255));
                CREATE TABLE IgnoredArtists(Artist varchar);
                    ";
            lock (_dbSyncObject)
            {
                SQLiteHelper.ExecuteNonQuery(_connectionString, sql);
            }
        }

        #endregion

        #region Developer Special Functions

        public DataSet GetDataSet(string sql)
        {
            lock (_dbSyncObject)
            {
                return SQLiteHelper.ExecuteDataSet(_connectionString, sql);
            }
        }

        #endregion

        public void SetLyrics(string title, string artist, string lyrics)
        {
            if (lyrics == null || lyrics.Trim().Length == 0)
            {
                //System.Diagnostics.Debugger.Break();
                return;
            }
            var parameters = new SQLiteParameter[3];
            parameters[0] = new SQLiteParameter("@pTitle", title);
            parameters[1] = new SQLiteParameter("@pArtist", artist);
            parameters[2] = new SQLiteParameter("@pLyrics", lyrics);
            int i;
            lock (_dbSyncObject)
            {
                i = SQLiteHelper.ExecuteNonQuery(_connectionString, "UPDATE Library SET Lyrics = @pLyrics WHERE Title LIKE @pTitle AND Artist LIKE @pArtist", parameters);
            }
        }

        public string GetLyrics(string title, string artist)
        {
            string sql = "SELECT Lyrics FROM Library WHERE Title LIKE @pTitle AND Artist LIKE @pArtist AND (Lyrics <> '' AND Lyrics IS NOT NULL) LIMIT 1";
            var parameters = new SQLiteParameter[2];
            parameters[0] = new SQLiteParameter("@pTitle", title);
            parameters[1] = new SQLiteParameter("@pArtist", artist);
            string lyrics;
            lock (_dbSyncObject)
            {
                lyrics = (SQLiteHelper.ExecuteScalar(_connectionString, sql, parameters) ?? "").ToString();
            }
            return lyrics;
        }
    }
}
