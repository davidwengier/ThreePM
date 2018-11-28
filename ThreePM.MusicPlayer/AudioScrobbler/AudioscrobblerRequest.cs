/*
 * Copyright (c) 2006 Monsur Hossain

 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the 
 * "Software"), to deal in the Software without restriction, including 
 * without limitation the rights to use, copy, modify, merge, publish, 
 * distribute, sublicense, and/or sell copies of the Software, and to 
 * permit persons to whom the Software is furnished to do so, subject to 
 * the following conditions:

 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ThreePM.MusicPlayer
{
    /// <summary>
    /// Sends/Processes requests to Audioscrobbler
    /// </summary>
    public class AudioscrobblerRequest
    {
        // date of the last track submission
        private DateTime _lastRequest = DateTime.MinValue;

        // interval to wait before next request
        private int _interval = 0;

        // url used to auth user when sending track information
        private string _urlPrefix = string.Empty;

        // indicates whether the handshake was successful or not
        private bool _handshakeSuccessful = false;

        // last.fm auth settings
        private string _username = string.Empty;
        private string _password = string.Empty;

        public string Username
        {
            set
            {
                _username = value;
            }
        }

        public string Password
        {
            set
            {
                // has the password before storing it
                _password = this.CalculateMD5(value);
            }
        }

        public int Interval
        {
            get
            {
                return _interval;
            }
        }

        /// <summary>
        /// Send a request to the audioscrobbler server
        /// parse the response into the approriate 
        /// AudioscrobblerReponse type
        /// </summary>
        private AudioscrobblerResponse Send(string url)
        {
            // the response object to return
            AudioscrobblerResponse aResponse = null;

            // create the request
            var request = (HttpWebRequest)WebRequest.Create(url);

            // set the method to POST
            request.Method = "POST";
            request.ContentLength = 0;

            // grab the response
            // TODO: Change response type to HttpWebResponse
            // TODO: Better error handling
            using (WebResponse response = request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        // parse the response string
                        aResponse = this.ParseResponse(reader.ReadToEnd());
                    }
                }
            }
            return aResponse;
        }

        /// <summary>
        /// Parse the response string into the approriate type
        /// </summary>
        private AudioscrobblerResponse ParseResponse(string responseString)
        {
            // if for any reason the response is empty (why would it be?),
            // return null
            if (responseString.Length == 0)
                return null;

            var response = new AudioscrobblerResponse();

            // figure out the response type and parse it approriately
            if (RequestStartsWith(responseString, "UPTODATE"))
            {
                response = GetResponse_UPTODATE(responseString);
            }
            else if (RequestStartsWith(responseString, "UPDATE"))
            {
                response = GetResponse_UPDATE(responseString);
            }
            else if (RequestStartsWith(responseString, "FAILED"))
            {
                response = GetResponse_FAILED(responseString);
            }
            else if (RequestStartsWith(responseString, "BADUSER"))
            {
                response = GetResponse_BADUSER(responseString);
            }
            else if (RequestStartsWith(responseString, "BADAUTH"))
            {
                response = GetResponse_BADAUTH(responseString);
            }
            else if (RequestStartsWith(responseString, "OK"))
            {
                response = GetResponse_OK(responseString);
            }
            else
            {
                response = GetResponse_UNKNOWN(responseString);
            }

            return response;
        }

        #region Response parsers

        // All the response parsers below work the same way
        // Set the approriate AudioscrobblerResponseType
        // and then user a regular expression to parse out the interval
        // and the variables

        private AudioscrobblerResponse GetResponse_UPTODATE(string responseString)
        {
            var response = new AudioscrobblerResponse();
            response.Type = AudioscrobblerResponseType.UPTODATE;

            string regex = @"UPTODATE\n(?<MD5Challenge>[^\n]*)\n(?<UrlToSubmitScript>[^\n]*)\nINTERVAL (?<Interval>[0-9]*)";
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
            var reg = new Regex(regex, options);

            Match match = reg.Match(responseString);
            if (match.Success)
            {
                response.Variables.Add("MD5Challenge", match.Groups["MD5Challenge"].Value);
                response.Variables.Add("UrlToSubmitScript", match.Groups["UrlToSubmitScript"].Value);
                response.Interval = Convert.ToInt32(match.Groups["Interval"].Value);
            }

            return response;
        }

        private AudioscrobblerResponse GetResponse_UPDATE(string responseString)
        {
            var response = new AudioscrobblerResponse();
            response.Type = AudioscrobblerResponseType.UPDATE;

            string regex = @"UPDATE (?<UpdateUrl>[^\n]*)\n(?<MD5Challenge>[^\n]*)\n(?<UrlToSubmitScript>[^\n]*)\nINTERVAL (?<Interval>[0-9]*)";
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
            var reg = new Regex(regex, options);

            Match match = reg.Match(responseString);
            if (match.Success)
            {
                response.Variables.Add("UpdateUrl", match.Groups["UpdateUrl"].Value);
                response.Variables.Add("MD5Challenge", match.Groups["MD5Challenge"].Value);
                response.Variables.Add("UrlToSubmitScript", match.Groups["UrlToSubmitScript"].Value);
                response.Interval = Convert.ToInt32(match.Groups["Interval"].Value);
            }

            return response;
        }

        private AudioscrobblerResponse GetResponse_FAILED(string responseString)
        {
            var response = new AudioscrobblerResponse();
            response.Type = AudioscrobblerResponseType.FAILED;

            string regex = @"FAILED (?<Reason>[^\n]*)\nINTERVAL (?<Interval>[0-9]*)";
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
            var reg = new Regex(regex, options);

            Match match = reg.Match(responseString);
            if (match.Success)
            {
                response.Variables.Add("Reason", match.Groups["Reason"].Value);
                response.Interval = Convert.ToInt32(match.Groups["Interval"].Value);
            }

            return response;
        }

        private AudioscrobblerResponse GetResponse_BADUSER(string responseString)
        {
            var response = new AudioscrobblerResponse();
            response.Type = AudioscrobblerResponseType.BADUSER;

            string regex = @"BADUSER\nINTERVAL (?<Interval>[0-9]*)";
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
            var reg = new Regex(regex, options);

            Match match = reg.Match(responseString);
            if (match.Success)
            {
                response.Interval = Convert.ToInt32(match.Groups["Interval"].Value);
            }

            return response;
        }

        private AudioscrobblerResponse GetResponse_BADAUTH(string responseString)
        {
            var response = new AudioscrobblerResponse();
            response.Type = AudioscrobblerResponseType.BADAUTH;

            string regex = @"BADAUTH\nINTERVAL (?<Interval>[0-9]*)";
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
            var reg = new Regex(regex, options);

            Match match = reg.Match(responseString);
            if (match.Success)
            {
                response.Interval = Convert.ToInt32(match.Groups["Interval"].Value);
            }

            return response;
        }

        private AudioscrobblerResponse GetResponse_OK(string responseString)
        {
            var response = new AudioscrobblerResponse();
            response.Type = AudioscrobblerResponseType.OK;

            string regex = @"OK\nINTERVAL (?<Interval>[0-9]*)";
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
            var reg = new Regex(regex, options);

            Match match = reg.Match(responseString);
            if (match.Success)
            {
                response.Interval = Convert.ToInt32(match.Groups["Interval"].Value);
            }

            return response;
        }

        private AudioscrobblerResponse GetResponse_UNKNOWN(string responseString)
        {
            var response = new AudioscrobblerResponse();
            response.Type = AudioscrobblerResponseType.UNKNOWN;
            return response;
        }

        /// <summary>
        /// Determines the response type by checking to see 
        /// what the http response string begins with
        /// </summary>
        private bool RequestStartsWith(string line, string requestType)
        {
            return line.StartsWith(requestType);
        }

        #endregion Response parsers

        /// <summary>
        /// Calculate the MD5 hash
        /// </summary>
        private string CalculateMD5(string input)
        {
            MD5 md = MD5CryptoServiceProvider.Create();
            var enc = new UTF8Encoding();
            byte[] buffer = enc.GetBytes(input);
            byte[] hash = md.ComputeHash(buffer);
            string md5 = string.Empty;
            for (int i = 0; i < hash.Length; i++)
            {
                md5 += hash[i].ToString("x2");
            }
            return md5;
        }

        // establish the connection between the client and audioscrobbler
        private void Handshake()
        {
            // handshake url
            // {0} = clientid
            // {1} = client version
            // {2} = username
            string handshakeUrl = "http://post.audioscrobbler.com/?hs=true&p=1.1&c={0}&v={1}&u={2}";

            // values for client
            string clientid = "tst";
            string clientversion = "1.0";

            // reset variables that are set during the handshake
            _urlPrefix = string.Empty;
            _handshakeSuccessful = false;

            // generate the approriate handshake url 
            handshakeUrl = string.Format(handshakeUrl, clientid, clientversion, _username);

            // send the response
            AudioscrobblerResponse response = this.Send(handshakeUrl);

            // set the interval value returned by the response
            _interval = response.Interval;

            // react based on the response type
            switch (response.Type)
            {
                // successful response: grab the url to send tracks to
                case AudioscrobblerResponseType.UPTODATE:
                    _urlPrefix = GetUrlPrefix(response.Variables["MD5Challenge"], response.Variables["UrlToSubmitScript"]);
                    _handshakeSuccessful = true;
                    break;

                // successful response: grab the url to send tracks to
                case AudioscrobblerResponseType.UPDATE:
                    _urlPrefix = GetUrlPrefix(response.Variables["MD5Challenge"], response.Variables["UrlToSubmitScript"]);
                    _handshakeSuccessful = true;
                    break;

                // invalid user
                case AudioscrobblerResponseType.BADUSER:
                    throw new AudioscrobblerException("Invalid User");

                // request failed for some other reason
                case AudioscrobblerResponseType.FAILED:
                    throw new AudioscrobblerException(response.Variables["Reason"]);
            }
        }

        /// <summary>
        /// After a successful handshake, an md5 challenge and url are sent back
        /// those can be used to generate a url to submit updated tracks to
        /// this function generates the static portion of that url based 
        /// on the username, password, md5 challenge and url.
        /// </summary>
        private string GetUrlPrefix(string md5Challenge, string urlToSubmitScript)
        {
            // format of the url used to authenticate the user 
            // on each track submission
            // {0} - username
            // {1} - MD5 response
            string urlPrefixFormat = "u={0}&s={1}";

            return urlToSubmitScript + "?" + string.Format(urlPrefixFormat, _username, CalculateMD5(_password + md5Challenge));
        }

        /// <summary>
        /// Submit a single track to audioscrobbler
        /// </summary>
        public void SubmitTrack(SongInfo track)
        {
            // verify that a successful handshake has occured
            if (_handshakeSuccessful == false)
            {
                Handshake();
            }

            // initialize the url to send requests to
            string url = _urlPrefix;

            url += ProcessTrack(track);

            // send the request
            AudioscrobblerResponse response = Send(url);

            // set the interval variable
            _interval = response.Interval;

            // parse the response type
            // (doesn't do anything for now)
            switch (response.Type)
            {
                case AudioscrobblerResponseType.BADAUTH:
                    break;

                case AudioscrobblerResponseType.FAILED:
                    break;

                case AudioscrobblerResponseType.OK:
                    break;
            }

            // update the date of the last request
            // (isn't used now, but could be used in conjunction
            // with interval to wait before submitting a request)
            _lastRequest = DateTime.Now;
        }

        private string ProcessTrack(SongInfo track)
        {
            // {0} - track num
            // {1} - artist name
            // {2} - track name
            // {3} - album name
            // {4} - musicbrainz id
            // {5} - track length
            // {6} - date in YYYY-MM-DD mm:hh:ss format
            string urlTrack = "&a[{0}]={1}&t[{0}]={2}&b[{0}]={3}&m[{0}]={4}&l[{0}]={5}&i[{0}]={6}";
            int dur = (int)track.Duration;
            if (dur == 0)
            {
                // defaults to about 3:20, in order to get audioscrobbler to work
                dur = 200;
            }
            return string.Format(urlTrack, 0, HttpUtility.UrlEncode(track.Artist), HttpUtility.UrlEncode(track.Title), HttpUtility.UrlEncode(track.Album), "", dur, HttpUtility.UrlEncode(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")));
        }
    }
}
