using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading;
using Un4seen.Bass.AddOn.Tags;
using System.ComponentModel;

namespace ThreePM.player
{
	public class Library : ILibrary, IDisposable
	{
		#region Declarations

		private ISynchronizeInvoke m_synchronizingObject;
		private Dictionary<string, FileSystemWatcher> watchers = new Dictionary<string, FileSystemWatcher>();
		private List<string> extensions = new List<string>();
		private object syncObj = new object();
		private string connString;
		private Thread scanThread = null;
		private List<DirectoryInfo> watchFolders = new List<DirectoryInfo>();
        private DataTable m_library;
		private delegate void ScanFolderMethod(string folder, int waitTime);

		#region Events

		public event EventHandler ScanStarting;
		public event EventHandler<ScanStatusEventArgs> ScanStatus;
		public event EventHandler ScanFinished;
        public event EventHandler SongCountChanged;

		#endregion

		#endregion

		#region Properties

		public ISynchronizeInvoke SynchronizingObject
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

		public int SongCount
		{
			get
			{
                return m_library.Rows.Count;
			}
		}

		#endregion

		#region Constructor

		public Library()
		{
			connString = "Data Source=" + "library.db" + ";Compress=True;New=True;";

            lock (syncObj)
            {
                UpdateDB();
            }
		}

        private void RefreshLibrary()
        {
            DataSet watch;
            lock (syncObj)
            {
                watch = SQLiteHelper.ExecuteDataSet(connString, "SELECT Folder FROM WatchFolders");
            }
            string where = "";
            string indent = "";
            foreach (DataRow dr in watch.Tables[0].Rows)
            {
                DirectoryInfo dir = new DirectoryInfo(dr["Folder"].ToString());
                if (dir.Exists)
                {
                    where += indent + "Filename LIKE '" + dir.FullName + "%'";
                    indent = " OR ";
                    watchFolders.Add(dir);
                }
            }
            if (String.IsNullOrEmpty(where)) where = "0=1";
            DataSet ds;
            lock (syncObj)
            {
                ds = SQLiteHelper.ExecuteDataSet(connString, "SELECT Filename FROM Library WHERE " + where);
            }
            m_library = ds.Tables[0];
            OnSongCountChanged();

			foreach (DataRow dr in watch.Tables[0].Rows)
			{
				DirectoryInfo dir = new DirectoryInfo(dr["Folder"].ToString());
				if (dir.Exists)
				{
					StartWatching(dir);
				}
			}

            ScanFoldersAsync();

        }

		public void Dispose()
		{
			try
			{
                foreach (FileSystemWatcher watcher in watchers.Values)
                {
                    watcher.Dispose();
                }
                watchers = null;
				if (scanThread != null)
				{
					scanThread.Abort();
				}
			}
			catch { }
		}

		#endregion

		#region OnEvent Methods

