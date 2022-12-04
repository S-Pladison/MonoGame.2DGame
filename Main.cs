using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Enums;
using Pladi.Input;
using Pladi.Scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms;

namespace Pladi
{
    public class Main : Game
    {
        public static SceneManager SceneManager { get; private set; }
        public static InputManager InputManager { get; private set; }

        public static SpriteBatch SpriteBatch { get => instance.spriteBatch; }
        public static Random Rand { get; private set; }
        public static Point ScreenSize { get; private set; }

        private static Form Form { get => Form.FromHandle(instance.Window.Handle).FindForm(); }

        // ...

        private static readonly string settingsFilePath = "settings.json";
        private static readonly int minScreenWidth = 1024;
        private static readonly int minScreenHeight = 768;
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
            Form.MinimumSize = new System.Drawing.Size(minScreenWidth, minScreenHeight);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (obj, args) =>
            {
                OnResolutionChanged(Window.ClientBounds.Width, Window.ClientBounds.Height);
            };
        }

        protected override void Initialize()
        {
            // Не удалять // LoadContent()
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
            var settings = new Settings
            {
                FullScreen = graphics.IsFullScreen,
                ScreenWidth = GraphicsDevice.Viewport.Width,
                ScreenHeight = GraphicsDevice.Viewport.Height
            };

            using var fs = new FileStream(settingsFilePath, FileMode.Create);

            var options = new JsonSerializerOptions();
            options.WriteIndented = true;

            await JsonSerializer.SerializeAsync<Settings>(fs, settings, options);
        }

        public async void LoadConfig()
        {
            if (!File.Exists(settingsFilePath))
            {
                using var _ = new FileStream(settingsFilePath, FileMode.Create);
                return;
            }

            try
            {
                using var fs = new FileStream(settingsFilePath, FileMode.Open);

                var settings = await JsonSerializer.DeserializeAsync<Settings>(fs);

                if (settings.FullScreen && !graphics.IsFullScreen)
                {
                    ToggleFullScreen();
                }

                SetDisplayMode(settings.ScreenWidth, settings.ScreenHeight);
            }
            catch
            {
                File.Delete(settingsFilePath);
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

        public static void SetDisplayMode(int width, int height)
        {
            var graphics = instance.graphics;

            width = Math.Max(width, minScreenWidth);
            height = Math.Max(height, height);

            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();

            ScreenSize = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            OnResolutionChanged(width, height);
        }

        public static void ToggleFullScreen()
            => instance.graphics.ToggleFullScreen();

        public static List<Point> GetSupportedScreenResolutions()
        {
            var result = new List<Point>();
            var displayModes = instance.graphics.GraphicsDevice.Adapter.SupportedDisplayModes;

            foreach (var mode in displayModes)
            {
                if (mode.Width < minScreenWidth || mode.Height < minScreenHeight) continue;

                result.Add(new Point(mode.Width, mode.Height));
            }

            return result.OrderBy(o => o.X).ThenBy(o => o.Y).ToList();
        }

        public static void ExitFromGame()
            => instance.Exit();

        private static void OnResolutionChanged(int width, int height)
        {
            if (width <= 0 || height <= 0) return;

            SceneManager.OnResolutionChanged(width, height);
        }
    }
}