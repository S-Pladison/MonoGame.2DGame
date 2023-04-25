using Microsoft.Xna.Framework;
using Pladi.Core;
using Pladi.Core.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Pladi
{
    public partial class Main : Game
    {
        public static void ToggleFullScreen()
            => SetDisplayMode(ScreenSize.X, ScreenSize.Y, !Instance.graphics.IsFullScreen);

        public static void SetDisplayMode(int width, int height)
            => SetDisplayMode(width, height, Instance.graphics.IsFullScreen);

        public static void SetDisplayMode(int width, int height, bool fullscreen)
        {
            var graphics = Instance.graphics;
            var form = GetForm();

            width = Math.Max(width, MinScreenWidth);
            height = Math.Max(height, MinScreenHeight);

            windowMaximized = form.WindowState.HasFlag(FormWindowState.Maximized);

            if (!graphics.IsFullScreen && fullscreen)
            {
                form.WindowState = FormWindowState.Normal;
            }

            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();

            if (graphics.IsFullScreen != fullscreen)
            {
                graphics.ToggleFullScreen();
            }

            ScreenSize = new Point(width, height);

            OnResolutionChanged(width, height);
        }

        public static List<Point> GetSupportedScreenResolutions()
        {
            var result = new List<Point>();
            var displayModes = Instance.graphics.GraphicsDevice.Adapter.SupportedDisplayModes;

            foreach (var mode in displayModes)
            {
                if (mode.Width < MinScreenWidth || mode.Height < MinScreenHeight) continue;

                result.Add(new Point(mode.Width, mode.Height));
            }

            return result.OrderBy(o => o.X).ThenBy(o => o.Y).ToList();
        }

        private static void OnResolutionChanged(int width, int height)
        {
            if (width <= 0 || height <= 0) return;

            ILoadable.GetInstance<SceneComponent>().OnResolutionChanged(width, height);
        }

        private static void CheckWindowSize()
        {
            int width = Instance.graphics.PreferredBackBufferWidth;
            int height = Instance.graphics.PreferredBackBufferHeight;

            if (ScreenSize.X != width || ScreenSize.Y != height)
            {
                SetDisplayMode(width, height);
            }
        }
    }
}