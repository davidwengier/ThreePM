using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace starH45.net.mp3.ui
{
    public class ProgressCircle : Control
	{
		#region Declarations

		private SolidBrush m_inactiveBrush;
        private SolidBrush m_activeBrush;
        private SolidBrush m_transitionBrush;
        private Color m_inactiveColour;
        private Color m_activeColour;
        private Color m_transitionColour;
        private Region innerBackgroundRegion;
        private GraphicsPath[] segmentPaths = new GraphicsPath[12];
        private bool m_behindIsActive = true;
        private int m_transitionSegment = -1;
        private System.Timers.Timer timer;

		#endregion

		#region Properties

		public Color InactiveSegmentColour
        {
            get
            {
                return m_inactiveColour;
            }
            set
            {
                m_inactiveColour = value;
                if (m_inactiveBrush != null)
                {
                    m_inactiveBrush.Dispose();
                }
                m_inactiveBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        public Color ActiveSegmentColour
        {
            get
            {
                return m_activeColour;
            }
            set
            {
                m_activeColour = value;
                if (m_activeBrush != null)
                {
                    m_activeBrush.Dispose();
                }
                m_activeBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        public Color TransistionSegmentColour
        {
            get
            {
                return m_transitionColour;
            }
            set
            {
                m_transitionColour = value;
                if (m_transitionBrush != null)
                {
                    m_transitionBrush.Dispose();
                }
                m_transitionBrush = new SolidBrush(value);
                Invalidate();
            }
        }

        public int Value
        {
            get
            {
                return m_transitionSegment;
            }
            set
            {
                if (value > 11 | value < -1)
                {
                    throw new ArgumentException("TransistionSegment must be between -1 and 11");
                }
                m_transitionSegment = value;
                Invalidate();
            }
		}

		#endregion

		#region Constructor

		public ProgressCircle()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.Size = new System.Drawing.Size(30, 30);
            InactiveSegmentColour = Color.FromArgb(218, 218, 218);
            ActiveSegmentColour = Color.FromArgb(35, 146, 33);
            TransistionSegmentColour = Color.FromArgb(129, 242, 121);

            CalculateSegments();
		}

		#endregion

		#region Public Methods

		public void Start()
        {
            timer = new System.Timers.Timer(50);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        public void Stop()
        {
            m_behindIsActive = true;
            Value = -1;
            timer.Stop();
            timer.Dispose();
		}

		public void Increment()
		{
			if (m_transitionSegment == 11)
			{
				m_transitionSegment = 0;
				m_behindIsActive = !m_behindIsActive;
			}
			else if (m_transitionSegment == -1)
			{
				m_transitionSegment = 0;
			}
			else
			{
				m_transitionSegment += 1;
			}
			Invalidate();
		}

		#endregion

		#region Overridden Methods

		protected override void Dispose(bool disposing)
        {
            m_activeBrush.Dispose();
            m_inactiveBrush.Dispose();
            m_transitionBrush.Dispose();
            base.Dispose(disposing);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.ExcludeClip(innerBackgroundRegion);
			for (int intCount = 0; intCount <= 11; intCount++)
			{
				if (this.Enabled)
				{
					if (intCount == m_transitionSegment)
					{
						//If this segment is the transistion segment, colour it differently
						e.Graphics.FillPath(m_transitionBrush, segmentPaths[intCount]);
					}
					else if (intCount < m_transitionSegment)
					{
						//This segment is behind the transistion segment
						if (m_behindIsActive)
						{
							//If behind the transistion should be active, 
							//colour it with the active colour
							e.Graphics.FillPath(m_activeBrush, segmentPaths[intCount]);
						}
						else
						{
							//If behind the transistion should be in-active, 
							//colour it with the in-active colour
							e.Graphics.FillPath(m_inactiveBrush, segmentPaths[intCount]);
						}
					}
					else
					{
						//This segment is ahead of the transistion segment
						if (m_behindIsActive)
						{
							//If behind the the transistion should be active, 
							//colour it with the in-active colour
							e.Graphics.FillPath(m_inactiveBrush, segmentPaths[intCount]);
						}
						else
						{
							//If behind the the transistion should be in-active, 
							//colour it with the active colour
							e.Graphics.FillPath(m_activeBrush, segmentPaths[intCount]);
						}
					}
				}
				else
				{
					//Draw all segments in in-active colour if not enabled
					e.Graphics.FillPath(m_inactiveBrush, segmentPaths[intCount]);
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
            Rectangle rctFull = new Rectangle(0, 0, this.Width, this.Height);
            RectangleF rctInner = new RectangleF((float)this.Width * (7F / 30F), (float)this.Height * (7F / 30F), (float)this.Width - ((float)this.Width * (7F / 30F) * 2), (float)this.Height - ((float)this.Height * (7F / 30F) * 2));
            GraphicsPath pthInnerBackground;
            //Create 12 segment pieces
            for (int intCount = 0; intCount <= 11; intCount++)
            {
                segmentPaths[intCount] = new GraphicsPath();
                //We subtract 90 so that the starting segment is at 12 o'clock
                segmentPaths[intCount].AddPie(rctFull, (intCount * 30) - 90, 25);
            }
            //Create the center circle cut-out
            pthInnerBackground = new GraphicsPath();
            pthInnerBackground.AddPie(rctInner.X, rctInner.Y, rctInner.Width, rctInner.Height, 0, 360);
            innerBackgroundRegion = new Region(pthInnerBackground);
		}

		#endregion
    }
}