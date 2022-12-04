using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Scenes
{
    public class SceneManager
    {
        public enum GameScenes
        {
            Splash,
            Menu,
            Settings,
            Game
        }

        // ...

        public bool CanChangeScene => expectedScene is null;

        // ...

        private Dictionary<GameScenes, Scene> scenes;
        private Scene current;

        private Texture2D pixel;
        private Effect effect;
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

        public void LoadContent(ContentManager content)
        {
            var e = content.Load<Texture2D>("Textures/Background");
            pixel = content.Load<Texture2D>("Textures/Pixel");
            effect = content.Load<Effect>("Effects/Background");
            //effect.Parameters["Texture1"].SetValue(e);

            foreach (var (_, scene) in scenes)
            {
                scene.LoadContent(content);
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

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise, effect);
            spriteBatch.Draw(pixel, new Rectangle((int)x, 0, Width, Height), Color.Black);
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