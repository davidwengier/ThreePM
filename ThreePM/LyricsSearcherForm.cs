using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ThreePM.Utilities;

namespace ThreePM
{
    public partial class LyricsSearcherForm : BaseForm
    {
        public bool OnlyLyricsFile = false;
        private LyricsHelper _helper;
        private DataSet _files;
        private int _count;
        private int _val;

        public LyricsSearcherForm()
        {
            InitializeComponent();
        }

        protected override void InitLibrary()
        {
            _helper = new LyricsHelper(this.Library);
            _files = this.Library.GetDataSet("SELECT LibraryID, Filename FROM Library WHERE (Lyrics IS NULL OR Lyrics = '') AND LibraryID >= " + Registry.GetValue("LyricsSearcherForm.LastDone", 0) + " ORDER BY LibraryID");
            _count = _files.Tables[0].Rows.Count;
            progressBar1.Maximum = _count;
            _helper.LyricsFound += new EventHandler<LyricsFoundEventArgs>(Helper_LyricsFound);
            _helper.LyricsNotFound += new EventHandler(Helper_LyricsNotFound);
            _val = 0;
            progressBar1.Value = _val;
            Registry.SetValue("LyricsSearcherForm.LastDone", Convert.ToInt32(_files.Tables[0].Rows[_val]["LibraryID"]));
            _helper.LoadLyrics(this.Library.GetSong(_files.Tables[0].Rows[_val]["Filename"].ToString()), false, OnlyLyricsFile, OnlyLyricsFile);
            lblStatus.Text = "Searching: " + _helper.Song.ToString();
        }

        private void LyricsSearcherForm_Load(object sender, EventArgs e)
        {

        }

        private void Helper_LyricsNotFound(object sender, EventArgs e)
        {
            if (_files != null)
            {
                _val++;
                progressBar1.Value = _val;
                Registry.SetValue("LyricsSearcherForm.LastDone", Convert.ToInt32(_files.Tables[0].Rows[_val]["LibraryID"]));
                _helper.LoadLyrics(this.Library.GetSong(_files.Tables[0].Rows[_val]["Filename"].ToString()), false, OnlyLyricsFile, OnlyLyricsFile);
                lblStatus.Text = "Searching: " + _helper.Song.ToString();
            }
        }

        private void Helper_LyricsFound(object sender, LyricsFoundEventArgs e)
        {
            this.Library.SetLyrics(_helper.Song.Title, _helper.Song.Artist, e.Lyrics);
            if (_files != null)
            {
                _val++;
                progressBar1.Value = _val;
                Registry.SetValue("LyricsSearcherForm.LastDone", Convert.ToInt32(_files.Tables[0].Rows[_val]["LibraryID"]));
                _helper.LoadLyrics(this.Library.GetSong(_files.Tables[0].Rows[_val]["Filename"].ToString()), false, OnlyLyricsFile, OnlyLyricsFile);
                lblStatus.Text = "Searching: " + _helper.Song.ToString();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _files = null;
            this.Close();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            _helper.LyricsFound -= new EventHandler<LyricsFoundEventArgs>(Helper_LyricsFound);
            _helper.LyricsNotFound -= new EventHandler(Helper_LyricsNotFound);
            Registry.SetValue("LyricsSearcherForm.LastDone", 0);
            InitLibrary();
        }
    }
}
