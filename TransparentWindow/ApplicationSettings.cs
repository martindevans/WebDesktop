using System.Collections.Generic;

namespace TransparentWindow
{
    public class ApplicationSettings
    {
        /// <summary>
        /// The base URL for all ReST API calls
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The ID of this client to include in ReST API calls
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// A mapping from screen ID -> Url to display on this screen
        /// </summary>
        public Dictionary<string, string> ScreenUrlMapping { get; private set; }

        /// <summary>
        /// The URL to display on screen without a configured URL
        /// </summary>
        public string DefaultUrl { get; private set; }

        public ApplicationSettings()
        {
            
        }

        public ApplicationSettings(string baseUrl, string clientId, Dictionary<string, string> screenUrlMapping, string defaultUrl)
        {
            BaseUrl = baseUrl;
            ClientId = clientId;
            ScreenUrlMapping = screenUrlMapping;
            DefaultUrl = defaultUrl;
        }
    }
}
