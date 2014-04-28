using System;
using System.Windows.Forms;
using Awesomium.Core;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using TransparentWindow.Forms;

namespace TransparentWindow.Awesomium
{
    public class AwesomiumMouseHook
        :MouseHookListener, IDisposable
    {
        private readonly WebViewForm _form;

        public AwesomiumMouseHook(WebViewForm webView)
            :base(new GlobalHooker())
        {
            Enabled = true;

            _form = webView;

            MouseMove += OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var x = e.X - _form.DesktopBounds.X;
            var y = e.Y - _form.DesktopBounds.Y;
            if (x >= 0 && y >= 0 && x <= _form.DesktopBounds.Width && y <= _form.DesktopBounds.Height)
                _form.WebView.InjectMouseMove(x, y);
        }
    }
}
