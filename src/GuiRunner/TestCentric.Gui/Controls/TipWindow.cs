// ***********************************************************************
// Copyright (c) Charlie Poole and TestCentric contributors.
// Licensed under the MIT License. See LICENSE file in root directory.
// ***********************************************************************

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NUnit;
using TestCentric.Gui.Views;

namespace TestCentric.Gui.Controls
{
    public class TipWindow : Form
    {
        // Margin of screen, used to limit TipWindow expansion
        private const int SCREEN_EDGE = 20;
        private const int SCREEN_MARGIN = 2 * SCREEN_EDGE;

        // Padding to leave inside the TipWindow around the text
        private const int PADDING_LEFT = 4;
        private const int PADDING_RIGHT = 4;
        private const int PADDING_TOP = 4;
        private const int PADDING_BOTTOM = 4;

        /// <summary>
        /// Direction in which to expand
        /// </summary>
        public enum ExpansionStyle
        {
            Horizontal,
            Vertical,
            Both
        }

        #region Instance Variables

        /// <summary>
        /// The control for which we are showing expanded text
        /// </summary>
        private Control _control;

        /// <summary>
        /// Timer used for auto-close
        /// </summary>
        private System.Windows.Forms.Timer _autoCloseTimer;

        /// <summary>
        /// Timer used for mouse leave delay
        /// </summary>
        private System.Windows.Forms.Timer _mouseLeaveTimer;

        /// <summary>
        /// Rectangle used to display text
        /// </summary>
        private Rectangle _textRect;

        #endregion

        #region Construction and Initialization

        public TipWindow(Label label)
        {
            InitializeComponent();
            InitializeControl(label);

            ItemBounds = label.ClientRectangle;
            TipText = label.Text;

            // Handle mouse leaving the label when not overlaid by the tip window
            if (MouseLeaveDelay > 0 && !Overlay)
                _control.MouseLeave += (s, e) => OnMouseLeave(e);
        }

        public TipWindow(ListBox listbox, int index)
        {
            InitializeComponent();
            InitializeControl(listbox);

            ItemBounds = listbox.GetItemRectangle(index);
            TipText = listbox.Items[index].ToString();
        }

        public TipWindow(TestCentricTreeView treeView)
        {
            InitializeComponent();
            InitializeControl(treeView);
        }

        private void InitializeComponent()
        {
            // 
            // TipWindow
            // 
            BackColor = System.Drawing.Color.LightYellow;
            ClientSize = new System.Drawing.Size(292, 268);
            ControlBox = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TipWindow";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;

        }

        private void InitializeControl(Control control)
        {
            _control = control;
            Owner = control.FindForm();
            Font = control.Font;
        }

        #endregion

        #region Public Properties and Methods

        public bool Overlay { get; set; }
        public ExpansionStyle Expansion { get; set; }
        public int AutoCloseDelay { get; set; }
        public int MouseLeaveDelay { get; set; }
        public string TipText { get; set; }
        public Rectangle ItemBounds { get; set; }
        public bool WantClicks { get; set; }

