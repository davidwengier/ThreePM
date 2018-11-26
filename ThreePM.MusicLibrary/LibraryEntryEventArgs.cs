using System;
using System.Collections.Generic;
using System.Text;

namespace ThreePM.MusicLibrary
{
    public class LibraryEntryEventArgs : EventArgs
    {
        private readonly LibraryEntry _libraryEntry;

        public LibraryEntry LibraryEntry
        {
            get { return _libraryEntry; }
        }

        public LibraryEntryEventArgs(LibraryEntry libraryEntry)
        {
            _libraryEntry = libraryEntry;
        }
    }
}
