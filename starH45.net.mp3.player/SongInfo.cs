using System;
using System.Collections.Generic;
using System.Text;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass;

namespace starH45.net.mp3.player
{
	/// <summary>
	/// Stores information about a song.
	/// </summary>
	public class SongInfo
	{
		#region Declarations

		protected string m_fileName;
		protected string m_artist;
		protected string m_albumArtist;
		protected string m_album;
		protected string m_title;
		protected int m_trackNumber;
		protected int m_year;
		protected double m_duration;
		protected string m_genre;
		protected bool m_hasTag = true;
		protected string m_toStringString;
		protected bool m_ignored;
		protected bool m_hasFrontCover;
		private System.Drawing.Image m_frontCover;

		#endregion

		#region Properties

		public bool HasFrontCover
		{
			get { return m_hasFrontCover; }
		}

		public string FileName
		{
			get { return m_fileName; }
		}

		public string Artist
		{
			get { return m_artist; }
		}

		public string AlbumArtist
		{
			get { return (String.IsNullOrEmpty(m_albumArtist) ? m_artist : m_albumArtist); }
		}

		public string Album
		{
			get { return m_album; }
		}

		public string Title
		{
			get { return m_title; }
		}

		public string Genre
		{
			get { return m_genre; }
		}

		public int TrackNumber
		{
			get { return m_trackNumber; }
		}

		public int Year
		{
			get { return m_year; }
		}

		public double Duration
		{
			get { return m_duration; }
		}

		public string DurationDescription
		{
			get { return Player.GetPositionDescription(m_duration); }
		}

		public bool Ignored
		{
			get { return m_ignored; }
		}

		#endregion

		#region Constructor

		protected SongInfo()
		{
		}

		public System.Drawing.Bitmap GetFrontCover(int width, int height)
		{
			if (m_frontCover == null) return null;
			return new System.Drawing.Bitmap(m_frontCover, new System.Drawing.Size(width, height));
		}

		public SongInfo(string fileName)
		{
			m_fileName = fileName;

			try
			{

				TAG_INFO tag = new TAG_INFO(m_fileName);
				int num1 = Un4seen.Bass.Bass.BASS_StreamCreateFile(m_fileName, 0, 0, BASSFlag.BASS_STREAM_DECODE);
				if (num1 != 0)
				{
					m_duration = Bass.BASS_ChannelBytes2Seconds(num1, Bass.BASS_ChannelGetLength(num1));
					BassTags.BASS_TAG_GetFromFile(num1, tag);
					Un4seen.Bass.Bass.BASS_StreamFree(num1);
				}

				int year = 0;
				int track = 0;
				
				if (tag.PictureCount > 0)
				{
					for (int i = 0; i < tag.PictureCount; i++)
					{
						if (tag.PictureGetType(i) == "FrontAlbumCover")
						{
							m_hasFrontCover = true;
							m_frontCover = tag.PictureGetImage(i);
						}
					}
				}


				if (tag.track.IndexOf('/') != -1)
				{
					int.TryParse(tag.track.Substring(0, tag.track.IndexOf('/')), out track);
				}
				else
				{
					int.TryParse(tag.track, out track);
				}
				int.TryParse(tag.year, out year);

				m_albumArtist = tag.albumartist;
				if (string.IsNullOrEmpty(m_albumArtist))
				{
					if (tag.NativeTag("TPE2") != null)
					{
						m_albumArtist = FixString(tag.NativeTag("TPE2"));
					}
					else if (tag.NativeTag("WM/AlbumArtist") != null)
					{
						m_albumArtist = FixString(tag.NativeTag("WM/AlbumArtist"));
					}
					else if (tag.NativeTag("ALBUM ARTIST") != null)
					{
						m_albumArtist = FixString(tag.NativeTag("ALBUM ARTIST"));
					}
					// Tag & Rename writes flac files with ENSEMBLE tags not ALBUM ARTIST tags, so fallback
					else if (tag.NativeTag("ENSEMBLE") != null)
					{
						m_albumArtist = FixString(tag.NativeTag("ENSEMBLE"));
					}
				}

				m_album = FixString(tag.album);
				if (string.IsNullOrEmpty(m_album))
				{
					// flac files have ALBUM1 sometimes
					if (tag.NativeTag("ALBUM1") != null)
					{
						m_album = FixString(tag.NativeTag("ALBUM1"));
					}
				}

				m_artist = FixString(tag.artist);
				m_title = FixString(tag.title);
				m_trackNumber = track;
				m_year = year;
				m_genre = FixString(tag.genre);
			}
			catch (System.Threading.ThreadAbortException)
			{
			}
			catch 
			{
				//System.Diagnostics.Debugger.Break();
			}

			ComputeToStringString();
		}

		public SongInfo(string fileName, string artist, string album, string title, int track, int year, float duration, string genre, string albumArtist,bool ignored)
		{
			m_fileName = FixString(fileName);
			m_artist = FixString(artist);
			m_album = FixString(album);
			m_title = FixString(title);
			m_trackNumber = track;
			m_year = year;
			m_duration = duration;
			m_genre = FixString(genre);
			m_albumArtist = FixString(albumArtist);
			m_ignored = ignored;

			ComputeToStringString(); 
		}

		protected static string FixString(object input)
		{
			if (input == null)
			{
				return null;
			}
			else
			{
				return input.ToString().Trim().Trim('\0');
			}
		}

		protected void ComputeToStringString()
		{
			m_hasTag = !String.IsNullOrEmpty(m_title);

			if (m_hasTag)
			{
				m_toStringString = Player.SongInfoFormatString;
				m_toStringString = m_toStringString.Replace("{Artist}", Artist);
				m_toStringString = m_toStringString.Replace("{Title}", Title);
				m_toStringString = m_toStringString.Replace("{Album}", Album);
				m_toStringString = m_toStringString.Replace("{AlbumArtist}", AlbumArtist);
				m_toStringString = m_toStringString.Replace("{Year}", Year.ToString());
				m_toStringString = m_toStringString.Replace("{Genre}", Genre);
				m_toStringString = m_toStringString.Replace("{TrackNumber}", TrackNumber.ToString());
				m_toStringString = m_toStringString.Replace("{Duration}", DurationDescription);
				m_toStringString = m_toStringString.Replace("{Ignored}", (m_ignored ? "Ignored" : ""));
			}
			else
			{
				m_toStringString = FileName;
			}
		}

		#endregion

		#region Overridden Methods

		public override string ToString()
		{
			return m_toStringString;
		}

		#endregion Overridden Methods
	}
}
