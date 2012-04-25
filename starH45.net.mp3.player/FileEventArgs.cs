using System;
using System.Collections.Generic;
using System.Text;

namespace starH45.net.mp3.player
{
	public class FileEventArgs : EventArgs
	{
		private string m_filename;

		public string Filename
		{
			get { return m_filename; }
		}

        public FileEventArgs(string filename)
		{
            m_filename = filename;
		}
	}
}
