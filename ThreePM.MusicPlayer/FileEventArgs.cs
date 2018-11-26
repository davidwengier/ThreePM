using System;
using System.Collections.Generic;
using System.Text;

namespace ThreePM.MusicPlayer
{
    public class FileEventArgs : EventArgs
    {
        private readonly string _filename;

        public string Filename
        {
            get { return _filename; }
        }

        public FileEventArgs(string filename)
        {
            _filename = filename;
        }
    }
}
