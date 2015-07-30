using System;
using Microsoft.Win32;
using System.Windows.Forms;
using TransparentWindow.Forms;
using TransparentWindow.Properties;

namespace TransparentWindow
{
    public class TrayManager
    {
        private readonly NotifyIcon _trayIcon;
        private bool _disposed = false;

        private readonly MenuItem _runOnStartup;
        private readonly MenuItem _screens;

        public TrayManager()
        {
            // Initialize Tray Icon
            _trayIcon = new NotifyIcon {
                Visible = true,
                Icon = Resources.Icon,
                Text = Resources.ApplicationName,

                ContextMenu = new ContextMenu(new MenuItem[] {
                    //List of configured screens
                    (_screens = new MenuItem("Screens")),

                    //Whether or not to run on startup
                    (_runOnStartup = new MenuItem("Run On Startup", (_, __) => ToggleRunOnStartup()) {
                        Checked = IsRunningOnStartup(),
                    }),

                    //Restart the application
                    new MenuItem("Restart", (_, __) => Application.Restart()),

                    //Exit the application
                    new MenuItem("Exit", (_, __) => {
                        Close();
                        Environment.Exit(1);
                    }),
                })
            };
        }

        #region window management
        public void AddScreen(WebViewForm form)
        {
            _screens.MenuItems.Add(new MenuItem(form.Screen.DeviceName, (_, __) => ConfigureForm(form)));
        }

        private void ConfigureForm(WebViewForm form)
        {
            form.ConfigForm.Show();
        }
        #endregion

        #region windows startup
        // The path to the key where Windows looks for startup applications
        readonly RegistryKey _rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private const string REGISTRY_KEY = "WEB_DESKTOP";
        private void ToggleRunOnStartup()
        {
            if (!IsRunningOnStartup()) {
                _rkApp.SetValue(REGISTRY_KEY, Application.ExecutablePath);
            } else {
                _rkApp.DeleteValue(REGISTRY_KEY, false);
            }

            _runOnStartup.Checked = IsRunningOnStartup();
        }

        public bool IsRunningOnStartup()
        {
            return _rkApp.GetValue(REGISTRY_KEY) != null;
        }
        #endregion

        public void Close()
        {
            if (!_disposed) {
                _disposed = true;

                _trayIcon.Visible = false;
                _trayIcon.Dispose();
            }
        }
    }
}
