using System;
using Awesomium.Core;
using Microsoft.Xna.Framework.Graphics;

namespace TransparentWindow.Awesomium
{
    public class TextureSurface
        :Surface
    {
        private readonly GraphicsDevice _device;
        private Texture2D _texture;

        public Texture2D Texture
        {
            get { return _texture; }
        }

        public TextureSurface(GraphicsDevice device)
        {
            _device = device;

            Disposed += Dispose;
        }

        //Called every time the view resizes
        protected override void Initialize(IWebView view, int width, int height)
        {
            if (_texture !=  null)
                _texture.Dispose();
            _texture = new Texture2D(_device, width, height, false, SurfaceFormat.Color);

            base.Initialize(view, width, height);
        }

        protected override void Paint(IntPtr srcBuffer, int srcRowSpan, AweRect srcRect, AweRect destRect)
        {
            unsafe
            {
                _texture.SetData(
                    0,
                    srcBuffer.ToPointer(),
                    new RECT(destRect.X, destRect.Y, destRect.X + destRect.Width, destRect.Y + destRect.Height),
                    (uint)srcRowSpan,
                    new RECT(srcRect.X, srcRect.Y, srcRect.X + srcRect.Width, srcRect.Y + srcRect.Height), D3DFORMAT.A8R8G8B8);
            }
            base.Paint(srcBuffer, srcRowSpan, srcRect, destRect);

            if (OnDraw != null)
                OnDraw();
        }

        protected override void Scroll(int dx, int dy, AweRect clipRect)
        {
            throw new NotSupportedException();
        }

        private void Dispose(object o, EventArgs e)
        {
            if (_texture != null)
                _texture.Dispose();
            _texture = null;
        }

        public event Action OnDraw;
    }
}
