using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Tiles;
using Pladi.Utils;

namespace Pladi.Scenes
{
    public class EditorScene : Scene
    {
        private Tilemap tilemap;
        private Grid grid;
        private Camera camera;

        // ...

        public override void Init()
        {
            tilemap = new Tilemap(100, 100, 4f);
            tilemap.SetTexture(TextureAssets.Tilemap, 8, 12);

            grid = new Grid(32, 32, 2);

            camera = new Camera(Main.SpriteBatch.GraphicsDevice.Viewport, 1f);
        }

        public override void Update(GameTime gameTime)
        {
            var input = Main.InputManager;
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (input.JustPressed(Keys.Escape))
            {
                Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Menu);
            }

            float cameraSpeed = 32 * 15 * delta;

            if (input.IsPressed(Keys.W))
            {
                camera.Location.Y -= cameraSpeed;
            }

            if (input.IsPressed(Keys.S))
            {
                camera.Location.Y += cameraSpeed;
            }

            if (input.IsPressed(Keys.D))
            {
                camera.Location.X += cameraSpeed;
            }

            if (input.IsPressed(Keys.A))
            {
                camera.Location.X -= cameraSpeed;
            }

            if (Main.InputManager.IsPressed(Keys.NumPad8))
            {
                camera.Zoom += 0.5f * delta;
            }

            if (Main.InputManager.IsPressed(Keys.NumPad2))
            {
                camera.Zoom -= 0.5f * delta;
            }

            if (!input.JustPressed(Input.MouseInputTypes.LeftButton)) return;

            var mousePosition = camera.ScreenToWorldSpace(Main.InputManager.GetMousePosition());
            var tileCoordsX = (int)(mousePosition.X / 32);
            var tileCoordsY = (int)(mousePosition.Y / 32);

            if (tileCoordsX < 0 || tileCoordsX >= tilemap.Width || tileCoordsY < 0 || tileCoordsY >= tilemap.Height) return;

            tilemap.SetTile(tileCoordsX, tileCoordsY, new Tile() { Type = 9 });
        }

        public override void OnResolutionChanged(int width, int height)
        {
            camera.Viewport = Main.SpriteBatch.GraphicsDevice.Viewport;
            tilemap.RecreateRenderTarget(Main.SpriteBatch.GraphicsDevice, width, height);
            grid.RecreateRenderTarget(Main.SpriteBatch.GraphicsDevice, width, height);
        }

        public override void PreDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            tilemap.Render(spriteBatch, camera);
            grid.Render(spriteBatch, camera);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var device = spriteBatch.GraphicsDevice;

            device.Clear(Color.DarkGray);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw(tilemap.RenderedTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw(grid.RenderedTexture, Vector2.Zero, null, new Color(240, 240, 240, 5), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();

            /*var mousePosition = Main.InputManager.GetMousePosition() + camera.Position;
            mousePosition.X = (int)(mousePosition.X / 32) * 32;
            mousePosition.Y = (int)(mousePosition.Y / 32) * 32;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.TransformMatrix);
            spriteBatch.Draw(TextureAssets.Pixel, mousePosition, new Rectangle(0, 0, 32, 32), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();*/

            var mousePosition = camera.ScreenToWorldSpace(Main.InputManager.GetMousePosition());
            var tileCoordsX = (int)(mousePosition.X / 32);
            var tileCoordsY = (int)(mousePosition.Y / 32);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Rendered tiles: {tilemap.RenderedTileCount}", new Vector2(5, 5), Color.White, 0, Vector2.Zero, 1f, 1f);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Tile coords [Mouse]: x:{tileCoordsX} y:{tileCoordsY}", new Vector2(5, 20), Color.White, 0, Vector2.Zero, 1f, 1f);
            spriteBatch.End();
        }
    }
}