        public void Display(TreeNode treeNode, string resultText)
        {
            // HACK: Needed until we create a hierarchy of TipWindows with implementations for different controls
            Guard.OperationValid(_control is TreeView, "Internal Error: 'Display(TreeNode node)' may only be called when control is a TreeView.");

            Hide();

            ItemBounds = treeNode.Bounds;
            Font = treeNode.NodeFont ?? treeNode.TreeView.Font;
            TipText = treeNode.Text;
            if (resultText != null)
                TipText += " " + resultText;

            AdjustLocation();

            Graphics g = Graphics.FromHwnd(Handle);
            Screen screen = Screen.FromControl(_control);
            SizeF layoutArea = new SizeF(screen.WorkingArea.Width - SCREEN_MARGIN, screen.WorkingArea.Height - SCREEN_MARGIN);
            if (Expansion == ExpansionStyle.Vertical)
                layoutArea.Width = ItemBounds.Width;
            else if (Expansion == ExpansionStyle.Horizontal)
                layoutArea.Height = ItemBounds.Height;

            Size sizeNeeded = Size.Ceiling(g.MeasureString(TipText, Font, layoutArea));

            // When used with a label, if the needed width is smaller than that of the
            // label, it can be visually confusing, so we adjust. This can only happen
            // with ExpansionStyle.Both, so we won't get here unless either the
            // height or the width is greater.
            if (_control is Label && sizeNeeded.Width < ItemBounds.Width)
                sizeNeeded.Width = ItemBounds.Width;

            ClientSize = sizeNeeded;
            Size = sizeNeeded + new Size(PADDING_LEFT + PADDING_RIGHT, PADDING_TOP + PADDING_BOTTOM);
            _textRect = new Rectangle(PADDING_LEFT, PADDING_TOP, sizeNeeded.Width, sizeNeeded.Height);

            if (Right > screen.WorkingArea.Right)
            {
                Left = Math.Max(
                    screen.WorkingArea.Right - Width - SCREEN_EDGE,
                    screen.WorkingArea.Left + SCREEN_EDGE);
            }

            if (Bottom > screen.WorkingArea.Bottom - SCREEN_EDGE)
            {
                if (Overlay)
                    Top = Math.Max(
                        screen.WorkingArea.Bottom - Height - SCREEN_EDGE,
                        screen.WorkingArea.Top + SCREEN_EDGE);

                if (Bottom > screen.WorkingArea.Bottom - SCREEN_EDGE)
                    Height = screen.WorkingArea.Bottom - SCREEN_EDGE - Top;

            }

            if (AutoCloseDelay > 0)
            {
                _autoCloseTimer = new System.Windows.Forms.Timer();
                _autoCloseTimer.Interval = AutoCloseDelay;
                _autoCloseTimer.Tick += (s, e) => Hide();
                _autoCloseTimer.Start();
            }

            Show();
        }

        #endregion

        #region Overrides

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Rectangle outlineRect = ClientRectangle;
            outlineRect.Width--;
            outlineRect.Height--;
            g.DrawRectangle(Pens.Black, outlineRect);
            g.DrawString(TipText, Font, Brushes.Black, _textRect);
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            if (_mouseLeaveTimer != null)
            {
                _mouseLeaveTimer.Stop();
                _mouseLeaveTimer.Dispose();
                System.Diagnostics.Debug.WriteLine("Entered TipWindow - stopped mouseLeaveTimer");
            }
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            if (MouseLeaveDelay > 0)
            {
                _mouseLeaveTimer = new System.Windows.Forms.Timer();
                _mouseLeaveTimer.Interval = MouseLeaveDelay;
                _mouseLeaveTimer.Tick += (s, e) => Hide();
                _mouseLeaveTimer.Start();
            }
        }

        [DllImport("user32.dll")]
        static extern uint SendMessage(
            IntPtr hwnd,
            int msg,
            IntPtr wparam,
            IntPtr lparam
            );

        protected override void WndProc(ref Message m)
        {
            uint WM_LBUTTONDOWN = 0x201;
            uint WM_RBUTTONDOWN = 0x204;
            uint WM_MBUTTONDOWN = 0x207;

            if (_control is Label)
                if (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_RBUTTONDOWN || m.Msg == WM_MBUTTONDOWN)
                {
                    if (m.Msg != WM_LBUTTONDOWN)
                        Hide();
                    SendMessage(_control.Handle, m.Msg, m.WParam, m.LParam);
                    return;
                }

            base.WndProc(ref m);
        }

        #endregion

        #region Private Methods

        private void AdjustLocation()
        {
            Point origin = _control.Parent.PointToScreen(_control.Location);
            origin.Offset(ItemBounds.Left, ItemBounds.Top);
            if (!Overlay) origin.Offset(0, ItemBounds.Height);
            Location = origin;
        }

        #endregion
    }
}
