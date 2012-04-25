using System;
using System.Collections.Generic;
using System.Text;

namespace starH45.net.mp3.library
{
    public class LibraryEntryEventsArgs : EventArgs
    {
		private LibraryEntry m_libraryEntry;

		public LibraryEntry LibraryEntry
        {
			get { return m_libraryEntry; }
        }

		public LibraryEntryEventsArgs(LibraryEntry libraryEntry)
        {
			m_libraryEntry = libraryEntry;
        }
    }
}
