using Awesomium.Core;
using Ninject;
using System;
using System.IO.Abstractions;
using System.Threading;
using System.Windows.Forms;
using TransparentWindow.Nancy;
using TransparentWindow.DataSource;

namespace TransparentWindow
{
    public class Program
    {
        #region startup
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var mutex = new Mutex(true, "TransparentDesktopMutex");
            if (mutex.WaitOne(0, false))
            {
                Program p = new Program();
                p.Run();
            }
            else
                MessageBox.Show("TransparentDesktop Is Already Running!");

            mutex.Dispose();
        }
        #endregion

        private Program()
        {
        }

        public void Run()
        {
            //Create DI kernel
            IKernel kernel = new StandardKernel();
            kernel.Bind<IFileSystem>().To<FileSystem>().InSingletonScope();

            //Load configuration from disk
            Configuration config = Configuration.Load(kernel.Get<IFileSystem>());
            kernel.Bind<Configuration>().ToConstant(config).InSingletonScope();

            //Startup server
            NancyCore.Start(kernel);

            //Application setup
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

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

            //Run until application is quit
            Application.Run();
        }
    }
}
