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
        private GameLevel level;

        private Camera camera;
        private Vector2 cameraSmoothVelocity;

        private Player player;

        private Grid grid;

        // ...

        private const float CameraSmoothTime = 0.5f;

        // ...

        public override void Init()
        {
            grid = new Grid(32, 32);

            /*try
            {
                /*tilemap = Tilemap.LoadFromFile("Editor.pll");
                tilemap.SetTexture(TextureAssets.Tilemap, 8, 12);
            }
            catch
            {*/

            try
            {
                level = GameLevel.LoadFromFile("Editor.pgm");
                level.BackTilemap.SetTexture(TextureAssets.Tilemap, 8, 12);
                level.CollisionTilemap.SetTexture(TextureAssets.CollisionTilemap, 2, 1);
            }
            catch
            {
                level = new GameLevel(100, 100, 4f);
            }

            /*}*/

            camera = new Camera(Main.SpriteBatch.GraphicsDevice.Viewport, 1f);
            player = new Player(TextureAssets.Player);
        }

        public override void OnActivate()
        {
            try
            {
                level = GameLevel.LoadFromFile("Editor.pgm");
                level.BackTilemap.SetTexture(TextureAssets.Tilemap, 8, 12);
                level.CollisionTilemap.SetTexture(TextureAssets.CollisionTilemap, 2, 1);
            }
            catch
            {
                level = new GameLevel(100, 100, 4f);
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

            player.Update(delta, level.CollisionTilemap);
            camera.Location = PladiUtils.SmoothDamp(camera.Location, player.Center, ref cameraSmoothVelocity, CameraSmoothTime, delta);
        }

        public override void OnResolutionChanged(int width, int height)
        {
            camera.Viewport = Main.SpriteBatch.GraphicsDevice.Viewport;
            level.RecreateRenderTargets(Main.SpriteBatch.GraphicsDevice, width, height);
        }

        public override void PreDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            level.Render(spriteBatch, camera);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var device = spriteBatch.GraphicsDevice;

            device.Clear(Color.DarkGray);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, camera.TransformMatrix);
            level.BackTilemap.Draw(spriteBatch, Vector2.Zero);
            player.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}