using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ThreePM
{
    public class Button : Control
    {
        #region Declarations

        private enum State
        {
            Normal,
            Pressed,
            Active
        }

        private State _state = State.Normal;
        private bool _active;
        private Image _mouseDownImage;
        private Image _normalImage;
        private Image _activeButton;

        #endregion

        #region Overridden Properties

        [Bindable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [Bindable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [Bindable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        #endregion

        #region Properties

        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                SetImage();
            }
        }

        public Image ActiveButton
        {
            get { return _activeButton; }
            set
            {
                _activeButton = value;
                SetImage();
            }
        }

        public Image MouseDownImage
        {
            get { return _mouseDownImage; }
            set
            {
                _mouseDownImage = value;
                SetImage();
            }
        }

        public Image NormalImage
        {
            get { return _normalImage; }
            set
            {
                _normalImage = value;
                SetImage();
            }
        }

        #endregion

        #region Constructor

        public Button()
        {
            base.SetStyle(ControlStyles.Selectable | ControlStyles.Opaque, false);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            this.TabStop = false;

            this.BackgroundImageLayout = ImageLayout.Center;
        }

        #endregion

        #region Methods

        private void SetImage()
        {
            switch (_state)
            {
                case State.Normal:
                    base.BackgroundImage = _normalImage;
                    break;
                case State.Pressed:
                    base.BackgroundImage = _mouseDownImage;
                    break;
                case State.Active:
                    base.BackgroundImage = _activeButton;
                    break;
            }
        }

        #endregion

        #region Overridden Methods

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _state = State.Pressed;
            SetImage();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _state = (_active ? State.Active : State.Normal);
            SetImage();
            base.OnMouseUp(e);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {

            base.SetBoundsCore(x, y, width, height, specified);
        }

        #endregion
    }
}
