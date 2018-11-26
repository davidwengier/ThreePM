using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ThreePM.UI
{
    /// <summary>
    /// Description of Ticker.
    /// </summary>
    public partial class Ticker
    {
        private readonly ComponentResourceManager _resources;
        private Image _bullet;
        private bool _fireWhileSliding = true;
        private bool _ignoreSet;
        private double _position = 50F;
        private double _duration = 100F;

        public event EventHandler PositionChanging;
        public event EventHandler PositionChanged;

        public bool FireWhileSliding
        {
            get
            {
                return _fireWhileSliding;
            }
            set
            {
                _fireWhileSliding = value;
            }
        }

        [Browsable(false)]
        public double Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                if (_position > value)
                {
                    _position = 0;
                }
                _duration = value;
            }
        }

        [Browsable(false)]
        public double Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_ignoreSet) return;
                _position = Math.Max(0, value);
                _position = Math.Min(value, _duration);
                Invalidate();
            }
        }

        public void SetPosition(double position)
        {
            if (this._position != position)
            {
                this.Position = position;
                if (PositionChanged != null)
                {
                    PositionChanged(this, EventArgs.Empty);
                }
            }
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                if (_bullet != null)
                {
                    _bullet.Dispose();
                }
                object o = _resources.GetObject(value.Name);
                if (o == null)
                {
                    o = _resources.GetObject("Gray");
                }
                _bullet = (Image)o;
            }
        }

        public Ticker()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            _resources = new System.ComponentModel.ComponentResourceManager(typeof(Ticker));
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (_bullet != null)
            {
                base.SetBoundsCore(x, y, width, _bullet.Height, specified);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen p = new Pen(ForeColor))
            {
                e.Graphics.DrawLine(p, 0, Height / 2, Width, Height / 2);
                e.Graphics.DrawLine(p, 0, Height / 2, Width, Height / 2);
                if (_duration > 0)
                {
                    int i = Convert.ToInt32((_position / _duration) * Width);
                    e.Graphics.DrawImage(_bullet, i - (_bullet.Width / 2), 0, _bullet.Width, _bullet.Height);
                }
            }

            base.OnPaint(e);
        }

        private void SetPosition(int percent, bool raiseEvent)
        {
            _position = ((float)percent / (float)Width) * _duration;
            _position = Math.Max(0, _position);
            _position = Math.Min(_position, _duration);
            if (raiseEvent)
            {
                if (PositionChanged != null)
                {
                    PositionChanged(this, EventArgs.Empty);
                }
            }
            else
            {
                if (PositionChanging != null)
                {
                    PositionChanging(this, EventArgs.Empty);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _ignoreSet = true;
            SetPosition(e.X, _fireWhileSliding);
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_ignoreSet)
            {
                SetPosition(e.X, _fireWhileSliding);
                Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            SetPosition(e.X, true);

            _ignoreSet = false;
            Invalidate();
            base.OnMouseUp(e);
        }
    }
}
