using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Awesomium.Core;
using Ninject;
using Ninject.Parameters;
using RestSharp;
using TransparentWindow.Forms;
using TransparentWindow.Nancy;

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

        private readonly string _baseUrl;
        private readonly string _clientId;

        private Program()
        {
            _baseUrl = ConfigurationManager.AppSettings["serverUrl"].ToString(CultureInfo.InvariantCulture);
            _clientId = ConfigurationManager.AppSettings["clientId"].ToString(CultureInfo.InvariantCulture);
        }

        public void Run()
        {
            //Create DI kernel
            IKernel kernel = new StandardKernel();

            if (bool.Parse(ConfigurationManager.AppSettings["localServer"].ToString(CultureInfo.InvariantCulture)))
            {
                //Startup server
                NancyCore.Start(kernel);
            }

            //Lookup settings from server (with exponential backoff) until success
            ApplicationSettings settings = Helpers.TryExponentialBackup(() => FetchSettings(new RestClient(_baseUrl), _clientId));

            //Application setup
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Awesomium setup
            WebCore.Initialize(new WebConfig
            {
                RemoteDebuggingPort = 41337,
                RemoteDebuggingHost = "127.0.0.1"
            });

            ScreenManager screenManager = new ScreenManager(settings);
            kernel.Bind<ScreenManager>().ToConstant(screenManager);

            //Create forms for each screen
            foreach (var screen in Screen.AllScreens)
            {
                screenManager.CreateFormForScreen(screen);
            }

            //Run until application is quit
            Application.Run();
        }

        private static ApplicationSettings FetchSettings(RestClient client, string clientId)
        {
            var request = new RestRequest("settings/{id}", Method.GET);
            request.AddUrlSegment("id", clientId);

            // execute the request
            var response = client.Execute<ApplicationSettings>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
                return null;

            response.Data.ClientId = clientId;
            response.Data.BaseUrl = client.BaseUrl;
            return response.Data;
        }
    }
}
