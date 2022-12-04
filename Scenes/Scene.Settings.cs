using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Enums;
using Pladi.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pladi.Scenes
{
    public class SettingsScene : Scene
    {
        private UserInterface userInterface;

        // ...

        public override void Init()
        {
            userInterface = new UserInterface();

            var button = new TextUIElement(FontAssets.DefaultMedium, "Fullscreen", Color.White);
            button.Position = new Vector2(10, 10);
            button.OnMouseClick += (evt, elem) => Main.ToggleFullScreen();
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            userInterface.AddElement(button);

            button = new TextUIElement(FontAssets.DefaultMedium, "Return", Color.White);
            button.Position = new Vector2(10, 40);
            button.OnMouseClick += (evt, elem) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Menu);
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            userInterface.AddElement(button);

            var screenResolutions = Main.GetSupportedScreenResolutions();

            for (int i = 0; i < screenResolutions.Count; i++)
            {
                var size = screenResolutions[i];

                button = new TextUIElement(FontAssets.DefaultMedium, $"{size.X}x{size.Y}", Color.White);
                button.Position = new Vector2(10, 70 + i * 30);
                button.OnMouseClick += (evt, elem) => Main.SetDisplayMode( size.X, size.Y);
                button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

                userInterface.AddElement(button);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1))
            {
                Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Menu);
            }

            userInterface.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(new Color(36, 34, 41));

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            userInterface.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}