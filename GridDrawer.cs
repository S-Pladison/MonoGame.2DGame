using Microsoft.Xna.Framework.Graphics;

namespace Pladi
{
    public class GridDrawer
    {
        public Texture2D RenderedTexture => target;

        // ...

        private RenderTarget2D target;

        private int cellSizeX;
        private int cellSizeY;

        // ...

        public GridDrawer(int cellSizeX, int cellSizeY)
        {
            this.cellSizeX = cellSizeX;
            this.cellSizeY = cellSizeY;

            RecreateRenderTarget(Main.SpriteBatch.GraphicsDevice, Main.SpriteBatch.GraphicsDevice.Viewport.Width, Main.SpriteBatch.GraphicsDevice.Viewport.Height);
        }

        // ...

        public void RecreateRenderTarget(GraphicsDevice device, int Width, int Height)
        {
            target = new RenderTarget2D(device, Width, Height);
        }
    }
}
