using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;

namespace Pladi
{
    public class Grid
    {
        public Texture2D RenderedTexture => target;

        // ...

        private RenderTarget2D target;
        private Texture2D texture;

        private int cellSizeX;
        private int cellSizeY;
        private uint width;

        // ...

        public Grid(int cellSizeX, int cellSizeY, uint width = 2)
        {
            texture = TextureAssets.Pixel;

            this.cellSizeX = cellSizeX;
            this.cellSizeY = cellSizeY;
            this.width = width;

            RecreateRenderTarget(Main.SpriteBatch.GraphicsDevice, Main.SpriteBatch.GraphicsDevice.Viewport.Width, Main.SpriteBatch.GraphicsDevice.Viewport.Height);
        }

        // ...

        public void RecreateRenderTarget(GraphicsDevice device, int Width, int Height)
        {
            target = new RenderTarget2D(device, Width, Height);
        }

        public void Render(SpriteBatch spriteBatch, Camera camera)
        {
            if (target is null) return;

            var device = spriteBatch.GraphicsDevice;

            device.SetRenderTarget(target);
            device.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.TransformMatrix);

            var cameraBounds = camera.VisibleArea;
            var offset = new Point(cameraBounds.X / cellSizeX, cameraBounds.Y / cellSizeY);
            var xMax = cameraBounds.Width / cellSizeX + offset.X + 1;
            var yMax = cameraBounds.Height / cellSizeY + offset.Y + 1;
            var halfWidth = (int)(width / 2);

            for (float x = offset.X; x <= xMax; x++)
            {
                var position = new Point((int)(x * cellSizeX) - halfWidth, cameraBounds.Y);
                var rectangle = new Rectangle(position.X, position.Y, (int)width, cameraBounds.Height);
                spriteBatch.Draw(TextureAssets.Pixel, rectangle, Color.White);
            }

            for (float y = offset.Y; y <= yMax; y++)
            {
                var position = new Point(cameraBounds.X, (int)(y * cellSizeY) - halfWidth);
                var rectangle = new Rectangle(position.X, position.Y, cameraBounds.Width, (int)width);
                spriteBatch.Draw(TextureAssets.Pixel, rectangle, Color.White);
            }

            spriteBatch.End();
            device.SetRenderTarget(null);
        }
    }
}
