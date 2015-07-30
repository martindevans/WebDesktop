using System;
using Awesomium.Core;
using Microsoft.Xna.Framework.Graphics;

namespace TransparentWindow.Awesomium
{
    public class TextureSurface
        :Surface
    {
        private readonly GraphicsDevice _device;

        public Texture2D Texture { get; private set; }

        public TextureSurface(GraphicsDevice device)
        {
            _device = device;

            Disposed += Dispose;
        }

        //Called every time the view resizes
        protected override void Initialize(IWebView view, int width, int height)
        {
            if (Texture !=  null)
                Texture.Dispose();
            Texture = new Texture2D(_device, width, height, false, SurfaceFormat.Color);

            base.Initialize(view, width, height);
        }

        protected override void Paint(IntPtr srcBuffer, int srcRowSpan, AweRect srcRect, AweRect destRect)
        {
            unsafe
            {
                Texture.SetData(
                    0,
                    srcBuffer.ToPointer(),
                    new RECT(destRect.X, destRect.Y, destRect.X + destRect.Width, destRect.Y + destRect.Height),
                    (uint)srcRowSpan,
                    new RECT(srcRect.X, srcRect.Y, srcRect.X + srcRect.Width, srcRect.Y + srcRect.Height), D3DFORMAT.A8R8G8B8
                );
            }
            base.Paint(srcBuffer, srcRowSpan, srcRect, destRect);

            if (OnDraw != null)
                OnDraw(destRect);
        }

        protected override void Scroll(int dx, int dy, AweRect clipRect)
        {
            throw new NotSupportedException();
        }

        private void Dispose(object o, EventArgs e)
        {
            if (Texture != null)
                Texture.Dispose();
            Texture = null;
        }

        public event Action<AweRect> OnDraw;
    }
}
