using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ThreePM.MusicPlayer;
using ThreePM.MusicLibrary;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ThreePM.Utilities
{
	public class HttpServer : IDisposable
	{
		private SongInfo lastSong;
		private string lastAlbumArt;
		private string lastLyrics;
		private LyricsHelper helper;

		#region Declarations

		private MusicPlayer.Player m_player;
		private MusicLibrary.Library m_library;
		private Socket m_listener;
		private string m_serverTemplate;

		#endregion

		#region Constructor

		public HttpServer(MusicPlayer.Player player, MusicLibrary.Library library)
		{
			m_serverTemplate = Properties.Resources.HttpServerTemplate;

			m_player = player;
			m_library = library;

			helper = new LyricsHelper(library);
			helper.LyricsNotFound += delegate { lastLyrics = "Not found."; };
			helper.LyricsFound += new EventHandler<LyricsFoundEventArgs>(helper_LyricsFound);

			// Create a new server socket, set up all the endpoints, bind the socket and then listen
			m_listener = new Socket(0, SocketType.Stream, ProtocolType.Tcp);
			IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 8001);
			m_listener.Bind(endpoint);
			m_listener.Listen(-1);
			// start listening
			m_listener.BeginAccept(AcceptFinished, null);
		}

		void helper_LyricsFound(object sender, LyricsFoundEventArgs e)
		{
			m_library.SetLyrics(helper.Song.Title, helper.Song.Artist, e.Lyrics);
			lastLyrics = e.Lyrics;
		}

		#endregion

		#region Private Methods

		private void AcceptFinished(IAsyncResult result)
		{
			if (m_listener == null) return;
			try
			{
				Socket s = m_listener.EndAccept(result);
				HttpProcessor processor = new HttpProcessor(s, this);
				ThreadStart DoWork = new ThreadStart(delegate
				{
					processor.Process();
				});
				DoWork.BeginInvoke(null, null);
			}
			catch { }
			finally
			{
				// start listening for the next one
				if (m_listener != null)
				{
					try
					{
						m_listener.BeginAccept(AcceptFinished, null);
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
									(lastLyrics == "Loading..." ? "<meta http-equiv=\"refresh\" content=\"5\">" : "") +
									lastLyrics.Replace(Environment.NewLine, "<br />") + "</body></html>";
						break;
					}
					case "albumart":
					{
						if (lastSong != m_player.CurrentSong)
						{
							lastSong = m_player.CurrentSong;

							lastLyrics = "Loading...";

							LibraryEntry entry = m_library.GetSong(lastSong.FileName) as LibraryEntry;
							if (entry != null && !String.IsNullOrEmpty(entry.Lyrics))
							{
								lastLyrics = entry.Lyrics;
							}
							else
							{
								helper.LoadLyrics(lastSong);
							}

							using (System.Drawing.Bitmap albumArt = AlbumArtHelper.GetAlbumArt(lastSong.FileName, 200, 200))
							{

								using (MemoryStream s = new MemoryStream())
								{
									albumArt.Save(s, System.Drawing.Imaging.ImageFormat.Jpeg);
									byte[] b = s.ToArray();
									lastAlbumArt = System.Text.UnicodeEncoding.GetEncoding(0).GetString(b);
								}
							}
						}

						request.Headers.Add("Content-type", "image/jpeg");
						request.Response = lastAlbumArt;

						break;
					}
					case "next":
					{
						m_player.Next();
						break;
					}
					case "pause":
					{
						m_player.Pause();
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
								m_player.PlayFile(m_library.GetLibrary(query, 50, true)[i].FileName);
							}
							else
							{
								m_player.PlayFile(m_library.GetLibrary(query, 50, true, "Lyrics")[i].FileName);
							}
						}
						else
						{
							m_player.Play();
						}
						break;
					}
					case "stop":
					{
						m_player.Stop();
						break;
					}
					case "previous":
					{
						m_player.Previous();
						break;
					}
					case "queue":
					{
						string query = args[1];
						query = System.Web.HttpUtility.UrlDecode(query);
						int i = Convert.ToInt32(args[2]);
						if (args.Length == 3)
						{
							m_player.Playlist.AddToEnd(m_library.GetLibrary(query, 50, true)[i]);
						}
						else
						{
							m_player.Playlist.AddToEnd(m_library.GetLibrary(query, 50, true, "Lyrics")[i]);
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
							LibraryEntry[] results = m_library.GetLibrary(query, 50, true, "Lyrics");

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
							LibraryEntry[] results = m_library.GetLibrary(query, 50, true);

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
						foreach (SongInfo file in m_player.Playlist)
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
						m_player.Playlist.Remove(Convert.ToInt32(args[1]));
						break;
					}
					case "position":
					{
						request.Response = Convert.ToInt32(m_player.Position * 1000).ToString();
						break;
					}
					case "duration":
					{
						request.Response = Convert.ToInt32(m_player.CurrentSong.Duration * 1000).ToString();
						break;
					}
					case "songtitle":
					{
						request.Response = m_player.CurrentSong.Artist + " - " + m_player.CurrentSong.Title;
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
						response = response.Replace("{Title}", m_player.CurrentSong.Title);
						response = response.Replace("{Artist}", m_player.CurrentSong.Artist);
						response = response.Replace("{Album}", m_player.CurrentSong.Album);
						response = response.Replace("{Track}", m_player.CurrentSong.TrackNumber.ToString());
						response = response.Replace("{Year}", m_player.CurrentSong.Year.ToString());
						response = response.Replace("{Filename}", m_player.CurrentSong.FileName);
						response = response.Replace("{Duration}", m_player.CurrentSong.DurationDescription);
						response = response.Replace("{PlayCount}", m_library.GetPlayCount(m_player.CurrentSong.FileName).ToString());

						request.Response = response;
						break;
					}
				}
			}
			else
			{
				// if we get here, just write out the normal stuff
				string response = m_serverTemplate;
				response = response.Replace("{Title}", m_player.CurrentSong.Title);
				response = response.Replace("{Artist}", m_player.CurrentSong.Artist);
				response = response.Replace("{Album}", m_player.CurrentSong.Album);
				response = response.Replace("{Track}", m_player.CurrentSong.TrackNumber.ToString());
				response = response.Replace("{Year}", m_player.CurrentSong.Year.ToString());
				response = response.Replace("{Filename}", m_player.CurrentSong.FileName);
				response = response.Replace("{Duration}", m_player.CurrentSong.DurationDescription);
				response = response.Replace("{PlayCount}", m_library.GetPlayCount(m_player.CurrentSong.FileName).ToString());

				request.Response = response;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			m_listener.Close();
			m_listener = null;
		}

		#endregion

		#region HttpRequest Class

		private class HttpRequest
		{
			private string m_response;
			private int m_responseCode;
			private string m_url;
			private Dictionary<string, string> m_headers = new Dictionary<string, string>();

			public Dictionary<string, string> Headers
			{
				get { return m_headers; }
			}

			public string Response
			{
				get { return m_response; }
				set { m_response = value; }
			}

			public string Url
			{
				get { return m_url; }
				set { m_url = value; }
			}

			public int ResponseCode
			{
				get { return m_responseCode; }
				set { m_responseCode = value; }
			}
		}

		#endregion

		#region HttpProcessor Class

		private class HttpProcessor
		{
			#region Declarations

			private HttpServer m_server;

			private Socket m_socket;
			private NetworkStream ns;
			private StreamReader sr;
			private StreamWriter sw;
			private string method;
			private string url;
			private string protocol;
			private Hashtable headers;
			private byte[] bytes = new byte[4096];

			#endregion

			#region Constructor

			public HttpProcessor(Socket s, HttpServer server)
			{
				this.m_socket = s;
				m_server = server;
				headers = new Hashtable();
			}

			#endregion

			#region Public Methods

			public void Process()
			{
				// Bundle up our sockets nice and tight in various streams
				ns = new NetworkStream(m_socket, FileAccess.ReadWrite);
				// It looks like these streams buffer
				sr = new StreamReader(ns);
				sw = new StreamWriter(ns);
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
						ns.Close();
						m_socket.Shutdown(SocketShutdown.Both);
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
					request = sr.ReadLine();
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
				method = tokens[0];
				if (!method.Equals("GET"))
				{
					WriteError(501, method + " not implemented");
					return false;
				}
				url = tokens[1];
				// Only accept valid urls
				if (!url.StartsWith("/"))
				{
					WriteError(400, "Bad URL");
					return false;
				}

				url = System.Web.HttpUtility.UrlDecode(url);
				//// Decode all encoded parts of the URL using the built in URI processing class
				//int i = 0;
				//while ((i = url.IndexOf("%", i)) != -1)
				//{
				//    url = url.Substring(0, i) + Uri.HexUnescape(url, ref i) + url.Substring(i);
				//}
				// Lets just make sure we are using HTTP, thats about all I care about
				protocol = tokens[2];
				if (!protocol.StartsWith("HTTP/"))
				{
					WriteError(400, "Bad protocol: " + protocol);
				}
				return true;
			}

			private bool ReadHeaders()
			{
				string line;
				string name = null;
				// The headers end with either a socket close (!) or an empty line
				while ((line = sr.ReadLine()) != null && line != "")
				{
					// If the value begins with a space or a hard tab then this
					// is an extension of the value of the previous header and
					// should be appended
					if (name != null && Char.IsWhiteSpace(line[0]))
					{
						headers[name] += line;
						continue;
					}
					// Headers consist of [NAME]: [VALUE] + possible extension lines
					int firstColon = line.IndexOf(":");
					if (firstColon != -1)
					{
						name = line.Substring(0, firstColon);
						String value = line.Substring(firstColon + 1).Trim();
						headers[name] = value;
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

					HttpRequest req = new HttpRequest();
					req.Url = url;
					req.ResponseCode = 200;
					System.Diagnostics.Debug.WriteLine("URL Requested: " + url);
					m_server.ProcessRequest(req);

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
						BufferedStream bs = new BufferedStream(streamToReadFrom);
						int read;
						while (left > 0 && (read = bs.Read(bytes, 0, (int)Math.Min(left, bytes.Length))) != 0)
						{
							ns.Write(bytes, 0, read);
							left -= read;
						}
						ns.Flush();
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
				WriteResult(status, message, (long)output.Length, null);
				sw.Write(output);
				try
				{
					sw.Flush();
				}
				catch
				{ }
			}

			private void WriteResult(int status, string message, long length, Dictionary<string, string> headers)
			{
				sw.Write("HTTP/1.0 " + status + " " + message + "\r\n");
				sw.Write("Content-Length: " + length + "\r\n");
				sw.Write("Connection: close\r\n");
				if (headers != null)
				{
					foreach (string s in headers.Keys)
					{
						sw.Write(s + ": " + headers[s] + "\r\n");
					}
				}
				sw.Write("\r\n");
				try
				{
					sw.Flush();
				}
				catch
				{ }
			}

			#endregion
		}

		#endregion
	}
}