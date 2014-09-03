using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using TransparentWindow.Forms;

namespace TransparentWindow.Nancy.Modules.Screens
{
    public class ScreensModule
        : NancyModule
    {
        private readonly DisplayManager _screenManager;

        public ScreensModule(DisplayManager screenManager)
            : base("screens")
        {
            _screenManager = screenManager;

            Get["/"] = GetScreens;
            Get["/{clientid}"] = GetScreensById;

            Post["/regions/{clientid}"] = PostRegion;
            Put["/regions/{clientid}"] = PutRegion;
            Delete["/regions/{clientid}"] = DeleteRegion;
        }

        private dynamic SerializeForm(WebViewForm f)
        {
            return new
            {
                ClientId = f.ClientId,
                Screen = f.Screen,
                Url = f.Invoke(new Func<Uri>(() => f.WebView.Source)).ToString(),
            };
        }

        private object GetScreens(dynamic arg)
        {
            return _screenManager.Forms.Select(a => SerializeForm(a.Value));
        }

        private object GetScreensById(dynamic arg)
        {
            var f = _screenManager.GetById((string)arg.clientid);
            return SerializeForm(f);
        }

        private object PostRegion(dynamic arg)
        {
            var rectangle = this.Bind<Rectangle>();
            var form = _screenManager.GetById((string) arg.clientid);

            GraphicsPath g = new GraphicsPath();
            g.AddRectangle(new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height));

            form.Invoke(new Action(() => form.AddClickRegion(g)));

            return HttpStatusCode.OK;
        }

        private object PutRegion(dynamic arg)
        {
            var rectangles = this.Bind<Rectangles>().Regions;
            var form = _screenManager.GetById((string)arg.clientid);

            GraphicsPath g = new GraphicsPath();
            foreach (var rectangle in rectangles)
                g.AddRectangle(new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height));

            return form.Invoke(new Action(() => form.SetClickRegion(g)));
        }

        private object DeleteRegion(dynamic arg)
        {
            var form = _screenManager.GetById((string)arg.clientid);

            return form.Invoke(new Action(form.ClearClickRegion));
        }

        private struct Rectangle
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
        }

        private struct Rectangles
        {
            public Rectangle[] Regions;
        }
    }
}