		private void OnSongCountChanged()
		{
			EventHandler handler = SongCountChanged;
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

		private void OnScanFinished()
		{
			EventHandler handler = ScanFinished;
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

		private void OnScanStarting()
		{
			EventHandler handler = ScanStarting;
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
		
		private void OnScanStatus(string status)
		{
			EventHandler<ScanStatusEventArgs> handler = ScanStatus;
			if (handler != null)
			{
				ScanStatusEventArgs e = new ScanStatusEventArgs(status);
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



		#endregion

		#region Folder Scanning Methods

        public void AddWatchFolder(string dir)
        {
            int i;
            lock (syncObj)
            {
                i = Convert.ToInt32(SQLiteHelper.ExecuteScalar(connString, "SELECT COUNT(ID) FROM WatchFolders WHERE Folder = '" + dir + "'"));
            }
            if (i == 0)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                watchFolders.Add(dirInfo);
				StartWatching(dirInfo);
                lock (syncObj)
                {
                    SQLiteHelper.ExecuteNonQuery(connString, "INSERT INTO WatchFolders (Folder) VALUES ('" + dir + "')");
                }
                if (scanThread != null && !scanThread.IsAlive)
                {
                    ScanFoldersAsync();
                }
            }
        }

		public string[] GetWatchFolders()
		{
			DataSet watch;
			lock (syncObj)
			{
				watch = SQLiteHelper.ExecuteDataSet(connString, "SELECT Folder FROM WatchFolders");
			}
			string[] result = new string[watch.Tables[0].Rows.Count];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = watch.Tables[0].Rows[i]["Folder"].ToString();
			}
			return result;
		}

		private void StartWatching(DirectoryInfo dir)
		{
			if (!watchers.ContainsKey(dir.FullName))
			{
				FileSystemWatcher watcher = new FileSystemWatcher();
				watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.CreationTime | NotifyFilters.LastWrite;
				watcher.Path = dir.FullName;
				watcher.IncludeSubdirectories = true;
				watcher.Changed += new FileSystemEventHandler(watcher_Changed);
				watcher.Created += new FileSystemEventHandler(watcher_Changed);
				watcher.EnableRaisingEvents = true;
				watchers.Add(dir.FullName, watcher);
			}
		}

		public void SetSupportedExtensions(string[] exts)
		{
			foreach (string ext in exts)
			{
				extensions.Add('.' + ext.Substring(ext.IndexOf('.') + 1).ToLower());
			}

            RefreshLibrary();
        }

		private void ScanFoldersAsync()
		{
			if (extensions.Count == 0)
			{
				throw new ArgumentException("You must set the supported extensions before you can scan for files.");
			}

			scanThread = new Thread(new ThreadStart(delegate
			{
				try
				{
					InternalScanFolder(0);
				}
				catch { }
			}));
            scanThread.IsBackground = true;
			scanThread.Start();
		}

		private void InternalScanFolder(int waitTime)
		{
            for (int i = 0; i < watchFolders.Count; i++)
            {
                ScanFolder(waitTime, watchFolders[i]);
            }

            watchFolders.Clear();
		}

        private void ScanFolder(int waitTime, DirectoryInfo directory)
        {
			OnScanStarting();

			string oldDir = "";
            foreach (FileInfo file in FileSearcher.GetFiles(directory, "*", SearchOption.AllDirectories))
            {
                if (extensions.Contains(file.Extension.ToLower()))
                {
					if (file.DirectoryName != oldDir)
					{
						OnScanStatus(file.DirectoryName);
					}
                    AddOrUpdateFile(file);
                }
                if (waitTime != -1)
                {
                    Thread.Sleep(waitTime);
                }
            }

			OnScanFinished();
        }

        private void AddOrUpdateFile(FileInfo file)
        {
            if (false == Exists(file))
            {
                Add(file);
            }
            else
            {
                if (NeedsUpdate(file))
                {
                    Update(file);
                }
            }
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string path = e.FullPath;
            if (File.Exists(path))
            {
                AddOrUpdateFile(new FileInfo(path));
            }
            else
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                watchFolders.Add(dirInfo);
                if (scanThread != null && !scanThread.IsAlive)
                {
                    ScanFoldersAsync();
                }
            }
        }

		#endregion

		#region Database Methods

		public void Update(FileInfo file)
		{
			SQLiteParameter[] parameters = new SQLiteParameter[6];
			parameters[0] = new SQLiteParameter("@pFileName", file.FullName);
			parameters[1] = new SQLiteParameter("@pDateAdded", DateTime.Now);

			//TAG_INFO tag = new TAG_INFO(file.FullName);

			//parameters[2] = new SQLiteParameter("@pArtist", tag.artist);
			//parameters[3] = new SQLiteParameter("@pTitle", tag.title);
			//parameters[4] = new SQLiteParameter("@pAlbum", tag.album);
			//parameters[5] = new SQLiteParameter("@pTrackNumber", tag.track );
			//lock (syncObj)
			//{
			//    SQLiteHelper.ExecuteNonQuery(connString, "UPDATE Library SET DateAdded = @pDateAdded, Artist = @pArtist, Title = @pTitle, Album = @pAlbum, TrackNumber = @pTrackNumber WHERE Filename = @pFileName", parameters);
			//}
        }

		public void Add(FileInfo file)
		{
            // add to our in-memory library first
            DataRow dr = m_library.NewRow();
            dr["Filename"] = file;
            m_library.Rows.Add(dr);

            OnSongCountChanged();

			SQLiteParameter[] parameters = new SQLiteParameter[1];
			parameters[0] = new SQLiteParameter("@pFileName", file.FullName);
            lock (syncObj)
            {
                SQLiteHelper.ExecuteNonQuery(connString, "INSERT INTO Library (Filename) VALUES (@pFileName)", parameters);
            }
			Update(file);
		}


		public bool Exists(FileInfo file)
		{
			SQLiteParameter[] parameters = new SQLiteParameter[1];
			parameters[0] = new SQLiteParameter("@pFileName", file.FullName);
            lock (syncObj)
            {
                return Convert.ToBoolean(SQLiteHelper.ExecuteScalar(connString, "SELECT COUNT(ID) FROM Library WHERE Filename = @pFileName", parameters));
            }
        }

		public bool NeedsUpdate(FileInfo file)
		{
			SQLiteParameter[] parameters = new SQLiteParameter[1];
			parameters[0] = new SQLiteParameter("@pFileName", file.FullName);
            object o ;
            lock (syncObj)
            {
                o = SQLiteHelper.ExecuteScalar(connString, "SELECT DateAdded FROM Library WHERE Filename = @pFileName", parameters);
            }
            if (o != null && o != DBNull.Value)
			{
				DateTime date = Convert.ToDateTime(o);
				if (date < file.LastWriteTime)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				// No date, so it needs updating
				return true;
			}
		}

		#endregion

		#region Library Methods

		public DataSet GetLibrary()
		{
			return GetLibrary(string.Empty);
		}


		public DataSet GetLibrary(string filter)
		{
			string sql = "SELECT * FROM Library ";
			if (!String.IsNullOrEmpty(filter))
			{
				sql += "WHERE Filename LIKE '%" + filter.Replace("'", "''") + "%' ";
			}
			sql += "ORDER BY Filename";
            lock (syncObj)
            {
                return SQLiteHelper.ExecuteDataSet(connString, sql);
            }
		}

		public string GetFilename(int index)
		{
            if (index < SongCount)
            {
                return m_library.Rows[index]["Filename"].ToString();
            }
            return null;
		}

		public DataSet Query(string sql)
		{
            lock (syncObj)
            {
                return SQLiteHelper.ExecuteDataSet(connString, sql);
            }
		}

		#endregion

		#region Database Version Methods

		private void UpdateDB()
		{
			string sql = "SELECT COUNT(*) FROM sqlite_master WHERE Name = 'Library' and type = 'table'";
            object o = SQLiteHelper.ExecuteScalar(connString, sql);
			if (Convert.ToInt32(o) == 0)
			{
				CreateDB();
			}

			sql = "SELECT Version FROM System";
            o = SQLiteHelper.ExecuteScalar(connString, sql);
			switch (Convert.ToInt32(o))
			{
				case 1:
				{
					UpgradeToVersion2();
					goto case 2;
				}
				case 2:
				{
					UpgradeToVersion3();
					goto case 3;
				}
				case 3:
				{
					SetVersion(3);
					break;
				}
				default:
				{
					throw new NotSupportedException("Version number " + o.ToString() + " not supported.");
				}
			}
		}

		private void SetVersion(int verNumber)
		{
			string sql = "UPDATE System SET Version = " + verNumber;
			SQLiteHelper.ExecuteNonQuery(connString, sql);
		}
		
		private void UpgradeToVersion2()
		{
			string sql = "CREATE INDEX Library_Filename ON Library(Filename);" +
						 "CREATE INDEX Library_ID ON Library(ID);";
			SQLiteHelper.ExecuteNonQuery(connString, sql);
		}
		
		private void UpgradeToVersion3()
		{
			string sql = "CREATE TABLE WatchFolders(ID integer primary key, Folder varchar(255));";
			SQLiteHelper.ExecuteNonQuery(connString, sql);
		}

		private void CreateDB()
		{
			string sql = "CREATE TABLE Library (ID integer primary key, Filename varchar(255), DateAdded datetime, DatePlayed datetime, PlayCount integer, Title varchar, Artist varchar, Album varchar, TrackNumber integer); " +
				"CREATE INDEX Library_Artist ON Library(Artist);" +
				"CREATE INDEX Library_Title ON Library(Title);" +
				"CREATE INDEX Library_DatePlayed ON Library(DatePlayed);" + 
				"CREATE TABLE System (Version integer); " +
				"INSERT INTO System (Version) VALUES (1);";
            SQLiteHelper.ExecuteNonQuery(connString, sql);			
		}

		#endregion

    }
}
