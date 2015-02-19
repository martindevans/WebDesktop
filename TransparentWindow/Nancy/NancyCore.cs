using Nancy.Hosting.Self;
using Ninject;
using NLog;
using Configuration = TransparentWindow.DataSource.Configuration;

namespace TransparentWindow.Nancy
{
    public static class NancyCore
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static NancyHost _host;

        public static void Start(IKernel kernel)
        {
            

            var config = new HostConfiguration
            {
                RewriteLocalhost = false,
                AllowChunkedEncoding = false,

                UnhandledExceptionCallback = x => _logger.Log(LogLevel.Warn, "Unhandled Nancy Exception", x)
            };

            var bootstrapper = new Bootstrapper(kernel);

            var url = kernel.Get<Configuration>().BaseUrl;
            _host = new NancyHost(bootstrapper, config, url);
            _host.Start();

            _logger.Info("Running WebAPI on {0}", url);
        }

        public static void Update()
        {
        }

        public static void Shutdown()
        {
            if (_host != null)
            {
                _host.Stop();
                _host.Dispose();
                _host = null;
            }
        }
    }
}
