using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using starH45.net.mp3.player;

namespace starH45.net.mp3
{
	/// <summary>
	/// NotifyForm allows to display MSN style/Skinned instant messaging popups
	/// </summary>
	/// <remarks>
	/// Adapted from http://www.codeproject.com/cs/miscctrl/taskbarnotifier.asp.
	/// 
	/// Original comments:
	/// C# NotifyForm Class v1.0
	/// by John O'Byrne - 02 december 2002
	/// 01 april 2003 : Small fix in the OnMouseUp handler
	/// 11 january 2003 : Patrick Vanden Driessche <pvdd@devbrains.be> added a few enhancements
	///           Small Enhancements/Bugfix
	///           Small bugfix: When Content text measures larger than the corresponding ContentRectangle
	///                         the focus rectangle was not correctly drawn. This has been solved.
	///           Added KeepVisibleOnMouseOver
	///           Added ReShowOnMouseOver
	///           Added If the Title or Content are too long to fit in the corresponding Rectangles,
	///                 the text is truncateed and the ellipses are appended (StringTrimming).
	/// </remarks>
	public partial class NotifyForm : BaseForm
	{
		#region Enums

		/// <summary>
		/// List of the different popup animation status
		/// </summary>
		public enum TaskbarStates
		{
			Hidden = 0,
			Appearing = 1,
			Visible = 2,
			Disappearing = 3,
		};

		#endregion Enums

		#region Events

		[field: NonSerialized()]
		public event EventHandler NextSong;

		[field: NonSerialized()]
		public event EventHandler ShowPlayer;

		#endregion Events

		#region Declarations

		private Timer m_timer = new Timer();

		private TaskbarStates m_taskbarState = TaskbarStates.Hidden;

		private int m_originalHeight = 0;
		private int m_originalTop = 0;
		private int m_hiddenTop = 0;
		private bool m_isMouseDown = false;

		private int m_showTime = 500;
		private int m_stayTime = 3000;
		private int m_hideTime = 500;

		private int m_showEventsInterval = 0;
		private int m_showPixelIncrement = 0;
		private int m_hideEventsInterval = 0;
		private int m_hidePixelIncrement = 0;

		private bool m_keepVisibleOnMouseOver = true;
		private bool m_reShowOnMouseOver = true;

		private bool m_isMouseOverPopup = false;

		#endregion Declarations

		#region Properties

		/// <summary>
		/// Get the current TaskbarState (hidden, showing, visible, hiding)
		/// </summary>
		public TaskbarStates TaskbarState
		{
			get { return m_taskbarState; }
			private set { m_taskbarState = value; }
		}

		public int ShowTime
		{
			get { return m_showTime; }
			set { m_showTime = value; }
		}

		public int StayTime
		{
			get { return m_stayTime; }
			set { m_stayTime = value; }
		}

		public int HideTime
		{
			get { return m_hideTime; }
			set { m_hideTime = value; }
		}

		#endregion Properties

		#region Constructor and Load Methods

		/// <summary>
		/// Initializes a new NotifyForm.
		/// </summary>
		public NotifyForm()
		{
			InitializeComponent();

			m_originalHeight = this.Height;

			this.WindowState = FormWindowState.Minimized;
			base.Show();
			base.Hide();
			this.WindowState = FormWindowState.Normal;

			m_timer.Enabled = true;
			m_timer.Tick += new EventHandler(Timer_Tick);
		}

		protected override void OnLoad(EventArgs e)
		{
			// Load the position and size in the base form
			base.OnLoad(e);

			// Store the original top position for saving to the registry
			m_originalTop = this.Top;
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			m_timer.Enabled = false;
			m_timer.Dispose();
			m_timer = null;
			base.OnClosing(e);
		}

		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);

