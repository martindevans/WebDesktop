using System;
using System.Configuration;
using System.Globalization;
using Nancy.Hosting.Self;
using Ninject;
using NLog;

namespace TransparentWindow.Nancy
{
    public static class NancyCore
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static NancyHost _host;

        public static void Start(IKernel kernel)
        {
            var uri = new Uri(ConfigurationManager.AppSettings["serverUrl"].ToString(CultureInfo.InvariantCulture));
            var config = new HostConfiguration
            {
                RewriteLocalhost = false,
                AllowChunkedEncoding = false,

                UnhandledExceptionCallback = x => _logger.LogException(LogLevel.Warn, "Unhandled Nancy Exception", x)
            };

            var bootstrapper = new Bootstrapper(kernel);

            _host = new NancyHost(bootstrapper, config, uri);
            _host.Start();

            _logger.Info("Running WebAPI on {0}", uri);
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
