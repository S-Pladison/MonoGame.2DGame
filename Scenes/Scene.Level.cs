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

        private Camera camera;
        private Vector2 cameraSmoothVelocity;

        private Player player;

        private Grid grid;

        // ...

        private const float CameraSmoothTime = 0.33f;

        // ...

        public override void Init()
        {
            grid = new Grid(32, 32);

            try
            {
                tilemap = Tilemap.LoadFromFile("Editor.pll");
                tilemap.SetTexture(TextureAssets.Tilemap, 8, 12);
            }
            catch
            {
                tilemap = new Tilemap(100, 10, 4f);
                tilemap.SetTexture(TextureAssets.Tilemap, 8, 12);

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
            }

            camera = new Camera(Main.SpriteBatch.GraphicsDevice.Viewport, 1f);
            player = new Player(TextureAssets.Player);
        }

        public override void OnActivate()
        {
            try
            {
                tilemap = Tilemap.LoadFromFile("Editor.pll");
                tilemap.SetTexture(TextureAssets.Tilemap, 8, 12);
            }
            catch
            {
                tilemap = new Tilemap(100, 10, 4f);
                tilemap.SetTexture(TextureAssets.Tilemap, 8, 12);

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
            }

            player.Position = new Vector2(0, 0);
            camera.Location = player.Center;
        }

        public override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Main.InputManager.JustPressed(Keys.Escape))
            {
                Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Menu);
            }

            if (Main.InputManager.IsPressed(Keys.NumPad8))
            {
                camera.Zoom += 0.5f * delta;
            }

            if (Main.InputManager.IsPressed(Keys.NumPad2))
            {
                camera.Zoom -= 0.5f * delta;
            }

            player.Update(delta, tilemap);
            camera.Location = PladiUtils.SmoothDamp(camera.Location, player.Center, ref cameraSmoothVelocity, CameraSmoothTime, delta);
        }

        public override void OnResolutionChanged(int width, int height)
        {
            camera.Viewport = Main.SpriteBatch.GraphicsDevice.Viewport;
            tilemap.RecreateRenderTarget(Main.SpriteBatch.GraphicsDevice, width, height);
        }

        public override void PreDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            tilemap.Render(spriteBatch, camera);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var device = spriteBatch.GraphicsDevice;

            device.Clear(Color.DarkGray);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw(tilemap.RenderedTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.TransformMatrix);
            player.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Rendered tiles: {tilemap.RenderedTileCount}", new Vector2(5, 5), Color.White, 0, Vector2.Zero, 1f, 2f);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Player position: {player.Position}", new Vector2(5, 20), Color.White, 0, Vector2.Zero, 1f, 2f);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Player velocity: {player.Velocity}", new Vector2(5, 35), Color.White, 0, Vector2.Zero, 1f, 2f);
            spriteBatch.End();
        }
    }
}