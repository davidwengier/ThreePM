using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using starH45.net.mp3.player;
using System.ComponentModel;

namespace starH45.net.mp3
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

        private State m_state = State.Normal;
        private bool m_active;
        private Image m_mouseDownImage;
        private Image m_normalImage;
        private Image m_activeButton;

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
            get { return m_active; }
            set { m_active = value;
            SetImage();
            }
        }

        public Image ActiveButton
        {
            get { return m_activeButton; }
            set
            {
                m_activeButton = value;
                SetImage();
            }
        }
        
        public Image MouseDownImage
        {
            get { return m_mouseDownImage; }
            set
            {
                m_mouseDownImage = value;
                SetImage();
            }
        }
        
        public Image NormalImage
        {
            get { return m_normalImage; }
            set { m_normalImage = value;
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
            switch (m_state)
            {
                case State.Normal:
                base.BackgroundImage = m_normalImage;
                break;
                case State.Pressed:
                base.BackgroundImage = m_mouseDownImage;
                break;
                case State.Active:
                base.BackgroundImage = m_activeButton;
                break;
            }
        }

        #endregion

        #region Overridden Methods

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_state = State.Pressed;
            SetImage();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            m_state = (m_active ? State.Active : State.Normal);
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
