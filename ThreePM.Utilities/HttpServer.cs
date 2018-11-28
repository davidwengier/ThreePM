using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ThreePM.MusicLibrary;
using ThreePM.MusicPlayer;

namespace ThreePM.Utilities
{
    public class HttpServer : IDisposable
    {
        private SongInfo _lastSong;
        private string _lastAlbumArt;
        private string _lastLyrics;
        private readonly LyricsHelper _helper;

        private readonly Player _player;
        private readonly Library _library;
        private Socket _listener;
        private readonly string _serverTemplate;

        #region Constructor

        public HttpServer(MusicPlayer.Player player, MusicLibrary.Library library)
        {
            _serverTemplate = Properties.Resources.HttpServerTemplate;

            _player = player;
            _library = library;

            _helper = new LyricsHelper(library);
            _helper.LyricsNotFound += delegate { _lastLyrics = "Not found."; };
            _helper.LyricsFound += new EventHandler<LyricsFoundEventArgs>(Helper_LyricsFound);

            // Create a new server socket, set up all the endpoints, bind the socket and then listen
            _listener = new Socket(0, SocketType.Stream, ProtocolType.Tcp);
            var endpoint = new IPEndPoint(IPAddress.Any, 8001);
            _listener.Bind(endpoint);
            _listener.Listen(-1);
            // start listening
            _listener.BeginAccept(AcceptFinished, null);
        }

        private void Helper_LyricsFound(object sender, LyricsFoundEventArgs e)
        {
            _library.SetLyrics(_helper.Song.Title, _helper.Song.Artist, e.Lyrics);
            _lastLyrics = e.Lyrics;
        }

        #endregion

        #region Private Methods

        private void AcceptFinished(IAsyncResult result)
        {
            if (_listener == null) return;
            try
            {
                Socket s = _listener.EndAccept(result);
                var processor = new HttpProcessor(s, this);
                var DoWork = new ThreadStart(delegate
                {
                    processor.Process();
                });
                DoWork.BeginInvoke(null, null);
            }
            catch { }
            finally
            {
                // start listening for the next one
                if (_listener != null)
                {
                    try
                    {
                        _listener.BeginAccept(AcceptFinished, null);
                    }
                    catch { }
                }
            }
        }

