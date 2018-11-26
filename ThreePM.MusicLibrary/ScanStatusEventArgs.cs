using System;
using System.Collections.Generic;
using System.Text;

namespace ThreePM.MusicLibrary
{
    public class ScanStatusEventArgs : EventArgs
    {
        private readonly string _status;

        public string Status
        {
            get { return _status; }
        }

        public ScanStatusEventArgs(string status)
        {
            _status = status;
        }
    }
}
