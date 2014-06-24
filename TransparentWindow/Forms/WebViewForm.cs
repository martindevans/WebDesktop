using System;
using System.Drawing;
using System.Windows.Forms;
using Awesomium.Core;
using Microsoft.Xna.Framework.Graphics;
using TransparentWindow.Awesomium;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TransparentWindow.Forms
{
    public partial class WebViewForm : FullScreenTransparentForm
    {
        public WebView WebView { get; private set; }
        public string ClientId { get; private set; }

        private readonly ApplicationSettings _settings;
        private readonly SpriteBatch _sprites;

        public WebViewForm(Screen screen, string clientId, ApplicationSettings settings)
            : base(screen)
        {
            ClientId = clientId;
            _settings = settings;

            _sprites = new SpriteBatch(GraphicsDevice);

            //Create web view
            WebView = WebCore.CreateWebView(screen.WorkingArea.Width, screen.WorkingArea.Height, WebViewType.Offscreen);
            WebView.IsTransparent = true;
            WebView.CreateSurface += CreateSurface;

            //Get URL mapping for this screen
            string url;
            if (!_settings.ScreenUrlMapping.TryGetValue(Screen.DeviceName, out url))
                url = _settings.DefaultUrl;
            var uri = new Uri(new Uri(_settings.BaseUrl), url);

            //Add client ID to query string
            string qr = "";
            if (uri.Query == "")
                qr += "?";
            else
                qr += "&";
            qr += "ClientId=" + ClientId;

            //Build complete URL
            uri = new Uri(uri + qr);

            //Navigate to it
            WebView.Source = uri;
        }

        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);

        //    ContextMenu = new ContextMenu(
        //        new MenuItem[]
        //        {
        //            new MenuItem("Blind", new MenuItem[]
        //            {
        //                new MenuItem("Blind"),
        //                new MenuItem("Drunk"),
        //                new MenuItem("Llamas"),
        //            }),
        //            new MenuItem("Drunk"),
        //            new MenuItem("Llamas"),
        //        }
        //    );
        //}

        private void CreateSurface(object sender, CreateSurfaceEventArgs args)
        {
            var surface = new TextureSurface(GraphicsDevice);
            surface.OnDraw += Invalidate;

            args.Surface = surface;
        }

        protected override void Draw()
        {
            base.Draw();

            if (WebView == null)
                return;

            var surface = (TextureSurface)WebView.Surface;
            if (surface != null)
            {
                _sprites.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                _sprites.Draw(surface.Texture, new Rectangle(0, 0, Width, Height), Color.White);
                _sprites.End();
            }
        }

        #region mouse injection
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            //Park mouse out of the way
            WebView.InjectMouseMove(-1, -1);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //Inject mouse move into web view
            WebView.InjectMouseMove(e.X, e.Y);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    WebView.InjectMouseDown(MouseButton.Left);
                    break;
                case MouseButtons.Right:
                    WebView.InjectMouseDown(MouseButton.Right);
                    break;
                case MouseButtons.Middle:
                    WebView.InjectMouseDown(MouseButton.Middle);
                    break;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    WebView.InjectMouseUp(MouseButton.Left);
                    break;
                case MouseButtons.Right:
                    WebView.InjectMouseUp(MouseButton.Right);
                    break;
                case MouseButtons.Middle:
                    WebView.InjectMouseUp(MouseButton.Middle);
                    break;
            }
        }
        #endregion
    }
}
