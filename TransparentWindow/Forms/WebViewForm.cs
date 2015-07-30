using Awesomium.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Windows.Forms;
using TransparentWindow.Awesomium;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = System.Drawing.Rectangle;

namespace TransparentWindow.Forms
{
    public partial class WebViewForm : FullScreenTransparentForm
    {
        public WebView WebView { get; private set; }
        public string ClientId { get; private set; }

        private readonly SpriteBatch _sprites;

        public ConfigureScreen ConfigForm { get; private set; }

        public WebViewForm(Screen screen, string clientId, Uri uri)
            : base(screen, false)
        {
            ClientId = clientId;

            _sprites = new SpriteBatch(GraphicsDevice);

            //Create a config screen
            ConfigForm = new ConfigureScreen(this);

            //Create web view
            WebView = WebCore.CreateWebView(screen.WorkingArea.Width, screen.WorkingArea.Height, WebViewType.Offscreen);
            WebView.IsTransparent = true;
            WebView.CreateSurface += CreateSurface;

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

        private void InvalidateRectangle(AweRect aweRect)
        {
            //Invalidate the area of the texture drawn to
            Invalidate(new Rectangle(aweRect.X, aweRect.Y, aweRect.Width, aweRect.Height));
        }

        protected override void Draw(Rectangle clipRectangle)
        {
            base.Draw(clipRectangle);

            if (WebView == null)
                return;

            var surface = (TextureSurface)WebView.Surface;
            if (surface != null)
            {
                _sprites.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);

                //Draw only the region which was invalidated
                var r = new Microsoft.Xna.Framework.Rectangle(clipRectangle.X, clipRectangle.Y, clipRectangle.Width, clipRectangle.Height);
                _sprites.Draw(surface.Texture, r, r, Color.White);

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
