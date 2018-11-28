namespace ThreePM
{
    public partial class InfoForm : BaseForm
    {
        public InfoForm()
        {
            InitializeComponent();
        }

        protected override void InitPlayer()
        {
            infoControl1.Player = this.Player;
        }

        protected override void InitLibrary()
        {
            infoControl1.Library = this.Library;
        }
    }
}