        private void ProcessRequest(HttpRequest request)
        {
            string[] args = request.Url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "lyrics":
                    {
                        request.Response = @"<html>
							<head>
							<style>
							body
							{ color:#FFFFEC; 
									 vertical-align:top;
									 margin:0px;
									 padding:0px;
									 border:0px;
									 font-family:Verdana; 
									 font-size:8pt; }
							</style>
							</head>
							<body bgcolor=""#706F77"" text=""#FFFFEC"">" +
                                    (_lastLyrics == "Loading..." ? "<meta http-equiv=\"refresh\" content=\"5\">" : "") +
                                    _lastLyrics.Replace(Environment.NewLine, "<br />") + "</body></html>";
                        break;
                    }
                    case "albumart":
                    {
                        if (_lastSong != _player.CurrentSong)
                        {
                            _lastSong = _player.CurrentSong;

                            _lastLyrics = "Loading...";

                            if (_library.GetSong(_lastSong.FileName) is LibraryEntry entry && !string.IsNullOrEmpty(entry.Lyrics))
                            {
                                _lastLyrics = entry.Lyrics;
                            }
                            else
                            {
                                _helper.LoadLyrics(_lastSong);
                            }

                            using (System.Drawing.Bitmap albumArt = AlbumArtHelper.GetAlbumArt(_lastSong.FileName, 200, 200))
                            {

                                using (var s = new MemoryStream())
                                {
                                    albumArt.Save(s, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    byte[] b = s.ToArray();
                                    _lastAlbumArt = System.Text.UnicodeEncoding.GetEncoding(0).GetString(b);
                                }
                            }
                        }

                        request.Headers.Add("Content-type", "image/jpeg");
                        request.Response = _lastAlbumArt;

                        break;
                    }
                    case "next":
                    {
                        _player.Next();
                        break;
                    }
                    case "pause":
                    {
                        _player.Pause();
                        break;
                    }
                    case "play":
                    {
                        if (args.Length > 1)
                        {
                            string query = args[1];
                            query = System.Web.HttpUtility.UrlDecode(query);
                            int i = Convert.ToInt32(args[2]);
                            if (args.Length == 3)
                            {
                                _player.PlayFile(_library.GetLibrary(query, 50, true)[i].FileName);
                            }
                            else
                            {
                                _player.PlayFile(_library.GetLibrary(query, 50, true, "Lyrics")[i].FileName);
                            }
                        }
                        else
                        {
                            _player.Play();
                        }
                        break;
                    }
                    case "stop":
                    {
                        _player.Stop();
                        break;
                    }
                    case "previous":
                    {
                        _player.Previous();
                        break;
                    }
                    case "queue":
                    {
                        string query = args[1];
                        query = System.Web.HttpUtility.UrlDecode(query);
                        int i = Convert.ToInt32(args[2]);
                        if (args.Length == 3)
                        {
                            _player.Playlist.AddToEnd(_library.GetLibrary(query, 50, true)[i]);
                        }
                        else
                        {
                            _player.Playlist.AddToEnd(_library.GetLibrary(query, 50, true, "Lyrics")[i]);
                        }
                        break;
                    }
                    case "lyricssearch":
                    {
                        string searchresults = "No results found.";
                        if (args.Length > 1)
                        {
                            string query = args[1];
                            System.Diagnostics.Debug.WriteLine("Lyrics Search: " + query);
                            LibraryEntry[] results = _library.GetLibrary(query, 50, true, "Lyrics");

                            if (results.Length > 0)
                            {
                                searchresults = "<table width=\"100%\">";
                                int i = 0;
                                foreach (LibraryEntry result in results)
                                {
                                    searchresults += "<tr" + (i % 2 == 0 ? " class=\"alternate\"" : "") + "><td width=\"100%\">" + result.ToString() + "</td><td nowrap>" +
                                        "<a href=\"javascript:doSearchResultsAction('play/" + System.Web.HttpUtility.UrlEncode(query) + "/" + i.ToString() + "/lyrics');\">Play</a> " +
                                        "<a href=\"javascript:doSearchResultsAction('queue/" + System.Web.HttpUtility.UrlEncode(query) + "/" + i.ToString() + "/lyrics');\">Queue</a> " +
                                        "</td></tr>";
                                    i++;
                                }
                                searchresults += "</table>";
                            }
                        }
                        request.Response = searchresults;
                        break;
                    }
                    case "search":
                    {
                        string searchresults = "No results found.";
                        if (args.Length > 1)
                        {
                            string query = args[1];
                            System.Diagnostics.Debug.WriteLine("Search: " + query);
                            LibraryEntry[] results = _library.GetLibrary(query, 50, true);

                            if (results.Length > 0)
                            {
                                searchresults = "<table width=\"100%\">";
                                int i = 0;
                                foreach (LibraryEntry result in results)
                                {
                                    searchresults += "<tr" + (i % 2 == 0 ? " class=\"alternate\"" : "") + "><td width=\"100%\">" + result.ToString() + "</td><td nowrap>" +
                                        "<a href=\"javascript:doSearchResultsAction('play/" + System.Web.HttpUtility.UrlEncode(query) + "/" + i.ToString() + "');\">Play</a> " +
                                        "<a href=\"javascript:doSearchResultsAction('queue/" + System.Web.HttpUtility.UrlEncode(query) + "/" + i.ToString() + "');\">Queue</a> " +
                                        "</td></tr>";
                                    i++;
                                }
                                searchresults += "</table>";
                            }
                        }
                        request.Response = searchresults;
                        break;
                    }
                    case "viewqueue":
                    {
                        string result = "<table width=\"100%\">";
                        int i = 0;
                        foreach (SongInfo file in _player.Playlist)
                        {
                            result += "<tr><td width=\"100%\">" + file.ToString() + "</td><td>" +
                                "<a href=\"javascript:doAction('delqueue/" + i.ToString() + "');\">Delete</a> " +
                                "</td></tr>";
                            i++;
                        }
                        result += "</table>";
                        request.Response = result;
                        break;
                    }
                    case "delqueue":
                    {
                        _player.Playlist.Remove(Convert.ToInt32(args[1]));
                        break;
                    }
                    case "position":
                    {
                        request.Response = Convert.ToInt32(_player.Position * 1000).ToString();
                        break;
                    }
                    case "duration":
                    {
                        request.Response = Convert.ToInt32(_player.CurrentSong.Duration * 1000).ToString();
                        break;
                    }
                    case "songtitle":
                    {
                        request.Response = _player.CurrentSong.Artist + " - " + _player.CurrentSong.Title;
                        break;
                    }
                    case "details":
                    {
                        string response = @"<table>
											<tr><td nowrap class=""label"">Filename</td><td>{Filename}</td></tr>
											<tr><td class=""label"">Title</td><td>{Title}</td></tr>
											<tr><td class=""label"">Artist</td><td>{Artist}</td></tr>
											<tr><td class=""label"">Duration</td><td>{Duration}</td></tr>
											<tr><td class=""label"">Album</td><td>{Album}</td></tr>
											<tr><td class=""label"">Year</td><td>{Year}</td></tr>
											<tr><td class=""label"">Track</td><td>{Track}</td></tr>
											<tr><td class=""label"">Play Count</td><td>{PlayCount}</td></tr>
										  </table>";

                        // if we get here, just write out the normal stuff
                        response = response.Replace("{Title}", _player.CurrentSong.Title);
                        response = response.Replace("{Artist}", _player.CurrentSong.Artist);
                        response = response.Replace("{Album}", _player.CurrentSong.Album);
                        response = response.Replace("{Track}", _player.CurrentSong.TrackNumber.ToString());
                        response = response.Replace("{Year}", _player.CurrentSong.Year.ToString());
                        response = response.Replace("{Filename}", _player.CurrentSong.FileName);
                        response = response.Replace("{Duration}", _player.CurrentSong.DurationDescription);
                        response = response.Replace("{PlayCount}", _library.GetPlayCount(_player.CurrentSong.FileName).ToString());

                        request.Response = response;
                        break;
                    }
                }
            }
            else
            {
                // if we get here, just write out the normal stuff
                string response = _serverTemplate;
                response = response.Replace("{Title}", _player.CurrentSong.Title);
                response = response.Replace("{Artist}", _player.CurrentSong.Artist);
                response = response.Replace("{Album}", _player.CurrentSong.Album);
                response = response.Replace("{Track}", _player.CurrentSong.TrackNumber.ToString());
                response = response.Replace("{Year}", _player.CurrentSong.Year.ToString());
                response = response.Replace("{Filename}", _player.CurrentSong.FileName);
                response = response.Replace("{Duration}", _player.CurrentSong.DurationDescription);
                response = response.Replace("{PlayCount}", _library.GetPlayCount(_player.CurrentSong.FileName).ToString());

                request.Response = response;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _listener.Close();
            _listener = null;
        }

