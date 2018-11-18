using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using ThreePM.MusicLibrary;
using ThreePM.MusicPlayer;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Xml;
using System.Reflection;

namespace ThreePM
{
	public class BaseForm : Form
	{
		#region Declarations

		private bool loadedValues = false;

		private bool m_snapTo = true;
		private int m_internalPadding;
		private int m_roundedRadius = 5;
		private bool m_roundedForm;
		private bool m_internalBorderAtTop;
		private int m_moveOrigX;
		private int m_moveOrigY;
		private bool m_moving;
		private bool m_moveAll;
		private bool m_resizing;
		private string m_resizeDirection;
		private bool m_autoSaveTop = true;
		private bool m_autoSaveLeft = true;
		private bool m_sizable;

		private LinearGradientBrush m_activeBrush;
		private LinearGradientBrush m_inactiveBrush;

		private Rectangle m_displayRectangle;
		private Rectangle m_captionRectangle;
		private Rectangle m_closeRect;
		private Rectangle m_minimizeRect;
		private Rectangle m_moveWindowRect;
		private Image m_closeButton;
		private Image m_minimizeButton;
		private Image m_moveWindowButton;

		private Color m_captionTopColor = Color.DarkGray;
		private Color m_captionTextBackColor = Color.SlateGray;
		private Color m_captionBottomColor = Color.Black;
		private Color m_captionTextColor = Color.Silver;

		private Color m_captionTopColorInactive = Color.DarkGray;
		private Color m_captionTextBackColorInactive = Color.DimGray;
		private Color m_captionBottomColorInactive = Color.Black;
		private Color m_captionTextColorInactive = Color.Silver;

		private Color m_borderColor = Color.Black;

		private bool m_activated;
		private int m_captionheight = -1;
		private Player m_player;
		private MusicLibrary.Library m_library;
		private string m_caption;

		private const int SnapSize = 20;

		private static BackForm BackForm = new BackForm();
		private Timer tmrResizePause = new Timer();

		#region Win32 Stuff

		//private const int WM_NCLBUTTONDBLCLK = 0xA3;
		//private const int WM_NCLBUTTONDOWN = 0xA1;
		//private const int WM_NCRBUTTONDOWN = 0xA4;
		//private const int WM_NCHITTEST = 0x84;
		//private const int HT_CLIENT = 0x01;
		//private const int HT_CAPTION = 0x2;
		private const int HTLEFT = 10;
		private const int HTRIGHT = 11;
		private const int HTTOP = 12;
		private const int HTTOPLEFT = 13;
		private const int HTTOPRIGHT = 14;
		private const int HTBOTTOM = 15;
		private const int HTBOTTOMLEFT = 16;
		private const int HTBOTTOMRIGHT = 17;

		//[DllImport("user32.dll")]
		//static extern bool ReleaseCapture();
		//[DllImport("User32.dll")]
		//private static extern int SendMessage(IntPtr hWnd, int msg, int wParam,  int lParam);

		#endregion

		#endregion

		#region Properties

		protected virtual Control.ControlCollection DynamicControlsContainer
		{
			get { return this.Controls; }
		}

		public bool SnapTo
		{
			get { return m_snapTo; }
			set { m_snapTo = value; }
		}

		public bool Sizable
		{
			get { return m_sizable; }
			set { m_sizable = value; }
		}

		public bool InternalBorderAtTop
		{
			get { return m_internalBorderAtTop; }
			set { m_internalBorderAtTop = value; OnResize(EventArgs.Empty); }
		}

		public int InternalBorderSize
		{
			get { return m_internalPadding; }
			set { m_internalPadding = value; OnResize(EventArgs.Empty); }
		}

		public int RoundedRadius
		{
			get { return m_roundedRadius; }
			set { m_roundedRadius = value; OnResize(EventArgs.Empty); }
		}

		public bool RoundedForm
		{
			get { return m_roundedForm; }
			set { m_roundedForm = value; OnResize(EventArgs.Empty); }
		}

		public Color InternalBorderColor
		{
			get { return m_borderColor; }
			set { m_borderColor = value; OnResize(EventArgs.Empty); Invalidate(); }
		}

		public Color CaptionTopColorInactive
		{
			get { return m_captionTopColorInactive; }
			set { m_captionTopColorInactive = value; OnResize(EventArgs.Empty); }
		}
		public Color CaptionTextBackColorInactive
		{
			get { return m_captionTextBackColorInactive; }
			set { m_captionTextBackColorInactive = value; OnResize(EventArgs.Empty); }
		}
		public Color CaptionBottomColorInactive
		{
			get { return m_captionBottomColorInactive; }
			set { m_captionBottomColorInactive = value; OnResize(EventArgs.Empty); }
		}
		public Color CaptionTextColorInactive
		{
			get { return m_captionTextColorInactive; }
			set { m_captionTextColorInactive = value; OnResize(EventArgs.Empty); }
		}

