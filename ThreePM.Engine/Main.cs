using System;
using System.IO;
using ThreePM.MusicLibrary;
using ThreePM.MusicPlayer;

namespace ThreePM.Engine
{
    public static class Main
    {
        #region Declarations

        private static string s_tempPlayList;
        private static MusicPlayer.Player s_player;
        private static MusicLibrary.Library s_library;
        private static ThreePM.Utilities.HttpServer s_server;

        #endregion

        #region Properties

        public static Player Player
        {
            get
            {
                return s_player;
            }
            set
            {
                s_player = value;
            }
        }

        public static Library Library
        {
            get
            {
                return s_library;
            }
            set
            {
                s_library = value;
            }
        }

        public static bool HttpServerEnabled
        {
            get
            {
                return s_server != null;
            }
            set
            {
                if (value)
                {
                    if (s_server == null)
                    {
                        s_server = new ThreePM.Utilities.HttpServer(Player, Library);
                    }
                }
                else
                {
                    if (s_server != null)
                    {
                        s_server.Dispose();
                        s_server = null;
                    }
                }
            }
        }

        #endregion

        #region Methods


        /// <summary>
        /// Gets stuff ready so you can hook up event handlers
        /// </summary>
        public static void Initialize()
        {
            Initialize(-1);
        }

        public static void Initialize(int deviceNumber)
        {
            Player = new MusicPlayer.Player(deviceNumber);
            Player.SongInfoFormatString = Utilities.GetValue("Player.SongInfoFormatString", "{Artist} - {Title}");
            Player.SecondsBeforeUpdatePlayCount = Utilities.GetValue("Player.SecondsBeforeUpdatePlayCount", 20);
            Player.IgnorePreviouslyPlayedSongsInRandomMode = Utilities.GetValue("Player.IgnorePreviouslyPlayedSongsInRandomMode", false);
            Player.NeverPlayIgnoredSongs = Utilities.GetValue("Player.NeverPlayIgnoredSongs", false);
            Player.AudioscrobblerEnabled = Utilities.GetValue("Player.AudioscrobblerEnabled", false);
            Player.AudioscrobblerUserName = Utilities.GetValue("Player.AudioscrobblerUserName", "");
            Player.AudioscrobblerPassword = Utilities.GetValue("Player.AudioscrobblerPassword", "");


            Player.RepeatCurrentTrack = Convert.ToBoolean(Utilities.GetValue("Player.RepeatCurrentTrack", false));
            Player.Playlist.PlaylistStyle = (PlaylistStyle)Utilities.GetValue("Player.PlaylistStyle", 0);

            Library = new MusicLibrary.Library();
        }

        /// <summary>
        /// Does the stuff
        /// </summary>
        public static void Start()
        {
            s_tempPlayList = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\ThreePM.m3u";


            Player.Library = Library;


            // load the last song that was being played
            string file = Utilities.GetValue("Player.CurrentSong", "");
            if (!string.IsNullOrEmpty(file))
            {
                // don't count the play since we're just restarting the same song
                float position = Utilities.GetValue("Player.Position", 0f);
                if (Player.LoadFile(file, Convert.ToInt32(position) <= Player.SecondsBeforeUpdatePlayCount))
                {
                    Player.Position = position;
                    Player.Play();
                }
            }

            Player.Playlist.LoadFromFile(s_tempPlayList);

            HttpServerEnabled = Convert.ToBoolean(Utilities.GetValue("MainForm.HttpServer", false));
        }

        /// <summary>
        /// Finishes the stuff
        /// </summary>
        public static void End()
        {
            Player.Pause();
            File.Delete(s_tempPlayList);
            Player.Playlist.SaveToFile(s_tempPlayList);

            if (Player.CurrentSong != null)
            {
                Utilities.SetValue("Player.CurrentSong", Player.CurrentSong.FileName);
                Utilities.SetValue("Player.Position", Player.Position);
            }

            if (s_server != null)
            {
                s_server.Dispose();
                s_server = null;
            }
            Player.Dispose();
            Library.Dispose();
            Player = null;
            Library = null;
        }

        #endregion
    }
}
