using System.Collections.Generic;
using Nancy;

namespace TransparentWindow.Nancy.Modules.Settings
{
    public class SettingsModule
        :NancyModule
    {
        //Temporary - use some kind of database in the future
        private readonly Dictionary<string, Settings> _database = new Dictionary<string, Settings>
        {
            {
                "TEST-PC", new Settings
                {
                    DefaultUrl = "/static/default.html",
                    ScreenUrlMapping = new Dictionary<string, string>
                    {
                        {"\\\\.\\DISPLAY2", "/views/Default/Blank"},
                        {"\\\\.\\DISPLAY1", "/views/Default/Default"}
                    },
                }
            }
        };

        public SettingsModule()
            :base("settings")
        {
            Get["/"] = ShowSettings;
            Get["/{id}"] = ShowSettingsForId;
        }

        private dynamic ShowSettings(dynamic request)
        {
            return _database;
        }

        private dynamic ShowSettingsForId(dynamic request)
        {
            Settings s;
            if (!_database.TryGetValue((string) request.id, out s))
                return HttpStatusCode.NotFound;
            return s;
        }

        private struct Settings
        {
            public Dictionary<string, string> ScreenUrlMapping;
            public string DefaultUrl;
        }
    }
}
