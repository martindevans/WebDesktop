using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TransparentWindow.Forms
{
    public class FullScreenTransparentForm
        : XnaForm
    {
        public Screen Screen { get; private set; }

        public FullScreenTransparentForm(Screen screen)
        {
            Screen = screen;

            FormBorderStyle = FormBorderStyle.None;  // no borders
            Visible = true;

            //Bounds = screen.WorkingArea;

            ShowInTaskbar = false;

            // Set the form click-through
            //int initialStyle = GetWindowLong(Handle, -20);
            //SetWindowLong(Handle, -20, initialStyle | 0x80000 | 0x20);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Bounds = Screen.WorkingArea;

            //Send to background and do not activate
            SetWindowPos(Handle, new IntPtr(1), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);

            // Set the form click-through
            //int initialStyle = GetWindowLong(Handle, -20);
            //SetWindowLong(Handle, -20, initialStyle | 0x80000 | 0x20);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            //Send to background but keep but remain active
            SetWindowPos(Handle, new IntPtr(1), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Extend aero glass style to whole form
            int[] margins = new int[] { 0, 0, Width, Height };
            DwmExtendFrameIntoClientArea(Handle, ref margins);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // do nothing here to stop window normal background painting
        }

        protected override void OnPaint(PaintEventArgs e)
        {
// ReSharper disable RedundantCheckBeforeAssignment
            if (Bounds != Screen.WorkingArea)
// ReSharper restore RedundantCheckBeforeAssignment
            {
                Bounds = Screen.WorkingArea;
            }

            base.OnPaint(e);
        }

        #region evil windows interop
        private const int WM_WINDOWPOSCHANGING = 0x0046;
        const UInt32 SWP_NOZORDER = 0x0004;
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("dwmapi.dll")]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref int[] pMargins);
        #endregion
    }
}
