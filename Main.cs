using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Input;
using Pladi.Scenes;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace Pladi
{
    public partial class Main : Game
    {
        public static SceneManager SceneManager { get; private set; }
        public static InputManager InputManager { get; private set; }

        public static bool IsGameActive { get => instance.IsActive; }
        public static SpriteBatch SpriteBatch { get => instance.spriteBatch; }
        public static Random Rand { get; private set; }
        public static Point ScreenSize { get; private set; }

        // ...

        private static readonly string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\config.json";
        private static readonly int minScreenWidth = 800;
        private static readonly int minScreenHeight = 500;
        private static bool windowMaximized;
        private static Main instance;

        // ...

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // ...

        public Main()
        {
            instance = this;

            SceneManager ??= new SceneManager();
            InputManager ??= new InputManager();

            Rand ??= new Random((int)DateTime.Now.Ticks);

            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            IsFixedTimeStep = false;

            var form = GetForm();
            form.MinimumSize = new System.Drawing.Size(minScreenWidth, minScreenHeight);

            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            FontAssets.Load(Content);
            TextureAssets.Load(Content);
            EffectAssets.Load(Content);
        }

        protected override void Initialize()
        {
            // Not remove // LoadContent()
            base.Initialize();

            LoadConfig();

            SceneManager.Init();
        }

        protected override void UnloadContent()
        {
            EffectAssets.Unload();
            TextureAssets.Unload();
            FontAssets.Unload();

            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            Window.Title = GetForm().WindowState.ToString() + " | " + graphics.PreferredBackBufferWidth + "x" + graphics.PreferredBackBufferHeight + " | " + windowMaximized;

            try
            {
                CheckWindowSize();

                base.Update(gameTime);

                InputManager.Update(gameTime);
                SceneManager.Update(gameTime);
            }
            catch (Exception)
            {
                // TODO: ...
                throw;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            try
            {
                SceneManager.PreDraw(gameTime, spriteBatch);

                base.Draw(gameTime);

                SceneManager.Draw(gameTime, spriteBatch);
            }
            catch (Exception)
            {
                // TODO: ...
                throw;
            }

            DrawMouse();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            SaveConfig();
        }

        // ...

        private async void SaveConfig()
        {
            var config = new Config();

            var screenConfig = config.Screen;
            screenConfig.Width = ScreenSize.X;
            screenConfig.Height = ScreenSize.Y;
            screenConfig.Fullscreen = graphics.IsFullScreen;
            screenConfig.WindowMaximized = windowMaximized;

            using var fs = new FileStream(configFilePath, FileMode.Create);

            await JsonSerializer.SerializeAsync<Config>(fs, config, new JsonSerializerOptions() { WriteIndented = true });
        }

        private async void LoadConfig()
        {
            try
            {
                using var fs = new FileStream(configFilePath, FileMode.Open);

                var form = GetForm();
                var config = await JsonSerializer.DeserializeAsync<Config>(fs);
                var screenConfig = config.Screen;

                if (screenConfig.WindowMaximized)
                {
                    form.WindowState = FormWindowState.Maximized;
                }

                SetDisplayMode(screenConfig.Width, screenConfig.Height, screenConfig.Fullscreen);
            }
            catch
            {
                File.Delete(configFilePath);
            }
        }

        private void DrawMouse()
        {
            if (!IsActive) return;

            spriteBatch.Begin();
            spriteBatch.Draw(TextureAssets.Cursor, InputManager.GetMousePosition(), null, Color.White, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        // ...

        public static void ExitFromGame()
            => instance.Exit();

        private static Form GetForm()
            => Form.FromHandle(instance.Window.Handle).FindForm();
    }
}