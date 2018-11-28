using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ThreePM.MusicPlayer;

namespace ThreePM
{
    public partial class AlbumArtPicker : BaseForm
    {
        private string _filename;
        private PictureBox _current;


        protected override Control.ControlCollection DynamicControlsContainer
        {
            get { return ControlsPanel.Controls; }
        }

        public AlbumArtPicker(SongInfo song)
        {
            InitializeComponent();

            _filename = song.FileName;
            this.Caption += song.Album + " - " + song.Artist;

            foreach (ToolStripItem item in contextMenuStrip2.Items)
            {
                item.Text = item.Text.Replace("<artist>", song.Artist).Replace("<title>", song.Title).Replace("<album>", song.Album);
            }

            PerformSearch("\"" + song.Artist + "\" \"" + song.Album + "\"");
        }

        private void PerformSearch(string query)
        {
            // First clear out any previous search results because they were obviously useless
            if (flowLayoutPanel1.InvokeRequired)
            {
                flowLayoutPanel1.BeginInvoke(new MethodInvoker(delegate
                {
                    flowLayoutPanel1.Controls.Clear();
                }));
            }
            else
            {
                flowLayoutPanel1.Controls.Clear();
            }

            txtSearch.Text = query;
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.Visible = true;
            string googleUrl = "http://images.google.com/images?q=" + System.Web.HttpUtility.UrlEncode(query); ;

            try
            {
                var req = WebRequest.Create(googleUrl);
                req.BeginGetResponse(DownloadFinished, req);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error attempting album art download:\n\n" + ex.Message + "", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void DownloadFinished(IAsyncResult result)
        {
            string html;
            var req = (WebRequest)result.AsyncState;
            WebResponse response = req.EndGetResponse(result);
            using (Stream stream = response.GetResponseStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    html = sr.ReadToEnd();
                }
            }

            List<GISResult> urls = GetImagesFromGoogle(html);

            Invoke(new MethodInvoker(delegate
            {
                progressBar1.Maximum = urls.Count;
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.Minimum = 0;
                progressBar1.Value = 0;
            }));

            using (var c = new WebClient())
            {
                foreach (GISResult res in urls)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        if (progressBar1.Style == ProgressBarStyle.Blocks)
                        {
                            progressBar1.Increment(1);
                        }
                    }));
                    try
                    {
                        var pct = Image.FromStream(c.OpenRead(res.ThumbURL));
                        AddPicture(res, pct);
                    }
                    catch
                    { }
                }
            }
            Invoke(new MethodInvoker(delegate
            {
                progressBar1.Visible = false;
            }));
        }

        private void AddPicture(GISResult s, Image pct)
        {
            var pic = new PictureBox
            {
                Tag = s,
                SizeMode = PictureBoxSizeMode.AutoSize,
                BackColor = Color.Black,
                Padding = new Padding(1),
                Image = pct
            };
            pic.MouseDown += new MouseEventHandler(pic_MouseDown);
            toolTip1.SetToolTip(pic, "URL: " + s.OrigURL + "\nThumbnail Size: " + pct.Width + " x " + pct.Height + "\nFull Size: " + s.OrigSize);
            flowLayoutPanel1.BeginInvoke(new MethodInvoker(delegate
            {
                flowLayoutPanel1.Controls.Add(pic);
            }));
        }

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                if (_current != sender as PictureBox)
                {
                    if (_current != null)
                    {
                        _current.BackColor = Color.Black;
                    }
                    _current = sender as PictureBox;
                    _current.BackColor = Color.Red;
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                var pic = (sender as PictureBox);
                thisIsTheCoverToolStripMenuItem.Enabled = true;
                downloadFullSizeToolStripMenuItem.Enabled = true;
                itsNotThisOneBuggerItOffToolStripMenuItem.Enabled = true;
                if (pic.Tag == null)
                {
                    downloadFullSizeToolStripMenuItem.Enabled = false;
                }
                contextMenuStrip1.Show(pic.PointToScreen(new Point(e.X, e.Y)));
            }
        }

        private class GISResult
        {
            public string ThumbURL;
            public string OrigURL;
            public string OrigSize;

            public override string ToString()
            {
                return OrigURL;
            }
        }

        private static List<GISResult> GetImagesFromGoogle(string html)
        {
            var urlList = new List<GISResult>();

            var imagesRegex = new Regex(@"dyn\.Img\(\""(?<url>.*?)\"",\""(?<ignore>.*?)\"",\""(?<googleID>.*?)\"",\""(?<img>.*?)\"",\""(?<ignore>.*?)\"",\""(?<ignore>.*?)\"",\""(?<ignore>.*?)\"",\""(?<ignore>.*?)\"",\""(?<ignore>.*?)\"",\""(?<size>.*?)\""");

            //Regex imagesRegex = new Regex(@"(\x3Ca\s+href=/imgres\" +
            //           @"x3Fimgurl=)(?<imgurl>http" +
            //           @"[^&>]*)([>&]{1})" +
            //           @"([^>]*)(>{1})(<img\ssrc\" +
            //           @"x3D)(""{0,1})(?<images>/images" +
            //           @"[^""\s>]*)([\s])+(width=)" +
            //           @"(?<width>[0-9,]*)\s+(height=)" +
            //           @"(?<height>[0-9,]*)");
            //Regex dataRegex = new Regex(@"([^>]*)(>)\s{0,1}(<br>){0,1}\s{0,1}" +
            //         @"(?<width>[0-9,]*)\s+x\s+(?<height>[0-9,]*)" +
            //         @"\s+pixels\s+-\s+(?<size>[0-9,]*)(k)");

            MatchCollection images = imagesRegex.Matches(html);
            //MatchCollection data = dataRegex.Matches(html);
            int i = 0;
            foreach (Match m in images)
            {
                var result = new GISResult();
                result.ThumbURL = "http://images.google.com/images?q=tbn:" + m.Groups["googleID"].Value + m.Groups["img"].Value;
                result.OrigURL = m.Groups["img"].Value;
                result.OrigSize = m.Groups["size"].Value;
                urlList.Add(result);
                i++;
            }

            return urlList;
        }

        private void thisIsTheCoverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_current != null)
            {
                string path = _filename;
                path = Path.GetDirectoryName(path);
                if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    path += Path.DirectorySeparatorChar;
                }
                path += "Folder.jpg";
                _current.Image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                File.SetAttributes(path, FileAttributes.System | FileAttributes.Hidden);

                //AlbumArtHelper.InvalidateCache(m_filename);

                // tell the player to fire the SongOpened event again if its still playing the same song
                // that way everything will refresh through the magic of...: EVENTS!
                if (this.Player != null)
                {
                    if (this.Player.CurrentSong.FileName.Equals(_filename))
                    {
                        this.Player.OnSongOpened();
                    }
                }

                this.Close();
            }
        }

        private void downloadFullSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_current != null && _current.Tag != null)
            {
                this.UseWaitCursor = true;
                Application.DoEvents();
                string url = _current.Tag.ToString();
                _current.Tag = null;
                using (var c = new WebClient())
                {
                    try
                    {
                        var pct = Image.FromStream(c.OpenRead(url));
                        _current.Image = pct;
                    }
                    catch { MessageBox.Show("Could not download fullsize picture. You can still use this thumbnail if you like."); }
                }
                this.UseWaitCursor = false;
            }
        }

        private void itsNotThisOneBuggerItOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_current != null)
            {
                flowLayoutPanel1.Controls.Remove(_current);
                _current = null;
                flowLayoutPanel1.PerformLayout();
            }
        }

        private void searchAgain_Click(object sender, EventArgs e)
        {
            string query = ((ToolStripMenuItem)sender).Text;
            PerformSearch(query);
        }

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            contextMenuStrip2.Show(MousePosition);
        }

        private void btnDelete_Click(object sender, System.EventArgs e)
        {
            _current = null;
            flowLayoutPanel1.Controls.Clear();
        }

        private void btnDeleteOne_Click(object sender, System.EventArgs e)
        {
            if (_current == null)
            {
                MessageBox.Show("Please select a picture.");
                return;
            }

            itsNotThisOneBuggerItOffToolStripMenuItem.PerformClick();
        }

        private void btnDownloadFullSize_Click(object sender, System.EventArgs e)
        {
            if (_current == null)
            {
                MessageBox.Show("Please select a picture.");
                return;
            }

            downloadFullSizeToolStripMenuItem.PerformClick();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            if (_current == null)
            {
                MessageBox.Show("Please select a picture.");
                return;
            }

            thisIsTheCoverToolStripMenuItem.PerformClick();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                PerformSearch(((TextBox)sender).Text);
                contextMenuStrip1.Hide();
            }
        }
    }
}
