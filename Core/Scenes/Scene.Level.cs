using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.Entities;
using Pladi.Core.UI;
using Pladi.Core.UI.Elements;
using System.Collections.Generic;

namespace Pladi.Core.Scenes
{
    public class LevelScene : Scene
    {
        private GraphicalUI graphicalUI;
        private List<Entity> entities;
        private Player player;

        // ...

        public LevelScene()
        {
            graphicalUI = new();
            entities = new();

            InitPlayer();
            InitEntities();
            InitUI();
        }

        // ...

        private void InitPlayer()
        {
            player = new()
            {
                Position = new Vector2(-15, -45),
                Width = 30,
                Height = 30
            };
        }

        private void InitEntities()
        {
            entities.Add(new Box()
            {
                Position = new Vector2(-50, 0),
                Width = 150,
                Height = 50,
                Color = Color.Red
            });

            entities.Add(new Box()
            {
                Position = new Vector2(100, 0),
                Width = 50,
                Height = 50,
                Color = Color.IndianRed
            });

            entities.Add(new Box()
            {
                Position = new Vector2(-100, 0),
                Width = 50,
                Height = 50,
                Color = Color.OrangeRed
            });

            entities.Add(new Box()
            {
                Position = new Vector2(-100, -50),
                Width = 50,
                Height = 50,
                Color = Color.DarkOrange
            });

            entities.Add(new Box()
            {
                Position = new Vector2(-100, -100),
                Width = 50,
                Height = 50,
                Color = Color.MonoGameOrange
            });
        }

        private void InitUI()
        {
            var panel = new PanelUIElement();
            panel.Width.SetPixel(100f);
            panel.Height.SetPixel(55f);
            panel.Left.SetPixel(10f);
            panel.Top.SetPixel(10f);
            graphicalUI.Append(panel);

            var text = new TextUIElement();
            text.OnPostUpdate += (elem) => (elem as TextUIElement).Text = $"" +
                $"FPS: {(int)Main.FrameCounter.AverageFramesPerSecond}\n" +
                $"Entities: {entities.Count}";
            text.Left.SetPixel(10f);
            text.Top.SetPixel(10f);
            panel.Append(text);
        }

        public override void Update()
        {
            UpdateEntities();

            graphicalUI.Update();
        }

        private void UpdateEntities()
        {
            foreach (var entity in entities.ToArray())
            {
                entity.Update();
            }

            player.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var device = spriteBatch.GraphicsDevice;
            device.Clear(Color.DarkGray);

            DrawEntities(spriteBatch);
            DrawUI(spriteBatch);
        }

        private void DrawEntities(SpriteBatch spriteBatch)
        {
            var matrix = Matrix.CreateTranslation(Main.ScreenSize.X / 2, Main.ScreenSize.Y / 2, 0);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, matrix);

            foreach (var entity in entities)
            {
                entity.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawUI(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            graphicalUI.Draw(spriteBatch);

            spriteBatch.End();
        }

        /*
        private GameLevel level;

        private Camera camera;
        private Vector2 cameraSmoothVelocity;

        private Player player;

        private Grid grid;

        // ...

        private const float CameraSmoothTime = 0.5f;

        // ...

        public override void OnActivate()
        {
            grid = new Grid(32, 32);
            camera = new Camera(Main.SpriteBatch.GraphicsDevice.Viewport, 1f);
            player = new Player(TextureAssets.Player);

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

            DrawTempInfo(spriteBatch);
        }

        private void DrawTempInfo(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Position: {player.Position}", new Vector2(5, 5), Color.White, 0, Vector2.Zero, 1f, 1f);
            spriteBatch.DrawStringWithShadow(FontAssets.DefaultSmall, $"Velocity: {player.Velocity}", new Vector2(5, 20), Color.White, 0, Vector2.Zero, 1f, 1f);

            spriteBatch.End();
        }*/
    }
}