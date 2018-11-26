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

        private bool _loadedValues = false;

        private bool _snapTo = true;
        private int _internalPadding;
        private int _roundedRadius = 5;
        private bool _roundedForm;
        private bool _internalBorderAtTop;
        private int _moveOrigX;
        private int _moveOrigY;
        private bool _moving;
        private bool _moveAll;
        private bool _resizing;
        private string _resizeDirection;
        private bool _autoSaveTop = true;
        private bool _autoSaveLeft = true;
        private bool _sizable;

        private LinearGradientBrush _activeBrush;
        private LinearGradientBrush _inactiveBrush;

        private Rectangle _displayRectangle;
        private Rectangle _captionRectangle;
        private Rectangle _closeRect;
        private Rectangle _minimizeRect;
        private Rectangle _moveWindowRect;
        private readonly Image _closeButton;
        private readonly Image _minimizeButton;
        private readonly Image _moveWindowButton;

        private Color _captionTopColor = Color.DarkGray;
        private Color _captionTextBackColor = Color.SlateGray;
        private Color _captionBottomColor = Color.Black;
        private Color _captionTextColor = Color.Silver;

        private Color _captionTopColorInactive = Color.DarkGray;
        private Color _captionTextBackColorInactive = Color.DimGray;
        private Color _captionBottomColorInactive = Color.Black;
        private Color _captionTextColorInactive = Color.Silver;

        private Color _borderColor = Color.Black;

        private bool _activated;
        private int _captionheight = -1;
        private Player _player;
        private Library _library;
        private string _caption;

        private const int SnapSize = 20;

        private static readonly BackForm s_backForm = new BackForm();
        private readonly Timer _resizePauseTimer = new Timer();

        #region Win32 Stuff

        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;

        #endregion

        #endregion

        #region Properties

        protected virtual Control.ControlCollection DynamicControlsContainer
        {
            get { return this.Controls; }
        }

        public bool SnapTo
        {
            get { return _snapTo; }
            set { _snapTo = value; }
        }

        public bool Sizable
        {
            get { return _sizable; }
            set { _sizable = value; }
        }

        public bool InternalBorderAtTop
        {
            get { return _internalBorderAtTop; }
            set { _internalBorderAtTop = value; OnResize(EventArgs.Empty); }
        }

        public int InternalBorderSize
        {
            get { return _internalPadding; }
            set { _internalPadding = value; OnResize(EventArgs.Empty); }
        }

        public int RoundedRadius
        {
            get { return _roundedRadius; }
            set { _roundedRadius = value; OnResize(EventArgs.Empty); }
        }

        public bool RoundedForm
        {
            get { return _roundedForm; }
            set { _roundedForm = value; OnResize(EventArgs.Empty); }
        }

        public Color InternalBorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; OnResize(EventArgs.Empty); Invalidate(); }
        }

        public Color CaptionTopColorInactive
        {
            get { return _captionTopColorInactive; }
            set { _captionTopColorInactive = value; OnResize(EventArgs.Empty); }
        }
        public Color CaptionTextBackColorInactive
        {
            get { return _captionTextBackColorInactive; }
            set { _captionTextBackColorInactive = value; OnResize(EventArgs.Empty); }
        }
        public Color CaptionBottomColorInactive
        {
            get { return _captionBottomColorInactive; }
            set { _captionBottomColorInactive = value; OnResize(EventArgs.Empty); }
        }
        public Color CaptionTextColorInactive
        {
            get { return _captionTextColorInactive; }
            set { _captionTextColorInactive = value; OnResize(EventArgs.Empty); }
        }

        public Color CaptionTopColor
        {
            get { return _captionTopColor; }
            set { _captionTopColor = value; OnResize(EventArgs.Empty); }
        }
        public Color CaptionTextBackColor
        {
            get { return _captionTextBackColor; }
            set { _captionTextBackColor = value; OnResize(EventArgs.Empty); }
        }
        public Color CaptionBottomColor
        {
            get { return _captionBottomColor; }
            set { _captionBottomColor = value; OnResize(EventArgs.Empty); }
        }
        public Color CaptionTextColor
        {
            get { return _captionTextColor; }
            set { _captionTextColor = value; OnResize(EventArgs.Empty); }
        }

        public bool AutoSaveTop
        {
            get { return _autoSaveTop; }
            set { _autoSaveTop = value; }
        }

        public bool AutoSaveLeft
        {
            get { return _autoSaveLeft; }
            set { _autoSaveLeft = value; }
        }

        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
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
                return _player;
            }
            set
            {
                _player = value;
                InitPlayer();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Library Library
        {
            get
            {
                return _library;
            }
            set
            {
                _library = value;
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
                _captionheight = -1;
            }
        }

        public override System.Drawing.Rectangle DisplayRectangle
        {
            get
            {
                if (_captionheight == -1)
                {
                    CalculateCaptionHeight();
                }
                return _displayRectangle;
            }
        }

        #endregion

        #region Overridden Methods

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.Player != null)
            {
                UnInitPlayer();
            }
            if (this.Library != null)
            {
                UnInitLibrary();
            }
            base.OnClosing(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            bool checkForMove = true;
            if (_sizable && e.Button == MouseButtons.Left)
            {
                checkForMove = !DoResizeStuff(true, e.X, e.Y);
            }


            if (checkForMove && (_captionheight == 0 || e.Y < _captionheight) && e.Button == MouseButtons.Left)
            {
                if (_closeRect.Contains(e.X, _closeRect.Top))
                {
                    Registry.SetValue(this.Name + ".Show", false);
                    this.Close();
                }
                else if (this.MinimizeBox && _minimizeRect.Contains(e.X, _minimizeRect.Top))
                {
                    this.WindowState = FormWindowState.Minimized;
                }
                else
                {
                    _moveOrigX = e.X;
                    _moveOrigY = e.Y;
                    _moving = true;
                    _moveAll = _moveWindowRect.Contains(e.X, _moveWindowRect.Top);
                }
                return;
            }
            else if (!checkForMove)
            {
                s_backForm.MinimumSize = this.MinimumSize;
                s_backForm.DesktopBounds = this.DesktopBounds;
                s_backForm.BringToFront();
                s_backForm.TopMost = true;
                s_backForm.Show();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {

            if (_resizing)
            {
                int MouseY = MousePosition.Y;
                int MouseX = MousePosition.X;
                _resizePauseTimer.Stop();
                _resizePauseTimer.Start();

                int thisX = this.Location.X;
                int thisY = this.Location.Y;
                int thisWidth = this.Width;
                int thisHeight = this.Height;

                int BackFormX = thisX;
                int BackFormY = thisY;
                int BackFormWidth = thisWidth;
                int BackFormHeight = thisHeight;

                switch (_resizeDirection)
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
                if (s_backForm.Width > eRect.Width)
                {
                    s_backForm.Width = eRect.Width;
                    s_backForm.Left = eRect.Left;
                }
                else
                {
                    s_backForm.Left = eRect.Left;
                    s_backForm.Width = eRect.Width;
                }
                if (s_backForm.Height > eRect.Height)
                {
                    s_backForm.Height = eRect.Height;
                    s_backForm.Top = eRect.Top;
                }
                else
                {
                    s_backForm.Top = eRect.Top;
                    s_backForm.Height = eRect.Height;
                }

            }
            else if (_moving)
            {
                int leftdiff = e.X - _moveOrigX;
                int topdiff = e.Y - _moveOrigY;

                if (_moveAll)
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
                    int left = this.Location.X + leftdiff;
                    int top = this.Location.Y + topdiff;

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
                if (_sizable)
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
                    if (SnapToRectangleLeft(f.DesktopBounds, ref thisLeft, 0, s_backForm))
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
                    if (SnapToRectangleLeft(f.DesktopBounds, ref thisRight, 0, s_backForm))
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
                    if (SnapToRectangleTop(f.DesktopBounds, ref thisTop, 0, s_backForm))
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
                    if (SnapToRectangleTop(f.DesktopBounds, ref thisBottom, 0, s_backForm))
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
            _activated = true;
            Invalidate();
            base.OnActivated(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _moving = false;

            if (_resizing)
            {
                _resizing = false;
                this.SetBounds(s_backForm.Location.X, s_backForm.Location.Y, s_backForm.Width, s_backForm.Height);
                s_backForm.Hide();
                _resizePauseTimer.Stop();
            }
            base.OnMouseUp(e);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            _activated = false;
            Invalidate();
            base.OnDeactivate(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                ReloadSkin(true);
                _loadedValues = true;
                if (_autoSaveLeft)
                {
                    int newLeft = Registry.GetValue(this.Name + ".Left", this.Left);
                    this.Left = this.Left < SystemInformation.VirtualScreen.Left ? 0 : newLeft;
                }
                if (_autoSaveTop)
                {
                    int newTop = Registry.GetValue(this.Name + ".Top", this.Top);
                    this.Top = this.Top < SystemInformation.VirtualScreen.Top ? 0 : newTop;
                }
                if (_sizable || this.FormBorderStyle == FormBorderStyle.Sizable) this.Width = Registry.GetValue(this.Name + ".Width", this.Width);
                if (_sizable || this.FormBorderStyle == FormBorderStyle.Sizable) this.Height = Registry.GetValue(this.Name + ".Height", this.Height);
            }
            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            _resizePauseTimer.Dispose();
            _activeBrush.Dispose();
            _inactiveBrush.Dispose();
            _moveWindowButton.Dispose();
            _minimizeButton.Dispose();
            _closeButton.Dispose();
            if (!this.DesignMode && _loadedValues)
            {
                if (_autoSaveLeft) Registry.SetValue(this.Name + ".Left", this.Left);
                if (_autoSaveTop) Registry.SetValue(this.Name + ".Top", this.Top);
                if (_sizable || this.FormBorderStyle == FormBorderStyle.Sizable) Registry.SetValue(this.Name + ".Width", this.Width);
                if (_sizable || this.FormBorderStyle == FormBorderStyle.Sizable) Registry.SetValue(this.Name + ".Height", this.Height);
            }
            base.Dispose(disposing);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (SolidBrush backBrush = new SolidBrush(_borderColor))
            {
                e.Graphics.FillRectangle(backBrush, 0, 0, this.Width, this.Height);
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            if (_captionheight == -1)
            {
                CalculateCaptionHeight();
            }

            LinearGradientBrush brush = (_activated ? _activeBrush : _inactiveBrush);
            e.Graphics.FillRectangle(brush, _captionRectangle);

            Rectangle captionRect = new Rectangle(_captionRectangle.X - 1, _captionRectangle.Y - 1, _captionRectangle.Width, _captionheight);

            TextRenderer.DrawText(e.Graphics, this.Caption, this.Font, captionRect, (_activated ? _captionTextColor : _captionTextColorInactive), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            if (this.FormBorderStyle != FormBorderStyle.None || this.ControlBox)
            {
                e.Graphics.DrawImage(_moveWindowButton, _moveWindowRect);
                e.Graphics.DrawImage(_closeButton, _closeRect);
            }

            if (this.MinimizeBox)
            {
                e.Graphics.DrawImage(_minimizeButton, _minimizeRect);
            }

            using (SolidBrush backBrush = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(backBrush, this.DisplayRectangle);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            // Do caption stuff
            CalculateCaptionHeight();

            if (this.FormBorderStyle != FormBorderStyle.None || this.ControlBox)
            {
                _moveWindowRect = new Rectangle(4, (_captionheight / 2) - (_moveWindowButton.Height / 2), _moveWindowButton.Width, _moveWindowButton.Height);
                _closeRect = new Rectangle(this.ClientSize.Width - _closeButton.Width - 4, (_captionheight / 2) - (_closeButton.Height / 2), _closeButton.Width, _closeButton.Height);
                _moveWindowRect.Offset(_captionRectangle.Location);
                _closeRect.Offset(-_captionRectangle.X, _captionRectangle.Y);
            }
            else
            {
                _moveWindowRect = Rectangle.Empty;
                _closeRect = Rectangle.Empty;
            }
            _minimizeRect = new Rectangle(_closeRect.Left - _minimizeButton.Width - 4, (_captionheight / 2) - (_minimizeButton.Height / 2), _minimizeButton.Width, _minimizeButton.Height);
            _minimizeRect.Offset(-_captionRectangle.X, _captionRectangle.Y);

            if (this.RoundedForm)
            {
                using (GraphicsPath gp = GetRoundedRect())
                {
                    this.Region = new Region(gp);
                }
            }
            else
            {
                this.Region = null;
            }

            base.OnResize(e);
        }

        private GraphicsPath GetRoundedRect()
        {
            float diameter = _roundedRadius * 2.0F;
            SizeF sizeF = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(new Point(-1, -1), sizeF);
            GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            // top left arc 
            path.AddArc(arc, 180, 90);

            // top right arc 
            arc.X = this.ClientRectangle.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc 
            arc.Y = this.ClientRectangle.Bottom - diameter;
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
            if (x <= this.InternalBorderSize)
            {
                if (y <= cornerSize)
                {
                    position = HTTOPLEFT;
                    _resizeDirection = "NW";
                }
                else if (this.Height - y <= cornerSize)
                {
                    position = HTBOTTOMLEFT;
                    _resizeDirection = "SW";
                }
                else
                {
                    position = HTLEFT;
                    _resizeDirection = "W";
                }
            }
            else if (this.Width - x <= this.InternalBorderSize)
            {
                if (y <= cornerSize)
                {
                    position = HTTOPRIGHT;
                    _resizeDirection = "NE";
                }
                else if (this.Height - y <= cornerSize)
                {
                    position = HTBOTTOMRIGHT;
                    _resizeDirection = "SE";
                }
                else
                {
                    position = HTRIGHT;
                    _resizeDirection = "E";
                }
            }
            else if (y <= 2)
            {
                if (x <= cornerSize)
                {
                    position = HTTOPLEFT;
                    _resizeDirection = "NW";
                }
                else if (this.Width - x <= cornerSize)
                {
                    position = HTTOPRIGHT;
                    _resizeDirection = "NE";
                }
                else
                {
                    position = HTTOP;
                    _resizeDirection = "N";
                }
            }
            else if (this.Height - y <= this.InternalBorderSize)
            {
                if (x <= cornerSize)
                {
                    position = HTBOTTOMLEFT;
                    _resizeDirection = "SW";
                }
                else if (this.Width - x <= cornerSize)
                {
                    position = HTBOTTOMRIGHT;
                    _resizeDirection = "SE";
                }
                else
                {
                    position = HTBOTTOM;
                    _resizeDirection = "S";
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
                _resizing = true;
            }

            return (position != 0);
        }

        #endregion

        #region Constructor

        public BaseForm()
        {
            _closeButton = Properties.Resources.CloseButton;
            _minimizeButton = Properties.Resources.MinimizeButton;
            _moveWindowButton = Properties.Resources.Window;

            _caption = "ThreePM";
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.Text = "";
            this.ControlBox = true;
            this.BackColor = Color.Black;
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this._resizePauseTimer.Tick += new EventHandler(tmrResizePause_Tick);
            this._resizePauseTimer.Interval = 500;
        }

        private void tmrResizePause_Tick(object sender, EventArgs e)
        {
            if (_resizing)
            {
                this.SetBounds(s_backForm.Location.X, s_backForm.Location.Y, s_backForm.Width, s_backForm.Height);
                _resizePauseTimer.Stop();
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
            if (string.IsNullOrEmpty(this.Caption))
            {
                _captionheight = 0;
                this.MinimumSize = new Size(100, 20);
            }
            else
            {
                Size s = TextRenderer.MeasureText(this.Caption, this.Font);
                _captionheight = s.Height + 5;
                this.MinimumSize = new Size(100, _captionheight);
            }


            _displayRectangle = new Rectangle(_internalPadding, _captionheight + _internalPadding, base.DisplayRectangle.Width - (_internalPadding * 2) - 1, base.DisplayRectangle.Height - _captionheight - (_internalPadding * 2) - 1);
            _captionRectangle = new Rectangle(-1, -1, this.Width + 1, _captionheight + 1);
            if (_internalPadding > 0)
            {
                if (_internalBorderAtTop)
                {
                    _displayRectangle = new Rectangle(_internalPadding, _captionheight + _internalPadding + 1, base.DisplayRectangle.Width - (_internalPadding * 2) - 1, base.DisplayRectangle.Height - _captionheight - (_internalPadding * 2) - 2);
                    _captionRectangle = new Rectangle(0, 0, this.Width, _captionheight);
                }
                else
                {
                    _displayRectangle = new Rectangle(_internalPadding, _captionheight + _internalPadding - 1, base.DisplayRectangle.Width - (_internalPadding * 2) - 1, base.DisplayRectangle.Height - _captionheight - (_internalPadding * 2));
                    _captionRectangle = new Rectangle(_internalPadding, _internalPadding, this.Width - (2 * _internalPadding) - 1, _captionheight);
                }
            }

            Rectangle brushRect = new Rectangle(_captionRectangle.X - 1, _captionRectangle.Y - 1, 10, _captionRectangle.Height + 2);

            if (_activeBrush != null) _activeBrush.Dispose();
            if (_inactiveBrush != null) _inactiveBrush.Dispose();

            _activeBrush = new LinearGradientBrush(brushRect, _captionBottomColor, _captionBottomColor, LinearGradientMode.Vertical);
            ColorBlend blend = new ColorBlend(4);
            blend.Colors[0] = _captionTopColor;
            blend.Positions[0] = 0F;
            blend.Colors[1] = _captionTextBackColor;
            blend.Positions[1] = 0.25F;
            blend.Colors[2] = _captionTextBackColor;
            blend.Positions[2] = 0.75F;
            blend.Colors[3] = _captionBottomColor;
            blend.Positions[3] = 1.0F;
            _activeBrush.InterpolationColors = blend;

            _inactiveBrush = new LinearGradientBrush(brushRect, _captionBottomColor, _captionBottomColor, LinearGradientMode.Vertical);
            blend = new ColorBlend(4);
            blend.Colors[0] = _captionTopColorInactive;
            blend.Positions[0] = 0F;
            blend.Colors[1] = _captionTextBackColorInactive;
            blend.Positions[1] = 0.25F;
            blend.Colors[2] = _captionTextBackColorInactive;
            blend.Positions[2] = 0.75F;
            blend.Colors[3] = _captionBottomColorInactive;
            blend.Positions[3] = 1.0F;
            _inactiveBrush.InterpolationColors = blend;
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
                    this.DynamicControlsContainer.Add(control);
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
            for (int i = this.DynamicControlsContainer.Count - 1; i >= 0; i--)
            {
                Control c = this.DynamicControlsContainer[i];
                if (c.Tag != null && c.Tag.ToString().Equals("addcontrol"))
                {
                    this.DynamicControlsContainer.Remove(c);
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
                                    if (p.X < 0) p.X = this.DisplayRectangle.Right + p.X;
                                    if (p.Y < 0) p.Y = this.DisplayRectangle.Bottom + p.Y;
                                    value = p;
                                    break;

                                }
                            case "size":
                                {
                                    Size s = (Size)new SizeConverter().ConvertFromString(prop.Attributes["value"].Value);
                                    if (s.Width < 0) s.Width = this.DisplayRectangle.Width + s.Width;
                                    if (s.Height < 0) s.Height = this.DisplayRectangle.Height + s.Height;
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
