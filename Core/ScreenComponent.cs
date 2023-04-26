using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XnaKeys = Microsoft.Xna.Framework.Input.Keys;

namespace Pladi.Core
{
    public class ScreenComponent : BasicComponent
    {
        // [public static properties and fields]

        public static readonly int MinWidth;
        public static readonly int MinHeight;

        // [static constructors]

        static ScreenComponent()
        {
            MinWidth = 800;
            MinHeight = 500;
        }

        // [public properties and fields]

        public int Width
        {
            get => width;
            set
            {
                width = value;
                OnResolutionChanged?.Invoke(width, height);
            }
        }

        public int Height
        {
            get => height;
            set
            {
                height = value;
                OnResolutionChanged?.Invoke(width, height);
            }
        }

        public Point Size
        {
            get => new(Width, Height);
            set
            {
                width = value.X;
                height = value.Y;
                OnResolutionChanged?.Invoke(width, height);
            }
        }

        public bool IsMaximized { get; private set; }
        public bool IsFullscreen { get; private set; }

        public Action<int, int> OnResolutionChanged;

        // [private properties and fields]

        private Form GameForm
        {
            get
            {
                try
                {
                    return Form.FromHandle(Game.Window.Handle)?.FindForm();
                }
                catch
                {
                    return null;
                }
            }
        }

        private int width;
        private int height;
        private GraphicsDeviceManager graphics;

        // [public methods]

        public override void Initialize()
        {
            width = graphics.PreferredBackBufferWidth;
            height = graphics.PreferredBackBufferHeight;

            GameForm.MinimumSize = new System.Drawing.Size(MinWidth, MinHeight);
            Game.Window.AllowUserResizing = true;
            UpdateOrder = int.MinValue + 1;
            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (graphics.PreferredBackBufferWidth != width || graphics.PreferredBackBufferHeight != height)
            {
                SetDisplayMode(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            }

            /*var input = ILoadable.GetInstance<InputComponent>();

            if (input.IsHeld(XnaKeys.LeftAlt) && input.JustPressed(XnaKeys.Enter))
            {
                ToggleFullScreen();
            }*/
        }

        public void SetGraphicsDeviceManager(GraphicsDeviceManager graphics)
            => this.graphics = graphics;

        public void SetMinDisplayMode()
            => SetDisplayMode(MinWidth, MinHeight);

        public void SetDisplayMode(int width, int height)
        {
            this.width = Math.Max(width, MinWidth);
            this.height = Math.Max(height, MinHeight);
           
            graphics.PreferredBackBufferWidth = this.width;
            graphics.PreferredBackBufferHeight = this.height;
            graphics.ApplyChanges();

            OnResolutionChanged?.Invoke(this.width, this.height);
        }

        public List<Point> GetSupportedScreenResolutions()
        {
            var result = new List<Point>();
            var displayModes = Game.GraphicsDevice.Adapter.SupportedDisplayModes;

            foreach (var mode in displayModes)
            {
                if (mode.Width < MinWidth || mode.Height < MinHeight) continue;

                result.Add(new Point(mode.Width, mode.Height));
            }

            return result.OrderBy(o => o.X).ThenBy(o => o.Y).ToList();
        }
    }
}
