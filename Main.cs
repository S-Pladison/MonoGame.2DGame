using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Input;
using Pladi.Scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace Pladi
{
    public partial class Main : Game
    {
        public static SceneManager SceneManager { get; private set; }
        public static InputManager InputManager { get; private set; }

        public static SpriteBatch SpriteBatch { get => instance.spriteBatch; }
        public static Random Rand { get; private set; }
        public static Point ScreenSize { get; private set; }

        // ...

        private static readonly string configFilePath = "config.json";
        private static readonly int minScreenWidth = 800;
        private static readonly int minScreenHeight = 720;
        private static bool windowMaximized;
        private static Main instance;

        // ...

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D mouseTexture;

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
            Window.ClientSizeChanged += (obj, args) =>
            {
                SetDisplayMode(Window.ClientBounds.Width, Window.ClientBounds.Height);
            };
        }

        protected override void Initialize()
        {
            // Not remove // LoadContent()
            base.Initialize();

            LoadConfig();

            SceneManager.Init();
        }

        protected override void LoadContent()
        {
            FontAssets.Load(Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            mouseTexture = Content.Load<Texture2D>("Textures/Cursor");

            SceneManager.LoadContent(Content);
        }

        protected override void UnloadContent()
        {
            FontAssets.Unload(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            Window.Title = DateTime.Now.ToString();

            try
            {
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

        public async void SaveConfig()
        {
            var config = new Config();

            var screenConfig = config.Screen;
            screenConfig.Width = graphics.PreferredBackBufferWidth;
            screenConfig.Height = graphics.PreferredBackBufferHeight;
            screenConfig.Fullscreen = graphics.IsFullScreen;
            screenConfig.WindowMaximized = windowMaximized;

            using var fs = new FileStream(configFilePath, FileMode.Create);

            await JsonSerializer.SerializeAsync<Config>(fs, config, new JsonSerializerOptions() { WriteIndented = true });
        }

        public async void LoadConfig()
        {
            if (!File.Exists(configFilePath))
            {
                using var _ = new FileStream(configFilePath, FileMode.Create);
                return;
            }

            try
            {
                using var fs = new FileStream(configFilePath, FileMode.Open);

                var form = GetForm();
                var config = await JsonSerializer.DeserializeAsync<Config>(fs);
                var screenConfig = config.Screen;

                if (screenConfig.WindowMaximized)
                {
                    form.FormBorderStyle = FormBorderStyle.Sizable;
                    form.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    form.FormBorderStyle = FormBorderStyle.Sizable;
                }

                form.BringToFront();

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
            spriteBatch.Draw(mouseTexture, InputManager.GetMousePosition(), null, Color.White, 0f, Vector2.Zero, 0.35f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        // ...

        public static void ExitFromGame()
            => instance.Exit();

        private static Form GetForm()
            => Form.FromHandle(instance.Window.Handle).FindForm();
    }
}