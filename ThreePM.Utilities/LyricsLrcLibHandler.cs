using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace ThreePM.Utilities
{
    internal class LyricsLrcLibHandler : ILyricsSiteHandler
    {
        private string _lyrics;

        public string SiteName
        {
            get { return "LRCLIB"; }
        }

        public string GetSearchURL(MusicPlayer.SongInfo song)
        {
            _lyrics = null;

            if (song == null || string.IsNullOrWhiteSpace(song.Title) || string.IsNullOrWhiteSpace(song.Artist))
            {
                return null;
            }

            return string.Format(
                "https://lrclib.net/api/search?artist_name={0}&track_name={1}",
                Uri.EscapeDataString(song.Artist),
                Uri.EscapeDataString(song.Title));
        }

        public bool GetLyrics(string htmlPage, out string lyrics)
        {
            lyrics = _lyrics;
            return !string.IsNullOrWhiteSpace(lyrics);
        }

        public LyricsSearchResults ProcessSearchResults(MusicPlayer.SongInfo song, string htmlPage, out string nextURL)
        {
            nextURL = "";

            if (song == null)
            {
                return LyricsSearchResults.NotFound;
            }

            LrcLibSearchResult[] results = DeserializeSearchResults(htmlPage);
            if (results == null || results.Length == 0)
            {
                return LyricsSearchResults.NotFound;
            }

            LrcLibSearchResult bestMatch = results
                .Select(result => new { Result = result, Score = ScoreResult(song, result) })
                .Where(result => result.Score > int.MinValue)
                .OrderByDescending(result => result.Score)
                .ThenBy(result => string.IsNullOrWhiteSpace(result.Result.PlainLyrics) ? 1 : 0)
                .FirstOrDefault()?.Result;

            if (bestMatch == null)
            {
                return LyricsSearchResults.NotFound;
            }

            _lyrics = PrepareLyrics(bestMatch);
            return string.IsNullOrWhiteSpace(_lyrics) ? LyricsSearchResults.NotFound : LyricsSearchResults.FoundOnThisPage;
        }

        private static LrcLibSearchResult[] DeserializeSearchResults(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Array.Empty<LrcLibSearchResult>();
            }

            try
            {
                var serializer = new DataContractJsonSerializer(typeof(LrcLibSearchResult[]));
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    return (LrcLibSearchResult[])serializer.ReadObject(stream);
                }
            }
            catch
            {
                return Array.Empty<LrcLibSearchResult>();
            }
        }

        private static int ScoreResult(MusicPlayer.SongInfo song, LrcLibSearchResult result)
        {
            if (result == null || result.Instrumental)
            {
                return int.MinValue;
            }

            string trackName = FirstNonEmpty(result.TrackName, result.Name);
            if (!IsPotentialMatch(song.Title, trackName))
            {
                return int.MinValue;
            }

            if (!IsArtistMatch(song, result.ArtistName))
            {
                return int.MinValue;
            }

            int score = 0;

            score += ScoreExactness(song.Title, trackName) * 100;
            score += ScoreExactness(song.Artist, result.ArtistName) * 60;
            score += ScoreExactness(song.AlbumArtist, result.ArtistName) * 25;
            score += ScoreExactness(song.Album, result.AlbumName) * 15;

            if (song.Duration > 0 && result.Duration > 0)
            {
                double durationDifference = Math.Abs(song.Duration - result.Duration);
                if (durationDifference <= 2)
                {
                    score += 25;
                }
                else if (durationDifference <= 10)
                {
                    score += 10;
                }
                else if (durationDifference <= 30)
                {
                    score += 3;
                }
            }

            if (!string.IsNullOrWhiteSpace(result.PlainLyrics))
            {
                score += 5;
            }
            else if (!string.IsNullOrWhiteSpace(result.SyncedLyrics))
            {
                score += 1;
            }

            return score;
        }

        private static string PrepareLyrics(LrcLibSearchResult result)
        {
            string lyrics = result.PlainLyrics;
            if (string.IsNullOrWhiteSpace(lyrics))
            {
                lyrics = StripSyncTags(result.SyncedLyrics);
            }

            if (string.IsNullOrWhiteSpace(lyrics))
            {
                return null;
            }

            return lyrics
                .Replace("\r\n", "<br />")
                .Replace("\n", "<br />")
                .Replace("\r", "<br />");
        }

        private static string StripSyncTags(string syncedLyrics)
        {
            if (string.IsNullOrWhiteSpace(syncedLyrics))
            {
                return null;
            }

            var lines = new List<string>();
            foreach (string line in syncedLyrics.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None))
            {
                string cleanedLine = Regex.Replace(line, @"\[[0-9]{1,2}:[0-9]{2}(?:\.[0-9]{1,3})?\]", "").TrimEnd();
                lines.Add(cleanedLine);
            }

            return string.Join(Environment.NewLine, lines).Trim();
        }

        private static bool IsArtistMatch(MusicPlayer.SongInfo song, string resultArtist)
        {
            return IsPotentialMatch(song.Artist, resultArtist)
                || IsPotentialMatch(song.AlbumArtist, resultArtist);
        }

        private static int ScoreExactness(string expected, string actual)
        {
            string normalizedExpected = Normalize(expected);
            string normalizedActual = Normalize(actual);

            if (string.IsNullOrEmpty(normalizedExpected) || string.IsNullOrEmpty(normalizedActual))
            {
                return 0;
            }

            if (normalizedExpected == normalizedActual)
            {
                return 3;
            }

            if (normalizedActual.Contains(normalizedExpected) || normalizedExpected.Contains(normalizedActual))
            {
                return 1;
            }

            return 0;
        }

        private static bool IsPotentialMatch(string expected, string actual)
        {
            return ScoreExactness(expected, actual) > 0;
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(value.Length);
            foreach (char character in value.ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(character))
                {
                    builder.Append(character);
                }
                else if (char.IsWhiteSpace(character))
                {
                    builder.Append(' ');
                }
            }

            return Regex.Replace(builder.ToString(), @"\s+", " ").Trim();
        }

        private static string FirstNonEmpty(params string[] values)
        {
            return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
        }

        [DataContract]
        private class LrcLibSearchResult
        {
            [DataMember(Name = "name")]
            public string Name { get; set; }

            [DataMember(Name = "trackName")]
            public string TrackName { get; set; }

            [DataMember(Name = "artistName")]
            public string ArtistName { get; set; }

            [DataMember(Name = "albumName")]
            public string AlbumName { get; set; }

            [DataMember(Name = "duration")]
            public double Duration { get; set; }

            [DataMember(Name = "instrumental")]
            public bool Instrumental { get; set; }

            [DataMember(Name = "plainLyrics")]
            public string PlainLyrics { get; set; }

            [DataMember(Name = "syncedLyrics")]
            public string SyncedLyrics { get; set; }
        }
    }
}
