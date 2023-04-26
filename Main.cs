using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core;
using Pladi.Core.Graphics.Renderers;
using Pladi.Core.Input;
using Pladi.Core.Scenes;
using Pladi.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;

namespace Pladi
{
    public class Main : Game
    {
        public static Main Instance { get; private set; }

        public static SpriteBatch SpriteBatch { get => Instance.spriteBatch; }
        public static Random Rand { get; private set; }
        public static float GlobalTimeWrappedHourly { get; private set; }
        public static float DeltaTime { get; private set; }

        public Action<GameTime> OnPreDraw;
        public Action<GameTime> OnPostDraw;

        // ...

        private static readonly string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\config.json";

        // ...

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // ...

        public Main()
        {
            Instance = this;

            graphics = new GraphicsDeviceManager(this)
            {
                PreferHalfPixelOffset = true,
            };

            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;

            Rand = new Random((int)DateTime.Now.Ticks);

            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();

            LoadLoadables();

            ILoadable.GetInstance<ScreenComponent>().SetGraphicsDeviceManager(graphics);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            FontAssets.Load(Content);
            TextureAssets.Load(Content);
            EffectAssets.Load(Content);
            AudioAssets.Load(Content);
        }

        protected override void Initialize()
        {
            // Not remove // LoadContent()
            base.Initialize();

            LoadConfig();
        }

        protected override void UnloadContent()
        {
            AudioAssets.Unload();
            EffectAssets.Unload();
            TextureAssets.Unload();
            FontAssets.Unload();

            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            //Window.Title = Form.FromHandle(Window.Handle).FindForm().WindowState.ToString() + " | " + graphics.PreferredBackBufferWidth + "x" + graphics.PreferredBackBufferHeight + " | " + windowMaximized;
            GlobalTimeWrappedHourly = (float)(gameTime.TotalGameTime.TotalSeconds % 3600.0);
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);

            /*try
            {
                base.Update(gameTime);
            }
            catch (Exception ex)
            {
                // TODO: ...
                throw new Exception(ex.Message);
            }*/
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            try
            {
                OnPreDraw?.Invoke(gameTime);

                ILoadable.GetInstance<RendererHandler>().Render(gameTime);

                base.Draw(gameTime);

                OnPostDraw?.Invoke(gameTime);
            }
            catch (Exception)
            {
                // TODO: ...
                throw;
            }
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            SaveConfig();
        }

        // ...

        private async void SaveConfig()
        {
            var config = new ConfigData();

            var screen = ILoadable.GetInstance<ScreenComponent>();
            var screenConfig = config.Screen;
            screenConfig.Width = screen.Width;
            screenConfig.Height = screen.Height;
            //screenConfig.Fullscreen = screen.IsFullscreen;
            //screenConfig.WindowMaximized = screen.IsMaximized;

            using var fs = new FileStream(configFilePath, FileMode.Create);

            await JsonSerializer.SerializeAsync<ConfigData>(fs, config, new JsonSerializerOptions() { WriteIndented = true });
        }

        private async void LoadConfig()
        {
            var screen = ILoadable.GetInstance<ScreenComponent>();

            try
            {
                using var fs = new FileStream(configFilePath, FileMode.Open);

                var form = GetForm();
                var config = await JsonSerializer.DeserializeAsync<ConfigData>(fs);
                var screenConfig = config.Screen;

                screen.SetDisplayMode(screenConfig.Width, screenConfig.Height, screenConfig.Fullscreen, screenConfig.WindowMaximized);
            }
            catch
            {
                File.Delete(configFilePath);

                screen.SetMinDisplayMode();
            }
        }

        // ...

        private static void LoadLoadables()
        {
            var queue = new PriorityQueue<ILoadable, float>();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsAbstract ||
                    type.ContainsGenericParameters ||
                    type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) is null ||
                    !type.IsAssignableTo(typeof(ILoadable))) continue;

                var instance = Activator.CreateInstance(type) as ILoadable;

                queue.Enqueue(instance, instance.LoadOrder);
            }

            while (queue.TryDequeue(out var instance, out var _))
            {
                ILoadable.AddInstance(instance);
            }

            ILoadable.LoadInstances();
        }

        private static Form GetForm()
            => Form.FromHandle(Instance.Window.Handle).FindForm();
    }
}