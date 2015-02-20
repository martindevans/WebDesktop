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

        public IEnumerable<KeyValuePair<string, string>>  DisplayMappings
        {
            get
            {
                return _document.Element("Configuration")
                    .Element("Displays")
                    .Elements("Display")
                    .Select(a => new KeyValuePair<string, string>(a.Attribute("id").Value.ToString(CultureInfo.InvariantCulture), a.Attribute("url").Value.ToString(CultureInfo.InvariantCulture)));
            }
        }

        public ushort Port
        {
            get
            {
                var v = _document.Element("Configuration")
                                 .Element("Server")
                                 .Attribute("port")
                                 .Value;

                return ushort.Parse(v);
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
                return _document.Element("Configuration")
                         .Element("Server")
                         .Elements("Path")
                         .Select(a => new KeyValuePair<string, string>(a.Attribute("id").Value.ToString(CultureInfo.InvariantCulture), a.Value.ToString(CultureInfo.InvariantCulture)));
            }
        }

        private Configuration(XDocument document)
        {
            _document = document;
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
            var doc = new Loader().LoadConfiguration(fileSystem);

            return new Configuration(doc);
        }

        public void Save()
        {
            new Loader().SaveConfiguration(_document);
        }

        private class Loader
        {
            private string RootDataDirectory
            {
                get
                {
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WebDesktop");
                }
            }

            private string GetConfigurationFilePath()
            {
                return Path.Combine(RootDataDirectory, "Configuration.xml");
            }

            public XDocument LoadConfiguration(IFileSystem filesystem)
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

            public void SaveConfiguration(XDocument document)
            {
                string xmlFile = GetConfigurationFilePath();

                document.Save(xmlFile);
            }
        }
    }
}
