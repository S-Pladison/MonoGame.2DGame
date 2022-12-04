using Microsoft.Xna.Framework;
using Pladi.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Pladi
{
    public partial class Main : Game
    {
        public static void SetDisplayMode(int width, int height)
            => SetDisplayMode(width, height, instance.graphics.IsFullScreen);

        public static void SetDisplayMode(int width, int height, bool fullscreen)
        {
            var graphics = instance.graphics;
            var form = GetForm();

            width = Math.Max(width, minScreenWidth);
            height = Math.Max(height, height);
            windowMaximized = form.WindowState == FormWindowState.Maximized;

            int width2, height2;
            bool flag = false;

            if (windowMaximized || graphics.IsFullScreen || fullscreen)
            {
                if (!graphics.IsFullScreen)
                {
                    width2 = Math.Max(graphics.PreferredBackBufferWidth, graphics.GraphicsDevice.Viewport.Width);
                    height2 = Math.Max(graphics.PreferredBackBufferHeight, graphics.GraphicsDevice.Viewport.Height);

                    if (width2 != graphics.PreferredBackBufferWidth || height2 != graphics.PreferredBackBufferHeight)
                    {
                        flag = true;
                    }
                }
                else
                {
                    width2 = graphics.PreferredBackBufferWidth;
                    height2 = graphics.PreferredBackBufferHeight;
                }
            }
            else
            {
                width2 = graphics.GraphicsDevice.Viewport.Width;
                height2 = graphics.GraphicsDevice.Viewport.Height;

                flag = graphics.PreferredBackBufferWidth != graphics.GraphicsDevice.Viewport.Width || graphics.PreferredBackBufferHeight != graphics.GraphicsDevice.Viewport.Height;
            }

            if (!fullscreen && !flag)
            {
                if (form.ClientSize.Width < graphics.PreferredBackBufferWidth)
                {
                    width = form.ClientSize.Width;
                    flag = true;
                }

                if (form.ClientSize.Height < graphics.PreferredBackBufferHeight)
                {
                    height = form.ClientSize.Height;
                    flag = true;
                }
            }

            if (graphics.IsFullScreen != fullscreen)
            {
                graphics.PreferredBackBufferWidth = width;
                graphics.PreferredBackBufferHeight = height;
                graphics.ApplyChanges();
                graphics.ToggleFullScreen();
            }

            if (width != width2 || height != height2 || flag)
            {
                graphics.PreferredBackBufferWidth = width;
                graphics.PreferredBackBufferHeight = height;

                if (width != width2 || height != height2)
                {
                    graphics.ApplyChanges();
                }

                if (!fullscreen)
                {
                    form.FormBorderStyle = FormBorderStyle.Sizable;
                    form.BringToFront();
                }
            }

            ScreenSize = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            OnResolutionChanged(width, height);
        }

        public static void ToggleFullScreen()
            => SetDisplayMode(ScreenSize.X, ScreenSize.Y, !instance.graphics.IsFullScreen);

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

        private static void OnResolutionChanged(int width, int height)
        {
            if (width <= 0 || height <= 0) return;

            SceneManager.OnResolutionChanged(width, height);
        }
    }
}