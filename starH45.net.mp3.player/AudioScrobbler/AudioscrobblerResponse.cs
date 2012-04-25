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
using System.Collections.Generic;

namespace starH45.net.mp3.player
{
	/// <summary>
	/// Wraps the text response from the audioscrobbler request
	/// </summary>
	internal class AudioscrobblerResponse
	{
		// the type of response this is
		private AudioscrobblerResponseType type;

		// the wait interval for the next response
		private int interval;

		// any other variables associated with this response
		private IDictionary<string, string> variables;

		public AudioscrobblerResponse()
		{
			variables = new Dictionary<string, string>();
		}

		// any other variables associated with this response
		public IDictionary<string, string> Variables
		{
			get { return variables; }
			set { variables = value; }
		}

		// the wait interval for the next response
		public int Interval
		{
			get { return interval; }
			set { interval = value; }
		}

		// the type of response this is
		public AudioscrobblerResponseType Type
		{
			get { return type; }
			set { type = value; }
		}
	}
}
