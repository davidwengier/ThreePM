using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using starH45.net.mp3.utilities;

namespace starH45.net.mp3
{
	public partial class LyricsSearcherForm : BaseForm
	{
		public bool OnlyLyricsFile = false;
		private LyricsHelper helper;
		DataSet files;
		int count;
		int val;

		public LyricsSearcherForm()
		{
			InitializeComponent();
		}

		protected override void InitLibrary()
		{
			helper = new LyricsHelper(Library);
			files = Library.GetDataSet("SELECT LibraryID, Filename FROM Library WHERE (Lyrics IS NULL OR Lyrics = '') AND LibraryID >= " + Utilities.GetValue("LyricsSearcherForm.LastDone", 0) + " ORDER BY LibraryID");
			count = files.Tables[0].Rows.Count;
			progressBar1.Maximum = count;
			helper.LyricsFound += new EventHandler<LyricsFoundEventArgs>(helper_LyricsFound);
			helper.LyricsNotFound += new EventHandler(helper_LyricsNotFound);
			val = 0;
			progressBar1.Value = val;
			Utilities.SetValue("LyricsSearcherForm.LastDone", Convert.ToInt32(files.Tables[0].Rows[val]["LibraryID"]));
			helper.LoadLyrics(Library.GetSong(files.Tables[0].Rows[val]["Filename"].ToString()), false, OnlyLyricsFile, OnlyLyricsFile);
			lblStatus.Text = "Searching: " + helper.Song.ToString();
		}

		private void LyricsSearcherForm_Load(object sender, EventArgs e)
		{
			
		}

		void helper_LyricsNotFound(object sender, EventArgs e)
		{
			if (files != null)
			{
				val++;
				progressBar1.Value = val;
				Utilities.SetValue("LyricsSearcherForm.LastDone", Convert.ToInt32(files.Tables[0].Rows[val]["LibraryID"]));
				helper.LoadLyrics(Library.GetSong(files.Tables[0].Rows[val]["Filename"].ToString()), false, OnlyLyricsFile, OnlyLyricsFile);
				lblStatus.Text = "Searching: " + helper.Song.ToString();
			}
		}

		void helper_LyricsFound(object sender, LyricsFoundEventArgs e)
		{
			Library.SetLyrics(helper.Song.Title, helper.Song.Artist, e.Lyrics);
			if (files != null)
			{
				val++;
				progressBar1.Value = val;
				Utilities.SetValue("LyricsSearcherForm.LastDone", Convert.ToInt32(files.Tables[0].Rows[val]["LibraryID"]));
				helper.LoadLyrics(Library.GetSong(files.Tables[0].Rows[val]["Filename"].ToString()), false, OnlyLyricsFile, OnlyLyricsFile);
				lblStatus.Text = "Searching: " + helper.Song.ToString();
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			files = null;
			this.Close();
		}

		private void btnRestart_Click(object sender, EventArgs e)
		{
			helper.LyricsFound -= new EventHandler<LyricsFoundEventArgs>(helper_LyricsFound);
			helper.LyricsNotFound -= new EventHandler(helper_LyricsNotFound);
			Utilities.SetValue("LyricsSearcherForm.LastDone", 0);
			InitLibrary();
		}
	}
}