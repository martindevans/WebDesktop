using System;
using System.Windows.Forms;

namespace TransparentWindow.Forms
{
    public partial class ConfigureScreen : Form
    {
        private readonly WebViewForm _webViewForm;

        public ConfigureScreen(WebViewForm webViewForm)
        {
            _webViewForm = webViewForm;

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            Text = _webViewForm.Screen.DeviceName;

            base.OnLoad(e);
        }
    }
}
