using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using System;

namespace Pladi.Core.Scenes
{
    public class SceneComponent : BasicComponent
    {
        public enum GameScenes
        {
            Splash,
            Menu,
            Settings,
            Game,
            Editor
        }

        // ...

        public bool CanChangeScene => expectedScene is null;
        public bool CanUpdateAndDrawScene => initSceneProgress >= 1f;
        public Scene Scene => current;

        // ...

        private Scene current;
        private float initSceneProgress;
        private GameScenes? expectedScene;

        // ...

        public override void Initialize()
        {
            current = CreateScene(GameScenes.Splash);
        }

        public override void Update(GameTime gameTime)
        {
            if (expectedScene is not null)
            {
                initSceneProgress += 1f * Main.DeltaTime;

                if (initSceneProgress >= 1f)
                {
                    expectedScene = null;
                }
                else if (initSceneProgress > 0.5f)
                {
                    current.OnDeactivate();
                    current = CreateScene(expectedScene.Value);
                    current.OnActivate();
                }
            }

            current.Update();
        }

        public void OnResolutionChanged(int width, int height)
        {
            current.OnResolutionChanged(width, height);
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = Main.SpriteBatch;

            current.Draw(spriteBatch);

            if (expectedScene is null) return;

            var Width = spriteBatch.GraphicsDevice.Viewport.Width;
            var Height = spriteBatch.GraphicsDevice.Viewport.Height;

            var x = Width * (1 - initSceneProgress) * 2 - Width;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise, EffectAssets.Background);
            spriteBatch.Draw(TextureAssets.Pixel, new Rectangle((int)x, 0, Width, Height), Color.Black);
            spriteBatch.End();
        }

        public void SetActiveScene(GameScenes gameScenes)
        {
            if (!CanChangeScene) return;

            expectedScene = gameScenes;
            initSceneProgress = 0f;
        }

        // ...

        private static Scene CreateScene(GameScenes gameScene)
        {
            return gameScene switch
            {
                GameScenes.Editor => new EditorScene(),
                GameScenes.Game => new LevelScene(),
                GameScenes.Menu => new MenuScene(),
                GameScenes.Settings => new SettingsScene(),
                GameScenes.Splash => new SplashScene(),
                _ => throw new Exception("..."),
            };
        }
    }
}