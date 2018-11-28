using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using ThreePM.MusicPlayer;

namespace ThreePM.UI
{
    public partial class StatisticsControl : UserControl
    {
        private Player _player;
        private MusicLibrary.Library _library;
        private DataSet _statistics;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Player Player
        {
            get { return _player; }
            set
            {
                _player = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MusicLibrary.Library Library
        {
            get { return _library; }
            set
            {
                _library = value;
                ShowStats();
            }
        }

        public StatisticsControl()
        {
            InitializeComponent();
        }

        public void ShowStats()
        {
            string sql = "";
            if (this.Player != null)
            {
                sql += @"
				SELECT 'Songs played this session', '" + this.Player.HistoryCount + @"';
				SELECT 'Songs in playlist', '" + this.Player.Playlist.Count + @"';";
            }

            sql += @"
				SELECT 'Total Songs', COUNT(LibraryID)  FROM Library;
				SELECT 'Total Plays', SUM(PlayCount) FROM Library;
				SELECT 'Total Distinct Songs Played', COUNT(LibraryID), ROUND((COUNT(LibraryID) * 100.0 )/ (SELECT COUNT(LibraryID)  FROM Library) , 2), 'percent' FROM Library WHERE PlayCount > 0;
				SELECT 'Total Songs Never Played', COUNT(LibraryID), ROUND((COUNT(LibraryID) * 100.0 )/ (SELECT COUNT(LibraryID)  FROM Library) , 2), 'percent' FROM Library WHERE PlayCount = 0;
				SELECT 'Total Ignored Tracks', COUNT(LibraryID), ROUND((COUNT(LibraryID) * 100.0 )/ (SELECT COUNT(LibraryID)  FROM Library) , 2), 'percent' FROM Library WHERE Ignored = 1;
				SELECT 'Total Deleted Tracks', COUNT(LibraryID), ROUND((COUNT(LibraryID) * 100.0 )/ (SELECT COUNT(LibraryID)  FROM Library) , 2), 'percent' FROM Library WHERE Deleted = 1;
				SELECT '', '';
				SELECT 'Total Distinct Artists', COUNT(*) FROM (SELECT DISTINCT Artist FROM Library);
				SELECT 'Total Distinct Album Artists', COUNT(*) FROM (SELECT DISTINCT AlbumArtist FROM Library);
				SELECT 'Total Distinct Albums', COUNT(*) FROM (SELECT DISTINCT Album FROM Library);
				SELECT 'Total Distinct Titles', COUNT(*) FROM (SELECT DISTINCT Title FROM Library);
				SELECT '', '';
				SELECT 'Songs with lyrics',  COUNT(LibraryID) FROM Library WHERE Lyrics <> '';
				SELECT '', '';
				SELECT 'Average song length', AVG(Duration) || ' ms', AVG(Duration), 'secs' FROM Library WHERE Ignored = 0;
				SELECT 'Longest 5 songs', Artist || ' - ' || Title, Duration, 'secs' FROM Library WHERE Ignored = 0 ORDER BY Duration DESC LIMIT 5;
				SELECT 'Shortest 5 songs', Artist || ' - ' || Title, Duration, 'secs' FROM Library WHERE Duration > 0 AND Ignored = 0 ORDER BY Duration ASC LIMIT 5;
				SELECT '', '';
				SELECT 'Most Common Artist', Artist, COUNT(LibraryID), 'tracks' FROM Library GROUP BY Artist ORDER BY COUNT(LibraryID) DESC LIMIT 1;
				SELECT 'Most Common Song Title', Title, COUNT(LibraryID), 'tracks' FROM Library GROUP BY Title ORDER BY COUNT(LibraryID) DESC LIMIT 1;
				SELECT 'Most Common Album Title', Album, COUNT(LibraryID), 'tracks' FROM Library GROUP BY Album ORDER BY COUNT(LibraryID) DESC LIMIT 1;
				SELECT 'Most Common Album Artist', AlbumArtist, COUNT(LibraryID), 'tracks' FROM Library GROUP BY AlbumArtist ORDER BY COUNT(LibraryID) DESC LIMIT 1;
				SELECT 'Most Common Track Number', TrackNumber, COUNT(LibraryID), 'tracks' FROM Library GROUP BY TrackNumber ORDER BY COUNT(LibraryID) DESC LIMIT 1;
				SELECT '', '';
				SELECT 'Track Count by Folder', Folder, COUNT(LibraryID), 'tracks' FROM Library INNER JOIN WatchFolders ON WatchFolders.WatchFolderID = Library.WatchFolderID GROUP BY Folder ORDER BY COUNT(LibraryID) DESC;
				SELECT 'Track Count by Artist (Top 5 only)', Artist, COUNT(LibraryID), 'tracks' FROM Library GROUP BY Artist ORDER BY COUNT(LibraryID) DESC LIMIT 5;
				SELECT 'Track Count by Album Title (Top 5 only)', Album, COUNT(LibraryID), 'tracks' FROM Library GROUP BY Album ORDER BY COUNT(LibraryID) DESC LIMIT 5;
				SELECT 'Track Count by Album Artist (Top 5 only)', AlbumArtist, COUNT(LibraryID), 'tracks' FROM Library GROUP BY AlbumArtist ORDER BY COUNT(LibraryID) DESC LIMIT 5;
				SELECT 'Track Count by File Type (Top 5 only)', LOWER(SUBSTR(Filename, -4, 4)), COUNT(LibraryID), 'tracks' FROM Library GROUP BY LOWER(SUBSTR(Filename, -4, 4)) ORDER BY COUNT(LibraryID) DESC LIMIT 5;
				SELECT '', '';
				SELECT 'Play Count by Folder', Folder, SUM(PlayCount), 'plays' FROM Library INNER JOIN WatchFolders ON WatchFolders.WatchFolderID = Library.WatchFolderID GROUP BY Folder ORDER BY SUM(PlayCount) DESC;
				SELECT 'Play Count by Artist (Top 5 only)', Artist, SUM(PlayCount), 'plays' FROM Library GROUP BY Artist ORDER BY SUM(PlayCount) DESC LIMIT 5;
				SELECT 'Play Count by Album Title (Top 5 only)', Album, SUM(PlayCount), 'plays' FROM Library GROUP BY Album ORDER BY SUM(PlayCount) DESC LIMIT 5;
				SELECT 'Play Count by Album Artist (Top 5 only)', AlbumArtist, SUM(PlayCount), 'plays' FROM Library GROUP BY AlbumArtist ORDER BY SUM(PlayCount) DESC LIMIT 5;
				SELECT 'Play Count by Track Number (Top 5 only)', TrackNumber, SUM(PlayCount), 'plays' FROM Library GROUP BY TrackNumber ORDER BY SUM(PlayCount) DESC LIMIT 5;
				SELECT 'Play Count by File Type (Top 5 only)', LOWER(SUBSTR(Filename, -4, 4)), SUM(PlayCount), 'plays' FROM Library GROUP BY LOWER(SUBSTR(Filename, -4, 4)) ORDER BY SUM(PlayCount) DESC LIMIT 5;
				SELECT '', '';
				SELECT 'Tracks with no Artist', COUNT(LibraryID) FROM Library WHERE Artist ISNULL OR Artist = '';
				SELECT 'Tracks with no Title', COUNT(LibraryID) FROM Library WHERE Title ISNULL OR Title = '';
				SELECT 'Tracks with no Album Title', COUNT(LibraryID) FROM Library WHERE Album ISNULL OR Album = '';
				SELECT 'Tracks with no Album Artist', COUNT(LibraryID) FROM Library WHERE AlbumArtist ISNULL OR AlbumArtist = '';
				SELECT 'Tracks with no Track Number', COUNT(LibraryID) FROM Library WHERE TrackNumber <= 0;
				SELECT '', '';
				SELECT 'Tracks with missing a Tag (not including track number) by Folder', Folder, COUNT(LibraryID), 'tracks' FROM Library INNER JOIN WatchFolders ON WatchFolders.WatchFolderID = Library.WatchFolderID WHERE 
					(Artist ISNULL OR Artist = '')
					OR (Title ISNULL OR Title = '')
					OR (Album ISNULL OR Album = '')
					OR (AlbumArtist ISNULL OR AlbumArtist = '')
					GROUP BY Library.WatchFolderID;
				";

            MethodInvoker DoWork = delegate
            {
                _statistics = this.Library.GetDataSet(sql);
                if (this.Created) Invoke((MethodInvoker)delegate { DisplayData(); });
            };
            DoWork.BeginInvoke(null, null);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            DisplayData();
        }

        private void DisplayData()
        {
            lstStatistics.BeginUpdate();
            lstStatistics.Items.Clear();

            if (_statistics != null)
            {
                foreach (DataTable dt in _statistics.Tables)
                {
                    try
                    {
                        if (dt.Rows.Count > 1)
                        {
                            lstStatistics.Items.Add(dt.Rows[0][0].ToString() + ":");
                            foreach (DataRow dr in dt.Rows)
                            {
                                string s = "            " + dr[1].ToString();
                                if (dr.ItemArray.Length == 4)
                                {
                                    if (dr[3].ToString() == "secs")
                                    {
                                        s += " (" + ThreePM.MusicPlayer.Player.GetPositionDescription(Convert.ToSingle(dr[2])) + " " + dr[3].ToString() + ")";
                                    }
                                    else if (dr[3].ToString() == "percent")
                                    {
                                        s += " (" + dr[2].ToString() + "%)";
                                    }
                                    else
                                    {
                                        s += " (" + dr[2].ToString() + " " + dr[3].ToString() + ")";
                                    }
                                }
                                lstStatistics.Items.Add(s);
                            }
                        }
                        else if (dt.Rows.Count == 1)
                        {
                            DataRow dr = dt.Rows[0];
                            string s = dr[0].ToString() + ": " + dr[1].ToString();
                            if (dr.ItemArray.Length == 4)
                            {
                                if (dr[3].ToString() == "secs")
                                {
                                    s += " (" + ThreePM.MusicPlayer.Player.GetPositionDescription(Convert.ToSingle(dr[2])) + " " + dr[3].ToString() + ")";
                                }
                                else if (dr[3].ToString() == "percent")
                                {
                                    s += " (" + dr[2].ToString() + "%)";
                                }
                                else
                                {
                                    s += " (" + dr[2].ToString() + " " + dr[3].ToString() + ")";
                                }
                            }
                            if (s.Trim().Equals(":")) s = "";
                            lstStatistics.Items.Add(s);
                        }
                    }
                    catch
                    {
                        // lol
                    }
                }
            }
            lstStatistics.EndUpdate();
        }
    }
}
