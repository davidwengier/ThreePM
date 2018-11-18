using System;
using System.Collections.Generic;
using System.Text;

namespace ThreePM.player
{
	public class ScanStatusEventArgs : EventArgs
	{
		private string m_status;

		public string Status
		{
			get { return m_status; }
		}

		public ScanStatusEventArgs(string status)
		{
			m_status = status;
		}
	}
}
