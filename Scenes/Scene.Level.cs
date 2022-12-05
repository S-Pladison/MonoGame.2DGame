using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Enitites;
using Pladi.Tiles;
using Pladi.Utils;

namespace Pladi.Scenes
{
    public class LevelScene : Scene
    {
        private Tilemap tilemap;
        private Tilemap wallMap;

        private Camera camera;
        private Vector2 cameraSmoothVelocity;

        private Player player;

        private GridDrawer grid;

        // ...

        private const float CameraSmoothTime = 0.33f;

        // ...

        public override void Init()
        {
            grid = new GridDrawer(32, 32);

            tilemap = new Tilemap(100, 10, 4f);
            tilemap.SetTexture(TextureAssets.Tilemap, 8, 12);

            wallMap = new Tilemap(100, 10, 4f);
            wallMap.SetTexture(TextureAssets.Tilemap, 8, 12);

            tilemap.SetTile(0, 3, new Tile() { Type = 9 });
            tilemap.SetTile(1, 3, new Tile() { Type = 9 });
            for (int i = 5; i < 90; i++)
            {
                tilemap.SetTile(i, 4, new Tile() { Type = 9 });
            }
            for (int i = 19; i < 90; i++)
            {
                tilemap.SetTile(i, 3, new Tile() { Type = 10 });
            }
            for (int i = 20; i < 90; i++)
            {
                tilemap.SetTile(i, 2, new Tile() { Type = 12 });
            }
            for (int i = 20; i < 90; i++)
            {
                tilemap.SetTile(i, 1, new Tile() { Type = 12 });
            }
            for (int i = 20; i < 90; i++)
            {
                tilemap.SetTile(i, 0, new Tile() { Type = 12 });
            }

            camera = new Camera(Main.SpriteBatch.GraphicsDevice.Viewport, 1f);
            player = new Player(TextureAssets.Player);
        }

        public override void OnActivate()
        {
            player.Position = new Vector2(32 * 7, 32 * 2);
            camera.Center = player.Center;
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1))
            {
                Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Menu);
            }

            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.Update(delta, tilemap);
            camera.Center = PladiUtils.SmoothDamp(camera.Center, player.Center, ref cameraSmoothVelocity, CameraSmoothTime, delta);
        }

        public override void OnResolutionChanged(int width, int height)
        {
            tilemap.RecreateRenderTarget(Main.SpriteBatch.GraphicsDevice, width, height);
            camera.Viewport = Main.SpriteBatch.GraphicsDevice.Viewport;
        }

        public override void PreDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            wallMap.Render(spriteBatch, camera);
            tilemap.Render(spriteBatch, camera);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var device = spriteBatch.GraphicsDevice;

            device.Clear(Color.DarkGray);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            //spriteBatch.Draw(t, Vector2.Zero, Color.White);
            //spriteBatch.Draw(wallMap.RenderedTexture, new Vector2(0, 32), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(tilemap.RenderedTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            //spriteBatch.Draw(grid.RenderedTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.TransformMatrix);

            var gridColor = new Color(240, 240, 240, 5);

            for (float x = 0; x < tilemap.Width; x++)
            {
                Rectangle rectangle = new Rectangle((int)(x * 32) - 1, 0, 2, 1000);
                spriteBatch.Draw(TextureAssets.Pixel, rectangle, gridColor);
            }

            for (float y = 0; y < tilemap.Height; y++)
            {
                Rectangle rectangle = new Rectangle(0, (int)(y * 32) - 1, 1000, 2);
                spriteBatch.Draw(TextureAssets.Pixel, rectangle, gridColor);
            }

            player.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            //var x = tileMap.TileCollision(Main.MousePosition + camera.Position, 0, 0);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Camera position: {camera.Position}", new Vector2(5, 5), Color.White, 0, Vector2.Zero, 1f, 1f);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Player position: {player.Position}", new Vector2(5, 20), Color.White, 0, Vector2.Zero, 1f, 1f);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Player velocity: {player.Velocity}", new Vector2(5, 35), Color.White, 0, Vector2.Zero, 1f, 1f);

            spriteBatch.End();
        }
    }
}