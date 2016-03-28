using System;
using System.Diagnostics.Contracts;
using System.Windows.Forms;
using Nancy.Helpers;
using TransparentWindow.DataSource;
using TransparentWindow.Properties;

namespace TransparentWindow.Forms
{
    public partial class ConfigureScreen : Form
    {
        private readonly Configuration _configuration;
        private readonly WebViewForm _webViewForm;

        public ConfigureScreen(Configuration configuration, WebViewForm webViewForm)
        {
            Contract.Requires(configuration != null);
            Contract.Requires(webViewForm != null);

            _configuration = configuration;
            _webViewForm = webViewForm;

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            Text = _webViewForm.Screen.DeviceName;
            Icon = Resources.Icon;

            int index = 0;
            foreach (var path in _configuration.Paths)
            {
                PathSelection.Items.Add(path.Key);

                var absPath = _webViewForm.Uri.AbsolutePath;

                //if (path.Value

                index++;
            }

            base.OnLoad(e);
        }

        private void ConfigureScreen_Load(object sender, EventArgs e)
        {
        }

        private void edit_Click(object sender, EventArgs e)
        {

            UrlInput.Show();

            var uri = new UriBuilder(_webViewForm.Uri);
            var query = HttpUtility.ParseQueryString(_webViewForm.Uri.Query);
            query.Remove("ClientId");
            uri.Query = query.ToString();

            UrlInput.Text = uri.ToString();

            PathSelection.Hide();
        }
    }
}
