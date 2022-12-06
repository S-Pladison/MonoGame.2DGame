using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using System;
using System.Collections.Generic;

namespace Pladi.Scenes
{
    public class SceneManager
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

        // ...

        private Dictionary<GameScenes, Scene> scenes;
        private Scene current;

        private float initSceneProgress;
        private GameScenes? expectedScene;

        // ...

        public SceneManager()
        {
            scenes = new();
            scenes.Add(GameScenes.Splash, new SplashScene());
            scenes.Add(GameScenes.Menu, new MenuScene());
            scenes.Add(GameScenes.Settings, new SettingsScene());
            scenes.Add(GameScenes.Game, new LevelScene());
            scenes.Add(GameScenes.Editor, new EditorScene());

            current = scenes[GameScenes.Splash];
        }

        // ...

        public void Init()
        {
            foreach (var (_, scene) in scenes)
            {
                scene.Init();
            }
        }

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (expectedScene is not null)
            {
                initSceneProgress += 1f * delta;

                if (initSceneProgress >= 1f)
                {
                    expectedScene = null;
                }
                else if (initSceneProgress > 0.5f)
                {
                    current.OnDeactivate();
                    current = scenes[expectedScene.Value];
                    current.OnActivate();
                }
            }

            current.Update(gameTime);
        }

        public void OnResolutionChanged(int width, int height)
        {
            foreach (var (_, scene) in scenes)
            {
                scene.OnResolutionChanged(width, height);
            }
        }

        public void PreDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            current.PreDraw(gameTime, spriteBatch);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            current.Draw(gameTime, spriteBatch);

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

            if (!scenes.ContainsKey(gameScenes))
            {
                throw new Exception("...");
            }

            expectedScene = gameScenes;
            initSceneProgress = 0f;
        }
    }
}