using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Tiles;
using Pladi.Utils;
using System;

namespace Pladi.Scenes
{
    public class EditorScene : Scene
    {
        private Tilemap tilemap;
        private Grid grid;
        private Camera camera;

        private int currentTileType;

        // ...

        public override void Init()
        {
            try
            {
                tilemap = Tilemap.LoadFromFile("Editor.pll");
                tilemap.SetTexture(TextureAssets.Tilemap, 8, 12);
            }
            catch
            {
                tilemap = new Tilemap(100, 100, 4f);
                tilemap.SetTexture(TextureAssets.Tilemap, 8, 12);
            }

            grid = new Grid(32, 32, 2);

            camera = new Camera(Main.SpriteBatch.GraphicsDevice.Viewport, 1f);
        }

        public override void OnDeactivate()
        {
            tilemap.SaveToFile("Editor.pll");
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

            if (input.JustPressed(Keys.NumPad7))
            {
                currentTileType++;
            }

            if (input.JustPressed(Keys.NumPad4))
            {
                currentTileType--;
            }

            var scroll = Main.InputManager.GetMouseScroll();

            if (scroll != 0)
            {
                camera.Zoom += Math.Sign(scroll) * 5f * delta;
            }

            if (!input.JustPressed(Input.MouseInputTypes.LeftButton)) return;

            var mousePosition = camera.ScreenToWorldSpace(Main.InputManager.GetMousePosition());
            var tileCoordsX = (int)(mousePosition.X / 32);
            var tileCoordsY = (int)(mousePosition.Y / 32);

            if (tileCoordsX < 0 || tileCoordsX >= tilemap.Width || tileCoordsY < 0 || tileCoordsY >= tilemap.Height) return;

            tilemap.SetTile(tileCoordsX, tileCoordsY, new Tile() { Type = (ushort)currentTileType });
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

            var mousePosition = camera.ScreenToWorldSpace(Main.InputManager.GetMousePosition());
            var tileCoordsX = (int)(mousePosition.X / 32);
            var tileCoordsY = (int)(mousePosition.Y / 32);

            device.Clear(Color.DarkGray);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw(tilemap.RenderedTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw(grid.RenderedTexture, Vector2.Zero, null, new Color(240, 240, 240, 5), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.TransformMatrix);
            {
                var drawPosition = new Vector2(tileCoordsX, tileCoordsY) * 32;
                spriteBatch.Draw(TextureAssets.Pixel, drawPosition + new Vector2(-1, 1), new Rectangle(0, 0, 2, 32), new Color(240, 240, 240, 90), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(TextureAssets.Pixel, drawPosition + new Vector2(-1, -1), new Rectangle(0, 0, 32, 2), new Color(240, 240, 240, 90), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(TextureAssets.Pixel, drawPosition + new Vector2(-1, -1) + new Vector2(32, 0), new Rectangle(0, 0, 2, 32), new Color(240, 240, 240, 90), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(TextureAssets.Pixel, drawPosition + new Vector2(1, -1) + new Vector2(0, 32), new Rectangle(0, 0, 32, 2), new Color(240, 240, 240, 90), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            spriteBatch.End();

            /*var mousePosition = Main.InputManager.GetMousePosition() + camera.Position;
            mousePosition.X = (int)(mousePosition.X / 32) * 32;
            mousePosition.Y = (int)(mousePosition.Y / 32) * 32;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.TransformMatrix);
            spriteBatch.Draw(TextureAssets.Pixel, mousePosition, new Rectangle(0, 0, 32, 32), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();*/

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Rendered tiles: {tilemap.RenderedTileCount}", new Vector2(5, 5), Color.White, 0, Vector2.Zero, 1f, 1f);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Tile coords [Mouse]: x:{tileCoordsX} y:{tileCoordsY}", new Vector2(5, 20), Color.White, 0, Vector2.Zero, 1f, 1f);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Tile type: {currentTileType}", new Vector2(5, 35), Color.White, 0, Vector2.Zero, 1f, 1f);
            spriteBatch.End();
        }
    }
}