        #endregion

        #region HttpRequest Class

        private class HttpRequest
        {
            private string _response;
            private int _responseCode;
            private string _url;
            private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

            public Dictionary<string, string> Headers
            {
                get { return _headers; }
            }

            public string Response
            {
                get { return _response; }
                set { _response = value; }
            }

            public string Url
            {
                get { return _url; }
                set { _url = value; }
            }

            public int ResponseCode
            {
                get { return _responseCode; }
                set { _responseCode = value; }
            }
        }

        #endregion

        #region HttpProcessor Class

        private class HttpProcessor
        {
            #region Declarations

            private HttpServer _server;

            private Socket _socket;
            private NetworkStream _ns;
            private StreamReader _sr;
            private StreamWriter _sw;
            private string _method;
            private string _url;
            private string _protocol;
            private readonly Hashtable _headers;
            private byte[] _bytes = new byte[4096];

            #endregion

            #region Constructor

            public HttpProcessor(Socket s, HttpServer server)
            {
                this._socket = s;
                _server = server;
                _headers = new Hashtable();
            }

            #endregion

            #region Public Methods

            public void Process()
            {
                // Bundle up our sockets nice and tight in various streams
                _ns = new NetworkStream(_socket, FileAccess.ReadWrite);
                // It looks like these streams buffer
                _sr = new StreamReader(_ns);
                _sw = new StreamWriter(_ns);
                // Parse the request, if that succeeds, read the headers, if that
                // succeeds, then write the given URL to the stream, if possible.
                while (ParseRequest())
                {
                    if (ReadHeaders())
                    {
                        // Copy the file to the socket
                        WriteURL();
                        // If keep alive is not active then we want to close down the streams
                        // and shutdown the socket
                        _ns.Close();
                        _socket.Shutdown(SocketShutdown.Both);
                        break;
                    }
                }
            }

            #endregion

            #region Private Methods

