using System;
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
            : base(screen, false)
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

        private void CreateSurface(object sender, CreateSurfaceEventArgs args)
        {
            var surface = new TextureSurface(GraphicsDevice);
            surface.OnDraw += InvalidateRectangle;

            args.Surface = surface;
        }

        private Rectangle _invalidated = default(Rectangle);
        private void InvalidateRectangle(AweRect aweRect)
        {
            //Invalidate the area of the texture drawn to
            Invalidate(new System.Drawing.Rectangle(aweRect.X, aweRect.Y, aweRect.Width, aweRect.Height));

            //Union this region into all other regions invalidated this frame
            _invalidated = Rectangle.Union(_invalidated, new Rectangle(aweRect.X, aweRect.Y, aweRect.Width, aweRect.Height));
        }

        protected override void Draw()
        {
            base.Draw();

            if (WebView == null)
                return;

            var surface = (TextureSurface)WebView.Surface;
            if (surface != null)
            {
                //Draw the section of the texture that has been invalidated
                _sprites.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                _sprites.Draw(surface.Texture, _invalidated, _invalidated, Color.White);
                _sprites.End();

                _invalidated = default(Rectangle);
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
