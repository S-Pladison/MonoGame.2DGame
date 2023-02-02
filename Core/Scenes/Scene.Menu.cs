using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pladi.Content;
using Pladi.Core.UI;
using Pladi.Core.UI.Elements;
using System.Collections.Generic;

namespace Pladi.Core.Scenes
{
    public class MenuScene : Scene
    {
        private GraphicalUI userInterface;

        // ...

        public override void OnActivate()
        {
            userInterface = new GraphicalUI();

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

            var panel = new PanelUIElement();
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
            text.OnMouseClick += (a, b) => { MessageBox.Show("1", "2", new List<string>() { "3" }); };*/

            userInterface.Append(panel);
        }

        public override void OnResolutionChanged(int width, int height)
        {
            userInterface.OnResolutionChanged(width, height);
        }

        public override void Update(GameTime gameTime)
        {
            userInterface.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Colors.MenuBackground);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            userInterface.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}