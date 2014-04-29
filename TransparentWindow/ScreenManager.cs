using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Nancy.Helpers;
using TransparentWindow.Forms;

namespace TransparentWindow
{
    public class ScreenManager
    {
        private readonly ApplicationSettings _settings;

        private readonly Dictionary<string, WebViewForm> _forms = new Dictionary<string, WebViewForm>();

        public IEnumerable<KeyValuePair<string, WebViewForm>> Forms
        {
            get { return _forms; }
        }

        public ScreenManager(ApplicationSettings settings)
        {
            _settings = settings;
        }

        public void CreateFormForScreen(Screen screen)
        {
            var id = Id(screen.DeviceName);

            var f1 = new WebViewForm(screen, id, _settings);
            _forms.Add(id, f1);
            f1.Show();
        }

        private string Id(string deviceName)
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
