using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TransparentWindow.Forms
{
    /// <summary>
    /// A form with an XNA GraphicsDevice
    /// </summary>
    public class XnaForm
        : Form
    {
        public GraphicsDevice GraphicsDevice { get; private set; }

        public XnaForm()
        {
            // Create graphics Device
            // Create device presentation parameters
            PresentationParameters p = new PresentationParameters
            {
                IsFullScreen = false,
                DeviceWindowHandle = Handle,
                BackBufferFormat = SurfaceFormat.Vector4,
                PresentationInterval = PresentInterval.One
            };

            // Create XNA graphics device
            GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, p);

            //Every time we have some spare time in the message processing queue, tick this
            Application.Idle += Tick;
        }

        readonly Stopwatch _stopWatch = Stopwatch.StartNew();

        readonly TimeSpan _targetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 30);
        readonly TimeSpan _maxElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 10);

        TimeSpan _accumulatedTime;
        TimeSpan _lastTime;

        protected void Tick(object sender, EventArgs eventArgs)
        {
            //All taken verbatim from: http://blogs.msdn.com/b/shawnhar/archive/2010/12/06/when-winforms-met-game-loop.aspx?Redirected=true

            TimeSpan currentTime = _stopWatch.Elapsed;
            TimeSpan elapsedTime = currentTime - _lastTime;
            _lastTime = currentTime;

            if (elapsedTime > _maxElapsedTime)
            {
                elapsedTime = _maxElapsedTime;
            }

            _accumulatedTime += elapsedTime;

            bool updated = false;

            while (_accumulatedTime >= _targetElapsedTime)
            {
                XnaUpdate(_targetElapsedTime);

                _accumulatedTime -= _targetElapsedTime;
                updated = true;
            }

            if (updated)
            {
                Invalidate();
            }
        }

        protected virtual void XnaUpdate(TimeSpan deltaTime)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            ResetGraphicsDevice();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal)
            {
                // Clear device with fully transparent black
                GraphicsDevice.Clear(Color.Transparent);

                //Draw stuff
                Draw();

                // Present the device contents into form
                try
                {
                    GraphicsDevice.Present();
                }
                catch (DeviceLostException)
                {
                }
            }
            else if (GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.NotReset)
            {
                ResetGraphicsDevice();
            }
        }

        private void ResetGraphicsDevice()
        {
            //Resize graphics device
            if (GraphicsDevice != null)
            {
                GraphicsDevice.Reset(new PresentationParameters
                {
                    IsFullScreen = false,
                    DeviceWindowHandle = Handle,
                    BackBufferFormat = SurfaceFormat.Vector4,
                    PresentationInterval = PresentInterval.One
                });
            }
        }

        protected virtual void Draw()
        {
        }
    }
}
