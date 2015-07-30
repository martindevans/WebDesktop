using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace TransparentWindow.DataSource
{
    public class Configuration
    {
        private readonly XDocument _document;
        private readonly XElement _configRoot;

        private IEnumerable<KeyValuePair<string, string>>  DisplayMappings
        {
            get
            {
                return _configRoot
                    .Elements("Display")
                    .Select(a => new KeyValuePair<string, string>(a.Attribute("id").Value.ToString(CultureInfo.InvariantCulture), a.Attribute("url").Value.ToString(CultureInfo.InvariantCulture)));
            }
        }

        private const ushort DEFAULT_PORT = 56347;
        private ushort Port
        {
            get
            {
                var server = _configRoot.Element("Server");
                if (server == null)
                    return DEFAULT_PORT;

                var port = server.Attribute("port");
                if (port == null)
                    return DEFAULT_PORT;

                return ushort.Parse(port.Value);
            }
        }

        public Uri BaseUrl
        {
            get
            {
                return new Uri("http://localhost:" + Port);
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Paths
        {
            get
            {
                var server = _configRoot.Element("Server");
                if (server == null)
                    return new KeyValuePair<string, string>[0];

                var paths = server.Elements("Path");

                return paths.Select(a => new KeyValuePair<string, string>(
                    a.Attribute("id").Value.ToString(CultureInfo.InvariantCulture),
                    a.Value.ToString(CultureInfo.InvariantCulture))
                );
            }
        }

        private Configuration(XDocument document)
        {
            _document = document;
            _configRoot = _document.Element("Configuration");

            if (_configRoot == null)
                throw new ArgumentException("document does not contain a <Configuration> element", "document");
        }

        public bool TryGetUrlForScreen(string screenName, out string url)
        {
            url = DisplayMappings.Where(a => a.Key == screenName)
                .Select(a => a.Value)
                .SingleOrDefault();

            return url != null;
        }

        public static Configuration Load(IFileSystem fileSystem)
        {
            var doc = Loader.LoadConfiguration(fileSystem);

            return new Configuration(doc);
        }

        public void Save()
        {
            Loader.SaveConfiguration(_document);
        }

        private static class Loader
        {
            private static string RootDataDirectory
            {
                get
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WebDesktop");
                }
            }

            private static string GetConfigurationFilePath()
            {
                return Path.Combine(RootDataDirectory, "Configuration.xml");
            }

            public static XDocument LoadConfiguration(IFileSystem filesystem)
            {
                string xmlFile = GetConfigurationFilePath();

                Directory.CreateDirectory(RootDataDirectory);

                if (!File.Exists(xmlFile))
                {
                    using (var defaultStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TransparentWindow.DataSource.DefaultConfiguration.xml"))
                    {
                        if (defaultStream == null)
                            throw new InvalidOperationException("No Repositories specified and cannot find default repositories");

                        using (var file = File.Create(xmlFile))
                            defaultStream.CopyTo(file);
                    }
                }

                using (var file = File.OpenRead(xmlFile))
                {
                    return XDocument.Load(file, LoadOptions.PreserveWhitespace);
                }
            }

            public static void SaveConfiguration(XDocument document)
            {
                string xmlFile = GetConfigurationFilePath();

                document.Save(xmlFile);
            }
        }
    }
}