            private bool ParseRequest()
            {
                // The number of requests handled by this persistent connection
                string request = null;
                try
                {
                    request = null;
                    request = _sr.ReadLine();
                }
                catch (IOException)
                {
                }
                // If the request line is null, then the other end has hung up on us.  A well
                // behaved client will do this after 15-60 seconds of inactivity.
                if (request == null)
                {
                    return false;
                }
                // HTTP request lines are of the form:
                // [METHOD] [Encoded URL] HTTP/1.?
                string[] tokens = request.Split(' ');
                if (tokens.Length != 3)
                {
                    WriteError(400, "Bad request");
                    return false;
                }
                // We currently only handle GET requests
                _method = tokens[0];
                if (!_method.Equals("GET"))
                {
                    WriteError(501, _method + " not implemented");
                    return false;
                }
                _url = tokens[1];
                // Only accept valid urls
                if (!_url.StartsWith("/"))
                {
                    WriteError(400, "Bad URL");
                    return false;
                }

                _url = System.Web.HttpUtility.UrlDecode(_url);
                //// Decode all encoded parts of the URL using the built in URI processing class
                //int i = 0;
                //while ((i = url.IndexOf("%", i)) != -1)
                //{
                //    url = url.Substring(0, i) + Uri.HexUnescape(url, ref i) + url.Substring(i);
                //}
                // Lets just make sure we are using HTTP, thats about all I care about
                _protocol = tokens[2];
                if (!_protocol.StartsWith("HTTP/"))
                {
                    WriteError(400, "Bad protocol: " + _protocol);
                }
                return true;
            }

            private bool ReadHeaders()
            {
                string line;
                string name = null;
                // The headers end with either a socket close (!) or an empty line
                while ((line = _sr.ReadLine()) != null && line != "")
                {
                    // If the value begins with a space or a hard tab then this
                    // is an extension of the value of the previous header and
                    // should be appended
                    if (name != null && char.IsWhiteSpace(line[0]))
                    {
                        _headers[name] += line;
                        continue;
                    }
                    // Headers consist of [NAME]: [VALUE] + possible extension lines
                    int firstColon = line.IndexOf(":");
                    if (firstColon != -1)
                    {
                        name = line.Substring(0, firstColon);
                        string value = line.Substring(firstColon + 1).Trim();
                        _headers[name] = value;
                    }
                    else
                    {
                        WriteError(400, "Bad header: " + line);
                        return false;
                    }
                }
                return line != null;
            }

            private void WriteURL()
            {
                try
                {
                    Stream streamToReadFrom = null;
                    long left = 0;

                    var req = new HttpRequest();
                    req.Url = _url;
                    req.ResponseCode = 200;
                    System.Diagnostics.Debug.WriteLine("URL Requested: " + _url);
                    _server.ProcessRequest(req);

                    if (req.Response == null)
                    {
                        WriteSuccess(0, req.Headers);
                    }
                    else
                    {
                        streamToReadFrom = new MemoryStream(System.Text.UnicodeEncoding.GetEncoding(0).GetBytes(req.Response));
                        // Write the content length and the success header to the stream
                        left = req.Response.Length;
                        WriteSuccess(left, req.Headers);

                        // Copy the contents of the file to the stream, ensure that we never write
                        // more than the content length we specified.  Just in case the file somehow
                        // changes out from under us, although I don't know if that is possible.
                        var bs = new BufferedStream(streamToReadFrom);
                        int read;
                        while (left > 0 && (read = bs.Read(_bytes, 0, (int)Math.Min(left, _bytes.Length))) != 0)
                        {
                            _ns.Write(_bytes, 0, read);
                            left -= read;
                        }
                        _ns.Flush();
                        bs.Close();
                    }
                }
                catch (Exception)
                {
                    WriteForbidden();
                }
            }

            private void WriteSuccess(long length, Dictionary<string, string> headers)
            {
                WriteResult(200, "OK", length, headers);
            }

            private void WriteForbidden()
            {
                WriteError(403, "Forbidden");
            }

            private void WriteError(int status, string message)
            {
                string output = "<h1>HTTP/1.0 " + status + " " + message + "</h1>";
                WriteResult(status, message, output.Length, null);
                _sw.Write(output);
                try
                {
                    _sw.Flush();
                }
                catch
                { }
            }

            private void WriteResult(int status, string message, long length, Dictionary<string, string> headers)
            {
                _sw.Write("HTTP/1.0 " + status + " " + message + "\r\n");
                _sw.Write("Content-Length: " + length + "\r\n");
                _sw.Write("Connection: close\r\n");
                if (headers != null)
                {
                    foreach (string s in headers.Keys)
                    {
                        _sw.Write(s + ": " + headers[s] + "\r\n");
                    }
                }
                _sw.Write("\r\n");
                try
                {
                    _sw.Flush();
                }
                catch
                { }
            }

            #endregion
        }

        #endregion
    }
}
