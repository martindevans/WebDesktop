using Awesomium.Core;
using Microsoft.Win32;
using Ninject;
using System;
using System.IO.Abstractions;
using System.Threading;
using System.Windows.Forms;
using TransparentWindow.DataSource;
using TransparentWindow.Nancy;

namespace TransparentWindow
{
    public class Program
        : ApplicationContext
    {
        #region startup
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var mutex = new Mutex(true, "TransparentDesktopMutex");
            if (mutex.WaitOne(0, false)) {

                //Application setup
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //Run
                Program p = new Program();
                p.Run();
            }
            else
                MessageBox.Show("TransparentDesktop Is Already Running!");

            mutex.Dispose();
        }
        #endregion

        private readonly TrayManager _trayManager;

        public Program()
        {
            _trayManager = new TrayManager();
        }

        private void Run()
        {
            //Create DI kernel
            IKernel kernel = new StandardKernel();
            kernel.Bind<IFileSystem>().To<FileSystem>().InSingletonScope();
            kernel.Bind<TrayManager>().ToConstant(_trayManager);

            //Load configuration from disk
            Configuration config = Configuration.Load(kernel.Get<IFileSystem>());
            kernel.Bind<Configuration>().ToConstant(config).InSingletonScope();

            //Awesomium setup
            WebCore.Initialize(new WebConfig
            {
                RemoteDebuggingPort = 41337,
                RemoteDebuggingHost = "127.0.0.1"
            });

            //Create forms for each screen
            DisplayManager screenManager = kernel.Get<DisplayManager>();
            kernel.Bind<DisplayManager>().ToConstant(screenManager);
            foreach (var screen in Screen.AllScreens)
                screenManager.CreateFormForScreen(screen);

            //Startup server
            NancyCore.Start(kernel);

            //Run until application is quit
            Application.Run(this);

            //Close tray icon when program closes
            _trayManager.Close();
        }
    }
}
