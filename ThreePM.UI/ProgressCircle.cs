using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ThreePM.UI
{
    public class ProgressCircle : Control
    {
        #region Declarations

        private SolidBrush _inactiveBrush;
        private SolidBrush _activeBrush;
        private SolidBrush _transitionBrush;
        private Color _inactiveColour;
        private Color _activeColour;
        private Color _transitionColour;
        private Region _innerBackgroundRegion;
        private readonly GraphicsPath[] _segmentPaths = new GraphicsPath[12];
        private bool _behindIsActive = true;
        private int _transitionSegment = -1;
        private System.Timers.Timer _timer;

        #endregion

        #region Properties

        public Color InactiveSegmentColour
        {
            get
            {
                return _inactiveColour;
            }
            set
            {
                _inactiveColour = value;
                if (_inactiveBrush != null)
                {
                    _inactiveBrush.Dispose();
                }
                _inactiveBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        public Color ActiveSegmentColour
        {
            get
            {
                return _activeColour;
            }
            set
            {
                _activeColour = value;
                if (_activeBrush != null)
                {
                    _activeBrush.Dispose();
                }
                _activeBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        public Color TransistionSegmentColour
        {
            get
            {
                return _transitionColour;
            }
            set
            {
                _transitionColour = value;
                if (_transitionBrush != null)
                {
                    _transitionBrush.Dispose();
                }
                _transitionBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        public int Value
        {
            get
            {
                return _transitionSegment;
            }
            set
            {
                if (value > 11 | value < -1)
                {
                    throw new ArgumentException("TransistionSegment must be between -1 and 11");
                }
                _transitionSegment = value;
                Invalidate();
            }
        }

        #endregion

        #region Constructor

        public ProgressCircle()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.Size = new System.Drawing.Size(30, 30);
            this.InactiveSegmentColour = Color.FromArgb(218, 218, 218);
            this.ActiveSegmentColour = Color.FromArgb(35, 146, 33);
            this.TransistionSegmentColour = Color.FromArgb(129, 242, 121);

            CalculateSegments();
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            _timer = new System.Timers.Timer(50);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            _timer.Start();
        }

        public void Stop()
        {
            _behindIsActive = true;
            this.Value = -1;
            _timer.Stop();
            _timer.Dispose();
        }

        public void Increment()
        {
            if (_transitionSegment == 11)
            {
                _transitionSegment = 0;
                _behindIsActive = !_behindIsActive;
            }
            else if (_transitionSegment == -1)
            {
                _transitionSegment = 0;
            }
            else
            {
                _transitionSegment += 1;
            }
            Invalidate();
        }

        #endregion

        #region Overridden Methods

        protected override void Dispose(bool disposing)
        {
            _activeBrush.Dispose();
            _inactiveBrush.Dispose();
            _transitionBrush.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.ExcludeClip(_innerBackgroundRegion);
            for (int intCount = 0; intCount <= 11; intCount++)
            {
                if (this.Enabled)
                {
                    if (intCount == _transitionSegment)
                    {
                        //If this segment is the transistion segment, colour it differently
                        e.Graphics.FillPath(_transitionBrush, _segmentPaths[intCount]);
                    }
                    else if (intCount < _transitionSegment)
                    {
                        //This segment is behind the transistion segment
                        if (_behindIsActive)
                        {
                            //If behind the transistion should be active, 
                            //colour it with the active colour
                            e.Graphics.FillPath(_activeBrush, _segmentPaths[intCount]);
                        }
                        else
                        {
                            //If behind the transistion should be in-active, 
                            //colour it with the in-active colour
                            e.Graphics.FillPath(_inactiveBrush, _segmentPaths[intCount]);
                        }
                    }
                    else
                    {
                        //This segment is ahead of the transistion segment
                        if (_behindIsActive)
                        {
                            //If behind the the transistion should be active, 
                            //colour it with the in-active colour
                            e.Graphics.FillPath(_inactiveBrush, _segmentPaths[intCount]);
                        }
                        else
                        {
                            //If behind the the transistion should be in-active, 
                            //colour it with the active colour
                            e.Graphics.FillPath(_activeBrush, _segmentPaths[intCount]);
                        }
                    }
                }
                else
                {
                    //Draw all segments in in-active colour if not enabled
                    e.Graphics.FillPath(_inactiveBrush, _segmentPaths[intCount]);
                }
            }
            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            CalculateSegments();
            base.OnResize(e);
        }

        #endregion

        #region Control Events

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Increment();
        }

        #endregion

        #region Private Methods

        private void CalculateSegments()
        {
            var rctFull = new Rectangle(0, 0, this.Width, this.Height);
            var rctInner = new RectangleF(this.Width * (7F / 30F), this.Height * (7F / 30F), this.Width - (this.Width * (7F / 30F) * 2), this.Height - (this.Height * (7F / 30F) * 2));
            GraphicsPath pthInnerBackground;
            //Create 12 segment pieces
            for (int intCount = 0; intCount <= 11; intCount++)
            {
                _segmentPaths[intCount] = new GraphicsPath();
                //We subtract 90 so that the starting segment is at 12 o'clock
                _segmentPaths[intCount].AddPie(rctFull, (intCount * 30) - 90, 25);
            }
            //Create the center circle cut-out
            pthInnerBackground = new GraphicsPath();
            pthInnerBackground.AddPie(rctInner.X, rctInner.Y, rctInner.Width, rctInner.Height, 0, 360);
            _innerBackgroundRegion = new Region(pthInnerBackground);
        }

        #endregion
    }
}
