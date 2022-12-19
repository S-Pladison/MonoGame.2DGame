using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Core.UI;
using Pladi.Core.UI.Elements;

namespace Pladi.Scenes
{
    public class SettingsScene : Scene
    {
        private GraphicalUI userInterface;

        // ...

        public override void Init()
        {
            /*var button = new TextUIElement(FontAssets.DefaultMedium, "Fullscreen", Color.White);
            button.Position = new Vector2(10, 10);
            button.OnMouseClick += (evt, elem) => Main.ToggleFullScreen();
            button.OnMouseOver += (evt, elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            userInterface.Append(button);

            button = new TextUIElement(FontAssets.DefaultMedium, "Return", Color.White);
            button.Position = new Vector2(10, 40);
            button.OnMouseClick += (evt, elem) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Menu);
            button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

            userInterface.Append(button);

            var screenResolutions = Main.GetSupportedScreenResolutions();

            for (int i = 0; i < screenResolutions.Count; i++)
            {
                var size = screenResolutions[i];

                button = new TextUIElement(FontAssets.DefaultMedium, $"{size.X}x{size.Y}", Color.White);
                button.Position = new Vector2(10, 70 + i * 30);
                button.OnMouseClick += (evt, elem) => Main.SetDisplayMode(size.X, size.Y);
                button.OnPostUpdate += (elem) => (elem as TextUIElement).SetColor(elem.IsMouseHovering ? Color.Gold : Color.White);

                userInterface.Append(button);
            }*/
        }

        public override void OnActivate()
        {
            userInterface = new GraphicalUI();

            var menuPanel = new MenuPanelUIElement(FontAssets.DefaultMedium);
            menuPanel.OnResolutionChanged += (evt, elem) => elem.SetRectangle(20, 0, 200, evt.Height);
            menuPanel.SetRectangle(20, 0, 200, Main.ScreenSize.Y);
            menuPanel.SetBackgroundColor(new Color(0, 0, 0, 90));
            menuPanel.AddButtons(
                ("Apply", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Game)),
                ("Return", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Menu))
            );
            userInterface.Append(menuPanel);

            var settingsPanel = new PanelUIElement(new Color(0, 0, 0, 130));
            settingsPanel.OnResolutionChanged += (evt, elem) => elem.SetRectangle(240, 0, evt.Width - 260, evt.Height);
            settingsPanel.SetRectangle(240, 0, Main.ScreenSize.X - 260, Main.ScreenSize.Y);
            userInterface.Append(settingsPanel);

            var text = new TextUIElement(FontAssets.DefaultMedium, "Video Settings", Color.White);
            text.SetPosition(10, 10);
            settingsPanel.Append(text);

            text = new TextUIElement(FontAssets.DefaultMedium, "Fullscreen", new Color(210, 210, 210), 0.8f);
            text.SetPosition(10, 50);
            settingsPanel.Append(text);

            text = new TextUIElement(FontAssets.DefaultMedium, "Resolution", new Color(210, 210, 210), 0.8f);
            text.SetPosition(10, 80);
            settingsPanel.Append(text);
        }

        public override void OnResolutionChanged(int width, int height)
        {
            userInterface?.OnResolutionChanged(width, height);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.InputManager.JustPressed(Keys.Escape))
            {
                Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Menu);
            }

            userInterface.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            userInterface.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}