			if (m_isMouseDown)
			{
				// The user moved the form, so store the new top position for saving to the registry
				m_originalTop = this.Top;
			}
		}

		#endregion Constructor and Load Methods

		#region Show Methods

		[DllImport("user32.dll")]
		private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

		/// <summary>
		/// Displays the popup for a certain amount of time
		/// </summary>
		public void Show(SongInfo song)
		{
			// Set the display controls
			lblArtist.Text = song.Artist;
			lblAlbum.Text = song.Album;
			lblTitle.Text = song.Title;
			pctAlbum.Song = song;

			// We calculate the pixel increment and the m_timer value for the showing animation
			int numberOfEvents;
			if (this.ShowTime > 10)
			{
				numberOfEvents = Math.Min((this.ShowTime / 10), m_originalHeight);
				m_showEventsInterval = this.ShowTime / numberOfEvents;
				m_showPixelIncrement = m_originalHeight / numberOfEvents;
			}
			else
			{
				m_showEventsInterval = 10;
				m_showPixelIncrement = this.Height;
			}

			// We calculate the pixel increment and the m_timer value for the hiding animation
			if (this.HideTime > 10)
			{
				numberOfEvents = Math.Min((this.HideTime / 10), m_originalHeight);
				m_hideEventsInterval = this.HideTime / numberOfEvents;
				m_hidePixelIncrement = m_originalHeight / numberOfEvents;
			}
			else
			{
				m_hideEventsInterval = 10;
				m_hidePixelIncrement = m_originalHeight;
			}

			switch (this.TaskbarState)
			{
				case TaskbarStates.Hidden:
				{
					this.TaskbarState = TaskbarStates.Appearing;

					// If this form has already been hidden, set it back to the hidden position
					if (m_hiddenTop != 0)
					{
						this.Top = m_hiddenTop;
					}

					m_timer.Interval = m_showEventsInterval;
					m_timer.Start();

					// We show the popup without stealing focus
					ShowWindow(this.Handle, 4);
					break;
				}
				case TaskbarStates.Appearing:
				{
					Refresh();
					break;
				}
				case TaskbarStates.Visible:
				{
					m_timer.Stop();
					m_timer.Interval = this.StayTime;
					m_timer.Start();
					Refresh();
					break;
				}
				case TaskbarStates.Disappearing:
				{
					m_timer.Stop();
					this.TaskbarState = TaskbarStates.Visible;
					m_timer.Interval = this.StayTime;
					m_timer.Start();
					Refresh();
					break;
				}
			}
		}

		/// <summary>
		/// Hides the popup
		/// </summary>
		public new void Hide()
		{
			if (this.TaskbarState != TaskbarStates.Hidden)
			{
				m_timer.Stop();
				this.TaskbarState = TaskbarStates.Hidden;
				base.Hide();
			}
		}

		#endregion Public Methods

		#region Event Handlers and Overrides
		
		protected void Timer_Tick(object sender, EventArgs e)
		{
			switch (this.TaskbarState)
			{
				case TaskbarStates.Appearing:
				{
					if (this.Height < m_originalHeight)
					{
						SetBounds(this.Left, this.Top - m_showPixelIncrement, this.Width, this.Height + m_showPixelIncrement);
					}
					else
					{
						m_timer.Stop();
						this.Height = m_originalHeight;
						m_timer.Interval = this.StayTime;
						this.TaskbarState = TaskbarStates.Visible;
						m_timer.Start();
					}
					break;
				}
				case TaskbarStates.Visible:
				{
					m_timer.Stop();
					m_timer.Interval = m_hideEventsInterval;
					if ((m_keepVisibleOnMouseOver && !m_isMouseOverPopup) || (!m_keepVisibleOnMouseOver))
					{
						this.TaskbarState = TaskbarStates.Disappearing;
					}
					m_timer.Start();
					break;
				}
				case TaskbarStates.Disappearing:
				{
					if (m_reShowOnMouseOver && m_isMouseOverPopup)
					{
						this.TaskbarState = TaskbarStates.Appearing;
					}
					else
					{
						Rectangle workAreaRectangle = Screen.GetWorkingArea(this);
						if (this.Top < workAreaRectangle.Bottom)
						{
							SetBounds(this.Left, this.Top + m_hidePixelIncrement, this.Width, this.Height - m_hidePixelIncrement);
						}
						else
						{
							Hide();

							// Store the hidden top position to restore before the form needs to be shown
							m_hiddenTop = this.Top;

							// Set the top position to the value that should be saved to the registry
							this.Top = m_originalTop;
						}
					}
					break;
				}
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			m_isMouseOverPopup = true;
			Refresh();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			m_isMouseOverPopup = false;
			Refresh();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			m_isMouseDown = true;
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			m_isMouseDown = false;
			base.OnMouseUp(e);
		}

		private void lblNext_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OnNextSong();
		}

		private void lblShowPlayer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OnShowPlayer();
		}

		#endregion Event Handlers and Overrides

		#region Event Methods

		private void OnNextSong()
		{
			EventHandler e = NextSong;
			if (e != null)
			{
				e(this, EventArgs.Empty);
			}
		}

		private void OnShowPlayer()
		{
			EventHandler e = ShowPlayer;
			if (e != null)
			{
				e(this, EventArgs.Empty);
			}
		}

		#endregion Event Methods
	}
}
