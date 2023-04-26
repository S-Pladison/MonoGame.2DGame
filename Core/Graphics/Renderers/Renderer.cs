using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Pladi.Core.Graphics.Renderers
{
    public abstract class Renderer : ILoadable, IComparable<Renderer>
    {
        public int LoadOrder { get; set; }
        public RenderTarget2D RenderTarget { get; protected set; }
        public bool WasPrepared { get; private set; }
        public Game Game { get; private set; }

        public virtual int RenderOrder { get; }
        public virtual Point Size
        {
            get => ILoadable.GetInstance<ScreenComponent>().Size;
        }

        public event OnRecreateRenderTargetDelegate OnRecreateRenderTarget;

        protected object extraData;

        // ...

        public Renderer()
        {
            LoadOrder = int.MinValue + 255;
        }

        // ...

        void ILoadable.Load()
        {
            Game = Main.Instance;

            ILoadable.GetInstance<RendererHandler>().Add(this);

            OnLoad();
        }

        public void PrepareRenderTarget(GraphicsDevice graphicsDevice)
        {
            if ((RenderTarget == null || RenderTarget.IsDisposed || RenderTarget.Width != Size.X || RenderTarget.Height != Size.Y))
            {
                RenderTarget = new(graphicsDevice, Size.X, Size.Y, false, graphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
                WasPrepared = false;
                OnRecreateRenderTarget?.Invoke(graphicsDevice, Size.X, Size.Y);
            }
        }

        public void Render(GameTime gameTime)
        {
            var spriteBatch = Main.SpriteBatch;
            var graphicsDevice = spriteBatch.GraphicsDevice;

            PrepareRenderTarget(graphicsDevice);

            if (WasPrepared) return;

            var targets = graphicsDevice.GetRenderTargets();

            graphicsDevice.SetRenderTarget(RenderTarget);
            graphicsDevice.Clear(Color.Transparent);

            OnRender(spriteBatch);

            graphicsDevice.SetRenderTargets(targets);

            WasPrepared = true;

            AfterRender(spriteBatch);
            
            extraData = null;
        }

        public void Request(object data)
        {
            Request(null, data);
        }

        public void Request()
        {
            Request(null);
        }

        public void Request(GameTime _, object data)
        {
            WasPrepared = false;
            extraData = data;
        }

        public void Request(GameTime _)
        {
            WasPrepared = false;
            extraData = null;
        }

        public bool TryGetTargetIfPrepared(out RenderTarget2D target)
        {
            if (WasPrepared)
            {
                target = RenderTarget;
                return true;
            }

            target = null;
            return false;
        }

        public int CompareTo(Renderer other)
        {
            return RenderOrder.CompareTo(other.RenderOrder);
        }

        protected virtual void OnLoad() { }
        protected virtual void AfterRender(SpriteBatch spriteBatch) { }
        protected abstract void OnRender(SpriteBatch spriteBatch);

        // ...

        public delegate void OnRecreateRenderTargetDelegate(GraphicsDevice device, int width, int height);
    }
}