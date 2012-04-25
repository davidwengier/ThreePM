using System;
using System.Collections.Generic;
using System.Text;

namespace starH45.net.mp3.library
{
    public class LibraryEntryEventArgs : EventArgs
    {
		private LibraryEntry m_libraryEntry;

		public LibraryEntry LibraryEntry
        {
			get { return m_libraryEntry; }
        }

		public LibraryEntryEventArgs(LibraryEntry libraryEntry)
        {
			m_libraryEntry = libraryEntry;
        }
    }
}
