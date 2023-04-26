using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pladi.Core.UI;
using Pladi.Core.UI.Elements;

namespace Pladi.Core.Scenes
{
    public class MenuScene : Scene
    {
        private GraphicalUI userInterface;

        // ...

        public override void OnActivate()
        {
            userInterface = new GraphicalUI();

            var panel = new PanelUIElement();
            panel.BackgroundColor = new Color(255, 255, 255, 255);
            panel.HorizontalAlign = 0.5f;
            panel.VerticalAlign = 0.5f;
            panel.Width.SetPercent(0.5f);
            panel.Height.SetPercent(0.5f);
            userInterface.Append(panel);

            var startGameButton = new TextUIElement("Начать")
            {
                HorizontalAlign = 0.5f,
                VerticalAlign = 0.5f
            };
            startGameButton.OnMouseOver += (_, e) =>
            {
                var text = (e as TextUIElement);
                text.FontScale = 1.1f;
                text.FontColor = Color.Gold;
            };
            startGameButton.OnMouseOut += (_, e) =>
            {
                var text = (e as TextUIElement);
                text.FontScale = 1.0f;
                text.FontColor = Color.White;
            };
            startGameButton.OnMouseClick += (_, _) => SceneComponent.SetActiveScene(SceneComponent.GameScenes.Game);
            panel.Append(startGameButton);

            var editorButton = new TextUIElement("Редактор")
            {
                HorizontalAlign = 1.0f,
                VerticalAlign = 1.0f
            };

            editorButton.Left.SetPixel(-20);
            editorButton.Top.SetPixel(-20);

            editorButton.OnMouseOver += (_, e) =>
            {
                var text = (e as TextUIElement);
                text.FontScale = 1.1f;
                text.FontColor = Color.Gold;
            };
            editorButton.OnMouseOut += (_, e) =>
            {
                var text = (e as TextUIElement);
                text.FontScale = 1.0f;
                text.FontColor = Color.White;
            };
            editorButton.OnMouseClick += (_, _) => SceneComponent.SetActiveScene(SceneComponent.GameScenes.Editor);
            panel.Append(editorButton);

            var inputField = new TextInputFieldUIElement();
            inputField.Left.SetPixel(10f);
            inputField.Top.SetPixel(10f);
            inputField.Width.SetPercent(1f);
            inputField.Height.SetPixel(50f);
            inputField.HintText = "...";
            panel.Append(inputField);

            /*var cTime = new TextUIElement(FontAssets.DefaultSmall, string.Empty, Color.White);
            cTime.OnPostUpdate += (e) => cTime.SetText(DateTime.Now.ToString() + "\nVersion: D.0.0.0.1");
            cTime.Left.SetValue(20f, 0f);
            cTime.Width.SetPercent(0f);
            panel.Append(cTime);*/

            /*var otherPanel = new PanelUIElement();
            otherPanel.Width.SetPixel(100f);
            otherPanel.Height.SetPixel(100f);
            otherPanel.HorizontalAlign = 0.5f;
            otherPanel.VerticalAlign = 0.5f;
            panel.Append(otherPanel);*/

            /*var menuPanel = new MenuPanelUIElement(FontAssets.DefaultMedium);
            menuPanel.OnResolutionChanged += (evt, elem) => elem.SetRectangle(20, 0, 200, evt.Height);
            menuPanel.SetRectangle(20, 0, 200, Main.ScreenSize.Y);
            menuPanel.SetBackgroundColor(new Color(0, 0, 0, 90));
            menuPanel.AddButtons(
                ("Start", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Game)),
                ("Editor", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Editor)),
                ("Settings", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Settings)),
                ("Exit", (_, _) => Main.ExitFromGame())
            );

            userInterface.Append(menuPanel);*/

            /*var menuPanel = new MenuPanelUIElement(FontAssets.DefaultMedium);
            menuPanel.OnResolutionChanged += (evt, elem) => elem.SetRectangle(20, 0, 200, evt.Height);
            menuPanel.SetRectangle(20, 0, 200, Main.ScreenSize.Y);
            menuPanel.SetBackgroundColor(new Color(0, 0, 0, 90));
            menuPanel.AddButtons(
                ("Start", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Game)),
                ("Editor", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Editor)),
                ("Settings", (_, _) => Main.SceneManager.SetActiveScene(SceneManager.GameScenes.Settings)),
                ("Exit", (_, _) => Main.ExitFromGame())
            );*/

            /*var panel = new PanelUIElement();
            panel.Left.SetValue(-50, 0.5f);
            panel.Top.SetValue(-50, 0.5f);
            panel.Width.SetValue(100, 0);
            panel.Height.SetValue(100, 0);

            var panel2 = new PanelUIElement();
            panel2.Left.SetValue(50, 0);
            panel2.Top.SetValue(50, 0);
            panel2.Width.SetValue(50, 0);
            panel2.Height.SetValue(50, 0);
            panel2.ClippingOutsideRectangle = true;
            panel2.SetBackgroundColor(Color.Red);
            panel.Append(panel2);

            var panel3 = new PanelUIElement();
            panel3.Width.SetValue(100, 0);
            panel3.Height.SetValue(25, 0);
            panel3.SetBackgroundColor(Color.Blue);
            panel3.OnMouseClick += (a, b) => { MessageBox.Show("1", "2", new List<string>() { "3" }); };
            panel2.Append(panel3);

            /*var text = new TextUIElement(FontAssets.DefaultMedium, "01.02.2023", Color.White);
            text.OnMouseClick += (a, b) => { MessageBox.Show("1", "2", new List<string>() { "3" }); };
            panel.Append(text);
            */
            //panel.OnMouseClick += (a, b) => { MessageBox.Show("1", "2", new List<string>() { "3" }); };

            /*var text = new TextUIElement(FontAssets.DefaultMedium, "01.02.2023", Color.White);
            text.Left.SetPercent(0.5f);
            text.Top.SetPercent(0.5f);
            text.OnMouseClick += (a, b) => { MessageBox.Show("1", "2", new List<string>() { "3" }); };

            userInterface.Append(panel);*/
        }

        public override void OnResolutionChanged(int width, int height)
        {
            userInterface.OnResolutionChanged(width, height);
        }

        public override void Update()
        {
            userInterface.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Colors.MenuBackground);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            userInterface.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}