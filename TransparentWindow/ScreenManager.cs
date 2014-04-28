using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
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
            using (var sha = new SHA256Managed())
            {
                var b = sha.ComputeHash(Encoding.UTF32.GetBytes(deviceName));

                StringBuilder builder = new StringBuilder(b.Length);
                for (int i = 0; i < b.Length; i++)
                    builder.Append(Encoding.ASCII.GetString(new byte[] { (byte)(b[i] % 25 + 97) }));

                return builder.ToString();
            }
        }

        public WebViewForm GetById(string clientid)
        {
            return _forms[clientid];
        }
    }
}
