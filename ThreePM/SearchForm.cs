using System;
using System.Windows.Forms;

namespace ThreePM
{
    /// <summary>
    /// Description of SearchForm.
    /// </summary>
    public partial class SearchForm : BaseForm
    {
        public SearchForm()
        {
            InitializeComponent();
        }

        protected override void InitLibrary()
        {
            searchControl1.Library = this.Library;
        }

        protected override void InitPlayer()
        {
            searchControl1.Player = this.Player;
            this.Player.SongForced += new EventHandler(Player_SongForced);
        }

        protected override void UnInitPlayer()
        {
            this.Player.SongForced -= new EventHandler(Player_SongForced);
        }

        private void Player_SongForced(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SearchForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void searchControl1_SongPlayed(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void searchControl1_SongQueued(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
