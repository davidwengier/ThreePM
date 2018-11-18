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
		System.ComponentModel.ComponentResourceManager resources;
		private Image m_bullet;
		private bool m_fireWhileSliding = true;
		private bool ignoreSet;
		private double position = 50F;
		private double duration = 100F;

        public event EventHandler PositionChanging;
		public event EventHandler PositionChanged;

		public bool FireWhileSliding
		{
			get
			{
				return m_fireWhileSliding;
			}
			set
			{
				m_fireWhileSliding = value;
			}
		}

		[Browsable(false)]
		public double Duration
		{
			get
			{
				return duration;
			}
			set
			{
				if (position > value)
				{
					position = 0;
				}
				duration = value;
			}
		}
		
		[Browsable(false)]
		public double Position
		{
			get
			{
				return position;
			}
			set
			{
				if (ignoreSet) return;
				position = Math.Max(0, value);
				position = Math.Min(value, duration);
				Invalidate();
			}
		}

		public void SetPosition(double position)
		{
			if (this.position != position)
			{
				Position = position;
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
				if (m_bullet != null)
				{
					m_bullet.Dispose();
				}
				object o = resources.GetObject(value.Name);
				if (o == null)
				{
					o = resources.GetObject("Gray");
				}
				m_bullet = (Image)o;
			}
		}

		public Ticker()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

			resources = new System.ComponentModel.ComponentResourceManager(typeof(Ticker));
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			if (m_bullet != null)
			{
				base.SetBoundsCore(x, y, width, m_bullet.Height, specified);
			}
		}
	
		protected override void OnPaint(PaintEventArgs e)
		{
			using (Pen p = new Pen(ForeColor))
			{
				e.Graphics.DrawLine(p, 0, Height / 2, Width, Height / 2);
				e.Graphics.DrawLine(p, 0, Height / 2, Width, Height / 2);
				if (duration > 0)
				{
					int i = Convert.ToInt32((position / duration) * Width);
					e.Graphics.DrawImage(m_bullet, i - (m_bullet.Width / 2), 0, m_bullet.Width, m_bullet.Height);
				}
			}

			base.OnPaint(e);
		}
		
		private void SetPosition(int percent, bool raiseEvent)
		{
			position = ((float)percent / (float)Width) * duration;
            position = Math.Max(0, position);
            position = Math.Min(position, duration);
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
			ignoreSet = true;
			SetPosition(e.X, m_fireWhileSliding);
			Invalidate();
			base.OnMouseDown(e);
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (ignoreSet)
			{
				SetPosition(e.X, m_fireWhileSliding);
				Invalidate();
			}
			base.OnMouseMove(e);
		}
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			SetPosition(e.X, true);
			
			ignoreSet = false;
			Invalidate();
			base.OnMouseUp(e);
		}
	}
}
