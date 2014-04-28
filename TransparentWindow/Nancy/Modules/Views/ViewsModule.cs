using Nancy;

namespace TransparentWindow.Nancy.Modules.Views
{
    public class ViewsModule
        :NancyModule
    {
        public ViewsModule()
            :base("views")
        {
            Get["/{search_path*}"] = FindView;
        }

        private dynamic FindView(dynamic request)
        {
            var path = (string)request.search_path;
            return View[path, Request.Query];
        }
    }
}
