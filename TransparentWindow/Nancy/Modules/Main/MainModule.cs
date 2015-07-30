using System.Windows.Forms;
using Nancy;
using Nancy.Routing;

namespace TransparentWindow.Nancy.Modules.Main
{
    public class MainModule
        : NancyModule
    {
        private readonly IRouteCacheProvider _routeCacheProvider;

        public MainModule(IRouteCacheProvider routeCacheProvider)
        {
            _routeCacheProvider = routeCacheProvider;

            Get["/"] = ListRoutes;
            Delete["/"] = Exit;
        }

        private dynamic ListRoutes(dynamic parameters)
        {
            var cache = _routeCacheProvider.GetCache();
            return View["routes.cshtml", cache];
        }

        private static dynamic Exit(dynamic parameters)
        {
            Application.Exit();
            return HttpStatusCode.OK;
        }
    }
}