		public Color CaptionTopColor
		{
			get { return m_captionTopColor; }
			set { m_captionTopColor = value; OnResize(EventArgs.Empty); }
		}
		public Color CaptionTextBackColor
		{
			get { return m_captionTextBackColor; }
			set { m_captionTextBackColor = value; OnResize(EventArgs.Empty); }
		}
		public Color CaptionBottomColor
		{
			get { return m_captionBottomColor; }
			set { m_captionBottomColor = value; OnResize(EventArgs.Empty); }
		}
		public Color CaptionTextColor
		{
			get { return m_captionTextColor; }
			set { m_captionTextColor = value; OnResize(EventArgs.Empty); }
		}

		public bool AutoSaveTop
		{
			get { return m_autoSaveTop; }
			set { m_autoSaveTop = value; }
		}

		public bool AutoSaveLeft
		{
			get { return m_autoSaveLeft; }
			set { m_autoSaveLeft = value; }
		}

		public string Caption
		{
			get
			{
				return m_caption;
			}
			set
			{
				m_caption = value;
				OnResize(EventArgs.Empty);
				Invalidate();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Player Player
		{
			get
			{
				return m_player;
			}
			set
			{
				m_player = value;
				InitPlayer();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Library Library
		{
			get
			{
				return m_library;
			}
			set
			{
				m_library = value;
				InitLibrary();
			}
		}

		#endregion

		#region Overridden Properties

		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
				m_captionheight = -1;
			}
		}

		public override System.Drawing.Rectangle DisplayRectangle
		{
			get
			{
				if (m_captionheight == -1)
				{
					CalculateCaptionHeight();
				}
				return m_displayRectangle;
			}
		}

		#endregion

		#region Overridden Methods

		protected override void OnClosing(CancelEventArgs e)
		{
			if (Player != null)
			{
				UnInitPlayer();
			}
			if (Library != null)
			{
				UnInitLibrary();
			}
			base.OnClosing(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			bool checkForMove = true;
			if (m_sizable && e.Button == MouseButtons.Left)
			{
				checkForMove = !DoResizeStuff(true, e.X, e.Y);
			}


			if (checkForMove && (m_captionheight == 0 || e.Y < m_captionheight) && e.Button == MouseButtons.Left)
			{
				if (m_closeRect.Contains(e.X, m_closeRect.Top))
				{
					Registry.SetValue(this.Name + ".Show", false);
					this.Close();
				}
				else if (MinimizeBox && m_minimizeRect.Contains(e.X, m_minimizeRect.Top))
				{
					this.WindowState = FormWindowState.Minimized;
				}
				else
				{
					m_moveOrigX = e.X;
					m_moveOrigY = e.Y;
					m_moving = true;
					m_moveAll = m_moveWindowRect.Contains(e.X, m_moveWindowRect.Top);
				}
				return;
			}
			else if (!checkForMove)
			{
				BackForm.MinimumSize = this.MinimumSize;
				BackForm.DesktopBounds = this.DesktopBounds;
				BackForm.BringToFront();
				BackForm.TopMost = true;
				BackForm.Show();
			}

			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{

			if (m_resizing)
			{
				int MouseY = MousePosition.Y;
				int MouseX = MousePosition.X;
				tmrResizePause.Stop();
				tmrResizePause.Start();

				int thisX = this.Location.X;
				int thisY = this.Location.Y;
				int thisWidth = this.Width;
				int thisHeight = this.Height;

				int BackFormX = thisX;
				int BackFormY = thisY;
				int BackFormWidth = thisWidth;
				int BackFormHeight = thisHeight;

				switch (m_resizeDirection)
				{
					case "N":
					{
						BackFormY = MouseY;
						BackFormHeight = (thisHeight + thisY) - BackFormY;
						break;
					}
					case "NW":
					{
						BackFormY = MouseY;
						BackFormHeight = (thisHeight + thisY) - BackFormY;
						BackFormX = MouseX;
						BackFormWidth = (thisWidth + thisX) - BackFormX;
						break;
					}
					case "W":
					{
						BackFormX = MouseX;
						BackFormWidth = (thisWidth + thisX) - BackFormX;
						break;
					}
					case "SW":
					{
						BackFormHeight = MouseY - thisY;
						BackFormX = MouseX;
						BackFormWidth = (thisWidth + thisX) - BackFormX;
						break;
					}
					case "S":
					{
						BackFormHeight = MouseY - thisY;
						break;
					}
					case "SE":
					{
						BackFormHeight = MouseY - thisY;
						BackFormWidth = MouseX - thisX;
						break;
					}
					case "E":
					{
						BackFormWidth = MouseX - thisX;
						break;
					}
					case "NE":
					{
						BackFormY = MouseY;
						BackFormHeight = (thisHeight + thisY) - BackFormY;
						BackFormWidth = MouseX - thisX;
						break;
					}
				}

				Rectangle eRect = new Rectangle(BackFormX, BackFormY, BackFormWidth, BackFormHeight);

				eRect = SnapBackForm(eRect);

				// This stops it from moving after it has resized too small
				if (eRect.Y > this.Bottom - this.MinimumSize.Height) eRect.Y = (this.Bottom - this.MinimumSize.Height);
				if (eRect.X > this.Right - this.MinimumSize.Width) eRect.X = (this.Right - this.MinimumSize.Width);

				//BackForm.DesktopBounds = eRect;

				// This makes sure that the BackForm doesn't flicker outside the BaseForm on the opposite edge
				if (BackForm.Width > eRect.Width)
				{
					BackForm.Width = eRect.Width;
					BackForm.Left = eRect.Left;
				}
				else
				{
					BackForm.Left = eRect.Left;
					BackForm.Width = eRect.Width;
				}
				if (BackForm.Height > eRect.Height)
				{
					BackForm.Height = eRect.Height;
					BackForm.Top = eRect.Top;
				}
				else
				{
					BackForm.Top = eRect.Top;
					BackForm.Height = eRect.Height;
				}

			}
			else if (m_moving)
			{
				int leftdiff = e.X - m_moveOrigX;
				int topdiff = e.Y - m_moveOrigY;

				if (m_moveAll)
				{
					// Get the EXTREME bounds of the forms
					Rectangle extreme = this.DesktopBounds;
					foreach (Form f in Application.OpenForms)
					{
						if (f.FormBorderStyle != FormBorderStyle.None || f.ControlBox)
						{
							extreme = Rectangle.Union(extreme, f.DesktopBounds);
						}
					}

					// Snap the EXTREME bounds of the forms to the window bounds
					Rectangle screenBounds = Screen.FromControl(this).WorkingArea;
					// need to take into account the mouse movement when you snap
					int mostLeft = extreme.Left + leftdiff;
					int mostTop = extreme.Top + topdiff;
					int originalMostLeft = mostLeft;
					int originalMostTop = mostTop;

					if (ModifierKeys != Keys.Control)
					{
						// need to have two separate if statements so that it can snap into the corners
						if (SnapToRectangleLeft(screenBounds, ref mostLeft, extreme.Width, this))
						{
							leftdiff += (mostLeft - originalMostLeft);
						}
						if (SnapToRectangleTop(screenBounds, ref mostTop, extreme.Height, this))
						{
							topdiff += (mostTop - originalMostTop);
						}
					}

					// Update the location of each form
					foreach (Form f in Application.OpenForms)
					{
						if (f.FormBorderStyle != FormBorderStyle.None || f.ControlBox)
						{
							f.SetDesktopLocation(f.Left + leftdiff, f.Top + topdiff);
						}
					}
				}
				else
				{
					int left = Location.X + leftdiff;
					int top = Location.Y + topdiff;

					if (ModifierKeys != Keys.Control)
					{
						// Check for snapping to other forms first
						bool hasSnappedLeft = false;
						bool hasSnappedTop = false;

						foreach (Form form in Application.OpenForms)
						{
							BaseForm f = form as BaseForm;
							if (f != null)
							{
								if (f != this && f.SnapTo)
								{
									if (!hasSnappedLeft && SnapToRectangleLeft(f.DesktopBounds, ref left, this.Width, this))
									{
										hasSnappedLeft = true;
									}

									if (!hasSnappedTop && SnapToRectangleTop(f.DesktopBounds, ref top, this.Height, this))
									{
										hasSnappedTop = true;
									}

									if (hasSnappedLeft && hasSnappedTop)
									{
										break;
									}
								}
							}
						}

						// Check for snapping to the screen now
						if (!hasSnappedLeft)
						{
							SnapToRectangleLeft(Screen.FromControl(this).WorkingArea, ref left, this.Width, this);
						}

						if (!hasSnappedTop)
						{
							SnapToRectangleTop(Screen.FromControl(this).WorkingArea, ref top, this.Height, this);
						}
					}

					SetDesktopLocation(left, top);
				}
			}
			else
			{
				if (m_sizable)
				{
					DoResizeStuff(false, e.X, e.Y);
				}
			}

			base.OnMouseMove(e);
		}

		private Rectangle SnapBackForm(Rectangle eRect)
		{
			if (ModifierKeys == Keys.Control) return eRect;
			foreach (Form form in Application.OpenForms)
			{
				BaseForm f = form as BaseForm;
				if (f != null && f != this && f.SnapTo)
				{
					int thisLeft = eRect.Left;
					int thisRight = eRect.Right;
					int thisTop = eRect.Top;
					int thisBottom = eRect.Bottom;

					//Snapping Left Edge
					if (SnapToRectangleLeft(f.DesktopBounds, ref thisLeft, 0, BackForm))
					{
						if (Math.Abs(eRect.Left - f.Left) < Math.Abs(eRect.Left - f.Right))
						{
							eRect = new Rectangle(f.Left, eRect.Top, eRect.Width + (eRect.Left - f.Left), eRect.Height);
						}
						else
						{
							eRect = new Rectangle(f.Right, eRect.Top, eRect.Width + (eRect.Left - f.Right), eRect.Height);
						}
					}
					//Snapping Right Edge
					if (SnapToRectangleLeft(f.DesktopBounds, ref thisRight, 0, BackForm))
					{
						if (Math.Abs(eRect.Right - f.Right) < Math.Abs(eRect.Right - f.Left))
						{
							eRect.Width = f.Right - eRect.Left;
						}
						else
						{
							eRect.Width = f.Left - eRect.Left;
						}
					}
					//Snapping Top Edge
					if (SnapToRectangleTop(f.DesktopBounds, ref thisTop, 0, BackForm))
					{
						if (Math.Abs(eRect.Top - f.Top) < Math.Abs(eRect.Top - f.Bottom))
						{
							eRect = new Rectangle(eRect.Left, f.Top, eRect.Width, eRect.Height + (eRect.Top - f.Top));
						}
						else
						{
							eRect = new Rectangle(eRect.Left, f.Bottom, eRect.Width, eRect.Height + (eRect.Top - f.Bottom));
						}
					}
					//Snapping Bottom Edge
					if (SnapToRectangleTop(f.DesktopBounds, ref thisBottom, 0, BackForm))
					{
						if (Math.Abs(eRect.Bottom - f.Bottom) < Math.Abs(eRect.Bottom - f.Top))
						{
							eRect.Height = f.Bottom - eRect.Top;
						}
						else
						{
							eRect.Height = f.Top - eRect.Top;
						}
					}
				}
			}
			return eRect;
		}

		protected override void OnActivated(EventArgs e)
		{
			m_activated = true;
			Invalidate();
			base.OnActivated(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			m_moving = false;

			if (m_resizing)
			{
				m_resizing = false;
				this.SetBounds(BackForm.Location.X, BackForm.Location.Y, BackForm.Width, BackForm.Height);
				BackForm.Hide();
				tmrResizePause.Stop();
			}
			base.OnMouseUp(e);
		}

		protected override void OnDeactivate(EventArgs e)
		{
			m_activated = false;
			Invalidate();
			base.OnDeactivate(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!this.DesignMode)
			{
				ReloadSkin(true);
				loadedValues = true;
				if (m_autoSaveLeft)
				{
					int newLeft = Registry.GetValue(this.Name + ".Left", this.Left);
					this.Left = this.Left < SystemInformation.VirtualScreen.Left ? 0 : newLeft;
				}
				if (m_autoSaveTop)
				{
					int newTop = Registry.GetValue(this.Name + ".Top", this.Top);
					this.Top = this.Top < SystemInformation.VirtualScreen.Top ? 0 : newTop;
				}
				if (m_sizable || FormBorderStyle == FormBorderStyle.Sizable) this.Width = Registry.GetValue(this.Name + ".Width", this.Width);
				if (m_sizable || FormBorderStyle == FormBorderStyle.Sizable) this.Height = Registry.GetValue(this.Name + ".Height", this.Height);
			}
			base.OnLoad(e);
		}

		protected override void Dispose(bool disposing)
		{
			tmrResizePause.Dispose();
			m_activeBrush.Dispose();
			m_inactiveBrush.Dispose();
			m_moveWindowButton.Dispose();
			m_minimizeButton.Dispose();
			m_closeButton.Dispose();
			if (!DesignMode && loadedValues)
			{
				if (m_autoSaveLeft) Registry.SetValue(this.Name + ".Left", Left);
				if (m_autoSaveTop) Registry.SetValue(this.Name + ".Top", Top);
				if (m_sizable || FormBorderStyle == FormBorderStyle.Sizable) Registry.SetValue(this.Name + ".Width", this.Width);
				if (m_sizable || FormBorderStyle == FormBorderStyle.Sizable) Registry.SetValue(this.Name + ".Height", this.Height);
			}
			base.Dispose(disposing);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			using (SolidBrush backBrush = new SolidBrush(m_borderColor))
			{
				e.Graphics.FillRectangle(backBrush, 0, 0, Width, Height);
			}

			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			if (m_captionheight == -1)
			{
				CalculateCaptionHeight();
			}

			LinearGradientBrush brush = (m_activated ? m_activeBrush : m_inactiveBrush);
			e.Graphics.FillRectangle(brush, m_captionRectangle);

			Rectangle captionRect = new Rectangle(m_captionRectangle.X - 1, m_captionRectangle.Y - 1, m_captionRectangle.Width, m_captionheight);

			TextRenderer.DrawText(e.Graphics, Caption, Font, captionRect, (m_activated ? m_captionTextColor : m_captionTextColorInactive), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

			if (FormBorderStyle != FormBorderStyle.None || ControlBox)
			{
				e.Graphics.DrawImage(m_moveWindowButton, m_moveWindowRect);
				e.Graphics.DrawImage(m_closeButton, m_closeRect);
			}

			if (MinimizeBox)
			{
				e.Graphics.DrawImage(m_minimizeButton, m_minimizeRect);
			}

			using (SolidBrush backBrush = new SolidBrush(BackColor))
			{
				e.Graphics.FillRectangle(backBrush, DisplayRectangle);
			}
		}

		protected override void OnResize(EventArgs e)
		{
			// Do caption stuff
			CalculateCaptionHeight();

			if (this.FormBorderStyle != FormBorderStyle.None || ControlBox)
			{
				m_moveWindowRect = new Rectangle(4, (m_captionheight / 2) - (m_moveWindowButton.Height / 2), m_moveWindowButton.Width, m_moveWindowButton.Height);
				m_closeRect = new Rectangle(ClientSize.Width - m_closeButton.Width - 4, (m_captionheight / 2) - (m_closeButton.Height / 2), m_closeButton.Width, m_closeButton.Height);
				m_moveWindowRect.Offset(m_captionRectangle.Location);
				m_closeRect.Offset(-m_captionRectangle.X, m_captionRectangle.Y);
			}
			else
			{
				m_moveWindowRect = Rectangle.Empty;
				m_closeRect = Rectangle.Empty;
			}
			m_minimizeRect = new Rectangle(m_closeRect.Left - m_minimizeButton.Width - 4, (m_captionheight / 2) - (m_minimizeButton.Height / 2), m_minimizeButton.Width, m_minimizeButton.Height);
			m_minimizeRect.Offset(-m_captionRectangle.X, m_captionRectangle.Y);

			if (RoundedForm)
			{
				using (GraphicsPath gp = GetRoundedRect())
				{
					Region = new Region(gp);
				}
			}
			else
			{
				Region = null;
			}

			base.OnResize(e);
		}

		private GraphicsPath GetRoundedRect()
		{
			float diameter = m_roundedRadius * 2.0F;
			SizeF sizeF = new SizeF(diameter, diameter);
			RectangleF arc = new RectangleF(new Point(-1, -1), sizeF);
			GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

			// top left arc 
			path.AddArc(arc, 180, 90);

			// top right arc 
			arc.X = ClientRectangle.Right - diameter;
			path.AddArc(arc, 270, 90);

			// bottom right arc 
			arc.Y = ClientRectangle.Bottom - diameter;
			path.AddArc(arc, 0, 90);

			// bottom left arc
			arc.X = -1;
			path.AddArc(arc, 90, 90);

			path.CloseFigure();

			return path;
		}

		private bool DoResizeStuff(bool doActualResize, int x, int y)
		{
			int cornerSize = 15;
			int position = 0;
			if (x <= InternalBorderSize)
			{
				if (y <= cornerSize)
				{
					position = HTTOPLEFT;
					m_resizeDirection = "NW";
				}
				else if (Height - y <= cornerSize)
				{
					position = HTBOTTOMLEFT;
					m_resizeDirection = "SW";
				}
				else
				{
					position = HTLEFT;
					m_resizeDirection = "W";
				}
			}
			else if (Width - x <= InternalBorderSize)
			{
				if (y <= cornerSize)
				{
					position = HTTOPRIGHT;
					m_resizeDirection = "NE";
				}
				else if (Height - y <= cornerSize)
				{
					position = HTBOTTOMRIGHT;
					m_resizeDirection = "SE";
				}
				else
				{
					position = HTRIGHT;
					m_resizeDirection = "E";
				}
			}
			else if (y <= 2)
			{
				if (x <= cornerSize)
				{
					position = HTTOPLEFT;
					m_resizeDirection = "NW";
				}
				else if (Width - x <= cornerSize)
				{
					position = HTTOPRIGHT;
					m_resizeDirection = "NE";
				}
				else
				{
					position = HTTOP;
					m_resizeDirection = "N";
				}
			}
			else if (Height - y <= InternalBorderSize)
			{
				if (x <= cornerSize)
				{
					position = HTBOTTOMLEFT;
					m_resizeDirection = "SW";
				}
				else if (Width - x <= cornerSize)
				{
					position = HTBOTTOMRIGHT;
					m_resizeDirection = "SE";
				}
				else
				{
					position = HTBOTTOM;
					m_resizeDirection = "S";
				}
			}

			switch (position)
			{
				case HTTOP:
				case HTBOTTOM:
				{
					Cursor.Current = Cursors.SizeNS;
					break;
				}
				case HTLEFT:
				case HTRIGHT:
				{
					Cursor.Current = Cursors.SizeWE;
					break;
				}
				case HTTOPLEFT:
				case HTBOTTOMRIGHT:
				{
					Cursor.Current = Cursors.SizeNWSE;
					break;
				}
				case HTTOPRIGHT:
				case HTBOTTOMLEFT:
				{
					Cursor.Current = Cursors.SizeNESW;
					break;
				}
				default:
				{
					Cursor.Current = Cursors.Default;
					break;
				}
			}

			if (doActualResize && position > 0)
			{
				//ReleaseCapture();
				//SendMessage(this.Handle, WM_NCLBUTTONDOWN, position, 0);

				m_resizing = true;
			}

			return (position != 0);
		}

		#endregion

		#region Constructor

		public BaseForm()
		{
			m_closeButton = Properties.Resources.CloseButton;
			m_minimizeButton = Properties.Resources.MinimizeButton;
			m_moveWindowButton = Properties.Resources.Window;

			m_caption = "ThreePM";
			this.DoubleBuffered = true;
			this.ResizeRedraw = true;
			this.Text = "";
			this.ControlBox = true;
			this.BackColor = Color.Black;
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
			this.tmrResizePause.Tick += new EventHandler(tmrResizePause_Tick);
			this.tmrResizePause.Interval = 500;
		}

		void tmrResizePause_Tick(object sender, EventArgs e)
		{
			if (m_resizing)
			{
				this.SetBounds(BackForm.Location.X, BackForm.Location.Y, BackForm.Width, BackForm.Height);
				tmrResizePause.Stop();
			}
		}

		#endregion

		#region Private Methods

		private static bool SnapToRectangleLeft(Rectangle rect, ref int left, int width, Form resizeTarget)
		{
			bool snapped = false;

			int right = left + width;

			if (resizeTarget.Top <= rect.Bottom && resizeTarget.Bottom >= rect.Top)
			{

				if (Math.Abs(right - rect.Left) < SnapSize)
				{
					left = rect.Left - width;
					snapped = true;
				}
				else if (Math.Abs(right - rect.Right) < SnapSize)
				{
					left = rect.Right - width;
					snapped = true;
				}
				else if (Math.Abs(left - rect.Left) < SnapSize)
				{
					left = rect.Left;
					snapped = true;
				}
				else if (Math.Abs(left - rect.Right) < SnapSize)
				{
					left = rect.Right;
					snapped = true;
				}

			}
			return snapped;
		}

		private static bool SnapToRectangleTop(Rectangle rect, ref int top, int height, Form resizeTarget)
		{
			bool snapped = false;

			int bottom = top + height;

			if (resizeTarget.Left <= rect.Right && resizeTarget.Right >= rect.Left)
			{

				if (Math.Abs(bottom - rect.Top) < SnapSize)
				{
					top = rect.Top - height;
					snapped = true;
				}
				else if (Math.Abs(bottom - rect.Bottom) < SnapSize)
				{
					top = rect.Bottom - height;
					snapped = true;
				}
				else if (Math.Abs(top - rect.Top) < SnapSize)
				{
					top = rect.Top;
					snapped = true;
				}
				else if (Math.Abs(top - rect.Bottom) < SnapSize)
				{
					top = rect.Bottom;
					snapped = true;
				}
			}

			return snapped;
		}

		private void CalculateCaptionHeight()
		{
			if (string.IsNullOrEmpty(Caption))
			{
				m_captionheight = 0;
				this.MinimumSize = new Size(100, 20);
			}
			else
			{
				Size s = TextRenderer.MeasureText(Caption, Font);
				m_captionheight = s.Height + 5;
				this.MinimumSize = new Size(100, m_captionheight);
			}


			m_displayRectangle = new Rectangle(m_internalPadding, m_captionheight + m_internalPadding, base.DisplayRectangle.Width - (m_internalPadding * 2) - 1, base.DisplayRectangle.Height - m_captionheight - (m_internalPadding * 2) - 1);
			m_captionRectangle = new Rectangle(-1, -1, Width + 1, m_captionheight + 1);
			if (m_internalPadding > 0)
			{
				if (m_internalBorderAtTop)
				{
					m_displayRectangle = new Rectangle(m_internalPadding, m_captionheight + m_internalPadding + 1, base.DisplayRectangle.Width - (m_internalPadding * 2) - 1, base.DisplayRectangle.Height - m_captionheight - (m_internalPadding * 2) - 2);
					m_captionRectangle = new Rectangle(0, 0, Width, m_captionheight);
				}
				else
				{
					m_displayRectangle = new Rectangle(m_internalPadding, m_captionheight + m_internalPadding - 1, base.DisplayRectangle.Width - (m_internalPadding * 2) - 1, base.DisplayRectangle.Height - m_captionheight - (m_internalPadding * 2));
					m_captionRectangle = new Rectangle(m_internalPadding, m_internalPadding, Width - (2 * m_internalPadding) - 1, m_captionheight);
				}
			}

			Rectangle brushRect = new Rectangle(m_captionRectangle.X - 1, m_captionRectangle.Y - 1, 10, m_captionRectangle.Height + 2);

			if (m_activeBrush != null) m_activeBrush.Dispose();
			if (m_inactiveBrush != null) m_inactiveBrush.Dispose();

			m_activeBrush = new LinearGradientBrush(brushRect, m_captionBottomColor, m_captionBottomColor, LinearGradientMode.Vertical);
			ColorBlend blend = new ColorBlend(4);
			blend.Colors[0] = m_captionTopColor;
			blend.Positions[0] = 0F;
			blend.Colors[1] = m_captionTextBackColor;
			blend.Positions[1] = 0.25F;
			blend.Colors[2] = m_captionTextBackColor;
			blend.Positions[2] = 0.75F;
			blend.Colors[3] = m_captionBottomColor;
			blend.Positions[3] = 1.0F;
			m_activeBrush.InterpolationColors = blend;

			m_inactiveBrush = new LinearGradientBrush(brushRect, m_captionBottomColor, m_captionBottomColor, LinearGradientMode.Vertical);
			blend = new ColorBlend(4);
			blend.Colors[0] = m_captionTopColorInactive;
			blend.Positions[0] = 0F;
			blend.Colors[1] = m_captionTextBackColorInactive;
			blend.Positions[1] = 0.25F;
			blend.Colors[2] = m_captionTextBackColorInactive;
			blend.Positions[2] = 0.75F;
			blend.Colors[3] = m_captionBottomColorInactive;
			blend.Positions[3] = 1.0F;
			m_inactiveBrush.InterpolationColors = blend;
		}

		#endregion

		#region Protected Methods

		protected virtual void InitLibrary()
		{
		}

		protected virtual void InitPlayer()
		{
		}

		protected virtual void UnInitLibrary()
		{
		}

		protected virtual void UnInitPlayer()
		{
		}

		#endregion

		#region Skinning Methods

		public void ReloadSkin()
		{
			ReloadSkin(false);
		}

		public virtual void ReloadSkin(bool loadBlankSkinFirst)
		{
			try
			{
				// load the skin from the xml file
				string xmlFile = Application.StartupPath;
				if (!xmlFile.EndsWith("\\"))
				{
					xmlFile += "\\";
				}
				xmlFile += "Skins\\";
				xmlFile += Registry.GetValue("BaseForm.Skin", "_no_file_found_");

				XmlDocument doc = new XmlDocument();
				doc.LoadXml(Properties.Resources.Blank_skin);
				ApplySkin(doc);

				if (System.IO.File.Exists(xmlFile))
				{
					doc.Load(xmlFile);
				}
				else
				{
					doc.LoadXml(Properties.Resources.Default_skin);
				}
				ApplySkin(doc);
			}
			catch { }
		}

		private void ApplySkin(XmlDocument doc)
		{
			// first, apply the form type="All" properties
			ApplySkin(doc, "/skin/form[@type='All']", this);

			// now the specific forms properties
			ApplySkin(doc, "/skin/form[@type='" + this.GetType().Name + "']", this);

			RemoveNewControls();

			foreach (Control c in ControlsRecursive(this.Controls))
			{
				// generic control properties
				ApplySkin(doc, "/skin/control[@type='All']", c);

				// control properties by type
				ApplySkin(doc, "/skin/control[@type='" + c.GetType().Name + "']", c);

				// control properties by name
				ApplySkin(doc, "/skin/control[@name='" + c.Name + "']", c);
			}

			foreach (Control c in ControlsRecursive(this.Controls))
			{
				// generic control properties
				ApplySkin(doc, "/skin/form[@type='" + this.GetType().Name + "']/control[@type='All']", c);

				// control properties by type
				ApplySkin(doc, "/skin/form[@type='" + this.GetType().Name + "']/control[@type='" + c.GetType().Name + "']", c);

				// control properties by name
				ApplySkin(doc, "/skin/form[@type='" + this.GetType().Name + "']/control[@name='" + c.Name + "']", c);
			}

			AddNewControls(doc);
		}

		private void AddNewControls(XmlDocument doc)
		{
			XmlNodeList nodes = doc.SelectNodes("/skin/form[@type='" + this.GetType().Name + "']/addcontrol");
			foreach (XmlNode node in nodes)
			{
				string name = node.Attributes["name"].Value;
				string type = node.Attributes["type"].Value;
				Control control = null;
				switch (type.ToLower())
				{
					case "picture":
					{
						control = new PictureBox();
						break;
					}
					case "label":
					{
						control = new Label();
						break;
					}
					case "albumartbox":
					{
						control = new AlbumArtBox();
						break;
					}
				}
				if (control != null)
				{
					control.Name = name;
					control.Tag = "addcontrol";
					DynamicControlsContainer.Add(control);
					// apply properties as normal
					ApplySkin(node, ".", control);

					if (node.Attributes["bringtofront"] != null)
					{
						control.BringToFront();
					}
				}
			}
		}

		private void RemoveNewControls()
		{
			for (int i = DynamicControlsContainer.Count - 1; i >= 0; i--)
			{
				Control c = DynamicControlsContainer[i];
				if (c.Tag != null && c.Tag.ToString().Equals("addcontrol"))
				{
					DynamicControlsContainer.Remove(c);
				}
			}
		}

		private IEnumerable<Control> ControlsRecursive(System.Collections.IList baseControl)
		{
			foreach (Control c in baseControl)
			{
				yield return c;
				foreach (Control c2 in ControlsRecursive(c.Controls))
				{
					yield return c2;
				}
			}
		}

		protected void ApplySkin(XmlNode doc, string query, Control control)
		{
			XmlNodeList nodes = doc.SelectNodes(query);
			foreach (XmlNode node in nodes)
			{
				ApplyProperties(control, node);
			}
		}

		private void ApplyProperties(Object control, XmlNode node)
		{
			XmlNodeList props = node.SelectNodes("./property");
			foreach (XmlNode prop in props)
			{
				string propName = prop.Attributes["name"].Value;
				string propType = prop.Attributes["type"].Value;
				if (propName.ToLower() == "zorder")
				{
					int zorder = (int)new Int32Converter().ConvertFromString(prop.Attributes["value"].Value);
					(control as Control).Parent.Controls.SetChildIndex(control as Control, zorder);
				}
				else
				{
					PropertyInfo propInfo = control.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase, null, null, new Type[0], null);
					if (propInfo != null)
					{
						object value = null;
						switch (propType.ToLower())
						{
							case "binding":
							{
								Control cont = control as Control;
								if (cont != null)
								{
									cont.DataBindings.Add(propName, this, prop.Attributes["value"].Value);
								}
								break;
							}
							case "colour":
							case "color":
							{
								value = new ColorConverter().ConvertFromInvariantString(prop.Attributes["value"].Value);
								break;
							}
							case "font":
							{
								value = new FontConverter().ConvertFromString(prop.Attributes["value"].Value);
								break;
							}
							case "string":
							{
								value = new StringConverter().ConvertFromString(prop.Attributes["value"].Value);
								break;
							}
							case "boolean":
							case "bool":
							{
								value = new BooleanConverter().ConvertFromString(prop.Attributes["value"].Value);
								break;
							}
							case "location":
							case "point":
							{
								Point p = (Point)new PointConverter().ConvertFromString(prop.Attributes["value"].Value);
								if (p.X < 0) p.X = DisplayRectangle.Right + p.X;
								if (p.Y < 0) p.Y = DisplayRectangle.Bottom + p.Y;
								value = p;
								break;

							}
							case "size":
							{
								Size s = (Size)new SizeConverter().ConvertFromString(prop.Attributes["value"].Value);
								if (s.Width < 0) s.Width = DisplayRectangle.Width + s.Width;
								if (s.Height < 0) s.Height = DisplayRectangle.Height + s.Height;
								value = s;
								break;
							}
							case "integer":
							case "int32":
							case "int":
							{
								value = new Int32Converter().ConvertFromString(prop.Attributes["value"].Value);
								break;
							}
							case "rect":
							case "rectangle":
							{
								value = new RectangleConverter().ConvertFromString(prop.Attributes["value"].Value);
								break;
							}
							case "margin":
							case "padding":
							{
								value = new PaddingConverter().ConvertFromString(prop.Attributes["value"].Value);
								break;
							}
							case "dock":
							{
								value = new EnumConverter(typeof(DockStyle)).ConvertFromString(prop.Attributes["value"].Value);
								break;
							}
							case "anchor":
							{
								value = new EnumConverter(typeof(AnchorStyles)).ConvertFromString(prop.Attributes["value"].Value);
								break;
							}
							case "pictureboxsizemode":
							case "sizemode":
							{
								value = new EnumConverter(typeof(PictureBoxSizeMode)).ConvertFromString(prop.Attributes["value"].Value);
								break;
							}
							case "contentalignment":
							case "textalign":
							case "align":
							case "alignment":
							{
								value = new EnumConverter(typeof(ContentAlignment)).ConvertFromString(prop.Attributes["value"].Value);
								break;
							}
							case "image":
							{
								string filename = prop.Attributes["value"].Value;
								filename = filename.Replace("{skindir}", System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "Skins"));
								value = Image.FromFile(filename);
								break;
							}
							case "object":
							{
								// that means this property has properties of its own, so....
								object propValue = propInfo.GetValue(control, null);
								if (propValue != null)
								{
									ApplyProperties(propValue, prop);
								}
								break;
							}
						}
						if (value != null)
						{
							propInfo.SetValue(control, value, null);
						}
					}
				}
			}
		}

		#endregion
	}
}
