using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Content;
using Pladi.Core;
using Pladi.Core.Graphics.Renderers;
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
        // [public static properties and fields]

        public static Main Instance { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static Random Rand { get; private set; }
        public static float GlobalTimeWrappedHourly { get; private set; }
        public static float DeltaTime { get; private set; }

        // [private static properties and fields]

        private static readonly string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\config.json";

        // [public properties and fields]

        public Action<GameTime> OnPreDraw;
        public Action<GameTime> OnPostDraw;

        // [constructors]

        public Main()
        {
            Instance = this;

            Graphics = new GraphicsDeviceManager(this)
            {
                PreferHalfPixelOffset = true,
            };

            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;

            Rand = new Random((int)DateTime.Now.Ticks);

            Graphics.SynchronizeWithVerticalRetrace = false;
            Graphics.ApplyChanges();
        }

        // [protected methods]

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            FontAssets.Load(Content);
            TextureAssets.Load(Content);
            EffectAssets.Load(Content);
            AudioAssets.Load(Content);
        }

        protected override void Initialize()
        {
            // Not remove // LoadContent()
            base.Initialize();

            LoadLoadables();
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

            try
            {
                base.Update(gameTime);
            }
            catch (Exception ex)
            {
                // TODO: ...
                throw new Exception(ex.Message);
            }
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

        // [private static methods]

        private static async void SaveConfig()
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

        private static async void LoadConfig()
        {
            var screen = ILoadable.GetInstance<ScreenComponent>();

            try
            {
                using var fs = new FileStream(configFilePath, FileMode.Open);

                var form = GetForm();
                var config = await JsonSerializer.DeserializeAsync<ConfigData>(fs);
                var screenConfig = config.Screen;

                //screen.SetDisplayMode(screenConfig.Width, screenConfig.Height, screenConfig.Fullscreen, screenConfig.WindowMaximized);
                screen.SetDisplayMode(screenConfig.Width, screenConfig.Height);
            }
            catch
            {
                File.Delete(configFilePath);

                screen.SetMinDisplayMode();
            }
        }

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
        {
            return Form.FromHandle(Instance.Window.Handle).FindForm();
        }
    }
}