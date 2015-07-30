using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using TransparentWindow.DataSource;
using TransparentWindow.Forms;

namespace TransparentWindow
{
    public class DisplayManager
    {
        private readonly Dictionary<string, WebViewForm> _forms = new Dictionary<string, WebViewForm>();

        private readonly Configuration _settings;
        private readonly TrayManager _trayManager;

        public IEnumerable<KeyValuePair<string, WebViewForm>> Forms
        {
            get { return _forms; }
        }

        public DisplayManager(Configuration settings, TrayManager trayManager)
        {
            _settings = settings;
            _trayManager = trayManager;
        }

        public void CreateFormForScreen(Screen screen)
        {
            var id = Id(screen.DeviceName);

            string url;
            if (!_settings.TryGetUrlForScreen(screen.DeviceName, out url))
                url = "/content/default/Blank.html";

            var f1 = new WebViewForm(screen, id, new Uri(_settings.BaseUrl, url));
            _forms.Add(id, f1);
            f1.Show();

            _trayManager.AddScreen(f1);
        }

        private static string Id(string deviceName)
        {
            using (var sha = new SHA1Managed())
            {
                var b = sha.ComputeHash(Encoding.UTF32.GetBytes(deviceName));

                return BitConverter.ToString(b).Replace("-", "");
            }
        }

        public WebViewForm GetById(string clientid)
        {
            return _forms[clientid];
        }
    }
}
