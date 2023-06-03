using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using System;
using System.Collections.Generic;

namespace Pladi.Core.Scenes
{
    public class SceneComponent : BaseComponent
    {
        public bool CanChangeScene => expectedScene is null;
        public bool CanUpdateAndDrawScene => initSceneProgress >= 1f;
        public Scene Scene => current;

        // ...

        private bool sceneWasChanged;
        private Scene current;
        private Scene expectedScene;
        private float initSceneProgress;
        private Dictionary<Type, Scene> sceneInstances;

        // ...

        public SceneComponent()
        {
            sceneInstances = new Dictionary<Type, Scene>();

            AddSceneInstance<EditorScene>();
            AddSceneInstance<LevelScene>();
            AddSceneInstance<LevelMenuScene>();
            AddSceneInstance<MenuScene>();
            AddSceneInstance<SettingsScene>();
            AddSceneInstance<SplashScene>();

            current = sceneInstances[typeof(SplashScene)];
        }

        // ...

        public override void Update(GameTime gameTime)
        {
            if (expectedScene is not null)
            {
                initSceneProgress += 1f * Main.DeltaTime;

                if (initSceneProgress >= 1f)
                {
                    expectedScene = null;
                }
                else if (initSceneProgress > 0.5f && !sceneWasChanged)
                {
                    current.OnDeactivate();
                    current = sceneInstances[expectedScene.GetType()];
                    sceneWasChanged = true;
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

        public void SetActiveScene<T>() where T : Scene
        {
            if (!CanChangeScene) return;

            sceneWasChanged = false;
            expectedScene = sceneInstances[typeof(T)];
            initSceneProgress = 0f;
        }

        public Scene GetSceneInstance<T>() where T : Scene
        {
            return sceneInstances[typeof(T)];
        }

        private void AddSceneInstance<T>() where T : Scene
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            instance.SceneComponent = this;
            sceneInstances[typeof(T)] = instance;
        }
    }
}