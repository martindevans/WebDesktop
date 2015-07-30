using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = System.Drawing.Rectangle;

namespace TransparentWindow.Forms
{
    /// <summary>
    /// A form with an XNA GraphicsDevice
    /// </summary>
    public class XnaForm
        : Form
    {
        private readonly bool _autoInvalidate;

        protected GraphicsDevice GraphicsDevice { get; private set; }

        protected XnaForm(bool autoInvalidate = true)
        {
            _autoInvalidate = autoInvalidate;

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

        private void Tick(object sender, EventArgs eventArgs)
        {
            //All taken verbatim from: http://blogs.msdn.com/b/shawnhar/archive/2010/12/06/when-winforms-met-game-loop.aspx?Redirected=true

            TimeSpan currentTime = _stopWatch.Elapsed;
            TimeSpan elapsedTime = currentTime - _lastTime;
            _lastTime = currentTime;

            if (elapsedTime > _maxElapsedTime)
                elapsedTime = _maxElapsedTime;

            _accumulatedTime += elapsedTime;

            bool updated = false;

            while (_accumulatedTime >= _targetElapsedTime)
            {
                XnaUpdate(_targetElapsedTime);

                _accumulatedTime -= _targetElapsedTime;
                updated = true;
            }

            if (updated && _autoInvalidate)
                Invalidate();
        }

        protected virtual void XnaUpdate(TimeSpan deltaTime)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            ResetGraphicsDevice();
        }

        private int _resetAttempts = 0;
        private int _resetCountdown = 0;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal)
            {
                //Reset exponential backoff
                _resetAttempts = 0;
                _resetCountdown = 0;

                // Clear device with fully transparent black
                GraphicsDevice.Clear(Color.Transparent);

                //Draw stuff
                Draw(e.ClipRectangle);

                // Present the device contents into form
                try
                {
                    var r = new Microsoft.Xna.Framework.Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);
                    GraphicsDevice.Present(r, r, Handle);
                }
                catch (DeviceLostException)
                {
                }
            }
            else if (GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.NotReset)
            {
                //Reset device with an exponential backoff
                //This prevents us locking up the system by spamming the graphics driver with reset requests
                if (_resetCountdown == 0)
                {
                    _resetAttempts++;
                    _resetCountdown = Math.Min(1000, 1 << _resetAttempts);
                    ResetGraphicsDevice();
                }
                else
                {
                    _resetCountdown--;
                }
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
                    PresentationInterval = PresentInterval.Two
                });
            }
        }

        protected virtual void Draw(Rectangle clipRectangle)
        {
        }
    }
}
