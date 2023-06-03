using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Core.Input;
using Pladi.Core.UI;

namespace Pladi.Core.Scenes
{
    public class SettingsScene : Scene
    {
        private GraphicalUI userInterface;

        // ...

        /*public override void Init()
        {
            var button = new TextUIElement(FontAssets.DefaultMedium, "Fullscreen", Color.White);
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
            }
        }*/

        public override void OnActivate()
        {
            userInterface = new GraphicalUI();

            /*var menuPanel = new MenuPanelUIElement(FontAssets.DefaultMedium);
            menuPanel.OnResolutionChanged += (evt, elem) => elem.SetRectangle(20, 0, 200, evt.Height);
            menuPanel.SetRectangle(20, 0, 200, Main.ScreenSize.Y);
            menuPanel.SetBackgroundColor(new Color(0, 0, 0, 90));
            menuPanel.AddButtons(
                ("Reset to Defaults", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Game)),
                ("Apply", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Game)),
                ("Return", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Menu))
            );
            userInterface.Append(menuPanel);

            var settingsPanel = new PanelUIElement();
            settingsPanel.OnResolutionChanged += (evt, elem) => elem.SetRectangle(240, 0, evt.Width - 260, evt.Height);
            settingsPanel.SetBackgroundColor(new Color(0, 0, 0, 130));
            settingsPanel.SetRectangle(240, 0, Main.ScreenSize.X - 260, Main.ScreenSize.Y);
            userInterface.Append(settingsPanel);

            var line = new PanelUIElement();
            line.OnResolutionChanged += (evt, elem) => elem.SetRectangle(0, 0, evt.Width - 260, 40);
            line.SetBackgroundColor(new Color(0, 0, 0, 40));
            line.SetRectangle(0, 0, Main.ScreenSize.X - 260, 40);
            settingsPanel.Append(line);

            var text = new TextUIElement(FontAssets.DefaultMedium, "Video Settings", Color.White);
            text.SetPosition(10, 10);
            settingsPanel.Append(text);

            text = new TextUIElement(FontAssets.DefaultMedium, "Fullscreen", Color.Gray, 0.8f);
            text.SetPosition(10, 50);
            settingsPanel.Append(text);

            text = new TextUIElement(FontAssets.DefaultMedium, "Resolution", Color.Gray, 0.8f);
            text.SetPosition(10, 80);
            settingsPanel.Append(text);

            line = new PanelUIElement();
            line.OnResolutionChanged += (evt, elem) => elem.SetRectangle(0, 110, evt.Width - 260, 40);
            line.SetBackgroundColor(new Color(0, 0, 0, 40));
            line.SetRectangle(0, 110, Main.ScreenSize.X - 260, 40);
            settingsPanel.Append(line);

            text = new TextUIElement(FontAssets.DefaultMedium, "Audio Settings", Color.White);
            text.SetPosition(10, 120);
            settingsPanel.Append(text);

            text = new TextUIElement(FontAssets.DefaultMedium, "Volume", Color.Gray, 0.8f);
            text.SetPosition(10, 160);
            settingsPanel.Append(text);*/
        }

        public override void OnResolutionChanged(int width, int height)
        {
            userInterface?.OnResolutionChanged(width, height);
        }

        public override void Update()
        {
            if (ILoadable.GetInstance<InputComponent>().JustPressed(Keys.Escape))
            {
                SceneComponent.SetActiveScene<MenuScene>();
            }

            userInterface.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(new Color(47, 54, 73));

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            userInterface.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}